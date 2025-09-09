using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

public class SocketIOManager : MonoBehaviour
{
  [SerializeField] private SlotBehaviour slotManager;
  [SerializeField] private UIManager uiManager;
  internal Root initialData = null;
  internal Root resultData = null;
  internal Payload jackpootPayload = null;
  internal Player playerdata = null;
  internal bool isResultdone = false;
  // protected string nameSpace="game"; //BackendChanges
  protected string nameSpace = "playground"; //BackendChanges
  private Socket gameSocket; //BackendChanges
  private SocketManager manager;
  protected string SocketURI = null;
  protected string TestSocketURI = "http://localhost:5000/";
  //protected string TestSocketURI = "http://localhost:5002/";
  [SerializeField] internal JSFunctCalls JSManager;
  [SerializeField] private string testToken;
  protected string gameID = "SL-RB";
  //protected string gameID = "";
  internal bool isLoaded = false;
  internal bool SetInit = false;
  private const int maxReconnectionAttempts = 6;
  private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(10);

  private bool isConnected = false; //Back2 Start
  private bool hasEverConnected = false;
  private const int MaxReconnectAttempts = 5;
  private const float ReconnectDelaySeconds = 2f;

  private float lastPongTime = 0f;
  private float pingInterval = 2f;
  private float pongTimeout = 3f;
  private bool waitingForPong = false;
  private int missedPongs = 0;
  private const int MaxMissedPongs = 5;
  private Coroutine PingRoutine; //Back2 end

  [SerializeField] private GameObject RaycastBlocker;

  private void Awake()
  {
    //Debug.unityLogger.logEnabled = false;
    isLoaded = false;
    SetInit = false;
  }

  private void Start()
  {
    OpenSocket();
  }

  void ReceiveAuthToken(string jsonData)
  {
    Debug.Log("Received data: " + jsonData);
    var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
    SocketURI = data.socketURL;
    myAuth = data.cookie;
    nameSpace = data.nameSpace;
  }


  string myAuth = null;

  private void OpenSocket()
  {
    //Create and setup SocketOptions
    SocketOptions options = new SocketOptions(); //Back2 Start
    options.AutoConnect = false;
    options.Reconnection = false;
    options.Timeout = TimeSpan.FromSeconds(3); //Back2 endtionDelay;
    options.Reconnection = true;
    options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket; //BackendChanges

#if UNITY_WEBGL && !UNITY_EDITOR
            JSManager.SendCustomMessage("authToken");
            StartCoroutine(WaitForAuthToken(options));
#else
    Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
    {
      return new
      {
        token = testToken,
      };
    };
    options.Auth = authFunction;
    // Proceed with connecting to the server
    SetupSocketManager(options);
#endif

    // #if UNITY_WEBGL && !UNITY_EDITOR
    //     string url = Application.absoluteURL;
    //     Debug.Log("Unity URL : " + url);
    //     ExtractUrlAndToken(url);

    //     Func<SocketManager, Socket, object> webAuthFunction = (manager, socket) =>
    //     {
    //       return new
    //       {
    //         token = testToken,
    //       };
    //     };
    //     options.Auth = webAuthFunction;
    // #else
    //     Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
    //     {
    //       return new
    //       {
    //         token = testToken,
    //       };
    //     };
    //     options.Auth = authFunction;
    // #endif
    //     // Proceed with connecting to the server
    //     SetupSocketManager(options);
  }


  private IEnumerator WaitForAuthToken(SocketOptions options)
  {
    // Wait until myAuth is not null
    while (myAuth == null)
    {
      Debug.Log("My Auth is null");
      yield return null;
    }
    while (SocketURI == null)
    {
      Debug.Log("My Socket is null");
      yield return null;
    }
    Debug.Log("My Auth is not null");

    // Once myAuth is set, configure the authFunction
    Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
          return new
          {
            token = myAuth
          };
        };
    options.Auth = authFunction;

    Debug.Log("Auth function configured with token: " + myAuth);

    // Proceed with connecting to the server
    SetupSocketManager(options);
    yield return null;
  }

  private void SetupSocketManager(SocketOptions options)
  {
    // Create and setup SocketManager
#if UNITY_EDITOR
    this.manager = new SocketManager(new Uri(TestSocketURI), options);
#else
    this.manager = new SocketManager(new Uri(SocketURI), options);
#endif

    if (string.IsNullOrEmpty(nameSpace))
    {
      gameSocket = this.manager.Socket;
    }
    else
    {
      Debug.Log("Namespace used :" + nameSpace);
      gameSocket = this.manager.GetSocket("/" + nameSpace);
    }
    // Set subscriptions
    gameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
    gameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected);
    gameSocket.On<Error>(SocketIOEventTypes.Error, OnError); ;
    gameSocket.On<string>("game:init", OnListenEvent);
    gameSocket.On<string>("result", OnResult);
    //  gameSocket.On<string>("bonus:result", OnResult);
    gameSocket.On<bool>("socketState", OnSocketState);
    gameSocket.On<string>("internalError", OnSocketError);
    gameSocket.On<string>("alert", OnSocketAlert);
    gameSocket.On<string>("AnotherDevice", OnSocketOtherDevice); //BackendChanges Finish
    gameSocket.On<string>("appBackground", MuteAudio); //BackendChanges Finish
    gameSocket.On<string>("pong", OnPongReceived);
    // Start connecting to the server
    manager.Open();
  }

  void MuteAudio(string data)
  {
    Debug.Log("MuteAudio Event called");
    slotManager.audioController.CheckFocusFunction(false, false);
  }

  // Connected event handler implementation
  void OnConnected(ConnectResponse resp)
  {
    Debug.Log("✅ Connected to server.");

    if (hasEverConnected)
    {
      uiManager.CheckAndClosePopups();
    }

    isConnected = true;
    hasEverConnected = true;
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    SendPing();
  }

  private void OnDisconnected() //Back2 Start
  {
    Debug.LogWarning("⚠️ Disconnected from server.");
    isConnected = false;
    uiManager.DisconnectionPopup();
    ResetPingRoutine();
  } //Back2 end

  private void OnPongReceived(string data) //Back2 Start
  {
    Debug.Log("✅ Received pong from server.");
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
    Debug.Log($"📦 Pong payload: {data}");
  } //Back2 end

  private void OnError(Error err)
  {
    Debug.LogError("Socket Error Message: " + err);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("error");
#endif
  }

  void OnResult(string data)
  {
    ParseResponse(data);
  }


  private void OnListenEvent(string data)
  {
    ParseResponse(data);
  }

  private void OnSocketState(bool state)
  {
    Debug.Log("my state is " + state);
  }

  private void OnSocketError(string data)
  {
    Debug.Log("Received error with data: " + data);
  }

  private void OnSocketAlert(string data)
  {
    Debug.Log("Received alert with data: " + data);
  }

  private void OnSocketOtherDevice(string data)
  {
    Debug.Log("Received Device Error with data: " + data);
    uiManager.ADfunction();
  }
  public void ExtractUrlAndToken(string fullUrl)
  {
    Uri uri = new Uri(fullUrl);
    string query = uri.Query; // Gets the query part, e.g., "?url=http://localhost:5000&token=e5ffa84216be4972a85fff1d266d36d0"

    Dictionary<string, string> queryParams = new Dictionary<string, string>();
    string[] pairs = query.TrimStart('?').Split('&');

    foreach (string pair in pairs)
    {
      string[] kv = pair.Split('=');
      if (kv.Length == 2)
      {
        queryParams[kv[0]] = Uri.UnescapeDataString(kv[1]);
      }
    }

    if (queryParams.TryGetValue("url", out string extractedUrl) &&
        queryParams.TryGetValue("token", out string token))
    {
      Debug.Log("Extracted URL: " + extractedUrl);
      Debug.Log("Extracted Token: " + token);
      testToken = token;
      SocketURI = extractedUrl;
    }
    else
    {
      Debug.LogError("URL or token not found in query parameters.");
    }
  }

  private void SendPing() //Back2 Start
  {
    ResetPingRoutine();
    PingRoutine = StartCoroutine(PingCheck());
  }

  void ResetPingRoutine()
  {
    if (PingRoutine != null)
    {
      StopCoroutine(PingRoutine);
    }
    PingRoutine = null;
  }

  private IEnumerator PingCheck()
  {
    while (true)
    {
      Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

      if (missedPongs == 0)
      {
        uiManager.CheckAndClosePopups();
      }

      // If waiting for pong, and timeout passed
      if (waitingForPong)
      {
        if (missedPongs == 2)
        {
          uiManager.ReconnectionPopup();
        }
        missedPongs++;
        Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

        if (missedPongs >= MaxMissedPongs)
        {
          Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
          isConnected = false;
          uiManager.DisconnectionPopup();
          yield break;
        }
      }

      // Send next ping
      waitingForPong = true;
      lastPongTime = Time.time;
      Debug.Log("📤 Sending ping...");
      SendDataWithNamespace("ping");
      yield return new WaitForSeconds(pingInterval);
    }
  } //Back2 end
  private void AliveRequest()
  {
    SendDataWithNamespace("YES I AM ALIVE");
  }
  void CloseGame()
  {
    Debug.Log("Unity: Closing Game");
    StartCoroutine(CloseSocket());
  }

  private void SendDataWithNamespace(string eventName, string json = null)
  {
    // Send the message
    if (gameSocket != null && gameSocket.IsOpen) //BackendChanges
    {
      if (json != null)
      {
        gameSocket.Emit(eventName, json);
        Debug.Log("JSON data sent: " + json);
      }
      else
      {
        gameSocket.Emit(eventName);
      }
    }
    else
    {
      Debug.LogWarning("Socket is not connected.");
    }
  }

  internal IEnumerator CloseSocket() //Back2 Start
  {
    RaycastBlocker.SetActive(true);
    ResetPingRoutine();

    Debug.Log("Closing Socket");

    manager?.Close();
    manager = null;

    Debug.Log("Waiting for socket to close");

    yield return new WaitForSeconds(0.5f);

    Debug.Log("Socket Closed");

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
  } //Back2 end



  private void ParseResponse(string jsonObject)
  {
    Debug.Log(jsonObject);
    Root myData = JsonConvert.DeserializeObject<Root>(jsonObject);

    string id = myData.id;

    switch (id)
    {
      case "InitData":
        {
          initialData = myData;
          if (!SetInit)
          {
            Debug.Log(jsonObject);
            PopulateSlotSocket(); //, LinesString
            SetInit = true;
          }
          else
          {
            RefreshUI();
          }
          break;
        }
      case "ResultData":
        {
          Debug.Log(jsonObject);
          resultData = myData;
          playerdata = myData.player;
          isResultdone = true;
          break;
        }

      case "BonusResult":
        {
          Debug.Log("Bonus Result : " + jsonObject);
          jackpootPayload = myData.payload;
          playerdata = myData.player;
          isResultdone = true;
          break;
        }


      case "ExitUser":
        {
          if (gameSocket != null) //BackendChanges
          {
            Debug.Log("Dispose my Socket");
            this.manager.Close();
          }
#if UNITY_WEBGL && !UNITY_EDITOR
          JSManager.SendCustomMessage("onExit");
#endif
          break;
        }
    }
  }

  private void RefreshUI()
  {
    uiManager.InitialiseUIData(initialData.uiData.paylines);
  }

  private void PopulateSlotSocket()  //, List<string> LineIds
  {
    slotManager.shuffleInitialMatrix();

    slotManager.SetInitialUI();

    isLoaded = true;
    RaycastBlocker.SetActive(false);
#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("OnEnter");
#endif
  }

  internal void AccumulateResult(int currBet)
  {
    isResultdone = false;
    // MessageData message = new MessageData();
    //  message.currentBet = currBet;

    // Serialize message data to JSON
    MessageData message = new MessageData();
    message.type = "SPIN";
    Debug.Log($"current bet is " + currBet);
    message.payload = new Data();
    message.payload.betIndex = currBet;
    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }
  internal void SendSelectedWildData(int index)
  {
    // WildData freeSpinoption = new WildData();
    // freeSpinoption.data = new Data();
    // freeSpinoption.data.option = index;
    // freeSpinoption.id = "FREESPINOPTION";
    // Data data = new Data();
    // data.option = index;
    // data.type = "FREESPINOPTION";

    MessageData message = new MessageData();
    message.type = "FREESPIN";
    message.payload = new Data();
    message.payload.Event = "FREESPINOPTION";
    message.payload.option = index;
    // Serialize message data to JSON
    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }

  internal void SendSelectedFlipCoin(List<int> coinPosition)
  {
    // MiniGameData miniGamedata = new MiniGameData();
    // miniGamedata.data = new Data();
    // miniGamedata.data.index = coinPosition;
    // miniGamedata.id = "BONUSCOINFLIP";

    // Data data = new Data();
    // data.index = coinPosition;
    // data.type = "BONUSCOINFLIP";

    MessageData message = new MessageData();
    message.type = "BONUS";
    message.payload = new Data();
    message.payload.Event = "BONUSCOINFLIP";
    message.payload.index = coinPosition;

    // Serialize message data to JSON
    string json = JsonUtility.ToJson(message);
    Debug.Log("@@@@ Free Mini Sent DATA :" + json);
    SendDataWithNamespace("request", json);
  }

  private List<string> RemoveQuotes(List<string> stringList)
  {
    for (int i = 0; i < stringList.Count; i++)
    {
      stringList[i] = stringList[i].Replace("\"", ""); // Remove inverted commas
    }
    return stringList;
  }

  private List<string> ConvertListListIntToListString(List<List<int>> listOfLists)
  {
    List<string> resultList = new List<string>();

    foreach (List<int> innerList in listOfLists)
    {
      // Convert each integer in the inner list to string
      List<string> stringList = new List<string>();
      foreach (int number in innerList)
      {
        stringList.Add(number.ToString());
      }

      // Join the string representation of integers with ","
      string joinedString = string.Join(",", stringList.ToArray()).Trim();
      resultList.Add(joinedString);
    }

    return resultList;
  }

  private List<string> ConvertListOfListsToStrings(List<List<string>> inputList)
  {
    List<string> outputList = new List<string>();

    foreach (List<string> row in inputList)
    {
      string concatenatedString = string.Join(",", row);
      outputList.Add(concatenatedString);
    }

    return outputList;
  }

  private List<string> TransformAndRemoveRecurring(List<List<string>> originalList)
  {
    // Flattened list
    List<string> flattenedList = new List<string>();
    foreach (List<string> sublist in originalList)
    {
      flattenedList.AddRange(sublist);
    }

    // Remove recurring elements
    HashSet<string> uniqueElements = new HashSet<string>(flattenedList);

    // Transformed list
    List<string> transformedList = new List<string>();
    foreach (string element in uniqueElements)
    {
      transformedList.Add(element.ToString().Replace(",", ""));
    }

    return transformedList;
  }
}

[Serializable]
public class BetData
{
  public double currentBet;
  // public double currentLines;
  public double spins;
}

[Serializable]
public class AuthData
{
  public string GameID;
}


[Serializable]
public class ExitData
{
  public string id;
}

[Serializable]
public class InitData
{
  public AuthData Data;
  public string id;
}

[Serializable]
public class AbtLogo
{
  public string logoSprite { get; set; }
  public string link { get; set; }
}

[Serializable]
public class GameData
{
  public List<List<string>> Reel { get; set; }
  public List<List<int>> Lines { get; set; }
  public bool canSwitchLines { get; set; }
  public List<int> LinesCount { get; set; }
  public List<int> autoSpin { get; set; }
  public List<List<string>> resultSymbols { get; set; }
  public List<int> linesToEmit { get; set; }
  public List<SymbolsToEmit> symbolsToEmit { get; set; }
  public double WinAmout { get; set; }
  public FreeSpins freeSpins { get; set; }
  public List<string> FinalsymbolsToEmit { get; set; }
  public List<string> FinalResultReel { get; set; }
  public double jackpot { get; set; }
  public bool isBonus { get; set; }
  public double BonusStopIndex { get; set; }

  public List<int> jackpotMultipliers { get; set; }

  public List<FreespinOption> freespinOptions { get; set; }

  public Bonus bonus { get; set; }
  public List<int> goldenReels { get; set; }
  public List<int> selectedIndex { get; set; }
  public string jackpotType { get; set; }
  public bool isOver { get; set; }
  public double winAmount { get; set; }


  // updated v2

  public List<double> bets { get; set; }



}

[Serializable]
public class FreeSpins
{
  public int count { get; set; }
  public bool isTriggered { get; set; }
}
[Serializable]
public class Bonus
{
  public bool isTriggered { get; set; }
}

// [Serializable]
// public class Message
// {
//   public GameData GameData { get; set; }
//   public UIData UIData { get; set; }
//   public PlayerData PlayerData { get; set; }
//   public List<string> BonusData { get; set; }
// }

[Serializable]
public class SymbolsToEmit
{
  public List<string> combination { get; set; }
  public float payout { get; set; }
}

[Serializable]
public class Root
{
  public string id { get; set; }
  public GameData gameData { get; set; }
  public Features features { get; set; }
  public UiData uiData { get; set; }
  public Player player { get; set; }

  public bool success { get; set; }
  public List<List<string>> matrix { get; set; }
  public Payload payload { get; set; }



}

[Serializable]
public class UiData
{
  public Paylines paylines { get; set; }
  public List<string> spclSymbolTxt { get; set; }
  public AbtLogo AbtLogo { get; set; }
  public string ToULink { get; set; }
  public string PopLink { get; set; }
}

[Serializable]
public class Paylines
{
  public List<Symbol> symbols { get; set; }
}

[Serializable]
public class Symbol
{
  // new updated 

  public int id { get; set; }
  public string name { get; set; }
  public List<int> multiplier { get; set; }
  public string description { get; set; }
}
// [Serializable]
// public class PlayerData
// {
//   public double Balance { get; set; }
//   public double haveWon { get; set; }
//   public double currentWining { get; set; }
// }
[Serializable]
public class AuthTokenData
{
  public string cookie;
  public string socketURL;
  public string nameSpace; //BackendChanges
}

[Serializable]
public class MessageData
{
  // public int option;
  // public List<int> index;
  public string type;
  public Data payload;

}
[Serializable]
public class Data
{
  public int betIndex;
  public string Event;
  public List<int> index;
  public int option;

}

// [Serializable]
// public class MiniGameData
// {
//   public Data data;
//   public string id;
// }
public class FreespinOption
{
  public int count { get; set; }
  public List<int> multiplier { get; set; }
}
[Serializable]
public class Features
{
  public List<FreespinOption> freespinOptions { get; set; }
  public List<int> jackpotMultipliers { get; set; }

  public List<int> goldenReels { get; set; }
  public FreeSpin freeSpin { get; set; }
  public Bonus bonus { get; set; }
}

[Serializable]
public class Player
{
  public double balance { get; set; }
  public double haveWon { get; set; }
  public double currentWining { get; set; }
}

[Serializable]
public class Payload
{
  public double winAmount { get; set; }
  public List<Win> wins { get; set; }


  //for Bonus Game

  public List<int> selectedIndex { get; set; }
  public string jackpotType { get; set; }
  public bool isOver { get; set; }
}

[Serializable]
public class Win
{
  public int symbolId { get; set; }
  public List<List<int>> positions { get; set; }
  public double payout { get; set; }
}

[Serializable]
public class FreeSpin
{
  public bool isTriggered { get; set; }
  public int count { get; set; }
}



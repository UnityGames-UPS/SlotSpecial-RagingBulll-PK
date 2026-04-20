using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    [SerializeField] private JSFunctCalls jsFunctCalls;

    [Header("Popus UI")]
    [SerializeField] private GameObject MainPopup_Object;

    [Header("Win Popup")]
    [SerializeField] private Image Win_Image;

    [SerializeField] public RectTransform Win_Image_RT;

    [SerializeField] private GameObject WinPopup_Object;
    [SerializeField] private RectTransform WinBgAnimation;
    [SerializeField] private Sprite BigWin_Sprite, HugeWin_Sprite, MegaWin_Sprite, Jackpot_Sprite;
    [SerializeField] private ImageAnimation JackpotImageAnimation;
    [SerializeField] private Sprite Major_Sprite, Minor_Sprite, Grand_Sprite;

    [SerializeField] private Button SkipWinAnimation;
    [SerializeField] private TMP_Text Win_Text;
    private Tween ImageRotationTween;
    private Tween ImageScaleTween;
    private Tween AnimationScaleTween;
    private Tween DelayTween;


    [Header("Disconnection Popup")]
    [SerializeField] private Button CloseDisconnect_Button;
    [SerializeField] private GameObject DisconnectPopup_Object;
    [SerializeField] private GameObject ReconnectPopup_Object;

    [Header("AnotherDevice Popup")]
    [SerializeField] private GameObject ADPopup_Object;

    [Header("LowBalance Popup")]
    [SerializeField] private Button LBExit_Button;
    [SerializeField] private GameObject LBPopup_Object;

    [Header("Audio Objects")]
    [SerializeField] private GameObject Settings_Object;
    [SerializeField] private Button SettingsQuit_Button;
    [SerializeField] private Button Sound_Button;
    [SerializeField] private Button Music_Button;
    [SerializeField] private RectTransform SoundToggle_RT;
    [SerializeField] private RectTransform MusicToggle_RT;

    [Header("Paytable Objects")]
    [SerializeField] internal GameObject PaytableMenuObject;
    [SerializeField] private Button Paytable_Button;
    [SerializeField] private Button PaytableClose_Button;
    [SerializeField] private Button PaytableLeft_Button;
    [SerializeField] private Button PaytableRight_Button;
    [SerializeField] private TMP_Text FreeSpin_Text;
    [SerializeField] private TMP_Text Jackpot_Text;
    [SerializeField] private TMP_Text Wild_Text;
    [SerializeField] private TMP_Text MiniGameDis_Text;
    [SerializeField] private TMP_Text MinorWin_Text;
    [SerializeField] private TMP_Text MajorWin_Text;
    [SerializeField] private TMP_Text GrandWin_Text;


    [SerializeField] private TMP_Text GrayWild_MultiplierText;
    [SerializeField] private TMP_Text RedWild_MultiplierText;
    [SerializeField] private TMP_Text OrangeWild_MultiplierText;
    [SerializeField] private TMP_Text BlueWild_MultiplierText;
    [SerializeField] private TMP_Text YellowWild_MultiplierText;
    [SerializeField] private TMP_Text PurpleWild_MultiplierText;


    [SerializeField] private List<GameObject> GameRulesPages = new();
    private int PageIndex;

    [Header("Game Quit Objects")]
    [SerializeField] private Button Quit_Button;
    [SerializeField] private Button QuitYes_Button;
    [SerializeField] private Button QuitNo_Button;
    [SerializeField] private GameObject QuitMenuObject;

    [Header("Menu Objects")]
    [SerializeField] private Button Menu_Button;
    [SerializeField] private Button Info_Button;
    [SerializeField] private Button Settings_Button;
    [SerializeField] private Button RaycastLayerButton;
    [SerializeField] private RectTransform Info_BttnTransform;
    [SerializeField] private RectTransform Settings_BttnTransform;

    [Header("Paytable Slot Text")]
    [SerializeField] private List<TMP_Text> SymbolsText = new();

    [SerializeField] private AudioController audioController;

    [SerializeField] private SlotBehaviour slotManager;

    [SerializeField] private SocketIOManager socketManager;

    [Header(" BG Theme Images")]
    public Sprite[] BG_ThemeSprites;
    public Sprite[] Reels_BGSprites;

    public Image Reels_BgImage;
    public Image Bg_ThemeImage;
    private Tween WinPopupTextTween;
    private bool isMusic = true;
    private bool isSound = true;
    private bool isExit = false;
    private bool isMenu = false;

    private void Awake()
    {
        // Make sure jsFunctCalls is assigned before using it
        if (jsFunctCalls != null)
        {
            jsFunctCalls.RegisterVisibilityListener(gameObject.name);
        }
        else
        {
            Debug.LogWarning("jsFunctCalls reference is null in Awake()");
        }
    }
    private void Start()
    {
        if (SkipWinAnimation) SkipWinAnimation.onClick.RemoveAllListeners();
        if (SkipWinAnimation) SkipWinAnimation.onClick.AddListener(SkipWin);

        if (RaycastLayerButton) RaycastLayerButton.onClick.RemoveAllListeners();
        if (RaycastLayerButton) RaycastLayerButton.onClick.AddListener(() => CanCloseMenu());

        if (LBExit_Button) LBExit_Button.onClick.RemoveAllListeners();
        if (LBExit_Button) LBExit_Button.onClick.AddListener(delegate { ClosePopup(LBPopup_Object); });

        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.RemoveAllListeners();
        if (CloseDisconnect_Button) CloseDisconnect_Button.onClick.AddListener(delegate { CallOnExitFunction(); }); //BackendChanges

        if (Sound_Button) Sound_Button.onClick.RemoveAllListeners();
        if (Sound_Button) Sound_Button.onClick.AddListener(delegate
        {
            Debug.Log("Here");
            if (isSound)
            {
                SoundOnOFF(false);
            }
            else
            {
                SoundOnOFF(true);
            }
        });

        if (Music_Button) Music_Button.onClick.RemoveAllListeners();
        if (Music_Button) Music_Button.onClick.AddListener(delegate
        {

            if (isMusic)
            {
                MusicONOFF(false);
            }
            else
            {
                MusicONOFF(true);
            }
        });

        if (Quit_Button) Quit_Button.onClick.RemoveAllListeners();
        if (Quit_Button) Quit_Button.onClick.AddListener(OpenQuitPanel);

        if (QuitNo_Button) QuitNo_Button.onClick.RemoveAllListeners();
        if (QuitNo_Button) QuitNo_Button.onClick.AddListener(delegate { ClosePopup(QuitMenuObject); });

        if (QuitYes_Button) QuitYes_Button.onClick.RemoveAllListeners();
        if (QuitYes_Button) QuitYes_Button.onClick.AddListener(CallOnExitFunction);

        if (Paytable_Button) Paytable_Button.onClick.RemoveAllListeners();
        if (Paytable_Button) Paytable_Button.onClick.AddListener(OpenPaytablePanel);

        if (PaytableClose_Button) PaytableClose_Button.onClick.RemoveAllListeners();
        if (PaytableClose_Button) PaytableClose_Button.onClick.AddListener(delegate { ClosePopup(PaytableMenuObject); });

        if (Menu_Button) Menu_Button.onClick.RemoveAllListeners();
        if (Menu_Button) Menu_Button.onClick.AddListener(delegate
        {
            if (!isMenu)
            {
                OpenCloseMenu(true);
            }
            else
            {
                OpenCloseMenu(false);
            }
        });

        if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
        if (Settings_Button) Settings_Button.onClick.AddListener(OpenSettingsPanel);

        if (SettingsQuit_Button) SettingsQuit_Button.onClick.RemoveAllListeners();
        if (SettingsQuit_Button) SettingsQuit_Button.onClick.AddListener(delegate { ClosePopup(Settings_Object); });

        if (PaytableLeft_Button) PaytableLeft_Button.onClick.RemoveAllListeners();
        if (PaytableLeft_Button) PaytableLeft_Button.onClick.AddListener(() => ChangePage(false));

        if (PaytableRight_Button) PaytableRight_Button.onClick.RemoveAllListeners();
        if (PaytableRight_Button) PaytableRight_Button.onClick.AddListener(() => ChangePage(true));
    }

    internal void CanCloseMenu()
    {
        if (isMenu)
        {
            OpenCloseMenu(false);
        }
    }

    private void ChangePage(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();

        //if (IncDec)
        // {
        //     if(PageIndex < GameRulesPages.Count - 1)
        //     {
        //         PageIndex++;
        //     }
        //     if(PageIndex == GameRulesPages.Count - 1)
        //     {
        //         if (PaytableRight_Button) PaytableRight_Button.interactable = true;
        //     }
        //     if(PageIndex > 0)
        //     {
        //         if(PaytableLeft_Button) PaytableLeft_Button.interactable = true;
        //     }
        // }
        // else
        // {
        //     if(PageIndex > 0)
        //     {
        //         PageIndex--;
        //     }
        //     if(PageIndex == 0)
        //     {
        //         if (PaytableLeft_Button) PaytableLeft_Button.interactable = true;
        //     }
        //     if(PageIndex < GameRulesPages.Count - 1)
        //     {
        //         if (PaytableRight_Button) PaytableRight_Button.interactable = true;
        //     }
        // }
        if (IncDec)
        {
            PageIndex = (PageIndex + 1) % GameRulesPages.Count;
        }
        else
        {
            PageIndex = (PageIndex - 1 + GameRulesPages.Count) % GameRulesPages.Count;
        }

        foreach (GameObject g in GameRulesPages)
        {
            g.SetActive(false);
        }
        if (GameRulesPages[PageIndex]) GameRulesPages[PageIndex].SetActive(true);

        if (PaytableLeft_Button) PaytableLeft_Button.interactable = true;
        if (PaytableRight_Button) PaytableRight_Button.interactable = true;
    }

    private void OpenCloseMenu(bool toggle)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (toggle)
        {
            isMenu = true;
            if (Info_Button) Info_Button.gameObject.SetActive(true);
            if (Settings_Button) Settings_Button.gameObject.SetActive(true);

            DOTween.To(() => Info_BttnTransform.anchoredPosition, (val) => Info_BttnTransform.anchoredPosition = val, new Vector2(Info_BttnTransform.anchoredPosition.x - 125, Info_BttnTransform.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Info_BttnTransform);
            });

            DOTween.To(() => Settings_BttnTransform.anchoredPosition, (val) => Settings_BttnTransform.anchoredPosition = val, new Vector2(Settings_BttnTransform.anchoredPosition.x - 250, Settings_BttnTransform.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_BttnTransform);
            });
        }
        else
        {
            isMenu = false;
            DOTween.To(() => Info_BttnTransform.anchoredPosition, (val) => Info_BttnTransform.anchoredPosition = val, new Vector2(Info_BttnTransform.anchoredPosition.x + 125, Info_BttnTransform.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Info_BttnTransform);
            });

            DOTween.To(() => Settings_BttnTransform.anchoredPosition, (val) => Settings_BttnTransform.anchoredPosition = val, new Vector2(Settings_BttnTransform.anchoredPosition.x + 250, Settings_BttnTransform.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Settings_BttnTransform);
            });

            DOVirtual.DelayedCall(0.1f, () =>
            {
                if (Info_Button) Info_Button.gameObject.SetActive(false);
                if (Settings_Button) Settings_Button.gameObject.SetActive(false);
            });
        }
    }

    private void SoundOnOFF(bool state)
    {
        if (state)
        {
            isSound = true;
            audioController.ToggleMute(!state, "sound");
            DOTween.To(() => SoundToggle_RT.anchoredPosition, (val) => SoundToggle_RT.anchoredPosition = val, new Vector2(SoundToggle_RT.anchoredPosition.x + 105, SoundToggle_RT.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Info_BttnTransform);
            });
        }
        else
        {
            isSound = false;
            audioController.ToggleMute(!state, "sound");
            DOTween.To(() => SoundToggle_RT.anchoredPosition, (val) => SoundToggle_RT.anchoredPosition = val, new Vector2(SoundToggle_RT.anchoredPosition.x - 105, SoundToggle_RT.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Info_BttnTransform);
            });
        }
    }

    private void MusicONOFF(bool state)
    {
        if (state)
        {
            isMusic = true;
            audioController.ToggleMute(!state, "music");
            DOTween.To(() => MusicToggle_RT.anchoredPosition, (val) => MusicToggle_RT.anchoredPosition = val, new Vector2(MusicToggle_RT.anchoredPosition.x + 105, MusicToggle_RT.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Info_BttnTransform);
            });
        }
        else
        {
            isMusic = false;
            audioController.ToggleMute(!state, "music");
            DOTween.To(() => MusicToggle_RT.anchoredPosition, (val) => MusicToggle_RT.anchoredPosition = val, new Vector2(MusicToggle_RT.anchoredPosition.x - 105, MusicToggle_RT.anchoredPosition.y), 0.1f).OnUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(Info_BttnTransform);
            });
        }
    }

    private void OpenSettingsPanel()
    {
        if (audioController) audioController.PlayButtonAudio();
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (Settings_Object) Settings_Object.SetActive(true);
        CanCloseMenu();
    }

    private void OpenQuitPanel()
    {
        if (audioController) audioController.PlayButtonAudio();
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
        if (QuitMenuObject) QuitMenuObject.SetActive(true);
        CanCloseMenu();
    }

    private void OpenPaytablePanel()
    {
        if (audioController) audioController.PlayButtonAudio();

        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        PageIndex = 0;

        foreach (GameObject g in GameRulesPages)
        {
            g.SetActive(false);
        }

        GameRulesPages[0].SetActive(true);
        if (PaytableLeft_Button) PaytableLeft_Button.interactable = true;
        if (PaytableRight_Button) PaytableRight_Button.interactable = true;

        if (PaytableMenuObject) PaytableMenuObject.SetActive(true);

        CanCloseMenu();
    }

    internal void LowBalPopup()
    {
        CanCloseMenu();
        OpenPopup(LBPopup_Object);
    }

    internal void DisconnectionPopup()
    {
        if (!isExit)
        {
            CanCloseMenu();
            OpenPopup(DisconnectPopup_Object);
        }
    }

    internal void ReconnectionPopup()
    {
        OpenPopup(ReconnectPopup_Object);
    }

    internal void CheckAndClosePopups()
    {
        if (ReconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(ReconnectPopup_Object);
        }
        if (DisconnectPopup_Object.activeInHierarchy)
        {
            ClosePopup(DisconnectPopup_Object);
        }
    }

    internal void PopulateWin(int value, double winAmount)
    {
        ClosePopup(PaytableMenuObject);
        Debug.Log($" Populated Win :" + value);
        switch (value)
        {
            case 1:
                if (Win_Image) Win_Image.sprite = BigWin_Sprite;
                break;
            case 2:
                if (Win_Image) Win_Image.sprite = HugeWin_Sprite;
                break;
            case 3:
                if (Win_Image) Win_Image.sprite = MegaWin_Sprite;
                break;
            case 4:
                if (Win_Image) Win_Image.sprite = Jackpot_Sprite;
                //JackpotImageAnimation.StartAnimation();
                break;
            case 5:
                if (Win_Image) Win_Image.sprite = Minor_Sprite;
                break;
            case 6:
                if (Win_Image) Win_Image.sprite = Major_Sprite;
                break;
            case 7:
                if (Win_Image) Win_Image.sprite = Grand_Sprite;
                break;
        }

        StartPopupAnim(winAmount);
    }

    void SkipWin()
    {
        if (MainPopup_Object.activeInHierarchy)
        {
            if (ImageScaleTween != null)
            {
                ImageScaleTween.Kill();
                ImageScaleTween = null;
            }
            if (JackpotImageAnimation.currentAnimationState == ImageAnimation.ImageState.PLAYING)
            {
                JackpotImageAnimation.StopAnimation();
            }
            if (AnimationScaleTween != null)
            {
                AnimationScaleTween.Kill();
                AnimationScaleTween = null;
            }
            if (DelayTween != null)
            {
                DelayTween.Kill();
                DelayTween = null;
            }
            ClosePopup(WinPopup_Object);
            // Win_Image.rectTransform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(() => ClosePopup(WinPopup_Object));
            // WinBgAnimation.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(()=> ImageRotationTween.Kill());
            slotManager.CheckPopups = false;
        }
    }
    private void StartPopupAnim(double amount)
    {
        CloseAllPanels();
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        audioController.PlayWLAudio("bigwin");

        // Set initial scale to 0.2
        Win_Image_RT.localScale = Vector3.one * 0.2f;
        // WinBgAnimation.localScale = Vector3.one * 0.2f;

        // Create a DOTween sequence
        Sequence popupSequence = DOTween.Sequence();

        // Scale up to 1
        popupSequence.Join(Win_Image_RT.DOScale(1.2f, 0.5f).SetEase(Ease.OutCirc));
        // popupSequence.Join(WinBgAnimation.DOScale(1f, 2f).SetEase(Ease.OutCirc));

        // Scale down to 0.1
        popupSequence.Append(Win_Image_RT.DOScale(0.5f, 0.6f).SetEase(Ease.InCirc));
        // popupSequence.Join(WinBgAnimation.DOScale(0.5f, 2f).SetEase(Ease.InOutSine));

        // Scale back up to 1
        popupSequence.Append(Win_Image_RT.DOScale(1.2f, 0.8f).SetEase(Ease.OutCirc));
        popupSequence.Append(Win_Image_RT.DOScale(1f, 0.4f).SetEase(Ease.OutCirc));


        double initAmount = 0;
        WinPopupTextTween = DOTween.To(() => initAmount, (val) => initAmount = val, amount, 2f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString("F3");
        });

        Invoke("SkipWin", 2.8f);
    }

    internal void ADfunction()
    {
        OpenPopup(ADPopup_Object);
    }

    internal void InitialiseUIData(Paylines symbolsText)
    {
        PopulateSymbolsPayout(symbolsText);
    }

    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Count; i++)
        {
            string text = null;
            if (paylines.symbols[i].multiplier[0] != 0)
            {
                text += "5x - " + paylines.symbols[i].multiplier[0] + "x";
            }
            if (paylines.symbols[i].multiplier[1] != 0)
            {
                text += "\n4x - " + paylines.symbols[i].multiplier[1] + "x";
            }
            if (paylines.symbols[i].multiplier[2] != 0)
            {
                text += "\n3x - " + paylines.symbols[i].multiplier[2] + "x";
            }
            if (SymbolsText[i]) SymbolsText[i].text = text;
        }

        for (int i = 0; i < paylines.symbols.Count; i++)
        {
            if (paylines.symbols[i].name.ToUpper() == "FREESPIN")
            {
                if (FreeSpin_Text) FreeSpin_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].name.ToUpper() == "SCATTER")
            {
                if (Jackpot_Text) Jackpot_Text.text = paylines.symbols[i].description.ToString();
            }
            if (paylines.symbols[i].name.ToUpper() == "WILD")
            {
                if (Wild_Text) Wild_Text.text = paylines.symbols[i].description.ToString();
            }

            if (paylines.symbols[i].name.ToUpper() == "POT")
            {
                if (MiniGameDis_Text) MiniGameDis_Text.text = paylines.symbols[i].description.ToString();
            }

        }
        SetWildMultiplierData();
        SetJackPotData();
    }

    public void SetWildMultiplierData()
    {
        //Set Free Spins Data
        if (GrayWild_MultiplierText) GrayWild_MultiplierText.text = socketManager.initialData.features.freespinOptions[0].count.ToString() + " Free Spins " + "\n" + string.Join("x, ", socketManager.initialData.features.freespinOptions[0].multiplier) + "x Multiplier";
        if (RedWild_MultiplierText) RedWild_MultiplierText.text = socketManager.initialData.features.freespinOptions[1].count.ToString() + " Free Spins " + "\n" + string.Join("x, ", socketManager.initialData.features.freespinOptions[1].multiplier) + "x Multiplier";
        if (OrangeWild_MultiplierText) OrangeWild_MultiplierText.text = socketManager.initialData.features.freespinOptions[2].count.ToString() + " Free Spins " + "\n" + string.Join("x, ", socketManager.initialData.features.freespinOptions[2].multiplier) + "x Multiplier";
        if (PurpleWild_MultiplierText) PurpleWild_MultiplierText.text = socketManager.initialData.features.freespinOptions[3].count.ToString() + " Free Spins " + "\n" + string.Join("x, ", socketManager.initialData.features.freespinOptions[3].multiplier) + "x Multiplier";
        if (BlueWild_MultiplierText) BlueWild_MultiplierText.text = socketManager.initialData.features.freespinOptions[4].count.ToString() + " Free Spins " + "\n" + string.Join("x, ", socketManager.initialData.features.freespinOptions[4].multiplier) + "x Multiplier";
        if (YellowWild_MultiplierText) YellowWild_MultiplierText.text = socketManager.initialData.features.freespinOptions[5].count.ToString() + " Free Spins " + "\n" + string.Join("x, ", socketManager.initialData.features.freespinOptions[5].multiplier) + "x Multiplier";
    }

    public void SetJackPotData()
    {
        MinorWin_Text.text = socketManager.initialData.features.jackpotMultipliers[0].ToString() + " x Total Bet";
        MajorWin_Text.text = socketManager.initialData.features.jackpotMultipliers[1].ToString() + " x Total Bet";
        GrandWin_Text.text = socketManager.initialData.features.jackpotMultipliers[2].ToString() + " x Total Bet ";
    }

    private void CallOnExitFunction()
    {
        isExit = true;
        audioController.PlayButtonAudio();
        slotManager.CallCloseSocket();
    }

    private void OpenPopup(GameObject Popup)
    {
        CloseAllPanels();
        if (audioController) audioController.PlayButtonAudio();

        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    internal void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(false);
        if (!DisconnectPopup_Object.activeSelf)
        {
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
        }
        CloseAllPanels();
    }

    private void UrlButtons(string url)
    {
        Application.OpenURL(url);
    }

    private IEnumerator DownloadImage(string url)
    {
        // Create a UnityWebRequest object to download the image
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        // Wait for the download to complete
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Apply the sprite to the target image
            //AboutLogo_Image.sprite = sprite;
        }
        else
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
    }
    public void OnFocusChanged(string value)
    {
        bool focused = value == "1";
        if (focused)
        {
            audioController.ToggleMute(false, "music");
            audioController.ToggleMute(false, "sound");
        }
        else
        {
            audioController.ToggleMute(true, "music");
            audioController.ToggleMute(true, "sound");
        }
        //socketManager?.HandleFusChanocge(focused);
    }
    internal void CloseAllPanels()
    {
        // Close all popup objects
        if (WinPopup_Object && WinPopup_Object.activeSelf) WinPopup_Object.SetActive(false);
        //   if (DisconnectPopup_Object && DisconnectPopup_Object.activeSelf) DisconnectPopup_Object.SetActive(false);
        //  if (ReconnectPopup_Object && ReconnectPopup_Object.activeSelf) ReconnectPopup_Object.SetActive(false);
        // if (ADPopup_Object && ADPopup_Object.activeSelf) ADPopup_Object.SetActive(false);
        if (LBPopup_Object && LBPopup_Object.activeSelf) LBPopup_Object.SetActive(false);
        if (Settings_Object && Settings_Object.activeSelf) Settings_Object.SetActive(false);
        if (PaytableMenuObject && PaytableMenuObject.activeSelf) PaytableMenuObject.SetActive(false);
        if (QuitMenuObject && QuitMenuObject.activeSelf) QuitMenuObject.SetActive(false);

        // Close menu if open
        // if (isMenu)
        // {
        //     OpenCloseMenu(false);
        // }
    }
}

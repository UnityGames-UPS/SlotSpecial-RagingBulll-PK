using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
// using Unity.VisualScripting;
// using Best.HTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp;

public class SlotBehaviour : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] myImages;  //images taken initially
    [SerializeField] private Sprite[] BGThemeImages;
    [SerializeField] private Sprite[] SlotReelBGImages;
    [SerializeField] private Sprite[] WildTypesImages;

    [Header("Slot Images")]
    [SerializeField] private List<SlotImage> images;     //class to store total images
    [SerializeField] private List<SlotImage> Tempimages;     //class to store the result matrix
    [SerializeField] private List<BoxScript> TempBoxScripts;
    [SerializeField] private List<Sprite> Box_Sprites;

    [Header("Mini Game")]
    [SerializeField] private List<SlotImage> MiniGameSprites;

    [SerializeField] private Sprite[] MinorgameSprites;
    [SerializeField] private Sprite[] MajorgameSprites;
    [SerializeField] private Sprite[] GrandgameSprites;


    [Header("Slots Transforms")]
    [SerializeField] private Transform[] Slot_Transform;

    private Dictionary<int, string> y_string = new Dictionary<int, string>();

    [Header("Buttons")]
    [SerializeField] private Button SlotStart_Button;
    [SerializeField] private Button AutoSpin_Button;
    [SerializeField] private Button AutoSpinStop_Button;
    [SerializeField] private Button TotalBetPlus_Button;
    [SerializeField] private Button TotalBetMinus_Button;
    [SerializeField] private Button LineBetPlus_Button;
    [SerializeField] private Button LineBetMinus_Button;
    [SerializeField] private Button SkipWinAnimation_Button;
    [SerializeField] private Button BonusSkipWinAnimation_Button;

    [SerializeField] private Button SymbolsToEmitSkipWinAnimation_Button;
    [SerializeField] private Button Turbo_Button;
    [SerializeField] private Button StopSpin_Button;
    [Header("Animated Sprites")]
    [SerializeField] private Sprite[] Bonus_Sprite;
    [SerializeField] private Sprite[] Cleopatra_Sprite;
    [SerializeField] Sprite TurboToggleSprite;

    [Header("Miscellaneous UI")]
    [SerializeField] private TMP_Text Balance_text;
    [SerializeField] private TMP_Text TotalBet_text;
    [SerializeField] private TMP_Text LineBet_text;
    [SerializeField] private TMP_Text TotalWin_text;
    [SerializeField] private TMP_Text BigWin_Text;
    [SerializeField] private TMP_Text BonusWin_Text;
    [SerializeField] private TMP_Text Minor_JackPot_Text;
    [SerializeField] private TMP_Text Major_JackPot_Text;
    [SerializeField] private TMP_Text Grand_JackPot_Text;
    [SerializeField] private int SelctedWildIndex;



    // [SerializeField] private TMP_Text BigWin_Text;
    // [SerializeField] private TMP_Text BonusWin_Text;

    [Header("Audio Management")]
    [SerializeField] internal AudioController audioController;

    [SerializeField] private UIManager uiManager;

    [Header("BonusGame Popup")]
    [SerializeField] private BonusController _bonusManager;

    [Header("Free Spins Board")]
    [SerializeField] private GameObject FSBoard_Object;
    [SerializeField] private TMP_Text FSnum_text;

    int tweenHeight = 0;  //calculate the height at which tweening is done

    [SerializeField] private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    private Tweener WinTween = null;

    [SerializeField] private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [SerializeField] private SocketIOManager SocketManager;

    [Header("Free Spin Wild Options")]
    [SerializeField] private Button GrayWild_Button;
    [SerializeField] private Button RedWild_Button;
    [SerializeField] private Button OrangeWild_Button;
    [SerializeField] private Button PurpleWild_Button;
    [SerializeField] private Button SkyBlueWild_Button;
    [SerializeField] private Button YellowWild_Button;
    [SerializeField] private Button GreenWild_Button;
    [SerializeField] private TMP_Text MystryChoice_Text;
    [SerializeField] private TMP_Text MysrtryNumber_Text;
    [SerializeField] private TMP_Text MysrtryMultiplier_Text;
    [SerializeField] private GameObject MysrtryNumberAnimation;
    [SerializeField] private GameObject MysrtryMultiplierAnimation;

    [SerializeField] private TMP_Text GrayWild_SpinCount_Text;
    [SerializeField] private TMP_Text GrayWild_Multiplier_Text;
    [SerializeField] private TMP_Text RedWild_SpinCount_Text;
    [SerializeField] private TMP_Text RedWild_Multiplier_Text;
    [SerializeField] private TMP_Text OrangeWild_SpinCount_Text;
    [SerializeField] private TMP_Text OrangeWild_Multiplier_Text;
    [SerializeField] private TMP_Text PurpleWild_SpinCount_Text;
    [SerializeField] private TMP_Text PurpleWild_Multiplier_Text;
    [SerializeField] private TMP_Text SkyBlueWild_SpinCount_Text;
    [SerializeField] private TMP_Text SKyBlueWild_Multiplier_Text;
    [SerializeField] private TMP_Text YellowWild_SpinCount_Text;
    [SerializeField] private TMP_Text YellowWild_Multiplier_Text;

    [SerializeField] private GameObject FreeSpinPanel;
    [SerializeField] private TMP_Text TotalSpinsText;
    [SerializeField] private TMP_Text LeftSpinsText;


    [Header("Animated Sprites")]

    [SerializeField] private Sprite[] Nine_Sprite;
    [SerializeField] private Sprite[] Ten_Sprite;
    [SerializeField] private Sprite[] J_Sprite;
    [SerializeField] private Sprite[] Q_Sprite;
    [SerializeField] private Sprite[] K_Sprite;
    [SerializeField] private Sprite[] A_Sprite;
    [SerializeField] private Sprite[] BlueCoin_Sprite;
    [SerializeField] private Sprite[] Card_Sprite;
    [SerializeField] private Sprite[] GreenCoin_Sprite;
    [SerializeField] private Sprite[] Gold_Sprite;
    [SerializeField] private Sprite[] Pot_Sprite;
    [SerializeField] private Sprite[] Wild_Sprite;
    [SerializeField] private Sprite[] Scatter_Sprite;
    [SerializeField] private Sprite[] GrayWild_Sprite;
    [SerializeField] private Sprite[] RedWild_Sprite;
    [SerializeField] private Sprite[] OrangeWild_Sprite;
    [SerializeField] private Sprite[] PurpleWild_Sprite;
    [SerializeField] private Sprite[] BlueWild_Sprite;
    [SerializeField] private Sprite[] YellowWild_Sprite;
    [SerializeField] private Sprite[] Green_Sprite;
    [SerializeField]
    private List<SpriteListWrapper> WildAnimationSprites = new List<SpriteListWrapper>();

    [SerializeField] private GameObject SymbolsToEmitAnimPanel;
    [SerializeField] private GameObject SymbolsToEmit_Asset_Parent;
    [SerializeField] private List<GameObject> SymbolsToEmit_Assets_List = new List<GameObject>();
    [SerializeField] private TMP_Text SymbolsWinning_Text;


    [Header("Bonus Mini Game")]
    [SerializeField] private GameObject MiniBonus_Game_Panel;
    [SerializeField] private Button Row1Column1_Button;
    [SerializeField] private Button Row1Column2_Button;
    [SerializeField] private Button Row1Column3_Button;
    [SerializeField] private Button Row1Column4_Button;
    [SerializeField] private Button Row2Column1_Button;
    [SerializeField] private Button Row2Column2_Button;
    [SerializeField] private Button Row2Column3_Button;
    [SerializeField] private Button Row2Column4_Button;
    [SerializeField] private Button Row3Column1_Button;
    [SerializeField] private Button Row3Column2_Button;
    [SerializeField] private Button Row3Column3_Button;
    [SerializeField] private Button Row3Column4_Button;

    [SerializeField] private TMP_Text MiniGame_LeftSpins_Text;
    [SerializeField] private int MiniGame_LeftSpinsCount;

    [SerializeField] private List<Button> MinigameCoinBtns;


    [Header("Golden Reels Image")]
    [SerializeField] private Image[] Reels;

    private bool IsSymbolsEmited = false;



    [SerializeField] private ImageAnimation[] AnimationsScripts;
    private Coroutine AutoSpinRoutine = null;
    private Coroutine FreeSpinRoutine = null;
    private Coroutine tweenroutine;
    private Coroutine BoxAnimRoutine = null;

    private Coroutine BOXCORoutine = null;

    float boostDuration = 1.5f;
    float boostFactor = 0.25f;
    private bool IsAutoSpin = false;
    private bool IsFreeSpin = false;
    private bool WinAnimationFin = true;

    internal bool IsSpinning = false;
    private bool CheckSpinAudio = false;
    internal bool CheckPopups = false;

    private int BetCounter = 0;
    private double currentBalance = 0;
    private double currentTotalBet = 0;
    protected int Lines = 20;
    int ReelIndexNumber = 0;

    [SerializeField] private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing
    private int numberOfSlots = 5;          //number of columns
    private bool StopSpinToggle;
    private float SpinDelay = 0.2f;
    private bool IsTurboOn;
    private bool WasAutoSpinOn;
    private bool IsStopTweening;
    private bool endBoostDuration;
    private Tween BalanceTween;
    private void Start()
    {
        IsAutoSpin = false;

        if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
        if (Turbo_Button) Turbo_Button.onClick.AddListener(TurboToggle);

        if (StopSpin_Button) StopSpin_Button.onClick.RemoveAllListeners();
        if (StopSpin_Button) StopSpin_Button.onClick.AddListener(() => { audioController.PlayButtonAudio(); StopSpinToggle = true; StopSpin_Button.gameObject.SetActive(false); });

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            SkipSymbolsEmitedAniamiton();
            StartSlots();
        });

        if (TotalBetPlus_Button) TotalBetPlus_Button.onClick.RemoveAllListeners();
        if (TotalBetPlus_Button) TotalBetPlus_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            ChangeBet(true);
        });

        if (TotalBetMinus_Button) TotalBetMinus_Button.onClick.RemoveAllListeners();
        if (TotalBetMinus_Button) TotalBetMinus_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            ChangeBet(false);
        });

        if (LineBetPlus_Button) LineBetPlus_Button.onClick.RemoveAllListeners();
        if (LineBetPlus_Button) LineBetPlus_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            ChangeBet(true);
        });

        if (LineBetMinus_Button) LineBetMinus_Button.onClick.RemoveAllListeners();
        if (LineBetMinus_Button) LineBetMinus_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            ChangeBet(false);
        });

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            AutoSpin();
        });

        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            StopAutoSpin(false);
        });

        if (SkipWinAnimation_Button) SkipWinAnimation_Button.onClick.RemoveAllListeners();
        if (SkipWinAnimation_Button) SkipWinAnimation_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            StopGameAnimation();
        });

        if (BonusSkipWinAnimation_Button) BonusSkipWinAnimation_Button.onClick.RemoveAllListeners();
        if (BonusSkipWinAnimation_Button) BonusSkipWinAnimation_Button.onClick.AddListener(delegate
        {
            uiManager.CanCloseMenu();
            StopGameAnimation();
        });

        if (SymbolsToEmitSkipWinAnimation_Button) SymbolsToEmitSkipWinAnimation_Button.onClick.RemoveAllListeners();
        if (SymbolsToEmitSkipWinAnimation_Button) SymbolsToEmitSkipWinAnimation_Button.onClick.AddListener(delegate
        {
            //  uiManager.CanCloseMenu();
            SkipSymbolsEmitedAniamiton();
        });

        if (FSBoard_Object) FSBoard_Object.SetActive(false);

        tweenHeight = (13 * IconSizeFactor) - 280;
        //Debug.Log("Tween Height: " + tweenHeight);

        // Wild Buttonns On click Method
        if (GrayWild_Button) GrayWild_Button.onClick.RemoveAllListeners();
        if (GrayWild_Button) GrayWild_Button.onClick.AddListener(() => SelectWildOnFreeSpins(0));

        if (RedWild_Button) RedWild_Button.onClick.RemoveAllListeners();
        if (RedWild_Button) RedWild_Button.onClick.AddListener(() => SelectWildOnFreeSpins(1));

        if (OrangeWild_Button) OrangeWild_Button.onClick.RemoveAllListeners();
        if (OrangeWild_Button) OrangeWild_Button.onClick.AddListener(() => SelectWildOnFreeSpins(2));

        if (PurpleWild_Button) PurpleWild_Button.onClick.RemoveAllListeners();
        if (PurpleWild_Button) PurpleWild_Button.onClick.AddListener(() => SelectWildOnFreeSpins(3));

        if (SkyBlueWild_Button) SkyBlueWild_Button.onClick.RemoveAllListeners();
        if (SkyBlueWild_Button) SkyBlueWild_Button.onClick.AddListener(() => SelectWildOnFreeSpins(4));

        if (YellowWild_Button) YellowWild_Button.onClick.RemoveAllListeners();
        if (YellowWild_Button) YellowWild_Button.onClick.AddListener(() => SelectWildOnFreeSpins(5));

        if (GreenWild_Button) GreenWild_Button.onClick.RemoveAllListeners();
        if (GreenWild_Button) GreenWild_Button.onClick.AddListener(() => StartCoroutine(OnMystryWildClicked()));


        // Mini Bonus Game Buttons 
        if (Row1Column1_Button) Row1Column1_Button.onClick.RemoveAllListeners();
        if (Row1Column1_Button) Row1Column1_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 0, 0 });
            Row1Column1_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row1Column1_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;

            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());

        });

        if (Row1Column2_Button) Row1Column2_Button.onClick.RemoveAllListeners();
        if (Row1Column2_Button) Row1Column2_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 0, 1 });
            Row1Column2_Button.interactable = false;

            SetMiniGameButtonsInteractable(false);
            Row1Column2_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;

            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row1Column3_Button) Row1Column3_Button.onClick.RemoveAllListeners();
        if (Row1Column3_Button) Row1Column3_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 0, 2 });
            Row1Column3_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row1Column3_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row1Column4_Button) Row1Column4_Button.onClick.RemoveAllListeners();
        if (Row1Column4_Button) Row1Column4_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 0, 3 });
            Row1Column4_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row1Column4_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row2Column1_Button) Row2Column1_Button.onClick.RemoveAllListeners();
        if (Row2Column1_Button) Row2Column1_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 1, 0 });
            Row2Column1_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row2Column1_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row2Column2_Button) Row2Column2_Button.onClick.RemoveAllListeners();
        if (Row2Column2_Button) Row2Column2_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 1, 1 });
            Row2Column2_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row2Column2_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });
        if (Row2Column3_Button) Row2Column3_Button.onClick.RemoveAllListeners();
        if (Row2Column3_Button) Row2Column3_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 1, 2 });
            Row2Column3_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row2Column3_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row2Column4_Button) Row2Column4_Button.onClick.RemoveAllListeners();
        if (Row2Column4_Button) Row2Column4_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 1, 3 });
            Row2Column4_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row2Column4_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row3Column1_Button) Row3Column1_Button.onClick.RemoveAllListeners();
        if (Row3Column1_Button) Row3Column1_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 2, 0 });
            Row3Column1_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row3Column1_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;



            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row3Column2_Button) Row3Column2_Button.onClick.RemoveAllListeners();
        if (Row3Column2_Button) Row3Column2_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 2, 1 });
            Row3Column2_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row3Column2_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row3Column3_Button) Row3Column3_Button.onClick.RemoveAllListeners();
        if (Row3Column3_Button) Row3Column3_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 2, 2 });
            Row3Column3_Button.interactable = false;

            SetMiniGameButtonsInteractable(false);
            Row3Column3_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;

            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

        if (Row3Column4_Button) Row3Column4_Button.onClick.RemoveAllListeners();
        if (Row3Column4_Button) Row3Column4_Button.onClick.AddListener(delegate
        {
            SocketManager.SendSelectedFlipCoin(new List<int> { 2, 3 });
            Row3Column4_Button.interactable = false;
            SetMiniGameButtonsInteractable(false);
            Row3Column4_Button.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = true;


            SocketManager.isResultdone = false;
            StartCoroutine(MiniGAmeAnim());
        });

    }

    void TurboToggle()
    {
        audioController.PlayButtonAudio();
        if (IsTurboOn)
        {
            IsTurboOn = false;
            Turbo_Button.GetComponent<ImageAnimation>().StopAnimation();
            Turbo_Button.image.sprite = TurboToggleSprite;
        }
        else
        {
            IsTurboOn = true;
            Turbo_Button.GetComponent<ImageAnimation>().StartAnimation();
        }
    }

    #region Autospin
    private void AutoSpin()
    {
        if (!IsAutoSpin)
        {
            IsAutoSpin = true;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
        }
    }

    private void StopAutoSpin(bool isfreespin)
    {
        if (IsAutoSpin)
        {
            StartCoroutine(StopAutoSpinCoroutine(isfreespin));
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
        }
    }

    private IEnumerator StopAutoSpinCoroutine(bool IsfreeSpin)
    {
        if (AutoSpinStop_Button) AutoSpinStop_Button.interactable = false;
        yield return new WaitUntil(() => !IsSpinning);
        if (!IsfreeSpin) ToggleButtonGrp(true);
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            AutoSpinStop_Button.interactable = true;
            tweenroutine = null;
            AutoSpinRoutine = null;
            IsAutoSpin = false;
            if (IsfreeSpin) ToggleButtonGrp(false);
            StopCoroutine(StopAutoSpinCoroutine(IsfreeSpin));
        }
    }
    #endregion

    #region FreeSpin
    internal void FreeSpin(int spins)
    {

        if (!IsFreeSpin)
        {
            if (FSnum_text) FSnum_text.text = spins.ToString();
            IsFreeSpin = true;
            ToggleButtonGrp(false);

            if (FreeSpinRoutine != null)
            {
                StopCoroutine(FreeSpinRoutine);
                FreeSpinRoutine = null;
            }
            FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));
        }
    }

    private IEnumerator FreeSpinCoroutine(int spinchances)
    {
        FreeSpinPanel.SetActive(true);
        int i = 0;
        int j = spinchances;
        while (i < spinchances)
        {

            j -= 1;
            if (TotalSpinsText) TotalSpinsText.text = spinchances.ToString();
            if (LeftSpinsText) LeftSpinsText.text = j.ToString();


            StartSlots(false, true);

            yield return tweenroutine;
            yield return new WaitForSeconds(SpinDelay);
            i++;
        }
        IsFreeSpin = false;
        yield return _bonusManager.BonusGameEndRoutine(true, 0);
        if (Balance_text) Balance_text.text = SocketManager.playerdata.balance.ToString("f3");
        FreeSpinPanel.SetActive(false);
        myImages[11] = WildTypesImages[1];
        Wild_Sprite = WildAnimationSprites[1].sprites.ToArray();

        if (WasAutoSpinOn)
        {
            AutoSpin();
        }
        else
        {
            ToggleButtonGrp(true);
        }
    }
    #endregion

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
            SlotStart_Button.interactable = true;
        }
    }

    #region LinesCalculation

    //Destroy Static Lines from button hovers
    internal void DestroyStaticLine()
    {
        PayCalculator.ResetStaticLine();
    }
    #endregion

    private void ChangeBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            BetCounter++;
            if (BetCounter >= SocketManager.initialData.gameData.bets.Count)
            {
                BetCounter = 0; // Loop back to the first bet
            }
        }
        else
        {
            BetCounter--;
            if (BetCounter < 0)
            {
                BetCounter = SocketManager.initialData.gameData.bets.Count - 1; // Loop to the last bet
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.gameData.bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.gameData.bets[BetCounter]).ToString();
        currentTotalBet = SocketManager.initialData.gameData.bets[BetCounter];
        // CompareBalance();
    }

    #region InitialFunctions
    internal void shuffleInitialMatrix(bool midTween = false)
    {
        if (IsStopTweening || StopSpinToggle)
        {
            return;
        }
        for (int i = 0; i < images.Count; i++)
        {
            for (int j = 0; j < images[i].slotImages.Count; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 13);
                if (j >= 8 && j <= 10 && midTween)
                {
                    continue;
                }
                images[i].slotImages[j].sprite = myImages[randomIndex];
            }
        }
    }

    internal void SetInitialUI()
    {
        Debug.Log("@@@@@ Balance :" + SocketManager.initialData.player.balance);
        BetCounter = 0;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.gameData.bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.gameData.bets[BetCounter]).ToString();
        if (TotalWin_text) TotalWin_text.text = "0.000";
        if (Balance_text) Balance_text.text = SocketManager.initialData.player.balance.ToString("f3");
        if (Minor_JackPot_Text) Minor_JackPot_Text.text = SocketManager.initialData.features.jackpotMultipliers[0].ToString() + " x";
        if (Major_JackPot_Text) Major_JackPot_Text.text = SocketManager.initialData.features.jackpotMultipliers[1].ToString() + " x";
        if (Grand_JackPot_Text) Grand_JackPot_Text.text = SocketManager.initialData.features.jackpotMultipliers[2].ToString() + " x";


        currentBalance = SocketManager.initialData.player.balance;
        currentTotalBet = SocketManager.initialData.gameData.bets[BetCounter];
        CompareBalance();
        SetWildFreeSpinData();
        uiManager.InitialiseUIData(SocketManager.initialData.uiData.paylines);
    }
    #endregion

    private void OnApplicationFocus(bool focus)
    {
        audioController.CheckFocusFunction(focus, CheckSpinAudio);
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        switch (val)
        {
            case 0:
                for (int i = 0; i < Nine_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Nine_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 1:
                for (int i = 0; i < Ten_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Ten_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 2:
                for (int i = 0; i < J_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(J_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 3:
                for (int i = 0; i < Q_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Q_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 4:
                for (int i = 0; i < K_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(K_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 5:
                for (int i = 0; i < A_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(A_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 6:
                for (int i = 0; i < BlueCoin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(BlueCoin_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 7:
                for (int i = 0; i < Card_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Card_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 8:
                for (int i = 0; i < GreenCoin_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(GreenCoin_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 9:
                for (int i = 0; i < Gold_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Gold_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 10:
                for (int i = 0; i < Pot_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Pot_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

            case 11:
                for (int i = 0; i < Wild_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Wild_Sprite[i]);
                }
                animScript.AnimationSpeed = 8f;
                break;

            case 12:
                for (int i = 0; i < Scatter_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Scatter_Sprite[i]);
                }
                animScript.AnimationSpeed = 10f;
                break;

        }
    }
    #region SlotSpin
    //starts the spin process
    private void StartSlots(bool autoSpin = false, bool bonus = false)
    {
        if (audioController) audioController.PlaySpinButtonAudio();

        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }
        }
        //WinningsAnim(false);
        if (SlotStart_Button) SlotStart_Button.interactable = false;

        StopGameAnimation();

        //PayCalculator.ResetLines();
        tweenroutine = StartCoroutine(TweenRoutine(bonus));

        if (TotalWin_text) TotalWin_text.text = "0.000";
    }

    //manage the Routine for spinning of the slots
    private IEnumerator TweenRoutine(bool bonus = false)
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin) // Check if balance is sufficient to place the bet
        {
            // CompareBalance();
            StopAutoSpin(false);
            yield return new WaitForSeconds(1);
            yield break;
        }
        ToggleButtonGrp(false);

        if (TotalWin_text) TotalWin_text.text = "0.000";
        ReelIndexNumber = 0;
        boostDuration = 1.5f;
        boostFactor = 0.5f;
        CheckSpinAudio = true;
        IsSpinning = true;
        foreach (ImageAnimation Script in AnimationsScripts)
        {
            if (Script) Script.StopAnimation();
        }
        //ToggleButtonGrp(false);
        if (!IsTurboOn && !IsFreeSpin && !IsAutoSpin)
        {
            StopSpin_Button.gameObject.SetActive(true);
        }
        for (int i = 0; i < numberOfSlots; i++) // Initialize tweening for slot animations
        {
            InitializeTweening(Slot_Transform[i]);
            if (!bonus) yield return new WaitForSeconds(0.1f);
        }
        if (BOXCORoutine != null) StopCoroutine(BOXCORoutine);
        SymbolsToEmitAnimPanel.SetActive(false);
        foreach (Image a in Reels)
        {
            if (a.enabled) a.enabled = false;
        }

        if (!bonus) // Deduct balance if not a bonus
        {
            BalanceDeduction();
        }

        SocketManager.AccumulateResult(BetCounter);
        yield return new WaitUntil(() => SocketManager.isResultdone);
        currentBalance = SocketManager.playerdata.balance;

        // for (int j = 0; j < SocketManager.resultData.resultSymbols.Count; j++) // Update slot images based on the results
        // {
        //     List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
        //     for (int i = 0; i < 5; i++)
        //     {

        //         if (images[i].slotImages[j]) images[i].slotImages[j].sprite = myImages[resultnum[i]];
        //         if (Tempimages[i].slotImages[j]) Tempimages[i].slotImages[j].sprite = myImages[resultnum[i]];
        //         PopulateAnimationSprites(Tempimages[i].slotImages[j].gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
        //     }

        // }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int resultNum = int.Parse(SocketManager.resultData.matrix[i][j]);
                print("resultNum: " + resultNum);
                print("image loc: " + j + " " + i);
                Tempimages[j].slotImages[i].sprite = myImages[resultNum];
                PopulateAnimationSprites(Tempimages[j].slotImages[i].GetComponent<ImageAnimation>(), resultNum);

            }
        }
        if (IsTurboOn)
        {
            // yield return new WaitForSeconds(0.1f);
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.1f);
                if (StopSpinToggle)
                {
                    break;
                }
            }
            StopSpin_Button.gameObject.SetActive(false);
        }
        IsStopTweening = true;
        for (int i = 0; i < numberOfSlots; i++) // Stop tweening for each slot
        {
            yield return StopTweening(5, Slot_Transform[i], i, StopSpinToggle);
        }

        StopSpinToggle = false;
        yield return alltweens[^1].WaitForCompletion();
        KillAllTweens();
        if (SocketManager.playerdata.currentWining > 0)
        {
            SpinDelay = 1.2f;
        }
        else
        {
            SpinDelay = 0.5f;
        }

        // CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit, SocketManager.resultData.jackpot);
        // if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString("F3");
        if (SocketManager.resultData.payload.winAmount > 0) WinningsTextAnimation(bonus); // Trigger winnings animation if applicable
        if (SocketManager.resultData.features.goldenReels.Count >= 3)
        {
            CheckPopups = true;
            CheckforJackpot();
        }
        else
        {
            CheckPopups = true;
            CheckWinPopups();

        }

        yield return new WaitUntil(() => !CheckPopups);
        if (SocketManager.resultData.payload.wins.Count > 0)
        {
            CheckPopups = true;
            IsSymbolsEmited = false;
            BOXCORoutine = StartCoroutine(BoxRoutine());

        }
        yield return new WaitUntil(() => !CheckPopups);
        //  SymbolsToEmitAnimPanel.SetActive(false);
        if (SocketManager.resultData.features.bonus.isTriggered)
        {
            yield return new WaitForSeconds(1f);
            SkipSymbolsEmitedAniamiton();
            CheckPopups = true;
            SetMiniGameButtonsInteractable(true);
            MiniBonus_Game_Panel.SetActive(true);
            MiniGame_LeftSpinsCount = 5;
            MiniGame_LeftSpins_Text.text = MiniGame_LeftSpinsCount.ToString() + " Chances Left";

        }
        yield return new WaitUntil(() => !CheckPopups);
        if (!SymbolsToEmitAnimPanel.activeInHierarchy && SocketManager.resultData.payload.wins.Count > 0) SymbolsToEmitAnimPanel.SetActive(true);

        Debug.Log($"@@@@ bonus game checkpopup is : " + CheckPopups);
        if (bonus) _bonusManager.FreeSpinTotalWin += SocketManager.resultData.payload.winAmount;
        Debug.Log($"Current Free Spin Winning :" + _bonusManager.FreeSpinTotalWin);

        if (SocketManager.playerdata.currentWining <= 0 && !SocketManager.resultData.features.freeSpin.isTriggered)
        {
            audioController.PlayWLAudio("lose");
        }

        yield return new WaitUntil(() => !CheckPopups);

        if ((IsFreeSpin || IsAutoSpin) && BoxAnimRoutine != null && !WinAnimationFin) // Waits for winning payline animation to finish when triggered bonus
        {
            yield return new WaitUntil(() => WinAnimationFin);
            //yield return new WaitForSeconds(0.5f);
            StopGameAnimation();
        }


        if (SocketManager.resultData.features.freeSpin.isTriggered)
        {
            Debug.Log(IsFreeSpin ? "Bonus In Bonus" : "First Time Bonus");
            ToggleButtonGrp(false);

            yield return new WaitForSeconds(1f);

            if (!IsFreeSpin)
            {
                SkipSymbolsEmitedAniamiton();
                _bonusManager.StartFreeSpin();
                SlotStart_Button.interactable = false;
                Debug.Log($"Satart slot button interactable is : " + SlotStart_Button.interactable);

            }

            if (IsAutoSpin)
            {
                WasAutoSpinOn = true;
                IsSpinning = false;
                StopAutoSpin(true);
                ToggleButtonGrp(false);
            }
        }
        if (!IsAutoSpin && !IsFreeSpin) // Reset spinning state and toggle buttons
        {
            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            IsSpinning = false;
        }
    }
    #endregion

    internal void CheckWinPopups()
    {
        Debug.Log("@@@@@ win Amount :" + SocketManager.resultData.payload.winAmount);
        double WinningAmount = SocketManager.resultData.payload.winAmount;
        if (WinningAmount >= currentTotalBet * 5 && WinningAmount < currentTotalBet * 10)
        {
            uiManager.PopulateWin(1, WinningAmount);
        }
        else if (WinningAmount >= currentTotalBet * 10 && WinningAmount < currentTotalBet * 15)
        {
            uiManager.PopulateWin(2, WinningAmount);
        }
        else if (WinningAmount >= currentTotalBet * 15 && WinningAmount < currentTotalBet * 20)
        {
            uiManager.PopulateWin(3, WinningAmount);
        }
        else if (WinningAmount >= currentTotalBet * 20)
        {
            uiManager.PopulateWin(4, WinningAmount);
        }
        else
        {
            CheckPopups = false;
        }
    }

    private void CheckforJackpot()
    {
        int GoldenreelCoun = SocketManager.resultData.features.goldenReels.Count;
        double WinningAmount = SocketManager.resultData.payload.winAmount;
        if (GoldenreelCoun >= 3)
        {
            CheckPopups = true;
            if (GoldenreelCoun == 3)
            {
                uiManager.PopulateWin(5, WinningAmount);
            }
            else if (GoldenreelCoun == 4)
            {
                uiManager.PopulateWin(6, WinningAmount);
            }
            else
            {
                uiManager.PopulateWin(7, WinningAmount);

            }
        }
    }

    private void WinningsTextAnimation(bool bonus = false)
    {
        Debug.Log($"Winnig animation : Bonus " + bonus);
        double winAmt = 0;
        double currentWin = 0;

        double currentBal = 0;
        double Balance = 0;

        double BonusWinAmt = 0;
        double currentBonusWinnings = 0;

        // if (bonus)
        // {
        //     try
        //     {
        //         BonusWinAmt = double.Parse(SocketManager.playerdata.currentWining.ToString("f3"));
        //         currentBonusWinnings = double.Parse(BonusWin_Text.text);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log("Error while conversion " + e.Message);
        //     }
        // }
        try
        {
            winAmt = double.Parse(SocketManager.resultData.payload.winAmount.ToString("f3"));
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            currentBal = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            Balance = double.Parse(SocketManager.playerdata.balance.ToString("f3"));
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            currentWin = double.Parse(TotalWin_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        // if (bonus)
        // {
        //     double CurrTotal = BonusWinAmt + currentBonusWinnings;
        //     DOTween.To(() => currentBonusWinnings, (val) => currentBonusWinnings = val, CurrTotal, 0.8f).OnUpdate(() =>
        //     {
        //         if (BonusWin_Text) BonusWin_Text.text = currentBonusWinnings.ToString("f3");
        //     });

        //     double start = 0;
        //     DOTween.To(() => start, (val) => start = val, BonusWinAmt, 0.8f).OnUpdate(() =>
        //     {
        //         if (BigWin_Text) BigWin_Text.text = start.ToString("f3");
        //     });
        // }
        // else
        // {
        DOTween.To(() => currentWin, (val) => currentWin = val, winAmt, 0.5f).OnUpdate(() =>
        {
            if (TotalWin_text) TotalWin_text.text = currentWin.ToString("f3");
            if (BigWin_Text) BigWin_Text.text = currentWin.ToString("f3");
        });
        BalanceTween?.Kill();
        DOTween.To(() => currentBal, (val) => currentBal = val, Balance, 1f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = currentBal.ToString("f3");
        });
        // }
    }

    private void BalanceDeduction()
    {
        double bet = 0;
        double balance = 0;
        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }
        double initAmount = balance;

        balance = balance - bet;

        BalanceTween = DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.3f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = initAmount.ToString("f3");
        });
    }

    //generate the payout lines generated 
    private IEnumerator BoxRoutine()
    {
        SymbolsToEmitAnimPanel.SetActive(true);
        WinAnimationFin = false;

        while (true)
        {

            foreach (var Win in SocketManager.resultData.payload.wins)
            {

                List<int> pointAnim = new List<int>();
                foreach (var pos in Win.positions)
                {
                    int row = pos[0]; // row
                    int col = pos[1]; // col
                    int combined = col * 10 + row;
                    pointAnim.Add(combined);
                }

                // Deactivate all symbols
                foreach (GameObject go in SymbolsToEmit_Assets_List)
                {
                    go.SetActive(false);
                }

                for (int k = 0; k < pointAnim.Count; k++)
                {
                    if (!SymbolsToEmit_Assets_List[k].GetComponent<Image>().sprite) Debug.Log($"Nt got symbol");


                    if (pointAnim[k] >= 10)
                    {
                        if (!SymbolsToEmit_Assets_List[k].GetComponent<Image>().sprite) Debug.Log($"Nt got symbol");
                        if (!Tempimages[(pointAnim[k] / 10) % 10].slotImages[pointAnim[k] % 10].gameObject.GetComponent<Image>().sprite) Debug.Log($"Nt got tempImages");
                        Tempimages[(pointAnim[k] / 10) % 10].slotImages[pointAnim[k] % 10].gameObject.GetComponent<ImageAnimation>().StartAnimation();
                        SymbolsToEmit_Assets_List[k].GetComponent<Image>().sprite = Tempimages[(pointAnim[k] / 10) % 10].slotImages[pointAnim[k] % 10].gameObject.GetComponent<Image>().sprite;
                    }
                    else
                    {
                        Tempimages[0].slotImages[pointAnim[k]].GetComponent<ImageAnimation>().StartAnimation();
                        SymbolsToEmit_Assets_List[k].GetComponent<Image>().sprite = Tempimages[0].slotImages[pointAnim[k]].GetComponent<Image>().sprite;
                    }
                    SymbolsToEmit_Assets_List[k].SetActive(true);
                }
                SymbolsWinning_Text.text = Win.payout.ToString();
                CheckPopups = false;


                yield return new WaitForSeconds(1.3f);

                for (int k = 0; k < pointAnim.Count; k++)
                {
                    if (pointAnim[k] >= 10)
                    {
                        // Tempimages[(pointAnim[k] / 10) % 10].slotImages[pointAnim[k] % 10].gameObject.SetActive(false);
                        Tempimages[(pointAnim[k] / 10) % 10].slotImages[pointAnim[k] % 10].gameObject.GetComponent<ImageAnimation>().StopAnimation();

                    }
                    else
                    {
                        // Tempimages[0].slotImages[pointAnim[k]].gameObject.SetActive(false);
                        Tempimages[0].slotImages[pointAnim[k]].GetComponent<ImageAnimation>().StopAnimation();
                    }
                }


            }


            if (IsFreeSpin || IsAutoSpin)
            {
                CheckPopups = false;
                StopCoroutine(BOXCORoutine);
                SymbolsToEmitAnimPanel.SetActive(false);

            }
            if (IsSymbolsEmited)
            {
                StopCoroutine(BOXCORoutine);
                SymbolsToEmitAnimPanel.SetActive(false);
                CheckPopups = false;
            }
            CheckPopups = false;
            yield return null;

        }
    }


    private IEnumerator MiniGAmeAnim()
    {
        yield return new WaitUntil(() => SocketManager.isResultdone);
        List<int> parts = SocketManager.jackpootPayload.selectedIndex;
        int combined = parts[0] * 10 + parts[1];
        MiniGame_LeftSpinsCount -= 1;
        MiniGame_LeftSpins_Text.text = MiniGame_LeftSpinsCount.ToString() + " Chances Left";

        if (combined >= 10)
        {
            var imageAnim = MiniGameSprites[(combined / 10) % 10].slotImages[combined % 10].gameObject.GetComponent<ImageAnimation>();
            if (SocketManager.jackpootPayload.jackpotType == "GRAND")
            {
                imageAnim.textureArray = GrandgameSprites.ToList();
                yield return new WaitUntil(() => imageAnim.textureArray != null && imageAnim.textureArray.Count > 0);
                imageAnim.StartAnimation();
                ResetMiniGamesbtns();
            }
            else if (SocketManager.jackpootPayload.jackpotType == "MAJOR")
            {
                imageAnim.textureArray = MajorgameSprites.ToList();
                yield return new WaitUntil(() => imageAnim.textureArray != null && imageAnim.textureArray.Count > 0);
                imageAnim.StartAnimation();
                ResetMiniGamesbtns();
            }
            else
            {
                imageAnim.textureArray = MinorgameSprites.ToList();
                yield return new WaitUntil(() => imageAnim.textureArray != null && imageAnim.textureArray.Count > 0);
                imageAnim.StartAnimation();
                ResetMiniGamesbtns();
            }
        }
        else
        {
            var imageAnim = MiniGameSprites[0].slotImages[combined].GetComponent<ImageAnimation>();
            if (SocketManager.jackpootPayload.jackpotType == "GRAND")
            {
                imageAnim.textureArray = GrandgameSprites.ToList();
                yield return new WaitUntil(() => imageAnim.textureArray != null && imageAnim.textureArray.Count > 0);
                imageAnim.StartAnimation();
                ResetMiniGamesbtns();
            }

            else if (SocketManager.jackpootPayload.jackpotType == "MAJOR")
            {
                imageAnim.textureArray = MajorgameSprites.ToList();
                yield return new WaitUntil(() => imageAnim.textureArray != null && imageAnim.textureArray.Count > 0);
                imageAnim.StartAnimation();
                ResetMiniGamesbtns();
            }

            else
            {
                imageAnim.textureArray = MinorgameSprites.ToList();
                yield return new WaitUntil(() => imageAnim.textureArray != null && imageAnim.textureArray.Count > 0);
                imageAnim.StartAnimation();
                ResetMiniGamesbtns();
            }

        }
        if (SocketManager.jackpootPayload.isOver)
        {
            yield return new WaitForSeconds(1f);
            if (SocketManager.jackpootPayload.winAmount > 0)
            {
                yield return _bonusManager.BonusGameEndRoutine(false, SocketManager.jackpootPayload.winAmount);
                if (Balance_text) Balance_text.text = SocketManager.playerdata.balance.ToString("f3");
            }
            yield return new WaitForSeconds(0.5f);
            MiniBonus_Game_Panel.SetActive(false);
            CheckPopups = false;
            Debug.Log($"@@@ IS over got ");
        }

    }

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }

    void ToggleButtonGrp(bool toggle)
    {
        Debug.Log($" toggle btn grp called :" + toggle);
        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (AutoSpin_Button && !IsAutoSpin) AutoSpin_Button.interactable = toggle;
        if (TotalBetPlus_Button) TotalBetPlus_Button.interactable = toggle;
        if (TotalBetMinus_Button) TotalBetMinus_Button.interactable = toggle;
        if (Turbo_Button) Turbo_Button.interactable = toggle;
        Debug.Log("@@@@@@@  toggle btn true" + toggle);

    }
    private void SetMiniGameButtonsInteractable(bool interactable)
    {
        Row1Column1_Button.interactable = interactable;
        Row1Column2_Button.interactable = interactable;
        Row1Column3_Button.interactable = interactable;
        Row1Column4_Button.interactable = interactable;
        Row2Column1_Button.interactable = interactable;
        Row2Column2_Button.interactable = interactable;
        Row2Column3_Button.interactable = interactable;
        Row2Column4_Button.interactable = interactable;
        Row3Column1_Button.interactable = interactable;
        Row3Column2_Button.interactable = interactable;
        Row3Column3_Button.interactable = interactable;
        Row3Column4_Button.interactable = interactable;
        if (interactable)
        {
            foreach (Button btn in MinigameCoinBtns)
            {
                btn.gameObject.GetComponent<Image>().sprite = MinorgameSprites[0];
                btn.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked = false;
                btn.gameObject.GetComponent<ImageAnimation>().textureArray.Clear();

            }
        }
    }

    private void ResetMiniGamesbtns()
    {
        foreach (Button btn in MinigameCoinBtns)
        {
            if (btn.gameObject.GetComponent<BonusGameCoin>().IsCoinClicked)
            {
                btn.interactable = false;
            }
            else
            {
                btn.interactable = true;
            }
        }
    }

    //Start the icons animation
    private void StartGameAnimation(GameObject animObjects)
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        if (temp.textureArray.Count > 0)
        {
            temp.StartAnimation();
            TempList.Add(temp);
        }
    }

    //Stop the icons animation
    internal void StopGameAnimation()
    {
        if (BoxAnimRoutine != null)
        {
            StopCoroutine(BoxAnimRoutine);
            BoxAnimRoutine = null;
            WinAnimationFin = true;
        }

        // CheckPopups = true;
        // StopCoroutine(BoxRoutine());

        if (TempBoxScripts.Count > 0)
        {
            for (int i = 0; i < TempBoxScripts.Count; i++)
            {
                foreach (BoxScripting b in TempBoxScripts[i].boxScripts)
                {
                    b.isAnim = false;
                    b.ResetBG();
                }
            }
        }

        if (SkipWinAnimation_Button) SkipWinAnimation_Button.gameObject.SetActive(false);
        if (BonusSkipWinAnimation_Button) BonusSkipWinAnimation_Button.gameObject.SetActive(false);

        if (TempList.Count > 0)
        {
            for (int i = 0; i < TempList.Count; i++)
            {
                TempList[i].StopAnimation();
            }
            TempList.Clear();
            TempList.TrimExcess();
        }

        PayCalculator.DontDestroyLines.Clear();
        PayCalculator.DontDestroyLines.TrimExcess();
        PayCalculator.ResetStaticLine();
    }


    public void SkipSymbolsEmitedAniamiton()
    {
        IsSymbolsEmited = true;
        if (BOXCORoutine != null) StopCoroutine(BOXCORoutine);
        SymbolsToEmitAnimPanel.SetActive(false);
        CheckPopups = false;
    }
    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        Tweener tweener = null;
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener = slotTransform.DOLocalMoveY(-2704, .3f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetDelay(0).OnStepComplete(() => { });
        tweener.Play();

        alltweens.Add(tweener);

    }

    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index, bool isStop = false)
    {
        if (!isStop)
        {
            int ReelCount = SocketManager.resultData.features.goldenReels.Count;
            endBoostDuration = false;
            int count = 0;

            if (SocketManager.resultData.features.goldenReels.Contains(index))
            {

                if (ReelIndexNumber >= 2)
                {
                    StartCoroutine(CheckforGoldenReelIndex3(boostDuration, index));
                    yield return new WaitUntil(() => endBoostDuration);
                    boostDuration += boostDuration + boostFactor;
                }
                else
                {
                    Reels[index].enabled = true;
                }
                ReelIndexNumber++;
            }



            bool IsRegister = false;
            yield return alltweens[index].OnStepComplete(delegate { IsRegister = true; });
            yield return new WaitUntil(() => IsRegister);
        }
        alltweens[index].Kill();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 97.75f, 0.5f).SetEase(Ease.OutBack, 2);
        //alltweens[index] = slotTransform.DOLocalMoveY(-1055 , 0.5f).SetEase(Ease.OutQuad);
        if (audioController) audioController.PlayWLAudio("spinStop");
        if (!IsTurboOn && !isStop)
        {
            yield return alltweens[index].WaitForCompletion();
        }

    }

    private IEnumerator CheckforGoldenReelIndex3(float boostDuration, int index)
    {

        yield return new WaitForSeconds(boostDuration);
        Reels[index].enabled = true;
        endBoostDuration = true;
    }



    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();
        IsStopTweening = false;
    }
    #endregion

    public void SetWildFreeSpinData()
    {
        //Set Free Spins Data
        if (GrayWild_SpinCount_Text) GrayWild_SpinCount_Text.text = SocketManager.initialData.features.freespinOptions[0].count.ToString();
        if (RedWild_SpinCount_Text) RedWild_SpinCount_Text.text = SocketManager.initialData.features.freespinOptions[1].count.ToString();
        if (OrangeWild_SpinCount_Text) OrangeWild_SpinCount_Text.text = SocketManager.initialData.features.freespinOptions[2].count.ToString();
        if (PurpleWild_SpinCount_Text) PurpleWild_SpinCount_Text.text = SocketManager.initialData.features.freespinOptions[3].count.ToString();
        if (SkyBlueWild_SpinCount_Text) SkyBlueWild_SpinCount_Text.text = SocketManager.initialData.features.freespinOptions[4].count.ToString();
        if (YellowWild_SpinCount_Text) YellowWild_SpinCount_Text.text = SocketManager.initialData.features.freespinOptions[5].count.ToString();

        //Set Multipliers Data
        if (GrayWild_Multiplier_Text) GrayWild_Multiplier_Text.text = string.Join(", ", SocketManager.initialData.features.freespinOptions[0].multiplier);
        if (RedWild_Multiplier_Text) RedWild_Multiplier_Text.text = string.Join(", ", SocketManager.initialData.features.freespinOptions[1].multiplier);
        if (OrangeWild_Multiplier_Text) OrangeWild_Multiplier_Text.text = string.Join(", ", SocketManager.initialData.features.freespinOptions[2].multiplier);
        if (PurpleWild_Multiplier_Text) PurpleWild_Multiplier_Text.text = string.Join(", ", SocketManager.initialData.features.freespinOptions[3].multiplier);
        if (SKyBlueWild_Multiplier_Text) SKyBlueWild_Multiplier_Text.text = string.Join(", ", SocketManager.initialData.features.freespinOptions[4].multiplier);
        if (YellowWild_Multiplier_Text) YellowWild_Multiplier_Text.text = string.Join(", ", SocketManager.initialData.features.freespinOptions[5].multiplier);

        // }


    }

    public void SelectWildOnFreeSpins(int index)
    {
        SocketManager.SendSelectedWildData(index);
        _bonusManager.IsWildSelected = true;
        myImages[11] = WildTypesImages[index];
        _bonusManager.FreeSpinCounts = SocketManager.initialData.features.freespinOptions[index].count;
        Wild_Sprite = WildAnimationSprites[index].sprites.ToArray();
    }

    private IEnumerator OnMystryWildClicked()
    {
        MystryChoice_Text.text = "";
        MysrtryMultiplier_Text.text = "";
        MysrtryNumberAnimation.gameObject.SetActive(true);
        MysrtryMultiplierAnimation.SetActive(true);
        int RandomIndex = UnityEngine.Random.Range(0, 6);
        yield return new WaitForSeconds(1.5f);

        MysrtryNumberAnimation.gameObject.SetActive(false);
        MysrtryMultiplierAnimation.SetActive(false);
        MystryChoice_Text.text = "FREE SPINS";
        MysrtryNumber_Text.text = SocketManager.initialData.features.freespinOptions[RandomIndex].count.ToString();
        MysrtryMultiplier_Text.text = string.Join(", ", SocketManager.initialData.features.freespinOptions[RandomIndex].multiplier);
        SelectWildOnFreeSpins(RandomIndex);

        yield return new WaitForSeconds(1f);

    }

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

[Serializable]
public class BoxScript
{
    public List<BoxScripting> boxScripts = new List<BoxScripting>(10);
}

[System.Serializable]
public class SpriteListWrapper
{
    public List<Sprite> sprites;
}


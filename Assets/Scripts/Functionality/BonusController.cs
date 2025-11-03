using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
    [SerializeField] private SlotBehaviour slotManager;
    [SerializeField] private SocketIOManager SocketManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioController _audioManager;
    [SerializeField] private GameObject FreeSpin_Panel;
    [SerializeField] private GameObject FreeSpinOpeningUI;

    [Header("Bonus Winning Popup")]
    [SerializeField] private GameObject MainPopup_Panel;
    [SerializeField] private GameObject BonusWinPopup_Object;
    [SerializeField] private TMP_Text BonusWinAmount_Text;
    [SerializeField] private TMP_Text BonusWinFreeSpinCoun_Text;


    internal bool IsWildSelected;
    internal int FreeSpinCounts;
    internal double FreeSpinTotalWin;

    [SerializeField] private GameObject BigBullAnimationPanel;

    internal void StartFreeSpin()
    {
        if (FreeSpin_Panel) FreeSpin_Panel.SetActive(true);
        FreeSpinOpeningUI.SetActive(true);
        uiManager.Bg_ThemeImage.sprite = uiManager.BG_ThemeSprites[1];
        uiManager.Reels_BgImage.sprite = uiManager.Reels_BGSprites[1];
        FreeSpinCounts = 0;
        FreeSpinTotalWin = 0;
        _audioManager.PlayBGAudio(true);
        StartCoroutine(FreeSpinStartRoutine());
    }

    private IEnumerator FreeSpinStartRoutine()
    {
        slotManager.MystryChoice_Text.text = "Mystry Choice";
        slotManager.MysrtryMultiplier_Text.text = "Mystry Multiplier";
        slotManager.MysrtryNumber_Text.text = "";

        yield return new WaitUntil(() => IsWildSelected == true);
        BigBullAnimationPanel.SetActive(true);
        yield return new WaitForSeconds(2.8f);

        slotManager.StopGameAnimation();


        FreeSpinOpeningUI.SetActive(true);
        // yield return StartCoroutine(TextAnimation(BonusOpeningText, BonusOpeningTitleRT, spins, 0, true));
        FreeSpinOpeningUI.SetActive(false);
        BigBullAnimationPanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        slotManager.FreeSpin(FreeSpinCounts);
        IsWildSelected = false;

    }

    internal IEnumerator BonusGameEndRoutine(bool IsfreeSpin, double WinAmount)
    {

        //  Debug.Log("@@@@ Game end routie called" + FreeSpinTotalWin);
        if (IsfreeSpin && FreeSpinTotalWin > 0)
        {
            // Debug.Log("@@@@ Game end routie called" + FreeSpinTotalWin);
            MainPopup_Panel.SetActive(true);
            BonusWinPopup_Object.SetActive(true);
            double currentValue = 0;
            DOTween.To(() => currentValue, x => currentValue = x, FreeSpinTotalWin, 1.3f)
           .OnUpdate(() =>
           {
               if (BonusWinAmount_Text) BonusWinAmount_Text.text = currentValue.ToString("f3");
           });
            if (BonusWinFreeSpinCoun_Text) BonusWinFreeSpinCoun_Text.text = "In " + FreeSpinCounts.ToString() + " Spins ";
            uiManager.Bg_ThemeImage.sprite = uiManager.BG_ThemeSprites[0];
            uiManager.Reels_BgImage.sprite = uiManager.Reels_BGSprites[0];
        }
        if (!IsfreeSpin)
        {
            MainPopup_Panel.SetActive(true);
            BonusWinPopup_Object.SetActive(true);
            double currentValue = 0;
            DOTween.To(() => currentValue, x => currentValue = x, WinAmount, 1.7f)
           .OnUpdate(() =>
           {
               if (BonusWinAmount_Text) BonusWinAmount_Text.text = currentValue.ToString("f3");
           });
            if (BonusWinFreeSpinCoun_Text) BonusWinFreeSpinCoun_Text.text = "";
        }

        yield return new WaitForSeconds(3f);
        MainPopup_Panel.SetActive(false);
        BonusWinPopup_Object.SetActive(false);
        _audioManager.PlayBGAudio(false);
        yield return null;
    }

    private IEnumerator TextAnimation(TMP_Text textObject, RectTransform imageObject, int IntGoal, double DoubleGoal, bool spin = false)
    {
        if (IntGoal != 0)
        {
            int start = 0;
            if (!spin)
            {
                DOTween.To(() => start, (val) => start = val, IntGoal, .8f).OnUpdate(() =>
                {
                    if (textObject) textObject.text = start.ToString("f3");
                });
            }
            else
            {
                DOTween.To(() => start, (val) => start = val, IntGoal, .8f).OnUpdate(() =>
                {
                    if (textObject) textObject.text = start.ToString() + " FREE SPINS.";
                });
            }

        }
        else if (DoubleGoal != 0)
        {
            double start = 0;
            DOTween.To(() => start, (val) => start = val, DoubleGoal, .8f).OnUpdate(() =>
             {
                 if (textObject) textObject.text = start.ToString("f3");
             });
        }

        yield return imageObject.DOScale(new Vector2(1.5f, 1.5f), 1.5f).SetLoops(2, LoopType.Yoyo).SetDelay(0).WaitForCompletion();
        yield return imageObject.DOScale(new Vector2(1, 1), 0.5f).WaitForCompletion();
        yield return new WaitForSeconds(1f);
    }
}

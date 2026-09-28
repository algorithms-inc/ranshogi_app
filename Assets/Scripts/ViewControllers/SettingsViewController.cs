using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using GFramework.Core;


public class SettingsViewController : MonoBehaviour, IGeneralButtonListManager
{
    private LoadingViewController loadingView;
    [SerializeField] LoadingViewController loadingViewPrefab;

    [SerializeField] Canvas canvas;

    private IGeneralButtonListStage _generalButtonListStage = null;

    private bool _isUserRegistered = false;
    List<string> buttonTextList;


    void Start()
    {
        AppManager.Instance.Setup();

        _isUserRegistered = Auth.instance.isUserRegistered();

        SetupStage();
    }


    private void SetupStage()
    {

        if(_isUserRegistered) { 
            if(Auth.instance.GetUser().isPayedUser) {
                buttonTextList = new List<string> {
                    "ユーザ情報変更",
                    "アプリ設定",
                    "アプリについて",
                    "ヘルプ",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "メニューに戻る",
                };
            } else {
                buttonTextList = new List<string> {
                    "プレミアム会員登録",
                    "ユーザ情報変更",
                    "アプリ設定",
                    "アプリについて",
                    "ヘルプ",
                    "",
                    "",
                    "",
                    "",
                    "メニューに戻る",
                };
            }
        } else {
            if(Auth.instance.GetUser().isPayedUser) {
                buttonTextList = new List<string> {
                    "ユーザ登録",
                    "ユーザ情報変更",
                    "アプリ設定",
                    "アプリについて",
                    "ヘルプ",
                    "",
                    "",
                    "",
                    "",
                    "メニューに戻る",
                };
            } else {
                buttonTextList = new List<string> {
                    "プレミアム会員登録",
                    "ユーザ登録",
                    "ユーザ情報変更",
                    "アプリ設定",
                    "アプリについて",
                    "ヘルプ",
                    "",
                    "",
                    "",
                    "メニューに戻る",
                };

            }
        }

        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "設定",
            buttonTextList: buttonTextList);

        _generalButtonListStage.ShowWithAnimation();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }



    // ===============================================
    // MARK: - ● OnTapButton ─────

    public void OnTapButton0() { executeMenuItem(0); }
    public void OnTapButton1() { executeMenuItem(1); }
    public void OnTapButton2() { executeMenuItem(2); }
    public void OnTapButton3() { executeMenuItem(3); }
    public void OnTapButton4() { executeMenuItem(4); }
    public void OnTapButton5() { executeMenuItem(5); }
    public void OnTapButton6() { executeMenuItem(6); }
    public void OnTapButton7() { executeMenuItem(7); }
    public void OnTapButton8() { executeMenuItem(8); }
    public void OnTapButton9() { executeMenuItem(9); }


    private void executeMenuItem(int buttonIndex)
    {

        if(buttonTextList[buttonIndex] == "ユーザ情報変更") {
            GoToUserSettings();
        } else if(buttonTextList[buttonIndex] == "アプリ設定") {
            GoToAppSettings();
        } else if(buttonTextList[buttonIndex] == "アプリについて") {
            GoToAppAbouts();
        } else if(buttonTextList[buttonIndex] == "プレミアム会員登録") {
            BuySubscription();
        } else if(buttonTextList[buttonIndex] == "メニューに戻る") {
            BackToMenu();
        } else if(buttonTextList[buttonIndex] == "ユーザ登録") {
            Register();
        } else if(buttonTextList[buttonIndex] == "ヘルプ") {
            Application.OpenURL("https://ranshogi-production.web.app/help.html");
        }
    }



    // ===============================================
    // MARK: - ● Method ─────

    private void GoToUserSettings() {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "UserSettings", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }

    private void GoToAppSettings() {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "AppSettings", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }

    private void GoToAppAbouts() {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "AppAboutsSettings", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }

    private void Register()
    {
        // 別端末で会員登録を行なっている可能性があるため、念の為再確認
        if (Auth.instance.isUserRegistered() == true) { return; }

        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "Register", nextStageType: EStageType.BLANK_PAGE);
    }

    private void BuySubscription() {
        SubscriptionView view = FindObjectOfType<SubscriptionView>();
        view.isInSettings = true;
        if(Auth.instance.IsUserVerified()) {
            view.ShowPremiumConfirmation();
        } else if(Auth.instance.isUserRegistered()) {
            view.ShowWaitingEmailConfirmationView();
        } else {
            view.ShowRegistrationView();
        }
    }


    private void BackToMenu()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "Menu", nextStageType: EStageType.MENU);
    }
}

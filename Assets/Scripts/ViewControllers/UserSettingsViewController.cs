using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Purchasing;

using GFramework.Core;


public class UserSettingsViewController : MonoBehaviour, IGeneralButtonListManager
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
                    "メールアドレス変更",
                    "パスワード変更",
                    "ニックネーム変更",
                    "プレミアム会員解約",
                    "ランキング掲載",
                    "ログアウト",
                    "退会",
                    "",
                    "",
                    "戻る",
                };
            } else {
                buttonTextList = new List<string> {
                    "メールアドレス変更",
                    "パスワード変更",
                    "ニックネーム変更",
                    "ランキング掲載",
                    #if UNITY_IOS
                    "プレミアム会員復元",
                    "ログアウト",
                    "退会",
                    #else
                    "ログアウト",
                    "退会",
                    "",
                    #endif
                    "",
                    "",
                    "戻る",
                };
            }
        } else {
            if(Auth.instance.GetUser().isPayedUser) {
                buttonTextList = new List<string> {
                    "ニックネーム変更",
                    "プレミアム会員解約",
                    "ランキング掲載",
                    "ログアウト",
                    "退会",
                    "",
                    "",
                    "",
                    "",
                    "戻る",
                };
            } else {
                buttonTextList = new List<string> {
                    "ニックネーム変更",
                    "ランキング掲載",
                    #if UNITY_IOS
                    "プレミアム会員復元",
                    "ログアウト",
                    "退会",
                    #else
                    "ログアウト",
                    "退会",
                    "",
                    #endif
                    "",
                    "",
                    "",
                    "",
                    "戻る",
                };
            }
        }

        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "ユーザ情報変更",
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
        if(buttonTextList[buttonIndex] == "メールアドレス変更") {
            ChangeEmail();
        } else if(buttonTextList[buttonIndex] == "パスワード変更") {
            ChangePassword();
        } else if(buttonTextList[buttonIndex] == "ニックネーム変更") {
            ChangeNickname();
        } else if(buttonTextList[buttonIndex] == "プレミアム会員解約") {
            HowToCancelSubscription();
        } else if(buttonTextList[buttonIndex] == "ログアウト") {
            Logout();
        } else if(buttonTextList[buttonIndex] == "退会") {
            DeleteUser();
        } else if(buttonTextList[buttonIndex] == "プレミアム会員復元") {
            Restore();
        } else if(buttonTextList[buttonIndex] == "戻る") {
            BackToSettings();
        } else if(buttonTextList[buttonIndex] == "ランキング掲載") {
            ShowRanking();
        }

    }

    private void Restore() {

        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("購読記録を確認中 ...", false, () => { });
        var manager = FindObjectOfType<IAPManager>();
        manager.SetHandlers(() => {}, RestoreSuccess, RestoreFailed);
        manager.Restore();
    }

    public void RestoreSuccess(Product product) {
        Destroy(loadingView.gameObject);
        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: "購読記録が確認できました。復元しますか？",
            textColorType: EWindowTextColorType.NORMAL,
            textButtonNo: "いいえ",
            textButtonYes: "はい",
            actionOnTapButtonYes: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide();
                loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
                loadingView.Init("購読記録を復元中 ...", false, () => { });
                var iapManager = FindObjectOfType<IAPManager>();
                iapManager.DoRestore(product, DoRestoreSuccess, (errorMessage) => {
                    AlertViewController.ShowOKConfirmation("購読記録を復元できませんでした。");
                });
            });
    }

    public void DoRestoreSuccess() {
        Destroy(loadingView.gameObject);
        Auth.instance.SetUserPayed();
        AppManager.Instance.SetIsPremiumUser(true);
        AlertViewController.ShowOKConfirmation("購読記録が復元され、プレミアム会員になりました。");
    }

    public void RestoreFailed(string error) {
        Destroy(loadingView.gameObject);
        AlertViewController.ShowOKConfirmation("購読記録を確認できませんでした。");
    }

    // ===============================================
    // MARK: - ● Method ─────

    private void ChangeEmail()
    {
        // 別端末で会員登録を行なっている可能性があるため、念の為再確認
        if (Auth.instance.isUserRegistered() == false) { return; }

        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "ChangeEmail", nextStageType: EStageType.BLANK_PAGE);
    }

    public void ShowRanking() {
        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: Auth.instance.GetUser().showNameOnRanking ? "現在ランキングに掲載されています。掲載しないようにしますか？" : "現在ランキングに掲載されていません。掲載するようにしますか？",
            textColorType: EWindowTextColorType.NORMAL,
            textButtonNo: "いいえ",
            textButtonYes: "はい",
            actionOnTapButtonYes: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide();
                loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
                loadingView.Init("更新中 ...", false, () => { });
                UpdateShowNameOnRankingRequest request = new UpdateShowNameOnRankingRequest();
                UpdateShowNameOnRankingRequest.ShowNameOnRanking showNameOnRanking = new UpdateShowNameOnRankingRequest.ShowNameOnRanking();
                showNameOnRanking.showNameOnRanking = Auth.instance.GetUser().showNameOnRanking ? 0 : 1;
                request._requestJson = JsonUtility.ToJson(showNameOnRanking);
                var context = SynchronizationContext.Current;
                Api.UpdateShowNameOnRanking(request, (result) => {
                    context.Post((state2) => {
                        Destroy(loadingView.gameObject);
                        AlertViewController.ShowOKConfirmation("更新しました。");
                        Auth.instance.SetShowNameOnRanking(!Auth.instance.GetUser().showNameOnRanking);
                    }, null);
                }, (errorMessage) => {
                    context.Post((state2) => {
                        Debug.Log(errorMessage);
                        Destroy(loadingView.gameObject);
                        AlertViewController.ShowOKConfirmation(errorMessage);
                    }, null);
                });

            });
    }


    private void Register()
    {
        // 別端末で会員登録を行なっている可能性があるため、念の為再確認
        if (Auth.instance.isUserRegistered() == true) { return; }

        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "Register", nextStageType: EStageType.BLANK_PAGE);
    }


    private void ChangePassword()
    {
        // 別端末で会員登録を行なっている可能性があるため、念の為再確認
        if (Auth.instance.isUserRegistered() == false) { return; }

        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "ChangePassword", nextStageType: EStageType.BLANK_PAGE);
    }


    private void ChangeNickname()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "ChangeNickname", nextStageType: EStageType.BLANK_PAGE);
    }


    private void Logout()
    {
        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: "ユーザ登録が完了していない場合、このアカウントにログインできなくなります。本当にログアウトしますか。",
            textColorType: EWindowTextColorType.CAUTION,
            textButtonNo: "いいえ",
            textButtonYes: "はい",
            actionOnTapButtonYes: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide();
                ExecuteSignout();
            });
    }

    private void ExecuteSignout()
    {
        Auth.instance.Signout();
    }


    private void DeleteUser()
    {
        // CHECK: 以下の内容で問題なく動くと思われますが、念のためコメントアウトしています
        // ↓
        //
        // AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
        //     message: "退会すると、このアカウントにログインできなくなります。本当に退会しますか。",
        //     textColorType: EWindowTextColorType.CAUTION,
        //     textButtonNo: "いいえ",
        //     textButtonYes: "はい",
        //     actionOnTapButtonYes: () => {
        //         AppManager.Instance.UICreator.WindowConfirmation.Hide();
        //         ExecuteDeleteUser();
        //     });

        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: "本当に退会しますか。",
            textColorType: EWindowTextColorType.CAUTION,
            textButtonNo: "いいえ",
            textButtonYes: "はい",
            actionOnTapButtonYes: () => {
                ExecuteDeleteUser();
            });
    }


    private void ExecuteDeleteUser() {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("退会処理中 ...", false, () => { });
        DeleteUserRequest request = new DeleteUserRequest();
        var context = SynchronizationContext.Current;
        Api.DeleteUser(request, (result) => {
            context.Post((state2) => {
                Destroy(loadingView.gameObject);
                AlertViewController.ShowOKConfirmation("退会しました。");
                ExecuteSignout();
            }, null);
        }, (errorMessage) => {
            context.Post((state2) => {
                Debug.Log(errorMessage);
                Destroy(loadingView.gameObject);
                AlertViewController.ShowOKConfirmation(errorMessage);
            }, null);
        });
    }


    private void BackToSettings()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "Settings", nextStageType: EStageType.MENU);
    }

    private void HowToCancelSubscription() {

        #if UNITY_IOS 
        var text = "プレミアム会員の解約はiPhone/iPadの「設定」アプリより可能です。解約方法の詳細は「ヘルプ」をご覧ください。";
        #else
        var text = "プレミアム会員の解約はGooglePlayストアの「サブスクリプション」より可能です。解約方法の詳細は「ヘルプ」をご覧ください。";
        #endif
        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: text,
            textColorType: EWindowTextColorType.NORMAL,
            textButtonNo: "いいえ",
            textButtonYes: "はい",
            actionOnTapButtonYes: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide();
                Application.OpenURL("https://ranshogi-production.web.app/help.html");
            });
    }
}

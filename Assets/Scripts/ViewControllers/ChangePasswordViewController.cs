using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using GFramework.Core;


public class ChangePasswordViewController : MonoBehaviour, IBlankPageManager
{
    [SerializeField] TMP_InputField newEmailTF;
    [SerializeField] TMP_InputField newPasswordTF;
    [SerializeField] TMP_InputField updateEmailTF;
    [SerializeField] TMP_InputField updatePasswordTF;
    [SerializeField] Canvas canvas;
    [SerializeField] AlertViewController alertPrefab;
    [SerializeField] LoadingViewController loadingViewPrefab;
    [SerializeField] TextMeshProUGUI CurrentIDText;
    [SerializeField] Image newView;
    [SerializeField] Image updateView;

    private TouchScreenKeyboard keyboard;
    private string currentEmail = "";
    private string currentPassword = "";
    private bool isInputingEmail = false;

    IBlankPageStage _inputNicknameStage = null;


    public void Start()
    {
        currentEmail = Auth.instance.GetUser().email;
        TouchScreenKeyboard.hideInput = true;
        Debug.Log(Auth.instance.auth.CurrentUser.Email);
        ShowUpdate();

        AppManager.Instance.Setup();
        _inputNicknameStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);
        _inputNicknameStage.ShowWithAnimation();
        SetupStatusBar();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    public void Update()
    {
        if (keyboard != null) 
        {
            if (isInputingEmail) {
                AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFEmail", keyboard.text);
                currentEmail = keyboard.text;
            } else { 
                string maskedPassword = "";
                for(int i=0; i < keyboard.text.Length; i++) {
                    maskedPassword += '*';
                }
                AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFPassword", maskedPassword);
                currentPassword = keyboard.text;
            }
            if(keyboard.status == TouchScreenKeyboard.Status.Done) {
                keyboard = null;
            }
        }
    }


    private void SetupStatusBar()
    {
        AppManager.Instance.ShowStatusBar();
    }



    // ===============================================
    // MARK: - ● Show Window Normal ─────

    private void ShowUpdate()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "パスワード変更",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFRegistration",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 1.6f)),
            text: "登録メールアドレスに、パスワード変更用のリンクを送信いたします。\n「メール送信」ボタンを押したのち、メールの到着をお待ちください。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "backButton",
               area: new FWindowElementArea(
                   origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                   width: 0.35f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "戻る",
               isEnabled: true,
               actionOnTap: () => {
                    _inputNicknameStage.HideWithAnimationAndChangeScene(nextSceneName: "Settings", nextStageType: EStageType.BLANK_PAGE);
               });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonSendMail",
               area: new FWindowElementArea(
                   origin: new Vector2(0.425f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                   width: 0.575f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.POSITIVE,
               text: "メール送信",
               isEnabled: true,
               actionOnTap: () => {
                // 念のためダブルタップを防ぐ
                AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonSendMail", value: false);
                UpdatePasswordButtonTapped();
               });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});
    }


    public void UpdatePasswordButtonTapped() {
        var context = SynchronizationContext.Current;
        CustomTokenRequest req = new CustomTokenRequest();
        //LoadingViewController loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        //loadingView.Init("Sending ...", false, () => { });
        Api.CustomToken(req,
            (customToken) => {
                Debug.Log("get custom token succeeded");
                Debug.Log(customToken.token);
                Auth.instance.SigninWithCustomtokenAsync(customToken.token, () => {
                    Debug.Log("login by custom token succeeded");
                    Auth.instance.auth.SendPasswordResetEmailAsync(Auth.instance.auth.CurrentUser.Email).ContinueWith(task =>
                    {
                        context.Post((state) => {
                            if (task.IsCanceled)
                            {
                                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                                AlertViewController.ShowOKConfirmation("メールの送信中にエラーが発生しました。");
                                return;
                            }
                            if (task.IsFaulted)
                            {
                                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                                AlertViewController.ShowOKConfirmation("メールの送信中にエラーが発生しました。");
                                return;
                            }

                            AlertViewController.ShowOKConfirmation("パスワード再設定用のメールを送信しました。");
                        }, null);
                    });


                }, (errorMessage) => { 
                    context.Post((state) => { 
                        AlertViewController.ShowOKConfirmation(errorMessage);
                    }, null);
                });
            }, (errorMessage) => {
                context.Post((state) => { 
                    //Destroy(loadingView.gameObject);
                    AlertViewController.ShowOKConfirmation(errorMessage);
                }, null);
            }
        );
    }


    public void updateCurrentEmailText() {
        CurrentIDText.text = "Current Email: " + Auth.instance.auth.CurrentUser.Email;
    }
    private string errorCodeToMessage(int code) {
        switch (code) {
            case 11:
                return "メールアドレスの形式が正しくありません";
            case 8:
            case 15:
                return "このメールアドレスは使用されています";
            case 23:
                return "パスワードは6文字以上の半角英数字記号にしてください";
            default:
                return "エラーが発生しました。しばらく時間をおいてお試しください。ErrorCode: " + code;
        }
    }

    private string errorCodeToMessage2(string code) {
        switch (code) {
            case "auth/cancelled-popup-request":
            case "auth/popup-closed-by-user":
                return null;
            case "auth/email-already-in-use":
                return "このメールアドレスは使用されています";
            case "auth/invalid-email":
                return "メールアドレスの形式が正しくありません";
            case "auth/user-disabled":
                return "サービスの利用が停止されています";
            case "auth/user-not-found":
                return "メールアドレスまたはパスワードが違います";
            case "auth/user-mismatch":
                return "認証されているユーザーと異なるアカウントが選択されました";
            case "auth/weak-password":
                return "パスワードは6文字以上にしてください";
            case "auth/wrong-password":
                return "メールアドレスまたはパスワードが違います";
            case "auth/popup-blocked":
                return "認証ポップアップがブロックされました。ポップアップブロックをご利用の場合は設定を解除してください";
            case "auth/operation-not-supported-in-this-environment":
            case "auth/auth-domain-config-required":
            case "auth/operation-not-allowed":
            case "auth/unauthorized-domain":
                return "現在この認証方法はご利用頂けません";
            case "auth/requires-recent-login":
                return "認証の有効期限が切れています";
            default:
                return "エラーが発生しました。しばらく時間をおいてお試しください";
        }
    }
}

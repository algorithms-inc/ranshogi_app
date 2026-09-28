using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using GFramework.Core;

public class RegisterViewController : MonoBehaviour, IBlankPageManager
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

    private IBlankPageStage _blankPageStage = null;


    public void Start()
    {
        currentEmail = Auth.instance.GetUser().email;
        TouchScreenKeyboard.hideInput = true;
        Debug.Log(Auth.instance.auth.CurrentUser.Email);
        if(Auth.instance.isUserRegistered()) {
            //newView.gameObject.SetActive(false);
            //updateView.gameObject.SetActive(true);
            //updateCurrentEmailText();
            ShowUpdate();
        } else {
            ShowRegistration();
        }

        AppManager.Instance.Setup();
        _blankPageStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);
        _blankPageStage.ShowWithAnimation();

        AppManager.Instance.ShowStatusBar();
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



    // ===============================================
    // MARK: - ● Show Window Normal ─────

    private void ShowRegistration()
    {
        //currentEmail = "user@example.com";
        //currentPassword = "password";

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "ユーザ登録",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.4f)),
            text: "ユーザ登録することで、複数の端末で同じアカウントにログインすることができます。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFMessageEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.7f),
                endPoint: new Vector2(1f, 0.8f)),
            text: "メールアドレス",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.8f),
                endPoint: new Vector2(1f, 0.97f)),
            actionOnTap: () =>
            {
                isInputingEmail = true;
                keyboard = TouchScreenKeyboard.Open(currentEmail, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "",
            text: currentEmail,
            isPasswordMode: false,
            hasError: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFMessagePassword",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.00f),
                endPoint: new Vector2(1f, 1.10f)),
            text: "パスワード",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFPassword",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.10f),
                endPoint: new Vector2(1f, 1.27f)),
            actionOnTap: () =>
            {
                isInputingEmail = false;
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true);
            },
            explanationText: "",
            text: currentPassword,
            isPasswordMode: true,
            hasError: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonBack",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 0.35f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isEnabled: true,
            actionOnTap: () => {
                _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "Settings", nextStageType: EStageType.BLANK_PAGE);
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonRegister",
            area: new FWindowElementArea(
                origin: new Vector2(0.425f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 0.575f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "登録",
            isEnabled: true,
            actionOnTap: () => {
                SignupButtonTapped();
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });
    }

    private void ShowUpdate()
    {}



    public void SignupButtonTapped()
    {
        var context = SynchronizationContext.Current;
        LoadingViewController loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("登録中...", false, () => { });

        //直近のログインが必要なので、カスタムトークンでログインする。
        CustomTokenRequest req = new CustomTokenRequest();
        Api.CustomToken(req,
            (customToken) => {
                Debug.Log("get custom token succeeded");
                Debug.Log(customToken.token);
                Auth.instance.SigninWithCustomtokenAsync(customToken.token, () => {
                    context.Post((state) =>
                    {
                        Debug.Log("login by custom token succeeded");
                        Firebase.Auth.Credential credential;
                        try {
                            credential = Firebase.Auth.EmailAuthProvider.GetCredential(currentEmail, currentPassword);
                        } catch {
                            Destroy(loadingView.gameObject);
                            AlertViewController.ShowOKConfirmation("登録に失敗しました。入力を確認してください。");
                            return;
                        }
                        Auth.instance.auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
                            if (task.IsCanceled) {
                                Debug.LogError("LinkWithCredentialAsync was canceled.");
                            } else if (task.IsFaulted) {
                                context.Post((state) => { 
                                    Destroy(loadingView.gameObject);
                                    //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                                    Firebase.FirebaseException error = task.Exception.InnerExceptions[0] as Firebase.FirebaseException;
                                    Debug.Log(error.ErrorCode);
                                    Debug.Log(error.ToString());

                                    AlertViewController.ShowOKConfirmation(errorCodeToMessage(error.ErrorCode));

                                    //alertView.Init("" + error.ToString());
                                }, null);
                            } else {
                                Firebase.Auth.FirebaseUser newUser = task.Result.User;
                                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1}) {2}",
                                    newUser.DisplayName, newUser.UserId, newUser.IsEmailVerified);

                                newUser.SendEmailVerificationAsync().ContinueWith(task => {
                                    context.Post((state) => { 
                                        if (task.IsCanceled)
                                        {
                                            Debug.LogError("SendEmailVerificationAsync was canceled.");
                                            return;
                                        }
                                        if (task.IsFaulted)
                                        {
                                            Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                                            return;
                                        }

                                        Debug.Log("Email sent successfully.");
                                        Destroy(loadingView.gameObject);
                                        //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                                        //alertView.Init("ご入力されたメールアドレスに認証用メールを送信しました。");
                                        AlertViewController.ShowOKConfirmation("ご入力されたメールアドレスに認証用メールを送信しました。");
                                    }, null);
                                });
                            }
                        });


                    }, null);
                }, (errorMessage) => { 
                    context.Post((state) => { 
                        Destroy(loadingView.gameObject);
                        AlertViewController.ShowOKConfirmation(errorMessage);
                    }, null);
                });
            }, (errorMessage) => {
                context.Post((state) => { 
                    Destroy(loadingView.gameObject);
                    AlertViewController.ShowOKConfirmation(errorMessage);
                }, null);
            }
        );
        return;
        //Api.PostLoginIDAndPassword(request,
        //    (customToken) => {
         //       Debug.Log("customToken");
           //     Debug.Log(customToken.token);

          //      Auth.instance.FirebaseSignout();
            //    Auth.instance.SigninWithCustomtoken(customToken.token, () => {
              //      context.Post((state) =>
              //      {
                //        Destroy(loadingView.gameObject);
                  //      AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                    //    alertView.Init(successMessage);

 //                   }, null);
   //             }, (errorMessage) => { 
     //               context.Post((state) => { 
       //                 Destroy(loadingView.gameObject);
         //               AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
           //             alertView.Init(errorMessage);
             //       }, null);
 //               });
   //         }, (errorMessage) => {
     //           Debug.Log(errorMessage);
       //         //EditorUtility.DisplayDialog("エラー", errorMessage, "OK", null);
         //       context.Post((state) => { 
           //         Destroy(loadingView.gameObject);
             //       AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
 //                   alertView.Init(errorMessage);
   //             }, null);
     //   });
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

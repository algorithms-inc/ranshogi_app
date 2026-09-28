using UnityEngine;
using TMPro;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using GFramework.Core;
using System;
using Unity.Collections;


public class InputNicknameViewController : MonoBehaviour, IBlankPageManager
{
    [SerializeField] TMP_InputField nicknameIF;
    [SerializeField] Canvas canvas;
    [SerializeField] LoadingViewController loadingViewPrefab;
    [SerializeField] TMP_InputField idIF;
    [SerializeField] TMP_InputField passwordIF;
    //[SerializeField] AlertViewController alertPrefab;

    private TouchScreenKeyboard keyboard;
    private string currentText = "";
    private bool agreed = false;

    private IBlankPageStage _inputNicknameStage = null;


    void Start()
    {
        //NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
        AppManager.Instance.Setup();
        _inputNicknameStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);

        _inputNicknameStage.ShowWithAnimation();
        SetupStatusBar();
        ShowWindow();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    void Update() {
        if (keyboard != null) 
        {
            AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFNickname", keyboard.text);
            currentText = keyboard.text;
            if(keyboard.status == TouchScreenKeyboard.Status.Done) {
                keyboard = null;
            }
        }
    }


    private void SetupStatusBar()
    {
        AppManager.Instance.SetLife(count: 3);
        AppManager.Instance.SetPlayerName(playerName: "---");
        AppManager.Instance.SetRate(rate: 1500);

        AppManager.Instance.ShowStatusBar();
    }



    // ===============================================
    // MARK: - ● Show Window Normal ─────

    private void ShowWindow()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "初めてご利用の方",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFNickname",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.4f)),
            text: "対局で表示されるニックネームをご入力ください。",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        #if UNITY_EDITOR || UNITY_STANDALONE
        string defaultText = "user" + UnityEngine.Random.Range(1, 10000);
        #else
        string defaultText = "";
        #endif

        currentText = defaultText;

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFNickname",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.42f),
                endPoint: new Vector2(1f, 0.57f)),
            actionOnTap: () =>
            {
                TouchScreenKeyboard.hideInput = true;
                keyboard = TouchScreenKeyboard.Open(currentText, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "半角英数字４文字以上",
            text: defaultText,
            isPasswordMode: false,
            hasError: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
          name: "checkBoxTermPrivacy",
          area: new FWindowElementArea(
              origin: new Vector2(0.04f, 0.67f),
              width: 0.12f,
              height: 0.12f),
          actionOnTap: () => { agreed = !agreed; },
          isOn: false,
          hasError: false,
          isEnabled: true,
          companionTextFieldSetting: null);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFPrivacyPolicy",
            area: new FWindowElementArea(
                origin: new Vector2(0.22f, 0.63f),
                endPoint: new Vector2(1f, 0.73f)),
            actionOnTap: () => {
                Application.OpenURL("https://ranshogi-production.web.app/privacy_policy.html");
            },
            text: "プライバシーポリシー",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.LINK,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFTo",
            area: new FWindowElementArea(
                origin: new Vector2(0.88f, 0.63f),
                endPoint: new Vector2(1f, 0.73f)),
            text: "と",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFTerm",
            area: new FWindowElementArea(
                origin: new Vector2(0.22f, 0.73f),
                endPoint: new Vector2(1f, 0.88f)),
            actionOnTap: () => {
                Application.OpenURL("https://ranshogi-production.web.app/terms_of_use.html");
            },
            text: "利用規約",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.LINK,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFAgree",
            area: new FWindowElementArea(
                origin: new Vector2(0.5f, 0.73f),
                endPoint: new Vector2(1f, 0.88f)),
            text: "に同意する",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonRegister",
               area: new FWindowElementArea(
                   origin: new Vector2(0f, 0.88f),
                   width: 1f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight() * 1.15f),
               buttonDesignType: EUIButtonDesignType.POSITIVE,
               text: "登録する",
               isEnabled: true,
               actionOnTap: () => {
                    SubmitButtonTapped();
               });

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.2f),
                endPoint: new Vector2(1f, 1.205f)),
            filePath: "UI/ImageBox/imagebox_line_a.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "SignInDesc",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.275f),
                endPoint: new Vector2(1f, 1.375f)),
            actionOnTap: () => { },
            text: "ユーザ登録済みの方",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonLogin",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 1f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "ログインへ",
            isEnabled: true,
            actionOnTap: () => {
                _inputNicknameStage.HideWithAnimationAndChangeScene(nextSceneName: "SignIn", nextStageType: EStageType.BLANK_PAGE);
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});
    }


    public void SubmitButtonTapped()
    {
        if(!agreed) { 
            AlertViewController.ShowOKConfirmation("利用規約・プライバシーポリシーに同意してください。");
            //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
            //alertView.Init("利用規約・プライバシーポリシーに同意してください。");
            AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled("buttonRegister", true);
            return;
        }
        LoadingViewController loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("", false, () => { });
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled("buttonRegister", false);
        NicknameRequest nicknameRequest = new NicknameRequest();
        NicknameRequest.Nickname nickname = new NicknameRequest.Nickname();
        nickname.nickname = currentText;
        nicknameRequest._requestJson = JsonUtility.ToJson(nickname);

        var context = SynchronizationContext.Current;
        Api.PostNickname(nicknameRequest,
            (customToken) =>
            {
                Auth.instance.SigninWithCustomtokenAsync(customToken.token, () =>
                {
                    context.Post((state) =>
                    {
                        Destroy(loadingView.gameObject);
                        _inputNicknameStage.HideWithAnimationAndChangeScene(nextSceneName: "Menu", nextStageType: EStageType.MENU);
                    }, null);
                }, (errorMessage) => { });
            }, (errorMessage) =>
            {
                context.Post((state) => {
                    Destroy(loadingView.gameObject);
                    AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled("buttonRegister", true);
                    //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                    //alertView.Init(errorMessage);
                    AlertViewController.ShowOKConfirmation(errorMessage);
                    Debug.Log(errorMessage);
                }, null);
            });
    }

    private static string SALT = "ChIcKeN_CuRrY";


    private static string ToHex(byte[] bytes, bool upperCase)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);
        for (int i = 0; i < bytes.Length; i++)
            result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
        return result.ToString();
    }

    private static string SHA256HexHashString(string StringIn)
    {
        string hashString;
        using (var sha256 = SHA256Managed.Create())
        {
            var hash = sha256.ComputeHash(Encoding.Default.GetBytes(StringIn));
            hashString = ToHex(hash, false);
        }

        return hashString;
    }


    private string generateHashedEmail(string loginId)
    {
        return SHA256HexHashString(InputNicknameViewController.SALT + loginId).Substring(0, 30) + "@example.com";
    }
}

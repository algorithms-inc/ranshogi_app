using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;
using TMPro;
using System.Threading;

public class SignInViewController : MonoBehaviour, IBlankPageManager
{
    private TouchScreenKeyboard keyboard;
    private string currentEmail = "";
    private string currentPassword = "";
    [SerializeField] LoadingViewController loadingViewPrefab;
    [SerializeField] TMP_InputField idIF;
    [SerializeField] TMP_InputField passwordIF;
    [SerializeField] Canvas canvas;

    private bool isEmailInput = true;

    private IBlankPageStage _blankPageStage = null;


    void Start()
    {
        AppManager.Instance.Setup();
        _blankPageStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);
        _blankPageStage.ShowWithAnimation();

        ShowWindow();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    void Update()
    {
             if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            if(isEmailInput) {
                AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFEmail", keyboard.text);
                currentEmail = keyboard.text;
            } else {
                AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFPassword", keyboard.text);
                currentPassword = keyboard.text;
            }
        }
   
    }


    private void ShowWindow()
    {
        //currentEmail = "user@example.com";
        //currentPassword = "password";
        #if UNITY_EDITOR || UNITY_STANDALONE
        currentEmail = "user@example.com";
        currentPassword = "password";
        #endif


        
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "ログイン",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.30f)),
            text: "メールアドレス",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.30f),
                endPoint: new Vector2(1f, 0.47f)),
            actionOnTap: () =>
            {
                isEmailInput = true;
                keyboard = TouchScreenKeyboard.Open(currentEmail, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "",
            text: currentEmail,
            isPasswordMode: false,
            hasError: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFPassword",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.55f),
                endPoint: new Vector2(1f, 0.65f)),
            text: "パスワード",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFPassword",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.65f),
                endPoint: new Vector2(1f, 0.82f)),
            actionOnTap: () =>
            {
                isEmailInput = false;
                keyboard = TouchScreenKeyboard.Open(currentPassword, TouchScreenKeyboardType.Default, false, false, true);
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
                   _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "InputNickname", nextStageType: EStageType.BLANK_PAGE);
               });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonLogin",
               area: new FWindowElementArea(
                   origin: new Vector2(0.425f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                   width: 0.575f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.POSITIVE,
               text: "ログイン",
               isEnabled: true,
               actionOnTap: () => {
                    SigninButtonTapped();
               });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});
    }


    public void SigninButtonTapped() {
        string id = currentEmail;
        string password = currentPassword;
        var context = SynchronizationContext.Current;
        LoadingViewController loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("ログイン中...", false, () => { });

        Auth.instance.SigninWithEmailAndPassword(id, password,
            () =>
            {
                context.Post((state) =>
                {
                    Destroy(loadingView.gameObject);
                    _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "Menu", nextStageType: EStageType.MENU);
                }, null);
            },
            (errorMessage) =>
            {
                context.Post((state) =>
                {
                    Destroy(loadingView.gameObject);
                    AlertViewController.ShowOKConfirmation(errorMessage);
                }, null);
            });
    }
}

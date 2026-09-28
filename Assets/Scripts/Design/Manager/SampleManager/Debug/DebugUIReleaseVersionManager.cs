using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// こちらの using を挿入する必要あり
using GFramework.Core;
using System;

// ===============================================
// MARK: - ● DebugUIReleaseVersionManager ─────

public class DebugUIReleaseVersionManager : MonoBehaviour, IGeneralButtonListManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // GeneralButtonListStage への参照
    private IGeneralButtonListStage _generalButtonListStage = null;


    private string currentEmail = "";
    private string currentPassword = "";

    //private string currentText = "";
    private string defaultText = "";


    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        AppManager.Instance.Setup();

        SetupStage();

        SetupStatusBar();
    }


    private void SetupStage()
    {
        var buttonTextList = new List<string> {
            "InputNicknameVC",  // 0
            "IntroductionVC",   // 1
            "MypageVC",         // 2
            "RegisterVC",       // 3
            "RegistrationReminderVC",  // 4
            "SigninVC",         // 5
            "ChangeEmailVC",    // 6
            "ChangeNicknameVC", // 7
            "ChangePasswordVC", // 8
            "Debug Menuに戻る" }; // 9


        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "Debug Menu",
            buttonTextList: buttonTextList);

        _generalButtonListStage.ShowWithAnimation();
    }


    private void SetupStatusBar()
    {
        AppManager.Instance.SetLife(count: 3);
        AppManager.Instance.SetPlayerName(playerName: "Taro");
        AppManager.Instance.SetRate(rate: 1234);

        AppManager.Instance.ShowStatusBar();
    }



    // ===============================================
    // MARK: - ● On Event ─────

    public void OnDidShowAnimation()
    {
        Debug.Log(GetType().ToString() + ": OnDidShowAnimation");
    }


    #region InputNicknameVC
    // InputNicknameVC
    public void OnTapButton0()
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

        string defaultText = "user" + UnityEngine.Random.Range(1, 10000);
        //currentText = defaultText;

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFNickname",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.42f),
                endPoint: new Vector2(1f, 0.57f)),
            actionOnTap: () =>
            {
                // keyboard = TouchScreenKeyboard.Open(currentText, TouchScreenKeyboardType.Default, false, false, false);
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
          actionOnTap: () => {  },
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
                Application.OpenURL("https://ranshogi-staging.web.app/privacy_policy.html");
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
                Application.OpenURL("https://ranshogi-staging.web.app/terms_of_use.html");
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
                    // SubmitButtonTapped();
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
                // nextStage = "SignIn";
                // AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: () => {});
                // _inputNicknameStage.HideWithAnimation(nextStageType: EStageType.BLANK_PAGE);
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion

    #region IntroductionVC
    // IntroductionVC
    public void OnTapButton1()
    {
        int _pageIndexOfContentsForBeginner = 0;
        int _pageMaxIndexOfContentsForBeginner = 2;

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldTitle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 1.6f)),
            text: "乱将棋にようこそ",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldExplanation",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.175f),
                endPoint: new Vector2(1f, 1.6f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxMain",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.7f),
                endPoint: new Vector2(1f, 1.35f)),
            filePath: "UI/ImageBox/imagebox_dummy_a.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonPrevious",
               area: new FWindowElementArea(
                   origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                   width: 0.35f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "戻る",
               isEnabled: true,
               actionOnTap: () => {
                   OnTapButtonBack();
               });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonNext",
               area: new FWindowElementArea(
                   origin: new Vector2(0.425f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                   width: 0.575f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.POSITIVE,
               text: "次へ",
               isEnabled: true,
               actionOnTap: () => {
                   OnTapButtonNext();
               });

        UpdatePageContentsForBeginner();

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);



        void OnTapButtonBack()
        {
            if (_pageIndexOfContentsForBeginner == 0) { return; }

            _pageIndexOfContentsForBeginner -= 1;

            UpdatePageContentsForBeginner();
        }

        void OnTapButtonNext()
        {
            if (_pageIndexOfContentsForBeginner == _pageMaxIndexOfContentsForBeginner)
            {
                AppManager.Instance.UICreator.WindowNormal.RemoveAllUIParts();
                // ニックネーム登録・ログイン選択画面へ遷移
                OnTapButtonNextStage();
                return;
            }

            _pageIndexOfContentsForBeginner += 1;

            UpdatePageContentsForBeginner();
        }

        void UpdatePageContentsForBeginner()
        {
            switch (_pageIndexOfContentsForBeginner)
            {
                case 0:
                    Debug.Log("UpdatePageContentsForBeginner: 0");
                    AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonPrevious", value: false);
                    AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldExplanation", text: "乱将棋は将棋をベースとしたスピーディーなオンライン対戦ゲームです。");
                    AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBoxMain", filePath: "Stage/InputNicknameStage/guide_01.png");
                    break;

                case 1:
                    Debug.Log("UpdatePageContentsForBeginner: 1");
                    AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonPrevious", value: true);
                    AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldExplanation", text: "マス目は６×６。AIが生成した互角の局面から始まります。駒が成れるのは、奥の２段の中です。");
                    AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBoxMain", filePath: "Stage/InputNicknameStage/guide_02.png");
                    break;

                case 2:
                    Debug.Log("UpdatePageContentsForBeginner: 2");
                    AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldExplanation", text: "「詰み・投了・持ち時間切れ」に加えて「トライ」があり、王を一番奥の段に進めても勝ちとなります。");
                    AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBoxMain", filePath: "Stage/InputNicknameStage/guide_03.png");
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        // NextStageボタンを押下
        void OnTapButtonNextStage()
        {
            // Debug.Log(this.GetType().ToString() + ": OnTapButtonGame");

            // _nextStageName = "InputNickname";
            // _blankPageStage.HideWithAnimation(nextStageType: EStageType.MENU);
        }


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion

    #region MypageVC
    public void OnTapButton2()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF1",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "マイページ",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF2",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.15f),
                endPoint: new Vector2(1f, 0.25f)),
            text: "ニックネーム",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF3",
            area: new FWindowElementArea(
                origin: new Vector2(0.05f, 0.27f),
                endPoint: new Vector2(1f, 0.37f)),
            text: "Dummy", //Auth.instance.GetUser().nickname,
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF4",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.40f),
                endPoint: new Vector2(1f, 0.50f)),
            text: "登録メールアドレス",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        // メールアドレスが認証済みかどうか
        var isAutholicated = false;

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF5",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.40f),
                endPoint: new Vector2(1f, 0.50f)),
            text: isAutholicated ? "" : "未認証",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.WARNING,
            alignment: EAlignment.TOP_RIGHT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF6",
            area: new FWindowElementArea(
                origin: new Vector2(0.05f, 0.52f),
                endPoint: new Vector2(1f, 0.62f)),
            text: "Dummy",//Auth.instance.GetUser().email,
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF7",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.65f),
                endPoint: new Vector2(1f, 0.75f)),
            text: "レート： " + "123",//"" + Auth.instance.GetUser().rating,
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        // 勝率計算
        float winCount = 0; //Auth.instance.GetUser().nWin
        float loseCount = 0; //Auth.instance.GetUser().nLose
        float matchCount = winCount + loseCount;
        float winRate = 0f;
        if (matchCount > 0) { winRate = winCount / matchCount * 100; }
        string textWinRate = winRate.ToString("F2") + "%";

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF8",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.78f),
                endPoint: new Vector2(1f, 0.88f)),
            text: "勝敗： " + winCount.ToString() + "勝 " + loseCount.ToString() + "敗",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT); 

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TF9",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.91f),
                endPoint: new Vector2(1f, 1.01f)),
            text: "勝率： " + textWinRate,
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT); 

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonHistory",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.20f),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "対局履歴",
            isEnabled: true,
            actionOnTap: () => {
                //HistoryButtonTapped();
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "backButton",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isEnabled: true,
            actionOnTap: () => {
                // BackButtonTapped();
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion

    #region RegisterVC
    // RegisterVC
    public void OnTapButton3()
    {
        currentEmail = "user@example.com";
        currentPassword = "password";

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
                // isInputingEmail = true;
                // keyboard = TouchScreenKeyboard.Open(currentEmail, TouchScreenKeyboardType.Default, false, false, false);
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
                // isInputingEmail = false;
                // keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true);
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
                // BackButtonTapped();
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
            // SignupButtonTapped();
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion

    #region RegistrationReminderVC
    // RegistrationReminderVC
    public void OnTapButton4()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "ご注意ください",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.8f)),
            text: "このまま対局をお楽しみいただけますが、お使いの携帯端末を買い替えるなどしてアプリがリセットされた場合、再びログインすることはできません。\nメニューの「設定」よりメールアドレスとパスワードを登録されることをお勧めいたします。",
            fontScale: 1f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonToMenu",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "メニューへ",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: () => {
                    SceneManager.LoadScene("Menu");
                });
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion
    
    #region SigninVC
    // SigninVC
    public void OnTapButton5()
    {
        currentEmail = "user@example.com";
        currentPassword = "password";

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
                // isEmailInput = true;
                // keyboard = TouchScreenKeyboard.Open(currentEmail, TouchScreenKeyboardType.Default, false, false, false);
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
                // isEmailInput = false;
                // keyboard = TouchScreenKeyboard.Open(currentPassword, TouchScreenKeyboardType.Default, false, false, true);
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
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: () => {
                    SceneManager.LoadScene("InputNickname");
                });
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
                // SigninButtonTapped();
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion

    #region ChangeEmailVC
    // ChangeEmailVC
    public void OnTapButton6()
    {
        currentEmail = "user@example.com";

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "メールアドレス変更",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFRegistration",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.33f)),
            text: "現在の登録メールアドレス",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFCurrentEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0.05f, 0.33f),
                endPoint: new Vector2(1f, 0.43f)),
            text: "dummy@email.com",//Auth.instance.auth.CurrentUser.Email,
            fontScale: 0.8f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFCurrentVerified",
            area: new FWindowElementArea(
                origin: new Vector2(0.05f, 0.43f),
                endPoint: new Vector2(1f, 0.53f)),
            text: "認証が完了していません",//Auth.instance.auth.CurrentUser.IsEmailVerified ? "認証済み" : "認証が完了していません",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.6f),
                endPoint: new Vector2(1f, 0.7f)),
            text: "新しいメールアドレス",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.73f),
                endPoint: new Vector2(1f, 0.88f)),
            actionOnTap: () =>
            {
                // isInputingEmail = true;
                // keyboard = TouchScreenKeyboard.Open(currentEmail, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "メールアドレス",
            text: currentEmail,
            isPasswordMode: false,
            hasError: false,
            isEnabled: true);

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
            // BackButtonTapped();
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonUpdateEmail",
            area: new FWindowElementArea(
            origin: new Vector2(0.425f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
            width: 0.575f,
            height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "変更",
            isEnabled: true,
            actionOnTap: () => {
            //    UpdateEmailButtonTapped();
            });

        if (true)
        {
            AppManager.Instance.UICreator.WindowNormal.TextField.ChangeTextColor(name: "TFCurrentVerified", textColorType: EWindowTextColorType.WARNING);
        }


        Debug.Log("####here");
        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion

    #region ChangeNicknameVC
    // ChangeNicknameVC
    public void OnTapButton7()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "ニックネーム変更",
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
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        // //string defaultText = Auth.instance.GetUser().nickname;
        // string defaultText = "moriteru2";
        // //string debugText = "";
        // currentText = defaultText;

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFNickname",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.55f),
                endPoint: new Vector2(1f, 0.72f)),
            actionOnTap: () =>
            {
                // keyboard = TouchScreenKeyboard.Open(currentText, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "半角英数字４文字以上",
            text: defaultText,
            isPasswordMode: false,
            hasError: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFRistriction",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.75f),
                endPoint: new Vector2(1f, 0.8f)),
            text: "半角英数字４文字以上",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT);

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
                // BackButtonTapped();
               });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonChange",
               area: new FWindowElementArea(
                    origin: new Vector2(0.425f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                    width: 0.575f,
                    height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.POSITIVE,
               text: "変更",
               isEnabled: true,
               actionOnTap: () => {
                // updateNickname();
               });

        Debug.Log("####here");
        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });



        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion

    #region ChangePasswordVC
    // ChangePasswordVC
    public void OnTapButton8()
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
                //    BackButtonTapped();
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
                //    UpdatePasswordButtonTapped();
               });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });


        // デバッグ用に閉じるボタンを追加
        AddButtonWindowClose();
    }
    #endregion



    public void OnTapButton9()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugMenuStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }


    private void AddButtonWindowClose()
    {
        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonDebugClose",
            area: new FWindowElementArea(
                origin: new Vector2(0.95f, -0.35f),
                width: 0.15f,
                height: 0.15f),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "×",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: () => {});
            });
    }

   

    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        _generalButtonListStage = null;
    }
}
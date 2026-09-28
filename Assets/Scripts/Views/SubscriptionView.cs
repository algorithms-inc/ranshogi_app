using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading;
using TMPro;
using GFramework.Core;
using UnityEngine.Purchasing;

public class SubscriptionView : MonoBehaviour {

    private TouchScreenKeyboard keyboard;
    private string currentEmail = "";
    private string currentPassword = "";
    private bool isInputingEmail = false;
    private RewardedAdManager adManager;
    [SerializeField] Canvas canvas;
    [SerializeField] LoadingViewController loadingViewPrefab;
    private LoadingViewController loadingView;

    public delegate void ButtonTapped();
    public delegate void RegisterButtonTapped(string email, string password);

    UnityAction onUserEarnedReward;

    string viewName = "";
    public bool isInSettings = false;

    public void Start()
    {
        adManager = FindObjectOfType<RewardedAdManager>();
        var iapManager = FindObjectOfType<IAPManager>();
        iapManager.SetHandlers(OnPurchaseSuccess, OnRestoreSuccess, OnPurchaseFailed);
    }

    public void Update()
    {
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            if (isInputingEmail) {
                if(viewName != "register" && viewName != "updateEmail") { return; }
                AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFEmail", keyboard.text);
                currentEmail = keyboard.text;
            } else { 
                if(viewName != "register") { return; }
                string maskedPassword = "";
                for(int i=0; i < keyboard.text.Length; i++) {
                    maskedPassword += '*';
                }
                AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFPassword", maskedPassword);
                currentPassword = keyboard.text;
            }
        }
    }

    public void SetOnUserEarnedReward(UnityAction onUserEarnedReward) {
        this.onUserEarnedReward = onUserEarnedReward;
    }

    /// <param name="limitMessage">上に出す制限の説明。null なら対局回数の制限の説明</param>
    /// <param name="unlimitedFeatures">プレミアム会員の説明で「広告なしで〜を無制限に使えます」に入れる言葉</param>
    public void ShowAdOrBuySubscriptionConfirmation(string limitMessage = null, string unlimitedFeatures = "対局とAI機能"){
        viewName = "ad_or_sub";
        isInSettings = false;
        // 広告を1回見ると、本日中は対局回数とAI機能(AIと対戦・AI評価値)の制限が両方なくなる
        limitMessage ??= "対局は1日3回までです。広告を見ると本日中は対局とAI機能が無制限になります。";
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1.0f, 0.4f)),
            text: limitMessage,
            // 長い説明は枠に収まるよう少し小さくする
            fontScale: limitMessage.Length > 40 ? 0.85f : 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);
        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonYES",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.405f),
                endPoint: new Vector2(1.0f, 0.58f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "広告を見る",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // WindowNormalを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> {
                    LoadAd();
                });
            });
        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.63f),
                endPoint: new Vector2(1f, 0.635f)),
            filePath: "UI/ImageBox/imagebox_line_a.png",
            actionOnTap: null);

        var iapManager = FindObjectOfType<IAPManager>();
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldDesc",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.68f),
                endPoint: new Vector2(1.0f, 1.17f)),
            text: "プレミアム会員（1ヶ月" + iapManager.GetPrice() + "・自動継続）となることで、広告なしで" + unlimitedFeatures + "を無制限に使えます。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);
        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonBuy",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.170f),
                endPoint: new Vector2(1.0f, 1.345f)),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "プレミアム会員になる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                // WindowNormalを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    if(Auth.instance.IsUserVerified()) {
                        ShowPremiumConfirmation();
                    } else if(Auth.instance.isUserRegistered()) {
                        ShowWaitingEmailConfirmationView();
                    } else {
                        ShowRegistrationView();
                    }

                });
            });
        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxB",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.390f),
                endPoint: new Vector2(1f, 1.395f)),
            filePath: "UI/ImageBox/imagebox_line_a.png",
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonDelete",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.430f),
                endPoint: new Vector2(1.0f, 1.605f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                // WindowNormalを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { });
            });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    public void ShowBuySubscriptionConfirmation(){
        viewName = "sub";
        var iapManager = FindObjectOfType<IAPManager>();
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldDesc",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.0f),
                endPoint: new Vector2(1.0f, 0.37f)),
            text: "プレミアム会員（1ヶ月" + iapManager.GetPrice() + "・自動継続）となることで、広告なしの無制限で対局できます。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);
        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonBuy",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.225f),
                endPoint: new Vector2(1.0f, 1.400f)),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "プレミアム会員になる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                // WindowNormalを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    CheckRegister();
                });
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonDelete",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.430f),
                endPoint: new Vector2(1.0f, 1.605f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                // WindowNormalを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { });
            });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }

    public void ShowPremiumConfirmation(){
        viewName = "premiumconfirm";
        var iapManager = FindObjectOfType<IAPManager>();
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldDesc",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.0f),
                endPoint: new Vector2(1.0f, 0.15f)),
            text: "プレミアム会員のご登録",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldPremium",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.15f),
                endPoint: new Vector2(1.0f, 0.33f)),
            text: "・プレミアム会員となることで広告なしで無制限に対局できます。",
            fontScale: 0.7f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldAuto",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.33f),
                endPoint: new Vector2(1.0f, 0.51f)),
            text: "・この定期購読は自動継続です。1ヶ月ごとに" + iapManager.GetPrice() + "の料金が請求されます。",
            fontScale: 0.7f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldCancel",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.51f),
                endPoint: new Vector2(1.0f, 0.69f)),
            text: "・解約方法に関しては「ヘルプ」をご覧ください。",
            fontScale: 0.7f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldRestore",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.69f),
                endPoint: new Vector2(1.0f, 0.95f)),
            text: "・機種変更時は購読登録時の乱将棋アカウントでログインすることで定期購読の状態を復元できます。",
            fontScale: 0.7f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldTerm",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.95f),
                endPoint: new Vector2(1.0f, 1.05f)),
            actionOnTap: () => {
                Application.OpenURL("https://ranshogi-production.web.app/terms_of_use.html");
            },
            text: "・利用規約",
            fontScale: 0.7f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.LINK,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldPP",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.05f),
                endPoint: new Vector2(1.0f, 1.15f)),
            actionOnTap: () => {
                Application.OpenURL("https://ranshogi-production.web.app/privacy_policy.html");
            },
            text: "・プライバシーポリシー",
            fontScale: 0.7f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.LINK,
            alignment: EAlignment.TOP_LEFT);
        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonBuy",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.23f),
                endPoint: new Vector2(1.0f, 1.405f)),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "購読する (" + iapManager.GetPrice() + "/月)",
                  
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                // WindowNormalを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
                    loadingView.Init("プレミアム会員登録中...", false, () => { });
                    iapManager.BuySubscription();

                });
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonDelete",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.430f),
                endPoint: new Vector2(1.0f, 1.605f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                // WindowNormalを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    if(isInSettings) {
                        if(Auth.instance.IsUserVerified()) {
                            // just close window
                        } else if(Auth.instance.isUserRegistered()) {
                            // can't happen
                        } else {
                            ShowRegistrationView();
                        }
                    } else {
                        if(Auth.instance.IsUserVerified()) {
                            // just close window
                            ShowAdOrBuySubscriptionConfirmation();
                        } else if(Auth.instance.isUserRegistered()) {
                            // can't happen
                            ShowAdOrBuySubscriptionConfirmation();
                        } else {
                            ShowRegistrationView();
                        }
                    }
                });
            });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }

    public static void ShowBuySubscriptionConfirmation(ButtonTapped handler = null) {
    }

    public void ShowRegistrationView() {
        viewName = "register";

        #if UNITY_EDITOR || UNITY_STANDALONE
        currentEmail = "user+rt" + UnityEngine.Random.Range(1, 10000) + "@example.com";
        currentPassword = "password";
        #endif

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
            text: "機種変更時のプレミアム会員状態の復元のため、プレミアム会員となる前にユーザ登録をお願いします。",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFMessageEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.6f),
                endPoint: new Vector2(1f, 0.7f)),
            text: "メールアドレス",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.7f),
                endPoint: new Vector2(1f, 0.87f)),
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
                origin: new Vector2(0f, 0.90f),
                endPoint: new Vector2(1f, 1.00f)),
            text: "パスワード",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFPassword",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.00f),
                endPoint: new Vector2(1f, 1.17f)),
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
                origin: new Vector2(0f, 1.45f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 0.35f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    if(isInSettings) {
                    } else {
                        ShowAdOrBuySubscriptionConfirmation();
                    }
                });
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonRegister",
            area: new FWindowElementArea(
                origin: new Vector2(0.425f, 1.45f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 0.575f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "登録",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    Register(currentEmail, currentPassword);
                });
                //SignupButtonTapped();
            });

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldTerm",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 1.52f),
                endPoint: new Vector2(1.0f, 1.62f)),
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    ShowPremiumConfirmation();
                });
            },
            text: "スキップする",
            fontScale: 0.7f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.LINK,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });
    }

    public void OnPurchaseSuccess() {
        var context = SynchronizationContext.Current;
        context.Post((state) => {
            Destroy(loadingView.gameObject);
            Auth.instance.SetUserPayed();
            AppManager.Instance.SetIsPremiumUser(true);
            AlertViewController.ShowOKConfirmation("プレミアム会員登録（1ヶ月300円・自動継続）が完了しました。ご注意事項はヘルプにてご確認ください。");
        }, null);
    }

    public void OnRestoreSuccess(Product product) {
        var context = SynchronizationContext.Current;
        context.Post((state) => {

            if(loadingView != null) {
                Destroy(loadingView.gameObject);
            }
            if(!Auth.instance.GetUser().isPayedUser) {
                TryRestore(product);
            }
        }, null);
    }

    public void TryRestore(Product product) {
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
                iapManager.DoRestore(product, DoRestoreSuccess, RestoreFailed);
            }
        );
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

    public void OnPurchaseFailed(string errorMessage) {
        var context = SynchronizationContext.Current;
        context.Post((state) => {
            Destroy(loadingView.gameObject);
            AlertViewController.ShowOKConfirmation(errorMessage);
        }, null);
    }

    public void ShowWaitingEmailConfirmationView() {
        viewName = "confirmation";

        var context = SynchronizationContext.Current;
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "メールアドレスのご確認",
            fontScale: 1.10f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.6f)),
            text: "確認メール本文に記載されているURLをクリックした後、下記ボタンを押してください。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonRegister",
            area: new FWindowElementArea(
                origin: new Vector2(0, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight() * 4 - 0.1f),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "プレミアム会員登録する",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    CheckConfirmation((isUserVerified) => {
                        if(isUserVerified) {
                            context.Post((state) => {
                                ShowPremiumConfirmation();
                            }, null);
                        } else {
                            AlertViewController.ShowOKConfirmation("メールアドレスの確認が完了しておりません。");
                            ShowWaitingEmailConfirmationView();
                        }
                    });
                });
                //SignupButtonTapped();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonResend",
            area: new FWindowElementArea(
                origin: new Vector2(0, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()*3),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "確認メールを再送する",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    Resend();
                });
                //SignupButtonTapped();
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonChangeEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()*2),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "メールアドレスを変更",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                    ShowUpdateEmailView(() => {
                        AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                            ShowWaitingEmailConfirmationView();
                        });

                    });
                });
                //SignupButtonTapped();
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "閉じる",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: ()=> { 
                });
                //SignupButtonTapped();
            });
        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });

    }

    public void ShowUpdateEmailView(ButtonTapped backButtonTapped)
    {
        viewName = "updateEmail";
        //currentEmail = "user@example.com";
        //currentPassword = "password";
        #if UNITY_EDITOR || UNITY_STANDALONE
        currentEmail = "user+rt" + UnityEngine.Random.Range(1, 10000) + "@example.com";
        #else
        currentEmail = Auth.instance.auth.CurrentUser.Email;
        #endif


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
            text: Auth.instance.auth.CurrentUser.Email,
            fontScale: 0.8f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFCurrentVerified",
            area: new FWindowElementArea(
                origin: new Vector2(0.05f, 0.43f),
                endPoint: new Vector2(1f, 0.53f)),
            text: Auth.instance.auth.CurrentUser.IsEmailVerified ? "確認済み" : "確認が完了していません",
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
                isInputingEmail = true;
                keyboard = TouchScreenKeyboard.Open(currentEmail, TouchScreenKeyboardType.Default, false, false, false);
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
                backButtonTapped();
            });

        var context = SynchronizationContext.Current;
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
                loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
                loadingView.Init("メールアドレスを更新中...", false, () => { });
                UpdateEmail(() => {
                    context.Post((state) => {
                        EncryptedPlayerPrefs.SetString("email", currentEmail);
                        Destroy(loadingView.gameObject);
                        AlertViewController.ShowOKConfirmation("ご入力されたメールアドレスに確認用メールを送信しました。");
                    }, null);
                }, (errorMessage) => {
                    context.Post((state) => {
                        Destroy(loadingView.gameObject);
                        AlertViewController.ShowOKConfirmation(errorMessage);
                    }, null);

                });
            });

        // 認証がまだである場合テキストの色を変える
        if (Auth.instance.auth.CurrentUser.IsEmailVerified == false)
        {
            AppManager.Instance.UICreator.WindowNormal.TextField.ChangeTextColor(name: "TFCurrentVerified", textColorType: EWindowTextColorType.WARNING);
        }


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });
    }


    private void FuncOnUserEarnedReward() {
        this.onUserEarnedReward();
    }

    private void ShowAd() {
        Destroy(loadingView.gameObject);
        adManager.onUserEarnedReward = FuncOnUserEarnedReward;
        adManager.Show();
    }

    private void FailedToLoadAd() {
        Destroy(loadingView.gameObject);
        AlertViewController.ShowOKConfirmation("広告の読み込みに失敗しました。しばらく時間を置いてからお試しください。");
    }

    public void LoadAd() {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("広告をロード中...", false, () => { });
        adManager.onAdLoaded = ShowAd;
        adManager.onLoadAdFailed = FailedToLoadAd;
        adManager.LoadAd();
    }

    private void BackFromRegister() {
        ShowPremiumConfirmation();
    }

    private void BackFromRegisterInSettings() {
        ShowBuySubscriptionConfirmation();
    }

    private void CheckConfirmation(UnityAction<bool> OnSuccessHandler) {
        var context = SynchronizationContext.Current;
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("ユーザ情報を取得中...", false, () => { });
        Auth.instance.ReloadUserAsync(() => {
            context.Post((state) => {
                Destroy(loadingView.gameObject);
                OnSuccessHandler(Auth.instance.IsUserVerified());
            }, null);
        }, (errorMessage) => {
            context.Post((state) => {
                Destroy(loadingView.gameObject);
                AlertViewController.ShowOKConfirmation(errorMessage);
                ShowWaitingEmailConfirmationView();
            }, null);
        });
    }

    private void Resend() {
        var context = SynchronizationContext.Current;
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("メールを送信中...", false, () => { });
        Auth.instance.SendEmailVerificationAsync(() => {
            context.Post((state) => {
                AlertViewController.ShowOKConfirmation("認証メールを送信しました。");
                Destroy(loadingView.gameObject);
                ShowWaitingEmailConfirmationView();
            }, null);
        }, (errorMessage) => {
            context.Post((state) => {
                Destroy(loadingView.gameObject);
                AlertViewController.ShowOKConfirmation(errorMessage);
                ShowWaitingEmailConfirmationView();
            }, null);
        });
    }

    private void ChangeEmail() {
        ShowRegistrationView();
    }

    // 課金動線での登録
    public void Register(string email, string password) {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("登録中...", false, () => { });
        var context = SynchronizationContext.Current;

        UnityAction<string> errorHandler = (errorMessage) => {
            context.Post((state) => {
                ShowRegistrationView();
                Destroy(loadingView.gameObject);
                AlertViewController.ShowOKConfirmation(errorMessage);
            }, null);
        };

        Auth.instance.RegisterUser(email, password, () => {
            context.Post((state) => {
                Auth.instance.Reauthenticate(email, password, () => {
                    context.Post((state) => {
                        Destroy(loadingView.gameObject);
                        EncryptedPlayerPrefs.SetString("email", email);
                        EncryptedPlayerPrefs.SetString("password", password);
                        ShowWaitingEmailConfirmationView();
                    }, null);
                }, errorHandler);
            }, null);

        }, errorHandler);
    }

    private void CheckRegister() {
        var context = SynchronizationContext.Current;
        if(Auth.instance.IsUserVerified()) {
            loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
            loadingView.Init("プレミアム会員登録中...", false, () => { });
            var iapManager = FindObjectOfType<IAPManager>();
            iapManager.SetHandlers(OnPurchaseSuccess, OnRestoreSuccess, OnPurchaseFailed);
            iapManager.BuySubscription();
        } else if(Auth.instance.isUserRegistered()) {
            ShowWaitingEmailConfirmationView();
        } else {
            ShowRegistrationView();
        }
    }

    private void UpdateEmail(UnityAction onSuccessHandler, UnityAction<string> onErrorHandler) {
        // currentEmail = フォームで入力された「新しいメール」
        string newEmail = currentEmail;

        CustomTokenRequest req = new CustomTokenRequest();
        Api.CustomToken(req,
            (customToken) => {
                Auth.instance.SigninWithCustomtokenAsync(customToken.token, () => {

                    var user = Auth.instance.auth.CurrentUser;
                    if (user == null) {
                        onErrorHandler("ユーザー情報の取得に失敗しました。ErrorCode: UE0");
                        return;
                    }

                    // ★ ここが変更点：
                    // 旧: UpdateEmailAsync(newEmail) → SendEmailVerificationAsync()
                    // 新: SendEmailVerificationBeforeUpdatingEmailAsync(newEmail)
                    user.SendEmailVerificationBeforeUpdatingEmailAsync(newEmail).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            onErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。ErrorCode: UE3");
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            Debug.LogError(task.Exception);
                            onErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。ErrorCode: UE4");
                            return;
                        }

                        Debug.Log("Verification email sent. Email will be updated after verification.");

                        // 注意：この時点ではまだ email は更新されていません。
                        // ユーザーがメール内リンクをクリックして初めて更新されます。
                        onSuccessHandler();
                    });

                }, (errorMessage) => {
                    onErrorHandler(errorMessage);
                });
            }, (errorMessage) => {
                onErrorHandler(errorMessage);
            }
        );
    }


}

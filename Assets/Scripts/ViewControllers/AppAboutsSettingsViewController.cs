using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using GFramework.Core;


public class AppAboutsSettingsViewController : MonoBehaviour, IGeneralButtonListManager
{
    private LoadingViewController loadingView;

    private IGeneralButtonListStage _generalButtonListStage = null;

    private bool _isUserRegistered = false;


    void Start()
    {
        AppManager.Instance.Setup();

        _isUserRegistered = Auth.instance.isUserRegistered();

        SetupStage();
    }


    private void SetupStage()
    {
        List<string> buttonTextList;

        buttonTextList = new List<string> {
            "お問い合わせ",
            "利用規約",
            "プライバシーポリシー",
            "ライセンス",
            "ヘルプ・乱将棋ルール",
            "",
            "",
            "",
            "",
            "戻る",
        };
        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "アプリについて",
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
        switch (buttonIndex)
        {
            case 0:
                Inquiry();
                break;
            case 1:
                TermsOfUse();
                break;
            case 2:
                PrivacyPolicy();
                break;
            case 3:
                License();
                break;
            case 4:
                Help();
                break;
            case 9:
                BackToSettings();
                break;
            default:
                break;
        }
    }



    // ===============================================
    // MARK: - ● Method ─────

    private void Inquiry()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSflVT90aRQ8CPerQnxDQmqevyWUTS2SmGKbwM4LbUANgjvJmw/viewform?usp=sf_link");
    }


    private void TermsOfUse()
    {
        Application.OpenURL("https://ranshogi-production.web.app/terms_of_use.html");
    }


    private void PrivacyPolicy()
    {
        Application.OpenURL("https://ranshogi-production.web.app/privacy_policy.html");
    }

    private void Help()
    {
        Application.OpenURL("https://ranshogi-production.web.app/help.html");
    }

    private void BackToSettings()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "Settings", nextStageType: EStageType.MENU);
    }

    private void License() {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textTitle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "ライセンス表示",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textContent",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.15f),
                endPoint: new Vector2(1f, 0.85f)),
            text: "このアプリにはフォント「Noto Sans Japanese」「Noto Serif Japanese」が含まれており、SIL Open Font License Version 1.1 に基づいて、提供されています。\n\nThis app includes the fonts \"Noto Sans Japanese\" and \"Noto Serif Japanese\" and is provided under the SIL Open Font License version 1.1.\n\n\"Noto Sans Japanese\"\n - Copyright 2014-2021 Adobe\n\n\"Noto Serif Japanese\"\n - Copyright 2012 Google Inc.",
            fontScale: 0.65f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);

        // 思考エンジン(やねうら王の改造版)はGPLv3なので、その旨とソースの入手先を出す
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textEngine",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.88f),
                endPoint: new Vector2(1f, 1.12f)),
            text: "思考エンジンには「やねうら王」を乱将棋向けに改造したものを使用しています。GNU General Public License v3 に基づいて提供されています。",
            fontScale: 0.65f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textEngineSource",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.12f),
                endPoint: new Vector2(1f, 1.28f)),
            actionOnTap: () => {
                Application.OpenURL("https://github.com/algorithms-inc/YaneuraOu_Ranshogi");
            },
            text: "ソースコードはこちら",
            fontScale: 0.65f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.LINK,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.425f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "閉じる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }
}

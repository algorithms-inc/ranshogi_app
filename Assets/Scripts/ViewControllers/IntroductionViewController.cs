using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;
using System;



// ===============================================
// MARK: - ● SampleInputNicknameManager ─────

// 空白ページでは、インターフェースとして IBlankPageManager を設定する必要あり
public class IntroductionViewController : MonoBehaviour, IBlankPageManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // BlankPageStage への参照
    private IBlankPageStage _blankPageStage = null;


    // -----------------------------------------------
    // MARK: - 初心者向け説明のページインデックス

    private int _pageIndexOfContentsForBeginner = 0;

    private int _pageMaxIndexOfContentsForBeginner = 1;


    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        Debug.Log(this.GetType().ToString() + ": Awake()");

        AppManager.Instance.Setup();

        _blankPageStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);

        _blankPageStage.ShowWithAnimation();

        SetupStatusBar();

        SetupWindowContentsForBeginner();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
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

    private void SetupWindowContentsForBeginner()
    {
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
    }


    private void OnTapButtonBack()
    {
        if (_pageIndexOfContentsForBeginner == 0) { return; }

        _pageIndexOfContentsForBeginner -= 1;

        UpdatePageContentsForBeginner();
    }


    private void OnTapButtonNext()
    {
        if (_pageIndexOfContentsForBeginner == _pageMaxIndexOfContentsForBeginner)
        {
            AppManager.Instance.UICreator.WindowNormal.RemoveAllUIParts();

            // ニックネーム登録・ログイン選択画面へ遷移
            _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "InputNickname", nextStageType: EStageType.BLANK_PAGE);
            
            return;
        }

        _pageIndexOfContentsForBeginner += 1;

        UpdatePageContentsForBeginner();
    }


    private void UpdatePageContentsForBeginner()
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
                AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldExplanation", text: "マス目は６×６。AIが生成した互角の局面から始まります。駒が成れるのは、奥の３段の中です。");
                AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBoxMain", filePath: "Stage/InputNicknameStage/guide_02.png");
                break;

            default:
                throw new InvalidOperationException();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// こちらの using を挿入する必要あり
using GFramework.Core;


// ===============================================
// MARK: - ● SampleSettingManager ─────

public class SampleSettingManager : MonoBehaviour, IGeneralButtonListManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // GeneralButtonListStage への参照
    private IGeneralButtonListStage _generalButtonListStage = null;



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
            "ユーザ登録・変更",
            "サウンド設定",
            "ニックネーム変更",
            "ログアウト",
            "退会",
            "盤面設定",
            "",
            "",
            "ライセンス表示",
            "メニューに戻る" };


        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "設定",
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


    public void OnTapButton0()
    {
        Debug.Log(GetType().ToString() + ": OnTapButton0");
    }


    public void OnTapButton1()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugAudioStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }


    public void OnTapButton2()
    {
        Debug.Log(GetType().ToString() + ": OnTapButton2");
    }


    public void OnTapButton3()
    {
        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: "ユーザ登録が完了していない場合、このアカウントにログインできなくなります。本当にログアウトしますか。",
            textColorType: EWindowTextColorType.CAUTION,
            textButtonNo: "いいえ",
            textButtonYes: "はい",
            actionOnTapButtonYes: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide();
            });

        Debug.Log(GetType().ToString() + ": OnTapButton3");
    }


    public void OnTapButton4()
    {
        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: "退会すると、このアカウントにログインできなくなります。本当に退会しますか。",
            textColorType: EWindowTextColorType.CAUTION,
            textButtonNo: "いいえ",
            textButtonYes: "はい",
            actionOnTapButtonYes: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide();
            });

        Debug.Log(GetType().ToString() + ": OnTapButton4");
    }

    public void OnTapButton5()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleSettingBoardStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }

    public void OnTapButton6() { /* Do Nothing */ }
    public void OnTapButton7() { /* Do Nothing */ }

    // License
    public void OnTapButton8()
    {
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


    public void OnTapButton9()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleMenuStageScene", nextStageType: EStageType.MENU);
    }


    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        _generalButtonListStage = null;
    }
}
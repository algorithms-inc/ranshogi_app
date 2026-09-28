using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// こちらの using を挿入する必要あり
using GFramework.Core;


// ===============================================
// MARK: - ● DebugMenuManager ─────

public class DebugMenuManager : MonoBehaviour, IGeneralButtonListManager
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
            "Debug UI",
            "Debug Audio",
            "Release Ver Test",
            "Debug Blank Page",
            "Window Confirmation",
            "Status Bar",
            "",
            "",
            "",
            "メニューに戻る" };


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


    public void OnTapButton0()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugUIStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }


    public void OnTapButton1()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugAudioStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }

    public void OnTapButton2()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugUIReleaseVersionStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }

    public void OnTapButton3()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleInputNicknameStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }


    // Window Confirmation
    public void OnTapButton4()
    {
        AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
            name: "textFieldA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(0.9f, 0.6f)),
            text: "WindowConfirmation",
            //text: "パスワードは6文字以上の半角英数字記号にしてください",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);



        AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.8f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                endPoint: new Vector2(0.9f, 0.8f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "閉じる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowConfirmation.Show();
    }
    
    public void OnTapButton5() {
         _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugStatusBarStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }
    public void OnTapButton6() { /* Do Nothing */ }
    public void OnTapButton7() { /* Do Nothing */ }
    public void OnTapButton8() { /* Do Nothing */ }


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
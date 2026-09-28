using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// こちらの using を挿入する必要あり
using GFramework.Core;


// ===============================================
// MARK: - ● DebugStatusBarManager ─────

public class DebugStatusBarManager : MonoBehaviour, IGeneralButtonListManager
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
            "Show",
            "Hide",
            "Set Life 0",
            "Set Life 1",
            "Set Life 2",
            "Set Life 3",
            "NORMAL USER",
            "PREMIUM USER",
            "",
            "メニューに戻る" };


        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "Debug Status Bar",
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
        AppManager.Instance.ShowStatusBar();
    }


    public void OnTapButton1()
    {   
        AppManager.Instance.HideStatusBar();
    }

    public void OnTapButton2()
    {
        AppManager.Instance.SetLife(count: 0);
    }

    public void OnTapButton3()
    {
        AppManager.Instance.SetLife(count: 1);
    }

    public void OnTapButton4()
    {
        AppManager.Instance.SetLife(count: 2);
    }

    public void OnTapButton5() {
        AppManager.Instance.SetLife(count: 3);
    }

    public void OnTapButton6() {
        AppManager.Instance.SetIsPremiumUser(value: false);
    }

    public void OnTapButton7() {
        AppManager.Instance.SetIsPremiumUser(value: true);
    }

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
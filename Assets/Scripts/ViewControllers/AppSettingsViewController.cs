using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using GFramework.Core;


public class AppSettingsViewController : MonoBehaviour, IGeneralButtonListManager
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
            "サウンド設定",
            "盤面設定",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "戻る",
        };

        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "アプリ設定",
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
                SoundSetting();
                break;
            case 1:
                BoardSetting();
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


    private void SoundSetting()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "SoundSetting", nextStageType: EStageType.BLANK_PAGE);
    }

    private void BoardSetting()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "BoardSetting", nextStageType: EStageType.BLANK_PAGE);
    }


    private void BackToSettings()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "Settings", nextStageType: EStageType.MENU);
    }
}

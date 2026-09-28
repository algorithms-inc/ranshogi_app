using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// こちらの using を挿入する必要あり
using GFramework.Core;

using GFramework;


// ===============================================
// MARK: - ● SampleMenuManager ─────

// MenuManager では、インターフェースとして IMenuManager を設定する必要あり
public class SampleMenuManager : MonoBehaviour, IMenuManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // MenuStage への参照
    private IMenuStage _menuStage = null;



    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        Debug.Log(this.GetType().ToString() + ": Awake()");

        SetupAppManager();

        SetupStage();

        SetupStatusBar();

        // Debug ModeではBGMをoffに
        if (AppManager.Instance.IsDebugMode)
        {
            AppManager.Instance.AudioController.SetIsMuteBGM(value: true);
        }
    }


    // AppManager のセットアップ
    // - 開発時にはさまざまなシーンが開始シーンとなります。そのため、どのシーンでも AppManager をセットアップする必要があります。
    // - AppManager のセットアップは何度呼んでも問題ありません（最初に呼ばれたときのみ実行されます）。
    private void SetupAppManager()
    {
        AppManager.Instance.Setup();
    }


    private void SetupStage()
    {
        _menuStage = AppManager.Instance.CreateMenuStage(menuManager: this);

        _menuStage.ShowWithAnimation();
    }


    private void SetupStatusBar()
    {
        AppManager.Instance.SetLife(count: 2);
        AppManager.Instance.SetPlayerName(playerName: "Taro");
        AppManager.Instance.SetRate(rate: 1234);

        AppManager.Instance.ShowStatusBar();        
    }



    // ===============================================
    // MARK: - ● Show Window Normal ─────

    private void ShowTestWindowNormal()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldName",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 1.6f)),
            text: "test message.",
            fontScale: 1.0f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }



    // ===============================================
    // MARK: - ● On Event ─────

    // 表示アニメーションが完了
    public void OnDidShowAnimation()
    {
        // Do Nothing

        if (Locator.Instance.debugManager.IsQuickFinishMatchMode)
        {
            OnTapButtonGame66();
        }
    }


    // 対局(66)を開始ボタンを押下
    public void OnTapButtonGame66()
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleGameStageScene", nextStageType: EStageType.GAME);
    }


    // 勝ち切れ乱将棋ボタンを押下
    public void OnTapButtonKachikire()
    {
        var today = System.DateTime.Today;
        _menuStage.ShowWindowKachikireCalendar(
            year: today.Year,
            month: today.Month,
            today: today,
            startDate: today.AddDays(-3),
            days: new List<FKachikireDay> {
                new FKachikireDay(date: today, isCleared: false),
                new FKachikireDay(date: today.AddDays(-1), isCleared: true),
            });
    }


    // 勝ち切れ乱将棋のカレンダーで月を切り替え
    public void OnTapKachikireChangeMonth(int year, int month)
    {
        _menuStage.ShowWindowKachikireCalendar(year: year, month: month, today: System.DateTime.Today, startDate: System.DateTime.Today.AddDays(-3), days: new List<FKachikireDay>());
    }


    // 勝ち切れ乱将棋のカレンダーで対局開始ボタンを押下
    public void OnTapKachikireStart(System.DateTime date)
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleGameStageScene", nextStageType: EStageType.GAME);
    }


    // 友達と対局ボタンを押下
    public void OnTapButtonGameWithFriend()
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleFriendGameMenuStageScene", nextStageType: EStageType.FRIEND_GAME_MENU);
    }


    // 対局履歴ボタンを押下
    public void OnTapButtonHistory()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonHistory");
    }


    // 成績ボタンを押下
    public void OnTapButtonRecord()
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleMypageStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }


    // 設定ボタンを押下
    public void OnTapButtonSetting()
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleSettingStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }


    // ヘルプボタンを押下
    public void OnTapButtonHelp()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonHelp");
    }



    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        _menuStage = null;
    }
}
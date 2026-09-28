using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// こちらの using を挿入する必要あり
using GFramework.Core;


// ===============================================
// MARK: - ● SampleHistoryManager ─────

// HistoryManager では、インターフェースとして IHistoryManager を設定する必要あり
public class SampleHistoryManager : MonoBehaviour, IHistoryManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // HistoryStage への参照
    private IHistoryStage _historyStage = null;


    // -----------------------------------------------
    // MARK: - サンプルを動かすための変数

    // 画面上に一度に表示できる棋譜アイテムの最大個数
    private int _maxDisplayNum = 0;


    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        Debug.Log(GetType().ToString() + ": Awake()");

        SetupAppManager();

        SetupStage();
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
        _historyStage = AppManager.Instance.CreateHistoryStage(historyManager: this);

        // 1ページに表示される項目数を取得(画面サイズにより可変となるが、現在は５で固定)
        _maxDisplayNum = _historyStage.GetHistoryItemMaxDisplayNum();


        // 現在のページ数を設定
        _historyStage.SetPresentPageNum(value: 1);
        _historyStage.SetMaxPageNum(value: 2);


        // 棋譜リストを設定
        SetupHistoryItem();


        // ページ操作のボタンの初期状態は Disabled であるため、必要なボタンを Enabled 状態にする
        _historyStage.SetButtonNextPageEnabled();
        _historyStage.SetButtonLastPageEnabled();


        _historyStage.ShowWithAnimation();
    }


    private void SetupHistoryItem()
    {
        var data0 = new FHistoryItemInitializer(
            historyDetailID: 0,
            gameNum: 9,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.WIN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/31(土) 12:59",
            dateAdditionalInfo: "今日",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Hanako",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data1 = new FHistoryItemInitializer(
            historyDetailID: 1,
            gameNum: 8,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.LOSE,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/31(金) 12:59",
            dateAdditionalInfo: "今日",
            color: Color.White,
            playerName: "Yamada Taro",
            opponentName: "Taro",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data2 = new FHistoryItemInitializer(
            historyDetailID: 2,
            gameNum: 7,
            boardType: EBoardType.BOARD_99,
            historyItemResult: EHistoryItemResult.DRAW,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/30(木) 12:59",
            dateAdditionalInfo: "昨日",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Hiroshi",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data3 = new FHistoryItemInitializer(
            historyDetailID: 3,
            gameNum: 6,
            boardType: EBoardType.BOARD_99,
            historyItemResult: EHistoryItemResult.UNKNOWN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/29(水) 12:59",
            dateAdditionalInfo: "２日前",
            color: Color.White,
            playerName: "Yamada Taro",
            opponentName: "Takeshi",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: false,
            isEnabled: false);

        var data4 = new FHistoryItemInitializer(
            historyDetailID: 4,
            gameNum: 5,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.WIN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/29(水) 12:59",
            dateAdditionalInfo: "２日前",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Jiro",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);


        List<FHistoryItemInitializer> historyItemInitializerList = new List<FHistoryItemInitializer>() { data0, data1, data2, data3, data4 };

        _historyStage.SetHistoryItemList(historyItemInitializerList: historyItemInitializerList);
    }



    // ===============================================
    // MARK: - ● On Event ─────

    public void OnDidShowAnimation()
    {
        Debug.Log(GetType().ToString() + ": OnDidShowAnimation");
    }


    public void OnTapButtonBack()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonBack");

        _historyStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleMypageStageScene", nextStageType: EStageType.BLANK_PAGE);
    }


    public void OnTapButtonShowHistoryDetail(int itemID)
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonShowItem");

        Debug.Log("棋譜詳細(" + itemID.ToString() + ")がタップされました");

        _historyStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleHistoryDetailStageScene", nextStageType: EStageType.HISTORY_DETAIL);
    }


    public void OnTapButtonNextPage()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonNextPage");


        // 現在のページ数を変更
        _historyStage.SetPresentPageNum(value: 2);


        // 棋譜の項目を変更
        var data0 = new FHistoryItemInitializer(
            historyDetailID: 4,
            gameNum: 4,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.LOSE,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/31(土) 12:59",
            dateAdditionalInfo: "今日",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Keiko",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data1 = new FHistoryItemInitializer(
            historyDetailID: 5,
            gameNum: 3,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.WIN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/31(金) 12:59",
            dateAdditionalInfo: "今日",
            color: Color.White,
            playerName: "Yamada Taro",
            opponentName: "Kenji",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data2 = new FHistoryItemInitializer(
            historyDetailID: 6,
            gameNum: 2,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.LOSE,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/30(木) 12:59",
            dateAdditionalInfo: "昨日",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Kenta",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data3 = new FHistoryItemInitializer(
            historyDetailID: 7,
            gameNum: 1,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.LOSE,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/29(水) 12:59",
            dateAdditionalInfo: "２日前",
            color: Color.White,
            playerName: "Yamada Taro",
            opponentName: "Sumire",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);


        List<FHistoryItemInitializer> historyItemInitializerList = new List<FHistoryItemInitializer>() { data0, data1, data2, data3 };

        _historyStage.SetHistoryItemList(historyItemInitializerList: historyItemInitializerList);



        // 必要に応じてボタンの使用可否を変更する
        _historyStage.SetButtonFirstPageEnabled();
        _historyStage.SetButtonPreviousPageEnabled();
        _historyStage.SetButtonNextPageDisabled();
        _historyStage.SetButtonLastPageDisabled();
    }


    public void OnTapButtonPreviousPage()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonPreviousPage");


        // 現在のページ数を変更
        _historyStage.SetPresentPageNum(value: 1);


        // 棋譜の項目を変更
        var data0 = new FHistoryItemInitializer(
            historyDetailID: 0,
            gameNum: 9,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.WIN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/31(土) 12:59",
            dateAdditionalInfo: "今日",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Hanako",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data1 = new FHistoryItemInitializer(
            historyDetailID: 1,
            gameNum: 8,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.LOSE,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/31(金) 12:59",
            dateAdditionalInfo: "今日",
            color: Color.White,
            playerName: "Yamada Taro",
            opponentName: "Taro",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data2 = new FHistoryItemInitializer(
            historyDetailID: 2,
            gameNum: 7,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.DRAW,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/30(木) 12:59",
            dateAdditionalInfo: "昨日",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Hiroshi",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);

        var data3 = new FHistoryItemInitializer(
            historyDetailID: 3,
            gameNum: 6,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.UNKNOWN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/29(水) 12:59",
            dateAdditionalInfo: "２日前",
            color: Color.White,
            playerName: "Yamada Taro",
            opponentName: "Takeshi",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: false,
            isEnabled: false);

        var data4 = new FHistoryItemInitializer(
            historyDetailID: 4,
            gameNum: 5,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.WIN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/29(水) 12:59",
            dateAdditionalInfo: "２日前",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Jiro",
            gradeRankType: EGradeRankType.GRADE_1,
            opponentGradeRankType: EGradeRankType.GRADE_2,
            previousRate: 1234,
            nextRate: 1255,
            previousGradeRankType: EGradeRankType.GRADE_1,
            nextGradeRankType: EGradeRankType.RANK_1,
            canExportData: true,
            isEnabled: true);


        List<FHistoryItemInitializer> historyItemInitializerList = new List<FHistoryItemInitializer>() { data0, data1, data2, data3, data4 };

        _historyStage.SetHistoryItemList(historyItemInitializerList: historyItemInitializerList);


        // 必要に応じてボタンの使用可否を変更する
        _historyStage.SetButtonFirstPageDisabled();
        _historyStage.SetButtonPreviousPageDisabled();
        _historyStage.SetButtonNextPageEnabled();
        _historyStage.SetButtonLastPageEnabled();
    }


    public void OnTapButtonFirstPage()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonFirstPage");


        // 現在のページ数を変更
        _historyStage.SetPresentPageNum(value: 1);

        // 棋譜の項目を変更
        // （サンプルコード省略）

        // 必要に応じてボタンの使用可否を変更する
        _historyStage.SetButtonFirstPageDisabled();
        _historyStage.SetButtonPreviousPageDisabled();
        _historyStage.SetButtonNextPageEnabled();
        _historyStage.SetButtonLastPageEnabled();
    }


    public void OnTapButtonLastPage()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonLastPage");


        // 現在のページ数を変更
        _historyStage.SetPresentPageNum(value: 2);

        // （サンプルコード省略）

        // 必要に応じてボタンの使用可否を変更する
        _historyStage.SetButtonFirstPageEnabled();
        _historyStage.SetButtonPreviousPageEnabled();
        _historyStage.SetButtonNextPageDisabled();
        _historyStage.SetButtonLastPageDisabled();
    }


 
    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        Debug.Log(GetType().ToString() + ": OnDestroy");

        _historyStage = null;
    }
}
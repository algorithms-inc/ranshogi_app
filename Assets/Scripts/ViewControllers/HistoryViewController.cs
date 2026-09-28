using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections.Generic;
using GFramework.Core;
using System.Linq;

public class HistoryViewController : MonoBehaviour, IHistoryManager
{
    [SerializeField] Canvas canvas;
    [SerializeField] LoadingViewController loadingViewPrefab;
    [SerializeField] HistoryCell historyCellPrefab;
    [SerializeField] GameObject contentView;
    private LoadingViewController loadingView;

    private IHistoryStage _historyStage = null;

    private int itemPerPage = 0;
    private int maxPage = 0;

    // screen page
    private int currentPage = 0;

    // mysql offset;

    private int nGames = 0;
    private List<Game> games = new List<Game>();


    void Start()
    {
        AppManager.Instance.Setup();
        _historyStage = AppManager.Instance.CreateHistoryStage(historyManager: this);
        itemPerPage = _historyStage.GetHistoryItemMaxDisplayNum();

        LoadHistory();
        _historyStage.ShowWithAnimation();
        _historyStage.SetPresentPageNum(value: 1);
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    public void OnTapButtonBack()
    {
        // マイページ(成績)から開いたときはマイページへ、メニューの「対局履歴」から開いたときはメニューへ戻る
        if (GameManager.instance.globalVariables.didComeFromMypageToHistory) {
            _historyStage.HideWithAnimationAndChangeScene(nextSceneName: "MyPage", nextStageType: EStageType.BLANK_PAGE);
        } else {
            _historyStage.HideWithAnimationAndChangeScene(nextSceneName: "Menu", nextStageType: EStageType.MENU);
        }
    }


    public void OnTapButtonShowHistoryDetail(int itemID)
    {
        var game = games.Find(g => g.id == itemID);
        GameManager.instance.globalVariables.gameForHistoryDetail = game;
        if(game.gameResult == GameResult.InvalidGame 
        || game.state != GameState.GameFinished
        || game.blackRateTo == 0
        || game.whiteRateTo == 0
        ) {
            AlertViewController.ShowOKConfirmation("棋譜データが存在しません。", AlertViewController.Type.OK);
            return;
        }

        _historyStage.HideWithAnimationAndChangeScene(nextSceneName: "historyDetail", nextStageType: EStageType.HISTORY_DETAIL);
    }


    public void OnTapButtonNextPage()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonNextPage");

        // 現在のページ数を変更
        currentPage++;
        ShowCurrentPage();
    }


    public void OnTapButtonPreviousPage()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonPreviousPage");

        currentPage--;
        // 現在のページ数を変更
        ShowCurrentPage();
    }


    public void OnTapButtonFirstPage()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonFirstPage");
        currentPage = 0;
        ShowCurrentPage();
    }


    public void OnTapButtonLastPage()
    {
        currentPage = maxPage;
        ShowCurrentPage();
    }


    private void ShowCurrentPage() {
        _historyStage.SetPresentPageNum(value: currentPage+1);
        var gameList = new List<FHistoryItemInitializer>();
        var maximumIndex = Mathf.Min(itemPerPage * (currentPage+1), nGames);
        if(games[currentPage * itemPerPage] == null) {
            LoadHistory();
            return;
        }
        var now = DateTime.Now;

        for(int i=itemPerPage * currentPage; i<maximumIndex; i++) {
            var game = games[i];
            if(game == null) { continue; }
            var isEnabled = true;
            if(game.gameResult == GameResult.InvalidGame 
            || game.state != GameState.GameFinished
            || game.blackRateTo == 0
            || game.whiteRateTo == 0
            || game.blackUid == null
            || game.whiteUid == null
            ) {
                isEnabled = false;
            }

            var playerColor = isEnabled ? (game.blackUid == Auth.instance.GetUser().firebaseUid ? Color.Black : Color.White) : Color.Black;
            var result = G.GameResultToEHistoryItemResult(game.gameResult, playerColor);

            TimeZone zone = TimeZone.CurrentTimeZone;
            TimeSpan offset = zone.GetUtcOffset(DateTime.Now);
            DateTime localGameFinishedAt = game.gameFinishedAt + offset;

            var interval = now - localGameFinishedAt;
            var dateAdditionalInfo = "";
            if(interval.Days > 0) {
                dateAdditionalInfo = interval.Days.ToString() + "日前";
            } else if(interval.Hours > 0) {
                dateAdditionalInfo = interval.Hours.ToString() + "時間前";
            } else if(interval.Minutes > 0) {
                dateAdditionalInfo = interval.Minutes.ToString() + "分前";
            } else {
                dateAdditionalInfo = interval.Seconds.ToString() + "秒前";
            }

            game.gameNum = games.Count - i;
            var historyItem = new FHistoryItemInitializer(
                historyDetailID: game.id,
                gameNum: game.gameNum,
                boardType: game.BoardType(), 
                historyItemResult: result,
                gameResultReasonType: EGameResultReasonType.CHECKMATE, // TODO: 正しい値を表示してください
                date: localGameFinishedAt.ToString( "yyyy/MM/dd HH:mm:ss" ),
                dateAdditionalInfo: dateAdditionalInfo,
                color: playerColor,
                playerName: playerColor == Color.Black ? game.blackNickname : game.whiteNickname,
                opponentName: playerColor == Color.Black ? game.whiteNickname : game.blackNickname,
                gradeRankType: G.RateToGradeRankType(game.RatingOf(playerColor)),
                opponentGradeRankType: G.RateToGradeRankType(game.RatingOf(G.Not(playerColor))),
                previousRate: game.RatingOf(playerColor),
                nextRate: playerColor == Color.Black ? game.blackRateTo : game.whiteRateTo,
                previousGradeRankType: G.RateToGradeRankType(game.RatingOf(playerColor)),
                nextGradeRankType: G.RateToGradeRankType(playerColor == Color.Black ? game.blackRateTo : game.whiteRateTo),
                canExportData: false,  // TODO: 棋譜データダウンロード機能が実装されたのちは、必要に応じて true を設定してください
                isEnabled: isEnabled
            );
            gameList.Add(historyItem);
        }

        if(currentPage >= 1) {
            _historyStage.SetButtonFirstPageEnabled();
            _historyStage.SetButtonPreviousPageEnabled();
        } else {
            _historyStage.SetButtonFirstPageDisabled();
            _historyStage.SetButtonPreviousPageDisabled();
        }
        if(currentPage < maxPage) {
            _historyStage.SetButtonNextPageEnabled();
            _historyStage.SetButtonLastPageEnabled();
        } else {
            _historyStage.SetButtonNextPageDisabled();
            _historyStage.SetButtonLastPageDisabled();
        }
        _historyStage.SetHistoryItemList(historyItemInitializerList: gameList);
    }
    bool firstLoad = true;

    private void LoadHistory() {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("サーバと通信中...", false, () => { });
        HistoryRequest request = new HistoryRequest();
        request._requestQueryString = "page=" + currentPage.ToString();
        var context = SynchronizationContext.Current;
        Api.History(request, (history) => {
            context.Post((state2) => {
                Destroy(loadingView.gameObject);
                nGames = history.nGames;
                if(firstLoad) {
                    firstLoad = false;
                    maxPage = (nGames-1) / itemPerPage;
                    _historyStage.SetMaxPageNum(value: maxPage+1);
                    games.AddRange(Enumerable.Repeat<Game>(null, nGames));
                }
                for(int i=itemPerPage * currentPage; i<itemPerPage * currentPage + history.games.Count(); i++) {
                    games[i] = history.games[i - itemPerPage * currentPage];
                }
                //games.InsertRange(itemPerPage * currentPage, history.games);
                ShowCurrentPage();
            }, null);
        }, (errorMessage) => {
            Debug.Log(errorMessage);
            Destroy(loadingView.gameObject);
        });
    }
}

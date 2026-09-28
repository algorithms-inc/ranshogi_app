using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using GFramework.Core;
using GFramework;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

public class GameController : MonoBehaviour, IGameManager
{

    Position position;

    GameState gameState = GameState.OnSearchingOpponent;
    Square tappedSquare = Square.SQ_Invalid;
    Piece tappedHandPiece = Piece.No_Piece;

    [SerializeField] Canvas canvas;
    private LoadingViewController loadingView;
    [SerializeField] LoadingViewController loadingViewPrefab;

    float blackTime = 300;
    float whiteTime = 300;
    int displayBlackTime = 300;
    int displayWhiteTime = 300;

    Game game;

    WebSocketManager wsm = new WebSocketManager();

    private IGameStage gameStage = null;
    private RewardedAdManager adManager;

    private bool isClosed = false;
    private bool shouldDoLatestMove = false;
    private bool shouldSendLatestMove = false;
    private Move latestMoveShouldBeDone;
    private bool didShowResult = false;
    private bool applicationFocus = false;
    private bool backButtonTapped = false;

    private bool keepPlayingGame = false;
    private bool isChoosingPromotion = false;

    Move currentMove;

    List<Move> moves = new List<Move>();

    // --- AI対戦(端末内エンジンとの対戦)用 ---
    // nullでなければサーバーには一切接続せず、RanshogiEngineを相手に指す。
    private AiGameSetting aiSetting = null;
    private bool IsAiGame => aiSetting != null;
    private bool isAiThinking = false;
    // AIが指すまでの最低の思考時間(秒)。この範囲でランダムに決める。
    private static readonly (float min, float max) AiMinThinkSeconds = (1f, 2f);
    // AIの1手あたりの思考時間の上限(ms)。深さ(強さ)に届く前でもこの時間で打ち切って指す
    private const int AiMaxThinkMilliseconds = 3000;

    void Awake() {
    }

    // Start is called before the first frame update
    void Start()
    {
        AppManager.Instance.Setup();

        adManager = FindFirstObjectByType<RewardedAdManager>();

        //FBoardSetSetting gameStageModeSetting = new FBoardSetSetting(
            //boardDesignType: 0,  // 盤のデザイン       現在 0 のみ
            //pieceDesignType: 0,  // 駒のデザイン       現在 0 のみ
            //pieceFrontType: 0,  // 駒表タイプ         0: 二文字, 1: 一文字
            //pieceBackTypeForRookGroup: 2,  // 駒裏タイプ(飛車系) 0: 楷書二文字, 1: くずし字, 2: 楷書一文字
            //pieceBackTypeForSilverGroup: 0,  // 駒裏タイプ(銀系)   0: 楷書二文字, 1: くずし字, 2: 略字一文字
            //pieceBackTypeForPawn: 0); // 駒裏タイプ(歩)     現在 0「と」のみ


        Debug.Log("GameManager.instance.globalVariables.boardType: " + GameManager.instance.globalVariables.boardType);
        gameStage = AppManager.Instance.CreateGameStage(
            gameManager: this,
            boardType: GameManager.instance.globalVariables.boardType,
            isWithFriendMode: isFriendMatch(),
            // AI対戦は相手探しを経ずにすぐ盤を出すので、再開と同じ扱い(対局前の状態から始める)にする。
            isNewGame: GameManager.instance.globalVariables.gameForReconnection == null
                && GameManager.instance.globalVariables.aiGame == null
        );
        if(GameManager.instance.globalVariables.boardType == EBoardType.BOARD_66) {
            position = new Position66();
        } else {
            position = new Position99();
        } 

        aiSetting = GameManager.instance.globalVariables.aiGame;

        if (IsAiGame) {
            // AI対戦は続けて対局しないので、結果画面から元の画面(検討画面/勝ち切れ乱将棋のカレンダー)へ戻す。
            if (aiSetting.kachikireDate != null) {
                // 勝ち切れ乱将棋は、結果画面から正解手順を見られるようにする
                gameStage.SetNextGameButtonText("正解手順");
                gameStage.SetHomeButtonText("戻る");
            } else {
                gameStage.SetNextGameButtonText("戻る");
            }
            // 対局中も投了ボタンの横に「待った」「戻る」を出す
            gameStage.SetAiGameButtonsShown(true);
            SetupAiGame();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            return;
        }

        wsm.Init(OnReceiveMessage, MatchingFinished, OnConnect, OnClose);
        if (GameManager.instance.globalVariables.gameForReconnection != null) {
            game = GameManager.instance.globalVariables.gameForReconnection;
            gameState = GameState.Preparing;
            //position.Set(game.sfen);
            //SetupGameStage();
            wsm.ConnectGame(game);
        } else if (isFriendMatch()) {
            gameStage.ShowOpponentSearcher();
            wsm.ConnectMatching(GameManager.instance.globalVariables.boardType, roomCode: GameManager.instance.globalVariables.roomCode);
        } else {
            gameStage.ShowOpponentSearcher();
            wsm.ConnectMatching(GameManager.instance.globalVariables.boardType);
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //SetupGameStage();
    }

    private void OnApplicationPause(bool pause) { 
        //Debug.Log("######### on application pause!!!" + pause);

    }

    private void OnApplicationFocus(bool hasFocus) {
        applicationFocus = hasFocus;
        if (!hasFocus) return;
        if (IsAiGame) return;   // AI対戦はサーバーに繋がっていないので再接続は不要
        TryReconnect();
    }

    private bool isFriendMatch() {
        return GameManager.instance.globalVariables.roomCode != null;
    }

    bool isReconnecting = false;

    private void TryReconnect() { 
        if(isClosed && !isReconnecting) {
            isReconnecting = true;
            loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
            loadingView.Init("Loading game ...", false, () => { });
            HistoryRequest request = new HistoryRequest();
            var context = SynchronizationContext.Current;
            Api.History(request, (history) => {
                context.Post((state2) => {
                    Destroy(loadingView.gameObject);
                    Game latestGame = history.games[0];
                    if (latestGame.state == GameState.OnGame) {
                        if(latestGame.moves().Count > 0 && latestGame.moves().Last() != currentMove && latestGame.moves().Last().color() == position.opponentColor && (position.moves.Count < 2 || position.moves[position.moves.Count-2] != latestGame.moves().Last())) {
                            shouldDoLatestMove = true;
                            latestMoveShouldBeDone = latestGame.moves().Last();
                        }
                        wsm.ConnectGame(latestGame);
                    } else {
                        if (!didShowResult)
                        {
                            FinishGame(latestGame.gameFinishReason, latestGame.gameResult, position.myColor == Color.Black ? latestGame.blackRateTo : latestGame.whiteRateTo);
                        }
                    }
                    isReconnecting = false;
                }, null);
            }, (errorMessage) => {
                //Debug.Log(errorMessage);
                Destroy(loadingView.gameObject);
                isReconnecting = false;
            });

        } else {
        }
    }

    private void SetupGameStage()
    {
        DoSetupGameStage();
    }

    private void DoSetupGameStage() {
        var player1Data = new FPlayerData(
            playerName: position.myColor == Color.Black ? game.blackNickname : game.whiteNickname,
            rate: Auth.instance.GetUser().RatingOf(GameManager.instance.globalVariables.boardType),
            gradeRankType: G.RateToGradeRankType(Auth.instance.GetUser().RatingOf(GameManager.instance.globalVariables.boardType)),
            color: position.myColor);


        var player2Data = new FPlayerData(
            playerName: position.myColor == Color.Black ? game.whiteNickname : game.blackNickname,
            rate: position.myColor == Color.Black ? game.whiteRating : game.blackRating,
            gradeRankType: G.RateToGradeRankType(position.myColor == Color.Black ? game.whiteRating : game.blackRating),
            color: position.myColor == Color.Black ? Color.White : Color.Black);


        var (player1PieceList, player2PieceList) = position.PieceLists();

        gameStage.ShowBoardSet(
            player1Data: player1Data, player1PieceList: player1PieceList,
            player2Data: player2Data, player2PieceList: player2PieceList, GameManager.instance.globalVariables.boardType);
        

        if (IsAiGame) {
            // AI対戦には持ち時間が無い
            gameStage.SetTimeLeftUnknown();
        } else {
            gameStage.SetTimeLeft(minute: 3, second: 0);
        }
    }

    public bool IsPlayableGameState() {
        return (gameState == GameState.OnGame);
    }

    public bool IsYourTurn() {
        return position.sideToMove == position.myColor;
    }

    // ===============================================
    // MARK: - ●  ─────

    public void SendMove(Move m) {
        byte[] data = { (byte)GameMessageFlag.Move, 0, 0, 0, 0 }; 
        byte[] moveBytes = BitConverter.GetBytes((int)m.move);
        data[1] = moveBytes[0];
        data[2] = moveBytes[1];
        data[3] = moveBytes[2];
        data[4] = moveBytes[3];

        //Debug.Log("Sending Data");
        //Debug.Log(data[0]);
        //Debug.Log(data[1]);
        //Debug.Log(data[2]);
        //Debug.Log(data[3]);
        wsm.SendData(data);

        //if(position.IsMated()) {
        //    gameState = GameState.GameFinishedWaitingServerResponse;
        //    byte[] d = { (byte)GameMessageFlag.ChangeGameState, 0, 0, 0, 0 };
        //    wsm.SendData(d);
        //}

    }


    public void DoMove(Move m) {
        bool doneByThisDevice = (position.sideToMove == position.myColor);
        var context = SynchronizationContext.Current;
        var currentColor = position.sideToMove;

        EPromotionAfterMoveType promotion = 
            (position.sideToMove == position.myColor) && !m.isDrop() ? (
                position.MustPromote(m.movedPiece(), m.to(), position.sideToMove) ? EPromotionAfterMoveType.PROMOTE : (
                    position.CanPromote(m.movedPiece(), m.from(), m.to(), position.sideToMove) ? EPromotionAfterMoveType.ASK_PROMOTE : EPromotionAfterMoveType.NOT_PROMOTE
                )
            ) : (m.isPromote() ? EPromotionAfterMoveType.PROMOTE : EPromotionAfterMoveType.NOT_PROMOTE);
        if(promotion == EPromotionAfterMoveType.PROMOTE) m.setPromote();

        currentMove = m;

        //Debug.Log("DoMove: " + m.from() + " " + m.to() + " " + m.movedPiece());

        context.Post((state) => {
            if (m.isDrop()) {
                gameStage.MovePiece(currentColor, G.TypeOf(m.movedPiece()), position.SquareToDesignSquare((int)m.to()));
            } else {
                gameStage.MovePiece(currentColor, position.SquareToDesignSquare((int)m.from()), position.SquareToDesignSquare((int)m.to()), promotion);
            }
            if(promotion != EPromotionAfterMoveType.ASK_PROMOTE) {
                ResetTapState(false);
            }
        }, null);
        if(promotion != EPromotionAfterMoveType.ASK_PROMOTE ) {
            DoMoveDone();
        } else {
            isChoosingPromotion = true;
        }
    }

    public void DoMoveDone() { 
        if (position.sideToMove == position.myColor && !IsAiGame) {
            if (isClosed) {
                shouldSendLatestMove = true;
            } else {
                SendMove(currentMove);
            }
        }
        position.DoMove(currentMove);
        moves.Add(currentMove);


        //Debug.Log("###: DoMoveDone");
        //position.Pieces(Color.Black).show();
        //position.Pieces(Color.White).show();
        if(position.IsMated()) {
            gameState = GameState.GameFinishedWaitingServerResponse;
            if (IsAiGame) {
                // 詰まされた側の負け。サーバーからの通知が来ないので自分で確定させる。
                FinishAiGame(GameFinishReason.Mate,
                    position.sideToMove == Color.Black ? GameResult.WhiteWon : GameResult.BlackWon);
            }
        } else {
            gameStage.ChangePresentPlayer(color: position.sideToMove);
            if (IsAiGame && position.sideToMove != position.myColor) {
                RequestAiMove();
            }
        }
    }

    public void ResetCells() {
        gameStage.DeselectPiece();
    }

    // 盤上の駒がないセルをタップ
    public void OnTap(Square sq)
    {

        if(!IsPlayableGameState() || !IsYourTurn() || isChoosingPromotion) return;

        //Debug.Log(this.GetType().ToString() + ": onTap(Square): " + sq.ToString());
        //Debug.Log(position.emphasizedCells[(int)sq]);

        //Debug.Log("onTap(Square): " + sq.ToString());

        if(position.emphasizedCells[(int)sq]) {
            Move m;
            if (tappedSquare != Square.SQ_Invalid)
            {
                //Debug.Log("Normal move");

                Piece movedPiece = position.Get(tappedSquare);
                m = G.MakeMove(tappedSquare, sq, movedPiece, false, false);
            }
            else
            {
                //Debug.Log("Drop move");
                m = G.MakeMove(Square.SQ_Invalid, sq, tappedHandPiece, false, true);
            }
            DoMove(m);
        } else {
            ResetTapState();
        }
    }


    // 盤上の駒をタップ
    public void OnTap(Piece piece, Square sq)
    {
        if(!IsPlayableGameState() || !IsYourTurn() || isChoosingPromotion) return;


        Debug.Log(this.GetType().ToString() + ": onTap(Square): " + sq.ToString() + ", " + piece);
        //Debug.Log(position.emphasizedCells[(int)sq]);

        if (position.emphasizedCells[(int)sq]) {
            //Debug.Log("Taped Emphasizedcell!!" + (int)sq);
            Move m;
            if (tappedSquare != Square.SQ_Invalid)
            {
                Piece movedPiece = position.Get(tappedSquare);
                m = G.MakeMove(tappedSquare, sq, movedPiece, false, false);
            }
            else
            {
                m = G.MakeMove(Square.SQ_Invalid, sq, tappedHandPiece, false, true);
            }
            DoMove(m);
            return;
        }
        tappedHandPiece = Piece.No_Piece;

        if (tappedSquare == sq)
        {
            gameStage.DeselectPiece();
            tappedSquare = Square.SQ_Invalid;
            ResetTapState();
            return;
        }

        ResetTapState();
        Piece pc = position.Get(sq);
        //Debug.Log(pc);
        if (G.ColorOf(pc) == position.sideToMove)
        {
            List<Square> squares = position.MovableBB(pc, sq).ToSquares();
            if (squares.Count > 0)
            {
                tappedSquare = sq;
                foreach (Square msq in squares)
                {
                    position.emphasizedCells[(int)msq] = true;
                }
                gameStage.SelectPiece(
                    position.sideToMove, 
                    position.SquareToDesignSquare((int)sq), 
                    squares.Select(s => position.SquareToDesignSquare((int)s)).ToList());
            }
            else
            {
                tappedSquare = Square.SQ_Invalid;
            }
        }

    }


    // 盤・駒設置アニメーション、対局情報などの表示完了
    public void OnDidShowBoardSet()
    {
        //Debug.Log(this.GetType().ToString() + ": onDidShowBoardSet()");
        gameState = GameState.OnGame;
        gameStage.StartGame();
        gameStage.ChangePresentPlayer(color: position.sideToMove);

        if (IsAiGame && position.sideToMove != position.myColor) {
            RequestAiMove();
        }
    }


    // 対局結果表示でボタン操作後、盤・駒の非表示完了
    public void OnDidHideBoardSet()
    {
        if(keepPlayingGame) {
            SceneManager.LoadScene("Game");

        } else {
            GoToMenu();
        }
    }


    // 自軍の駒台の駒をタップ
    public void OnTapOnWaitingSpace(Piece piece)
    {
        //Debug.Log(this.GetType().ToString() + ": onTapOnWaitingSpace(Piece): " + piece.ToString());
        if(!IsPlayableGameState() || !IsYourTurn() || isChoosingPromotion) return;
        //Debug.Log("tapped pc: " + piece);
        if (tappedHandPiece == piece || G.ColorOf(piece) != position.sideToMove)
        {
            ResetTapState();
            return;
        }
        ResetTapState();
        Bitboard bb = position.DropableBBOf(piece);
        List<Square> squares = bb.ToSquares();
        if (squares.Count > 0)
        {
            foreach (Square msq in squares)
            {
                //Debug.Log(msq);
                position.emphasizedCells[(int)msq] = true;
            }
            gameStage.SelectPiece(position.sideToMove, G.TypeOf(piece), squares.Select(s => position.SquareToDesignSquare((int)s)).ToList());
            tappedHandPiece = piece;
        }
        else
        {
            tappedHandPiece = Piece.No_Piece;
        }
    }


    // 盤・自軍駒台外をタップ
    public void OnTapOutOfBoard()
    {
        //Debug.Log(this.GetType().ToString() + ": onTapOutOfBoard()");
    }


    // 投了ボタンをタップ
    public void OnTapButtonResign()
    {
        if(!IsPlayableGameState() || !IsYourTurn()) return;
        if(isChoosingPromotion) return;
        //Debug.Log(this.GetType().ToString() + ": onTapButtonResign()");
        if (IsAiGame) {
            ResetTapState();
            FinishAiGame(GameFinishReason.PlayerResigned,
                position.myColor == Color.Black ? GameResult.WhiteWon : GameResult.BlackWon);
            return;
        }
        Move m = G.MakeResignMove();
        byte[] data = { (byte)GameMessageFlag.Move, 0, 0, 0, 0 }; 
        byte[] moveBytes = BitConverter.GetBytes((int)m.move);
        data[1] = moveBytes[0];
        data[2] = moveBytes[1];
        data[3] = moveBytes[2];
        data[4] = moveBytes[3];
        wsm.SendData(data);
        ResetTapState();
    }

    
    // AI対戦で「待った」ボタンをタップ。自分の直前の手(とそれに対するAIの手)をすぐ取り消して自分の手番に戻す
    public void OnTapButtonUndoInAiGame()
    {
        if (!IsAiGame || !IsPlayableGameState() || isChoosingPromotion) return;

        if (moves.Count - aiSetting.startMoveCount < UndoCountInAiGame()) {
            AlertViewController.ShowOKConfirmation("待ったできる手がありません。");
            return;
        }

        UndoInAiGame();
    }


    // 待ったで戻す手数。AIが指した後ならAIの手と自分の手の2手、AIが考えている間なら自分の手の1手
    private int UndoCountInAiGame() => IsYourTurn() ? 2 : 1;


    private void UndoInAiGame()
    {
        // 確認している間にAIが指したり対局が終わったりしていることがあるので、ここで数え直す
        if (this == null || !IsPlayableGameState()) return;
        int undoCount = UndoCountInAiGame();
        if (moves.Count - aiSetting.startMoveCount < undoCount) return;

        // 考えている途中のAIの手は、対局中でなくなると捨てられる(RequestAiMove)。
        // エンジンは同時に1つしか考えられないので、次の思考を待たせないよう読みも打ち切る
        gameState = GameState.Preparing;
        if (isAiThinking) RanshogiEngine.Stop();
        ResetTapState();

        // 盤の表示を作り直すため、取り消した後の手順で対局画面を開き直す(AI機能の回数は数えない)
        aiSetting.initialMoves = moves.GetRange(0, moves.Count - undoCount);
        GameManager.instance.globalVariables.aiGame = aiSetting;
        gameStage.HideBoardSetAndChangeScene("Game", EStageType.GAME);
    }


    // AI対戦で「戻る」ボタンをタップ。確認してから対局をやめ、始めた画面へ戻る
    public void OnTapButtonBackInAiGame()
    {
        if (!IsAiGame || !IsPlayableGameState() || isChoosingPromotion) return;

        AppManager.Instance.UICreator.ShowSimpleWindowConfirmation(
            message: "対局をやめて戻りますか？",
            textColorType: EWindowTextColorType.NORMAL,
            textButtonNo: "続ける",
            textButtonYes: "戻る",
            actionOnTapButtonYes: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: () => {
                    if (this == null || !IsPlayableGameState()) return;
                    gameState = GameState.Preparing;
                    if (isAiThinking) RanshogiEngine.Stop();
                    ResetTapState();
                    ReturnFromAiGame();
                });
            });
    }


    // AI対戦を始めた画面へ戻る
    private void ReturnFromAiGame()
    {
        if (aiSetting.kachikireDate != null) {
            // メニューに戻ったら、対局した日の月のカレンダーを開く
            GameManager.instance.globalVariables.kachikireMonthToOpen = aiSetting.kachikireDate.Substring(0, 7);
            gameStage.HideBoardSetAndChangeScene("Menu", EStageType.MENU);
        } else {
            // 検討画面は gameForHistoryDetail(元の棋譜)と historyDetailPly(手数)で元の表示に戻る
            gameStage.HideBoardSetAndChangeScene("historyDetail", EStageType.HISTORY_DETAIL);
        }
    }


    // 「対局待機中のユーザがいます」の通知の「対局する」を、AI対戦中に押したとき。
    // AI対戦をやめてメニューへ移る(メニューがそのまま対局を始める)
    public void LeaveAiGameForMatching()
    {
        if (!IsAiGame) return;

        if (IsPlayableGameState()) {
            gameState = GameState.Preparing;
            if (isAiThinking) RanshogiEngine.Stop();
            ResetTapState();
        }

        GameManager.instance.globalVariables.aiGame = null;

        // 検討画面には戻らないので、戻るために取ってあった手順を捨てる。
        // 残したままだと、次に別の棋譜の検討を開いたときに、その棋譜へこの手順を並べてしまう
        GameManager.instance.globalVariables.historyDetailMoves = null;
        GameManager.instance.globalVariables.historyDetailPly = 0;
        GameManager.instance.globalVariables.gameForHistoryDetail = null;

        gameStage.HideBoardSetAndChangeScene("Menu", EStageType.MENU);
    }


    // 成る／成らないの選択完了
    public void OnDidSelectPromotion(Piece piece, Square sq, EPromotionResultType promotionResultType)
    {
        if (promotionResultType == EPromotionResultType.PROMOTED)
        {
            // 成った
            //Debug.Log(this.GetType().ToString() + ": " + piece.ToString() + "(" + sq.ToString() + ") selected to be promoted");
            currentMove.setPromote();
        }
        else
        {
            // case .NOT_PROMOTED
            // 成らなかった
            //Debug.Log(this.GetType().ToString() + ": " + piece.ToString() + "(" + sq.ToString() + ") selected not to be promoted");
        }
        isChoosingPromotion = false;
        DoMoveDone();
    }

    public void OnTapBackOnOpponentSearcher()
    {
        backButtonTapped = true;
        //Debug.Log(this.GetType().ToString() + ": onTapBackOnOpponentSearcher()");

        if(isFriendMatch()) {
            gameStage.HideOpponentSearcherForBackAndChangeScene("Friend", EStageType.MENU);
        } else {
            gameStage.HideOpponentSearcherForBackAndChangeScene("Menu", EStageType.MENU);
        }
    }


    // 対戦相手捜索画面の非表示化が完了(戻る押下時)
    public void OnDidHideOpponentSearcherForBack()
    {
        GoToMenu();
    }


    // 対戦相手捜索画面の非表示化が完了(対戦開始時)
    public void OnDidHideOpponentSearcherForGame()
    {
        //Debug.Log(this.GetType().ToString() + ": onDidHideOpponentSearcherForGame()");
        if(!isFriendMatch()) {
            Auth.instance.GetUser().decrementLife();
            //Debug.Log("decrementedLife: " + Auth.instance.GetUser().nGamesLeftWithReward());
            AppManager.Instance.SetLife(count: Auth.instance.GetUser().nGamesLeftWithReward());
        }
        position.Set(game.sfen);
        SetupGameStage();
    }


    public void OnTapButtonBoardOnGameResult()
    {
        //Debug.Log(this.GetType().ToString() + ": OnTapButtonBoardOnGameResult()");
    }


	public void OnTapButtonHistoryDetail()
	{
        // 対局結果画面において「検討」ボタンを押したときの処理を記述してください
        GameManager.instance.globalVariables.gameForHistoryDetail = game;

        GameManager.instance.globalVariables.didComeFromGameScene = true;
        gameStage.HideBoardSetAndChangeScene("historyDetail", EStageType.HISTORY_DETAIL);

	}
    

    // 対局結果表示で「盤面」表示後、対局結果表示に戻る
    public void OnTapButtonBoardToGameResultOnGameResult()
    {
        //Debug.Log(this.GetType().ToString() + ": OnTapButtonBoardToGameResultOnGameResult()");
    }


    // 対局結果表示で「詳細」押下
    public void OnTapButtonDetailOnGameResult()
    {
        //Debug.Log(this.GetType().ToString() + ": OnTapButtonDetailOnGameResult()");
    }


    // 対局結果表示で「詳細」表示後、対局結果表示に戻る
    public void OnTapButtonDetailToGameResultOnGameResult()
    {
        //Debug.Log(this.GetType().ToString() + ": OnTapButtonDetailToGameResultOnGameResult()");
    }

    public void onUserEarnedReward() {
        Auth.instance.GetUser().incrementAdLife(3);
        AppManager.Instance.SetLife(count: Auth.instance.GetUser().nGamesLeftWithReward());
        keepPlayingGame = true;
        gameStage.HideBoardSetAndChangeScene("Game", EStageType.GAME);
    }

    private void FailedToLoadAd() {
        Destroy(loadingView.gameObject);
        AlertViewController.ShowOKConfirmation("広告の読み込みに失敗しました。しばらく時間を置いてからお試しください。");
    }

    private void ShowAd() {
        Destroy(loadingView.gameObject);
        adManager.onUserEarnedReward = onUserEarnedReward;
        adManager.Show();
    }

    private void LoadAd() {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("広告をロード中...", false, () => { });
        adManager.onAdLoaded = ShowAd;
        adManager.onLoadAdFailed = FailedToLoadAd;
        adManager.LoadAd();
    }


    // 対局結果表示で「次の対局へ／友達ともう一度対局する」押下
    public void OnTapButtonNextGameOnGameResult()
    {
        // 勝ち切れ乱将棋では「正解手順」ボタン。検討画面を開き、初期局面の読み筋を出す
        if (IsAiGame && aiSetting.kachikireDate != null) {
            ShowKachikireAnswer();
            return;
        }

        // AI対戦では「戻る」ボタンになっている。始めた画面へ戻す。
        if (IsAiGame) {
            ReturnFromAiGame();
            return;
        }

        if(Auth.instance.GetUser().nGamesLeftWithReward() == 0 && !Auth.instance.GetUser().isPayedUser && !isFriendMatch()) {
            //AlertViewController.ShowOKConfirmation("対局は1日3回までです。 広告を見ることで引き続き対局できます。", AlertViewController.Type.YesNo, LoadAd);
            SubscriptionView view = FindFirstObjectByType<SubscriptionView>();
            view.SetOnUserEarnedReward(onUserEarnedReward);
            view.ShowAdOrBuySubscriptionConfirmation();

        } else {
            keepPlayingGame = true;
            gameStage.HideBoardSetAndChangeScene("Game", EStageType.GAME);
        }
    }


    // 対局結果表示で「HOME」押下。勝ち切れ乱将棋では「戻る」ボタンになっている
    public void OnTapButtonHomeOnGameResult()
    {
        //Debug.Log(this.GetType().ToString() + ": OnTapButtonHomeOnGameResult()");
        if (IsAiGame) {
            ReturnFromAiGame();
            return;
        }
        gameStage.HideBoardSetAndChangeScene("Menu", EStageType.MENU);
    }


    /// 勝ち切れ乱将棋の「正解手順」。検討画面を開き、初期局面の読み筋(正解手順)を出しておく
    private void ShowKachikireAnswer()
    {
        GameManager.instance.globalVariables.gameForHistoryDetail = game;
        GameManager.instance.globalVariables.didComeFromGameScene = true;
        GameManager.instance.globalVariables.showAiEvaluationOnOpen = true;
        gameStage.HideBoardSetAndChangeScene("historyDetail", EStageType.HISTORY_DETAIL);
    }



    // ===============================================
    // MARK: - ● Window ─────

    // ゲーム結果ウィンドウでOK押下
    public void OnCloseGameResultWindow()
    {
        //Debug.Log(this.GetType().ToString() + ": onCloseGameResultWindow()");
    }

    // 注意ウィンドウでOK押下
    public void OnCloseNoticeWindow()
    {
        //Debug.Log(this.GetType().ToString() + ": onCloseNoticeWindow()");
    }

    // エラーウィンドウでOK押下
    public void OnCloseErrorWindow()
    {
        //Debug.Log(this.GetType().ToString() + ": onCloseErrorWindow()");
    }

    void OnDestroy() {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        if (!IsAiGame) wsm.Close();
    }

    private void ResetTapState(bool withResetCells = true)
    {
        if(withResetCells) {
            ResetCells();
        }
        position.ResetEmphasizedCells();
        tappedSquare = Square.SQ_Invalid;
        tappedHandPiece = Piece.No_Piece;
    }

    public void onTapOld(Square sq)
    {
    }


    void SetSfen(string sfen) { 
    }

    void OnReceiveMessage(byte[] rawData, string data) {
        if (gameState == GameState.OnSearchingOpponent || gameState == GameState.Preparing && data != null) {
            try {
                game = JsonUtility.FromJson<Game>(data);
            } catch {
                Debug.Log("not game data");
                Debug.Log(data);
            }
            gameStage.HideOpponentSearcherForGame();
        } else if(gameState == GameState.OnGame || gameState == GameState.GameFinishedWaitingServerResponse) {
            GameMessageFlag flag = (GameMessageFlag)rawData[0];
            switch(flag) {

                case GameMessageFlag.Move:
                case GameMessageFlag.MateMove:
                    if (gameState != GameState.OnGame) return;
                    Move m = new Move();
                    m.move = (Move32)(rawData[1] + (rawData[2] << 8) + (rawData[3] << 16) + (rawData[4] << 24));
                    //position.DoMove(m);
                    //gameViewController.DoMove(m, true, position.InCheck(), (Bitboard)position.checkersBB.Clone());
                    //Debug.Log("Move Received. from: " + m.from().ToString() + " to: " + m.to().ToString());
                    DoMove(m);
                    //Debug.Log("isMated");
                    //Debug.Log(position.IsMated());
                    //Debug.Log("next sidetomove");
                    //Debug.Log(position.sideToMove);

                    break;
                case GameMessageFlag.ChangeGameState:
                    //Debug.Log("change game state");
                    if (gameState != GameState.OnGame && gameState != GameState.GameFinishedWaitingServerResponse ) return;
                    GameState newGameState = (GameState)rawData[1];
                    //Debug.Log(newGameState + " hoge");
                    gameState = newGameState;
                    if(newGameState != GameState.GameFinished) return;

                    GameFinishReason reason = (GameFinishReason)rawData[2];
                    GameResult result = (GameResult)rawData[3];
                    int newRating = rawData[4] * 256 + rawData[5];
                    FinishGame(reason, result, newRating);
                    //SceneManager.LoadScene("Menu");
                    break;
                case GameMessageFlag.SyncTime:
                    blackTime = (float)rawData[2] * 256 + (float)rawData[3] + ((float)rawData[4] / 1000f);
                    whiteTime = (float)rawData[5] * 256 + (float)rawData[6] + ((float)rawData[7] / 1000f);

                    displayBlackTime = (int)blackTime;
                    displayWhiteTime = (int)whiteTime;

                    gameStage.SetTimeLeft(Color.Black, displayBlackTime / 60, displayBlackTime % 60);
                    gameStage.SetTimeLeft(Color.White, displayWhiteTime / 60, displayWhiteTime % 60);

                    //gameViewController.SetBlackTime(displayBlackTime);
                    //gameViewController.SetWhiteTime(displayWhiteTime);

                    break;
                default: 
                    break;
            }
        }
    }

    void FinishGame(GameFinishReason reason, GameResult result, int newRating) {
        gameStage.HideTextBalloonError();
        gameState = GameState.GameFinished;
        game.SetMoves(moves);
        game.gameFinishReason = reason;
        game.gameResult = result;
        if (IsAiGame) {
            // AI対戦はサーバを通らないので、終了時刻をここで入れる。
            // 入れないと、検討画面の「◯◯日前」が DateTime.MinValue から計算されてしまう
            game.gameFinishedAt = DateTime.Now;
        }
        if(position.myColor == Color.Black) {
            game.rateDiff = Math.Abs(newRating - game.blackRating);
            game.blackUid = Auth.instance.GetUser().firebaseUid;
       } else {
            game.rateDiff = Math.Abs(newRating - game.whiteRating);
            game.whiteUid = Auth.instance.GetUser().firebaseUid;
        }
        if(result == GameResult.BlackWon) {
            game.blackRateTo = game.blackRating + game.rateDiff;
            game.whiteRateTo = game.whiteRating - game.rateDiff;
        } else if(result == GameResult.WhiteWon) {
            game.whiteRateTo = game.whiteRating + game.rateDiff;
            game.blackRateTo = game.blackRating - game.rateDiff;
        } else {
            game.whiteRateTo = game.whiteRating;
            game.blackRateTo = game.blackRating;
        }
 
        int previousRating = Auth.instance.GetUser().RatingOf(GameManager.instance.globalVariables.boardType);
        //gameViewController.FinishGame(reason, result, position.myColor, newRating);
        //Debug.Log(Auth.instance.GetUser());
        if (!IsAiGame) {
            // AI対戦はレーティングに影響させない。
            Auth.instance.SetNewRating(newRating, GameManager.instance.globalVariables.boardType);
        }
        GameManager.instance.globalVariables.gameForReconnection = null;

        EGameResultType gameResult = EGameResultType.DRAW;
        if((result == GameResult.WhiteWon && position.myColor == Color.White) || (result == GameResult.BlackWon && position.myColor == Color.Black)) {
            gameResult = EGameResultType.WIN;
        }
        if ((result == GameResult.WhiteWon && position.myColor == Color.Black) || (result == GameResult.BlackWon && position.myColor == Color.White)) {
            gameResult = EGameResultType.LOSE;
        }

        didShowResult = true;

        Coroutine coroutine = StartCoroutine(DelayMethod(1.0f, () => {
            gameStage.ShowGameResult(
            gameResultType: gameResult,
            gameResultReasonType: ConvertResultReasonType(reason),
            consecutiveCount: 2,
            previousRate: previousRating,
            nextRate: newRating,
            previousGradeRankType: G.RateToGradeRankType(previousRating),
            nextGradeRankType: G.RateToGradeRankType(newRating)
            );
        }));
    }

    void MatchingFinished(Game game) {
        gameState = GameState.Preparing;
        wsm.Close();
        wsm.ConnectGame(game);
    }

    void OnConnect() {
        gameStage.HideTextBalloonError();
        isClosed = false;
        if(GameManager.instance.globalVariables.gameForReconnection != null) {
            //Debug.Log("####reconnect do moves");
            gameState = game.state;
            position.Set(game.sfen);
            foreach(Move m in game.moves()) {
                position.DoMove(m);
            }
            moves = game.moves();
            GameManager.instance.globalVariables.gameForReconnection = null;
            SetupGameStage();
        } else {
            if(shouldSendLatestMove) { 
                //Debug.Log("####shouldSendLatestMove");
                shouldSendLatestMove = false;
                SendMove(currentMove);
            }
            if(shouldDoLatestMove) {
                //Debug.Log("####shouldDoLatestMove");
                shouldDoLatestMove = false;
                DoMove(latestMoveShouldBeDone);
            }
        }　

    }

    void OnClose(string errorMessage) {
        isClosed = true;
        //Debug.Log("On Close");
        Debug.Log("errorMessage: " + errorMessage);
        if(gameState == GameState.OnSearchingOpponent && !backButtonTapped) {
            GameManager.instance.globalVariables.alertMessageOnMenu = "サーバでエラーが発生しました。しばらく時間を置いて再度お試しください。";
            gameStage.HideOpponentSearcherForBackAndChangeScene("Menu", EStageType.MENU);
        }
        if(gameState == GameState.OnGame) {
            gameStage.ShowTextBalloonError("接続エラー。再接続中...");
            StartCoroutine(ReconnectCoroutine());
        }
        if(applicationFocus) {
            //TryReconnect();
        }
    }

    IEnumerator ReconnectCoroutine() {
        while(isClosed) {
            yield return new WaitForSeconds(5.0f);
            TryReconnect();
        }
    }

    private void GoToMenu() { 
        // シーンを抜ける前に破壊処理を行う
        if(isFriendMatch()) {
            SceneManager.LoadScene("Friend");
        } else {
            SceneManager.LoadScene("Menu");
        }
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    private EGameResultReasonType ConvertResultReasonType(GameFinishReason reason) {

        switch(reason) {
            case GameFinishReason.Mate: return EGameResultReasonType.CHECKMATE;
            case GameFinishReason.PlayerResigned: return EGameResultReasonType.RESIGNATION;
            case GameFinishReason.PlayerTimeout: return EGameResultReasonType.TIMEUP;
            case GameFinishReason.PlayerDisconnect: return EGameResultReasonType.TIMEUP;
            case GameFinishReason.Try: return EGameResultReasonType.TRY;
            case GameFinishReason.RepetitionOfMovesDraw: return EGameResultReasonType.REPETITION_OF_MOVES;
            case GameFinishReason.RepetitionOfMovesCheckFoul: return EGameResultReasonType.REPETITION_OF_MOVES;
            case GameFinishReason.LimitOfMoves: return EGameResultReasonType.LIMIT_OF_MOVES;
            default: return EGameResultReasonType.CHECKMATE;
        }
    }

    // ===============================================
    // MARK: - ● 対局中設定 ─────

    // 対局中設定で「音を消す」を操作
    public void OnTapPlayAudio(bool isOn)
    {
        Debug.Log(this.GetType().ToString() + ": OnTapPlayAudio(): " + isOn.ToString());
        // ユーザ設定を記録してください               var isCheckBoxOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxMaster");
        AppManager.Instance.AudioController.SetIsMuteMaster(value: !isOn);
        PlayerPrefs.SetInt("IsMuteMaster", isOn ? 0 : 1);
    }


    // 対局結果表示で「相手の名前を表示」を操作
    public void OnTapShowNamePlayer2(bool isOn)
    {
        Debug.Log(this.GetType().ToString() + ": OnTapShowNamePlayer2(): " + isOn.ToString());
        // ユーザ設定を記録してください
        PlayerPrefs.SetInt("ShowOpponentName", isOn ? 1 : 0);
        if (isOn) {
            Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER2);
        } else {
            Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER2);
        }
    }


    // 対局結果表示で「自分の名前を表示」を操作
    public void OnTapShowNamePlayer1(bool isOn)
    {
        Debug.Log(this.GetType().ToString() + ": OnTapShowNamePlayer1(): " + isOn.ToString());
        // ユーザ設定を記録してください
        PlayerPrefs.SetInt("ShowPlayerName", isOn ? 1 : 0);
        if (isOn) {
            Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER1);
        } else {
            Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER1);
        }
    }

    // ===============================================
    // MARK: - ● AI対戦(端末内エンジン) ─────

    /// 棋譜の局面からAI対戦を開始する。サーバーには接続しない。
    private void SetupAiGame()
    {
        var user = Auth.instance.GetUser();
        int rating = user.RatingOf(GameManager.instance.globalVariables.boardType);

        // サーバーから来るGameの代わりに、ローカルで対局データを組み立てる。
        game = new Game();
        game.sfen = aiSetting.rootSfen;
        game.gameNum = aiSetting.gameNum;
        // 勝ち切れ乱将棋の正解手順。検討画面の「AI評価値」が、端末で読まずにこれを出す
        game.pv = aiSetting.kachikirePv;
        game.boardType = "ranshogi";
        game.gameStartedAt = DateTime.Now;
        // 対局が終わるときに入れ直す。対局中に検討画面を開いても「◯◯日前」が壊れないようにしておく
        game.gameFinishedAt = DateTime.Now;
        game.blackNickname = aiSetting.playerColor == Color.Black ? aiSetting.PlayerName : aiSetting.AiName;
        game.whiteNickname = aiSetting.playerColor == Color.Black ? aiSetting.AiName : aiSetting.PlayerName;
        // レーティングは動かさないので、両者とも現在値にしておく(結果画面で増減0になる)。
        game.blackRating = rating;
        game.whiteRating = rating;
        if (aiSetting.playerColor == Color.Black) {
            game.blackUid = user.firebaseUid;
        } else {
            game.whiteUid = user.firebaseUid;
        }

        // 評価関数の読み込み(0.3秒ほど)を初手の思考より前に済ませておく。
        _ = RanshogiEngine.InitAsync();

        position.Set(aiSetting.rootSfen);
        // 棋譜のsfenには「その棋譜での自分の色」が入っているので、選んだ側で上書きする。
        position.myColor = aiSetting.playerColor;
        position.opponentColor = G.Not(aiSetting.playerColor);

        // 待ったで戻せるのは、この対局で指した手だけ(開き直しても最初の手数を覚えておく)。
        if (aiSetting.startMoveCount < 0) {
            aiSetting.startMoveCount = aiSetting.initialMoves.Count;
        }

        // 検討画面で表示していた手数まで進める。
        foreach (var m in aiSetting.initialMoves) {
            // sfen由来の指し手は動かす駒が入っていないことがある。
            // 入っていないと Position.DoMove() が玉の移動を認識できないので盤面から補完する。
            Move move = m;
            if (!move.isDrop() && move.movedPiece() == Piece.No_Piece) {
                move = G.MakeMove(move.from(), move.to(), position.Get(move.from()), move.isPromote(), false);
            }
            position.DoMove(move);
            moves.Add(move);
        }

        SetupGameStage();
    }


    /// エンジンに手番を渡して指し手を貰う。UIスレッドは止めない。
    private async void RequestAiMove()
    {
        if (isAiThinking) return;
        isAiThinking = true;
        try {
            if (!RanshogiEngine.IsReady) {
                bool ok = await RanshogiEngine.InitAsync();
                if (!ok) {
                    gameStage.ShowTextBalloonError("AIの準備に失敗しました");
                    return;
                }
            }

            float startedAt = Time.realtimeSinceStartup;

            string rootSfen = UsiConverter.ToEngineSfen(aiSetting.rootSfen);
            // 負けが確定したあとは、王手以外の最善手を指させる。
            // (負けを引き延ばすためだけの王手の連続を避ける。勝敗がつくまでの強さは変わらない)
            string best = await RanshogiEngine.BestMoveAsync(rootSfen, UsiConverter.ToUsi(moves), aiSetting.level,
                                                            AiMaxThinkMilliseconds, avoidChecksWhenLost: true);

            // エンジンは一瞬で指すことがあるので、人が相手をしている感覚になるよう
            // 考えている時間を最低 AiMinThinkSeconds 秒は取る(探索が長ければ待ち足さない)。
            float waitSeconds = UnityEngine.Random.Range(AiMinThinkSeconds.min, AiMinThinkSeconds.max)
                - (Time.realtimeSinceStartup - startedAt);
            if (waitSeconds > 0f) {
                await Task.Delay(TimeSpan.FromSeconds(waitSeconds));
            }

            // 思考中に投了や画面遷移で対局が終わっていたら破棄する。
            if (this == null || gameState != GameState.OnGame) return;

            if (best == null) {
                gameStage.ShowTextBalloonError("AIの思考でエラーが発生しました");
                return;
            }

            if (best == "resign" || best == "win") {
                FinishAiGame(GameFinishReason.PlayerResigned,
                    position.myColor == Color.Black ? GameResult.BlackWon : GameResult.WhiteWon);
                return;
            }

            DoMove(UsiConverter.FromUsi(best, position));
        } finally {
            isAiThinking = false;
        }
    }


    /// AI対戦の決着。サーバーからの通知が無いので自前で結果画面まで進める。
    private void FinishAiGame(GameFinishReason reason, GameResult result)
    {
        if (didShowResult) return;
        gameState = GameState.GameFinished;
        // レートは動かさないので、現在値をそのまま渡す(増減0で表示される)。
        FinishGame(reason, result, Auth.instance.GetUser().RatingOf(GameManager.instance.globalVariables.boardType));
        RecordKachikireClearIfNeeded(result);
    }


    /// 勝ち切れ乱将棋で勝ったら、サーバに勝利を記録する(メニューのカレンダーにマークがつく)。
    private void RecordKachikireClearIfNeeded(GameResult result)
    {
        if (aiSetting.kachikireDate == null) return;

        bool isPlayerWon = (result == GameResult.BlackWon && position.myColor == Color.Black)
            || (result == GameResult.WhiteWon && position.myColor == Color.White);
        if (!isPlayerWon) return;

        var request = new KachikireClearRequest {
            _requestJson = JsonUtility.ToJson(new KachikireClearRequest.Clear { date = aiSetting.kachikireDate }),
        };
        Api.RecordKachikireClear(request, (_) => { }, (errorMessage) => {
            Debug.LogWarning("[Kachikire] 勝利の記録に失敗しました: " + errorMessage);
            gameStage.ShowTextBalloonError("勝利の記録に失敗しました");
        });
    }

}

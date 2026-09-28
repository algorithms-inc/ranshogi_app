using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using GFramework.Core;


public class HistoryDetailViewController : MonoBehaviour, IHistoryDetailManager
{
    const string UnsupportedPositionMessageForEvaluation = "この対局は以前のルールの局面のため、AI評価値には対応していません。";

    Position position;
    Game game;

    // 本譜(対局で実際に指された手)
    List<Move> actualMoves = new List<Move>();
    // 表示している手順。本譜か、駒を動かしたり読み筋を再生したりしてできた変化
    List<Move> moves = new List<Move>();
    int ply = 0;

    private IHistoryDetailStage _historyDetailStage = null;

    // 駒の選択状態(対局画面の GameController と同じ持ち方)
    Square tappedSquare = Square.SQ_Invalid;
    Piece tappedHandPiece = Piece.No_Piece;
    bool isChoosingPromotion = false;

    // 吹き出しに出している読み筋
    List<Move> shownPvMoves = new List<Move>();
    bool isEvaluating = false;


    void Start()
    {
        AppManager.Instance.Setup();

        game = GameManager.instance.globalVariables.gameForHistoryDetail;
        actualMoves = game.moves();
        moves = actualMoves;

        // AI対戦から戻ってきたときは、対戦を始めたときに表示していた手順・手数に戻す
        var restoreMoves = GameManager.instance.globalVariables.historyDetailMoves;
        int restorePly = GameManager.instance.globalVariables.historyDetailPly;
        GameManager.instance.globalVariables.historyDetailMoves = null;
        GameManager.instance.globalVariables.historyDetailPly = 0;
        if (restoreMoves != null) {
            moves = restoreMoves;
        }

        position = CreatePosition(0);
        SetupStage();

        if (restoreMoves != null || restorePly > 0) {
            ShowNPly(Mathf.Clamp(restorePly, 0, moves.Count));
        }

        // 勝ち切れ乱将棋の結果画面の「正解手順」から開いたときは、初期局面の読み筋を出して、
        // そのまま手順を再生した状態にしておく (進む/戻るでなぞれる)
        if (GameManager.instance.globalVariables.showAiEvaluationOnOpen) {
            GameManager.instance.globalVariables.showAiEvaluationOnOpen = false;

            // 正解手順を持っていない古い局面では、端末で読ませない。AI機能の1日3回を黙って使ってしまうため
            if (string.IsNullOrWhiteSpace(game.pv) == false) {
                // サーバの読み筋を使うので、この呼び出しは待たずに読み筋が入る
                EvaluateCurrentPosition();
                OnTapButtonReplayAiVariation();
            }
        }
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    private void SetupStage()
    {
        _historyDetailStage = AppManager.Instance.CreateHistoryDetailStage(historyDetailManager: this, game.BoardType());

        // 現在の手数・最大手数を設定
        _historyDetailStage.SetPresentMoveNum(value: 0);
        _historyDetailStage.SetMaxMoveNum(value: moves.Count);

        // ページ操作のボタンの初期状態は Disabled であるため、必要なボタンを Enabled 状態にする
        if (moves.Count >= 1)
        {
            _historyDetailStage.SetButtonStepForwardEnabled();
            _historyDetailStage.SetButtonJumpToLastEnabled();
        }

        var playerColor = game.blackUid == Auth.instance.GetUser().firebaseUid ? Color.Black : Color.White;
        var result = G.GameResultToEHistoryItemResult(game.gameResult, playerColor);

        var now = DateTime.Now;
        var interval = now - game.gameFinishedAt;
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

        var gameResultReasonType = EGameResultReasonType.UNKOWN;
        switch(game.gameFinishReason) {
            case GameFinishReason.Mate:
                gameResultReasonType = EGameResultReasonType.CHECKMATE;
                break;
            case GameFinishReason.Try:
                gameResultReasonType = EGameResultReasonType.TRY;
                break;
            case GameFinishReason.PlayerTimeout:
                gameResultReasonType = EGameResultReasonType.TIMEUP;
                break;
            case GameFinishReason.PlayerResigned:
                gameResultReasonType = EGameResultReasonType.RESIGNATION;
                break;
            case GameFinishReason.RepetitionOfMovesDraw:
                gameResultReasonType = EGameResultReasonType.REPETITION_OF_MOVES;
                break;
        }

        var historyDetailSetting = new FHistoryItemInitializer(
            historyDetailID: game.id,
            gameNum: game.gameNum,
            boardType: game.BoardType(),
            historyItemResult: result,
            gameResultReasonType: gameResultReasonType,
            date: game.gameStartedAt.ToString( "yyyy/MM/dd HH:mm:ss" ),
            dateAdditionalInfo: dateAdditionalInfo,
            color: playerColor,
            playerName: playerColor == Color.Black ? game.blackNickname : game.whiteNickname,
            opponentName: playerColor == Color.Black ? game.whiteNickname : game.blackNickname,
            gradeRankType: G.RateToGradeRankType(game.RatingOf(playerColor)),
            opponentGradeRankType: G.RateToGradeRankType(game.RatingOf(G.Not(playerColor))),
            previousRate: (playerColor == Color.Black ? game.blackRateTo : game.whiteRateTo) + (result == EHistoryItemResult.WIN ? -game.rateDiff : game.rateDiff),
            nextRate: playerColor == Color.Black ? game.blackRateTo : game.whiteRateTo,
            previousGradeRankType: G.RateToGradeRankType(playerColor == Color.Black ? game.blackRating : game.whiteRating),
            nextGradeRankType: G.RateToGradeRankType(playerColor == Color.Black ? game.blackRateTo : game.whiteRateTo),
            canExportData: false,  // TODO: 棋譜データダウンロード機能が実装されたのちは、必要に応じて true を設定してください
            isEnabled: true
            );

        _historyDetailStage.SetupGameInfo(historyDetailSetting: historyDetailSetting);

        var player1Data = new FPlayerData(
            playerName: position.myColor == Color.Black ? game.blackNickname : game.whiteNickname,
            rate: game.RatingOf(playerColor),
            gradeRankType: G.RateToGradeRankType(game.RatingOf(playerColor)),
            color: position.myColor);

        var player2Data = new FPlayerData(
            playerName: position.myColor == Color.Black ? game.whiteNickname : game.blackNickname,
            rate: game.RatingOf(G.Not(playerColor)),
            gradeRankType: G.RateToGradeRankType(game.RatingOf(G.Not(playerColor))),
            color: position.myColor == Color.Black ? Color.White : Color.Black);

        var (p1List, p2List) = position.PieceLists();

        _historyDetailStage.SetupBoardSet(
             moveNum: 1,
             boardType: game.BoardType(),
             player1Data: player1Data,
             player1PieceList: p1List,
             player2Data: player2Data,
             player2PieceList: p2List);


        _historyDetailStage.ShowWithAnimation();
    }

    public void OnTapButtonBack()
    {
        if(GameManager.instance.globalVariables.didComeFromGameScene) {
            _historyDetailStage.HideWithAnimationAndChangeScene(nextSceneName: "Menu", nextStageType: EStageType.MENU);
        } else {
            _historyDetailStage.HideWithAnimationAndChangeScene(nextSceneName: "History", nextStageType: EStageType.HISTORY);
        }
    }


    public void OnTapButtonStepForward()
    {
        if (ply < moves.Count) ShowNPly(ply + 1);
    }


    private void ResetButtonStates() {
        // 必要に応じてボタンの使用可否を変更する
        if(moves.Count == ply) {
            _historyDetailStage.SetButtonStepForwardDisabled();
            _historyDetailStage.SetButtonJumpToLastDisabled();
        } else {
            _historyDetailStage.SetButtonStepForwardEnabled();
            _historyDetailStage.SetButtonJumpToLastEnabled();
        }
        if(ply > 0) {
            _historyDetailStage.SetButtonStepBackEnabled();
            _historyDetailStage.SetButtonJumpToFirstEnabled();
        } else {
            _historyDetailStage.SetButtonStepBackDisabled();
            _historyDetailStage.SetButtonJumpToFirstDisabled();
        }
    }


    public void OnTapButtonStepBack()
    {
        if (ply > 0) ShowNPly(ply - 1);
    }


    public void OnTapButtonJumpToFirst()
    {
        ShowNPly(0);
    }


    public void OnTapButtonJumpToLast()
    {
        ShowNPly(moves.Count);
    }


    public void ShowNPly(int n) {
        ply = n;

        ResetTapState();
        _historyDetailStage.HideAiEvaluationBalloon();

        position = CreatePosition(ply);
        var (p1List, p2List) = position.PieceLists();

        _historyDetailStage.SetMaxMoveNum(value: moves.Count);
        _historyDetailStage.ChangeMoveNumAndPiece(
               moveNum: ply,
               player1PieceList: p1List,
               player2PieceList: p2List);

        ResetButtonStates();
        _historyDetailStage.SetButtonBackToActualPlayEnabled(value: !IsOnActualPlay());
    }


    // 表示している手順の n 手目までを並べた局面を作る
    private Position CreatePosition(int n)
    {
        Position pos = game.BoardType() == EBoardType.BOARD_66 ? (Position)new Position66() : new Position99();
        pos.Set(game.sfen);
        for (int i = 0; i < n; i++) {
            pos.DoMove(moves[i]);
        }
        return pos;
    }


    // 表示している手順が本譜から分かれた手数。本譜のままなら int.MaxValue
    private int BranchPly()
    {
        if (ReferenceEquals(moves, actualMoves)) return int.MaxValue;

        int n = Math.Min(moves.Count, actualMoves.Count);
        for (int i = 0; i < n; i++) {
            if (!IsSameMove(moves[i], actualMoves[i])) return i;
        }
        return moves.Count == actualMoves.Count ? int.MaxValue : n;
    }


    // サーバの棋譜と盤で指した手は、持っている情報(取った駒など)が違うことがあるので、指し手として同じかだけを見る
    private static bool IsSameMove(Move a, Move b)
    {
        if (a.isDrop() != b.isDrop() || a.to() != b.to() || a.isPromote() != b.isPromote()) return false;
        return a.isDrop() ? G.TypeOf(a.movedPiece()) == G.TypeOf(b.movedPiece()) : a.from() == b.from();
    }


    // ===============================================
    // MARK: - ● 駒を動かす ─────

    // 盤上の駒がないセルをタップ
    public void OnTap(Square sq)
    {
        if (isChoosingPromotion) return;

        if (position.emphasizedCells[(int)sq]) {
            DoMoveToTappedSquare(sq);
        } else {
            ResetTapState();
        }
    }


    // 盤上の駒をタップ
    public void OnTap(Piece piece, Square sq)
    {
        if (isChoosingPromotion) return;

        if (position.emphasizedCells[(int)sq]) {
            DoMoveToTappedSquare(sq);
            return;
        }

        // 選択中の駒をもう一度タップしたら選択をやめる
        bool isSameSquare = tappedSquare == sq;
        ResetTapState();
        if (isSameSquare) return;

        // 手番側の駒だけ動かせる
        Piece pc = position.Get(sq);
        if (pc == Piece.No_Piece || G.ColorOf(pc) != position.sideToMove) return;

        List<Square> squares = position.MovableBB(pc, sq).ToSquares();
        if (squares.Count == 0) return;

        tappedSquare = sq;
        foreach (Square msq in squares) {
            position.emphasizedCells[(int)msq] = true;
        }
        _historyDetailStage.SelectPiece(
            position.sideToMove,
            position.SquareToDesignSquare((int)sq),
            squares.Select(s => position.SquareToDesignSquare((int)s)).ToList());
    }


    // 駒台の駒をタップ
    public void OnTapOnWaitingSpace(Piece piece)
    {
        if (isChoosingPromotion) return;

        bool isSamePiece = tappedHandPiece == piece;
        ResetTapState();
        if (isSamePiece || G.ColorOf(piece) != position.sideToMove) return;

        // 持っていない駒の枠をタップしたとき
        int[] hand = position.sideToMove == Color.Black ? position.blackHand : position.whiteHand;
        if (hand[G.HandIndexOf(piece)] == 0) return;

        List<Square> squares = position.DropableBBOf(piece).ToSquares();
        if (squares.Count == 0) return;

        tappedHandPiece = piece;
        foreach (Square msq in squares) {
            position.emphasizedCells[(int)msq] = true;
        }
        _historyDetailStage.SelectPiece(
            position.sideToMove,
            G.TypeOf(piece),
            squares.Select(s => position.SquareToDesignSquare((int)s)).ToList());
    }


    // 盤・駒台の外をタップ
    public void OnTapOutOfBoard()
    {
        if (isChoosingPromotion) return;
        ResetTapState();
    }


    // 選択中の駒を、タップしたマスへ動かす(打つ)
    private void DoMoveToTappedSquare(Square to)
    {
        Move m = tappedSquare != Square.SQ_Invalid
            ? G.MakeMove(tappedSquare, to, position.Get(tappedSquare), false, false)
            : G.MakeMove(Square.SQ_Invalid, to, tappedHandPiece, false, true);
        ResetTapState();

        if (m.isDrop()) {
            PlayMove(m);
            return;
        }
        if (position.MustPromote(m.movedPiece(), m.to(), position.sideToMove)) {
            m.setPromote();
            PlayMove(m);
            return;
        }
        if (position.CanPromote(m.movedPiece(), m.from(), m.to(), position.sideToMove)) {
            isChoosingPromotion = true;
            _historyDetailStage.ShowWindowPromotion(onSelect: (promote) => {
                isChoosingPromotion = false;
                if (promote) m.setPromote();
                PlayMove(m);
            });
            return;
        }
        PlayMove(m);
    }


    // 指した手で一手進める。表示中の手順の次の手と違うときは、そこから先を新しい変化にする
    private void PlayMove(Move m)
    {
        if (ply < moves.Count && IsSameMove(moves[ply], m)) {
            ShowNPly(ply + 1);
            return;
        }

        var newMoves = moves.GetRange(0, ply);
        newMoves.Add(m);
        moves = newMoves;
        ShowNPly(ply + 1);
    }


    private void ResetTapState()
    {
        _historyDetailStage.DeselectPiece();
        position.ResetEmphasizedCells();
        tappedSquare = Square.SQ_Invalid;
        tappedHandPiece = Piece.No_Piece;
    }


    // ===============================================
    // MARK: - ● AI評価値 ─────

    /// 「AI評価値」ボタンが押されたときの処理
    public void OnTapButtonAiEvaluation()
    {
        if (_historyDetailStage.IsAiEvaluationBalloonShown) {
            _historyDetailStage.HideAiEvaluationBalloon();
            return;
        }
        if (!RanshogiEngine.IsSupportedPosition(game.sfen)) {
            AlertViewController.ShowOKConfirmation(UnsupportedPositionMessageForEvaluation);
            return;
        }
        EvaluateCurrentPosition();
    }


    // いま表示している局面をAIに読ませて吹き出しに出す。一度読んだ局面は端末に保存してあるものを出す
    private async void EvaluateCurrentPosition()
    {
        if (isEvaluating) return;

        if (position.IsMated()) {
            _historyDetailStage.ShowAiEvaluationBalloon(text: $"詰みです（{AiEvaluation.ColorName(position.sideToMove)}の負け）", canReplay: false);
            return;
        }

        // 開始局面は、サーバが局面を作ったときに深く読んだ読み筋を持っている。
        // 端末の思考は短いので読み筋が浅く、進めた先で形勢が変わることがある。サーバの読み筋の方が確かなのでそちらを使う
        // (端末で読まないので、1日3回の回数も消費しない)
        string serverPvInfo = ServerEvaluationOfStartPosition();
        if (serverPvInfo != null) {
            ShowEvaluation(serverPvInfo, headWithoutScore: "正解手順");
            return;
        }

        string key = CurrentEvaluationKey();
        string pvInfo = AiEvaluationCache.Get(key);
        if (pvInfo == null) {
            // AI機能は1日3回まで(読んだことのある局面は数えない)。超えたら広告のウィンドウを出し、広告を見終わったら読む
            if (!AiUsageLimit.TryUse(onUnlocked: () => { if (this != null) EvaluateCurrentPosition(); })) return;

            string rootSfen = UsiConverter.ToEngineSfen(game.sfen);
            var usiMoves = UsiConverter.ToUsi(moves.GetRange(0, ply));

            _historyDetailStage.ShowAiEvaluationBalloon(text: "AIが考えています…", canReplay: false);
            _historyDetailStage.SetButtonAiEvaluationEnabled(false);
            isEvaluating = true;

            RanshogiEngine.SearchResult result = null;
            try {
                if (await RanshogiEngine.InitAsync()) {
                    result = await RanshogiEngine.SearchAsync(rootSfen, usiMoves, AiEvaluation.SearchDepth, AiEvaluation.SearchMovetimeMs);
                }
            } finally {
                isEvaluating = false;
            }

            // 読んでいる間に画面を離れた
            if (this == null) return;
            _historyDetailStage.SetButtonAiEvaluationEnabled(true);

            if (result == null) {
                if (key == CurrentEvaluationKey()) {
                    _historyDetailStage.ShowAiEvaluationBalloon(text: "AIが読めませんでした", canReplay: false);
                }
                return;
            }

            pvInfo = result.PvInfo;
            AiEvaluationCache.Put(key, pvInfo);

            // 読んでいる間に局面を変えたときは、保存だけして出さない
            if (key != CurrentEvaluationKey()) return;
        }

        ShowEvaluation(pvInfo);
    }


    /// <summary>
    /// 開始局面を本譜のまま表示しているときだけ、サーバが持っている評価値と読み筋を
    /// エンジンの出力と同じ形("score cp .. pv ..")で返す。それ以外は null(端末で読む)。
    /// </summary>
    private string ServerEvaluationOfStartPosition()
    {
        if (ply != 0 || !IsOnActualPlay()) return null;
        if (string.IsNullOrWhiteSpace(game.pv)) return null;

        // 勝ち切れ乱将棋の正解手順は評価値を持っていないので、読み筋だけを返す
        if (!game.score.HasValue) return $"pv {game.pv.Trim()}";

        // サーバの評価値は先手から見た値。AiEvaluation は手番から見た値として読むのでそろえる
        int value = position.sideToMove == Color.Black ? game.score.Value : -game.score.Value;
        return $"score cp {value} pv {game.pv.Trim()}";
    }


    // 表示中の局面の、読み筋の保存に使うキー
    private string CurrentEvaluationKey()
    {
        string rootSfen = UsiConverter.ToEngineSfen(game.sfen);
        return AiEvaluationCache.Key(RanshogiEngine.PositionArgs(rootSfen, UsiConverter.ToUsi(moves.GetRange(0, ply))));
    }


    private void ShowEvaluation(string pvInfo, string headWithoutScore = null)
    {
        var evaluation = new AiEvaluation(pvInfo);
        Square previousTo = ply > 0 ? moves[ply - 1].to() : Square.SQ_Invalid;
        var (text, pvMoves) = evaluation.Describe(
            position: CreatePosition(ply), previousTo: previousTo, headWithoutScore: headWithoutScore);

        shownPvMoves = pvMoves;
        _historyDetailStage.ShowAiEvaluationBalloon(text: text, canReplay: pvMoves.Count > 0);
    }


    /// 吹き出しの「手順を再生」が押されたときの処理。いまの局面から読み筋を並べ、進むボタンで再生できるようにする。
    /// 読み筋は駒を動かしたときと同じく変化として扱い、「本譜」ボタンで本譜に戻る
    public void OnTapButtonReplayAiVariation()
    {
        if (shownPvMoves.Count == 0) return;

        var replayMoves = moves.GetRange(0, ply);
        replayMoves.AddRange(shownPvMoves);
        moves = replayMoves;
        ShowNPly(ply);
    }


    /// 「本譜」ボタンが押されたときの処理。変化をやめて、本譜から分かれた局面に戻す
    public void OnTapButtonBackToActualPlay()
    {
        int branchPly = BranchPly();
        if (branchPly == int.MaxValue) return;

        moves = actualMoves;
        ShowNPly(Math.Min(branchPly, actualMoves.Count));
    }


    // 本譜を表示しているか (駒を動かしたり読み筋を再生したりしてできた変化が無いか)
    private bool IsOnActualPlay() => BranchPly() == int.MaxValue;


    // ===============================================
    // MARK: - ● AIと対戦 ─────

    /// <summary>
    /// いま盤に表示している局面からAIと対戦する(駒を動かしたり読み筋を再生したりしていれば、その局面から)。
    /// 先後の選択と難易度はUI側から渡す。
    /// </summary>
    public void StartGameVsAI(Color playerColor, RanshogiEngine.Level level = RanshogiEngine.Level.Hard)
    {
        if (game.BoardType() != EBoardType.BOARD_66) {
            // エンジンが6x6専用のため。
            Debug.LogWarning("[Ranshogi] AI対戦は6x6のみ対応しています。");
            return;
        }

        var setting = new AiGameSetting {
            rootSfen     = game.sfen,
            initialMoves = moves.GetRange(0, ply),
            playerColor  = playerColor,
            level        = level,
            gameNum      = game.gameNum,
            PlayerName   = Auth.instance.GetUser().nickname,
            AiName       = "AI",
        };

        GameManager.instance.globalVariables.aiGame            = setting;
        GameManager.instance.globalVariables.historyDetailPly  = ply;
        GameManager.instance.globalVariables.historyDetailMoves = ReferenceEquals(moves, actualMoves) ? null : new List<Move>(moves);
        GameManager.instance.globalVariables.boardType         = game.BoardType();
        GameManager.instance.globalVariables.gameForReconnection = null;
        GameManager.instance.globalVariables.roomCode          = null;

        _historyDetailStage.HideWithAnimationAndChangeScene(nextSceneName: "Game", nextStageType: EStageType.GAME);
    }


    /// 「AIと対戦」ボタンが押されたときの処理。以前のルールの局面(エンジンで扱えない)なら案内だけ出す
    public void OnTapButtonAiMatch()
    {
        if (!RanshogiEngine.IsSupportedPosition(game.sfen)) {
            AlertViewController.ShowOKConfirmation(RanshogiEngine.UnsupportedPositionMessage);
            return;
        }
        _historyDetailStage.ShowWindowAiMatch();
    }


    /// 「AIと対戦」ウィンドウで先手/後手のボタンが押されたときの処理
    public void OnTapButtonStartAiMatch(Color playerColor, RanshogiEngine.Level level)
    {
        if (!RanshogiEngine.IsSupportedPosition(game.sfen)) {
            AlertViewController.ShowOKConfirmation(RanshogiEngine.UnsupportedPositionMessage);
            return;
        }
        // AI機能は1日3回まで。超えたら広告のウィンドウを出し、広告を見終わったら対局を始める
        if (!AiUsageLimit.TryUse(onUnlocked: () => { if (this != null) StartGameVsAI(playerColor, level); })) return;

        StartGameVsAI(playerColor, level);
    }


    public void OnTapExportData(EKifuDataType kifuDataType)
    {
        Debug.Log(GetType().ToString() + ": OnTapExportData" + kifuDataType.ToString());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using GFramework;

// こちらの using を挿入する必要あり
using GFramework.Core;


// ===============================================
// MARK: - ● SampleGameManager ─────

// GameManager では、インターフェースとして IGameManager を設定する必要あり
public class SampleGameManager : MonoBehaviour, IGameManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // GameStage への参照
    private IGameStage _gameStage = null;


    // -----------------------------------------------
    // MARK: - サンプルを動かすための変数

    // - 盤は66か99か
    private readonly EBoardType _boardType = EBoardType.BOARD_66;

    // - 友達との対局か
    private readonly bool _isWithFriend = false;

    // - 新しい対局か中断からの再開か
    private readonly bool _isNewGame = true;

    // - 盤の表示が完了しているか
    private bool _isBoardShown = false;

    // - 対局相手探索／友達の入室待ちフレーム数(frame/sec * sec)
    private int _presentWaitFrameForOpponentSearcher = 0;
    private int _endFrameForOpponentSearcher = default!;

    // - 対局内で各プレイヤーが擬似的な操作をするための経過時間管理
    private float _elapsedTime = 0;
    // - 各プレイヤーの操作管理用
    private int _commandIndex = 0;
    // - 成り選択待ち状態か
    private bool _isWaitingPromotionSelect = false;



    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        Debug.Log(this.GetType().ToString() + ": Awake()");

        if (Locator.Instance.debugManager.IsQuickFinishMatchMode)
        {
            _endFrameForOpponentSearcher = 1;
        }
        else
        {
            _endFrameForOpponentSearcher = 30 * 1;
        }

        SetupAppManager();
        SetupStage();

        // 新規対局の場合、対戦相手を探す表示を行う
        if (_isNewGame)
        {
            ShowOpponentSearcher();
        }
        else
        // 対局再開の場合、盤表示を行う
        {
            ShowBoardSet();
            _isBoardShown = true;
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
        switch (Locator.Instance.debugManager.BoardType)
        {
            case EBoardType.BOARD_66:
                _gameStage = AppManager.Instance.CreateGameStage(
                gameManager: this,
                boardType: _boardType,
                isWithFriendMode: _isWithFriend,
                isNewGame: _isNewGame);
                break;

            case EBoardType.BOARD_99:
                _gameStage = AppManager.Instance.CreateGameStage(
                gameManager: this,
                boardType: _boardType,
                isWithFriendMode: _isWithFriend,
                isNewGame: _isNewGame);
                break;

            default:
                throw new InvalidOperationException();
        }
    }


    // 対戦相手捜索画面を表示
    private void ShowOpponentSearcher()
    {
        _gameStage.ShowOpponentSearcher();
    }



    // ===============================================
    // MARK: - ● Opponent Searcher ─────

    // 対戦相手捜索画面で戻るを押下
    public void OnTapBackOnOpponentSearcher()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapBackOnOpponentSearcher()");
        
        if (_isWithFriend == false)
        {
            // 通常の対局の時
            _gameStage.HideOpponentSearcherForBackAndChangeScene(nextSceneName: "SampleMenuStageScene", nextStageType: EStageType.MENU);
        }
        else
        {
            // 友達と対局の時
            _gameStage.HideOpponentSearcherForBackAndChangeScene(nextSceneName: "SampleFriendGameMenuStageScene", nextStageType: EStageType.FRIEND_GAME_MENU);
        }
    }


    // 対戦相手捜索画面の非表示化が完了(対戦開始時)
    public void OnDidHideOpponentSearcherForGame()
    {
        Debug.Log(this.GetType().ToString() + ": OnDidHideOpponentSearcherForGame()");

        ShowBoardSet();
        _isBoardShown = true;
    }



    // ===============================================
    // MARK: - ● Board Set ─────

    private void ShowBoardSet()
    {
        switch (Locator.Instance.debugManager.BoardType)
        {
            case EBoardType.BOARD_66:
                ShowBoardSet66();
                break;

            case EBoardType.BOARD_99:
                ShowBoardSet99();
                break;

            default:
                throw new InvalidOperationException();
        }
    }


    private void ShowBoardSet66()
    {
        var player1Data = new FPlayerData(
                playerName: "Taro",
                rate: 1000,
                gradeRankType: EGradeRankType.RANK_4,
                color: Color.Black);

        var player1PieceList = new List<FPieceAndSquare>
        {
            new(pieceType: PieceType.King,       square: Square.SQ_16),
            new(pieceType: PieceType.Rook,       square: Square.SQ_22),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
            new(pieceType: PieceType.Gold,       square: Square.SQ_46),
            new(pieceType: PieceType.Silver,     square: Square.SQ_56),
            new(pieceType: PieceType.Knight,     square: Square.SQ_66),
            new(pieceType: PieceType.Lance,      square: Square.SQ_15),
            new(pieceType: PieceType.Pawn,       square: Square.SQ_45),
            new(pieceType: PieceType.Dragon,     square: Square.SQ_25),
            new(pieceType: PieceType.Horse,      square: Square.SQ_35),
            new(pieceType: PieceType.Pro_Silver, square: Square.SQ_55),
            new(pieceType: PieceType.Pro_Knight, square: Square.SQ_65),
            new(pieceType: PieceType.Pro_Lance,  square: Square.SQ_14),
            new(pieceType: PieceType.Pro_Pawn,   square: Square.SQ_44),
            // 駒台にあるものは SQ_Invalid を指定
            new(pieceType: PieceType.Rook,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Gold,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
            //new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),

            // テストで2枚ずつ設置
            new(pieceType: PieceType.Rook,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Gold,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
            //new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),

            // テストで3枚目を設置
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
        };

        var player2Data = new FPlayerData(
            playerName: "Hanako",
            rate: 2000,
            gradeRankType: EGradeRankType.GRADE_1,
            color: Color.White);

        var player2PieceList = new List<FPieceAndSquare>
        {
            new(pieceType: PieceType.King,       square: Square.SQ_61),
            new(pieceType: PieceType.Rook,       square: Square.SQ_51),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
            // 駒台にあるものは SQ_Invalid を指定
            new(pieceType: PieceType.Rook,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Gold,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
            new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid)
        };

      
        _gameStage.ShowBoardSet(
            player1Data: player1Data, player1PieceList: player1PieceList,
            player2Data: player2Data, player2PieceList: player2PieceList,
            EBoardType.BOARD_66
            );

        _gameStage.SetTimeLeft(minute: 10, second: 0);
    }


    private void ShowBoardSet99()
    {
        var player1Data = new FPlayerData(
            playerName: "Taro",
            rate: 1000,
            gradeRankType: EGradeRankType.RANK_3,
            color: Color.Black);

        var player1PieceList = new List<FPieceAndSquare>
        {
            new(pieceType: PieceType.King,       square: Square.SQ_16),
            new(pieceType: PieceType.Rook,       square: Square.SQ_22),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
            new(pieceType: PieceType.Gold,       square: Square.SQ_46),
            new(pieceType: PieceType.Silver,     square: Square.SQ_56),
            new(pieceType: PieceType.Knight,     square: Square.SQ_66),
            new(pieceType: PieceType.Lance,      square: Square.SQ_15),
            new(pieceType: PieceType.Pawn,       square: Square.SQ_45),
            new(pieceType: PieceType.Dragon,     square: Square.SQ_25),
            new(pieceType: PieceType.Horse,      square: Square.SQ_35),
            new(pieceType: PieceType.Pro_Silver, square: Square.SQ_55),
            new(pieceType: PieceType.Pro_Knight, square: Square.SQ_65),
            new(pieceType: PieceType.Pro_Lance,  square: Square.SQ_14),
            new(pieceType: PieceType.Pro_Pawn,   square: Square.SQ_44),
            // 駒台にあるものは SQ_Invalid を指定
            new(pieceType: PieceType.Rook,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Gold,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
            new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),

            // テストで2枚ずつ設置
            new(pieceType: PieceType.Rook,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Gold,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
            new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),


            // テストで3枚目を設置
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
        };

        var player2Data = new FPlayerData(
            playerName: "Hanako",
            rate: 2000,
            gradeRankType: EGradeRankType.GRADE_1,
            color: Color.White);

        var player2PieceList = new List<FPieceAndSquare>
        {
            new(pieceType: PieceType.King,       square: Square.SQ_61),
            new(pieceType: PieceType.Rook,       square: Square.SQ_51),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
            // 駒台にあるものは SQ_Invalid を指定
            new(pieceType: PieceType.Rook,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Bishop,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Gold,       square: Square.SQ_Invalid),
            new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
            new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
            new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid)
        };

      
        _gameStage.ShowBoardSet(
            player1Data: player1Data, player1PieceList: player1PieceList,
            player2Data: player2Data, player2PieceList: player2PieceList,
            EBoardType.BOARD_99
            );

        _gameStage.SetTimeLeft(minute: 10, second: 0);
    }



    // ===============================================
    // MARK: - ● Board Set ─────

    // 盤・駒設置アニメーション、対局情報などの表示完了
    public void OnDidShowBoardSet()
    {
        _gameStage.StartGame();
    }



    // ===============================================
    // MARK: - ● Game ─────

    // 盤上の駒がないセルをタップ
    public void OnTap(Square sq)
    {
        Debug.Log(this.GetType().ToString() + ": OnTap(Square): " + sq.ToString());
    }


    // 盤上の駒をタップ
    public void OnTap(Piece piece, Square sq)
    {
        Debug.Log(this.GetType().ToString() + ": OnTap(Piece, Square): " + piece.ToString() + ", " + sq.ToString());
    }

    // 自軍の駒台の駒をタップ
    public void OnTapOnWaitingSpace(Piece piece)
    {
        Debug.Log(this.GetType().ToString() + ": OnTapOnWaitingSpace(Piece): " + piece.ToString());
    }


    // 盤・自軍駒台外をタップ
    public void OnTapOutOfBoard()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapOutOfBoard()");
    }


    // 投了ボタンをタップ
    public void OnTapButtonResign()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonResign()");
    }


    // AI対戦で「待った」ボタンをタップ
    public void OnTapButtonUndoInAiGame()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonUndoInAiGame()");
    }


    // AI対戦で「戻る」ボタンをタップ
    public void OnTapButtonBackInAiGame()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonBackInAiGame()");
    }


    // 成る／成らないの選択完了
    public void OnDidSelectPromotion(Piece piece, Square sq, EPromotionResultType promotionResultType)
    {
        if (promotionResultType == EPromotionResultType.PROMOTED)
        {
            // 成った
            Debug.Log(this.GetType().ToString() + ": " + piece.ToString() + "(" + sq.ToString() + ") selected to be promoted");
        }
        else
        {
            // case .NOT_PROMOTED
            // 成らなかった
            Debug.Log(this.GetType().ToString() + ": " + piece.ToString() + "(" + sq.ToString() + ") selected not to be promoted");
        }

        _isWaitingPromotionSelect = false;
    }


    // ===============================================
    // MARK: - ● Game Result ─────

    // 対局結果表示で「検討」押下
    public void OnTapButtonHistoryDetail()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonHistoryDetail()");

        _gameStage.HideBoardSetAndChangeScene(nextSceneName: "SampleHistoryDetailStageScene", nextStageType: EStageType.HISTORY_DETAIL);
    }

    // 対局結果表示で「次の対局へ／友達ともう一度対局する」押下
    public void OnTapButtonNextGameOnGameResult()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonNextGameOnGameResult()");

        _gameStage.HideBoardSetAndChangeScene(nextSceneName: "SampleMenuStageScene", nextStageType: EStageType.MENU);
    }


    // 対局結果表示で「HOME」押下
    public void OnTapButtonHomeOnGameResult()
    {
        Debug.Log(this.GetType().ToString() + ": OnTapButtonHomeOnGameResult()");

        _gameStage.HideBoardSetAndChangeScene(nextSceneName: "SampleMenuStageScene", nextStageType: EStageType.MENU);
    }


    // ===============================================
    // MARK: - ● 対局中設定 ─────

    // 対局中設定で「音を消す」を操作
    public void OnTapPlayAudio(bool isOn)
    {
        Debug.Log(this.GetType().ToString() + ": OnTapPlayAudio(): " + isOn.ToString());
        // ユーザ設定を記録してください
    }


    // 対局結果表示で「相手の名前を表示」を操作
    public void OnTapShowNamePlayer2(bool isOn)
    {
        Debug.Log(this.GetType().ToString() + ": OnTapShowNamePlayer2(): " + isOn.ToString());
        // ユーザ設定を記録してください
    }


    // 対局結果表示で「自分の名前を表示」を操作
    public void OnTapShowNamePlayer1(bool isOn)
    {
        Debug.Log(this.GetType().ToString() + ": OnTapShowNamePlayer1(): " + isOn.ToString());
        // ユーザ設定を記録してください
    }


    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        Debug.Log(this.GetType().ToString() + ": OnDestroy");

        _gameStage = null;
    }



    // ===============================================
    // MARK: - ● 駒の自動操作処理 ─────

    public void Update()
    {
        switch (Locator.Instance.debugManager.BoardType)
        {
            case EBoardType.BOARD_66:
                UpdateForBoard66();
                break;

            case EBoardType.BOARD_99:
                UpdateForBoard99();
                break;

            default:
                throw new InvalidOperationException();
        }
    }


    private void UpdateForBoard66()
    {
        // 新規対局の場合の処理（デモとして一定フレーム待つ処理）
        if (_isNewGame)
        {
            if (_presentWaitFrameForOpponentSearcher < _endFrameForOpponentSearcher)
            {
                _presentWaitFrameForOpponentSearcher += 1;
                return;
            }
            else if (_presentWaitFrameForOpponentSearcher == _endFrameForOpponentSearcher)
            {
                // 対局相手が見つかったら、対局相手を探す表示を終了する
                _gameStage.HideOpponentSearcherForGame();
                _presentWaitFrameForOpponentSearcher += 1;

                return;
            }
        }

        // 盤の表示が完了していない場合、スキップ
        if (_isBoardShown == false) { return; }

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime < 0.5f)
        {
            return;
        }


        if (_isWaitingPromotionSelect == false)
        {
            if (_commandIndex == 0)
            {
                if (Locator.Instance.debugManager.IsQuickFinishMatchMode)
                {
                    Debug.Log("対局結果表示");
                    _gameStage.ShowGameResult(
                        gameResultType: EGameResultType.WIN,
                        gameResultReasonType: EGameResultReasonType.CHECKMATE,
                        consecutiveCount: 2, // 勝敗の連続数。引き分けの時には表示されないため、値は任意
                        previousRate: 1300,
                        nextRate: 1301,
                        previousGradeRankType: EGradeRankType.GRADE_1,
                        nextGradeRankType: EGradeRankType.RANK_1);

                    _commandIndex = 999;
                }

                _commandIndex += 1;
                return;
            }

            switch (_commandIndex)
            {
                case 0:
                    _gameStage.ShowTextBalloonNormal(text: "対局開始：先手");

                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");

                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 59);

                    Debug.Log("黒：盤上の飛車を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_22,
                        squareListOfPlaceTarget: new List<Square>() {
                            Square.SQ_21,
                            Square.SQ_23,
                            Square.SQ_24,
                            Square.SQ_62,
                            Square.SQ_52,
                            Square.SQ_42,
                            Square.SQ_32,
                            Square.SQ_12});

                    
                    break;

                case 1:
                    _gameStage.ShowTextBalloonError(text: "通信が不安定です");

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 58);

                    Debug.Log("黒：盤上の飛車を移動→成る／成らないを確認");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_22,
                        squareTo: Square.SQ_24,
                        promotionAfterMoveType: EPromotionAfterMoveType.ASK_PROMOTE);

                    _isWaitingPromotionSelect = true;

                    break;

                case 2:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 59);

                    Debug.Log("白：駒台の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        pieceTypeOnWaitingSpace: PieceType.Pawn,
                        squareTo: Square.SQ_43);
                    break;

                case 3:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 57);

                    Debug.Log("黒：駒台の金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        pieceTypeOnWaitingSpace: PieceType.Gold,
                        squareListOfPlaceTarget: new List<Square>() {
                        Square.SQ_11, Square.SQ_21, Square.SQ_31, Square.SQ_12, Square.SQ_22, Square.SQ_32,
                        Square.SQ_42, Square.SQ_52, Square.SQ_62, Square.SQ_13, Square.SQ_23, Square.SQ_33,
                        Square.SQ_43, Square.SQ_53, Square.SQ_63, Square.SQ_24, Square.SQ_34, Square.SQ_54, Square.SQ_64});
                    break;

                case 4:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 56);

                    Debug.Log("黒：駒の選択をキャンセル");
                    _gameStage.DeselectPiece();

                    break;

                case 5:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 55);

                    Debug.Log("黒：駒台の銀を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        pieceTypeOnWaitingSpace: PieceType.Silver,
                        squareListOfPlaceTarget: new List<Square>() {
                        Square.SQ_11, Square.SQ_21, Square.SQ_31, Square.SQ_12, Square.SQ_22, Square.SQ_32,
                        Square.SQ_42, Square.SQ_52, Square.SQ_62, Square.SQ_13, Square.SQ_23, Square.SQ_33,
                        Square.SQ_43, Square.SQ_53, Square.SQ_63, Square.SQ_24, Square.SQ_34, Square.SQ_54, Square.SQ_64});
                    break;

                case 6:
                    _gameStage.HideTextBalloonError();

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 54);

                    Debug.Log("黒：駒台の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        pieceTypeOnWaitingSpace: PieceType.Silver,
                        squareTo: Square.SQ_32);
                    break;

                case 7:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 58);

                    Debug.Log("白：盤上の飛車を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        squareFrom: Square.SQ_51,
                        squareTo: Square.SQ_55,
                        promotionAfterMoveType: EPromotionAfterMoveType.PROMOTE);
                    break;

                case 8:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 53);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_44,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_53, Square.SQ_43, Square.SQ_33, Square.SQ_54, Square.SQ_34 });
                    break;

                case 9:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 52);

                    Debug.Log("黒：駒の選択をキャンセル");
                    _gameStage.DeselectPiece();
                    break;

                case 10:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 51);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_44,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_53, Square.SQ_43, Square.SQ_33, Square.SQ_54, Square.SQ_34 });
                    break;

                case 11:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 50);

                    Debug.Log("黒：盤上のと金を移動");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_44,
                        squareTo: Square.SQ_43,
                        promotionAfterMoveType: EPromotionAfterMoveType.NOT_PROMOTE);
                    break;

                case 12:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 57);

                    Debug.Log("白：駒台の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        pieceTypeOnWaitingSpace: PieceType.Silver,
                        squareTo: Square.SQ_63);
                    break;

                case 13:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 49);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_43,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_52, Square.SQ_42, Square.SQ_53, Square.SQ_33, Square.SQ_44 });
                    break;

                case 14:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 48);

                    Debug.Log("黒：他の駒を選択したことで、既存の駒の選択をキャンセル");
                    _gameStage.DeselectPiece();

                    Debug.Log("黒：盤上の銀を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_32,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_41, Square.SQ_31, Square.SQ_21, Square.SQ_23 });
                    break;

                case 15:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 47);

                    Debug.Log("黒：盤上の銀を移動→成る／成らないを確認");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_32,
                        squareTo: Square.SQ_41,
                        promotionAfterMoveType: EPromotionAfterMoveType.ASK_PROMOTE);

                    _isWaitingPromotionSelect = true;

                    break;

                case 16:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 56);

                    Debug.Log("白：盤上の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        squareFrom: Square.SQ_63,
                        squareTo: Square.SQ_54,
                        promotionAfterMoveType: EPromotionAfterMoveType.NOT_PROMOTE);
                    break;

                case 17:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 46);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_43,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_52, Square.SQ_42, Square.SQ_32, Square.SQ_53, Square.SQ_33, Square.SQ_44 });
                    break;

                case 18:
                    _gameStage.HideTextBalloonNormal();

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 45);

                    Debug.Log("黒：盤上のと金を移動");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_43,
                        squareTo: Square.SQ_52,
                        promotionAfterMoveType: EPromotionAfterMoveType.NOT_PROMOTE);

                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("対局結果表示");
                    _gameStage.ShowGameResult(
                        gameResultType: EGameResultType.WIN,
                        gameResultReasonType: EGameResultReasonType.CHECKMATE,
                        consecutiveCount: 2, // 勝敗の連続数。引き分けの時には表示されないため、値は任意
                        previousRate: 1300,
                        nextRate: 1800,
                        previousGradeRankType: EGradeRankType.GRADE_1,
                        nextGradeRankType: EGradeRankType.RANK_1);

                    break;


                default:
                    break;
            }

            _elapsedTime = 0;
            _commandIndex++;
        }
    }


    private void UpdateForBoard99()
    {
        // 新規対局の場合の処理（デモとして一定フレーム待つ処理）
        if (_isNewGame)
        {
            if (_presentWaitFrameForOpponentSearcher < _endFrameForOpponentSearcher)
            {
                _presentWaitFrameForOpponentSearcher += 1;
                return;
            }
            else if (_presentWaitFrameForOpponentSearcher == _endFrameForOpponentSearcher)
            {
                // 対局相手が見つかったら、対局相手を探す表示を終了する
                _gameStage.HideOpponentSearcherForGame();
                _presentWaitFrameForOpponentSearcher += 1;

                return;
            }
        }

        // 盤の表示が完了していない場合、スキップ
        if (_isBoardShown == false) { return; }

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime < 0.5f)
        {
            return;
        }


        if (_isWaitingPromotionSelect == false)
        {
            switch (_commandIndex)
            {
                case 0:
                    _gameStage.ShowTextBalloonNormal(text: "対局開始：先手");


                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");

                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 59);

                    Debug.Log("黒：盤上の飛車を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_22,
                        squareListOfPlaceTarget: new List<Square>() {
                            Square.SQ_21,
                            Square.SQ_23,
                            Square.SQ_24,
                            Square.SQ_62,
                            Square.SQ_52,
                            Square.SQ_42,
                            Square.SQ_32,
                            Square.SQ_12});
                    break;

                case 1:
                    _gameStage.ShowTextBalloonError(text: "通信が不安定です");

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 58);

                    Debug.Log("黒：盤上の飛車を移動→成る／成らないを確認");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_22,
                        squareTo: Square.SQ_24,
                        promotionAfterMoveType: EPromotionAfterMoveType.ASK_PROMOTE);

                    _isWaitingPromotionSelect = true;

                    break;

                case 2:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 59);

                    Debug.Log("白：駒台の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        pieceTypeOnWaitingSpace: PieceType.Pawn,
                        squareTo: Square.SQ_11);
                    break;

                case 3:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 57);

                    Debug.Log("黒：駒台の金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        pieceTypeOnWaitingSpace: PieceType.Gold,
                        squareListOfPlaceTarget: new List<Square>() {
                        Square.SQ_11, Square.SQ_21, Square.SQ_31, Square.SQ_12, Square.SQ_22, Square.SQ_32,
                        Square.SQ_42, Square.SQ_52, Square.SQ_62, Square.SQ_13, Square.SQ_23, Square.SQ_33,
                        Square.SQ_43, Square.SQ_53, Square.SQ_63, Square.SQ_24, Square.SQ_34, Square.SQ_54, Square.SQ_64});
                    break;

                case 4:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 56);

                    Debug.Log("黒：駒の選択をキャンセル");
                    _gameStage.DeselectPiece();

                    break;

                case 5:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 55);

                    Debug.Log("黒：駒台の銀を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        pieceTypeOnWaitingSpace: PieceType.Silver,
                        squareListOfPlaceTarget: new List<Square>() {
                        Square.SQ_11, Square.SQ_21, Square.SQ_31, Square.SQ_12, Square.SQ_22, Square.SQ_32,
                        Square.SQ_42, Square.SQ_52, Square.SQ_62, Square.SQ_13, Square.SQ_23, Square.SQ_33,
                        Square.SQ_43, Square.SQ_53, Square.SQ_63, Square.SQ_24, Square.SQ_34, Square.SQ_54, Square.SQ_64});
                    break;

                case 6:
                    _gameStage.HideTextBalloonError();

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 54);

                    Debug.Log("黒：駒台の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        pieceTypeOnWaitingSpace: PieceType.Silver,
                        squareTo: Square.SQ_32);
                    break;

                case 7:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 58);

                    Debug.Log("白：盤上の飛車を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        squareFrom: Square.SQ_51,
                        squareTo: Square.SQ_55,
                        promotionAfterMoveType: EPromotionAfterMoveType.PROMOTE);
                    break;

                case 8:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 53);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_44,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_53, Square.SQ_43, Square.SQ_33, Square.SQ_54, Square.SQ_34 });
                    break;

                case 9:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 52);

                    Debug.Log("黒：駒の選択をキャンセル");
                    _gameStage.DeselectPiece();
                    break;

                case 10:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 51);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_44,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_53, Square.SQ_43, Square.SQ_33, Square.SQ_54, Square.SQ_34 });
                    break;

                case 11:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 50);

                    Debug.Log("黒：盤上のと金を移動");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_44,
                        squareTo: Square.SQ_43,
                        promotionAfterMoveType: EPromotionAfterMoveType.NOT_PROMOTE);
                    break;

                case 12:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 57);

                    Debug.Log("白：駒台の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        pieceTypeOnWaitingSpace: PieceType.Silver,
                        squareTo: Square.SQ_63);
                    break;

                case 13:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 49);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_43,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_52, Square.SQ_42, Square.SQ_53, Square.SQ_33, Square.SQ_44 });
                    break;

                case 14:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 48);

                    Debug.Log("黒：他の駒を選択したことで、既存の駒の選択をキャンセル");
                    _gameStage.DeselectPiece();

                    Debug.Log("黒：盤上の銀を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_32,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_41, Square.SQ_31, Square.SQ_21, Square.SQ_23 });
                    break;

                case 15:
                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 47);

                    Debug.Log("黒：盤上の銀を移動→成る／成らないを確認");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_32,
                        squareTo: Square.SQ_41,
                        promotionAfterMoveType: EPromotionAfterMoveType.ASK_PROMOTE);

                    _isWaitingPromotionSelect = true;

                    break;

                case 16:
                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("白：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.White, minute: 9, second: 56);

                    Debug.Log("白：盤上の銀を移動");
                    _gameStage.MovePiece(
                        color: Color.White,
                        squareFrom: Square.SQ_63,
                        squareTo: Square.SQ_54,
                        promotionAfterMoveType: EPromotionAfterMoveType.NOT_PROMOTE);
                    break;

                case 17:
                    _gameStage.ChangePresentPlayer(color: Color.Black);

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 46);

                    Debug.Log("黒：盤上のと金を選択");
                    _gameStage.SelectPiece(
                        color: Color.Black,
                        squareOfPiece: Square.SQ_43,
                        squareListOfPlaceTarget: new List<Square>() { Square.SQ_52, Square.SQ_42, Square.SQ_32, Square.SQ_53, Square.SQ_33, Square.SQ_44 });
                    break;

                case 18:
                    _gameStage.HideTextBalloonNormal();

                    Debug.Log("黒：残り時間更新");
                    _gameStage.SetTimeLeft(color: Color.Black, minute: 9, second: 45);

                    Debug.Log("黒：盤上のと金を移動");
                    _gameStage.MovePiece(
                        color: Color.Black,
                        squareFrom: Square.SQ_43,
                        squareTo: Square.SQ_52,
                        promotionAfterMoveType: EPromotionAfterMoveType.NOT_PROMOTE);

                    _gameStage.ChangePresentPlayer(color: Color.White);

                    Debug.Log("対局結果表示");
                    _gameStage.ShowGameResult(
                        gameResultType: EGameResultType.WIN,
                        gameResultReasonType: EGameResultReasonType.CHECKMATE,
                        consecutiveCount: 2, // 勝敗の連続数。引き分けの時には表示されないため、値は任意
                        previousRate: 1300,
                        nextRate: 1300,
                        previousGradeRankType: EGradeRankType.RANK_3,
                        nextGradeRankType: EGradeRankType.RANK_3);

                    break;


                default:
                    break;
            }

            _elapsedTime = 0;
            _commandIndex++;
        } 
    }
}
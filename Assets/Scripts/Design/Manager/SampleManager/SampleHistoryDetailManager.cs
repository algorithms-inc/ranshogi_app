using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// こちらの using を挿入する必要あり
using GFramework.Core;
using GFramework;

// ===============================================
// MARK: - ● SampleHistoryDetailManager ─────

// HistoryDetailManager では、インターフェースとして IHistoryDetailManager を設定する必要あり
public class SampleHistoryDetailManager : MonoBehaviour, IHistoryDetailManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // HistoryDetailStage への参照
    private IHistoryDetailStage _historyDetailStage = null;

    // サンプルを動かすための変数
    private int _presentMoveNum = 0;
    private int _maxMoveNum = 6;
    private bool _isActualPlay = true;


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
        _historyDetailStage = AppManager.Instance.CreateHistoryDetailStage(historyDetailManager: this, boardType: EBoardType.BOARD_66);

        // 現在の手数・最大手数を設定
        _historyDetailStage.SetPresentMoveNum(value: _presentMoveNum);
        _historyDetailStage.SetMaxMoveNum(value: _maxMoveNum);


        // ページ操作のボタンの初期状態は Disabled であるため、必要なボタンを Enabled 状態にする
        UpdateButton();


        var historyDetailSetting = new FHistoryItemInitializer(
            historyDetailID: 1,
            gameNum: 5,
            boardType: EBoardType.BOARD_66,
            historyItemResult: EHistoryItemResult.WIN,
            gameResultReasonType: EGameResultReasonType.CHECKMATE,
            date: "2022/12/31(土) 12:59",
            dateAdditionalInfo: "今日",
            color: Color.Black,
            playerName: "Yamada Taro",
            opponentName: "Hanako",
            gradeRankType: EGradeRankType.RANK_1,
            opponentGradeRankType: EGradeRankType.RANK_2,
            previousRate: 123,
            nextRate: 124,
            previousGradeRankType: EGradeRankType.RANK_1,
            nextGradeRankType: EGradeRankType.GRADE_1,
            canExportData: true,
            isEnabled: true);

        _historyDetailStage.SetupGameInfo(historyDetailSetting: historyDetailSetting);


        var pieceList = GenerateBoardForActualPlay(moveNum: 0);

        var player1Data = new FPlayerData(
            playerName: "Taro",
            rate: 1000,
            gradeRankType: EGradeRankType.RANK_1,
            color: Color.Black);

        var player2Data = new FPlayerData(
            playerName: "Hanako",
            rate: 2000,
            gradeRankType: EGradeRankType.RANK_2,
            color: Color.White);


        _historyDetailStage.SetupBoardSet(
             moveNum: 1,
             boardType: EBoardType.BOARD_66,
             player1Data: player1Data,
             player1PieceList: pieceList.player1PieceList,
             player2Data: player2Data,
             player2PieceList: pieceList.player2PieceList);

        _historyDetailStage.ShowWithAnimation();
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

        _historyDetailStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleHistoryStageScene", nextStageType: EStageType.HISTORY);
    }


    public void OnTapButtonStepForward()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonStepForward");

        _presentMoveNum++;
        UpdateBoardSet();
    }


    public void OnTapButtonStepBack()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonStepBack");
        
        _presentMoveNum--;
        UpdateBoardSet();
    }


    public void OnTapButtonJumpToFirst()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonJumpToFirst");

        _presentMoveNum = 0;
        UpdateBoardSet();
    }


    public void OnTapButtonJumpToLast()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonJumpToLast");

        _presentMoveNum = _maxMoveNum;
        UpdateBoardSet();
    }


    public void OnTapButtonAiEvaluation()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonAiEvaluation");

        if (_historyDetailStage.IsAiEvaluationBalloonShown)
        {
            _historyDetailStage.HideAiEvaluationBalloon();
            return;
        }
        _historyDetailStage.ShowAiEvaluationBalloon(text: "評価値 +350（先手有利）\n▲３三銀成 △同玉 ▲２四金打", canReplay: true);
    }


    public void OnTapButtonReplayAiVariation()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonReplayAiVariation");

        _isActualPlay = false;
        _presentMoveNum = 0;
        _historyDetailStage.HideAiEvaluationBalloon();
        UpdateBoardSet();
    }


    public void OnTapButtonBackToActualPlay()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonBackToActualPlay");

        _isActualPlay = true;
        _presentMoveNum = 0;
        _historyDetailStage.SetButtonBackToActualPlayEnabled(value: false);
        UpdateBoardSet();
    }


    public void OnTap(Square sq)
    {
        Debug.Log(GetType().ToString() + ": OnTap " + sq.ToString());
    }


    public void OnTap(Piece piece, Square sq)
    {
        Debug.Log(GetType().ToString() + ": OnTap " + piece.ToString() + " " + sq.ToString());
    }


    public void OnTapOnWaitingSpace(Piece piece)
    {
        Debug.Log(GetType().ToString() + ": OnTapOnWaitingSpace " + piece.ToString());
    }


    public void OnTapOutOfBoard()
    {
        Debug.Log(GetType().ToString() + ": OnTapOutOfBoard");
    }


    public void OnTapExportData(EKifuDataType kifuDataType)
    {
        Debug.Log(GetType().ToString() + ": OnTapExportData" + kifuDataType.ToString());
    }


    public void OnTapButtonAiMatch()
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonAiMatch");
        _historyDetailStage.ShowWindowAiMatch();
    }


    public void OnTapButtonStartAiMatch(Color playerColor, RanshogiEngine.Level level)
    {
        Debug.Log(GetType().ToString() + ": OnTapButtonStartAiMatch " + playerColor.ToString() + " " + level.ToString());
    }



    // ===============================================
    // MARK: - ● Update Board Set ─────

    private void UpdateBoardSet()
    {
        (List<FPieceAndSquare> player1PieceList, List<FPieceAndSquare> player2PieceList) pieceList;

        if (_isActualPlay)
        {
            pieceList = GenerateBoardForActualPlay(moveNum: _presentMoveNum);
        }
        else
        {
            pieceList = GenerateBoardForAIVariation(moveNum: _presentMoveNum);
        }

        _historyDetailStage.ChangeMoveNumAndPiece(
            moveNum: _presentMoveNum,
            player1PieceList: pieceList.player1PieceList,
            player2PieceList: pieceList.player2PieceList);

        UpdateButton();
    }



    // ===============================================
    // MARK: - ● ボタンの表示状態更新 ─────

    private void UpdateButton()
    {
        if (_presentMoveNum == 0)
        {
            _historyDetailStage.SetButtonJumpToFirstDisabled();
        }
        else
        {
            _historyDetailStage.SetButtonJumpToFirstEnabled();
        }

        if (_presentMoveNum == _maxMoveNum)
        {
            _historyDetailStage.SetButtonJumpToLastDisabled();
        }
        else
        {
            _historyDetailStage.SetButtonJumpToLastEnabled();
        }

        if (_presentMoveNum > 0)
        {
            _historyDetailStage.SetButtonStepBackEnabled();
        }
        else
        {
            _historyDetailStage.SetButtonStepBackDisabled();
        }

        if (_presentMoveNum < _maxMoveNum)
        {
            _historyDetailStage.SetButtonStepForwardEnabled();
        }
        else
        {
            _historyDetailStage.SetButtonStepForwardDisabled();
        }
    }


    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        Debug.Log(GetType().ToString() + ": OnDestroy");

        _historyDetailStage = null;
    }


    private (List<FPieceAndSquare> player1PieceList, List<FPieceAndSquare> player2PieceList) GenerateBoardForActualPlay(int moveNum)
    {
        List<FPieceAndSquare> player1PieceList = new();
        List<FPieceAndSquare> player2PieceList = new();

        switch (moveNum) {
            case 0:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_16),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_66),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 1:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_66),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 2:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_66),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_22),
                        // 駒台
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 3:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Pro_Knight,     square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_22),
                        // 駒台
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 4:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Pro_Knight,     square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_22),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_33),
                        // 駒台
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 5:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Dragon,     square: Square.SQ_22),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Pro_Knight,     square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_33),
                        // 駒台
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 6:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Dragon,     square: Square.SQ_22),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Dragon,     square: Square.SQ_56),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_33),
                        // 駒台
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;
            
            default:
                throw new NotImplementedException();
        }

        return (player1PieceList: player1PieceList, player2PieceList: player2PieceList);               
    }


    private (List<FPieceAndSquare> player1PieceList, List<FPieceAndSquare> player2PieceList) GenerateBoardForAIVariation(int moveNum)
    {
        List<FPieceAndSquare> player1PieceList = new();
        List<FPieceAndSquare> player2PieceList = new();

        switch (moveNum) {
            case 0:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_16),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_66),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 1:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_66),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 2:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_66),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_22),
                        // 駒台
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 3:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Pro_Knight,     square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_22),
                        // 駒台
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 4:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_26),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Pro_Knight,     square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_22),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_33),
                        // 駒台
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 5:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Dragon,     square: Square.SQ_22),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Silver, square: Square.SQ_56),
                        new(pieceType: PieceType.Pro_Knight,     square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Rook,       square: Square.SQ_51),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_33),
                        // 駒台
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;

            case 6:
                player1PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_15),
                        new(pieceType: PieceType.Dragon,     square: Square.SQ_22),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_36),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_46),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_65),
                        // 駒台
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Pawn,       square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                player2PieceList = new List<FPieceAndSquare>
                    {
                        new(pieceType: PieceType.King,       square: Square.SQ_61),
                        new(pieceType: PieceType.Dragon,     square: Square.SQ_56),
                        new(pieceType: PieceType.Bishop,     square: Square.SQ_41),
                        new(pieceType: PieceType.Gold,       square: Square.SQ_31),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_21),
                        new(pieceType: PieceType.Pro_Knight, square: Square.SQ_11),
                        new(pieceType: PieceType.Lance,      square: Square.SQ_33),
                        // 駒台
                        new(pieceType: PieceType.Knight,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid),
                        new(pieceType: PieceType.Silver,     square: Square.SQ_Invalid)
                    };
                break;
            
            default:
                throw new NotImplementedException();
        }

        return (player1PieceList: player1PieceList, player2PieceList: player2PieceList);               
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework.Core
{
    /// <summary>
    /// 駒に移動を命じた際に、成る／成らないをどうするか
    /// </summary>
    public enum EPromotionAfterMoveType
    {
        NOT_PROMOTE,
        PROMOTE,
        ASK_PROMOTE
    }


    /// <summary>
    /// プレイヤーが成る／成らないを選択した結果
    /// </summary>
    public enum EPromotionResultType
    {
        PROMOTED,
        NOT_PROMOTED
    }


    /// <summary>
    /// ゲーム勝敗結果
    /// </summary>
    public enum EGameResultType
    {
        WIN,
        LOSE,
        DRAW
    }


    /// <summary>
    /// ゲーム勝敗の理由
    /// </summary>
    public enum EGameResultReasonType
    {
        // 勝ち負け共通 -----

        // 詰み
        CHECKMATE,
        // トライ
        TRY,
        // 投了
        RESIGNATION,
        // 時間切れ
        TIMEUP,


        // 引き分け -----

        // 手数制限到達
        LIMIT_OF_MOVES,

        // 千日手
        REPETITION_OF_MOVES,


        // その他 ----- (基本的には使用することはない)

        // 未分類
        UNKOWN
    }

    public static class EGameResultReasonTypeExtensions
    {
        public static string ToString(this EGameResultReasonType gameResultReasonType, EGameResultType gameResultType)
        {
            switch (gameResultType)
            {
                case EGameResultType.WIN:
                    return gameResultReasonType switch
                    {
                        EGameResultReasonType.CHECKMATE => "詰み",
                        EGameResultReasonType.TRY => "トライ",
                        EGameResultReasonType.RESIGNATION => "相手の投了",
                        EGameResultReasonType.TIMEUP => "相手の時間切れ",
                        EGameResultReasonType.LIMIT_OF_MOVES => "手数の制限",
                        EGameResultReasonType.REPETITION_OF_MOVES => "千日手",
                        _ => throw new InvalidOperationException(),
                    };

                case EGameResultType.LOSE:
                    return gameResultReasonType switch
                    {
                        EGameResultReasonType.CHECKMATE => "詰み",
                        EGameResultReasonType.TRY => "相手のトライ",
                        EGameResultReasonType.RESIGNATION => "投了",
                        EGameResultReasonType.TIMEUP => "時間切れ",
                        EGameResultReasonType.LIMIT_OF_MOVES => "手数の制限",
                        EGameResultReasonType.REPETITION_OF_MOVES => "千日手",
                        _ => throw new InvalidOperationException(),
                    };

                case EGameResultType.DRAW:
                    return gameResultReasonType switch
                    {
                        EGameResultReasonType.CHECKMATE => "詰み",
                        EGameResultReasonType.TRY => "相手のトライ",
                        EGameResultReasonType.RESIGNATION => "投了",
                        EGameResultReasonType.TIMEUP => "時間切れ",
                        EGameResultReasonType.LIMIT_OF_MOVES => "手数の制限",
                        EGameResultReasonType.REPETITION_OF_MOVES => "千日手",
                        _ => throw new InvalidOperationException(),
                    };

                default:
                    throw new InvalidOperationException();
            }
        }
    }


    public interface IGameStage
    {
        // ===============================================
        // MARK: - ● Opponent Searcher ─────

        // 待機画面を表示
        void ShowOpponentSearcher();

        // 待機画面を非表示 (戻る操作のために)にしてシーンを移動
        void HideOpponentSearcherForBackAndChangeScene(string nextSceneName, EStageType nextStageType);

        // 待機画面を非表示 (対局に進むため)
        void HideOpponentSearcherForGame();


        // ===============================================
        // MARK: - ● Board Set ─────

        // 盤セットを表示
        void ShowBoardSet(
            in FPlayerData player1Data, in List<FPieceAndSquare> player1PieceList,
            in FPlayerData player2Data, in List<FPieceAndSquare> player2PieceList,
            EBoardType boardType
            );
        
        // 対局開始
        void StartGame();

        // 盤セットを非表示にする(対局終了後、別画面に遷移する際に使用)
        void HideBoardSetAndChangeScene(string nextSceneName, EStageType nextStageType);



        // ===============================================
        // MARK: - ● UI ─────

        // 残り時刻の表示(プレイヤー両者)
        void SetTimeLeft(int minute, int second);

        // 残り時刻の表示(プレイヤー指定)
        void SetTimeLeft(Color color, int minute, int second);

        // 残り時刻の代わりに「--:--」を表示(プレイヤー両者。時間制限のないAI対戦)
        void SetTimeLeftUnknown();

        // AI対戦で、投了ボタンの横に「待った」「戻る」ボタンを出すか
        void SetAiGameButtonsShown(bool value);

        // 打ち手のプレイヤーを切り替える
        void ChangePresentPlayer(Color color);

        // 対局結果表示の「次の対局へ」ボタンの文字を変える(AI対戦では「戻る」にする)。対局結果の表示前に呼ぶこと。
        void SetNextGameButtonText(string text);

        void SetHomeButtonText(string text);



        // ===============================================
        // MARK: - ● Window ─────

        // 対局結果を表示
        void ShowGameResult(
            EGameResultType gameResultType,
            EGameResultReasonType gameResultReasonType,
            int consecutiveCount,
            int previousRate,
            int nextRate,
            EGradeRankType previousGradeRankType,
            EGradeRankType nextGradeRankType);


        // ===============================================
        // MARK: - ● Check (王手) ─────

        // 王手を受けていることを表示
        void ShowCheck(Color color);


        // ===============================================
        // MARK: - ● Action ─────

        // 駒の選択
        void SelectPiece(Color color, Square squareOfPiece, in List<Square> squareListOfPlaceTarget);

        // 駒の選択
        void SelectPiece(Color color, PieceType pieceTypeOnWaitingSpace, in List<Square> squareListOfPlaceTarget);

        // 選択状態の駒を非選択状態し、選択状態のセルを元にもどす
        void DeselectPiece();

        // 盤上の駒の移動
        void MovePiece(Color color, Square squareFrom, Square squareTo, EPromotionAfterMoveType promotionAfterMoveType);

        // 駒台の駒の移動
        void MovePiece(Color color, PieceType pieceTypeOnWaitingSpace, Square squareTo);


        // ===============================================
        // MARK: - ● Check Status ─────

        // 駒選択ウィンドウが表示されているか
        bool IsPromotionSelectorShown();

        // 投了確認ウィンドウが表示されているか
        bool IsResignSelectorShown();  


        // ===============================================
        // MARK: - ● Text Balloon ─────

        // テキストバルーン(通常)を表示
        void ShowTextBalloonNormal(string text);

        // テキストバルーン(通常)を非表示
        void HideTextBalloonNormal();

        // テキストバルーン(エラー)を表示
        void ShowTextBalloonError(string text);

        // テキストバルーン(エラー)を非表示
        void HideTextBalloonError();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GFramework.Core
{
    public interface IHistoryDetailStage
    {
        // ===============================================
        // MARK: - ● Show / Hide ─────

        // 全体を表示にする(アニメーションあり)
        void ShowWithAnimation();

        // 全体を非表示にしてシーンを切り替える(アニメーションあり)
        void HideWithAnimationAndChangeScene(string nextSceneName, EStageType nextStageType);


        // ===============================================
        // MARK: - ● Info ─────

        // 現在のページ数を設定
        void SetPresentMoveNum(int value);

        // 最大ページ数を設定
        void SetMaxMoveNum(int value);

        // 棋譜データ形式の選択肢として最初に選択されているものを設定
        void SetDefaultSelectedKifuDataType(EKifuDataType kifuDataType);


        // ===============================================
        // MARK: - ● Button ─────

        // 次へボタンを使用可能に
        void SetButtonStepForwardEnabled();
        // 次へボタンを使用不可に
        void SetButtonStepForwardDisabled();

        // 前へボタンを使用可能に
        void SetButtonStepBackEnabled();
        // 前へボタンを使用可能に
        void SetButtonStepBackDisabled();


        // 最後へジャンプボタンを使用可能に
        void SetButtonJumpToFirstEnabled();
        // 最初へジャンプボタンを使用不可に
        void SetButtonJumpToFirstDisabled();

        // 最後へジャンプボタンを使用可能に
        void SetButtonJumpToLastEnabled();
        // 最後へジャンプボタンを使用不可に
        void SetButtonJumpToLastDisabled();


        // ===============================================
        // MARK: - ● Board ─────

        // 対局情報を表示
        void SetupGameInfo(in FHistoryItemInitializer historyDetailSetting);

        // 盤のセットアップ
        void SetupBoardSet(
            int moveNum,
            EBoardType boardType,
            in FPlayerData player1Data,
            in List<FPieceAndSquare> player1PieceList,
            in FPlayerData player2Data,
            in List<FPieceAndSquare> player2PieceList);

        // 駒の配置変更(66)
        void ChangeMoveNumAndPiece(
            int moveNum,
            in List<FPieceAndSquare> player1PieceList,
            in List<FPieceAndSquare> player2PieceList);

        // 手番側の駒を選択し、動かせるマスを表示する(盤上の駒)
        void SelectPiece(Color color, Square squareOfPiece, in List<Square> squareListOfPlaceTarget);

        // 手番側の駒を選択し、打てるマスを表示する(駒台の駒)
        void SelectPiece(Color color, PieceType pieceTypeOnWaitingSpace, in List<Square> squareListOfPlaceTarget);

        // 駒の選択を解除する
        void DeselectPiece();

        // 成る／成らないを選ぶウィンドウを表示。成るなら true を返す
        void ShowWindowPromotion(Action<bool> onSelect);


        // ===============================================
        // MARK: - ● AI Evaluation ─────

        // 「本譜」ボタンの使用可否(駒を動かしたり読み筋を再生したりして本譜から外れているときだけ押せる)
        void SetButtonBackToActualPlayEnabled(bool value);

        // 「AI評価値」ボタンの使用可否(読んでいる間は押せなくする)
        void SetButtonAiEvaluationEnabled(bool value);

        // 相手の名前・持ち駒の位置に、評価値と読み筋の吹き出しを表示。canReplay なら「手順を再生」ボタンも出す
        void ShowAiEvaluationBalloon(string text, bool canReplay);

        // 吹き出しを閉じる
        void HideAiEvaluationBalloon();

        // 吹き出しを表示しているか
        bool IsAiEvaluationBalloonShown { get; }

        // 「AIと対戦」ウィンドウ(強さ・手番を選んで対局開始)を表示
        void ShowWindowAiMatch();
    }
}
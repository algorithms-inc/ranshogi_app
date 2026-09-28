using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework.Core
{
    public interface IHistoryStage
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
        void SetPresentPageNum(int value);

        // 最大ページ数を設定
        void SetMaxPageNum(int value);


        // ===============================================
        // MARK: - ● Button ─────

        // 次のページへボタンを使用可能に
        void SetButtonNextPageEnabled();
        // 次のページへボタンを使用不可に
        void SetButtonNextPageDisabled();

        // 前のページへボタンを使用可能に
        void SetButtonPreviousPageEnabled();
        // 前のページへボタンを使用可能に
        void SetButtonPreviousPageDisabled();


        // 最初のページへボタンを使用可能に
        void SetButtonFirstPageEnabled();
        // 最初のページへボタンを使用不可に
        void SetButtonFirstPageDisabled();

        // 最後のページへボタンを使用可能に
        void SetButtonLastPageEnabled();
        // 最後のページへボタンを使用可能に
        void SetButtonLastPageDisabled();


        // ===============================================
        // MARK: - ● ItemList ─────

        // １画面に一度に表示できる棋譜項目数を取得
        int GetHistoryItemMaxDisplayNum();

        // 棋譜項目を設定する
        void SetHistoryItemList(in List<FHistoryItemInitializer> historyItemInitializerList);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework.Core
{
    // 勝ち切れ乱将棋のカレンダーに表示する、局面がある日
    public readonly struct FKachikireDay
    {
        public readonly DateTime Date;
        public readonly bool IsCleared;

        public FKachikireDay(DateTime date, bool isCleared)
        {
            Date = date.Date;
            IsCleared = isCleared;
        }
    }


    public interface IMenuStage
    {
        // ===============================================
        // MARK: - ● Show / Hide ─────

        // 全体を表示にする(アニメーションあり)
        void ShowWithAnimation();

        // 全体を非表示にしてシーンを切り替える(アニメーションあり)
        void HideWithAnimationAndChangeScene(string nextSceneName, EStageType nextStageType);


        // ===============================================
        // MARK: - ● Window Kachikire ─────

        // 勝ち切れ乱将棋のカレンダーを表示する。表示中なら指定月の内容に差し替える
        // - startDate: 勝ち切れ乱将棋を始めた日。これより前の日・月は選べない
        // - days: その月で局面がある日 (today より先の日は含めない)
        void ShowWindowKachikireCalendar(int year, int month, DateTime today, DateTime startDate, IReadOnlyList<FKachikireDay> days);
    }
}
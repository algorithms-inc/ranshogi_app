using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using GFramework.Core;


namespace GFramework
{
    /// <summary>
    /// ステータスバーを管理するマネジャ
    /// </summary>
    public interface IStatusBarManager
    {
        StatusBarLayouter Layouter { get; }
        void Setup();

        EMoveOrderType GetMoveOrderType(EPlayerType playerType);
        void SetGradeRankTypeForPlayer1(EMoveOrderType moveOrderType, EGradeRankType gradeRankType, EBoardType boardType);
        void SetGradeRankTypeForPlayer2(EMoveOrderType moveOrderType, EGradeRankType gradeRankType, EBoardType boardType);

        void SetupGameObjectForPlayer2(GameObject parentGameObject, string playerName, EMoveOrderType moveOrderType, EGradeRankType gradeRankType, EBoardType boardType);

        void Show(EPlayerType playerType);
        void Hide(EPlayerType playerType);

        String GetPlayerName(EPlayerType playerType);
        void SetPlayerNameForPlayer1(string playerName);
        void SetPlayerNameForPlayer2(string playerName);
        void SetLife(int count);
        void SetRate(int rate);

        void Destroy();

        void ChangeStatusBarModeForPlayer1(StatusBarBehaviour.EStatusBarMode statusBarMode);

        void SetStatusBarUserType(EStatusBarUserType userType);

        void ChangePresentPlayer(EPlayerType playerType);

        void SetTimeLeft(EPlayerType playerType, int minute, int second);

        // 残り時刻の代わりに文字を出す(時間制限のないAI対戦の "--:--")
        void SetTimeLeftText(EPlayerType playerType, string text);

        bool IsNameShown(EPlayerType playerType);
        void ShowName(EPlayerType playerType);
        void HideName(EPlayerType playerType);
        void SetStatusBarForBehaviourPlayer2NullWhenCloseGameStage();
    }
}

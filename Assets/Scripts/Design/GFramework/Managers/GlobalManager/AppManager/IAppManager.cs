using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GFramework.Core
{
    /// <summary>
    /// アプリ全体に関する事項を管理するマネジャ
    /// </summary>
    public interface IAppManager
    {
        EStageType PreviousStageType { get; }
        EStageType PresentStageType { get; }

        bool IsDebugMode { get; }


        public IUICreator UICreator { get; }
        public IAudioController AudioController { get; }
        public IDesignController DesignController { get; }
        
        void DestroyStageAndLoadScene(string nextSceneName);

        IGameStage CreateGameStage(IGameManager gameManager, EBoardType boardType, bool isWithFriendMode, bool isNewGame);


        IMenuStage CreateMenuStage(IMenuManager menuManager);

        IHistoryStage CreateHistoryStage(IHistoryManager historyManager);

        IHistoryDetailStage CreateHistoryDetailStage(IHistoryDetailManager historyDetailManager, EBoardType boardType);

        IBlankPageStage CreateBlankPageStage(IBlankPageManager blankPageManager);

        IGeneralButtonListStage CreateGeneralButtonListStage(IGeneralButtonListManager generalButtonListManager, string title, List<string> buttonTextList);


        

        void Setup();
        void Destroy();




        // ===============================================
        // MARK: - ● Status Bar ─────

        // プレミアムユーザかどうかを設定する
        void SetIsPremiumUser(bool value);

        // ステータスバーを表示する
        void ShowStatusBar();

        // ステータスバーを非表示にする
        void HideStatusBar();

        // 名前を更新
        void SetPlayerName(string playerName);

        // 残りライフを更新
        void SetLife(int count);

        // レートを更新
        void SetRate(int rate);


        bool IsNameOnStatusBarShownForPlayer1 { get; }
        void HideNameOnStatusBarForPlayer1();
        void ShowNameOnStatusBarForPlayer1();

        bool IsNameOnStatusBarShownForPlayer2 { get; }
        void HideNameOnStatusBarForPlayer2();
        void ShowNameOnStatusBarForPlayer2();
    }
}
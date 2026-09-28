using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework.Core
{
    public interface IGeneralButtonListStage
    {
        // ===============================================
        // MARK: - ● Show / Hide ─────

        // 全体を表示にする(アニメーションあり)
        void ShowWithAnimation();

        // 全体を非表示にしてシーンを切り替える(アニメーションあり)
        void HideWithAnimationAndChangeScene(string nextSceneName, EStageType nextStageType);
    }
}
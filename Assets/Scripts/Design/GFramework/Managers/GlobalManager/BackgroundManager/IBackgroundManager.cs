using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;


namespace GFramework
{
    /// 背景を管理するマネジャ
    public interface IBackgroundManager
    {
        void Destroy();

        BackgroundBehaviour.EBackgroundFlowType GetBackgroundFlowType();

        void ChangePlayerColor(EPlayerType playerType);
    }
}
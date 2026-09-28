using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    /// <summary>
    /// デザイン側のデバッグ・マネジャのインターフェース
    /// </summary>
    public interface IDebugManager
    {
        EBoardType BoardType{get; set;}
        bool IsWithFriendMode{get; set;}
        bool IsQuickFinishMatchMode{get; set;}

        void Destroy();
    }
}
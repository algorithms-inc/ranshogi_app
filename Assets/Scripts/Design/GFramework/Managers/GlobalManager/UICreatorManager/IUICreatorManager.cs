using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using GFramework.Core;


namespace GFramework
{
    /// <summary>
    /// ウィンドウ・マネジャのインターフェース
    /// </summary>
    public interface IUICreatorManager: IUICreator
    {
        void Setup();



        // Text Setting
        int FontSize(float fontScale);
        float LineSpacing { get; }

        // Layouter
        WindowLayouter Layouter { get; }

        void Destroy();


        #region Debug

        void ShowLayoutArea();

        #endregion
    }
}
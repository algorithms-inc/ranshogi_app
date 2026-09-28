using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    public enum EUIButtonDesignType
    {
        POSITIVE,
        DEFAULT,
        WARNING,
        WINDOW_CLOSE
    }

    public interface IUIButtonCreator
    {
        void Add(
            string name,
            in FWindowElementArea area,
            Action actionOnTap,
            string text,
            EUIButtonDesignType buttonDesignType = EUIButtonDesignType.DEFAULT,
            bool isPlaySE = true,
            bool isEnabled = true);
        void Remove(string name);
        bool IsEnabled(string name);
        void SetIsEnabled(string name, bool value);
        // ボタンの上に画像を重ねる (アイコンやマーク用)。area はボタンと同じくウィンドウ内の座標
        void SetCompanionImage(string name, in FWindowElementArea area, string filePath);
    }
}
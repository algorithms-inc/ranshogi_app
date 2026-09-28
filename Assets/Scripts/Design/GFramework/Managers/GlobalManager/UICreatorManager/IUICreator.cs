using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using GFramework.Core;


namespace GFramework.Core
{
    /// <summary>
    /// ウィンドウ上のテキストの表示色
    /// - 将来、アプリの全体デザインをユーザが選べるようにすることもできる可能性があるため、赤や青などの具体的な色ではなく、抽象的に指定
    /// </summary>
    public enum EWindowTextColorType
    {
        NORMAL,
        CAUTION,
        WARNING,
        LINK,
        DISABLED
    }


    /// <summary>
    /// 方向を示す
    /// - エリアにテキストなどを表示する際のアンカー箇所の指定などに使用
    /// </summary>
    public enum EAlignment
    {
        TOP_LEFT,
        TOP_CENTER,
        TOP_RIGHT,
        MIDDLE_LEFT,
        MIDDLE_CENTER,
        MIDDLE_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_CENTER,
        BOTTOM_RIGHT
    }


    /// <summary>
    /// UI作成機能
    /// </summary>
    public interface IUICreator
    {
        /// <summary>
        /// ウィンドウ(上部)
        /// </summary>
        UIWindowCreator WindowTop { get; }

        /// <summary>
        /// ウィンドウ(通常)
        /// </summary>
        UIWindowCreator WindowNormal { get; }

        /// <summary>
        /// ウィンドウ(確認用)
        /// - ウィンドウ(通常)と同時に表示することができ、かつその上に表示される
        /// </summary>
        UIWindowCreator WindowConfirmation { get; }

        /// <summary>
        /// ウィンドウ(エラー表示用)
        /// - ウィンドウ(通常)と同時に表示することができ、かつその上に表示される
        /// </summary>
        UIWindowCreator WindowError { get; }


        /// <summary>
        /// ウィンドウ(確認用)
        /// - YES/NO 選択を持つ簡単なウィンドウ(通常)を表示する
        /// </summary>
        void ShowSimpleWindowConfirmation(string message, EWindowTextColorType textColorType, string textButtonNo, string textButtonYes, Action actionOnTapButtonYes);

        /// <summary>
        /// ボタンの高さの共通サイズ
        /// </summary>
        float GetBaseButtonHeight();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;



namespace GFramework.Core
{
    public interface IUIWindowCreator
    {
        #region Window Control

        /// <summary>
        /// ウィンドウが表示されているか
        /// </summary>
        bool IsShown();


        /// <summary>
        /// ウィンドウを表示する
        /// </summary>
        /// <param name="onDidShowAction">ウィンドウ表示完了時のコールバック</param>
        void Show(Action onDidShowAction = null);


        /// <summary>
        /// ウィンドウを非表示にする
        /// - ウィンドウ上のUIは全て削除される
        /// </summary>
        /// <param name="onDidHideAction">ウィンドウ非表示完了時のコールバック</param>
        void Hide(Action onDidHideAction = null);


        /// <summary>
        /// ウィンドウ上のUIのアクションが実行可能かどうか
        /// </summary>
        bool IsActionEnabled();


        /// <summary>
        /// ウィンドウ上のUIのアクションの実行可否を設定
        /// - ウィンドウ上のUI全てを一時的に止める場合などに使用
        /// </summary>
        void SetIsActionEnabled(bool value);


        /// <summary>
        /// ウィンドウ上のUIの全てを削除する
        /// </summary>
        void RemoveAllUIParts();

        #endregion


        #region Window UI Element Control

        /// <summary>
        /// テキストフィールドの操作
        /// </summary>
        IUITextFieldCreator TextField { get; }


        /// <summary>
        /// ボタンの操作
        /// </summary>
        IUIButtonCreator Button { get; }


        /// <summary>
        /// イメージボックスの操作
        /// </summary>
        IUIImageBoxCreator ImageBox { get; }


        /// <summary>
        /// インプットフィールドの操作
        /// </summary>
        IUIInputFieldCreator InputField { get; }


        /// <summary>
        /// チェックボックスの操作
        /// </summary>
        IUICheckBoxCreator CheckBox { get; }


        /// <summary>
        /// ラジオボタングループの操作
        /// </summary>
        IUIRadioButtonGroupCreator RadioButtonGroup { get; }


        /// <summary>
        /// スライダーの操作
        /// </summary>
        IUISliderCreator Slider { get; }

        #endregion
    }
}
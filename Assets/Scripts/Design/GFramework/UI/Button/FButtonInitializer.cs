using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    /// ボタンの画像タイプ
    public enum EButtonImageType
    {
        BUTTON_RESIZABLE_DEFAULT,
        BUTTON_RESIZABLE_POSITIVE,
        BUTTON_RESIZABLE_WARNING,
        BUTTON_UNRESIZABLE_PROMOTION_SELECTOR,
        BUTTON_UNRESIZABLE_RESIGN,
        BUTTON_UNRESIZABLE_RESIGN_SELECTOR_CANSEL,
        BUTTON_UNRESIZABLE_RESIGN_SELECTOR_OK,
        BUTTON_UNRESIZABLE_WINDOW_CLOSE,
        INPUTFIELD_DEFAULT,
    }


    /// ボタンの表示状態
    public enum EButtonStatusType
    {
        DEFAULT,
        PRESSED,
        HOVER,
        DISABLED
    }


    /// タップの判定タイプ
    public enum ETapJudgeType
    {
        DOWN,
        CLICK
    }


    /// ボタンの初期化構造体
    public struct FButtonInitializer
    {
        // 入力判定を設置する Canvas
        private readonly Canvas _targetCanvas;
        public Canvas TargetCanvas => _targetCanvas;

        private readonly EButtonImageType _buttonImageType;
        public EButtonImageType ButtonImageType => _buttonImageType;

        private readonly FArea _area;
        public FArea Area => _area;

        private readonly ETapJudgeType _tapJudgeType;
        public ETapJudgeType TapJudgeType => _tapJudgeType;

        private readonly bool _isPlaySE;
        public bool IsPlaySE => _isPlaySE;

        private readonly bool _isEnabled;
        public bool IsEnabled => _isEnabled;

        // 画像を設置する順(判定エリアは Canvas に置かれる)
        private readonly int _sortingOrder;
        public int SortingOrder => _sortingOrder;


        /// 押下時の動作
        private Action _action;
        public Action Action => _action;


        public FButtonInitializer(
            Canvas targetCanvas,
            EButtonImageType buttonImageType,
            FArea area,
            ETapJudgeType tapJudgeType,
            bool isPlaySE,
            bool isEnabled,
            int sortingOrder,
            Action action)
        {
            _targetCanvas = targetCanvas;
            _buttonImageType = buttonImageType;
            _area = area;
            _isPlaySE = isPlaySE;
            _isEnabled = isEnabled;
            _tapJudgeType = tapJudgeType;
            _sortingOrder = sortingOrder;
            _action = action;
        }
    }
}
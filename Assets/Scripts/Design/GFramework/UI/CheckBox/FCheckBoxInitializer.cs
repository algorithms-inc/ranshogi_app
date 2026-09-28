using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    // チェックボックスとして扱うか、ラジオボタンとして扱うか
    // - 画像に違いがある
    // - ラジオボタンである場合、ON状態のものを押してもOFFにはならない
    public enum ECheckBoxFunctionType
    {
        CHECK_BOX,
        RADIO_BUTTON
    }



    /// チェックボックスの初期化構造体
    public struct FCheckBoxInitializer
    {
        // 入力判定を設置する Canvas
        private readonly Canvas _targetCanvas;
        public Canvas TargetCanvas => _targetCanvas;

        private readonly FArea _area;
        public FArea Area => _area;

        private readonly bool _isOn;
        public bool IsOn => _isOn;

        private readonly bool _hasError;
        public bool HasError => _hasError;

        private readonly bool _isPlaySE;
        public bool IsPlaySE => _isPlaySE;

        private readonly bool _isEnabled;
        public bool IsEnabled => _isEnabled;


        private readonly ECheckBoxFunctionType _functionType;
        public ECheckBoxFunctionType FunctionType => _functionType;

        // 画像を設置する順(判定エリアは Canvas に置かれる)
        private readonly int _sortingOrder;
        public int SortingOrder => _sortingOrder;

        // 押下時の動作
        private Action _action;
        public Action Action => _action;


        public FCheckBoxInitializer(
            Canvas targetCanvas,
            FArea area,
            bool isOn,
            bool hasError,
            bool isEnabled,
            ECheckBoxFunctionType functionType,
            int sortingOrder,
            Action action,
            bool isPlaySE = true)
        {
            _targetCanvas = targetCanvas;
            _area = area;
            _isOn = isOn;
            _hasError = hasError;
            _isPlaySE = isPlaySE;
            _isEnabled = isEnabled;
            _functionType = functionType;
            _sortingOrder = sortingOrder;
            _action = action;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    /// 入力フォームのタイプ
    public enum EInputFieldType
    {
        NORMAL,
        PASSWORD
    }


    /// 入力フォームの初期化構造体
    public struct FInputFieldInitializer
    {
        // 入力判定を設置する Canvas
        private readonly Canvas _targetCanvas;
        public Canvas TargetCanvas => _targetCanvas;

        private readonly FArea _area;
        public FArea Area => _area;

        private readonly string _explanationText;
        public string DefaultText => _explanationText;

        private readonly string _text;
        public string Text => _text;

        private readonly int _maxDisplayTextLength;
        public int MaxDisplayTextLength => _maxDisplayTextLength;

        private readonly bool _isPlaySE;
        public bool IsPlaySE => _isPlaySE;

        private readonly bool _isEnabled;
        public bool IsEnabled => _isEnabled;

        private readonly bool _hasError;
        public bool HasError => _hasError;

        private readonly EInputFieldType _inputFieldType;
        public EInputFieldType InputFieldType => _inputFieldType;


        // 画像を設置する順(判定エリアは Canvas に置かれる)
        private readonly int _sortingOrder;
        public int SortingOrder => _sortingOrder;

        // 押下時の動作
        private Action _action;
        public Action Action => _action;


        public FInputFieldInitializer(
            Canvas targetCanvas,
            FArea area,
            string explanationText,
            string text,
            int maxDisplayTextLength,
            EInputFieldType inputFieldType,
            bool hasError,
            bool isPlaySE,
            bool isEnabled,
            int sortingOrder,
            Action action)
        {
            _targetCanvas = targetCanvas;
            _area = area;
            _explanationText = explanationText;
            _text = text;
            _maxDisplayTextLength = maxDisplayTextLength;
            _inputFieldType = inputFieldType;
            _hasError = hasError;
            _isPlaySE = isPlaySE;
            _isEnabled = isEnabled;
            _sortingOrder = sortingOrder;
            _action = action;
        }
    }
}
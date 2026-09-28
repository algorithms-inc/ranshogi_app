using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    /// スライダーの画像タイプ
    public enum ESliderImageType
    {
        SLIDER_DEFAULT,
    }


    /// スライダーの初期化構造体
    public struct FSliderInitializer
    {
        // 入力判定を設置する Canvas
        private readonly Canvas _targetCanvas;
        public Canvas TargetCanvas => _targetCanvas;

        private readonly ESliderImageType _sliderImageType;
        public ESliderImageType SliderImageType => _sliderImageType;

        private readonly FArea _area;
        public FArea Area => _area;

        private int _maxStep;
        public int MaxStep => _maxStep;

        private readonly int _presentStep;
        public int PresentSetp => _presentStep;

        private readonly bool _isPlaySE;
        public bool IsPlaySE => _isPlaySE;

        private readonly bool _isEnabled;
        public bool IsEnabled => _isEnabled;

        // 画像を設置する順(判定エリアは Canvas に置かれる)
        private readonly int _sortingOrder;
        public int SortingOrder => _sortingOrder;


        /// 押下時の動作
        private Action _actionOnChangeValue;
        public Action Action => _actionOnChangeValue;


        public FSliderInitializer(
            Canvas targetCanvas,
            ESliderImageType sliderImageType,
            FArea area,
            int maxStep,
            int presentStep,
            bool isPlaySE,
            bool isEnabled,
            int sortingOrder,
            Action actionOnChangeStep)
        {
            _targetCanvas = targetCanvas;
            _sliderImageType = sliderImageType;
            _area = area;
            _maxStep = maxStep;
            _presentStep = presentStep;
            _isPlaySE = isPlaySE;
            _isEnabled = isEnabled;
            _sortingOrder = sortingOrder;
            _actionOnChangeValue = actionOnChangeStep;
        }
    }
}
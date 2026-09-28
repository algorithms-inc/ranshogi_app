using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public struct FTextFieldInitializer
    {
        private readonly string _text;
        public string Text => _text;


        private readonly EFontType _fontType;
        public EFontType FontType => _fontType;


        private readonly int _fontSize;
        public int FontSize => _fontSize;


        private readonly EAlignment _alignment;
        public EAlignment Alignment => _alignment;

        private readonly FArea _area;
        public FArea Area => _area;

        private readonly float _lineSpacing;
        public float LineSpacing => _lineSpacing;


        private readonly bool _isOverFlowHorizontal;
        public bool IsOverFlowHorizontal => _isOverFlowHorizontal;

        private readonly bool _isOverFlowVertical;
        public bool IsOverFlowVertical => _isOverFlowVertical;

        private readonly bool _isResizeTextForBestFit;
        public bool IsResizeTextForBestFit => _isResizeTextForBestFit;

        private readonly FColor3 _color;
        public FColor3 Color => _color;


        private readonly float _defaultAlpha;
        public float DefaultAlpha => _defaultAlpha;


        private readonly Canvas _targetCanvas;
        public Canvas TargetCanvas => _targetCanvas;


        private readonly ETapJudgeType _tapJudgeType;
        public ETapJudgeType TapJudgeType => _tapJudgeType;

        private readonly Action _action;
        public Action Action => _action;



        public FTextFieldInitializer(
            string text,
            EFontType fontType,
            int fontSize,
            EAlignment alignment,
            FArea area,
            float lineSpacing,
            bool isOverFlowHorizontal,
            bool isOverFlowVertical,
            bool isResizeTextForBestFit,
            FColor3 color,
            float defaultAlpha,

            Canvas targetCanvas,
            ETapJudgeType tapJudgeType = ETapJudgeType.DOWN,
            Action action = null)
        {
            _text = text;
            _fontType = fontType;
            _fontSize = fontSize;
            _alignment = alignment;
            _area = area;
            _lineSpacing = lineSpacing;
            _isOverFlowHorizontal = isOverFlowHorizontal;
            _isOverFlowVertical = isOverFlowVertical;
            _isResizeTextForBestFit = isResizeTextForBestFit;
            _color = color;
            _defaultAlpha = defaultAlpha;
            _targetCanvas = targetCanvas;

            _tapJudgeType = tapJudgeType;
            _action = action;
        }
    }
}
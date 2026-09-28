using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;
using static System.Collections.Specialized.BitVector32;


namespace GFramework.Core
{
    // Unity では Nullable が使用しづらいため、struct ではなく class で設定
    public class FCompanionTextFieldSetting
    {
        private readonly FWindowElementArea _windowElementArea;
        public FWindowElementArea WindowElementArea => _windowElementArea;

        private readonly string _text;
        public string Text => _text;

        private readonly EFontType _fontType;
        public EFontType FontType => _fontType;

        private readonly float _fontScale;
        public float FontScale => _fontScale;

        private readonly EAlignment _alignment;
        public EAlignment Alignment => _alignment;

        
        public FCompanionTextFieldSetting(
            in FWindowElementArea windowElementArea,
            string text,
            EFontType fontType = EFontType.SERIF,
            float fontScale = 1.0f,
            EAlignment alignment = EAlignment.MIDDLE_LEFT)
        {
            _windowElementArea = windowElementArea;
            _text = text;
            _fontType = fontType;
            _fontScale = fontScale;
            _alignment = alignment;
        }
    }

    public interface IUICheckBoxCreator
    {
        void Add(
            string name,
            in FWindowElementArea area,
            Action actionOnTap,
            bool isOn = false,
            bool hasError = false,
            bool isEnabled = true,
            FCompanionTextFieldSetting companionTextFieldSetting = null);
        void Remove(string name);
        bool IsEnabled(string name);
        void SetIsEnabled(string name, bool value);
        bool IsOn(string name);
        void SetIsOn(string name, bool value);
        bool HasError(string name);
        void SetHasError(string name, bool value);
    }
}
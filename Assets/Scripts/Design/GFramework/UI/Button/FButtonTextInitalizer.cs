using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    /// ボタンの初期化構造体
    public struct FButtonTextInitializer
    {
        // 表示テキスト
        private readonly string _text;
        public string Text => _text;

        // フォントタイプ
        private readonly EFontType _fontType;
        public EFontType FontType => _fontType;

        // フォントサイズ
        private readonly int _fontSize;
        public int FontSize => _fontSize;

        private readonly EAlignment _alignment;
        public EAlignment Alignment => _alignment;


        public FButtonTextInitializer(
            string text,
            EFontType fontType,
            int fontSize,
            EAlignment alignment)
        {
            _text = text;
            _fontType = fontType;
            _fontSize = fontSize;
            _alignment = alignment;
        }
    }
}
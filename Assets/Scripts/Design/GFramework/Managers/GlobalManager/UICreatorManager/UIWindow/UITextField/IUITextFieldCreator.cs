using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    public interface IUITextFieldCreator
    {
        void Add(
            string name,
            in FWindowElementArea area,
            string text,
            EFontType fontType = EFontType.SERIF,
            float fontScale = 1.0f,
            EWindowTextColorType textColorType = EWindowTextColorType.NORMAL,
            EAlignment alignment = EAlignment.TOP_LEFT,
            Action actionOnTap = null);
        void Remove(string name);
        void ChangeText(string name, string text);
        void ChangeTextColor(string name, EWindowTextColorType textColorType);
    }
}
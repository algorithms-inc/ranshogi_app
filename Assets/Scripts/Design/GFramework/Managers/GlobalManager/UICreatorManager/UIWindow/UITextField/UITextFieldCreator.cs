using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public class UITextFieldCreator : IUITextFieldCreator
    {
        private WindowBehaviour _windowBehaviour;

        public UITextFieldCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;
        }

        public void Add(
            string name,
            in FWindowElementArea area,
            string text,
            EFontType fontType,
            float fontScale,
            EWindowTextColorType textColorType,
            EAlignment alignment,
            Action actionOnTap)
        {
            area.Validate(windowType: _windowBehaviour.WindowType);

            _windowBehaviour.AddTextFieldBehaviour(
                name: name,
                windowElementArea: area,
                fontType: fontType,
                text: text,
                fontScale: fontScale,
                textColorType: textColorType,
                alignment: alignment,
                actionOnTap: actionOnTap);
        }

        public void Remove(string name)
        {
            _windowBehaviour.RemoveTextFieldBehaviour(name: name);
        }

        public void ChangeText(string name, string text)
        {
            _windowBehaviour.ChangeText(name: name, text: text);
        }

        public void ChangeTextColor(string name, EWindowTextColorType textColorType)
        {
            _windowBehaviour.ChangeTextColor(name: name, textColorType: textColorType);
        }
    }
}
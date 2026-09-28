using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public class UIInputFieldCreator : IUIInputFieldCreator
    {
        private WindowBehaviour _windowBehaviour;

        public UIInputFieldCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;
        }


        public void Add(
            string name,
            in FWindowElementArea area,
            Action actionOnTap,
            string explanationText,
            string text,
            int maxDisplayTextLength,
            bool isPasswordMode,
            bool hasError,
            bool isPlaySE,
            bool isEnabled)
        {
            area.Validate(windowType: _windowBehaviour.WindowType);
            
            _windowBehaviour.AddInputField(
                name: name,
                windowElementArea: area,
                explanationText: explanationText,
                text: text,
                maxDisplayTextLength: maxDisplayTextLength,
                isPasswordMode: isPasswordMode,
                hasError: hasError,
                isPlaySE: isPlaySE,
                isEnabled: isEnabled,
                actionOnTap: actionOnTap);
        }


        public void Remove(string name)
        {
            _windowBehaviour.RemoveInputField(name: name);
        }


        public bool IsEnabled(string name)
        {
            return _windowBehaviour.IsInputFieldEnabled(name: name);
        }


        public void SetIsEnabled(string name, bool value)
        {
            _windowBehaviour.SetInputFieldIsEnabled(name: name, value: true);
        }

        public void ChangeText(string name, string text)
        {
            _windowBehaviour.SetInputFieldText(name: name, text: text);
        }

        public string GetText(string name)
        {
            return _windowBehaviour.GetInputFieldText(name: name);
        }


        public bool HasError(string name)
        {
            return _windowBehaviour.HasInputFieldError(name: name);
        }

        public void SetHasError(string name, bool value)
        {
            _windowBehaviour.SetInputFieldHasError(name: name, value: value);
        }
    }
}
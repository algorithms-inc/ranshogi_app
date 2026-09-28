using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public class UIButtonCreator : IUIButtonCreator
    {
        private WindowBehaviour _windowBehaviour;

        public UIButtonCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;
        }

        public void Add(
            string name,
            in FWindowElementArea area,
            Action actionOnTap,
            string text,
            EUIButtonDesignType buttonDesignType,
            bool isPlaySE,
            bool isEnabled)
        {
            area.Validate(windowType: _windowBehaviour.WindowType);
            
            _windowBehaviour.AddButton(
                name: name,
                windowElementArea: area,
                buttonDesignType: buttonDesignType,
                text: text,
                isPlaySE: isPlaySE,
                isEnabled: isEnabled,
                actionOnTap: actionOnTap);
        }

        public void Remove(string name)
        {
            _windowBehaviour.RemoveButton(name: name);
        }

        public bool IsEnabled(string name)
        {
            return _windowBehaviour.IsButtonEnabled(name: name);
        }

        public void SetIsEnabled(string name, bool value)
        {
            _windowBehaviour.SetButtonIsEnabled(name: name, value: value);
        }

        public void SetCompanionImage(string name, in FWindowElementArea area, string filePath)
        {
            area.Validate(windowType: _windowBehaviour.WindowType);

            _windowBehaviour.SetButtonCompanionImage(name: name, windowElementArea: area, filePath: filePath);
        }
    }
}
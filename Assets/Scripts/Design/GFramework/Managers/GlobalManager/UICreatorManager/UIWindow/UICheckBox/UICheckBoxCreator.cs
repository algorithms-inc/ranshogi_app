using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;
using System.Diagnostics;
using UnityEngine;


namespace GFramework
{
    public class UICheckBoxCreator : IUICheckBoxCreator
    {
        private WindowBehaviour _windowBehaviour;

        public UICheckBoxCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;
        }

        public void Add(
            string name,
            in FWindowElementArea area,
            Action actionOnTap,
            bool isOn = false,
            bool hasError = false,
            bool isEnabled = true,
            FCompanionTextFieldSetting companionTextFieldSetting = null)
        {
            UnityEngine.Debug.Log("windowType: " + _windowBehaviour.WindowType);
            area.Validate(windowType: _windowBehaviour.WindowType);

            _windowBehaviour.AddCheckBox(
                name: name,
                windowElementArea: area,
                isOn: isOn,
                hasError: hasError,
                isEnabled: isEnabled,
                actionOnTap: actionOnTap,
                companionTextFieldSetting: companionTextFieldSetting);
        }

        public void Remove(string name)
        {
            _windowBehaviour.RemoveCheckBox(name: name);
        }

        public bool IsEnabled(string name)
        {
            return _windowBehaviour.IsCheckBoxEnabled(name: name);
        }

        public void SetIsEnabled(string name, bool value)
        {
            _windowBehaviour.SetCheckBoxIsEnabled(name: name, value: value);
        }

        public bool IsOn(string name)
        {
            return _windowBehaviour.IsCheckBoxOn(name: name);
        }

        public void SetIsOn(string name, bool value)
        {
            _windowBehaviour.SetCheckBoxIsOn(name: name, value: value);
        }

        public bool HasError(string name)
        {
            return _windowBehaviour.HasCheckBoxError(name: name);
        }

        public void SetHasError(string name, bool value)
        {
            _windowBehaviour.SetCheckBoxHasError(name: name, value: true);
        }
    }
}
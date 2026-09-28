using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;
using System.Diagnostics;


namespace GFramework
{
    public class UIRadioButtonGroupCreator : IUIRadioButtonGroupCreator
    {
        private WindowBehaviour _windowBehaviour;

        public UIRadioButtonGroupCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;
        }

        public void Add(
            string name,
            in List<FWindowElementArea> areaList,
            in List<FCompanionTextFieldSetting> companionTextFieldSettingList,
            int indexOfOn,
            Action actionOnTap,
            bool isEnabled)
        {
            areaList.ForEach(area => area.Validate(windowType: _windowBehaviour.WindowType));
            
            _windowBehaviour.AddRadioButtonGroup(
                name: name,
                windowElementAreaList: areaList,
                companionTextFieldSettingList: companionTextFieldSettingList,
                indexOfOn: indexOfOn,
                isEnabled: isEnabled,
                actionOnChange: actionOnTap);
        }

        public void Remove(string name)
        {
            _windowBehaviour.RemoveRadioButtonGroup(name: name);
        }

        public bool IsEnabled(string name)
        {
            return _windowBehaviour.IsRadioButtonGroupEnabled(name: name);
        }

        public void SetIsEnabled(string name, bool value)
        {
            _windowBehaviour.SetRadioButtonGroupIsEnabled(name: name, value: value);
        }

        public int GetIndexOfOn(string name)
        {
            UnityEngine.Debug.Log(name);
            return _windowBehaviour.GetRadioButtonGroupIndexOfOn(name: name);
        }

        public void SetIndexOfOn(string name, int indexOfOn)
        {
            _windowBehaviour.SetRadioButtonGroupIndexOfOn(name: name, indexOfOn: indexOfOn);
        }
    }
}
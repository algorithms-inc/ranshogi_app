using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    public interface IUIRadioButtonGroupCreator
    {
        void Add(
            string name,
            in List<FWindowElementArea> areaList,
            in List<FCompanionTextFieldSetting> companionTextFieldSettingList = null,
            int indexOfOn = 0,
            Action actionOnTap = null,
            bool isEnabled = true);
        void Remove(string name);
        bool IsEnabled(string name);
        void SetIsEnabled(string name, bool value);
        int GetIndexOfOn(string name);
        void SetIndexOfOn(string name, int indexOfOn);
    }
}
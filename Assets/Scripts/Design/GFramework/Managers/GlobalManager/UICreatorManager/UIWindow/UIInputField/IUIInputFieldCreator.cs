using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    public interface IUIInputFieldCreator
    {
        void Add(
            string name,
            in FWindowElementArea area,
            Action actionOnTap,
            string explanationText = "",
            string text = "",
            int maxDisplayTextLength = 0,
            bool isPasswordMode = false,
            bool hasError = false,
            bool isPlaySE = false,
            bool isEnabled = true);
        void Remove(string name);
        bool IsEnabled(string name);
        void SetIsEnabled(string name, bool value);
        void ChangeText(string name, string text);
        string GetText(string name);
        bool HasError(string name);
        void SetHasError(string name, bool value);
    }
}
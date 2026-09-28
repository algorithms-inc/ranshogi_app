using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    public interface IUISliderCreator
    {
        void Add(
            string name,
            in FWindowElementArea area,
            int maxStep,
            int presentStep = 0,
            Action actionOnChangeStep = null,
            bool isPlaySE = true,
            bool isEnabled = true);
        void Remove(string name);
        bool IsEnabled(string name);
        void SetIsEnabled(string name, bool value);
        int GetMaxStep(string name);
        int GetPresentStep(string name);
        void SetPresentStep(string name, int step);
        void AddPresentStep(string name, int deltaStep);
    }
}
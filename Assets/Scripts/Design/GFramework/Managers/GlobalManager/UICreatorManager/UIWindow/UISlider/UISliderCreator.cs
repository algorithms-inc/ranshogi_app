using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public class UISliderCreator : IUISliderCreator
    {
        private WindowBehaviour _windowBehaviour;

        public UISliderCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;
        }

        public void Add(
            string name,
            in FWindowElementArea area,
            int maxStep,
            int presentStep ,
            Action actionOnChangeStep,
            bool isPlaySE,
            bool isEnabled)
        {
            area.Validate(windowType: _windowBehaviour.WindowType);
            
            _windowBehaviour.AddSlider(
                name: name,
                windowElementArea: area,
                maxStep: maxStep,
                presentStep: presentStep,
                isPlaySE: isPlaySE,
                isEnabled: isEnabled,
                actionOnChangeStep: actionOnChangeStep);
        }

        public void Remove(string name)
        {
            _windowBehaviour.RemoveSlider(name: name);
        }

        public bool IsEnabled(string name)
        {
            return _windowBehaviour.IsSliderEnabled(name: name);
        }

        public void SetIsEnabled(string name, bool value)
        {
            _windowBehaviour.SetSliderIsEnabled(name: name, value: value);
        }

        public int GetMaxStep(string name)
        {
            return _windowBehaviour.GetSliderMaxStep(name: name);
        }

        public int GetPresentStep(string name)
        {
            return _windowBehaviour.GetSliderPresentStep(name: name);
        }

        public void SetPresentStep(string name, int step)
        {
            _windowBehaviour.SetSliderPresentStep(name: name, step: step);
        }

        public void AddPresentStep(string name, int deltaStep)
        {
            _windowBehaviour.AddSliderPresentStep(name: name, deltaStep: deltaStep);
        }
    }
}
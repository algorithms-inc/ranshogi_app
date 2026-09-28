using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public class UIWindowCreator : IUIWindowCreator
    {
        private readonly WindowBehaviour _windowBehaviour;

        private IUITextFieldCreator _UITextField;
        public IUITextFieldCreator TextField => _UITextField;

        private IUIButtonCreator _UIButton;
        public IUIButtonCreator Button => _UIButton;

        private IUIImageBoxCreator _UIImageBox;
        public IUIImageBoxCreator ImageBox => _UIImageBox;

        private IUIInputFieldCreator _UIInputField;
        public IUIInputFieldCreator InputField => _UIInputField;

        private IUICheckBoxCreator _UICheckBox;
        public IUICheckBoxCreator CheckBox => _UICheckBox;

        private IUIRadioButtonGroupCreator _UIRadioButtonGroup;
        public IUIRadioButtonGroupCreator RadioButtonGroup => _UIRadioButtonGroup;

        private IUISliderCreator _UISlider;
        public IUISliderCreator Slider => _UISlider;



        private Action _onDidShowAction = null;
        private Action _onDidHideAction = null;


        public UIWindowCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;

            _UITextField = new UITextFieldCreator(windowBehaviour: _windowBehaviour);
            _UIButton = new UIButtonCreator(windowBehaviour: _windowBehaviour);
            _UIImageBox = new UIImageBoxCreator(windowBehaviour: _windowBehaviour);
            _UIInputField = new UIInputFieldCreator(windowBehaviour: _windowBehaviour);
            _UICheckBox = new UICheckBoxCreator(windowBehaviour: _windowBehaviour);
            _UIRadioButtonGroup = new UIRadioButtonGroupCreator(windowBehaviour: _windowBehaviour);
            _UISlider = new UISliderCreator(windowBehaviour: _windowBehaviour);
        }



        public bool IsShown()
        {
            return _windowBehaviour.IsShown;
        }

        public void Show(Action onDidShowAction = null)
        {
            if (_windowBehaviour.IsShown) { return; }

            _onDidShowAction = onDidShowAction;
            

            _windowBehaviour.Show();
            _onDidShowAction?.Invoke();
        }


        public void Hide(Action onDidHideAction = null)
        {
            if (_windowBehaviour.IsShown == false) { return; }


            _onDidHideAction = onDidHideAction;
            

            _windowBehaviour.Hide();
            _onDidHideAction?.Invoke();
        }

        public bool IsActionEnabled()
        {
            return _windowBehaviour.IsActionEnabled;
        }


        public void SetIsActionEnabled(bool value)
        {
            _windowBehaviour.SetIsActionEnabled(value: value);
        }


        public void RemoveAllUIParts()
        {
            _windowBehaviour.RemoveAllUIParts();
        }
    }
}
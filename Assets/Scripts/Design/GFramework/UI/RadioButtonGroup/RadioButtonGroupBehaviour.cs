using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class RadioButtonGroupBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        // Radio Button
        private List<CheckBoxBehaviour> _radioButtonBehaviourList = new List<CheckBoxBehaviour>();

        private int _indexOfOn = 0;
        public int IndexOfOn => _indexOfOn;

        private WindowBehaviour _windowBehaviour;

        // Alpha
        public float Alpha => UtlSprite.GetSpriteAlpha(gameObject: _radioButtonBehaviourList[0].gameObject);


        // Action
        private bool _isActionEnabled = true;
        public bool IsActionEnabled => _isActionEnabled;
        private Action _action;

        // is Enabled (主に表示上の切り替えをするものであり、Tap の実行可否は別の変数で管理)
        private bool _isEnabled = true;
        public bool IsEnabled => _isEnabled;

        // is Shown
        private bool _isShown = true;
        public bool IsShown => _isShown;

        // is Play SE
        private bool _isPlaySE = true;
        public bool IsPlaySE => _isPlaySE;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(FRadioButtonGroupInitializer initializer, WindowBehaviour windowBehaviour)
        {
            _indexOfOn = initializer.IndexOfOn;
            _action = initializer.Action;
            _isEnabled = initializer.IsEnabled;
            _isPlaySE = initializer.IsPlaySE;
            _windowBehaviour = windowBehaviour;

            SetupRadioButton(areaList: initializer.AreaList, companionTextFieldSettingList: initializer.CompanionTextFieldSettingList, targetCanvas: initializer.TargetCanvas);

            Hide();
        }


        private void SetupRadioButton(in List<FWindowElementArea> areaList, in List<FCompanionTextFieldSetting> companionTextFieldSettingList, Canvas targetCanvas)
        {
            Assert.IsTrue(2 <= areaList.Count);

            var index = 0;

            foreach (var i in areaList)
            {
                GameObject gameObjectRadioButton = new GameObject(name: name);
                var radioButtonBehaviour = gameObjectRadioButton.AddComponent<CheckBoxBehaviour>();

                gameObjectRadioButton.transform.parent = gameObject.transform;


                int sortingOrder = _windowBehaviour.WindowType switch
                {
                    EWindowType.NORMAL => Config.WINDOW_NORMAL_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                    EWindowType.ERROR => Config.WINDOW_ERROR_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                    EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                    _ => throw new NotImplementedException(),
                };


                var area = _windowBehaviour.CalcUIPartsArea(windowElementArea: i);
                var isOn = index == _indexOfOn;

                var initializer = new FCheckBoxInitializer(
                    targetCanvas: targetCanvas,
                    area: area.AsYReversed(),
                    isOn: isOn,
                    hasError: false,
                    isPlaySE: _isPlaySE,
                    isEnabled: _isEnabled,
                    functionType: ECheckBoxFunctionType.RADIO_BUTTON,
                    sortingOrder: sortingOrder,
                    action: () => { FireAction(); });

                radioButtonBehaviour.SetupComponent(initializer: initializer, windowBehaviour: _windowBehaviour);


                _radioButtonBehaviourList.Add(radioButtonBehaviour);

                index++;
            }


            if (companionTextFieldSettingList == null) { return; }

            // CompanionTextField の設定

            Assert.IsTrue(companionTextFieldSettingList.Count == _radioButtonBehaviourList.Count);

            index = 0;

            foreach (var i in companionTextFieldSettingList)
            {
                if (i != null)
                {
                    _radioButtonBehaviourList[index].SetupCompanionTextField(setting: i);
                }

                index++;
            }
        }



        // ===============================================
        // MARK: - ● Debug ─────

        /// Debug: タップエリアを表示
        public void ShowTapArea()
        {
            _radioButtonBehaviourList.ForEach(i => i.ShowTapArea());
        }


        /// Debug: タップエリアを非表示
        public void HideTapArea()
        {
            _radioButtonBehaviourList.ForEach(i => i.HideTapArea());
        }



        // ===============================================
        // MARK: - ● Action ─────

        /// Action の実行可否をセット
        public void SetIsActionEnabled(bool value)
        {
            _isActionEnabled = value;
        }


        /// Action をセット
        public void SetAction(Action action)
        {
            _action = action;
        }


        /// Action を実行
        /// - 何らかのラジオボタンがONになった時に呼ばれる関数
        private void FireAction()
        {
            _radioButtonBehaviourList[_indexOfOn].SetIsOn(value: false);
            _indexOfOn = CheckChangedRadioButtonIndex();

            if (_isActionEnabled)
            {
                _action?.Invoke();
            }
        }


        // どのラジオボタンがONになったかを調べる
        private int CheckChangedRadioButtonIndex()
        {
            var resultIndex = 0;

            for (int i = 0; i < _radioButtonBehaviourList.Count; i++)
            {
                // 実際には _indexOfOn はすでにOFF状態になっているので、調べても問題はないが念の為スキップ
                if (i == _indexOfOn) { continue; }

                if (_radioButtonBehaviourList[i].IsOn)
                {
                    resultIndex = i;
                }
            }

            return resultIndex;
        }



        // ===============================================
        // MARK: - ● Local Position and Size ─────

        public Vector2 GetLocalPosition()
        {
            throw new NotImplementedException();
        }


        public void SetLocalPosition(Vector2 position)
        {
            throw new NotImplementedException();
        }


        public Vector2 GetSize()
        {
            throw new NotImplementedException();
        }


        public void SetSize(Vector2 size)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Get / Set Color ─────

        public FColor3 GetColor()
        {
            throw new NotImplementedException();
        }


        public void SetColor(FColor3 color)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public void SetAlpha(float alpha)
        {
            _radioButtonBehaviourList.ForEach(i => i.SetAlpha(alpha: alpha));
        }


        public void SetColorAndAlpha(FColor3 color, float alpha)
        {
            _radioButtonBehaviourList.ForEach(i => i.SetColorAndAlpha(color: color, alpha: alpha));
        }



        // ===============================================
        // MARK: - ● Set Index of On ─────

        private bool IsValidIndexOfOn(int value)
        {
            return (0 <= value) && (value < _radioButtonBehaviourList.Count);
        }


        public void SetIndexOfOn(int indexOfOn)
        {
            Assert.IsTrue(IsValidIndexOfOn(value: indexOfOn));

            if (_indexOfOn == indexOfOn) { return; }

            _radioButtonBehaviourList[_indexOfOn].SetIsOn(value: false);

            _indexOfOn = indexOfOn;
            _radioButtonBehaviourList[_indexOfOn].SetIsOn(value: true);
        }



        // ===============================================
        // MARK: - ● Fade Duration ─────

        public void SetFadeDuration(float durationFadeIn, float durationFadeOut)
        {
            _radioButtonBehaviourList.ForEach(i => i.SetFadeDuration(durationFadeIn: durationFadeIn, durationFadeOut: durationFadeOut));
        }



        // ===============================================
        // MARK: - ● Set is Enable ─────

        public void SetIsEnabled(bool value)
        {
            if (value == _isEnabled) { return; }

            _isEnabled = value;
            _radioButtonBehaviourList.ForEach(i => i.SetIsEnabled(value: _isEnabled, willChangeButtonTexture: true));
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void Show()
        {
            if (_isShown) { return; }

            _radioButtonBehaviourList.ForEach(i => i.Show());

            _isShown = true;
        }


        public void Hide()
        {
            if (_isShown == false) { return; }

            _radioButtonBehaviourList.ForEach(i => i.Hide());

            _isShown = false;
        }


        public void FadeIn()
        {
            if (_isShown) { return; }

            _radioButtonBehaviourList.ForEach(i => i.FadeIn());

            _isShown = true;
        }


        public void FadeOut()
        {
            if (_isShown == false) { return; }

            _radioButtonBehaviourList.ForEach(i => i.FadeOut());

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            DestroyRelatedGameObject();
            ReleaseReference();
        }


        private void DestroyRelatedGameObject()
        {
            _radioButtonBehaviourList.ForEach(i => Utl.SafeDestroy(behaviour: i));
        }


        private void ReleaseReference()
        {
            _radioButtonBehaviourList = null;

            _action = null;
        }
    }
}
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
    public class InputFieldBehaviour : AButtonBaseBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private string _explanationText = "";
        public string ExplanationText => _explanationText;

        private string _text = "";
        public string Text => _text;

        // has Error
        private bool _hasError = false;
        public bool HasError => _hasError;


        private EInputFieldType _inputFieldType;

        // 最大表示文字数 (0なら無制限)
        private int _maxDisplayTextLength = 0;


        // TODO: 最終的にはConfigやColorManagerにまとめる

        // エラー時にテキストの色を変えることはないため用意せず
        private static FColor3 TEXT_COLOR_TEXT_DEFAULT = new FColor3(r: 0f, g: 0f, b: 0f);
        private static FColor3 TEXT_COLOR_EXPLANATION_TEXT_DEFAULT = new FColor3(r: 0.5f, g: 0.5f, b: 0.5f);


        public void SetupComponent(in FInputFieldInitializer initializer)
        {
            _text = initializer.Text;
            _maxDisplayTextLength = initializer.MaxDisplayTextLength;
            _explanationText = initializer.DefaultText;
            _inputFieldType = initializer.InputFieldType;
            _area = initializer.Area;
            _isPlaySE = initializer.IsPlaySE;
            _isEnabled = initializer.IsEnabled;
            _hasError = initializer.HasError;


            SetupButtonBody(sortingOrder: initializer.SortingOrder);
            SetupTapInput(targetCanvas: initializer.TargetCanvas);
            SetAction(action: initializer.Action);


            var buttonTextInitializer = new FButtonTextInitializer(
                text: "", // Password / 入力なしなどの状態があるため、まずは空文字で用意
                fontType: EFontType.SERIF,
                fontSize: (int)(initializer.Area.size.y * 0.5f),
                alignment: EAlignment.MIDDLE_LEFT);

            SetupText(buttonTextInitializer: buttonTextInitializer);

            SetText(text: _text);

            SetIsEnabled(value: initializer.IsEnabled, willChangeButtonTexture: true);

            Hide();
        }


        /// 画像のセットアップ
        private void SetupButtonBody(int sortingOrder)
        {
            string texturePathUnderResources;

            if (_isEnabled)
            {
                texturePathUnderResources = Locator.Instance.textureManager.GetFilePathOfInputFieldImage(
                    buttonStatusType: EButtonStatusType.DEFAULT,
                    hasError: _hasError);
            }
            else
            {
                texturePathUnderResources = Locator.Instance.textureManager.GetFilePathOfInputFieldImage(
                    buttonStatusType: EButtonStatusType.DISABLED,
                    hasError: _hasError);
            }

            SetupButtonBody(texturePathUnderResources: texturePathUnderResources, sortingOrder: sortingOrder, isResizable: true);
        }


        protected override void ChangeButtonTexture(EButtonStatusType buttonStatusType)
        {
            string texturePathUnderResources = Locator.Instance.textureManager.GetFilePathOfInputFieldImage(
                buttonStatusType: buttonStatusType,
                hasError: _hasError);

            ChangeButtonTexture(texturePathUnderResources: texturePathUnderResources, isResizable: true);
        }



        // ===============================================
        // MARK: - ● Text ─────

        public override void SetText(string text)
        {
            _text = text;

            if (_text == "")
            {
                base.SetText(text: _explanationText);
                base.SetTextColor(color: TEXT_COLOR_EXPLANATION_TEXT_DEFAULT);
                return;
            }


            if (_inputFieldType == EInputFieldType.PASSWORD)
            {
                var count = _text.Length;

                var secretText = "";

                for (int i = 0; i < count; i++)
                {
                    secretText += "*";
                }

                base.SetText(text: secretText);
                base.SetTextColor(color: TEXT_COLOR_TEXT_DEFAULT);
            }
            else
            {
                // EInputFieldType.DEFAULT

                // 最大表示文字数を超えていたら、末尾を「...」にする
                // _text は正しい値なので修正はしない
                if ((_maxDisplayTextLength > 0) && (_text.Length > _maxDisplayTextLength))
                {
                    base.SetText(text: _text.Substring(startIndex: 0, length: _maxDisplayTextLength) + "...");
                }
                else
                {
                    base.SetText(text: _text);
                }

                base.SetTextColor(color: TEXT_COLOR_TEXT_DEFAULT);
            }
        }


        public void SetInputFieldType(EInputFieldType inputFieldType)
        {
            if (_inputFieldType == inputFieldType) { return; };

            _inputFieldType = inputFieldType;


            if (inputFieldType == EInputFieldType.PASSWORD)
            {
                SetText(text: _text);
            }
        }


        // ===============================================
        // MARK: - ● Has Error ─────

        public void SetHasError(bool value)
        {
            if (_hasError == value) { return; }

            _hasError = value;

            if (_isEnabled)
            {
                ChangeButtonTexture(buttonStatusType: EButtonStatusType.DEFAULT);
            }
            else
            {
                ChangeButtonTexture(buttonStatusType: EButtonStatusType.DISABLED);
            }
        }
    }
}
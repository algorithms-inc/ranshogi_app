using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class TextFieldBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private Text _textComponent;

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _textComponent.color.a;

        public TextAnchor Alignment => _textComponent.alignment;


        // Input
        private TapInputBehaviour _tapInputBehaviour;

        // Judge Type
        private ETapJudgeType _tapJudgeType;

        // Area
        private FArea _area;

        // Tap Block (アニメーション実行時のタップブロック)
        private bool _isTapBlockedByAnimation = false; // アニメーション実行時のタップブロック
        public bool IsTapBlockedByAnimation
        {
            set { _isTapBlockedByAnimation = value; }
            get { return _isTapBlockedByAnimation; }
        }

        // Action
        private bool _isActionEnabled = true;
        public bool IsActionEnabled => _isActionEnabled;
        private Action _action;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(in FTextFieldInitializer textInitializer)
        {
            _area = textInitializer.Area;
            _tapJudgeType = textInitializer.TapJudgeType;
            _action = textInitializer.Action;
            

            SetupGameObjectParent(targetCanvas: textInitializer.TargetCanvas);
            SetupText(textInitializer: textInitializer);

            SetDefaultAlpha(alpha: textInitializer.DefaultAlpha);

            if (_action != null)
            {
                // テキストと同じターゲットキャンバスで問題なさそう
                SetupTapInput(targetCanvas: textInitializer.TargetCanvas);
            }

            Hide();
        }


        private void SetupGameObjectParent(Canvas targetCanvas)
        {
            gameObject.transform.parent = targetCanvas.transform;
        }


        private void SetupText(in FTextFieldInitializer textInitializer)
        {
            // ここでスケールを1で指定しないと、親GameObjectの逆比で表示される（表示上、1となるように値がセットされる）
            transform.localScale = new Vector2(1, 1);

            _textComponent = gameObject.AddComponent<Text>();
            _textComponent.raycastTarget = false;


            _textComponent.lineSpacing = textInitializer.LineSpacing;

            _textComponent.horizontalOverflow = textInitializer.IsOverFlowHorizontal ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap;
            _textComponent.verticalOverflow = textInitializer.IsOverFlowVertical ? VerticalWrapMode.Overflow : VerticalWrapMode.Truncate;

            SetLocalPosition(position: textInitializer.Area.CenterPoint);
            SetSize(size: textInitializer.Area.size);
            SetFont(fontType: textInitializer.FontType);
            SetFontSize(size: textInitializer.FontSize);
            SetAlignment(alignment: textInitializer.Alignment);
            SetText(text: textInitializer.Text);
            SetColorAndAlpha(color: textInitializer.Color, alpha: textInitializer.DefaultAlpha);

            // true にしても上手く機能しないことが多い
            _textComponent.resizeTextForBestFit = textInitializer.IsResizeTextForBestFit;
        }


        // タップ入力のセットアップ
        private void SetupTapInput(Canvas targetCanvas)
        {
            var gameObjectTapInput = new GameObject(name: "TapInputForText");

            _tapInputBehaviour = gameObjectTapInput.AddComponent<TapInputBehaviour>();

            gameObjectTapInput.transform.parent = targetCanvas.transform;

            _tapInputBehaviour.SetupComponent(
                size: _area.size,
                position: _area.CenterPoint,
                isEnabled: true,
                actionOnPointerDown: null,
                actionOnPointerUp: null,
                actionOnPointerClick: null,
                actionOnPointerEnter: null,
                actionOnPointerExit: null);


            switch (_tapJudgeType)
            {
                case ETapJudgeType.DOWN:
                    _tapInputBehaviour.SetActionOnPointerDown(
                        action: (ETapInputType tapInputType, int durationFrame) => { FireAction(); });
                    break;

                case ETapJudgeType.CLICK:
                    _tapInputBehaviour.SetActionOnPointerClick(
                        action: (ETapInputType tapInputType, int durationFrame) => { FireAction(); });
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }


        // ===============================================
        // MARK: - ● Action ─────

        /// Action の実行可否をセット
        public void SetIsActionEnabled(bool value)
        {
            _isActionEnabled = value;
        }


        /// Action を実行
        private void FireAction()
        {
            if (_isTapBlockedByAnimation == true) { return; }


            if (_isActionEnabled)
            {
                _action?.Invoke();
            }
        }



        // ===============================================
        // MARK: - ● Local Position and Size ─────

        public Vector2 GetLocalPosition()
        {
            return _textComponent.rectTransform.localPosition;
        }


        public void SetLocalPosition(Vector2 position)
        {
            _textComponent.rectTransform.localPosition = position;
        }


        public Vector2 GetSize()
        {
            return _textComponent.rectTransform.sizeDelta;
        }


        public void SetSize(Vector2 size)
        {
            _textComponent.rectTransform.sizeDelta = size;
        }



        // ===============================================
        // MARK: - ● Set Font / Font Size ─────

        public int GetFontSize()
        {
            return _textComponent.fontSize;
        }


        public void SetFontSize(int size)
        {
            _textComponent.fontSize = size;
        }



        public void SetFont(EFontType fontType)
        {
            _textComponent.font = Locator.Instance.fontManager.GetFont(fontType: fontType);
        }



        // ===============================================
        // MARK: - ● Get / Set Text ─────

        public string GetText()
        {
            return _textComponent.text;
        }


        public void SetText(string text)
        {
            _textComponent.text = text;
        }


        // ===============================================
        // MARK: - ● Get / Set Color ─────

        public FColor3 GetColor()
        {
            return new FColor3(_textComponent.color);
        }


        public void SetColor(FColor3 color)
        {
            _textComponent.color = color.AsSystemColor();
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _textComponent.color = new FColor4(color: GetColor(), a: alpha).AsSystemColor();
        }


        public void SetColorAndAlpha(FColor3 color, float alpha)
        {
            _textComponent.color = new FColor4(color: color, a: alpha).AsSystemColor();
        }



        // ===============================================
        // MARK: - ● Set / Get Alignment ─────

        public void SetAlignment(EAlignment alignment)
        {
            _textComponent.alignment = Utl.ConvertToTextAnchor(alignment: alignment);
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; };

            _isTapBlockedByAnimation = false;

            ResetAlpha();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; };

            _isTapBlockedByAnimation = true;

            SetAlpha(alpha: 0f);

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; };

            RegisterAction(action: CAction.CreateImageActionFade(targetBehaviour: this, fadeType: CAction.EFadeType.FADE_IN, duration: Config.ANIMATION_DURATION_2, actionOnDidFinish: () => { _isTapBlockedByAnimation = false; }));

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; };

            _isTapBlockedByAnimation = true;

            RegisterAction(action: CAction.CreateImageActionFade(targetBehaviour: this, fadeType: CAction.EFadeType.FADE_OUT, duration: Config.ANIMATION_DURATION_2));

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
            Utl.SafeDestroy(behaviour: _tapInputBehaviour);
        }

        private void ReleaseReference()
        {
            _tapInputBehaviour = null;

            _action = null;
        }
    }
}
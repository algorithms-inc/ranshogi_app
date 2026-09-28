using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    /// ボタン系UIの基本となるビヘイビア
    public abstract class AButtonBaseBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        // Text
        private TextFieldBehaviour _textFieldBehaviour;

        // Body
        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;

        // Companion Image
        private SingleImageActionBehaviour _companionImageSingleImageActionBehaviour;

        // Input
        private TapInputBehaviour _tapInputBehaviour;

        // Judge Type
        protected ETapJudgeType _tapJudgeType;

        // Area
        protected FArea _area;

        // Alpha
        public float Alpha => UtlSprite.GetSpriteAlpha(gameObject: _bodySingleImageActionBehaviour.gameObject);

        // Tap Block (アニメーション実行時のタップブロック)
        // - 表示はされているが、タップを発生させたくない時に設定
        // - ボタン外からも設定することができる
        private bool _isTapBlockedByAnimation = false;
        public bool IsTapBlockedByAnimation
        {
            set { _isTapBlockedByAnimation = value; }
            get { return _isTapBlockedByAnimation; }
        }

        // for Action
        private bool _isActionEnabled = true;
        public bool IsActionEnabled => _isActionEnabled;
        private Action _action;

        // is Play SE
        protected bool _isPlaySE = true;
        public bool IsPlaySE => _isPlaySE;

        // is Enabled (主に表示上の切り替えをするものであり、Actionの実行可否は別の変数で管理)
        protected bool _isEnabled = true;
        public bool IsEnabled => _isEnabled;

        // is Shown
        private bool _isShown = true;
        public bool IsShown => _isShown;

        // companionImage 設定のために保存しておく SortingOrder
        private int _sortingOrder = 0;


        // ===============================================
        // MARK: - ● Setup ─────

        /// 画像のセットアップ
        protected void SetupButtonBody(string texturePathUnderResources, int sortingOrder, bool isResizable)
        {
            GameObject gameObjectButtonBody;

            _sortingOrder = sortingOrder;

            if (isResizable)
            {
                gameObjectButtonBody = UtlSprite.CreateGameObjectWithResizableSprite(
                    name: "ButtonBody",
                    texturePathUnderResources: texturePathUnderResources,
                    willCacheIfNeeded: true,
                    sortingOrder: _sortingOrder,
                    transformParent: transform,
                    border: new Vector4(x: 64f, y: 32f, z: 64f, w: 32f)); // 縦方向が短い画像になりやすいので、縦方向はborderを小さくしている

                var spriteRenderer = gameObjectButtonBody.GetComponent<SpriteRenderer>();
                spriteRenderer.size = Utl.ConvertImageSizeToUnit(imageSize: _area.size);
            }
            else
            {
                gameObjectButtonBody = UtlSprite.CreateGameObjectWithSprite(
                    name: "ButtonBody",
                    texturePathUnderResources: texturePathUnderResources,
                    willCacheIfNeeded: true,
                    sortingOrder: _sortingOrder,
                    transformParent: transform);

                var textureSize = UtlSprite.GetTextureSize(gameObject: gameObjectButtonBody);
                gameObjectButtonBody.transform.localScale = _area.size / textureSize;
            }


            transform.localScale = new Vector2(1f, 1f);
            transform.position = Utl.ConvertImageSizeToUnit(imageSize: _area.CenterPoint);

            _bodySingleImageActionBehaviour = gameObjectButtonBody.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(defaultAlpha: 1, durationFadeIn: 0.3f, durationFadeOut: 0.3f);

            _bodySingleImageActionBehaviour.SetupComponent(initializer: initializer);
        }


        /// タップ入力のセットアップ
        protected void SetupTapInput(Canvas targetCanvas)
        {
            var gameObjectTapInput = new GameObject(name: "TapInput");

            _tapInputBehaviour = gameObjectTapInput.AddComponent<TapInputBehaviour>();

            gameObjectTapInput.transform.parent = targetCanvas.transform;

            Action<ETapInputType, int> basicAction = (ETapInputType tapInputType, int durationFrame) => { ChangeButtonTextureAccordingToTapInputType(tapInputType: tapInputType); };

            _tapInputBehaviour.SetupComponent(
                size: _area.size,
                position: _area.CenterPoint,
                isEnabled: _isEnabled,
                actionOnPointerDown: basicAction,
                actionOnPointerUp: basicAction,
                actionOnPointerClick: basicAction,
                actionOnPointerEnter: basicAction,
                actionOnPointerExit: basicAction);


            switch (_tapJudgeType)
            {
                case ETapJudgeType.DOWN:
                    _tapInputBehaviour.SetActionOnPointerDown(
                        action: (ETapInputType tapInputType, int durationFrame) => { FireAction(); ChangeButtonTextureAccordingToTapInputType(tapInputType: tapInputType); });
                    break;

                case ETapJudgeType.CLICK:
                    _tapInputBehaviour.SetActionOnPointerClick(
                        action: (ETapInputType tapInputType, int durationFrame) => { FireAction(); ChangeButtonTextureAccordingToTapInputType(tapInputType: tapInputType); });
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ● Text ─────

        public void SetupText(in FButtonTextInitializer buttonTextInitializer)
        {
            Assert.IsTrue(_textFieldBehaviour == null);

            Canvas targetCanvas = _tapInputBehaviour.transform.parent.GetComponent<Canvas>();

            // 左づめにすることがあるので、テキストの表示サイズは小さくする
            var area = new FArea(
                p1: _area.CenterPoint,
                p2: new Vector2(_area.size.x * 0.92f, _area.size.y),
                areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);


            var textInitializer = new FTextFieldInitializer(
                text: buttonTextInitializer.Text,
                fontType: buttonTextInitializer.FontType,
                fontSize: buttonTextInitializer.FontSize,
                alignment: buttonTextInitializer.Alignment,
                area: area,
                lineSpacing: 0f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1,
                targetCanvas: targetCanvas);

            var newGameObject = new GameObject(name: "ButtonText");
            newGameObject.transform.parent = gameObject.transform;

            _textFieldBehaviour = newGameObject.AddComponent<TextFieldBehaviour>();


            _textFieldBehaviour.SetupComponent(textInitializer: textInitializer);

            if (_isShown) {_textFieldBehaviour.Show(); } else {_textFieldBehaviour.Hide();}

            ChangeTextAlphaAccordingToIsEnabled();
        }


        public virtual void SetText(string text)
        {
            Assert.IsTrue(_textFieldBehaviour != null);

            _textFieldBehaviour.SetText(text: text);
        }


        /// <summary>作った後で文字の大きさを変える。並んだボタンの文字サイズを揃えるときに使う。</summary>
        public void SetFontSize(int size)
        {
            Assert.IsTrue(_textFieldBehaviour != null);

            _textFieldBehaviour.SetFontSize(size: size);
        }



        // ===============================================
        // MARK: - ● Companion Image ─────

        public void SetupCompanionImage(string texturePathUnderResources, FArea area)
        {
            Assert.IsTrue(_companionImageSingleImageActionBehaviour == null);


            GameObject gameObjectCompanionImage;

            gameObjectCompanionImage = UtlSprite.CreateGameObjectWithSprite(
                name: "ButtonCompanionImage",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: _sortingOrder + 1,
                transformParent: transform);

            var textureSize = UtlSprite.GetTextureSize(gameObject: gameObjectCompanionImage);

            gameObjectCompanionImage.transform.localScale = area.GetPartialArea(ratio: textureSize).size / textureSize;
            gameObjectCompanionImage.transform.position = Utl.ConvertImageSizeToUnit(imageSize: area.CenterPoint);

            _companionImageSingleImageActionBehaviour = gameObjectCompanionImage.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(defaultAlpha: 1, durationFadeIn: 0.3f, durationFadeOut: 0.3f);

            _companionImageSingleImageActionBehaviour.SetupComponent(initializer: initializer);

            if (_isShown)
            {
                _companionImageSingleImageActionBehaviour.Show();
            }
            else
            {
                _companionImageSingleImageActionBehaviour.Hide();
            }

            ChangeCompanionImageAlphaAccordingToIsEnabled();
        }



        // ===============================================
        // MARK: - ● Debug ─────

        /// Debug: タップエリアを表示
        public void ShowTapArea()
        {
            _tapInputBehaviour.ShowArea();
        }


        /// Debug: タップエリアを非表示
        public void HideTapArea()
        {
            _tapInputBehaviour.HideArea();
        }



        // ===============================================
        // MARK: - ● Button ─────

        protected void ChangeButtonTexture(string texturePathUnderResources, bool isResizable)
        {
            if (isResizable)
            {
                UtlSprite.ChangeTextureForResizable(gameObject: _bodySingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
            }
            else
            {
                UtlSprite.ChangeTexture(gameObject: _bodySingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
            }
        }


        private void ChangeTextAlphaAccordingToIsEnabled()
        {
            if (_textFieldBehaviour == null) { return; }

            if (_isEnabled)
            {
                _textFieldBehaviour.SetDefaultAlpha(alpha: 1f);
            }
            else
            {
                _textFieldBehaviour.SetDefaultAlpha(alpha: 0.75f);
            }

            _textFieldBehaviour.ResetAlpha();
        }


        private void ChangeCompanionImageAlphaAccordingToIsEnabled()
        {
            if (_companionImageSingleImageActionBehaviour == null) { return; }

            if (_isEnabled)
            {
                _companionImageSingleImageActionBehaviour.SetDefaultAlpha(alpha: 1f);
            }
            else
            {
                _companionImageSingleImageActionBehaviour.SetDefaultAlpha(alpha: 0.75f);
            }

            _companionImageSingleImageActionBehaviour.ResetAlpha();
        }


        protected abstract void ChangeButtonTexture(EButtonStatusType buttonStatusType);


        private void ChangeButtonTextureAccordingToTapInputType(ETapInputType tapInputType)
        {
            if (_isEnabled == false) { return; }


            EButtonStatusType buttonStatusType = tapInputType switch
            {
                ETapInputType.POINTER_DOWN => EButtonStatusType.HOVER,
                ETapInputType.POINTER_UP => EButtonStatusType.DEFAULT,
                ETapInputType.POINTER_CLICK => EButtonStatusType.DEFAULT,
                ETapInputType.POINTER_ENTER => EButtonStatusType.DEFAULT,
                ETapInputType.POINTER_EXIT => EButtonStatusType.DEFAULT,
                _ => throw new InvalidOperationException(),
            };


            ChangeButtonTexture(buttonStatusType: buttonStatusType);
        }


        private void ChangeButtonTextureAccordingToIsEnabled()
        {
            if (_isEnabled)
            {
                ChangeButtonTexture(buttonStatusType: EButtonStatusType.DEFAULT);
            }
            else
            {
                ChangeButtonTexture(buttonStatusType: EButtonStatusType.DISABLED);
            }
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
        private void FireAction()
        {
            if (_isTapBlockedByAnimation == true) { return; }
            if (_isEnabled == false) { return; }

            PlaySE();

            if (_isActionEnabled)
            {
                _action?.Invoke();
            }
        }



        // ===============================================
        // MARK: - ● Play SE ─────

        private void PlaySE()
        {
            if (_isPlaySE == false) { return; }

            Locator.Instance.audioManager.Play(SEType: ESEType.TAP_NORMAL);
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
        // MARK: - ● Get / Set Text Color ─────

        public FColor3 GetTextColor()
        {
            return _textFieldBehaviour.GetColor();
        }


        public void SetTextColor(FColor3 color)
        {
            _textFieldBehaviour.SetColor(color: color);
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public void SetAlpha(float alpha)
        {
            // textComponent.color = new FColor4(color: GetColor(), a: alpha).AsSystemColor();

            UtlSprite.SetSpriteAlpha(gameObject: _bodySingleImageActionBehaviour.gameObject, alpha: alpha);
            _textFieldBehaviour?.SetAlpha(alpha: alpha);
            _companionImageSingleImageActionBehaviour?.SetAlpha(alpha: alpha);
        }


        public void SetColorAndAlpha(FColor3 color, float alpha)
        {
            // textComponent.color = new FColor4(color: color, a: alpha).AsSystemColor();

            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Eneble ─────

        public void SetIsEnabled(bool value, bool willChangeButtonTexture)
        {
            if (value == _isEnabled) { return; }

            _isEnabled = value;
            _tapInputBehaviour.SetIsEnabled(value: _isEnabled);

            if (willChangeButtonTexture)
            {
                ChangeButtonTextureAccordingToIsEnabled();
                ChangeTextAlphaAccordingToIsEnabled();
                ChangeCompanionImageAlphaAccordingToIsEnabled();
            }
        }



        // ===============================================
        // MARK: - ● Fade Duration ─────

        public void SetFadeDuration(float durationFadeIn, float durationFadeOut)
        {
            _bodySingleImageActionBehaviour.SetFadeDuration(durationFadeIn: durationFadeIn, durationFadeOut: durationFadeOut);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void Show()
        {
            if (_isShown) { return; }

            _isTapBlockedByAnimation = false;

            _bodySingleImageActionBehaviour.Show();
            _textFieldBehaviour?.Show();
            _companionImageSingleImageActionBehaviour?.Show();

            _tapInputBehaviour.SetIsEnabled(value: true);

            _isShown = true;
        }


        public void Hide()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            _bodySingleImageActionBehaviour.Hide();
            _textFieldBehaviour?.Hide();
            _companionImageSingleImageActionBehaviour?.Hide();

            _tapInputBehaviour.SetIsEnabled(value: false);

            _isShown = false;
        }


        public void FadeIn()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.FadeIn(actionOnDidFinish: () => {
                _isTapBlockedByAnimation = false; // _isTapBlockedByAnimation をフェード操作の後で解除する
                _tapInputBehaviour.SetIsEnabled(value: true);
            });
            _textFieldBehaviour?.FadeIn();
            _companionImageSingleImageActionBehaviour?.FadeIn();

            _isShown = true;
        }


        public void FadeOut()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            _bodySingleImageActionBehaviour.FadeOut();
            _textFieldBehaviour?.FadeOut();
            _companionImageSingleImageActionBehaviour?.FadeOut();

            _tapInputBehaviour.SetIsEnabled(value: false);

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
            Utl.SafeDestroy(behaviour: _textFieldBehaviour); // null チェックは遷移先で行っている
            Utl.SafeDestroy(behaviour: _companionImageSingleImageActionBehaviour);
        }


        private void ReleaseReference()
        {
            _textFieldBehaviour = null;
            _tapInputBehaviour = null;
            _companionImageSingleImageActionBehaviour = null;

            _action = null;
        }
    }
}

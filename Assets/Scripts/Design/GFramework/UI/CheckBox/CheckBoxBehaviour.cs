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
    // チェックボックス、ラジオボタン
    public class CheckBoxBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        // Window Behaviour
        private WindowBehaviour _windowBehaviour;

        // Base
        private SingleImageActionBehaviour _baseSingleImageActionBehaviour;
        public SingleImageActionBehaviour BaseSingleImageActionBehaviour => _baseSingleImageActionBehaviour;

        // Mark
        private SingleImageActionBehaviour _markSingleImageActionBehaviour;
        public SingleImageActionBehaviour MarkSingleImageActionBehaviour => _markSingleImageActionBehaviour;

        // Input
        private TapInputBehaviour _tapInputBehaviour;

        // Judge Type
        private ETapJudgeType _tapJudgeType = ETapJudgeType.DOWN;

        // Area
        private FArea _area;

        // Alpha
        public float Alpha => UtlSprite.GetSpriteAlpha(gameObject: _baseSingleImageActionBehaviour.gameObject);

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

        // On / Off
        private bool _isOn = false;
        public bool IsOn => _isOn;

        // has Error
        private bool _hasError = false;
        public bool HasError => _hasError;

        // Function Type
        private ECheckBoxFunctionType _functionType;

        // Highlighted 状態か
        private bool _isHighlighted = false;
        public bool IsHighlighted => _isHighlighted;

        // is Enabled (主に表示上の切り替えをするものであり、Tap の実行可否は別の変数で管理)
        private bool _isEnabled = true;
        public bool IsEnabled => _isEnabled;

        // is Shown
        private bool _isShown = true;
        public bool IsShown => _isShown;

        // is Play SE
        private bool _isPlaySE = true;
        public bool IsPlaySE => _isPlaySE;


        // ------------
        // Companion Text
        // - ペアとなるタップ可能なテキスト

        private TextFieldBehaviour _companionTextFieldBehaviour = null;

        private Canvas _targetCanvas = null;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(in FCheckBoxInitializer initializer, WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;

            _area = initializer.Area;
            _isOn = initializer.IsOn;
            _hasError = initializer.HasError;
            _isEnabled = initializer.IsEnabled;
            _isPlaySE = initializer.IsPlaySE;
            _action = initializer.Action;
            _functionType = initializer.FunctionType;

            _targetCanvas = initializer.TargetCanvas;

            SetupImage(sortingOrder: initializer.SortingOrder);
            SetupTapInput(targetCanvas: _targetCanvas);

            Hide();
        }


        private void SetupImage(int sortingOrder)
        {
            SetupBase(sortingOrder: sortingOrder);
            SetupMark(sortingOrder: sortingOrder + 1);

            transform.localScale = new Vector2(1f, 1f);
            transform.position = Utl.ConvertImageSizeToUnit(imageSize: _area.CenterPoint);
        }


        private void SetupBase(int sortingOrder)
        {
            var texturePathUnderResources = GetTexturePathUnderResourcesForBase();

            GameObject gameObject = UtlSprite.CreateGameObjectWithSprite(
                name: "CheckBoxBase",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform);

            var textureSize = UtlSprite.GetTextureSize(gameObject: gameObject);
            gameObject.transform.localScale = _area.size / textureSize;


            _baseSingleImageActionBehaviour = gameObject.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);

            _baseSingleImageActionBehaviour.SetupComponent(initializer: initializer);
        }


        private string GetTexturePathUnderResourcesForBase()
        {
            switch (_functionType)
            {
                case ECheckBoxFunctionType.CHECK_BOX:
                    return GetTexturePathUnderResourcesForCheckBoxBase();

                case ECheckBoxFunctionType.RADIO_BUTTON:
                    return GetTexturePathUnderResourcesForRadioButtonBase();

                default:
                    throw new NotImplementedException();
            };
        }


        private string GetTexturePathUnderResourcesForCheckBoxBase()
        {
            // チェックボックスが無効である場合、ハイライトもエラー表示も行わない
            // チェックボックスが有効である場合、ハイライト＞エラー＞初期表示 の順で表示する
            if (_isEnabled == false)
            {
               return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUICheckBox.BASE_DISABLED);
            }
            else
            {
                // case: _isEnabled = true
                if (_isHighlighted)
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUICheckBox.BASE_HIGHLIGHTED);
                }
                else if (_hasError)
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUICheckBox.BASE_ERROR);
                }
                else
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUICheckBox.BASE_DEFAULT);
                }
            }
        }


        private string GetTexturePathUnderResourcesForRadioButtonBase()
        {
            // チェックボックスが無効である場合、ハイライトもエラー表示も行わない
            // チェックボックスが有効である場合、ハイライト＞エラー＞初期表示 の順で表示する
            if (_isEnabled == false)
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUIRadioButton.BASE_DISABLED);
            }
            else
            {
                // case: _isEnabled = true
                if (_isHighlighted)
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUIRadioButton.BASE_HIGHLIGHTED);
                }
                else if (_hasError)
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUIRadioButton.BASE_ERROR);
                }
                else
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUIRadioButton.BASE_DEFAULT);
                }
            }
        }


        private void SetupMark(int sortingOrder)
        {
            var texturePathUnderResources = GetTexturePathUnderResourcesForMark();

            GameObject gameObject = UtlSprite.CreateGameObjectWithSprite(
                name: "CheckBoxMark",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform);

            var textureSize = UtlSprite.GetTextureSize(gameObject: gameObject);
            gameObject.transform.localScale = _area.size / textureSize;


            _markSingleImageActionBehaviour = gameObject.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);

            _markSingleImageActionBehaviour.SetupComponent(initializer: initializer);
        }


        private string GetTexturePathUnderResourcesForMark()
        {
            switch (_functionType)
            {
                case ECheckBoxFunctionType.CHECK_BOX:
                    return GetTexturePathUnderResourcesForCheckBoxMark();

                case ECheckBoxFunctionType.RADIO_BUTTON:
                    return GetTexturePathUnderResourcesForRadioButtonMark();

                default:
                    throw new NotImplementedException();
            };
        }


        private string GetTexturePathUnderResourcesForCheckBoxMark()
        {
            // ハイライト、エラー表示は行わない
            // チェックボックスが有効／無効、ON／OFF のみ表示を切り替える
            if (_isOn == false)
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUICheckBox.MARK_OFF);
            }
            else
            {
                // case: _isOn = true
                if (_isEnabled == false)
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUICheckBox.MARK_ON_DISABLED);
                }
                else
                {
                    // case: _isEnabled == true
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUICheckBox.MARK_ON_DEFAULT);
                }
            }
        }


        private string GetTexturePathUnderResourcesForRadioButtonMark()
        {
            // ハイライト、エラー表示は行わない
            // チェックボックスが有効／無効、ON／OFF のみ表示を切り替える
            if (_isOn == false)
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUIRadioButton.MARK_OFF);
            }
            else
            {
                // case: _isOn = true
                if (_isEnabled == false)
                {
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUIRadioButton.MARK_ON_DISABLED);
                }
                else
                {
                    // case: _isEnabled == true
                    return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUIRadioButton.MARK_ON_DEFAULT);
                }
            }
        }




        // タップ入力のセットアップ
        private void SetupTapInput(Canvas targetCanvas)
        {
            var gameObjectTapInput = new GameObject(name: "TapInputForCheckBox");

            _tapInputBehaviour = gameObjectTapInput.AddComponent<TapInputBehaviour>();

            gameObjectTapInput.transform.parent = targetCanvas.transform;

            Action<ETapInputType, int> basicAction = (ETapInputType tapInputType, int durationFrame) => { UpdateHighlightedState(tapInputType: tapInputType); };

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
                        action: (ETapInputType tapInputType, int durationFrame) => { UpdateHighlightedState(tapInputType: tapInputType); FireAction(); });
                    break;

                case ETapJudgeType.CLICK:
                    _tapInputBehaviour.SetActionOnPointerClick(
                        action: (ETapInputType tapInputType, int durationFrame) => { UpdateHighlightedState(tapInputType: tapInputType); FireAction(); });
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupCompanionTextField(FCompanionTextFieldSetting setting)
        {
            Assert.IsTrue(_companionTextFieldBehaviour == null);

            GameObject gameObjectText = new GameObject(name: name);
            _companionTextFieldBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            var area = _windowBehaviour.CalcUIPartsArea(windowElementArea: setting.WindowElementArea);

            var textInitializer = new FTextFieldInitializer(
                text: setting.Text,
                fontType: setting.FontType,
                fontSize: Locator.Instance.UICreatorManager.FontSize(fontScale: setting.FontScale),
                alignment: setting.Alignment,
                area: area.AsYReversed(),
                lineSpacing: Locator.Instance.UICreatorManager.LineSpacing,
                isOverFlowHorizontal: false,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: Utl.ConvertUIColorToColor3(textColorType: EWindowTextColorType.NORMAL),
                defaultAlpha: 1f,
                targetCanvas: _targetCanvas,
                tapJudgeType: ETapJudgeType.DOWN,
                action: () => { GenerateVirtualTap(); });

            _companionTextFieldBehaviour.SetupComponent(textInitializer: textInitializer);
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
        // MARK: - ● Set Is On ─────

        public void SetIsOn(bool value)
        {
            if (_isOn == value) { return; }

            _isOn = value;

            // ユーザ操作によるアニメーションがあれば、強制的に終了する
            ForceFinishActionIfNeeded();

            // マークのアニメーションを登録する
            RegisterMarkAnimationOnOff();
        }



        // ===============================================
        // MARK: - ● Set Has Error ─────

        public void SetHasError(bool value)
        {
            if (_hasError == value) { return; }

            _hasError = value;

            UpdateTextureForBase();
        }



        // ===============================================
        // MARK: - ● Texture ─────

        public void UpdateTextureForMark()
        {
            UtlSprite.ChangeTexture(
                gameObject: _markSingleImageActionBehaviour.gameObject,
                texturePathUnderResources: GetTexturePathUnderResourcesForMark(),
                willCacheIfNeeded: true);
        }


        private void UpdateTextureForBase()
        {
            UtlSprite.ChangeTexture(
                gameObject: _baseSingleImageActionBehaviour.gameObject,
                texturePathUnderResources: GetTexturePathUnderResourcesForBase(),
                willCacheIfNeeded: true);
        }


        private void UpdateHighlightedState(ETapInputType tapInputType)
        {
            if (_isEnabled == false) { return; }

            _isHighlighted = (tapInputType == ETapInputType.POINTER_DOWN);

            UpdateTextureForBase();
        }



        // ===============================================
        // MARK: - ● Play SE ─────

        private void PlaySE()
        {
            if (_isPlaySE == false) { return; }

            ESEType SEType = _isOn ? ESEType.CHECKBOX_ON : ESEType.CHECKBOX_OFF;

            Locator.Instance.audioManager.Play(SEType: SEType);
        }



        // ===============================================
        // MARK: - ● Action ─────

        // CompanionText がタップされた時に、チェックボックにもタップを与える
        // - この場合、TapInputType を取得できていないので、チェックボックスはハイライト表示されない
        private void GenerateVirtualTap()
        {
            FireAction();
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

            // ラジオボタンの場合、ONをタップしてもOFFにはできない
            if ((_functionType == ECheckBoxFunctionType.RADIO_BUTTON) && _isOn) { return; }


            _isOn = !_isOn;

            PlaySE();
            RegisterMarkAnimationOnOff();

            if (_isActionEnabled)
            {
                _action?.Invoke();
            }
        }


        private void RegisterMarkAnimationOnOff()
        {
            if (_isOn)
            {
                RegisterAction(action: CAction.CreateCheckBoxActionSelectOn(checkBoxBehaviour: this));
            }
            else
            {
                RegisterAction(action: CAction.CreateCheckBoxActionSelectOff(checkBoxBehaviour: this));
            }
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
            UtlSprite.SetSpriteAlpha(gameObject: _baseSingleImageActionBehaviour.gameObject, alpha: alpha);
            UtlSprite.SetSpriteAlpha(gameObject: _markSingleImageActionBehaviour.gameObject, alpha: alpha);
            _companionTextFieldBehaviour?.SetAlpha(alpha: alpha);
        }


        public void SetColorAndAlpha(FColor3 color, float alpha)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Eneble ─────

        public void SetIsEnabled(bool value, bool willChangeButtonTexture)
        {
            if (value == _isEnabled) { return; }

            _isEnabled = value;
            _tapInputBehaviour.SetIsEnabled(value: _isEnabled);

            UpdateTextureForBase();
            UpdateTextureForMark();

            // TODO: コンパニオンテキストの色を変更する
        }



        // ===============================================
        // MARK: - ● Fade Duration ─────

        public void SetFadeDuration(float durationFadeIn, float durationFadeOut)
        {
            _baseSingleImageActionBehaviour.SetFadeDuration(durationFadeIn: durationFadeIn, durationFadeOut: durationFadeOut);
            _markSingleImageActionBehaviour.SetFadeDuration(durationFadeIn: durationFadeIn, durationFadeOut: durationFadeOut);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void Show()
        {
            if (_isShown) { return; }

            _isTapBlockedByAnimation = false;

            _baseSingleImageActionBehaviour.Show();
            _markSingleImageActionBehaviour.Show();
            _companionTextFieldBehaviour?.Show();

            _isShown = true;
        }


        public void Hide()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            _baseSingleImageActionBehaviour.Hide();
            _markSingleImageActionBehaviour.Hide();
            _companionTextFieldBehaviour?.Hide();

            _isShown = false;
        }


        public void FadeIn()
        {
            if (_isShown) { return; }

            SetIsEnabled(value: true, willChangeButtonTexture: true);
            _baseSingleImageActionBehaviour.FadeIn(actionOnDidFinish: () => { _isTapBlockedByAnimation = false; }); // _isTapBlockedByAnimation をフェード操作の後で解除する
            _markSingleImageActionBehaviour.FadeIn();
            _companionTextFieldBehaviour?.FadeIn();

            _isShown = true;
        }


        public void FadeOut()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            SetIsEnabled(value: false, willChangeButtonTexture: false);
            _baseSingleImageActionBehaviour.FadeOut();
            _markSingleImageActionBehaviour.FadeOut();
            _companionTextFieldBehaviour?.FadeOut();

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
            Utl.SafeDestroy(behaviour: _companionTextFieldBehaviour);
        }


        private void ReleaseReference()
        {
            _windowBehaviour = null;

            _baseSingleImageActionBehaviour = null;
            _markSingleImageActionBehaviour = null;
            _tapInputBehaviour = null;

            _companionTextFieldBehaviour = null;
            _targetCanvas = null;

            _action = null;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.Networking.UnityWebRequest;


// TODO: 後からWindownに要素を足すと、非表示状態になってしまう点を直す

namespace GFramework
{
    public class WindowBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private EWindowType _windowType;
        public EWindowType WindowType => _windowType;

        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _windowBodySingleImageActionBehaviour;

        private Canvas _canvas;

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _windowBodySingleImageActionBehaviour.Alpha;

        private GameObject _gameObjectCurtain = null;
        private CurtainBehaviour _curtainBehaviour = null;

        private TapInputBehaviour _tapInputBlocker = null;

        private bool _isActionEnabled = true;
        public bool IsActionEnabled => _isActionEnabled;

        private readonly List<TextFieldBehaviour> _textFieldBehaviourList = new List<TextFieldBehaviour>();
        private readonly List<ButtonBehaviour> _buttonBehaviourList = new List<ButtonBehaviour>();
        private readonly List<ImageBoxBehaviour> _imageBoxBehaviourList = new List<ImageBoxBehaviour>();
        private readonly List<InputFieldBehaviour> _inputFieldBehaviourList = new List<InputFieldBehaviour>();
        private readonly List<CheckBoxBehaviour> _checkBoxBehaviourList = new List<CheckBoxBehaviour>();
        private readonly List<RadioButtonGroupBehaviour> _radioButtonGroupBehaviourList = new List<RadioButtonGroupBehaviour>();
        private readonly List<SliderBehaviour> _sliderBehaviourList = new List<SliderBehaviour>();


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(EWindowType windowType)
        {
            _windowType = windowType;

            SetupGameObject();

            SetupCanvas();

            SetupTapInputBlocker();

            SetupCurtain();

            // 初期状態は非表示(パーツを破壊しない別処理)
            HideWithoutDestroyParts();
        }


        private void SetupGameObject()
        {
            SetupBody();
            SetupBaseImageSize();

            SetLocalScale();
        }


        private void SetupBody()
        {
            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_BODY__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_BODY__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_BODY__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_BODY__LAYERED_ORDERID.SortingOrder,                
                _ => throw new NotImplementedException(),
            };


            _windowBodySingleImageActionBehaviour = UtlSprite.CreateGameObjectWithResizableSpriteAndSingleImageBehaviour(
                name: "WindowBody",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(windowType: _windowType),
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform,
                border: new Vector4(x: 32f, y: 32f, z: 32f, w: 32f),
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 1,
                    durationFadeOut: 1)
                );


            var spriteRenderer = _windowBodySingleImageActionBehaviour.gameObject.GetComponent<SpriteRenderer>();

            FArea windowArea = Locator.Instance.UICreatorManager.Layouter.GetAreaWindowBase(windowType: _windowType);
            spriteRenderer.size = Utl.ConvertImageSizeToUnit(imageSize: windowArea.size);


            _windowBodySingleImageActionBehaviour.transform.localScale = new Vector2(1f, 1f);

            transform.position = Utl.ConvertImageSizeToUnit(imageSize: windowArea.CenterPoint);
        }


        private void SetupBaseImageSize()
        {
            // TODO: ここを実装する
            // baseImageSize = Utl.ConvertImageSizeToUnit(imageSize);
        }

        private void SetupCanvas()
        {
            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_TEXT__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_TEXT__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_TEXT__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_TEXT__LAYERED_ORDERID.SortingOrder,
                _ => throw new InvalidOperationException(),
            };

            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: sortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupTapInputBlocker()
        {
            _tapInputBlocker = Utl.CreateTapInputFullScreen(
                canvas: _canvas,
                isEnabled: false,
                action: (ETapInputType tapInputType, int durationFrame) => { });
        }


        private void SetupCurtain()
        {
            _gameObjectCurtain = new GameObject("Curtain");
            _gameObjectCurtain.transform.parent = gameObject.transform;

            CurtainBehaviour.ECurtainType curtainType = _windowType switch
            {
                EWindowType.TOP => CurtainBehaviour.ECurtainType.WINDOW_TOP,
                EWindowType.NORMAL => CurtainBehaviour.ECurtainType.WINDOW_NORMAL,
                EWindowType.CONFIRMATION => CurtainBehaviour.ECurtainType.WINDOW_CONFIRMATION,
                EWindowType.ERROR => CurtainBehaviour.ECurtainType.WINDOW_ERROR,
                _ => throw new InvalidOperationException(),
            };

            _curtainBehaviour = _gameObjectCurtain.AddComponent<CurtainBehaviour>();
            _curtainBehaviour.SetupComponent(curtainType: curtainType);
        }


        // ===============================================
        // MARK: - ● Debug Print ─────

        public void DebugPrint()
        {
            var result = "WindowBehaviour: DebugPrint()\n";


            result += $"text: {_textFieldBehaviourList.Count} ";

            if (_textFieldBehaviourList.Count > 0)
            {
                result += "(";
                _textFieldBehaviourList.ForEach(i => result += $"{i.name} ");
                result += ")";
            }


            result += $"button: {_buttonBehaviourList.Count} ";

            if (_buttonBehaviourList.Count > 0)
            {
                result += "(";
                _buttonBehaviourList.ForEach(i => result += $"{i.name} ");
                result += ")";
            }

            result += $"image box: {_imageBoxBehaviourList.Count} ";

            if (_imageBoxBehaviourList.Count > 0)
            {
                result += "(";
                _imageBoxBehaviourList.ForEach(i => result += $"{i.name} ");
                result += ")";
            }


            result += $"input form: {_inputFieldBehaviourList.Count} ";

            if (_inputFieldBehaviourList.Count > 0)
            {
                result += "(";
                _inputFieldBehaviourList.ForEach(i => result += $"{i.name} ");
                result += ")";
            }


            result += $"check box: {_checkBoxBehaviourList.Count} ";

            if (_checkBoxBehaviourList.Count > 0)
            {
                result += "(";
                _checkBoxBehaviourList.ForEach(i => result += $"{i.name} ");
                result += ")";
            }


            result += $"radio button group: {_radioButtonGroupBehaviourList.Count} ";

            if (_radioButtonGroupBehaviourList.Count > 0)
            {
                result += "(";
                _radioButtonGroupBehaviourList.ForEach(i => result += $"{i.name} ");
                result += ")";
            }


            result += $"slider: {_sliderBehaviourList.Count} ";

            if (_sliderBehaviourList.Count > 0)
            {
                result += "(";
                _sliderBehaviourList.ForEach(i => result += $"{i.name} ");
                result += ")";
            }



            Debug.Log(result);
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositon(Vector2 unit)
        {
            Utl.SetLocalPosition(gameObject: _windowBodySingleImageActionBehaviour.gameObject, unit: unit);
        }


        public void SetLocalPositonByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: _windowBodySingleImageActionBehaviour.gameObject, baseImageSize: _baseImageSize, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            transform.localScale = new Vector2(x: 1f, y: 1f);
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _windowBodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }


        public void SetAlphaZeroOfBodyAndShadow()
        {
            SetAlpha(alpha: 0);
        }




        // ===============================================
        // MARK: - ● HideWithoutDestroyParts ─────

        public void HideWithoutDestroyParts()
        {
            if (_isShown == false) { return; };

            _windowBodySingleImageActionBehaviour.Hide();
            _curtainBehaviour.Hide();
            HideText();
            HideButton();
            HideInputField();

            _isShown = false;
        }


        // ===============================================
        // MARK: - ● Action Enability ─────

        public void SetIsActionEnabled(bool value)
        {
            if (_isActionEnabled == value) { return; }

            _isActionEnabled = value;

            _textFieldBehaviourList.ForEach(i => i.SetIsActionEnabled(value: _isActionEnabled));
            _buttonBehaviourList.ForEach(i => i.SetIsActionEnabled(value: _isActionEnabled));
            _imageBoxBehaviourList.ForEach(i => i.SetIsActionEnabled(value: _isActionEnabled));
            _inputFieldBehaviourList.ForEach(i => i.SetIsActionEnabled(value: _isActionEnabled));
            _checkBoxBehaviourList.ForEach(i => i.SetIsActionEnabled(value: _isActionEnabled));
            _radioButtonGroupBehaviourList.ForEach(i => i.SetIsActionEnabled(value: _isActionEnabled));
            _sliderBehaviourList.ForEach(i => i.SetIsActionEnabled(value: _isActionEnabled));
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; };

            _tapInputBlocker.SetIsEnabled(value: true);
            _windowBodySingleImageActionBehaviour.Show();
            ShowText();
            ShowButton();
            ShowImageBox();
            ShowInputField();
            ShowCheckBox();
            ShowRaidoBoxGroup();
            ShowSlider();

            _curtainBehaviour.Show();

            _isShown = true;
        }

        private void ShowText()
        {
            _textFieldBehaviourList.ForEach(i => i.Show());
        }

        private void ShowButton()
        {
            _buttonBehaviourList.ForEach(i => i.Show());
        }

        private void ShowImageBox()
        {
            _imageBoxBehaviourList.ForEach(i => i.Show());
        }

        private void ShowInputField()
        {
            _inputFieldBehaviourList.ForEach(i => i.Show());
        }

        private void ShowCheckBox()
        {
            _checkBoxBehaviourList.ForEach(i => i.Show());
        }

        private void ShowRaidoBoxGroup()
        {
            _radioButtonGroupBehaviourList.ForEach(i => i.Show());
        }

        private void ShowSlider()
        {
            _sliderBehaviourList.ForEach(i => i.Show());
        }



        public override void Hide()
        {
            if (_isShown == false) { return; };

            _tapInputBlocker.SetIsEnabled(value: false);
            _windowBodySingleImageActionBehaviour.Hide();
            _curtainBehaviour.Hide();
            HideText();
            HideButton();
            HideImageBox();
            HideInputField();
            HideCheckBox();
            HideRadioBoxGroup();
            HideSlider();

            DestroyAllWindowElement();

            _isShown = false;
        }


        private void HideText()
        {
            _textFieldBehaviourList.ForEach(i => i.Hide());
        }

        private void HideButton()
        {
            _buttonBehaviourList.ForEach(i => i.Hide());
        }

        private void HideImageBox()
        {
            _imageBoxBehaviourList.ForEach(i => i.Hide());
        }

        private void HideInputField()
        {
            _inputFieldBehaviourList.ForEach(i => i.Hide());
        }

        private void HideCheckBox()
        {
            _checkBoxBehaviourList.ForEach(i => i.Hide());
        }

        private void HideRadioBoxGroup()
        {
            _radioButtonGroupBehaviourList.ForEach(i => i.Hide());
        }

        private void HideSlider()
        {
            _sliderBehaviourList.ForEach(i => i.Hide());
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            throw new NotImplementedException();
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            throw new NotImplementedException();
        }


        private void DestroyAllWindowElement()
        {
            Utl.SafeDestroy(behaviourList: _textFieldBehaviourList);
            _textFieldBehaviourList.Clear();

            Utl.SafeDestroy(behaviourList: _buttonBehaviourList);
            _buttonBehaviourList.Clear();

            Utl.SafeDestroy(behaviourList: _imageBoxBehaviourList);
            _imageBoxBehaviourList.Clear();

            Utl.SafeDestroy(behaviourList: _inputFieldBehaviourList);
            _inputFieldBehaviourList.Clear();

            Utl.SafeDestroy(behaviourList: _checkBoxBehaviourList);
            _checkBoxBehaviourList.Clear();

            Utl.SafeDestroy(behaviourList: _radioButtonGroupBehaviourList);
            _radioButtonGroupBehaviourList.Clear();

            Utl.SafeDestroy(behaviourList: _sliderBehaviourList);
            _sliderBehaviourList.Clear();
        }



        #region Free UI Creator

        // ===============================================
        // MARK: - ● Free UI Creator ─────

        #region all


        public void RemoveAllUIParts()
        {
            DestroyAllWindowElement();
        }

        #endregion


        #region Text

        // name で特定の textBehavirour を取得
        private TextFieldBehaviour GetTextFieldBehaviour(string name)
        {
            TextFieldBehaviour result = null;

            foreach (var i in _textFieldBehaviourList)
            {
                if (i.name == name) { result = i; }
            }

            Assert.IsTrue(result != null);

            return result;
        }


        // 特定の textBehavirour の存在を確認
        private bool HasTextFieldBehaviour(string name)
        {
            bool result = false;

            foreach (var i in _textFieldBehaviourList)
            {
                if (i.name == name) { result = true; }
            }

            return result;
        }


        // window のメインエリアからUIパーツのエリアを求める
        public FArea CalcUIPartsArea(in FWindowElementArea windowElementArea)
        {
            FArea windowMainArea = Locator.Instance.UICreatorManager.Layouter.GetAreaWindowMain(windowType: _windowType).AsYReversed();

            switch (_windowType)
            {
                case EWindowType.TOP:
                    return new FArea(
                    origin: new Vector2(
                            windowMainArea.origin.x + windowMainArea.size.x * windowElementArea.Origin.x / Config.WindowTopSizeRatio.x,
                            windowMainArea.origin.y + windowMainArea.size.y * windowElementArea.Origin.y / Config.WindowTopSizeRatio.y),
                        size: new Vector2(
                            windowMainArea.size.x * windowElementArea.Size.x / Config.WindowTopSizeRatio.x,
                            windowMainArea.size.y * windowElementArea.Size.y / Config.WindowTopSizeRatio.y));

                case EWindowType.CONFIRMATION:
                    return new FArea(
                    origin: new Vector2(
                            windowMainArea.origin.x + windowMainArea.size.x * windowElementArea.Origin.x / Config.WindowConfirmationSizeRatio.x,
                            windowMainArea.origin.y + windowMainArea.size.y * windowElementArea.Origin.y / Config.WindowConfirmationSizeRatio.y),
                        size: new Vector2(
                            windowMainArea.size.x * windowElementArea.Size.x / Config.WindowConfirmationSizeRatio.x,
                            windowMainArea.size.y * windowElementArea.Size.y / Config.WindowConfirmationSizeRatio.y));

                case EWindowType.NORMAL:
                case EWindowType.ERROR:
                    return new FArea(
                        origin: new Vector2(
                            windowMainArea.origin.x + windowMainArea.size.x * windowElementArea.Origin.x,
                            windowMainArea.origin.y + windowMainArea.size.y * windowElementArea.Origin.y / 1.6f),
                        size: new Vector2(
                            windowMainArea.size.x * windowElementArea.Size.x,
                            windowMainArea.size.y * windowElementArea.Size.y / 1.6f));

                default:
                    throw new InvalidOperationException();
            }
        }


        // textFieldBehaviour を追加
        public void AddTextFieldBehaviour(
            string name,
            FWindowElementArea windowElementArea,
            string text,
            EFontType fontType,
            float fontScale,
            EWindowTextColorType textColorType,
            EAlignment alignment,
            Action actionOnTap)
        {
            Assert.IsTrue(HasTextFieldBehaviour(name) == false, "同じ名前のUIパーツが、すでに存在しています");


            GameObject gameObjectText = new GameObject(name: name);
            var textFieldBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            var area = CalcUIPartsArea(windowElementArea: windowElementArea);

            var textInitializer = new FTextFieldInitializer(
                text: text,
                fontType: fontType,
                fontSize: Locator.Instance.UICreatorManager.FontSize(fontScale: fontScale),
                alignment: alignment,
                area: area.AsYReversed(),
                lineSpacing: Locator.Instance.UICreatorManager.LineSpacing,
                isOverFlowHorizontal: false,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: Utl.ConvertUIColorToColor3(textColorType: textColorType),
                defaultAlpha: 1f,
                targetCanvas: _canvas,
                tapJudgeType: ETapJudgeType.DOWN,
                action: actionOnTap);

            textFieldBehaviour.SetupComponent(textInitializer: textInitializer);


            _textFieldBehaviourList.Add(textFieldBehaviour);


            if (_isShown)
            {
                textFieldBehaviour.Show();
            }
        }

        public void RemoveTextFieldBehaviour(string name)
        {
            Assert.IsTrue(HasTextFieldBehaviour(name: name));

            var textFieldBehaviour = GetTextFieldBehaviour(name: name);
            _textFieldBehaviourList.Remove(textFieldBehaviour);

            Utl.SafeDestroy(behaviour: textFieldBehaviour);
        }


        public void ChangeText(string name, string text)
        {
            Assert.IsTrue(HasTextFieldBehaviour(name: name));

            GetTextFieldBehaviour(name: name).SetText(text: text);
        }


        public void ChangeTextColor(string name, EWindowTextColorType textColorType)
        {
            Assert.IsTrue(HasTextFieldBehaviour(name: name));


            var color3 = Utl.ConvertUIColorToColor3(textColorType: textColorType);
            GetTextFieldBehaviour(name: name).SetColor(color: color3);
        }

        #endregion

        #region Button

        // name で特定の buttonBehavirour を取得
        private ButtonBehaviour GetButtonBehaviour(string name)
        {
            ButtonBehaviour result = null;

            foreach (var i in _buttonBehaviourList)
            {
                if (i.name == name) { result = i; }
            }

            Assert.IsTrue(result != null);

            return result;
        }


        // 特定の buttonBehavirour の存在を確認
        private bool HasButtonBehaviour(string name)
        {
            bool result = false;

            foreach (var i in _buttonBehaviourList)
            {
                if (i.name == name) { result = true; }
            }

            return result;
        }


        public void AddButton(
            string name,
            in FWindowElementArea windowElementArea,
            EUIButtonDesignType buttonDesignType,
            string text,
            bool isPlaySE,
            bool isEnabled,
            Action actionOnTap)
        {
            Assert.IsTrue(HasButtonBehaviour(name) == false, "同じ名前のUIパーツが、すでに存在しています");


            GameObject gameObjectButton = new GameObject(name: name);
            var buttonBehaviour = gameObjectButton.AddComponent<ButtonBehaviour>();

            gameObjectButton.transform.parent = gameObject.transform;


            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_BUTTON__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_BUTTON__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_BUTTON__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_BUTTON__LAYERED_ORDERID.SortingOrder,
                _ => throw new NotImplementedException(),
            };


            var area = CalcUIPartsArea(windowElementArea: windowElementArea);

            EButtonImageType buttonImageType = buttonDesignType switch
            {
                EUIButtonDesignType.DEFAULT => EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                EUIButtonDesignType.POSITIVE => EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                EUIButtonDesignType.WARNING => EButtonImageType.BUTTON_RESIZABLE_WARNING,
                EUIButtonDesignType.WINDOW_CLOSE => EButtonImageType.BUTTON_UNRESIZABLE_WINDOW_CLOSE,
                _ => throw new NotImplementedException(),
            };


            var buttonInitializer = new FButtonInitializer(
                targetCanvas: _canvas,
                buttonImageType: buttonImageType,
                area: area.AsYReversed(),
                tapJudgeType: ETapJudgeType.CLICK,
                isPlaySE: isPlaySE,
                isEnabled: isEnabled,
                sortingOrder: sortingOrder,
                action: actionOnTap);

            buttonBehaviour.SetupComponent(initializer: buttonInitializer);

            var buttonTextInitializer = new FButtonTextInitializer(
                text: text,
                fontType: EFontType.SERIF,
                fontSize: (int)(area.size.y * 0.5f),
                alignment: EAlignment.MIDDLE_CENTER);
            buttonBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);


            _buttonBehaviourList.Add(buttonBehaviour);


            if (_isShown)
            {
                buttonBehaviour.Show();
            }
        }

        public void SetButtonCompanionImage(string name, in FWindowElementArea windowElementArea, string filePath)
        {
            Assert.IsTrue(HasButtonBehaviour(name: name));

            var area = CalcUIPartsArea(windowElementArea: windowElementArea);

            // ボタン本体と同じく Y 反転した座標で渡す
            GetButtonBehaviour(name: name).SetupCompanionImage(texturePathUnderResources: filePath, area: area.AsYReversed());
        }

        public void RemoveButton(string name)
        {
            Assert.IsTrue(HasButtonBehaviour(name: name));

            var buttonBehaviour = GetButtonBehaviour(name: name);
            _buttonBehaviourList.Remove(buttonBehaviour);

            Utl.SafeDestroy(behaviour: buttonBehaviour);
        }



        public bool IsButtonEnabled(string name)
        {
            Assert.IsTrue(HasButtonBehaviour(name: name));

            return GetButtonBehaviour(name: name).IsEnabled;
        }


        public void SetButtonIsEnabled(string name, bool value)
        {
            Assert.IsTrue(HasButtonBehaviour(name: name));

            GetButtonBehaviour(name: name).SetIsEnabled(value: value, willChangeButtonTexture: true);
        }

        #endregion

        #region Image Box

        // name で特定の imageBoxBehavirour を取得
        private ImageBoxBehaviour GetImageBoxBehaviour(string name)
        {
            ImageBoxBehaviour result = null;

            foreach (var i in _imageBoxBehaviourList)
            {
                if (i.name == name) { result = i; }
            }

            Assert.IsTrue(result != null);

            return result;
        }


        // 特定の imageBoxBehavirour の存在を確認
        private bool HasImageBoxBehaviour(string name)
        {
            bool result = false;

            foreach (var i in _imageBoxBehaviourList)
            {
                if (i.name == name) { result = true; }
            }

            return result;
        }


        public void AddImageBox(
            string name,
            in FWindowElementArea windowElementArea,
            string filePath,
            Action actionOnTap)
        {
            Assert.IsTrue(HasImageBoxBehaviour(name) == false, "同じ名前のUIパーツが、すでに存在しています");


            GameObject gameObjectImageBox = new GameObject(name: name);
            var imageBoxBehaviour = gameObjectImageBox.AddComponent<ImageBoxBehaviour>();

            gameObjectImageBox.transform.parent = gameObject.transform;


            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_IMAGE_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_IMAGE_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_IMAGE_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_IMAGE_BOX__LAYERED_ORDERID.SortingOrder,
                _ => throw new NotImplementedException(),
            };

            var area = CalcUIPartsArea(windowElementArea: windowElementArea);


            var imageBoxInitializer = new FImageBoxInitializer(
                targetCanvas: _canvas,
                area: area.AsYReversed(),
                texturePathUnderResources: filePath,
                sortingOrder: sortingOrder,
                action: actionOnTap);


            imageBoxBehaviour.SetupComponent(initializer: imageBoxInitializer);


            _imageBoxBehaviourList.Add(imageBoxBehaviour);


            if (_isShown)
            {
                imageBoxBehaviour.Show();
            }
        }

        public void RemoveImageBox(string name)
        {
            Assert.IsTrue(HasImageBoxBehaviour(name: name));

            var imageBoxBehaviour = GetImageBoxBehaviour(name: name);
            _imageBoxBehaviourList.Remove(imageBoxBehaviour);

            Utl.SafeDestroy(behaviour: imageBoxBehaviour);
        }

        public void ChangeImageBoxImage(string name, string filePath)
        {
            Assert.IsTrue(HasImageBoxBehaviour(name: name));

            GetImageBoxBehaviour(name: name).ChangeImage(texturePathUnderResources: filePath);
        }

        #endregion

        #region InputForm

        // name で特定の inputFieldBehavirour を取得
        private InputFieldBehaviour GetInputFieldBehaviour(string name)
        {
            InputFieldBehaviour result = null;

            foreach (var i in _inputFieldBehaviourList)
            {
                if (i.name == name) { result = i; }
            }

            Assert.IsTrue(result != null);

            return result;
        }


        // 特定の inputFieldBehavirour の存在を確認
        private bool HasInputFieldBehaviour(string name)
        {
            bool result = false;

            foreach (var i in _inputFieldBehaviourList)
            {
                if (i.name == name) { result = true; }
            }

            return result;
        }


        public void AddInputField(
            string name,
            in FWindowElementArea windowElementArea,
            string explanationText,
            string text,
            int maxDisplayTextLength,
            bool isPasswordMode,
            bool hasError,
            bool isPlaySE,
            bool isEnabled,
            Action actionOnTap)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name) == false, "同じ名前のUIパーツが、すでに存在しています");


            GameObject gameObjectButton = new GameObject(name: name);
            var inputFieldBehaviour = gameObjectButton.AddComponent<InputFieldBehaviour>();

            gameObjectButton.transform.parent = gameObject.transform;


            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_INPUT_FIELD__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_INPUT_FIELD__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_INPUT_FIELD__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_INPUT_FIELD__LAYERED_ORDERID.SortingOrder,
                _ => throw new NotImplementedException(),
            };


            var area = CalcUIPartsArea(windowElementArea: windowElementArea);

            EInputFieldType inputFieldType = isPasswordMode ? EInputFieldType.PASSWORD : EInputFieldType.NORMAL;


            var initializer = new FInputFieldInitializer(
                targetCanvas: _canvas,
                area: area.AsYReversed(),
                explanationText: explanationText,
                text: text,
                maxDisplayTextLength: maxDisplayTextLength,
                inputFieldType: inputFieldType,
                hasError: hasError,
                isPlaySE: isPlaySE,
                isEnabled: isEnabled,
                sortingOrder: sortingOrder,
                action: actionOnTap);

            inputFieldBehaviour.SetupComponent(initializer: initializer);


            _inputFieldBehaviourList.Add(inputFieldBehaviour);


            if (_isShown)
            {
                inputFieldBehaviour.Show();
            }
        }


        public void RemoveInputField(string name)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name: name));

            var inputFieldBehaviour = GetInputFieldBehaviour(name: name);
            _inputFieldBehaviourList.Remove(inputFieldBehaviour);

            Utl.SafeDestroy(behaviour: inputFieldBehaviour);
        }


        public bool IsInputFieldEnabled(string name)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name: name));

            return GetInputFieldBehaviour(name: name).IsEnabled;
        }


        public void SetInputFieldIsEnabled(string name, bool value)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name: name));

            GetInputFieldBehaviour(name: name).SetIsEnabled(value: value, willChangeButtonTexture: true);
        }


        public void SetInputFieldText(string name, string text)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name: name));

            GetInputFieldBehaviour(name: name).SetText(text: text);
        }


        public string GetInputFieldText(string name)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name: name));

            return GetInputFieldBehaviour(name: name).Text;
        }


        public bool HasInputFieldError(string name)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name: name));

            return GetInputFieldBehaviour(name: name).HasError;
        }

        public void SetInputFieldHasError(string name, bool value)
        {
            Assert.IsTrue(HasInputFieldBehaviour(name: name));

            GetInputFieldBehaviour(name: name).SetHasError(value: value);
        }



        #endregion

        #region CheckBox

        // name で特定の checkBoxBehavirour を取得
        private CheckBoxBehaviour GetCheckBoxBehaviour(string name)
        {
            CheckBoxBehaviour result = null;

            foreach (var i in _checkBoxBehaviourList)
            {
                if (i.name == name) { result = i; }
            }

            Assert.IsTrue(result != null);

            return result;
        }


        // 特定の checkBoxBehavirour の存在を確認
        private bool HasCheckBoxBehaviour(string name)
        {
            bool result = false;

            foreach (var i in _checkBoxBehaviourList)
            {
                if (i.name == name) { result = true; }
            }

            return result;
        }


        public void AddCheckBox(
            string name,
            in FWindowElementArea windowElementArea,
            bool isOn,
            bool hasError,
            bool isEnabled,
            Action actionOnTap,
            FCompanionTextFieldSetting companionTextFieldSetting)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name) == false, "同じ名前のUIパーツが、すでに存在しています");


            GameObject gameObjectCheckBox = new GameObject(name: name);
            var checkBoxBehaviour = gameObjectCheckBox.AddComponent<CheckBoxBehaviour>();

            gameObjectCheckBox.transform.parent = gameObject.transform;


            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                _ => throw new NotImplementedException(),
            };


            var area = CalcUIPartsArea(windowElementArea: windowElementArea);

            var initializer = new FCheckBoxInitializer(
                targetCanvas: _canvas,
                area: area.AsYReversed(),
                isOn: isOn,
                hasError: hasError,
                isEnabled: isEnabled,
                functionType: ECheckBoxFunctionType.CHECK_BOX,
                sortingOrder: sortingOrder,
                action: actionOnTap,
                isPlaySE: true);

            checkBoxBehaviour.SetupComponent(initializer: initializer, windowBehaviour: this);

            if (companionTextFieldSetting != null)
            {
                checkBoxBehaviour.SetupCompanionTextField(setting: companionTextFieldSetting);
            }

            _checkBoxBehaviourList.Add(checkBoxBehaviour);


            if (_isShown)
            {
                checkBoxBehaviour.Show();
            }
        }

        public void RemoveCheckBox(string name)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name: name));

            var checkBoxBehaviour = GetCheckBoxBehaviour(name: name);
            _checkBoxBehaviourList.Remove(checkBoxBehaviour);

            Utl.SafeDestroy(behaviour: checkBoxBehaviour);
        }


        public bool IsCheckBoxEnabled(string name)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name: name));

            return GetCheckBoxBehaviour(name: name).IsEnabled;
        }


        public void SetCheckBoxIsEnabled(string name, bool value)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name: name));

            GetCheckBoxBehaviour(name: name).SetIsEnabled(value: value, willChangeButtonTexture: true);
        }


        public bool IsCheckBoxOn(string name)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name: name));

            return GetCheckBoxBehaviour(name: name).IsOn;
        }

        public void SetCheckBoxIsOn(string name, bool value)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name: name));

            GetCheckBoxBehaviour(name: name).SetIsOn(value: value);
        }


        public bool HasCheckBoxError(string name)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name: name));

            return GetCheckBoxBehaviour(name: name).HasError;
        }

        public void SetCheckBoxHasError(string name, bool value)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name: name));

            GetCheckBoxBehaviour(name: name).SetHasError(value: value);
        }

        #endregion


        #region RadioButtonGroup

        // name で特定の radioBoxGroupBehavirour を取得
        private RadioButtonGroupBehaviour GetRadioButtonGroupBehaviour(string name)
        {
            RadioButtonGroupBehaviour result = null;

            foreach (var i in _radioButtonGroupBehaviourList)
            {
                if (i.name == name) { result = i; }
            }

            Assert.IsTrue(result != null);

            return result;
        }


        // 特定の radioButtonGroupBehavirour の存在を確認
        private bool HasRadioButtonGroupBehaviour(string name)
        {
            bool result = false;

            foreach (var i in _radioButtonGroupBehaviourList)
            {
                if (i.name == name) { result = true; }
            }

            return result;
        }

        public void AddRadioButtonGroup(
            string name,
            in List<FWindowElementArea> windowElementAreaList,
            in List<FCompanionTextFieldSetting> companionTextFieldSettingList,
            int indexOfOn,
            bool isEnabled,
            Action actionOnChange)
        {
            Assert.IsTrue(HasCheckBoxBehaviour(name) == false, "同じ名前のUIパーツが、すでに存在しています");


            GameObject gameObjectRadioButtonGroup = new GameObject(name: name);
            var radioButtonGroupBehaviour = gameObjectRadioButtonGroup.AddComponent<RadioButtonGroupBehaviour>();

            gameObjectRadioButtonGroup.transform.parent = gameObject.transform;


            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_CHECK_BOX__LAYERED_ORDERID.SortingOrder,
                _ => throw new NotImplementedException(),
            };


            var initializer = new FRadioButtonGroupInitializer(
                targetCanvas: _canvas,
                areaList: windowElementAreaList,
                companionTextFieldSettingList: companionTextFieldSettingList,
                indexOfOn: indexOfOn,
                isEnabled: isEnabled,
                sortingOrder: sortingOrder,
                action: actionOnChange,
                isPlaySE: true);

            radioButtonGroupBehaviour.SetupComponent(initializer: initializer, windowBehaviour: this);


            _radioButtonGroupBehaviourList.Add(radioButtonGroupBehaviour);


            if (_isShown)
            {
                radioButtonGroupBehaviour.Show();
            }
        }


        public void RemoveRadioButtonGroup(string name)
        {
            Assert.IsTrue(HasRadioButtonGroupBehaviour(name: name));

            var radioButtonGroupBehaviour = GetRadioButtonGroupBehaviour(name: name);
            _radioButtonGroupBehaviourList.Remove(radioButtonGroupBehaviour);

            Utl.SafeDestroy(behaviour: radioButtonGroupBehaviour);
        }


        public bool IsRadioButtonGroupEnabled(string name)
        {
            Assert.IsTrue(HasRadioButtonGroupBehaviour(name: name));

            return GetRadioButtonGroupBehaviour(name: name).IsEnabled;
        }


        public void SetRadioButtonGroupIsEnabled(string name, bool value)
        {
            Assert.IsTrue(HasRadioButtonGroupBehaviour(name: name));

            GetRadioButtonGroupBehaviour(name: name).SetIsEnabled(value: value);
        }


        public int GetRadioButtonGroupIndexOfOn(string name)
        {
            Assert.IsTrue(HasRadioButtonGroupBehaviour(name: name));

            return GetRadioButtonGroupBehaviour(name: name).IndexOfOn;
        }

        public void SetRadioButtonGroupIndexOfOn(string name, int indexOfOn)
        {
            Assert.IsTrue(HasRadioButtonGroupBehaviour(name: name));

            GetRadioButtonGroupBehaviour(name: name).SetIndexOfOn(indexOfOn: indexOfOn);
        }

        #endregion


        #region Slider

        // name で特定の sliderBehavirour を取得
        private SliderBehaviour GetSliderBehaviour(string name)
        {
            SliderBehaviour result = null;

            foreach (var i in _sliderBehaviourList)
            {
                if (i.name == name) { result = i; }
            }

            Assert.IsTrue(result != null);

            return result;
        }


        // 特定の sliderBehavirour の存在を確認
        private bool HasSliderBehaviour(string name)
        {
            bool result = false;

            foreach (var i in _sliderBehaviourList)
            {
                if (i.name == name) { result = true; }
            }

            return result;
        }


        public void AddSlider(
            string name,
            in FWindowElementArea windowElementArea,
            int maxStep,
            int presentStep,
            bool isPlaySE,
            bool isEnabled,
            Action actionOnChangeStep)
        {
            Assert.IsTrue(HasSliderBehaviour(name) == false, "同じ名前のUIパーツが、すでに存在しています");


            GameObject gameObjectSlider = new GameObject(name: name);
            var sliderBehaviour = gameObjectSlider.AddComponent<SliderBehaviour>();

            gameObjectSlider.transform.parent = gameObject.transform;

            var area = CalcUIPartsArea(windowElementArea: windowElementArea);

            int sortingOrder = _windowType switch
            {
                EWindowType.TOP => Config.WINDOW_TOP_SLIDER__LAYERED_ORDERID.SortingOrder,
                EWindowType.NORMAL => Config.WINDOW_NORMAL_SLIDER__LAYERED_ORDERID.SortingOrder,
                EWindowType.CONFIRMATION => Config.WINDOW_CONFIRMATION_SLIDER__LAYERED_ORDERID.SortingOrder,
                EWindowType.ERROR => Config.WINDOW_ERROR_SLIDER__LAYERED_ORDERID.SortingOrder,
                _ => throw new NotImplementedException(),
            };


            var initializer = new FSliderInitializer(
                targetCanvas: _canvas,
                sliderImageType: ESliderImageType.SLIDER_DEFAULT,
                area: area.AsYReversed(),
                maxStep: maxStep,
                presentStep: presentStep,
                isPlaySE: isPlaySE,
                isEnabled: isEnabled,
                sortingOrder: sortingOrder,
                actionOnChangeStep: actionOnChangeStep);


            sliderBehaviour.SetupComponent(initializer: initializer);


            _sliderBehaviourList.Add(sliderBehaviour);


            if (_isShown)
            {
                sliderBehaviour.Show();
            }
        }


        public void RemoveSlider(string name)
        {
            Assert.IsTrue(HasSliderBehaviour(name: name));

            var sliderBehaviour = GetSliderBehaviour(name: name);
            _sliderBehaviourList.Remove(sliderBehaviour);

            Utl.SafeDestroy(behaviour: sliderBehaviour);
        }


        public bool IsSliderEnabled(string name)
        {
            Assert.IsTrue(HasSliderBehaviour(name: name));

            return GetSliderBehaviour(name: name).IsEnabled;
        }


        public void SetSliderIsEnabled(string name, bool value)
        {
            Assert.IsTrue(HasSliderBehaviour(name: name));

            GetSliderBehaviour(name: name).SetIsEnabled(value: value);
        }

        public int GetSliderMaxStep(string name)
        {
            return GetSliderBehaviour(name: name).MaxStep;
        }

        public int GetSliderPresentStep(string name)
        {
            return GetSliderBehaviour(name: name).PresentStep;
        }

        public void SetSliderPresentStep(string name, int step)
        {
            GetSliderBehaviour(name: name).SetPresentStep(value: step);
        }

        public void AddSliderPresentStep(string name, int deltaStep)
        {
            GetSliderBehaviour(name: name).AddPresentStep(deltaStep: deltaStep);
        }

        #endregion


        #endregion


        #region On Event
        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _windowBodySingleImageActionBehaviour = null;
            _canvas = null;

            _tapInputBlocker = null;

            _gameObjectCurtain = null;
            _curtainBehaviour = null;
        }

        #endregion
    }
}
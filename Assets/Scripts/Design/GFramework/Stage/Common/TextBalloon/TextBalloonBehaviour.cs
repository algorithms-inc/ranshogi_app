using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    // Canvasを内包しているため、GameObject 自体の position や scale は初期値（中央におく）
    // 内包する Canvas も全体を覆う
    // 内包する text や SingleImageActionBehaviourBalloon の位置を変更している

    public class TextBalloonBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _textFieldBehaviour.Alpha;



        // -----------------------------------------------
        // MARK: - Game Object

        private Canvas _canvas;

        private SingleImageActionBehaviour _singleImageActionBehaviourBalloon;
        public SingleImageActionBehaviour SingleImageActionBehaviourBalloon => _singleImageActionBehaviourBalloon;

        private TextFieldBehaviour _textFieldBehaviour;



        // -----------------------------------------------
        // MARK: - Layouter

        private TextBalloonLayouter _layouter;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(in FArea area, int sortingOrder)
        {
            _layouter = new TextBalloonLayouter(area: area);

            SetupCanvas(sortingOrder: sortingOrder);

            SetupText(area: area);

            SetupTextBalloonBase(area: area, sortingOrder: sortingOrder);

            Hide();
        }


        private void SetupCanvas(int sortingOrder)
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: sortingOrder + 1);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }
        

        private void SetupText(FArea area)
        {
            Assert.IsTrue(_textFieldBehaviour == null);

            FTextFieldInitializer textInitializer = new FTextFieldInitializer(
                text: "",
                fontType: EFontType.SERIF,
                fontSize: (int)(area.size.y * 0.7),
                alignment: EAlignment.MIDDLE_CENTER,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas);

            var newGameObject = new GameObject(name: "TextFieldForTextBalloon");
            newGameObject.transform.parent = gameObject.transform;

            _textFieldBehaviour = newGameObject.AddComponent<TextFieldBehaviour>();
            _textFieldBehaviour.SetupComponent(textInitializer: textInitializer);
        }


        private void SetupTextBalloonBase(in FArea area, int sortingOrder)
        {
            _singleImageActionBehaviourBalloon = UtlSprite.CreateGameObjectWithResizableSpriteAndSingleImageBehaviour(
                name: "BalloonBase",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePathOfTextBalloonBase(),
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform,
                border: new Vector4(x: 64f, y: 64f, z: 64f, w: 64f),
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.4f,
                    durationFadeOut: 0.4f)
                );


            var spriteRenderer = _singleImageActionBehaviourBalloon.gameObject.GetComponent<SpriteRenderer>();

            spriteRenderer.size = Utl.ConvertImageSizeToUnit(imageSize: area.size);

            _singleImageActionBehaviourBalloon.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: area.CenterPoint);
        }


        // ===============================================
        // MARK: - ● Set Text ─────

        public void SetTextAndFadeInIfNeeded(string text)
        {
            SetText(text: text);

            if (IsShown == false)
            {
                FadeIn();
            }
        }


        private void SetText(string text)
        {
            _textFieldBehaviour.SetText(text: text);
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositon(float unitY)
        {
            var unit = new Vector2(transform.localPosition.x, unitY);
            Utl.SetLocalPosition(gameObject: gameObject, unit: unit);
        }

        public void SetLocalPositonByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: gameObject, baseImageSize: _baseImageSize, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _textFieldBehaviour.SetAlpha(alpha: alpha);
            _singleImageActionBehaviourBalloon.SetAlpha(alpha: alpha);
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public void SetColor(Vector3 color)
        {
            UtlSprite.ChangeColor(gameObject: _singleImageActionBehaviourBalloon.gameObject, color: color);
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; }

            SetAlpha(alpha: 1f);

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            SetAlpha(alpha: 0f);

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }


            var action = CAction.CreateImageActionFade(targetBehaviour: this, fadeType: CAction.EFadeType.FADE_IN, duration: Config.ANIMATION_DURATION_3);
            RegisterAction(action: action);
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }


            var action = CAction.CreateImageActionFade(targetBehaviour: this, fadeType: CAction.EFadeType.FADE_OUT, duration: Config.ANIMATION_DURATION_3);
            RegisterAction(action: action);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _canvas = null;
            _textFieldBehaviour = null;
            _singleImageActionBehaviourBalloon = null;

            _layouter?.Destroy();
        }
    }
}
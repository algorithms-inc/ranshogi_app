using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class ResignSelectorBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private GameStageBehaviour _gameStageBehaviour = null;

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;

        private ButtonBehaviour _buttonBehaviourCansel;
        private ButtonBehaviour _buttonBehaviourOK;

        private Canvas _canvas;


        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => throw new NotImplementedException();


        // -----------------------------------------------
        // MARK: - Layout
        #region Layout

        private LayoutRoot _layoutResignSelector;

        private LayoutFrame _frameButtonArea;

        private LayoutFrame _frameButtonCansel;
        private LayoutFrame _frameButtonOK;

        #endregion



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(GameStageBehaviour gameStageBehaviour)
        {
            _gameStageBehaviour = gameStageBehaviour;

            SetupLayout();

            SetupGameObject();

            Hide();
        }



        // ===============================================
        // MARK: - ● Setup Layout ─────

        public void SetupLayout()
        {
            FArea areaResignSelector = _gameStageBehaviour.BoardSetBehaviour.GetAreaSelectorOnGameStage();
            _layoutResignSelector = new LayoutRoot(size: areaResignSelector.size, spacing: new FVec4(0, 0, 0, 0), position: areaResignSelector.origin);

            _frameButtonArea = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 0.7f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _layoutResignSelector.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonArea});


            _frameButtonCansel = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.44f, 0.57f)),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonOK = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.44f, 0.57f)),
                align: EDirection.DOWN_RIGHT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonCansel,
                _frameButtonOK});


            _layoutResignSelector.Update();
        }



        // ===============================================
        // MARK: - ● Setup Game Object ─────

        private void SetupGameObject()
        {
            SetupCanvas();

            SetupBody();
            SetupButton();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "InputCanvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: transform,
                sortingOrder: Config.RESIGN_SELECTOR_INPUT__LAYERED_ORDERID__DEFAULT.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupBody()
        {
            string texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForResignSelector.BODY);

            var gameObjectResignSelectorBody = UtlSprite.CreateGameObjectWithSprite(
                name: "ResignSelectorBody",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: Config.RESIGN_SELECTOR_BODY__LAYERED_ORDERID__DEFAULT.SortingOrder,
                transformParent: transform);

            _bodySingleImageActionBehaviour = gameObjectResignSelectorBody.AddComponent<SingleImageActionBehaviour>();
            var initializer = new FSingleImageActionBehaviourInitializer(defaultAlpha: 1, durationFadeIn: 0.15f, durationFadeOut: 0.15f);
            _bodySingleImageActionBehaviour.SetupComponent(initializer: initializer);


            FArea resignSelectorArea = _layoutResignSelector.Area.AsYReversed();

            gameObjectResignSelectorBody.transform.position = Utl.ConvertImageSizeToUnit(imageSize: resignSelectorArea.CenterPoint);

            var localScale = resignSelectorArea.size / Config.RESIGN_SELECTOR_BODY__IMAGE_SIZE;
            gameObjectResignSelectorBody.transform.localScale = localScale;
        }


        private void SetupButton()
        {
            ButtonBehaviour support(string name, EButtonImageType buttonImageType, FArea buttonArea, Action actionOnFire)
            {
                GameObject gameObjectButton = new GameObject(name: name);
                var buttonBehaviour = gameObjectButton.AddComponent<ButtonBehaviour>();

                gameObjectButton.transform.parent = gameObject.transform;

                var buttonInitializer = new FButtonInitializer(
                    targetCanvas: _canvas,
                    buttonImageType: buttonImageType,
                    area: buttonArea,
                    tapJudgeType: ETapJudgeType.CLICK,
                    isPlaySE: true,
                    isEnabled: true,
                    sortingOrder: Config.RESIGN_SELECTOR_BUTTON__LAYERED_ORDERID__DEFAULT.SortingOrder,
                    action: actionOnFire);

                buttonBehaviour.SetupComponent(initializer: buttonInitializer);

                // 通常とは異なる短い非表示／非表示アニメーションを設定している
                buttonBehaviour.SetFadeDuration(durationFadeIn: 0.15f, durationFadeOut: 0.15f);

                return buttonBehaviour;
            }

            _buttonBehaviourCansel = support(
                name: "ButtonCansel",
                buttonImageType: EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_CANSEL,
                buttonArea: _frameButtonCansel.Area.AsYReversed(),
                actionOnFire: () => { OnAnyButtonDown(selectResultType: ESelectResultType.CANSEL); });

            _buttonBehaviourOK = support(
                name: "ButtonOK",
                buttonImageType: EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_OK,
                buttonArea: _frameButtonOK.Area.AsYReversed(),
                actionOnFire: () => { OnAnyButtonDown(selectResultType: ESelectResultType.OK); });
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("ResignSelector", _layoutResignSelector.Area.AsYReversed()),
                ("ResignSelectorButtonCansel", _frameButtonCansel.Area.AsYReversed()),
                ("ResignSelectorButtonOk", _frameButtonOK.Area.AsYReversed())
            });
        }



        // ===============================================
        // MARK: - ● On Event ─────

        private void OnAnyButtonDown(ESelectResultType selectResultType)
        {
            _gameStageBehaviour.OnDidSelectResign(selectResultType: selectResultType);

            FadeOut();
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }


        public override void Show()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.Show();
            _buttonBehaviourCansel.Show();
            _buttonBehaviourOK.Show();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.Hide();
            _buttonBehaviourCansel.Hide();
            _buttonBehaviourOK.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.FadeIn();
            _buttonBehaviourCansel.FadeIn();
            _buttonBehaviourOK.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.FadeOut();
            _buttonBehaviourCansel.FadeOut();
            _buttonBehaviourOK.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();


            ReleaseReference();

            _layoutResignSelector?.Destroy();
        }


        private void ReleaseReference()
        {
            _gameStageBehaviour = null;
            _bodySingleImageActionBehaviour = null;
            _buttonBehaviourCansel = null;
            _buttonBehaviourOK = null;
            _canvas = null;
        }
    }
}
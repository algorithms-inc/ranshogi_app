using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;



namespace GFramework
{
    public class BackgroundBehaviour : ImageActionBehaviour
    {
        public enum EBackgroundFlowType
        {
            NONE,
            UP,
            DOWN
        }


        // ===============================================
        // MARK: - ● Property  ─────

        private EBackgroundFlowType _backgroundFlowType = EBackgroundFlowType.NONE;
        public EBackgroundFlowType BackgroundFlowType => _backgroundFlowType;

        
        // -----------------------------------------------
        // MARK: - Game Object

        // 基本背景画像(= Player 1の色)
        private SingleImageActionBehaviour _singleImageActionBehaviourBodyA;

        // Player 2 の番手の場合の色
        private SingleImageActionBehaviour _singleImageActionBehaviourBodyB;

        // 波紋
        private readonly List<RippleBehaviour> _rippleBehaviourList = new List<RippleBehaviour>();


        // -----------------------------------------------
        // MARK: - Others

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _singleImageActionBehaviourBodyA.Alpha;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent()
        {
            SetupGameObject();

            Hide();
        }


        private void SetupGameObject()
        {
            SetupBodyA();
            SetupBodyB();
            SetupRipple();
        }


        private void SetupBodyA()
        {
            _singleImageActionBehaviourBodyA = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "BackgroundBodyA",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForBackground.BODY_A),
                willCacheIfNeeded: true,
                sortingOrder: Config.BACKGROUND_BODY_A__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 1f,
                    durationFadeOut: 1f)
                );

            UtlSprite.FitGameObjectWithSingleImageActionBehaviourToArea(
                gameObject: _singleImageActionBehaviourBodyA.gameObject,
                area: Locator.Instance.layoutManager.AreaBackground,
                scaleMode: EScaleMode.FIT_AND_FILL);
        }


        private void SetupBodyB()
        {
            _singleImageActionBehaviourBodyB = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "BackgroundBodyB",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForBackground.BODY_B),
                willCacheIfNeeded: true,
                sortingOrder: Config.BACKGROUND_BODY_B__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f)
                );

            UtlSprite.FitGameObjectWithSingleImageActionBehaviourToArea(
               gameObject: _singleImageActionBehaviourBodyB.gameObject,
               area: Locator.Instance.layoutManager.AreaBackground,
               scaleMode: EScaleMode.FIT_AND_FILL);
        }


        private void SetupRipple()
        {
            void Support(string name, ERippleType rippleType)
            {
                var area = Locator.Instance.layoutManager.GetAreaOfRipple(rippleType: rippleType);


                var gameObject = new GameObject(name: name);
                gameObject.transform.parent = transform;
                
                var rippleBehaviour = gameObject.AddComponent<RippleBehaviour>();
                rippleBehaviour.SetupComponent(
                    rippleType: rippleType,
                    initializer: new FSingleImageActionBehaviourInitializer(
                        defaultAlpha: Config.BACKGROUND_RIPPLE__ALPHA_DEFAULT,
                        durationFadeIn: Config.ANIMATION_DURATION_10,
                        durationFadeOut: Config.ANIMATION_DURATION_3));
                _rippleBehaviourList.Add(rippleBehaviour);

                gameObject.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: area.CenterPoint);
            }

            Support(name: "RippleA", rippleType: ERippleType.A);
            Support(name: "RippleB", rippleType: ERippleType.B);
            Support(name: "RippleC", rippleType: ERippleType.C);
            Support(name: "RippleD", rippleType: ERippleType.D);
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _singleImageActionBehaviourBodyA.SetAlpha(alpha: alpha);
        }



        // ===============================================
        // MARK: - ● Animation ─────

        public void DoNormalAnimation()
        {
            _rippleBehaviourList[0].DoNormalAnimation(
                swingCounterForAlpha: new CSwingCounter(count: 400, min: 0, max: 800, isForwardDirection: true),
                oneWayCounterHorizontalMove: new COnewayCounter(count: 5000, min: 0, max: 10000, isForwardDirection: true),
                swingCounterScale: new CSwingCounter(count: 500, min: 0, max: 1000, isForwardDirection: true));

            _rippleBehaviourList[1].DoNormalAnimation(
                swingCounterForAlpha: new CSwingCounter(count: 350, min: 0, max: 700, isForwardDirection: false),
                oneWayCounterHorizontalMove: new COnewayCounter(count: 6000, min: 0, max: 12000,  isForwardDirection: true),
                swingCounterScale: new CSwingCounter(count: 750, min: 0, max: 1500, isForwardDirection: true));

            _rippleBehaviourList[2].DoNormalAnimation(
                swingCounterForAlpha: new CSwingCounter(count: 350, min: 0, max: 700, isForwardDirection: true),
                oneWayCounterHorizontalMove: new COnewayCounter(count: 5500, min: 0, max: 11000, isForwardDirection: true),
                swingCounterScale: new CSwingCounter(count: 450, min: 0, max: 900, isForwardDirection: false));

            _rippleBehaviourList[3].DoNormalAnimation(
                swingCounterForAlpha: new CSwingCounter(count: 400, min: 0, max: 800, isForwardDirection: false),
                oneWayCounterHorizontalMove: new COnewayCounter(count: 6500, min: 0, max: 13000, isForwardDirection: true),
                swingCounterScale: new CSwingCounter(count: 600, min: 0, max: 1200, isForwardDirection: false));
        }



        // ===============================================
        // MARK: - ● Background Flow Type ─────

        public void SetBackgroundFlowType(EBackgroundFlowType backgroundFlowType)
        {
            _backgroundFlowType = backgroundFlowType;
        }



        // ===============================================
        // MARK: - ● ChangeToPlayer ─────

        public void ChangeToPlayer(EPlayerType playerType)
        {
            if (AppManager.Instance.DesignController.IsChangeBackground() == false) { return; }

            switch (playerType)
            {
                case EPlayerType.PLAYER1:
                    _singleImageActionBehaviourBodyB.FadeOut();
                    break;

                case EPlayerType.PLAYER2:
                    _singleImageActionBehaviourBodyB.FadeIn();
                    break;

                default: throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; }

            _singleImageActionBehaviourBodyA.Show();
            // bodyBは表示しない

            _rippleBehaviourList.ForEach(i => i.Show());

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _singleImageActionBehaviourBodyA.Hide();
            _singleImageActionBehaviourBodyB.Hide();

            _rippleBehaviourList.ForEach(i => i.Hide());

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _singleImageActionBehaviourBodyA.FadeIn();
            // bodyBは表示しない

            _rippleBehaviourList.ForEach(i => i.FadeIn());

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _singleImageActionBehaviourBodyA.FadeOut();
            _singleImageActionBehaviourBodyB.FadeOut();

            _rippleBehaviourList.ForEach(i => i.FadeOut());

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _singleImageActionBehaviourBodyA = null;
            _singleImageActionBehaviourBodyB = null;

            _rippleBehaviourList.Clear();
        }
    }
}
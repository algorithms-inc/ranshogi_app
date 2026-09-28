using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public sealed class BackgroundManager : IBackgroundManager
    {
        private BackgroundBehaviour _backgroundBehaviour;
        private SingleImageActionBehaviour _singleImageActionBehaviourScreenEdge;


        public BackgroundManager()
        {
            Setup();
        }


        private void Setup()
        {
            SetupGameObjectBackground();
            SetupGameObjectScreenEdge();

            _backgroundBehaviour.FadeIn();
            _backgroundBehaviour.DoNormalAnimation();
        }


        private void SetupGameObjectBackground()
        {
            var gameObjectBackground = new GameObject("Background");
            _backgroundBehaviour = gameObjectBackground.AddComponent<BackgroundBehaviour>();
            _backgroundBehaviour.SetupComponent();

            Utl.SetDontDestroyOnLoad(targetObject: gameObjectBackground);
        }


        private void SetupGameObjectScreenEdge()
        {
            // タブレット端末のみでエッジを表示する
            if (Utl.IsSmartPhone()) { return; }
            

            _singleImageActionBehaviourScreenEdge = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "ScreenEdge",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForBackground.SCREEN_EDGE),
                willCacheIfNeeded: true,
                sortingOrder: Config.BACKGROUND_EDGE__LAYERED_ORDERID.SortingOrder,
                transformParent: _backgroundBehaviour.transform,
                initializer: new FSingleImageActionBehaviourInitializer(defaultAlpha: 1.0f, durationFadeIn: 0.3f, durationFadeOut: 0.3f));

            UtlSprite.FitGameObjectWithSingleImageActionBehaviourToArea(
                gameObject: _singleImageActionBehaviourScreenEdge.gameObject,
                area: Locator.Instance.layoutManager.AreaBackground.AsYReversed(),
                scaleMode: EScaleMode.STRETCH);

            _singleImageActionBehaviourScreenEdge.Show();
        }


        public BackgroundBehaviour.EBackgroundFlowType GetBackgroundFlowType()
        {
            return _backgroundBehaviour.BackgroundFlowType;
        }


        public void ChangePlayerColor(EPlayerType playerType)
        {
            _backgroundBehaviour.ChangeToPlayer(playerType: playerType);
        }


        public void Destroy()
        {
            _backgroundBehaviour = null;
            _singleImageActionBehaviourScreenEdge = null;
        }
    }
}
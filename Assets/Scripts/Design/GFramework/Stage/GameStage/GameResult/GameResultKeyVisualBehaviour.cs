using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class GameResultKeyVisualBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private EGameMode _gameMode = EGameMode.NORMAL;
        private EGameResultType _gameResultType = EGameResultType.WIN;
        private int _consecutiveCount = 0;


        // -----------------------------------------------
        // MARK: - Game Object

        private bool _isShown = false;
        public bool IsShown => _isShown;

        public override float Alpha => _bodySingleImageActionBehaviour.Alpha;

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;
        private SingleImageActionBehaviour _consecutiveCountSingleImageActionBehaviour;



        // -----------------------------------------------
        // MARK: - Layout

        private GameResultKeyVisualLayouter _layouter;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(EGameMode gameMode, EGameResultType gameResultType, int consecutiveCount, FArea area)
        {
            _gameMode = gameMode;
            _gameResultType = gameResultType;
            _consecutiveCount = consecutiveCount;

            SetupLayout(area: area);

            SetupGameObject();
        }



        // ===============================================
        // MARK: - ● Layout ─────

        public void SetupLayout(FArea area)
        {
            // YReversed の値を渡す必要がある
            _layouter = new GameResultKeyVisualLayouter(area: area.AsYReversed());
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void SetupGameObject()
        {
            SetupBody();

            if (ShouldShowConsecutiveCount())
            {
                SetupConsecutiveCount();
            }
        }


        private void SetupBody()
        {
            _bodySingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "KeyVisual",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(gameResultType: _gameResultType),
                willCacheIfNeeded: true,
                sortingOrder: Config.GAME_RESULT_GAME_RESULT_IMAGE__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f),
                area: _layouter.AreaKeyVisual,
                scaleMode: EScaleMode.FIT
                );
        }


        private void SetupConsecutiveCount()
        {
            Assert.IsTrue(ShouldShowConsecutiveCount());

            _consecutiveCountSingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "ConsecutiveCount",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePathOfGameResultConsecutiveCount(gameResultType: _gameResultType, count: _consecutiveCount),
                willCacheIfNeeded: true,
                sortingOrder: Config.GAME_RESULT_CONSECUTIVE_COUNT__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f),
                area: _layouter.AreaConsecutiveCount,
                scaleMode: EScaleMode.FIT
                );
        }



        // ===============================================
        // MARK: - ● Consecutive Count ─────

        private bool ShouldShowConsecutiveCount()
        {
            return false;

            // TODO: いったん表示しない設定にしておく
            // return (_gameMode == EGameMode.NORMAL) && (_consecutiveCount >= 2) && (_gameResultType != EGameResultType.DRAW);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            _bodySingleImageActionBehaviour.SetAlpha(alpha: alpha);

            if (ShouldShowConsecutiveCount())
            {
                _consecutiveCountSingleImageActionBehaviour.SetAlpha(alpha: alpha);
            }
        }

        public override void Show()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.Show();

            if (ShouldShowConsecutiveCount())
            {
                _consecutiveCountSingleImageActionBehaviour.Show();
            }

            _isShown = true;
        }

        public override void Hide()
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.Hide();

            if (ShouldShowConsecutiveCount())
            {
                _consecutiveCountSingleImageActionBehaviour.Hide();
            }

            _isShown = false;
        }



        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.FadeIn();

            if (ShouldShowConsecutiveCount())
            {
                _consecutiveCountSingleImageActionBehaviour.FadeIn();
            }

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.FadeOut();

            if (ShouldShowConsecutiveCount())
            {
                _consecutiveCountSingleImageActionBehaviour.FadeOut();
            }

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _layouter?.Destroy();

            _bodySingleImageActionBehaviour = null;
            _consecutiveCountSingleImageActionBehaviour = null;
        }
    }
}
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
    public class SliderBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        // Base
        private SingleImageActionBehaviour _baseSingleImageActionBehaviour;
        public SingleImageActionBehaviour BaseSingleImageActionBehaviour => _baseSingleImageActionBehaviour;

        // Bar
        private SingleImageActionBehaviour _barSingleImageActionBehaviour;
        public SingleImageActionBehaviour BarSingleImageActionBehaviour => _barSingleImageActionBehaviour;

        // Mark
        private SingleImageActionBehaviour _markSingleImageActionBehaviour;
        public SingleImageActionBehaviour MarkSingleImageActionBehaviour => _markSingleImageActionBehaviour;

        // Input
        private TapAndSwipeInputBehaviour _tapAndSwipeInputBehaviour;

        // Area
        private FArea _area;

        // Alpha
        public float Alpha => UtlSprite.GetSpriteAlpha(gameObject: _baseSingleImageActionBehaviour.gameObject);

        // Step
        private int _maxStep = 0;
        public int MaxStep => _maxStep;


        private int _lastStep = 0;

        private int _presentStep = 0;
        public int PresentStep => _presentStep;


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

        // is Play SE
        private bool _isPlaySE = true;
        public bool IsPlaySE => _isPlaySE;

        // is Enabled
        private bool _isEnabled = true;
        public bool IsEnabled => _isEnabled;

        // is Shown
        private bool _isShown = true;
        public bool IsShown => _isShown;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(FSliderInitializer initializer)
        {
            _area = initializer.Area;
            _maxStep = initializer.MaxStep;
            _presentStep = initializer.PresentSetp;
            _isPlaySE = initializer.IsPlaySE;
            _isEnabled = initializer.IsEnabled;
            _action = initializer.Action;

            _lastStep = _presentStep;


            SetupBase(sortingOrder: initializer.SortingOrder);
            SetupBar(sortingOrder: initializer.SortingOrder + 1);
            SetupMark(sortingOrder: initializer.SortingOrder + 2);
            SetupTapAndSwipeInput(targetCanvas: initializer.TargetCanvas);

            transform.position = Utl.ConvertImageSizeToUnit(imageSize: _area.CenterPoint);

            //UpdateBarAndmark();


            Hide();
        }


        private void SetupBase(int sortingOrder)
        {
            GameObject gameObject = UtlSprite.CreateGameObjectWithSprite(
                name: "SliderBase",
                texturePathUnderResources: GetTexturePathUnderResourcesForBase(),
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform);

            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.size = Utl.ConvertImageSizeToUnit(imageSize: _area.size);

            _baseSingleImageActionBehaviour = gameObject.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);

            _baseSingleImageActionBehaviour.SetupComponent(initializer: initializer);


            var textureSize = UtlSprite.GetTextureSize(gameObject: _baseSingleImageActionBehaviour.gameObject);

            var sizeOfBase = new Vector2(x: _area.size.x - _area.size.y, y: _area.size.y);
            var scale = sizeOfBase / textureSize;

            _baseSingleImageActionBehaviour.transform.localScale = scale;
        }


        private void SetupBar(int sortingOrder)
        {
            GameObject gameObject = UtlSprite.CreateGameObjectWithSprite(
                name: "SliderBar",
                texturePathUnderResources: GetTexturePathUnderResourcesForBar(),
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform);

            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.size = Utl.ConvertImageSizeToUnit(imageSize: _area.size);

            _barSingleImageActionBehaviour = gameObject.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);

            _barSingleImageActionBehaviour.SetupComponent(initializer: initializer);

            UpdateBar();
        }


        private void SetupMark(int sortingOrder)
        {
            GameObject gameObject = UtlSprite.CreateGameObjectWithSprite(
                name: "SliderMark",
                texturePathUnderResources: GetTexturePathUnderResourcesForMark(),
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform);

            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.size = Utl.ConvertImageSizeToUnit(imageSize: _area.size);

            _markSingleImageActionBehaviour = gameObject.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);

            _markSingleImageActionBehaviour.SetupComponent(initializer: initializer);


            var textureSize = UtlSprite.GetTextureSize(gameObject: _markSingleImageActionBehaviour.gameObject);
            var sizeOfMark = new Vector2(x: _area.size.y, y: _area.size.y);
            var scale = sizeOfMark / textureSize;

            _markSingleImageActionBehaviour.transform.localScale = scale;


            UpdateMark();
        }



        // ===============================================
        // MARK: - ● File Path ─────

        private string GetTexturePathUnderResourcesForBase()
        {
            if (_isEnabled)
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUISlider.BASE_DEFAULT);
            }
            else
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUISlider.BASE_DISABLED);
            }
        }


        private string GetTexturePathUnderResourcesForBar()
        {
            if (_isEnabled)
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUISlider.BAR_DEFAULT);
            }
            else
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUISlider.BAR_DISABLED);
            }
        }


        private string GetTexturePathUnderResourcesForMark()
        {
            if (_isEnabled)
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUISlider.MARK_DEFAULT);
            }
            else
            {
                return Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForUISlider.MARK_DISABLED);
            }
        }



        // ===============================================
        // MARK: - ● ─────

        // タップ入力のセットアップ
        private void SetupTapAndSwipeInput(Canvas targetCanvas)
        {
            var gameObjectTapInputAndSwipe = new GameObject(name: "TapAndSwipeInputForSlider");

            _tapAndSwipeInputBehaviour = gameObjectTapInputAndSwipe.AddComponent<TapAndSwipeInputBehaviour>();

            gameObjectTapInputAndSwipe.transform.parent = targetCanvas.transform;

            _tapAndSwipeInputBehaviour.SetupComponent(
                size: _area.size,
                position: _area.CenterPoint,
                isEnabled: _isEnabled,
                actionOnPointerDown: (i, t) =>
                {
                    //Debug.Log("Swipe: PointerDown" + t.ToString());
                    MoveMarkAccordingToTap(tapPositionX: t.x);
                    FireAction();
                },
                actionOnPointerUp: (i, t) =>
                {
                    //Debug.Log("Swipe: PointerUp " + t.ToString());
                },
                actionOnPointerSwipe: (i, t) =>
                {
                    //Debug.Log("Swipe: PointerSwipe " + t.ToString());
                    MoveMarkAccordingToTap(tapPositionX: t.x);
                    FireAction();
                });
        }



        // ===============================================
        // MARK: - ● Update Bar and Mark ─────

        private void UpdateBarAndMark()
        {
            UpdateBar();
            UpdateMark();
        }

        private float CalcProgressOfStep()
        {
            return (float)_presentStep / (float)_maxStep;
        }

        private void UpdateBar()
        {
            // scale
            var textureSize = UtlSprite.GetTextureSize(gameObject: _barSingleImageActionBehaviour.gameObject);

            var progress = CalcProgressOfStep();
            var sizeOfBar = new Vector2(x: (_area.size.x - _area.size.y) * progress, y: _area.size.y);
            var scale = sizeOfBar / textureSize;

            _barSingleImageActionBehaviour.transform.localScale = scale;


            // position

            var newCenterPointX = Utl.ConvertImageSizeToUnit(imageSize: _area.CenterPoint.x - (_area.size.x - _area.size.y - sizeOfBar.x) / 2f - _area.CenterPoint.x);
            var presentLocalPosition = _barSingleImageActionBehaviour.transform.localPosition;

            _barSingleImageActionBehaviour.transform.localPosition = new Vector3(x: newCenterPointX, y: presentLocalPosition.y, z: presentLocalPosition.z);
        }


        private void UpdateMark()
        {
            // position
            var positionXWhenStep0 = _area.CenterPoint.x - (_area.size.x - _area.size.y) / 2f;
            var progress = CalcProgressOfStep();
            var newCenterPointX = Utl.ConvertImageSizeToUnit(imageSize: positionXWhenStep0 + (_area.size.x - _area.size.y) * progress - _area.CenterPoint.x);

            var presentLocalPosition = _markSingleImageActionBehaviour.transform.localPosition;

            _markSingleImageActionBehaviour.transform.localPosition = new Vector3(x: newCenterPointX, y: presentLocalPosition.y, z: presentLocalPosition.z);
        }


        private void MoveMarkAccordingToTap(float tapPositionX)
        {
            _lastStep = _presentStep;
            _presentStep = CalcNearestStep(tapPositionX: tapPositionX);

            if (_presentStep == _lastStep) { return; }

            PlaySE();
            UpdateBarAndMark();
        }

        private void PlaySE()
        {
            if (_isPlaySE == false) { return; }

            Locator.Instance.audioManager.Play(SEType: ESEType.SLIDER_CHANGE_VALUE);
        }


        private int CalcNearestStep(float tapPositionX)
        {
            // - タップ位置は左下を原点とするスクリーン座標で返される (at Canvas)
            // - UIは画面中央を原点とし、右下方向にXYが展開される (at スクリーン？)
            // - スライダーは横方向の動きのみ考慮すればいいので、Yについては考慮しなくていい

            var screenSizeX = Utl.GetScreenSize().x;

            var barAreaOriginXAsTapCoordinate = _area.origin.x + _area.size.y / 2f + screenSizeX / 2f;
            var barAreaEndPointXAsTapCoordinate = _area.EndPoint.x - _area.size.y / 2f + screenSizeX / 2f;


            float wrappedTapPositionX = tapPositionX;

            if (wrappedTapPositionX < barAreaOriginXAsTapCoordinate)
            {
                wrappedTapPositionX = barAreaOriginXAsTapCoordinate;
            }
            else if (barAreaEndPointXAsTapCoordinate < wrappedTapPositionX)
            {
                wrappedTapPositionX = barAreaEndPointXAsTapCoordinate;
            }

            var temp = (wrappedTapPositionX - barAreaOriginXAsTapCoordinate) / (barAreaEndPointXAsTapCoordinate - barAreaOriginXAsTapCoordinate) * _maxStep;

            return (int)Math.Round(temp);
        }


        // ===============================================
        // MARK: - ● Step ─────

        public void SetPresentStep(int value)
        {
            Assert.IsTrue(IsValidStep(value: value));

            _presentStep = value;

            if (_lastStep == _presentStep) { return; }

            UpdateBarAndMark();

            // ユーザによるタップが存在している可能性があるため、強制的にタップを解除する
            _tapAndSwipeInputBehaviour.ForceStop();
        }


        private bool IsValidStep(int value)
        {
            if (value < 0) { return false; }

            if (_maxStep < value) { return false; }

            return true;
        }


        public void AddPresentStep(int deltaStep)
        {
            var newStep = GetWrapStep(step: _presentStep + deltaStep);

            if (_presentStep == newStep) { return; }

            SetPresentStep(value: newStep);
        }


        // 与えられた値を正しいステップに収める
        private int GetWrapStep(int step)
        {
            if (step < 0) { return 0; };
            if (_maxStep < step) { return _maxStep; };

            return step;
        }



        // ===============================================
        // MARK: - ● Debug ─────

        /// Debug: タップエリアを表示
        public void ShowTapArea()
        {
            _tapAndSwipeInputBehaviour.ShowArea();
        }


        /// Debug: タップエリアを非表示
        public void HideTapArea()
        {
            _tapAndSwipeInputBehaviour.HideArea();
        }



        // ===============================================
        // MARK: - ● Action ─────

        /// Action の実行可否をセット
        public void SetIsActionEnabled(bool value)
        {
            _isActionEnabled = value;

            // ユーザによるタップが残っている可能性があるので、強制的に解除
            _tapAndSwipeInputBehaviour.ForceStop();
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
            if (_lastStep == _presentStep) { return; }

            if (_isActionEnabled)
            {
                _action?.Invoke();
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
        }


        public void SetColorAndAlpha(FColor3 color, float alpha)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Fade Duration ─────

        public void SetFadeDuration(float durationFadeIn, float durationFadeOut)
        {
            _baseSingleImageActionBehaviour.SetFadeDuration(durationFadeIn: durationFadeIn, durationFadeOut: durationFadeOut);
            _barSingleImageActionBehaviour.SetFadeDuration(durationFadeIn: durationFadeIn, durationFadeOut: durationFadeOut);
            _markSingleImageActionBehaviour.SetFadeDuration(durationFadeIn: durationFadeIn, durationFadeOut: durationFadeOut);
        }



        // ===============================================
        // MARK: - ● Eneble ─────

        public void SetIsEnabled(bool value)
        {
            if (value == _isEnabled) { return; }

            _isEnabled = value;


            UpdateTexture();

            _tapAndSwipeInputBehaviour.SetIsEnabled(value: _isEnabled);

            // ユーザによるタップが残っている可能性があるので、強制的に解除
            _tapAndSwipeInputBehaviour.ForceStop();
        }



        // ===============================================
        // MARK: - ● Update Texture ─────

        private void UpdateTexture()
        {
            UtlSprite.ChangeTexture(
                gameObject: _baseSingleImageActionBehaviour.gameObject,
                texturePathUnderResources: GetTexturePathUnderResourcesForBase(),
                willCacheIfNeeded: true);

            UtlSprite.ChangeTexture(
                gameObject: _barSingleImageActionBehaviour.gameObject,
                texturePathUnderResources: GetTexturePathUnderResourcesForBar(),
                willCacheIfNeeded: true);

            UtlSprite.ChangeTexture(
                gameObject: _markSingleImageActionBehaviour.gameObject,
                texturePathUnderResources: GetTexturePathUnderResourcesForMark(),
                willCacheIfNeeded: true);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void Show()
        {
            if (_isShown) { return; }

            _isTapBlockedByAnimation = false;

            _baseSingleImageActionBehaviour.Show();
            _barSingleImageActionBehaviour.Show();
            _markSingleImageActionBehaviour.Show();

            _isShown = true;
        }


        public void Hide()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            _baseSingleImageActionBehaviour.Hide();
            _barSingleImageActionBehaviour.Hide();
            _markSingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public void FadeIn()
        {
            if (_isShown) { return; }

            _baseSingleImageActionBehaviour.FadeIn(actionOnDidFinish: () => { _isTapBlockedByAnimation = false; }); // _isTapBlockedByAnimation をフェード操作の後で解除する
            _barSingleImageActionBehaviour.FadeIn();
            _markSingleImageActionBehaviour.FadeIn();

            _isShown = true;
        }


        public void FadeOut()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            _baseSingleImageActionBehaviour.FadeOut();
            _barSingleImageActionBehaviour.FadeOut();
            _markSingleImageActionBehaviour.FadeOut();

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
            Utl.SafeDestroy(behaviour: _tapAndSwipeInputBehaviour);
        }


        private void ReleaseReference()
        {
            _baseSingleImageActionBehaviour = null;
            _barSingleImageActionBehaviour = null;
            _markSingleImageActionBehaviour = null;
            _tapAndSwipeInputBehaviour = null;

            _action = null;
        }
    }
}
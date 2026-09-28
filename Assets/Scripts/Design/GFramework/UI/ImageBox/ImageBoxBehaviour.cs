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
    public class ImageBoxBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        // Base
        private SingleImageActionBehaviour _baseSingleImageActionBehaviour;
        public SingleImageActionBehaviour BaseSingleImageActionBehaviour => _baseSingleImageActionBehaviour;

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

        // is Shown
        private bool _isShown = true;
        public bool IsShown => _isShown;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(FImageBoxInitializer initializer)
        {
            _area = initializer.Area;
            _action = initializer.Action;

            SetupBase(texturePathUnderResources: initializer.TexturePathUnderResources, sortingOrder: initializer.SortingOrder);
            SetupTapInput(targetCanvas: initializer.TargetCanvas);

            Hide();
        }


        private void SetupBase(string texturePathUnderResources, int sortingOrder)
        {
            GameObject gameObject = UtlSprite.CreateGameObjectWithSprite(
                name: "ImageBoxBase",
                texturePathUnderResources: texturePathUnderResources,
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

           
            transform.position = Utl.ConvertImageSizeToUnit(imageSize: _area.CenterPoint);

            UpdateLocalScaleAccordingToTextureSize();
        }


        // テクスチャサイズを調べて、表示エリアに合わせて拡大縮小する
        private void UpdateLocalScaleAccordingToTextureSize()
        {
            var textureSize = UtlSprite.GetTextureSize(gameObject: _baseSingleImageActionBehaviour.gameObject);
            var scale = _area.size / textureSize;

            transform.localScale = scale;
        }


        // タップ入力のセットアップ
        private void SetupTapInput(Canvas targetCanvas)
        {
            var gameObjectTapInput = new GameObject(name: "TapInputForCheckBox");

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
        // MARK: - ● Change Image ─────

        public void ChangeImage(string texturePathUnderResources)
        {
            UtlSprite.ChangeTexture(
                gameObject: _baseSingleImageActionBehaviour.gameObject,
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true);

            UpdateLocalScaleAccordingToTextureSize();
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
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void Show()
        {
            if (_isShown) { return; }

            _isTapBlockedByAnimation = false;

            _baseSingleImageActionBehaviour.Show();

            _isShown = true;
        }


        public void Hide()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            _baseSingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public void FadeIn()
        {
            if (_isShown) { return; }

            _baseSingleImageActionBehaviour.FadeIn(actionOnDidFinish: () => { _isTapBlockedByAnimation = false; }); // _isTapBlockedByAnimation をフェード操作の後で解除する

            _isShown = true;
        }


        public void FadeOut()
        {
            if (_isShown == false) { return; }

            _isTapBlockedByAnimation = true;

            _baseSingleImageActionBehaviour.FadeOut();

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
            _baseSingleImageActionBehaviour = null;
            _tapInputBehaviour = null;

            _action = null;
        }
    }
}
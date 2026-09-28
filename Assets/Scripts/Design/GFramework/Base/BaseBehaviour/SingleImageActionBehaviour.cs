using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public readonly struct FSingleImageActionBehaviourInitializer
    {
        // ===============================================
        // MARK: - ● Property ────

        public float DefaultAlpha { get; }
        public float DurationFadeIn { get; }
        public float DurationFadeOut { get; }


        // ===============================================
        // MARK: - ● Constructor ─────

        public FSingleImageActionBehaviourInitializer(
            float defaultAlpha,
            float durationFadeIn,
            float durationFadeOut)
        {
            DefaultAlpha = defaultAlpha;
            DurationFadeIn = durationFadeIn;
            DurationFadeOut = durationFadeOut;
        }
    }



    /// <summary>
    /// 他の GameObject を内包しない画像表示 GameObject
    /// - 初期状態は Hide
    /// </summary>
    public class SingleImageActionBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private bool _isShown = true;
        public bool IsShown => _isShown;

        private float _durationFadeIn;
        private float _durationFadeOut;

        public override float Alpha => UtlSprite.GetSpriteAlpha(gameObject: gameObject);



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(in FSingleImageActionBehaviourInitializer initializer)
        {
            SetDefaultAlpha(alpha: initializer.DefaultAlpha);
            _durationFadeIn = initializer.DurationFadeIn;
            _durationFadeOut = initializer.DurationFadeOut;

            Hide();
        }


        // ===============================================
        // MARK: - ● Set BaseSize ─────

        public void SetBaseImageSizeUsingSpriteTextureSize()
        {
            _baseImageSize = UtlSprite.GetTextureSize(gameObject: this.gameObject);
        }


        public void SetBaseImageSize(Vector2 baseImageSize)
        {
            _baseImageSize = baseImageSize;
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            UtlSprite.SetSpriteAlpha(gameObject: gameObject, alpha: alpha);
        }



        // ===============================================
        // MARK: - ● Set Fade Duration ─────

        public void SetFadeDuration(float durationFadeIn, float durationFadeOut)
        {
            _durationFadeIn = durationFadeIn;
            _durationFadeOut = durationFadeOut;
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; }

            SetAlpha(alpha: DefaultAlpha);

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            SetAlpha(alpha: 0);

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            var action = CAction.CreateImageActionFade(
                targetBehaviour: this,
                fadeType: CAction.EFadeType.FADE_IN,
                duration: _durationFadeIn,
                actionOnDidFinish: actionOnDidFinish);

            RegisterAction(action: action);

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            var action = CAction.CreateImageActionFade(
                targetBehaviour: this,
                fadeType: CAction.EFadeType.FADE_OUT,
                duration: _durationFadeOut,
                actionOnDidFinish: actionOnDidFinish);

            RegisterAction(action: action);

            _isShown = false;
        }
    }
}
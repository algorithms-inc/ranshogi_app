using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class CImageActionFade : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private ImageActionBehaviour _targetBehaviour;

        private readonly EFadeType _fadeType;

        private float _beginAlpha;

        private float _endAlpha;

        private Action _actionOnDidFinish;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CImageActionFade(
            ImageActionBehaviour targetBehaviour,
            EFadeType fadeType,
            float duration,
            Action actionOnDidFinish = null) : base(targetBehaviour)
        {
            _targetBehaviour = targetBehaviour;

            _fadeType = fadeType;
            _duration = duration;

            _actionOnDidFinish = actionOnDidFinish;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            switch (_fadeType)
            {
                case EFadeType.FADE_IN:
                    _beginAlpha = 0;
                    _endAlpha = _targetBehaviour.DefaultAlpha;
                    break;

                case EFadeType.FADE_OUT:
                    _beginAlpha = _targetBehaviour.Alpha;
                    _endAlpha = 0;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }


        protected override void ProcessMain()
        {
            float elapsedTimeAlpha = GetElapsedTimeAlpha();

            Fade(elapsedTimeAlpha: elapsedTimeAlpha);
        }


        private void Fade(float elapsedTimeAlpha) => 
            _targetBehaviour.SetAlpha(alpha: Interpole.SinCurveInStrong(begin: _beginAlpha, end: _endAlpha, alpha: elapsedTimeAlpha));


        protected override void ProcessOnLastFrame()
        {
            switch (_fadeType)
            {
                case EFadeType.FADE_IN:
                    _targetBehaviour.Show();
                    break;

                case EFadeType.FADE_OUT:
                    _targetBehaviour.Hide();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            _actionOnDidFinish?.Invoke();

            _actionOnDidFinish = null;
            _targetBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

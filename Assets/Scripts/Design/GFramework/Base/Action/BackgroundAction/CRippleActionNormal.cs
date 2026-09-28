using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO: unit の値を正しく設定する。

namespace GFramework
{
    public class CRippleActionNormal : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private RippleBehaviour _rippleBehaviour;

        private readonly float _defaultAlpha;
        private Vector2 _originalPosition;
        private readonly float _originalScale;

        private FMinMax _minMaxAlpha;
        private FMinMax _minMaxVerticalMove;
        private FMinMax _minMaxScale;

        private readonly CSwingCounter _counterForAlpha;
        private readonly COnewayCounter _counterForVerticalMove;
        private readonly CSwingCounter _counterForScale;


        // ===============================================
        // MARK: - ● Constructor ─────

        public CRippleActionNormal(
            RippleBehaviour rippleBehaviour,
            float plusMinusDeltaAlpha,
            float plusMinusDeltaVerticalMove,
            float plusMinusScale,
            CSwingCounter swingCounterForAlpha,
            COnewayCounter oneWayCounterForVerticalMove,
            CSwingCounter swingCounterForScale,
            float duration) : base(rippleBehaviour)
        {
            _rippleBehaviour = rippleBehaviour;
            Assert.IsTrue(rippleBehaviour != null);

            _defaultAlpha = Config.BACKGROUND_RIPPLE__ALPHA_DEFAULT;

            _minMaxAlpha = new(
                min: _defaultAlpha - plusMinusDeltaAlpha,
                max: _defaultAlpha + plusMinusDeltaAlpha,
                hasMin: true,
                hasMax: true);


            _originalPosition = new Vector2(x: 0, y: 0);

            _minMaxVerticalMove = new(
                min: _originalPosition.x - plusMinusDeltaVerticalMove,
                max: _originalPosition.x + plusMinusDeltaVerticalMove,
                hasMin: true,
                hasMax: true);


            _originalScale = rippleBehaviour.transform.localScale.x;

            _minMaxScale = new(
                min: _originalScale - plusMinusScale,
                max: _originalScale + plusMinusScale,
                hasMin: true,
                hasMax: true);


            _duration = duration;

            _counterForAlpha = swingCounterForAlpha.Copy as CSwingCounter;
            _counterForVerticalMove = oneWayCounterForVerticalMove.Copy as COnewayCounter;
            _counterForScale = swingCounterForScale.Copy as CSwingCounter;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            // do nothing
        }


        protected override void ProcessMain()
        {
            float elapsedTimeAlphaA = _counterForAlpha.Alpha;
            ChangeAlpha(elapsedTimeAlpha: elapsedTimeAlphaA);

            float elapsedTimeAlphaB = _counterForVerticalMove.Alpha;
            DoVerticalMove(elapsedTimeAlpha: elapsedTimeAlphaB);

            float elapsedTimeAlphaC = _counterForScale.Alpha;
            ChangeScale(elapsedTimeAlpha: elapsedTimeAlphaC);


            _counterForAlpha.Up();
            _counterForVerticalMove.Up();
            _counterForScale.Up();


            // 時間を経過させないことで、アニメーションを永続化している
            _elapsedTime = 0.0f;
        }


        private void ChangeAlpha(float elapsedTimeAlpha) =>
            _rippleBehaviour.SetAlpha(
                alpha: Interpole.SinCurveInOut(
                    _minMaxAlpha.Min,
                    _minMaxAlpha.Max,
                    elapsedTimeAlpha));


        private void DoVerticalMove(float elapsedTimeAlpha)
        {
            var minMax = new FMinMax(min: 0.0f, max: 3.0f, hasMin: true, hasMax: true);
            float alpha;


            switch (Locator.Instance.backgroundManager.GetBackgroundFlowType())
            {
                case BackgroundBehaviour.EBackgroundFlowType.NONE:
                    return;

                case BackgroundBehaviour.EBackgroundFlowType.UP:
                    _counterForVerticalMove.SetIsForwardDirection(value: true);
                    alpha = Interpole.SinCurveOut(minMax.Min, minMax.Max, elapsedTimeAlpha);
                    break;

                case BackgroundBehaviour.EBackgroundFlowType.DOWN:
                    _counterForVerticalMove.SetIsForwardDirection(value: false);
                    alpha = Interpole.SinCurveIn(minMax.Min, minMax.Max, elapsedTimeAlpha);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            
            _rippleBehaviour.SetLocalPositon(unitY: alpha);
        }


        private void ChangeScale(float elapsedTimeAlpha)
        {
            float alpha = Interpole.SinCurveInOut(_minMaxScale.Min, _minMaxScale.Max, elapsedTimeAlpha);

            _rippleBehaviour.SetLocalScaleOfWhole(localScale: new Vector2(x: alpha, y: alpha));
        }


        protected override void ProcessOnLastFrame() =>
            _rippleBehaviour = null;


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

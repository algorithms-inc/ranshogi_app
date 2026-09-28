using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CCheckBoxActionSelect : CAction
    {
        // ===============================================
        // MARK: - ● Enum ─────

        public enum ECheckBoxAnimationType
        {
            SELECT_ON,
            SELECT_OFF
        }



        // ===============================================
        // MARK: - ● Property ─────

        private CheckBoxBehaviour _checkBoxBehaviour;

        private readonly ECheckBoxAnimationType _animationType;

        private readonly float _baseScale;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CCheckBoxActionSelect(
            CheckBoxBehaviour checkBoxBehaviour,
            float duration,
            ECheckBoxAnimationType animationType) : base(checkBoxBehaviour)
        {
            _checkBoxBehaviour = checkBoxBehaviour;
            Assert.IsTrue(_checkBoxBehaviour != null);

            _duration = duration;
            _animationType = animationType;
            _baseScale = _checkBoxBehaviour.MarkSingleImageActionBehaviour.gameObject.transform.localScale.x;

            if (_animationType == ECheckBoxAnimationType.SELECT_ON)
            {
                // マークを ON 状態の画像に切り替える
                _checkBoxBehaviour.UpdateTextureForMark();
            }


            // タップを無効に
            _checkBoxBehaviour.IsTapBlockedByAnimation = true;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            // do nothing
        }


        protected override void ProcessMain() => Fade(elapsedTimeAlpha: GetElapsedTimeAlpha());


        private void Fade(float elapsedTimeAlpha)
        {
            switch (_animationType)
            {
                case ECheckBoxAnimationType.SELECT_ON:
                    float scaleA = Interpole.BounceUpDown(baseValue: 1f, peakValue: 1.4f, elapsedTimeAlpha);

                    _checkBoxBehaviour.MarkSingleImageActionBehaviour.gameObject.transform.localScale = new Vector3(_baseScale * scaleA, _baseScale * scaleA, 1);
                    break;

                case ECheckBoxAnimationType.SELECT_OFF:
                    float alpha = Interpole.SinCurveInStrong(1.0f, 0.5f, elapsedTimeAlpha);
                    UtlSprite.SetSpriteAlpha(gameObject: _checkBoxBehaviour.MarkSingleImageActionBehaviour.gameObject, alpha: alpha);
                    float scaleB = Interpole.BounceUpDown(baseValue: 1f, peakValue: 1.3f, elapsedTimeAlpha);
                    _checkBoxBehaviour.MarkSingleImageActionBehaviour.gameObject.transform.localScale = new Vector3(_baseScale * scaleB, _baseScale * scaleB, 1);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }


        protected override void ProcessOnLastFrame()
        {
            if (_animationType == ECheckBoxAnimationType.SELECT_OFF)
            {
                // マークを OFF 状態の画像に切り替える
                _checkBoxBehaviour.UpdateTextureForMark();

                // αを元に戻す
                UtlSprite.SetSpriteAlpha(gameObject: _checkBoxBehaviour.MarkSingleImageActionBehaviour.gameObject, alpha: 1.0f);
            }

            // タップを有効に
            _checkBoxBehaviour.IsTapBlockedByAnimation = false;

            _checkBoxBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

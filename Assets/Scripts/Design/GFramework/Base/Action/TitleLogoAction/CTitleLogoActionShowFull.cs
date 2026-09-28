using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO: unit の値を正しく設定する。

namespace GFramework
{
    public class CTitleLogoActionShowFull : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private TitleLogoBehaviour _titleLogoBehaviour;


        // ===============================================
        // MARK: - ● Constructor ─────

        public CTitleLogoActionShowFull(
            TitleLogoBehaviour titleLogoBehaviour,
            float duration) : base(titleLogoBehaviour)
        {
            _titleLogoBehaviour = titleLogoBehaviour;
            Assert.IsTrue(_titleLogoBehaviour != null);
            _duration = duration;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            _titleLogoBehaviour.SetAlpha(alpha: 0f);

            UtlSprite.ChangeSpriteBlendMode(gameObject: _titleLogoBehaviour.SingleImageActionBehaviourLogoBody.gameObject, spriteBlendMode: ESpriteBlendMode.ADD);

            _titleLogoBehaviour.gameObject.transform.localScale = new Vector2(1.0f, 1.0f);
        }


        protected override void ProcessMain()
        {
            var elapsedAlpha = GetElapsedTimeAlpha();

            _titleLogoBehaviour.SetAlpha(alpha: elapsedAlpha);

            var scaleValue = Interpole.SinCurveIn(begin: 0.98f, end: 1f, alpha: elapsedAlpha);
            _titleLogoBehaviour.gameObject.transform.localScale = new Vector2(scaleValue, scaleValue);
        }


        protected override void ProcessOnLastFrame()
        {
            _titleLogoBehaviour.SetAlpha(alpha: 1f);

            UtlSprite.ChangeSpriteBlendMode(
                gameObject: _titleLogoBehaviour.SingleImageActionBehaviourLogoBody.gameObject,
                spriteBlendMode: ESpriteBlendMode.NORMAL);

            _titleLogoBehaviour.gameObject.transform.localScale = new Vector2(1.0f, 1.0f);
        

            _titleLogoBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CActionFade : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private readonly FActionData _actionData;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CActionFade(
            ActionBehaviour actionImageBehaviour,
            FActionData actionData,
            float duration) : base(actionImageBehaviour)
        {
            _actionData = actionData;
            Assert.IsTrue(_actionData.GameObjectWithSprite);

            _duration = duration;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            // Do Nothing
        }


        protected override void ProcessMain() => 
            Fade(elapsedTimeAlpha: GetElapsedTimeAlpha());


        private void Fade(float elapsedTimeAlpha)
        {
            float alpha = Interpole.SinCurveInStrong(_actionData.BeginValue, _actionData.EndValue, elapsedTimeAlpha);

            if (_actionData.GameObjectWithSprite)
            {
                UtlSprite.SetSpriteAlpha(gameObject: _actionData.GameObjectWithSprite, alpha: alpha);
            }
        }


        protected override void ProcessOnLastFrame()
        {
            // Do Nothing
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }



    public class CActionFadeMulti : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private readonly List<FActionData> _actionDataList;


        // ===============================================
        // MARK: - ● Constructor ─────

        public CActionFadeMulti(
            ActionBehaviour actionBehaviour,
            List<FActionData> actionDataList,
            float duration) : base(actionBehaviour)
        {
            _actionDataList = actionDataList;
            _duration = duration;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            // Do Nothing
        }


        protected override void ProcessMain() => Fade(elapsedTimeAlpha: GetElapsedTimeAlpha());


        private void Fade(float elapsedTimeAlpha)
        {
            foreach (var i in _actionDataList)
            {
                float alpha = Interpole.SinCurveInStrong(begin: i.BeginValue, end: i.EndValue, alpha: elapsedTimeAlpha);

                if (i.GameObjectWithSprite)
                {
                    UtlSprite.SetSpriteAlpha(gameObject: i.GameObjectWithSprite, alpha: alpha);
                }
            } 
        }


        protected override void ProcessOnLastFrame()
        {
            // Do Nothing
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CPieceCountActionChangeCount : CAction
    {
        // ===============================================
        // MARK: - ● Enum ─────

        public enum EChangeCountType
        {
            INCREMENT,
            DECREMENT
        }



        // ===============================================
        // MARK: - ● Property ─────

        private PieceCountBehaviour _pieceCountBehaviour;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CPieceCountActionChangeCount(
            ActionBehaviour actionBehaviour,
            float duration,
            EChangeCountType changeCountType) : base(actionBehaviour)
        {
            _pieceCountBehaviour = actionBehaviour as PieceCountBehaviour;
            Assert.IsTrue(_pieceCountBehaviour != null);

            _duration = duration;

            // 特に処理は行なっていない
            switch (changeCountType)
            {
                case EChangeCountType.INCREMENT:
                    break;

                case EChangeCountType.DECREMENT:
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            // do nothing
        }


        protected override void ProcessMain()
        {
            float elapsedTimeAlpha = GetElapsedTimeAlpha();

            Fade(elapsedTimeAlpha: elapsedTimeAlpha);
        }


        private void Fade(float elapsedTimeAlpha)
        {
            Vector3 color = Interpole.SinCurveInStrong(begin: new Vector3(x: 1.0f, y: 0.5f, z: 0.0f), end: new Vector3(x: 0.5f, y: 0.5f, z: 0.5f), elapsedTimeAlpha);

            UtlSprite.ChangeColor(gameObject: _pieceCountBehaviour.GameObjectPieceCount, color: color);


            float scale = Interpole.BounceUpDown(baseValue: 1f, peakValue: 1.4f, elapsedTimeAlpha);

            _pieceCountBehaviour.GameObjectPieceCount.transform.localScale = new Vector3(scale, scale, 1);
        }


        protected override void ProcessOnLastFrame()
        {
            _pieceCountBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CPieceActionFlip : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private PieceBehaviour _pieceBehaviour;
        private readonly float _executionTimingAlpha;
        private bool _isExecuted = false;


        // ===============================================
        // MARK: - ● Constructor ─────

        public CPieceActionFlip(
            ActionBehaviour actionImageBehaviour,
            float duration,
            float executionTimingAlpha) : base(actionImageBehaviour)
        {
            _pieceBehaviour = actionImageBehaviour as PieceBehaviour;

            // 成ることができる駒であることを確認
            Assert.IsTrue(Utl.CanBeNari(pieceType: _pieceBehaviour.PieceState.PieceType));

            _duration = duration;
            _executionTimingAlpha = executionTimingAlpha;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            // do nothing
        }


        protected override void ProcessMain()
        {
            if (_duration == 0 || (_isExecuted == false && GetElapsedTimeAlpha() >= _executionTimingAlpha))
            {
                Flip();
                _isExecuted = true;
            }
        }


        private void Flip() => _pieceBehaviour.UpdateTexture();


        protected override void ProcessOnLastFrame() =>
            _pieceBehaviour = null;


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public class GradeRankAction : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private GameResultGradeRankIconBehaviour _gameResultGradeRankIconBehaviour;

        private readonly EGradeRankType _previousGradeRankType;
        private readonly EGradeRankType _nextGradeRankType;

        private readonly float _delay = 1.5f;
        private readonly float _trueDuration = 0.5f;

        private bool _isAlreadyChangeGradeRankCharacter = false;

        private float _originalScaleX = default!;



        // ===============================================
        // MARK: - ● Constructor ─────

        public GradeRankAction(
            GameResultGradeRankIconBehaviour gameResultGradeRankIconBehaviour,
            EGradeRankType previousGradeRankType,
            EGradeRankType nextGradeRankType) : base(gameResultGradeRankIconBehaviour)
        {
            _gameResultGradeRankIconBehaviour = gameResultGradeRankIconBehaviour;
            _previousGradeRankType = previousGradeRankType;
            _nextGradeRankType = nextGradeRankType;

            _duration = _delay + _trueDuration;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame() =>
            _originalScaleX = _gameResultGradeRankIconBehaviour.GetScaleX();


        protected override void ProcessMain()
        {
            if ((int)_previousGradeRankType == (int)_nextGradeRankType) { return; }

            
            var elapsedTimeAlpha = GetElapsedTimeAlphaBetween(min: _delay, max: _duration);

            if (elapsedTimeAlpha == 0.0f)
            {
                return;
            }
            else
            {
                if (_isAlreadyChangeGradeRankCharacter == false)
                {
                    _gameResultGradeRankIconBehaviour.ChangeRankGradeCharacterAsNext();
                    _isAlreadyChangeGradeRankCharacter = true;
                }


                float scaleParameter;

                if ((int)_previousGradeRankType < (int)_nextGradeRankType)
                {
                    // グレードアップ
                    scaleParameter = 4f;
                }
                else
                {
                    // グレードダウン
                    scaleParameter = 0.1f;
                }

                var valueScaleX = Interpole.SinCurveInOut((float)_originalScaleX * scaleParameter, (float)_originalScaleX, elapsedTimeAlpha);
                var valueAlpha = Interpole.SinCurveOut(0f, 1f, elapsedTimeAlpha);
                _gameResultGradeRankIconBehaviour.UpdateAnimation(scale: new Vector2(valueScaleX, valueScaleX), alpha: valueAlpha);
            }
        }


        protected override void ProcessOnLastFrame()
        {
            // 誤差で最終値がでないことがあるため、強制表示
            _gameResultGradeRankIconBehaviour.UpdateAnimation(scale: new Vector2(_originalScaleX, _originalScaleX), alpha: 1f);

            _gameResultGradeRankIconBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

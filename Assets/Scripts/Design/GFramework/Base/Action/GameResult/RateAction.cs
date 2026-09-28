using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class RateAction : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private GameResultRateInfoBehaviour _gameResultRateInfoBehaviour;

        private readonly int _beginNum;
        private readonly int _endNum;



        // ===============================================
        // MARK: - ● Constructor ─────

        public RateAction(
            GameResultRateInfoBehaviour gameResultRateInfoBehaviour,
            int beginNum,
            int endNum) : base(gameResultRateInfoBehaviour)
        {
            _gameResultRateInfoBehaviour = gameResultRateInfoBehaviour;
            _beginNum = beginNum;
            _endNum = endNum;

            
            float diff = Math.Abs(_endNum - _beginNum);
            _duration = diff switch
            {
                > 150 => 2.5f,
                > 100 => 2f,
                > 50  => 1.5f,
                _     => 1f
            };
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            // do nothing
        }


        protected override void ProcessMain()
        {
            var elapsedTimeAlpha = GetElapsedTimeAlpha();
            var num = CalcNum(elapsedTimeAlpha: elapsedTimeAlpha);

            _gameResultRateInfoBehaviour.ChangeRate(rate: num);
        }


        private int CalcNum(float elapsedTimeAlpha) => 
            (int)Interpole.SinCurveOut(begin: (float)_beginNum, end: (float)_endNum, alpha: elapsedTimeAlpha);


        protected override void ProcessOnLastFrame()
        {
            // 誤差で最終値がでないことがあるため、強制表示
            _gameResultRateInfoBehaviour.ChangeRate(rate: _endNum);

            _gameResultRateInfoBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

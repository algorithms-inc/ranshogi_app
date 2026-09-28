using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public readonly struct FGameResultData
    {
        private readonly String _namePlayer1;
        public String NamePlayer1 => _namePlayer1;

        private readonly String _namePlayer2;
        public String NamePlayer2 => _namePlayer2;

        private readonly EMoveOrderType _moveOrderTypePlayer1;
        public EMoveOrderType MoveOrderTypePlayer1 => _moveOrderTypePlayer1;


        private readonly EGameResultType _gameResultType;
        public EGameResultType GameResultType => _gameResultType;

        private readonly EGameResultReasonType _gameResultReasonType;
        public EGameResultReasonType GameResultReasonType => _gameResultReasonType;

        private readonly int _consecutiveCount;
        public int ConsecutiveCount => _consecutiveCount;

        private readonly int _previousRate;
        public int PreviousRate => _previousRate;

        private readonly int _nextRate;
        public int NextRate => _nextRate;

        private readonly EGradeRankType _previousGradeRankType;
        public EGradeRankType PreviousGradeRankType => _previousGradeRankType;

        private readonly EGradeRankType _nextGradeRankType;
        public EGradeRankType NextGradeRankType => _nextGradeRankType;


        public FGameResultData(
            string namePlayer1,
            string namePlayer2,
            EMoveOrderType moveOrderTypePlayer1,
            EGameResultType gameResultType,
            EGameResultReasonType gameResultReasonType,
            int consecutiveCount,
            int previousRate,
            int nextRate,
            EGradeRankType previousGradeRankType,
            EGradeRankType nextGradeRankType)
        {
            _namePlayer1 = namePlayer1;
            _namePlayer2 = namePlayer2;
            _moveOrderTypePlayer1 = moveOrderTypePlayer1;
            _gameResultType = gameResultType;
            _gameResultReasonType = gameResultReasonType;
            _consecutiveCount = consecutiveCount;
            _previousRate = previousRate;
            _nextRate = nextRate;
            _previousGradeRankType = previousGradeRankType;
            _nextGradeRankType = nextGradeRankType;
        }


        public int GetDeltaRate()
        {
            return _nextRate - _previousRate;
        }
    }
}
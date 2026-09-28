using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework.Core
{
    public readonly struct FPlayerData
    {
        private readonly string _playerName;
        public readonly string PlayerName => _playerName;

        private readonly int _rate;
        public readonly int Rate => _rate;

        private readonly EGradeRankType _gradeRankType;
        public readonly EGradeRankType GradeRankType => _gradeRankType;

        private readonly Color _color;
        public readonly Color Color => _color;


        public FPlayerData(
            string playerName,
            int rate,
            EGradeRankType gradeRankType,
            Color color)
        {
            _playerName = playerName;
            _rate = rate;
            _gradeRankType = gradeRankType;
            _color = color;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public struct FPlayerDataForFramework
    {
        private readonly string _playerName;
        public readonly string PlayerName => _playerName;

        private readonly int _rate;
        public readonly int Rate => _rate;

        private readonly EGradeRankType _gradeRankType;
        public readonly EGradeRankType GradeRankType => _gradeRankType;

        private readonly List<FPiecePlaceData> _piecePlaceDataList;
        public readonly List<FPiecePlaceData> PiecePlaceDataList => _piecePlaceDataList;

        private readonly EMoveOrderType _moveOrderType;
        public readonly EMoveOrderType MoveOrderType => _moveOrderType;


        public FPlayerDataForFramework(
            string playerName,
            int rate,
            EGradeRankType gradeRankType,
            List<FPiecePlaceData> piecePlaceDataList,
            EMoveOrderType moveOrderType)
        {
            _playerName = playerName;
            _rate = rate;
            _gradeRankType = gradeRankType;
            _piecePlaceDataList = piecePlaceDataList;
            _moveOrderType = moveOrderType;

            Validate();
        }


        private void Validate()
        {
            // 少なくとも１つの王がなければならない(兼：駒が一つはなければならない)
            Assert.IsTrue(HasOnlyOneOushou());

            // 二歩状態ではない
            Assert.IsTrue(HasNotNiFu());

            // 同じマスに複数の駒が置かれてはいないかは、Player1 2 合わせて確認するため、ここでは行わない。
        }


        private readonly bool HasOnlyOneOushou()
        {
            int count = 0;

            foreach (FPiecePlaceData i in _piecePlaceDataList)
            {
                if (i.PieceType == EPieceType.OUSHOU)
                {
                    count += 1;
                }
            }

            return count == 1;
        }


        private readonly bool HasNotNiFu()
        {
            bool[] hasFuInColumn = new bool[9];


            foreach (FPiecePlaceData i in _piecePlaceDataList)
            {
                if (i.PieceType == EPieceType.FUHYOU && i.IsOnBoard && i.IsNari == false)
                {
                    if (hasFuInColumn[i.BoardCrd.X - 1] == true)
                    {
                        return false;
                    }
                    else
                    {
                        hasFuInColumn[i.BoardCrd.X - 1] = true;
                    }
                }
            }


            return true;
        }
    }
}
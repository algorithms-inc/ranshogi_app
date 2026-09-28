using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public struct FGameStageInitializer
    {
        private FPlayerDataForFramework _playerData1;
        private FPlayerDataForFramework _playerData2;

        private EPlayerType _gyokushouPlayer;

        private readonly EBoardType _boardType;
        public EBoardType BoardType => _boardType;


        public FPlayerDataForFramework PlayerData1 => _playerData1;
        public FPlayerDataForFramework PlayerData2 => _playerData2;
        public EPlayerType GyokushouPlayer => _gyokushouPlayer;


        public FGameStageInitializer(
            EBoardType boardType,
            FPlayerDataForFramework playerData1,
            FPlayerDataForFramework playerData2)
        {
            _boardType = boardType;

            _playerData1 = playerData1;
            _playerData2 = playerData2;

            
            // 仮置
            _gyokushouPlayer = EPlayerType.PLAYER1;

            Validate();

            Setup();
        }


        private void Validate()
        {
            Assert.IsTrue(HasNotTwoPieceOnSameCell());
            Assert.IsTrue(IsNotSameMoveOrderType());
        }


        private bool HasNotTwoPieceOnSameCell()
        {
            bool[] hasPieceInCell;

            if (_boardType == EBoardType.BOARD_66)
            {
                hasPieceInCell = new bool[36];
            }
            else
            {
                // EBoardType.BOARD_99
                hasPieceInCell = new bool[81];
            }


            bool Support(List<FPiecePlaceData> piecePlaceDataList, EBoardType boardType)
            {
                foreach (FPiecePlaceData i in piecePlaceDataList)
                {
                    if (i.BoardCrd.IsOnBoard(boardType: boardType))
                    {
                        if (hasPieceInCell[i.BoardCrd.AsIndex(boardType: boardType)] == true)
                        {
                            return false;
                        }
                        else
                        {
                            hasPieceInCell[i.BoardCrd.AsIndex(boardType: boardType)] = true;
                        }
                    }
                }

                return true;
            }


            if (Support(piecePlaceDataList: _playerData1.PiecePlaceDataList, boardType: _boardType) == false) { return false; }
            if (Support(piecePlaceDataList: _playerData2.PiecePlaceDataList, boardType: _boardType) == false) { return false; }


            return true;
        }


        private bool IsNotSameMoveOrderType()
        {
            return _playerData1.MoveOrderType != _playerData2.MoveOrderType;
        }


        private void Setup()
        {
            CalcGyokushouPlayer();
        }


        /// <summary>
        /// 玉将側のプレイヤーを決める
        /// </summary>
        /// <note>
        /// スコアが低いほうが玉将となる。
        /// スコアが同じ場合、playerIDが大きいほうが玉将となる。
        /// </note>
        private void CalcGyokushouPlayer()
        {
            if (_playerData1.Rate < _playerData2.Rate)
            {
                _gyokushouPlayer = EPlayerType.PLAYER1;
                return;
            }

            if (_playerData1.Rate > _playerData2.Rate)
            {
                _gyokushouPlayer = EPlayerType.PLAYER2;
                return;
            }

            // case: playerData1.Score == playerData2

            _gyokushouPlayer = EPlayerType.PLAYER2;
        }

    }
}
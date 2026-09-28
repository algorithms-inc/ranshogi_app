using System;
using GFramework.Core;
using UnityEngine.Assertions;

namespace GFramework
{
    public struct FBoardDiffBoardDataElement
    {
        private readonly EBoardType _boardType;
        public readonly EBoardType BoardType => _boardType;

        private readonly EPlayerType _playerType;
        public readonly EPlayerType PlayerType => _playerType;

        private readonly FBoardCrd _crd;
        public readonly FBoardCrd Crd => _crd;

        private readonly EPieceType _pieceType;
        public readonly EPieceType PieceType => _pieceType;

        private readonly bool _isNari;
        public readonly bool IsNari => _isNari;


        public readonly string AsString()
        {
            var playerTypeString = _playerType == EPlayerType.PLAYER1 ? "P1" : "P2";
            var stringIsNari = _isNari ? "成" : "";
            return playerTypeString + " " + _crd + " " + stringIsNari + _pieceType.ToJapaneseString();
        }

        public FBoardDiffBoardDataElement(
            EBoardType boardType,
            EPlayerType playerType,
            FBoardCrd crd,
            EPieceType pieceType,
            bool isNari)
        {
            _boardType = boardType;
            _playerType = playerType;
            _crd = crd;
            _pieceType = pieceType;
            _isNari = isNari;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public struct FPieceState
    {
        // Member Variable


        private EPlayerType _playerType;
        public EPlayerType PlayerType => _playerType;

        private readonly EPieceType _pieceType;
        public EPieceType PieceType => _pieceType;

        private readonly bool _isGyokushou;
        public bool IsGyokushou => _isGyokushou;

        private bool _isNari;
        public bool IsNari => _isNari;


        // Constructor
        public FPieceState(EPlayerType playerType, EPieceType pieceType, bool isGyokushou, bool isNari)
        {
            _playerType = playerType;
            _pieceType = pieceType;
            _isGyokushou = isGyokushou;
            _isNari = isNari;

            if (isGyokushou)
            {
                Assert.IsTrue(Utl.CanBeGyokushou(pieceType: pieceType));
            }

            if (isNari)
            {
                Assert.IsTrue(Utl.CanBeNari(pieceType: pieceType));
            }
        }


        public void ChangeToNari()
        {
            Assert.IsTrue(_isNari == false);
            Assert.IsTrue(Utl.CanBeNari(pieceType: _pieceType));

            _isNari = true;
        }


        public void ChangePlayerTypeToOppositeTypeAndSetNotNariIfNeeded()
        {
            Assert.IsTrue(_isGyokushou == false);

            ChangePlayerTypeToOpposite();

            ChangeToNotNariIfNeeded();
        }


        private void ChangePlayerTypeToOpposite()
        {
            _playerType = (_playerType == EPlayerType.PLAYER1) ? EPlayerType.PLAYER2 : EPlayerType.PLAYER1;
        }

        private void ChangeToNotNariIfNeeded()
        {
            _isNari = false;
        }
    }
}
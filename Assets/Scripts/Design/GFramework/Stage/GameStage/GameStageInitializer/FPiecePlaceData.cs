using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public struct FPiecePlaceData
    {
        private EPieceType _pieceType;
        public EPieceType PieceType => _pieceType;

        private bool _isNari;
        public bool IsNari => _isNari;

        private FBoardCrd _boardCrd;
        public FBoardCrd BoardCrd => _boardCrd;

        private EBoardType _boardType;
        public bool IsOnBoard => _boardCrd.IsOnBoard(boardType: _boardType);
        
        
        public FPiecePlaceData(
            EPieceType pieceType,
            bool isNari,
            EBoardType boardType,
            FBoardCrd boardCrd)
        {
            _pieceType = pieceType;
            _isNari = isNari;
            _boardType = boardType;
            _boardCrd = boardCrd;

            Validate();
        }

        private void Validate()
        {
            if (_isNari)
            {
                // 本当に成ることが出来る駒であるか
                Assert.IsTrue(Utl.CanBeNari(pieceType: _pieceType));

                // 成り駒は盤上に置かれなければならない
                Assert.IsTrue(_boardCrd.IsOnBoard(boardType: _boardType));
            }

            if (_pieceType == EPieceType.OUSHOU)
            {
                // 王は盤上に置かれなければならない
                Assert.IsTrue(_boardCrd.IsOnBoard(boardType: _boardType));
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GFramework.Core 
{
    public struct FPieceAndSquare
    {
        private readonly PieceType _pieceType;
        public PieceType PieceType => _pieceType;

        private Square _square;
        public Square Square => _square;

        public readonly bool IsOnBoard => _square != Square.SQ_Invalid;
        public readonly bool IsOnWaitingSpace => _square == Square.SQ_Invalid;


        public FPieceAndSquare(
            PieceType pieceType,
            Square square)
        {
            _pieceType = pieceType;
            _square = square;
        }
    }
}
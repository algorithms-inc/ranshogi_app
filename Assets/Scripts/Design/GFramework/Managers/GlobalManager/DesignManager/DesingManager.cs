using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public sealed class DesignManager : IDesignManager
    {
        private bool _isShowNumOnBoard = true;
        private bool _isChangeBackground = true;
        private bool _isShowLastMove = true;
        private int _designIDOfPiceFront = 0;
        private int _designIDOfPieceBackForGroupRook = 0;
        private int _designIDOfPieceBackForGroupSilver = 0;

        private readonly int _maxDesingIDOfPieceFront = 1;
        private readonly int _maxDesingIDOfPieceBackForGroupRook = 2;
        private readonly int _maxDesingIDOfPieceBackForGroupSilver = 2;


        // ===============================================
        // MARK: - ● Constructor ─────

        public DesignManager()
        {
            
        }



        public void Destroy()
        {
            // do nothing
        }


        // ------------------------------------
        // Core に公開されているもの

        public bool IsShowNumOnBoard()
        {
            return _isShowNumOnBoard;
        }

        public bool IsChangeBackground()
        {
            return _isChangeBackground;
        }

        public bool IsShowLastMove()
        {
            return _isShowLastMove;
        }

        public int GetDesignIDOfPiceFront()
        {
            return _designIDOfPiceFront;
        }
        
        public int GetDesignIDOfPieceBackForGroupRook()
        {
            return _designIDOfPieceBackForGroupRook;
        }

        public int GetDesignIDOfPieceBackForGroupSilver()
        {
            return _designIDOfPieceBackForGroupSilver;
        }

        public void SetIsShowNumOnBoard(bool value)
        {
            _isShowNumOnBoard = value;
        }

        public void SetIsChangeBackground(bool value)
        {
            _isChangeBackground = value;
        }

        public void SetIsShowLastMove(bool value)
        {
            _isShowLastMove = value;
        }

        public void SetDesignIDOfPieceFront(int value)
        {
            Assert.IsTrue(IsValidDesignIDOfPieceFront(value));

            _designIDOfPiceFront = value;
        }

        public void SetDesignIDOfPieceBackForGroupRook(int value)
        {
            Assert.IsTrue(IsValidDesignIDOfPieceBackForGroupRook(value));

            _designIDOfPieceBackForGroupRook = value;
        }

        public void SetDesignIDOfPieceBackForGroupSilver(int value)
        {
            Assert.IsTrue(IsValidDesignIDOfPieceBackForGroupSilver(value));

            _designIDOfPieceBackForGroupSilver = value;
        }


        // ------------------------------------
        // Validation

        private bool IsValidDesignIDOfPieceFront(int value)
        {
            return (0 <= value && value <= _maxDesingIDOfPieceFront);
        }

        private bool IsValidDesignIDOfPieceBackForGroupRook(int value)
        {
            return (0 <= value && value <= _maxDesingIDOfPieceBackForGroupRook);
        }

        private bool IsValidDesignIDOfPieceBackForGroupSilver(int value)
        {
            return (0 <= value && value <= _maxDesingIDOfPieceBackForGroupSilver);
        }
    }
}

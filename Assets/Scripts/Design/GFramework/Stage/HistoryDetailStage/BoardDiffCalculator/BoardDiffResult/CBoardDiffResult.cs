using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CBoardDiffResult
    {
        private readonly CBoardDiffResultForBoard _resultBoard = new CBoardDiffResultForBoard();
        public CBoardDiffResultForBoard ResultBoard => _resultBoard;

        private readonly CBoardDiffResultForWaitingSpace _resultWaitingSpacePlayer1 = new CBoardDiffResultForWaitingSpace();
        public CBoardDiffResultForWaitingSpace ResultWaitingSpacePlayer1 => _resultWaitingSpacePlayer1;

        private readonly CBoardDiffResultForWaitingSpace _resultWaitingSpacePlayer2 = new CBoardDiffResultForWaitingSpace();
        public CBoardDiffResultForWaitingSpace ResultWaitingSpacePlayer2 => _resultWaitingSpacePlayer2;

        public CBoardDiffResult(
            CBoardDiffResultForBoard resultBoard,
            CBoardDiffResultForWaitingSpace resultWaitingSpacePlayer1,
            CBoardDiffResultForWaitingSpace resultWaitingSpacePlayer2)
        {
            _resultBoard = resultBoard;
            _resultWaitingSpacePlayer1 = resultWaitingSpacePlayer1;
            _resultWaitingSpacePlayer2 = resultWaitingSpacePlayer2;
        }
    }


    public class CBoardDiffResultForBoard
    {
        public List<FBoardDiffBoardDataElement> RemoveList = new List<FBoardDiffBoardDataElement>();
        public List<FBoardDiffBoardDataElement> AddList = new List<FBoardDiffBoardDataElement>();

        public List<string> AsStringList()
        {
            var resultList = new List<string>();

            foreach (var data in RemoveList)
            {
                resultList.Add("Remove: " + data.AsString());
            }

            foreach (var data in AddList)
            {
                resultList.Add("Add: " + data.AsString());
            }

            return resultList;
        }

        public string AsString()
        {
            return string.Join(" ", AsStringList());
        }

        public CBoardDiffResultForBoard()
        {
            // do nothing
        }
    }


    public struct FBoardDiffResultForWaitingSpaceElement
    {
        public EPieceType PieceType;
        public int Delta;

        public string AsString()
        {
            return "" + PieceType.ToJapaneseString() + "(" + Delta + ")";
        }

        public FBoardDiffResultForWaitingSpaceElement(EPieceType pieceType, int delta)
        {
            PieceType = pieceType;
            Delta = delta;
        }
    }
}
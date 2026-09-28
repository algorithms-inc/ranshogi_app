using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class TestCaseBoardCrd : ITestCase
    {
        public void ShowBeginLog()
        {
            Debug.Log("Test: " + GetType().Name + ": Begin");
        }

        public void ShowEndLog()
        {
            Debug.Log("Test: " + GetType().Name + ": End");
        }

        public void Execute()
        {
            ShowBeginLog();

            TestConstructor();

            ShowEndLog();
        }


        private void TestConstructor()
        {
            FBoardCrd targetBoardCrd;

            // case: Color.Black

            var boardType = EBoardType.BOARD_66;

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_11, moveOrderType: EMoveOrderType.FIRST);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 1, y: 1));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.FIRST) == Square.SQ_11);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_21, moveOrderType: EMoveOrderType.FIRST);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 2, y: 1));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.FIRST) == Square.SQ_21);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_61, moveOrderType: EMoveOrderType.FIRST);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 6, y: 1));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.FIRST) == Square.SQ_61);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_12, moveOrderType: EMoveOrderType.FIRST);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 1, y: 2));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.FIRST) == Square.SQ_12);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_16, moveOrderType: EMoveOrderType.FIRST);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 1, y: 6));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.FIRST) == Square.SQ_16);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_66, moveOrderType: EMoveOrderType.FIRST);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 6, y: 6));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.FIRST) == Square.SQ_66);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_Invalid, moveOrderType: EMoveOrderType.FIRST);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 0, y: 0));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.FIRST) == Square.SQ_Invalid);


            // case: Color.White

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_11, moveOrderType: EMoveOrderType.SECOND);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 6, y: 6));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.SECOND) == Square.SQ_11);
            
            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_21, moveOrderType: EMoveOrderType.SECOND);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 5, y: 6));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.SECOND) == Square.SQ_21);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_61, moveOrderType: EMoveOrderType.SECOND);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 1, y: 6));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.SECOND) == Square.SQ_61);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_12, moveOrderType: EMoveOrderType.SECOND);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 6, y: 5));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.SECOND) == Square.SQ_12);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_16, moveOrderType: EMoveOrderType.SECOND);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 6, y: 1));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.SECOND) == Square.SQ_16);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_66, moveOrderType: EMoveOrderType.SECOND);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 1, y: 1));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.SECOND) == Square.SQ_66);

            targetBoardCrd = new FBoardCrd(boardType: boardType, square: Square.SQ_Invalid, moveOrderType: EMoveOrderType.SECOND);
            Assert.IsTrue(targetBoardCrd == new FBoardCrd(x: 0, y: 0));
            Assert.IsTrue(targetBoardCrd.ConvertToSquare(boardType: boardType, moveOrderType: EMoveOrderType.SECOND) == Square.SQ_Invalid);
        }
    }
}
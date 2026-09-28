using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    /// <summary>
    /// 盤の座標管理。TOP-RIGHTを(1,1)-BOTTOM-LEFTを(6,6)とする
    /// </summary>
    public struct FBoardCrd
    {
        private int _x;
        private int _y;

        public int X
        {
            set { _x = value; }
            get => _x;
        }

        public int Y
        {
            set { _y = value; }
            get => _y;
        }

        public static FBoardCrd AsOnWaitingSpace => new FBoardCrd(x: 0, y: 0);

        public override string ToString()
        {
            return "(" + X.ToString() + "," + Y.ToString() + ")";
        }


        // ===============================================
        // MARK: - ● Constructor ─────

        // x = 0, y = 0 のとき駒台上を表す
        public FBoardCrd(int x, int y)
        {
            _x = x;
            _y = y;
        }


        public FBoardCrd(EBoardType boardType, Square square, EMoveOrderType moveOrderType)
        {
            if (square == Square.SQ_Invalid)
            {
                _x = 0;
                _y = 0;
            }
            else
            {
                var (tempX, tempY) = ConvertSquareToXY(square);

                int squareIndex = (int)square;

                if (moveOrderType == EMoveOrderType.FIRST)
                {
                    _x = tempX;
                    _y = tempY;
                }
                else // moveOrderType == EMoveOrderType.SECOND
                {
                    _x = (boardType.GetLineCount() + 1) - tempX;
                    _y = (boardType.GetLineCount() + 1) - tempY;
                }
            }
        }


        private static (int, int) ConvertSquareToXY(Square square)
        {
            return square switch
            {
                Square.SQ_11 => (1, 1),
                Square.SQ_12 => (1, 2),
                Square.SQ_13 => (1, 3),
                Square.SQ_14 => (1, 4),
                Square.SQ_15 => (1, 5),
                Square.SQ_16 => (1, 6),
                Square.SQ_17 => (1, 7),
                Square.SQ_18 => (1, 8),
                Square.SQ_19 => (1, 9),

                Square.SQ_21 => (2, 1),
                Square.SQ_22 => (2, 2),
                Square.SQ_23 => (2, 3),
                Square.SQ_24 => (2, 4),
                Square.SQ_25 => (2, 5),
                Square.SQ_26 => (2, 6),
                Square.SQ_27 => (2, 7),
                Square.SQ_28 => (2, 8),
                Square.SQ_29 => (2, 9),

                Square.SQ_31 => (3, 1),
                Square.SQ_32 => (3, 2),
                Square.SQ_33 => (3, 3),
                Square.SQ_34 => (3, 4),
                Square.SQ_35 => (3, 5),
                Square.SQ_36 => (3, 6),
                Square.SQ_37 => (3, 7),
                Square.SQ_38 => (3, 8),
                Square.SQ_39 => (3, 9),

                Square.SQ_41 => (4, 1),
                Square.SQ_42 => (4, 2),
                Square.SQ_43 => (4, 3),
                Square.SQ_44 => (4, 4),
                Square.SQ_45 => (4, 5),
                Square.SQ_46 => (4, 6),
                Square.SQ_47 => (4, 7),
                Square.SQ_48 => (4, 8),
                Square.SQ_49 => (4, 9),

                Square.SQ_51 => (5, 1),
                Square.SQ_52 => (5, 2),
                Square.SQ_53 => (5, 3),
                Square.SQ_54 => (5, 4),
                Square.SQ_55 => (5, 5),
                Square.SQ_56 => (5, 6),
                Square.SQ_57 => (5, 7),
                Square.SQ_58 => (5, 8),
                Square.SQ_59 => (5, 9),

                Square.SQ_61 => (6, 1),
                Square.SQ_62 => (6, 2),
                Square.SQ_63 => (6, 3),
                Square.SQ_64 => (6, 4),
                Square.SQ_65 => (6, 5),
                Square.SQ_66 => (6, 6),
                Square.SQ_67 => (6, 7),
                Square.SQ_68 => (6, 8),
                Square.SQ_69 => (6, 9),

                Square.SQ_71 => (7, 1),
                Square.SQ_72 => (7, 2),
                Square.SQ_73 => (7, 3),
                Square.SQ_74 => (7, 4),
                Square.SQ_75 => (7, 5),
                Square.SQ_76 => (7, 6),
                Square.SQ_77 => (7, 7),
                Square.SQ_78 => (7, 8),
                Square.SQ_79 => (7, 9),

                Square.SQ_81 => (8, 1),
                Square.SQ_82 => (8, 2),
                Square.SQ_83 => (8, 3),
                Square.SQ_84 => (8, 4),
                Square.SQ_85 => (8, 5),
                Square.SQ_86 => (8, 6),
                Square.SQ_87 => (8, 7),
                Square.SQ_88 => (8, 8),
                Square.SQ_89 => (8, 9),

                Square.SQ_91 => (9, 1),
                Square.SQ_92 => (9, 2),
                Square.SQ_93 => (9, 3),
                Square.SQ_94 => (9, 4),
                Square.SQ_95 => (9, 5),
                Square.SQ_96 => (9, 6),
                Square.SQ_97 => (9, 7),
                Square.SQ_98 => (9, 8),
                Square.SQ_99 => (9, 9),

                Square.SQ_Invalid => (0, 0),
                Square.SQ_NB => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException()
            };
        }


        // ===============================================
        // MARK: - ● Method ─────

        public bool IsOnBoard(EBoardType boardType)
        {
            var lineCount = boardType.GetLineCount();
            return 1 <= _x && _x <= lineCount && 1 <= _y && _y <= lineCount;
        }


        public bool IsOnWaitingSpace(EBoardType boardType)
        {
            return !IsOnBoard(boardType: boardType);
        }


        public Vector2Int AsCrdTopLeftOrigin(EBoardType boardType)
        {
            Assert.IsTrue(IsOnBoard(boardType: boardType));

            var lineCount = boardType.GetLineCount();

            return new Vector2Int(lineCount - _x, _y - 1);
        }


        /// <summary>
        /// RANSHOGI: (1,1) = 0 〜 (6,6) = 35 の index で返す
        /// NORMAL_SHOGI: (1,1) = 0 〜 (9,9) = 80 の index で返す
        /// </summary>
        public int AsIndex(EBoardType boardType)
        {
            Assert.IsTrue(IsOnBoard(boardType: boardType));

            var lineCount = boardType.GetLineCount();

            return lineCount * (_y - 1) + (_x - 1);
        }


        public FBoardCrd AsCrdBottomLeftOrigin(EBoardType boardType)
        {
            Assert.IsTrue(IsOnBoard(boardType: boardType));

            var lineCount = boardType.GetLineCount();

            return new FBoardCrd(lineCount - _x + 1, lineCount - _y + 1);
        }



        // ===============================================
        // MARK: - ● Convert ─────

        public Square ConvertToSquare(EBoardType boardType, EMoveOrderType moveOrderType)
        {
            if (boardType == EBoardType.BOARD_66) {

                if (moveOrderType == EMoveOrderType.FIRST)
                {
                    // (x - 1) * 6 + y - 1 を展開
                    var squareIndex = 6 * _x + _y - 7;
                    return (Square)squareIndex;
                }
                else // moveOrderType == EMoveOrderType.SECOND
                {
                    // (6 - x) * 6 + (6 - y) を展開
                    var squareIndex = 42 - 6 * _x - _y;
                    return (Square)squareIndex;
                }
            } else {
                if (moveOrderType == EMoveOrderType.FIRST)
                {
                    // (x - 1) * 9 + y - 1 を展開
                    var squareIndex = 9 * _x + _y - 10;
                    return (Square)squareIndex;
                }
                else // moveOrderType == EMoveOrderType.SECOND
                {
                    // (9 - x) * 9 + (9 - y) を展開
                    var squareIndex = 90 - 9 * _x - _y;
                    return (Square)squareIndex;
                }
            }
        }



        // ===============================================
        // MARK: - ● Operator: Equatable

        public override bool Equals(object obj)
        {
            return _x == ((FBoardCrd)obj)._x && _y == ((FBoardCrd)obj)._y;
        }


        public override int GetHashCode()
        {
            return 0;
        }


        public static bool operator ==(FBoardCrd left, FBoardCrd right)
        {
            return (left._x == right._x) && (left._y == right._y);
        }


        public static bool operator !=(FBoardCrd left, FBoardCrd right)
        {
            return !(left == right);
        }



        // ===============================================
        // MARK: - ● Operator

        public static FBoardCrd operator +(FBoardCrd lhv, FBoardCrd rhv)
        {
            return new FBoardCrd(lhv._x + rhv._x, lhv._y + rhv._y);
        }

        public static FBoardCrd operator -(FBoardCrd lhv, FBoardCrd rhv)
        {
            return new FBoardCrd(lhv._x - rhv._x, lhv._y - rhv._y);
        }
    }
}
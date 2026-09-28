using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework.Core
{
    public enum EBoardType
    {
        BOARD_66,
        BOARD_99
    }



    public static class EBoardTypeExtensions
    {
        public static int GetLineCount(this EBoardType boardType)
        {
            switch (boardType)
            {
                case EBoardType.BOARD_66:
                    return 6;
                case EBoardType.BOARD_99:
                    return 9;
                default:
                    throw new ArgumentOutOfRangeException(nameof(boardType), boardType, null);
            }
        }


        public static int GetMaxLineIndex(this EBoardType boardType)
        {
            switch (boardType)
            {
                case EBoardType.BOARD_66:
                    return 5;
                case EBoardType.BOARD_99:
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException(nameof(boardType), boardType, null);
            }
        }

        public static string ConvertToString(this EBoardType boardType)
        {
            switch (boardType)
            {
                case EBoardType.BOARD_66:
                    return "乱将棋（6×6）";
                case EBoardType.BOARD_99:
                    return "将棋（9×9）";
                default:
                    throw new ArgumentOutOfRangeException(nameof(boardType), boardType, null);
            }
        }
    }
}
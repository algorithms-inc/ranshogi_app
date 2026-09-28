using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    
    public enum EGradeRankType
    {
        // ===============================================
        // MARK: - ● 定数 ─────

        NONE = 0,
        GRADE_15 = 1,
        GRADE_14 = 2,
        GRADE_13 = 3,
        GRADE_12 = 4,
        GRADE_11 = 5,
        GRADE_10 = 6,
        GRADE_9 = 7,
        GRADE_8 = 8,
        GRADE_7 = 9,
        GRADE_6 = 10,
        GRADE_5 = 11,
        GRADE_4 = 12,
        GRADE_3 = 13,
        GRADE_2 = 14,
        GRADE_1 = 15,
        RANK_1 = 16,
        RANK_2 = 17,
        RANK_3 = 18,
        RANK_4 = 19,
        RANK_5 = 20,
        RANK_6 = 21,
        RANK_7 = 22,
        RANK_8 = 23,
        RANK_9 = 24,
        RANK_10 = 25,
        AI = 99
    }
}

namespace GFramework
{
    public static class EGradeRankTypeExtensions
    {
        public static bool IsGrade(this EGradeRankType gradeRank)
        {
            switch (gradeRank)
            {
                case EGradeRankType.NONE:
                case EGradeRankType.GRADE_15:
                case EGradeRankType.GRADE_14:
                case EGradeRankType.GRADE_13:
                case EGradeRankType.GRADE_12:
                case EGradeRankType.GRADE_11:
                case EGradeRankType.GRADE_10:
                case EGradeRankType.GRADE_9:
                case EGradeRankType.GRADE_8:
                case EGradeRankType.GRADE_7:
                case EGradeRankType.GRADE_6:
                case EGradeRankType.GRADE_5:
                case EGradeRankType.GRADE_4:
                case EGradeRankType.GRADE_3:
                case EGradeRankType.GRADE_2:
                case EGradeRankType.GRADE_1:
                    return true;
                case EGradeRankType.RANK_1:
                case EGradeRankType.RANK_2:
                case EGradeRankType.RANK_3:
                case EGradeRankType.RANK_4:
                case EGradeRankType.RANK_5:
                case EGradeRankType.RANK_6:
                case EGradeRankType.RANK_7:
                case EGradeRankType.RANK_8:
                case EGradeRankType.RANK_9:
                case EGradeRankType.RANK_10:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gradeRank), gradeRank, null);  
            }
        }

        public static string ToJapaneseName(this EGradeRankType gradeRank)
        {
            switch (gradeRank)
            {
                case EGradeRankType.NONE:
                    return "なし";
                case EGradeRankType.GRADE_15:
                    return "15級";
                case EGradeRankType.GRADE_14:
                    return "14級";
                case EGradeRankType.GRADE_13:
                    return "13級";
                case EGradeRankType.GRADE_12:
                    return "12級";
                case EGradeRankType.GRADE_11:
                    return "11級";
                case EGradeRankType.GRADE_10:
                    return "10級";
                case EGradeRankType.GRADE_9:
                    return "９級";
                case EGradeRankType.GRADE_8:
                    return "８級";
                case EGradeRankType.GRADE_7:
                    return "７級";
                case EGradeRankType.GRADE_6:
                    return "６級";
                case EGradeRankType.GRADE_5:
                    return "５級";
                case EGradeRankType.GRADE_4:
                    return "４級";
                case EGradeRankType.GRADE_3:
                    return "３級";
                case EGradeRankType.GRADE_2:
                    return "２級";
                case EGradeRankType.GRADE_1:
                    return "１級";
                case EGradeRankType.RANK_1:
                    return "初段";
                case EGradeRankType.RANK_2:
                    return "二段";
                case EGradeRankType.RANK_3:
                    return "三段";
                case EGradeRankType.RANK_4:
                    return "四段";
                case EGradeRankType.RANK_5:
                    return "五段";
                case EGradeRankType.RANK_6:
                    return "六段";
                case EGradeRankType.RANK_7:
                    return "七段";
                case EGradeRankType.RANK_8:
                    return "八段";
                case EGradeRankType.RANK_9:
                    return "九段";
                case EGradeRankType.RANK_10:
                    return "十段";
                case EGradeRankType.AI:
                    return "AI";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gradeRank), gradeRank, null);
            }
        }
    }
}
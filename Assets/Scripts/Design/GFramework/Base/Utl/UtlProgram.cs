using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework;


namespace GFramework.Core
{
    public static class UtlProgram
    {
		public static string ConvertGradeRankTypeToIconFilePath(EGradeRankType gradeRankType, EBoardType boardType) => Locator.Instance.textureManager.GetFilePathOfGradeRankIcon(gradeRankType: gradeRankType, boardType: boardType, isDefault: true);

		public static string GetFilePathOfGradeRankCharacter(EGradeRankType gradeRankType) => Locator.Instance.textureManager.GetFilePathOfGradeRankCharacter(gradeRankType);
	}
}
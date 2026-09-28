using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using GFramework.Core;

// TODO: filePath Mangaer を用意したほうがよさそう
namespace GFramework
{
    /// テクスチャ・マネジャのインターフェース
    public interface ITextureManager
    {
        /// ファイルからテクスチャを生成する
        Texture2D LoadTexture(string filePathUnderResources, bool willCacheIfNeeded);

        /// テクスチャをキャッシュに事前読み込みする
        void PreLoadTexture(List<string> filePathUnderResourcesList);

        /// テクスチャのキャッシュをクリアする
        void ClearTexture();


        string GetFilePath(ETextureTypeForDummy textureType);
        string GetFilePath(ETextureTypeForBackground textureType);
        string GetFilePath(ETextureTypeForCurtain textureType);
        string GetFilePath(ETextureTypeForStatusBar textureType);
        string GetFilePath(ETextureTypeForTitleLogo textureType);
        string GetFilePath(EBoardType boardType, ETextureTypeForBoard textureType);
        string GetFilePath(ETextureTypeForWaitingSpace textureType);
        string GetFilePath(ETextureTypeForPromotionSelector textureType);
        string GetFilePath(ETextureTypeForResignSelector textureType);
        string GetFilePath(ETextureTypeForUISlider textureType);
        string GetFilePath(ETextureTypeForUICheckBox textureType);
        string GetFilePath(ETextureTypeForUIRadioButton textureType);
        string GetFilePath(ETextureTypeForButtonIcon textureType);
        string GetFilePath(ETextureTypeForMenuStage textureType);
        string GetFilePathOfImageText(int designID, char character);
        string GetFilePath(EWindowType windowType);
        string GetFilePath(EButtonImageType buttonImageType, EButtonStatusType buttonStatusType);
        string GetFilePathOfInputFieldImage(EButtonStatusType buttonStatusType, bool hasError);
        string GetFilePathOfTextBalloonBase();
        string GetFilePathOfGradeRankIcon(EGradeRankType gradeRankType, EBoardType boardType, bool isDefault);
        string GetFilePathOfGradeRankCharacter(EGradeRankType gradeRankType);
        string GetFilePath(EGameResultType gameResultType);
        string GetFilePathOfGameResultConsecutiveCount(EGameResultType gameResultType, int count);
        string GetFilePathOfGameResultRateBaseImage(EBoardType boardType);
        string GetFilePathOfGameResultRateArrowImage(GameResultRateInfoBehaviour.EGameResultRateArrowType rateResultArrowType);
        string GetFilePath(EHistoryItemResult historyItemResult);
        string GetFilePath(EBoardType boardType);

        void Destroy();
    }
}
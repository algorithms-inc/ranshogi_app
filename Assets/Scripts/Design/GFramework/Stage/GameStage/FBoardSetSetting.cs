using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    // boardDesignType              // 盤のデザイン       現在 0 のみ
    // pieceDesignType              // 駒のデザイン       現在 0 のみ
    // pieceFrontType               // 駒表タイプ         0: 二文字, 1: 一文字
    // pieceBackTypeForRookGroup    // 駒裏タイプ(飛車系) 0: 楷書二文字, 1: くずし字, 2: 楷書一文字
    // pieceBackTypeForSilverGroup  // 駒裏タイプ(銀系)   0: 楷書二文字, 1: くずし字, 2: 略字一文字
    // pieceBackTypeForPawn        // 駒裏タイプ(歩)     現在 0「と」のみ
    public struct FBoardSetSetting
    {
        public FBoardImageSetSetting boardImageSetSetting;
        public FPieceImageSetSetting pieceImageSetSetting;


        public FBoardSetSetting(
            int boardDesignType,
            int pieceDesignType,
            int pieceFrontType,
            int pieceBackTypeForRookGroup,
            int pieceBackTypeForSilverGroup,
            int pieceBackTypeForPawn)
        {
            boardImageSetSetting = new FBoardImageSetSetting(designType: boardDesignType);

            pieceImageSetSetting = new FPieceImageSetSetting(
                designType: pieceDesignType,
                frontType: pieceFrontType,
                backTypeForHishaGroup: pieceBackTypeForRookGroup,
                backTypeForGinshouGroup: pieceBackTypeForSilverGroup,
                backTypeForFuhyou: pieceBackTypeForPawn);
        }
    }
}
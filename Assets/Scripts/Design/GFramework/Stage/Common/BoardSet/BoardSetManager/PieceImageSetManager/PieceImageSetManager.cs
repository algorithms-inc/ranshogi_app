using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public sealed class PieceImageSetManager : IPieceImageSetManager
    {
        private FPieceImageSetSetting _pieceImageSetSetting;

        public PieceImageSetManager()
        {
            _pieceImageSetSetting = FPieceImageSetSetting.Default;
        }


        public void SetPieceImageSetSetting(in FPieceImageSetSetting pieceImageSetSetting)
        {
            _pieceImageSetSetting = pieceImageSetSetting;
        }


        public string GetFilePathOfImagePieceBody(in FPieceState pieceState)
        {
            // 例：
            // デザイン大カテ0,プレイヤー1,玉,表デザインタイプ0
            // "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_0g_a0_p1"

            // デザイン大カテ0,プレイヤー2,銀,裏デザインタイプ1
            // "Stage/Common/BoardSet/Pieces/d0/p2/piece_d0_4_b1_p2"


            string folderNameRoot = "Stage/Common/BoardSet/Pieces";


            string folderNameDesignType = "d" + _pieceImageSetSetting.designType.ToString();

            string folderNamePlayer = pieceState.PlayerType switch
            {
                EPlayerType.PLAYER1 => "p1",
                EPlayerType.PLAYER2 => "p2",
                _ => throw new InvalidOperationException(),
            };

            string fileNamePrefix = "piece";


            string fileNameDesignType = folderNameDesignType;


            string fileNamePieceID;

            
            switch (pieceState.PieceType)
            {
                case EPieceType.OUSHOU:
                    fileNamePieceID = (pieceState.IsGyokushou) ? "0g" : "0";
                    break;

                case EPieceType.HISHA:
                case EPieceType.KAKUGYOU:
                case EPieceType.KINSHOU:
                case EPieceType.GINSHOU:
                case EPieceType.KEIMA:
                case EPieceType.KYOUSHA:
                case EPieceType.FUHYOU:
                    fileNamePieceID = ((int)pieceState.PieceType).ToString();
                    break;
                
                default:
                    throw new InvalidOperationException();
            }


            string fileNameSide = (pieceState.IsNari == false) ? "a" : "b";


            string fileNameDesignFont;

            if (pieceState.IsNari == false)
            {
                fileNameDesignFont = _pieceImageSetSetting.frontType.ToString();
            }
            else
            {
                switch (pieceState.PieceType)
                {
              
                    case EPieceType.HISHA:
                    case EPieceType.KAKUGYOU:
                        fileNameDesignFont = _pieceImageSetSetting.backTypeForHishaGroup.ToString();
                        break;

                    case EPieceType.GINSHOU:
                    case EPieceType.KEIMA:
                    case EPieceType.KYOUSHA:
                        fileNameDesignFont = _pieceImageSetSetting.backTypeForGinshouGroup.ToString();
                        break;

                    case EPieceType.FUHYOU:
                        fileNameDesignFont = _pieceImageSetSetting.backTypeForFuhyou.ToString();
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            string fileNamePlayer = folderNamePlayer;


            string foloderPath = folderNameRoot + "/" + folderNameDesignType + "/" + folderNamePlayer + "/";
            string fileName = fileNamePrefix + "_" + fileNameDesignType + "_" + fileNamePieceID + "_" + fileNameSide + fileNameDesignFont + "_" + fileNamePlayer;


            return foloderPath + fileName + ".png";
        }


        public string GetFilePathOfImagePieceShadow(in FPieceState pieceState)
        {
            // 例：
            // デザイン大カテ0,プレイヤー1,王タイプ
            // "Stage/Common/BoardSet/Pieces/d0/shadow/piece_shadow_d0_0_p1"

            // 例：
            // デザイン大カテ0,プレイヤー1,歩タイプ
            // "Stage/Common/BoardSet/Pieces/d0/shadow/piece_shadow_d0_7_p1"


            string folderNameRoot = "Stage/Common/BoardSet/Pieces";


            string folderNameDesignType = "d" + _pieceImageSetSetting.designType.ToString();


            string folderNameShadow = "shadow";


            string fileNamePrefix = "piece_shadow";


            string fileNameDesignType = folderNameDesignType;


            string fileNamePieceSize = ((int)Utl.ConvertPieceTypeToPieceSizeType(pieceType: pieceState.PieceType)).ToString();

            string fileNamePlayer = pieceState.PlayerType switch
            {
                EPlayerType.PLAYER1 => "p1",
                EPlayerType.PLAYER2 => "p2",
                _ => throw new InvalidOperationException(),
            };



            string foloderPath = folderNameRoot + "/" + folderNameDesignType + "/" + folderNameShadow + "/";
            string fileName = fileNamePrefix + "_" + fileNameDesignType + "_" + fileNamePieceSize + "_" + fileNamePlayer;


            return foloderPath + fileName + ".png";
        }
    }
}
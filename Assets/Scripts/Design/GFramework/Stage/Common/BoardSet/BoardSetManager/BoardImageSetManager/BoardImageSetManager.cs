using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public struct FBoardImageSetSetting
    {
        public int designType;


        public FBoardImageSetSetting(int designType)
        {
            this.designType = designType;
        }


        public static FBoardImageSetSetting Default = new FBoardImageSetSetting(0);
    }


    public sealed class BoardImageSetManager : IBoardImageSetManager
    {
        private FBoardImageSetSetting _boardImageSetSetting;


        public void SetBoardImageSetSetting(in FBoardImageSetSetting boardImageSetSetting)
        {
            _boardImageSetSetting = boardImageSetSetting;
        }

        public string GetFilePathOfImageWaitingSpaceBody(EWaitingSpaceType waitingSpaceType, EPlayerType playerType)
        {
            string result;

            switch (waitingSpaceType)
            {
                case EWaitingSpaceType.HORIZONTAL:
                    if (playerType == EPlayerType.PLAYER1)
                    {
                        result = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForWaitingSpace.BODY_HORIZONTAL_P1);
                    }
                    else
                    {
                        result = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForWaitingSpace.BODY_HORIZONTAL_P2);
                    }
                    break;

                case EWaitingSpaceType.SQUARE:
                    if (playerType == EPlayerType.PLAYER1)
                    {
                        result = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForWaitingSpace.BODY_SQUARE_P1);
                    }
                    else
                    {
                        result = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForWaitingSpace.BODY_SQUARE_P2);
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }


            return result;
        }



        public string GetFilePathOfImageWaitingSpaceShadow(EWaitingSpaceType waitingSpaceType, EPlayerType playerType)
        {
            string result = waitingSpaceType switch
            {
                EWaitingSpaceType.HORIZONTAL => Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForWaitingSpace.SHADOW_HORIZONTAL),
                EWaitingSpaceType.SQUARE => Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForWaitingSpace.SHADOW_SQUARE),
                _ => throw new InvalidOperationException(),
            };

            return result;
        }


        public string GetFilePathOfImagePieceCount(int count)
        {
            // 例
            // BoardSet/PieceCount/d0/piece_count_d0_blank.png
            // BoardSet/PieceCount/d0/piece_count_d0_2.png
            //                :
            // BoardSet/PieceCount/d0/piece_count_d0_12.png

            string pathBase = "Stage/Common/BoardSet/PieceCount/d" + _boardImageSetSetting.designType.ToString() + "/piece_count_d" + _boardImageSetSetting.designType.ToString() + "_";
            string extention = ".png";

            if (count == 0 || count == 1)
            {
                return pathBase + "blank" + extention;
            }
            else if (count <= 12)
            {
                return pathBase + count.ToString() + extention;
            }

            throw new NotImplementedException();
        }


        public string GetFilePathOfImageCell(CellBehaviour.ECellType cellType)
        {
            // TODO: moved は色を変更するだけ、の場合、画像は用意しない

            // 例
            // BoardSet/Cell/d0/cell_d0_place_target.png
            // BoardSet/Cell/d0/cell_d0_moved.png

            string pathBase = "Stage/Common/BoardSet/Cell/d" + _boardImageSetSetting.designType.ToString() + "/cell_d" + _boardImageSetSetting.designType.ToString() + "_";
            string extention = ".png";


            string fileNameForCellType = cellType switch
            {
                CellBehaviour.ECellType.DEFAULT => throw new NotImplementedException(),  // DEFAULT 時は見えなくなっているので、特に画像を変更する必要がない
                CellBehaviour.ECellType.PLACE_TARGET => "place_target",
                CellBehaviour.ECellType.LAST_MOVED => "last_moved",
                _ => throw new NotImplementedException(),
            };

            return pathBase + fileNameForCellType + extention;
        }
    }
}
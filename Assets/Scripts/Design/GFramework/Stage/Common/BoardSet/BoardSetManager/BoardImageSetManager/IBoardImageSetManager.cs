using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;


namespace GFramework
{
    /// <summary>
    /// 駒デザインのセットと対応するファイル名を管理するマネジャ
    /// </summary>
    public interface IBoardImageSetManager
    {
        /// <summary>
        /// 盤画像の設定をセットする
        /// </summary>
        /// <param name="pieceImageSetSetting"></param>
        void SetBoardImageSetSetting(in FBoardImageSetSetting boardImageSetSetting);


        /// <summary>
        /// 持ち駒台画像のパスを取得する
        /// </summary>
        /// <returns></returns>
        string GetFilePathOfImageWaitingSpaceBody(EWaitingSpaceType waitingSpaceType, EPlayerType playerType);


        /// <summary>
        /// 持ち駒台画像の影のパスを取得する
        /// </summary>
        /// <returns></returns>
        string GetFilePathOfImageWaitingSpaceShadow(EWaitingSpaceType waitingSpaceType, EPlayerType playerType);


        /// <summary>
        /// 駒数の画像のパスを取得する
        /// </summary>
        /// <returns></returns>
        string GetFilePathOfImagePieceCount(int count);



        string GetFilePathOfImageCell(CellBehaviour.ECellType cellType);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    /// <summary>
    /// 駒デザインのセットと対応するファイル名を管理するマネジャ
    /// </summary>
    public interface IPieceImageSetManager
    {
        /// <summary>
        /// 駒画像の設定をセットする
        /// </summary>
        /// <returns></returns>
        void SetPieceImageSetSetting(in FPieceImageSetSetting pieceImageSetSetting);


        /// <summary>
        /// 駒の画像パスを取得する
        /// </summary>
        /// <returns></returns>
        string GetFilePathOfImagePieceBody(in FPieceState pieceState);


        /// <summary>
        /// 駒の影の画像パスを取得する
        /// </summary>
        /// <returns></returns>
        string GetFilePathOfImagePieceShadow(in FPieceState pieceState);
    }
}
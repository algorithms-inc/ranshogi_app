using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using GFramework.Core;


namespace GFramework.Core
{
    /// <summary>
    /// デザイン設定機能
    /// </summary>
    public interface IDesignController
    {
        //int GetBaseDesignID();

        /// 盤面に数字を表示するかどうかを取得
        bool IsShowNumOnBoard();
        /// 相手の番に背景色を変えるかを取得
        bool IsChangeBackground();
        /// 相手の移動先を強調するかを取得
        bool IsShowLastMove();
        /// 駒の表デザインIDを取得
        int GetDesignIDOfPiceFront();
        /// 駒の裏デザインIDを取得(飛・角)
        int GetDesignIDOfPieceBackForGroupRook();
        /// 駒の裏デザインIDを取得(銀・桂・香)
        int GetDesignIDOfPieceBackForGroupSilver();

        //void SetBaseDesignID(int value);

        /// 盤面に数字を表示するかどうかを設定
        void SetIsShowNumOnBoard(bool value);
        /// 相手の番に背景色を変えるかを設定
        void SetIsChangeBackground(bool value);
        /// 相手の移動先を強調するかを設定
        void SetIsShowLastMove(bool value);
        /// 駒の表デザインIDを設定
        void SetDesignIDOfPieceFront(int value);
        /// 駒の裏デザインIDを設定(飛・角)
        void SetDesignIDOfPieceBackForGroupRook(int value);
        /// 駒の裏デザインIDを設定(銀・桂・香)
        void SetDesignIDOfPieceBackForGroupSilver(int value);
    }
}
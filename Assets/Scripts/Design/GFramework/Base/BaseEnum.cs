using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    /// <summary>
    /// パラメータの種類
    /// </summary>
    public enum EParamType
    {
        VALUE,
        RATIO
    }


    /// <summary>
    /// 画面の方向
    /// </summary>
    public enum EScreenOrientation
    {
        PORTRAIT,
        PORTRAIT_UPSIDEDOWN,
        LANDSCAPE_LEFT,
        LANDSCAPE_RIGHT
    }


    /// <summary>
    /// 方向
    /// </summary>
    public enum EDirection
    {
        UP         = 0,
        UP_RIGHT   = 1,
        RIGHT      = 2,
        DOWN_RIGHT = 3,
        DOWN       = 4,
        DOWN_LEFT  = 5,
        LEFT       = 6,
        UP_LEFT    = 7,
        STAY       = 8,
        NULL       = 9,
    }


    /// <summary>
    /// 駒の種別
    /// </summary>
    public enum EPieceType
    {
        OUSHOU   = 0,
        HISHA    = 1,
        KAKUGYOU = 2,
        KINSHOU  = 3,
        GINSHOU  = 4,
        KEIMA    = 5,
        KYOUSHA  = 6,
        FUHYOU   = 7
    }

    public static class EPieceTypeExtensions
    {
        public static string ToJapaneseString(this EPieceType pieceType)
        {
            return pieceType switch
            {
                EPieceType.OUSHOU   => "王",
                EPieceType.HISHA    => "飛",
                EPieceType.KAKUGYOU => "角",
                EPieceType.KINSHOU  => "金",
                EPieceType.GINSHOU  => "銀",
                EPieceType.KEIMA    => "桂",
                EPieceType.KYOUSHA  => "香",
                EPieceType.FUHYOU   => "歩",
                _                   => throw new InvalidOperationException(),
            };
        }
    }


    /// <summary>
    /// 駒のサイズ種別
    /// </summary>
    public enum EPieceSizeType
    {
        SIZE_OUSHOU      = 0,
        SIZE_HISHA_GROUP = 1,
        SIZE_KIN_GROUP   = 3,
        SIZE_KEIMA       = 5,
        SIZE_KYOUSHA     = 6,
        SIZE_FUHYOU      = 7
    }


    /// <summary>
    /// プレイヤー種別
    /// </summary>
    public enum EPlayerType
    {
        PLAYER1 = 0,
        PLAYER2 = 1
    }


    /// <summary>
    /// 先手・後手
    /// </summary>
    public enum EMoveOrderType
    {
        FIRST  = 0,
        SECOND = 1
    }


    /// <summary>
    /// 駒台表示種別
    /// </summary>
    public enum EWaitingSpaceType
    {
        HORIZONTAL,
        SQUARE
    }


    public enum ETextSizeType
    {
        SMALL,
        MIDDLE,
        LARGE
    }


    public enum ESelectResultType
    {
        CANSEL,
        OK
    }
}

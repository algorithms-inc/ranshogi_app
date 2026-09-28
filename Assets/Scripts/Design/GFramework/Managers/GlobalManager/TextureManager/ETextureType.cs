using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;

namespace GFramework
{
    public enum ETextureTypeForDummy
    {
        AREA,
    }


    public enum ETextureTypeForBackground
    {
        BODY_A,
        BODY_B,
        SCREEN_EDGE,
        RIPPLE_A,
        RIPPLE_B,
        RIPPLE_C,
        RIPPLE_D,
    }

    public enum ETextureTypeForCurtain
    {
        WHITE,
        BLACK,
        BLUE,
        RED,
    }

    public enum ETextureTypeForStatusBar
    {
        BODY,
        LIFE_BODY,
        HOLE,
        EMBLEM_PREMIUM
    }

    public enum ETextureTypeForTitleLogo
    {
        BODY,
    }

    public enum ETextureTypeForBoard
    {
        BODY,
        BODY_NUM_BLACK,
        BODY_NUM_WHITE,
        SHADOW,
    }

    public enum ETextureTypeForWaitingSpace
    {
        BODY_HORIZONTAL_P1,
        BODY_HORIZONTAL_P2,
        BODY_SQUARE_P1,
        BODY_SQUARE_P2,
        SHADOW_HORIZONTAL,
        SHADOW_SQUARE,
    }

    public enum ETextureTypeForPromotionSelector
    {
        BODY,
    }

    public enum ETextureTypeForResignSelector
    {
        BODY,
    }

    public enum ETextureTypeForUISlider
    {
        BASE_DEFAULT,
        BASE_DISABLED,
        BAR_DEFAULT,
        BAR_DISABLED,
        MARK_DEFAULT,
        MARK_DISABLED,
    }

    public enum ETextureTypeForUICheckBox
    {
        BASE_DEFAULT,
        BASE_HIGHLIGHTED,
        BASE_ERROR,
        BASE_DISABLED,
        MARK_ON_DEFAULT,
        MARK_ON_DISABLED,
        MARK_OFF,
    }

    public enum ETextureTypeForUIRadioButton
    {
        BASE_DEFAULT,
        BASE_HIGHLIGHTED,
        BASE_ERROR,
        BASE_DISABLED,
        MARK_ON_DEFAULT,
        MARK_ON_DISABLED,
        MARK_OFF,
    }

    public enum ETextureTypeForButtonIcon
    {
        NEXT,
        PREVIOUS,
        TO_FIRST,
        TO_LAST,
        SETTING,
        PLAY_RANSHOGI,
        PLAY_SHOGI,
        PLAY_WITH_FRIEND
    }

    public enum ETextureTypeForMenuStage
    {
        BUTTON_DEFAULT,
    }


}
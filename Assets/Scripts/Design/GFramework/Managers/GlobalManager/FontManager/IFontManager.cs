using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    namespace Core
    {
        public enum EFontType
        {
            SERIF,
            BLOCK
        }
    }


    /// <summary>
    /// フォントを管理するマネジャ
    /// </summary>
    public interface IFontManager
    {
        Font GetFont(EFontType fontType);
    }
}
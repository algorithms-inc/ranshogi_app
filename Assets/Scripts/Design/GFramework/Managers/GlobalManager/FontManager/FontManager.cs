using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public sealed class FontManager : IFontManager
    {
        private Font _fontSerif;
        private Font _fontBlock;

        public FontManager()
        {
            Setup();
        }


        private void Setup()
        {
            Font support(string fileName)
            {
                Font result = Resources.Load<Font>("Font/" + fileName);
                Assert.IsTrue(result != null);

                return result;
            }

            _fontSerif = support(fileName: Config.FONT_FILENAME__SERIF);
            _fontBlock = support(fileName: Config.FONT_FILENAME__BLOCK);
        }


        public Font GetFont(EFontType fontType)
        {
            return fontType switch
            {
                EFontType.SERIF => _fontSerif,
                EFontType.BLOCK => _fontBlock,
                _ => throw new NotImplementedException(),
            };
        }


    }
}
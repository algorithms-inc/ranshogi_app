using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public struct FPieceImageSetSetting
    {
        public int designType;
        public int frontType;
        public int backTypeForHishaGroup;
        public int backTypeForGinshouGroup;
        public int backTypeForFuhyou;

        public FPieceImageSetSetting(
            int designType,
            int frontType,
            int backTypeForHishaGroup,
            int backTypeForGinshouGroup,
            int backTypeForFuhyou)
        {
            this.designType = designType;
            this.frontType = frontType;
            this.backTypeForHishaGroup = backTypeForHishaGroup;
            this.backTypeForGinshouGroup = backTypeForGinshouGroup;
            this.backTypeForFuhyou = backTypeForFuhyou;

            Assert.IsTrue(this.designType >= 0);
            Assert.IsTrue(this.frontType >= 0);
            Assert.IsTrue(this.backTypeForHishaGroup >= 0);
            Assert.IsTrue(this.backTypeForGinshouGroup >= 0);
            Assert.IsTrue(this.backTypeForFuhyou >= 0);
        }


        public static FPieceImageSetSetting Default = new FPieceImageSetSetting(0, 0, 0, 0, 0);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    /// ImageText の初期化設定
    public readonly struct FImageTextInitializer
    {
        // ===============================================
        // MARK: - ● Property ─────

        /// デザインID
		public int DesignID { get; }

        /// 表示最大文字数
		public int MaxLength { get; }

        /// 文字スペースの割合リスト
		public float[] SpacingRatioList { get; }

		/// テキスト左寄せ／右寄せ
		public ImageTextBehaviour.ETextAlign TextAlign { get; }

        /// GameObject に対する表示アンカー
		public EDirection AnchorDirection { get; }

        /// 表示レイヤー
		public int SortingOrder { get; }


		// ===============================================
		// MARK: - ● Constructor ─────

		/// コンストラクタ（文字スペースの割合一定）
		public FImageTextInitializer(
            int designID,
            int maxLength,
            float spacingRatio,
            ImageTextBehaviour.ETextAlign textAlign,
            EDirection anchorDirection,
            int sortingOrder)
        {
            Assert.IsTrue(maxLength >= 1);

            DesignID = designID;
            MaxLength = maxLength;
            TextAlign = textAlign;
            AnchorDirection = anchorDirection;
            SortingOrder = sortingOrder;

            SpacingRatioList = new float[maxLength - 1];

            for (int index = 0; index <= SpacingRatioList.Length - 1; index++)
            {
                SpacingRatioList[index] = spacingRatio;
            }
        }


        /// コンストラクタ（文字スペースの割合指定）
        public FImageTextInitializer(
            int designID,
            int maxLength,
            float[] spacingRatioList,
            ImageTextBehaviour.ETextAlign textAlign,
            EDirection anchorDirection,
            int sortingOrder)
        {
            Assert.IsTrue(maxLength >= 1);
            Assert.IsTrue(maxLength == spacingRatioList.Length + 1);

            DesignID = designID;
            MaxLength = maxLength;
            SpacingRatioList = spacingRatioList;
            TextAlign = textAlign;
            AnchorDirection = anchorDirection;
            SortingOrder = sortingOrder;
        }
    }
}
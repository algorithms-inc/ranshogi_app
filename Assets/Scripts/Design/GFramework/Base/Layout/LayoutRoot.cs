using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class LayoutRoot : LayoutFrame
    {
        public LayoutRoot(Vector2 size, FVec4 spacing, Vector2 position) : base(
            sizeSetting: new FLayoutSizeSettingByValue(size: size),
            align: EDirection.NULL,
            priority: 0,
            spacing: spacing,
            offset: new Vector2(0, 0))
        {
            UpdatePosition(position: position);
        }

        public LayoutRoot(in FArea area, in FVec4 spacing) : base(
            sizeSetting: new FLayoutSizeSettingByValue(size: area.size),
            align: EDirection.NULL,
            priority: 0,
            spacing: spacing,
            offset: new Vector2(0, 0))
        {
            UpdatePosition(position: area.origin);
        }


        // ===============================================
        // MARK: - ● Medhod (reDefine default) ─────

        /// root は親を持つことができない
        public new void SetParent(PLayout parent)
        {
            Assert.IsTrue(false);
        }



        // ===============================================
        // MARK: - ● Medhod (original) ─────

        /// レイアウトを再計算する
        /// - 子の追加・削除を行った場合に実行すること
        public void Update()
        {
            CalcChildSize();

            CalcChildPosition();
        }
    }
}

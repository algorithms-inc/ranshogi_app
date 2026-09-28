using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public abstract class PLayouter
    {
        // ===============================================
        // MARK: - ● Abstruct Method ─────

        /// 破壊処理
        public abstract void Destroy();

        /// レイアウトを表示する
        public abstract void ShowLayoutArea();
    }
}

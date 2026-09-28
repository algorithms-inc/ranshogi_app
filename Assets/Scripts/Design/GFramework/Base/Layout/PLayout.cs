using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public abstract class PLayout
    {
        // ===============================================
        // MARK: - ● Property ─────

        // 表示位置 (原点左上)
        public abstract Vector2 Position { get; }

        // 親に対する配置 (使用しないことあり)
        public abstract EDirection Align { get; }

        /// エリア
        public FArea Area => new(origin: Position, size: Size);

        /// パディングを考慮したエリアを取得
        public FArea AreaWithPadding
        {
            get {
                var origin = Position + Spacing.UP_LEFT;
                return new FArea(origin: origin, size: sizeWithSpacing);
            }
        }

        /// サイズ
        public abstract Vector2 Size { get; }

        /// 内側余白
        public abstract FVec4 Spacing { get; }



        /// 内部余白を考慮したサイズ
        public Vector2 sizeWithSpacing = new(0, 0);

        /// 優先度
        public abstract int Priority { get; }

        /// 子のリスト
        public List<PLayout> childList = new();

        /// 親
        /// - 循環参照を防ぐために、弱参照 WeakReference<CLayoutBase> を用いた場合、WeakReference<CLayoutBase> から VSack HStack などの元クラスの確認が難しくなる
        /// - そのため、不要になった場合には、Destroy() を行うようにする。
        public PLayout parent;



        // ===============================================
        // MARK: - ● Method ─────

        /// 子をリストで追加する
        /// - Parameter newChildList: 追加する子のリスト
        public void RegisterChildByList(List<PLayout> newChildList)
        {
            newChildList.ForEach(i => RegisterChild(i));
        }


        /// 子を追加する
        /// - Parameter newChild: 追加する子
        /// - 登録順に並ぶ
        public void RegisterChild(PLayout newChild)
        {
            newChild.SetParent(this);

            childList.Add(newChild);
        }


        /// 子のリストをプライオリティ順に並べて取得(spacerは除く)
        public List<PLayout> GetPrioritySortedChildListWithoutSpacer()
        {
            List<PLayout> tempList = childList.FindAll(i => !(i is LayoutSpacer));

            tempList.Sort((a, b) => a.Priority - b.Priority);

            return tempList;
        }


        public (List<LayoutSpacer> spacerList, float sumOfFlexivility) GetSpacerListAndSumOfFlexivility()
        {
            List<LayoutSpacer> spacerList = new();
            float sumOfFlexivility = 0;

            foreach (var i in childList)
            {
                // 「パターンマッチング」という記述方式。null出ない場合に実行される
                if (i is LayoutSpacer layoutSpacer)
                {
                    spacerList.Add(layoutSpacer);
                    sumOfFlexivility += layoutSpacer.GetFlexibility();
                }
            }

            return (spacerList, sumOfFlexivility);
        }


        /// 親を設定する
        /// - Parameter parent: 親
        /// - 循環参照を防ぐため、一度設定すると変更することはできない
        public void SetParent(PLayout parent)
        {
            /// 循環参照を防止
            Assert.IsTrue(this.parent == null);

            this.parent = parent;
        }


        /// 子のサイズを取得
        /// - Parameter index: 子のIndex
        /// - Returns: サイズ
        public Vector2 GetChildSize(int index)
        {
            return childList[index].Size;
        }



        /// 自身のサイズからスペースを引いたサイズを更新する
        public void UpdateSizeWithSpacing()
        {
            sizeWithSpacing = new Vector2(Size.x - Spacing.SumOfLR, Size.y - Spacing.SumOfUD);
        }


        /// 位置を更新し、子の位置を再計算する
        /// - Parameter position: 新たな位置
        /// - 同じ位置に設定される場合でも処理を行う(サイズが変更となっている場合があり、位置が変わることがあるから)
        public abstract void UpdatePosition(Vector2 position);


        /// 削除処理
        public void Destroy()
        {
            // 子を破壊する
            childList.ForEach(i => i.Destroy());

            // 実際には、リストから削除するような操作は行わない。なぜならば forEach 内でリストを破壊するような操作はエラーを招くため。
            // ここでは参照を放棄することでクラスをGCに破壊してもらうようにしている。
            // なお、Behaviour ではないので、Unity Editor においては次回プレイ時に前のクラス類が削除される。
            parent = null;
            childList.Clear();
        }


        // ===============================================
        // MARK: - ● Abstruct Method ─────

        /// 親として、子のサイズを決定する
        public abstract void CalcChildSize();

        /// 子として、提案されたサイズを元に自身のサイズを決定する
        public abstract Vector2 CalcSize(Vector2 prosposedMaxSize, Vector2 originalSize);

        /// 親として、子の位置を決定する
        public abstract void CalcChildPosition();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class LayoutHStack : PLayout
    {
        private Vector2 _position = Vector2.zero;
        public override Vector2 Position => _position;

        private EDirection _align;
        public override EDirection Align => _align;

        public override Vector2 Size => sizeSetting.Size;

        private FVec4 _spacing;
        public override FVec4 Spacing => _spacing;

        private int _priority;
        public override int Priority => _priority;


        // -----------------------------------------------
        // MARK: - Property (original)

        public ILayoutSizeSetting sizeSetting;

        /// 並べる要素間のスペース
        private float _innerSpacing;

        /// 並べる物の配置
        /// - 子の Align は無視される
        private EDirection _innerAlign;

        /// protocol に含めたほうがいいかも。未使用
        private Vector2 offset;

        /// 幅の残り
        private float _restOfWidth = 0;

        /// プライオリティで並び替えた子のリスト
        private List<PLayout> _prioritySortedChildListWithoutSpacer = new();



        // ===============================================
        // MARK: - ● Constructor ─────

        public LayoutHStack(
            ILayoutSizeSetting sizeSetting,
            EDirection align,
            int priority,
            FVec4 spacing,
            float innerSpacing,
            EDirection innerAlign,
            Vector2 offset)
        {
            this.sizeSetting = sizeSetting;
            _align = align;
            _priority = priority;
            _spacing = spacing;
            this._innerSpacing = innerSpacing;
            this._innerAlign = innerAlign;
            this.offset = offset;

            UpdateSizeWithSpacing();
        }



        // ===============================================
        // MARK: - ● Method (from Protocol)─────

        public override Vector2 CalcSize(Vector2 prosposedMaxSize, Vector2 originalSize)
        {
            // 自身のサイズを計算
            var size = sizeSetting.CalcAndUpdateSize(maxSize: prosposedMaxSize, originalSize: originalSize);

            // スペースを考慮したサイズを更新
            UpdateSizeWithSpacing();

            // 子のサイズを決定
            CalcChildSize();

            return size;
        }


        public override void CalcChildSize()
        {
            if (childList.Count == 0) { return; }


            // スペーサー以外のレイアウト要素
            _prioritySortedChildListWithoutSpacer = GetPrioritySortedChildListWithoutSpacer();

            _restOfWidth = sizeWithSpacing.x - (_innerSpacing * childList.Count - 1);

            int maxIndex = _prioritySortedChildListWithoutSpacer.Count - 1;

            
            for (int index = 0; index <= maxIndex; index++)
            {
                _restOfWidth -= ProposeMaxSizeToChildWithoutSpacer(index: index).x;
            }


            // スペーサー
            var spacerListAndSumOfFlexivility = GetSpacerListAndSumOfFlexivility();

            if (spacerListAndSumOfFlexivility.spacerList.Count != 0)
            {
                var restOfWidthPer1Flexivility = _restOfWidth / spacerListAndSumOfFlexivility.sumOfFlexivility;

                foreach (var i in spacerListAndSumOfFlexivility.spacerList)
                {
                    i.SetSize(value: restOfWidthPer1Flexivility * i.GetFlexibility());
                }

                _restOfWidth = 0;
            }
        }


        /// 返却値は子が決定したサイズ
        private Vector2 ProposeMaxSizeToChildWithoutSpacer(int index)
        {
            var proposingSize = new Vector2(_restOfWidth, sizeWithSpacing.y);

            var childSize = _prioritySortedChildListWithoutSpacer[index].CalcSize(
                prosposedMaxSize: proposingSize,
                originalSize: sizeWithSpacing);

            return childSize;
        }


        public override void CalcChildPosition()
        {
            if (childList.Count == 0) { return; }


            int maxIndex = childList.Count - 1;

            // --------------------------------
            // 結果を格納する座標リストを、子の数だけ用意

            List<Vector2> crdList = new List<Vector2>();

            for (int index = 1; index <= childList.Count; index++)
            {
                crdList.Add(new Vector2(0, 0));
            }


            // --------------------------------
            // 自身のスペースを除いた原点を算出
            Vector2 origin = _position + _spacing.UP_LEFT;

            Vector2 newCrd = new(0, 0);


            // --------------------------------
            // 各子の position.x を算出する

            // 1つ目
            switch (Utl.DecomposedToLR(_innerAlign))
            {
                case EDirection.LEFT:
                    newCrd = new Vector2(origin.x, crdList[0].y);
                    crdList[0] = newCrd;
                    break;

                case EDirection.RIGHT:
                    newCrd = new Vector2(origin.x + _restOfWidth, crdList[0].y);
                    crdList[0] = newCrd;
                    break;

                case EDirection.STAY:
                    newCrd = new Vector2(origin.x + (_restOfWidth / 2), crdList[0].y);
                    crdList[0] = newCrd;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            // 2つ目以降
            if (childList.Count >= 2)
            {
                for (int index = 1; index <= maxIndex; index++)
                {
                    newCrd = new Vector2(
                        (childList[index - 1].Size.x + crdList[index - 1].x) + _innerSpacing,
                        crdList[index].y);
                    crdList[index] = newCrd;
                }
            }


            // --------------------------------
            // 各子の position.y を算出し、子に決定した position をセット

            for (int index = 0; index <= maxIndex; index++)
            {
                switch (Utl.DecomposedToUD(_innerAlign))
                {
                    case EDirection.UP:
                        newCrd = new Vector2(
                            crdList[index].x,
                            origin.y);
                        crdList[index] = newCrd;
                        break;

                    case EDirection.DOWN:
                        newCrd = new Vector2(
                            crdList[index].x,
                            _position.y + Size.y - _spacing.bottom - childList[index].Size.y);
                        crdList[index] = newCrd;
                        break;

                    case EDirection.STAY:
                        newCrd = new Vector2(
                            crdList[index].x,
                            _position.y + (Size.y - childList[index].Size.y) / 2);
                        crdList[index] = newCrd;
                        break;

                    default:
                        throw new InvalidOperationException();
                }


                childList[index].UpdatePosition(position: crdList[index]);
            }
        }


        public override void UpdatePosition(Vector2 position)
        {
            _position = position;

            CalcChildPosition();
        }
    }
}

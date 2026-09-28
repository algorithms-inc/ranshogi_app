using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class LayoutFrame : PLayout
    {
        // ===============================================
        // MARK: - ● Property (original) ─────

        private Vector2 _position = Vector2.zero;
        public override Vector2 Position => _position;

        public ILayoutSizeSetting sizeSetting;

        private EDirection _align;
        public override EDirection Align => _align;

        public override Vector2 Size => sizeSetting.Size;

        private FVec4 _spacing;
        public override FVec4 Spacing => _spacing;

        private int _priority;
        public override int Priority => _priority;



        // ===============================================
        // MARK: - ● Constructor ─────

        public LayoutFrame(
            ILayoutSizeSetting sizeSetting,
            EDirection align,
            int priority,
            FVec4 spacing,
            Vector2 offset)
        {
            this.sizeSetting = sizeSetting;
            _align = align;
            _priority = priority;
            _spacing = spacing;


            UpdateSizeWithSpacing();
        }



        // ===============================================
        // MARK: - ● Implement Abstruct Calss ─────

        public override Vector2 CalcSize(Vector2 prosposedMaxSize, Vector2 originalSize)
        {
            // 自身のサイズを計算
            Vector2 size = sizeSetting.CalcAndUpdateSize(maxSize: prosposedMaxSize, originalSize: originalSize);

            // スペースを考慮したサイズを更新
            UpdateSizeWithSpacing();

            // 子のサイズを決定
            CalcChildSize();


            return size;
        }



        public override void CalcChildSize()
        {
            if (childList.Count == 0) { return; }


            int maxIndex = childList.Count - 1;

            for (int index = 0; index <= maxIndex; index++)
            {
                _ = ProposeMaxSizeToChild(index: index);
            }
        }


        private Vector2 ProposeMaxSizeToChild(int index)
        {
            Vector2 childSize = childList[index].CalcSize(prosposedMaxSize: sizeWithSpacing, originalSize: sizeWithSpacing);

            return childSize;
        }


        public override void CalcChildPosition()
        {
            foreach (PLayout i in childList)
            {
                Vector2 position = CalcChildPosition(childSize: i.Size, childAlign: i.Align);
                i.UpdatePosition(position: position);
            }
        }


        /// 子の位置を計算
        /// - Parameters:
        ///   - childSize: 子のサイズ
        ///   - childAlign: 子のアライン
        /// - Returns: 位置
        public Vector2 CalcChildPosition(Vector2 childSize, EDirection childAlign)
        {
            Vector2 sizeWithSpacing = new(Size.x - _spacing.SumOfLR, Size.y - _spacing.SumOfUD);
            Vector2 positionWithSpacing = _position + _spacing.UP_LEFT;


            return childAlign switch
            {
                EDirection.UP => positionWithSpacing + new Vector2((sizeWithSpacing.x - childSize.x) / 2, 0),
                EDirection.UP_RIGHT => positionWithSpacing + new Vector2(sizeWithSpacing.x - childSize.x, 0),
                EDirection.RIGHT => positionWithSpacing + new Vector2(sizeWithSpacing.x - childSize.x, (sizeWithSpacing.y - childSize.y) / 2),
                EDirection.DOWN_RIGHT => positionWithSpacing + new Vector2(sizeWithSpacing.x - childSize.x, sizeWithSpacing.y - childSize.y),
                EDirection.DOWN => positionWithSpacing + new Vector2((sizeWithSpacing.x - childSize.x) / 2, sizeWithSpacing.y - childSize.y),
                EDirection.DOWN_LEFT => positionWithSpacing + new Vector2(0, sizeWithSpacing.y - childSize.y),
                EDirection.LEFT => positionWithSpacing + new Vector2(0, (sizeWithSpacing.y - childSize.y) / 2),
                EDirection.UP_LEFT => positionWithSpacing,
                EDirection.STAY => positionWithSpacing + new Vector2((sizeWithSpacing.x - childSize.x) / 2, (sizeWithSpacing.y - childSize.y) / 2),
                // fall thorough
                _ => throw new InvalidOperationException(),
            };
        }


        public override void UpdatePosition(Vector2 position)
        {
            _position = position;

            CalcChildPosition();
        }
    }
}

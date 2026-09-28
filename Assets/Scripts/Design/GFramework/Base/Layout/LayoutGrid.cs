using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class LayoutGrid : PLayout
    {
        private Vector2 _position = Vector2.zero;
        public override Vector2 Position => _position;

        private EDirection _align;
        public override EDirection Align => _align;

        public override Vector2 Size => _sizeSetting.Size;

        private FVec4 _spacing;
        public override FVec4 Spacing => _spacing;


        // ===============================================
        // MARK: - ● Property (original) ─────

        /// サイズ設定
        private ILayoutSizeSetting _sizeSetting;

        /// 並べる要素間のスペース
        private Vector2 _innerSpacingXY;

        /// セルの個数(XY)
        private Vector2Int _cellCount;

        /// セルのXY
        private Vector2 _cellSize = new(0, 0);

        /// セルごとのパディング
        private FVec4 _cellPadding;

        /// protocol に含めたほうがいいかも。未使用
        private Vector2 offset_;

        private int _priority;
        public override int Priority => _priority;


        // ===============================================
        // MARK: - ● Constructor ─────

        public LayoutGrid(
            ILayoutSizeSetting sizeSetting,
            EDirection align,
            int priority,
            FVec4 spacing,
            Vector2 innerSpacingXY,
            Vector2Int cellCount,
            FVec4 cellPadding,
            Vector2 offset)
        {
            _sizeSetting = sizeSetting;
            _align = align;
            _priority = priority;
            _spacing = spacing;
            _innerSpacingXY = innerSpacingXY;
            _cellCount = cellCount;
            _cellPadding = cellPadding;
            offset_ = offset;

            UpdateSizeWithSpacing();
            UpdateCellSize();
        }



        // ===============================================
        // MARK: - ● Method (from Protocol)─────

        public override Vector2 CalcSize(Vector2 prosposedMaxSize, Vector2 originalSize)
        {
            // 自身のサイズを計算
            Vector2 size = _sizeSetting.CalcAndUpdateSize(maxSize: prosposedMaxSize, originalSize: originalSize);

            // スペースを考慮したサイズを更新
            UpdateSizeWithSpacing();

            // セルのサイズを更新
            UpdateCellSize();

            return size;
        }


        public override void CalcChildSize()
        {
            throw new InvalidOperationException();
        }


        public override void CalcChildPosition()
        {
            throw new InvalidOperationException();
        }


        public override void UpdatePosition(Vector2 position)
        {
            _position = position;
        }


        // ===============================================
        // MARK: - ● Method (original)─────

        private void UpdateCellSize()
        {
            float sumOfInnerSpacingX = _innerSpacingXY.x * (_cellCount.x - 1);
            float sumOfInnerSpacingY = _innerSpacingXY.y * (_cellCount.y - 1);

            float x = (sizeWithSpacing.x - sumOfInnerSpacingX) / _cellCount.x;
            float y = (sizeWithSpacing.y - sumOfInnerSpacingY) / _cellCount.y;

            _cellSize = new Vector2(x, y);
        }


        private bool IsValidCrd(Vector2Int crd) =>
            (_cellCount.x > crd.x) && (_cellCount.y > crd.y);


        private bool IsValidCellIndex(int index) =>
            _cellCount.x * _cellCount.y > index;


        public FArea GetCellArea(Vector2Int crd)
        {
            Assert.IsTrue(IsValidCrd(crd));

            Vector2 relativePoint = new Vector2(_cellSize.x * crd.x, _cellSize.y * crd.y);

            Vector2 resultOrigin = relativePoint + GetOrigin();

            return new FArea(origin: resultOrigin, size: _cellSize);
        }


        public FArea GetCellArea(int index)
        {
            Assert.IsTrue(IsValidCellIndex(index));

            int x = index % _cellCount.x;
            int y = index / _cellCount.x;

            return GetCellArea(crd: new Vector2Int(x, y));
        }


        public FArea GetCellAreaWithPadding(Vector2Int crd)
        {
            Vector2 relativePoint = new Vector2((_cellSize.x * crd.x) + _cellPadding.top, (_cellSize.y * crd.y) + _cellPadding.right);

            Vector2 resultOrigin = relativePoint + GetOrigin();

            return new FArea(origin: resultOrigin, size: CellSizeWithPadding());
        }


        public FArea GetCellAreaWithPadding(int index)
        {
            Assert.IsTrue(IsValidCellIndex(index));

            int x = index % _cellCount.x;
            int y = index / _cellCount.x;

            return GetCellAreaWithPadding(crd: new Vector2Int(x, y));
        }


		private Vector2 GetOrigin() => new(_position.x + _spacing.left, _position.y + _spacing.top);


		public Vector2 CellSizeWithPadding() => new(_cellSize.x - _cellPadding.SumOfLR, _cellSize.y - _cellPadding.SumOfUD);


		// 複数グリッドの選択

		public FArea GetCellArea(Vector2Int fromCrd, Vector2Int toCrd)
        {
            Assert.IsTrue(IsValidFromToCrd(fromCrd: fromCrd, toCrd: toCrd));

            Vector2 resultFromCrd = GetCellArea(crd: fromCrd).origin;
            Vector2 resultToCrd = GetCellArea(crd: toCrd).EndPoint;

            return new FArea(p1: resultFromCrd, p2: resultToCrd, areaConstructorType: FArea.EAreaConstructorType.ORIGIN_ENDPOINT);
        }


        public FArea GetCellAreaWithPadding(Vector2Int fromCrd, Vector2Int toCrd)
        {
            Assert.IsTrue(IsValidFromToCrd(fromCrd: fromCrd, toCrd: toCrd));

            Vector2 resultFromCrd = GetCellAreaWithPadding(crd: fromCrd).origin;
            Vector2 resultToCrd = GetCellAreaWithPadding(crd: toCrd).EndPoint;

            return new FArea(p1: resultFromCrd, p2: resultToCrd, areaConstructorType: FArea.EAreaConstructorType.ORIGIN_ENDPOINT);
        }


        private bool IsValidFromToCrd(Vector2Int fromCrd, Vector2Int toCrd)
        {
            return (fromCrd.x <= toCrd.x) && (fromCrd.y <= toCrd.y);
        }
    }
}
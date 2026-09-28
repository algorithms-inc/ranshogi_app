using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    /// レイアウト用のスペーサー
    /// - HStack / VStack の中に入れて使用する
    /// - 通常のレイアウト要素とは別に計算されるため、priority値は設定できない。兄弟要素にpriority = 0 のものがあっても問題ない。
    /// - flexibility値を設定することで、他のスペーサーとは異なる拡縮を設定できる。
    public class LayoutSpacer : PLayout
    {
        // ===============================================
        // MARK: - ● Enum ─────

        private enum ELayoutSpacerType
        {
            H_SPACER,
            V_SPACER
        }

        private Vector2 _position = Vector2.zero;
        public override Vector2 Position => _position;

        private EDirection _align;
        public override EDirection Align => _align;

        private Vector2 _size = new(0, 0);
        public override Vector2 Size => _size;

        private FVec4 _spacing;
        public override FVec4 Spacing => _spacing;

        private int _priority;
        public override int Priority => _priority;


        // ===============================================
        // MARK: - ● Property (original) ─────

        private ELayoutSpacerType _type = ELayoutSpacerType.H_SPACER;

        private float _flexibility;



        // ===============================================
        // MARK: - ● Constructor ─────

        public LayoutSpacer(float flexibility = 1)
        {
            _flexibility = flexibility;

            _align = EDirection.STAY;
            _priority = 0;
            _spacing = FVec4.ZERO;


            UpdateSizeWithSpacing();
        }



        // ===============================================
        // MARK: - ● Medhod (reDefine default) ─────

        /// 親を設定する
        /// - Parameter parent: 親
        /// - 循環参照を防ぐため、一度設定すると変更することはできない
        public new void SetParent(PLayout parent)
        {
            /// 循環参照を防止
            if (base.parent != null)
            {
                Assert.IsTrue(false);
                return;
            }


            this.parent = parent;

            /// 親が、HStack か VStack か確認する
            if (base.parent is LayoutHStack)
            {
                _type = ELayoutSpacerType.H_SPACER;
            }
            else if (base.parent is LayoutVStack)
            {
                _type = ELayoutSpacerType.V_SPACER;
            }
            else
            {
                Assert.IsTrue(false);
                return;
            }
        }



        // ===============================================
        // MARK: - ● Method (from Protocol)─────

        public override Vector2 CalcSize(Vector2 prosposedMaxSize, Vector2 originalSize)
        {
            throw new InvalidOperationException();
        }


        public override void CalcChildSize()
        {
            throw new InvalidOperationException();
        }

        /// すべての子の位置を設定する
        public override void CalcChildPosition()
        {
            throw new InvalidOperationException();
        }


        public override void UpdatePosition(Vector2 position)
        {
            _position = position;
        }



        // ===============================================
        // MARK: - ● Method (original) ─────

        public void SetSize(float value)
        {
            // 親に応じてサイズを決定
            // - 最低値は 0 でも良いが、とりあえず 1 は残しておく
            _size = _type switch
            {
                ELayoutSpacerType.H_SPACER => new(value, 1),
                ELayoutSpacerType.V_SPACER => new(1, value),
                _ => throw new InvalidOperationException(),
            };
        }

        
        public float GetFlexibility()
        {
            return _flexibility;
        }
    }
}

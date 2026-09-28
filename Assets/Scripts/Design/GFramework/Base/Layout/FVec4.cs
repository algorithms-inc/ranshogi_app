using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public struct FVec4
    {
        public float top;
        public float right;
        public float bottom;
        public float left;

        public static readonly FVec4 ZERO = new(x: 0, y: 0);

        public new readonly string ToString() =>
            "(t:" + top.ToString() + ", r: " + right.ToString() + ", b: " + bottom.ToString() + ", l: " + left.ToString() + ")";


        public FVec4(float top, float right, float bottom, float left)
        {
            this.top = top;
            this.right = right;
            this.bottom = bottom;
            this.left = left;
        }

        public FVec4(float xy)
        {
            top = xy;
            right = xy;
            bottom = xy;
            left = xy;
        }

        public FVec4(float x, float y)
        {
            top = y;
            right = x;
            bottom = y;
            left = x;
        }


        /// UD の合計
        public float SumOfUD => top + bottom;

        /// LR の合計
        public float SumOfLR => right + left;

        /// 座標(UP_LEFT)
        public Vector2 UP_LEFT => new Vector2(left, top);


        // ===============================================
        // MARK: - ● Operator: Equatable

        public override readonly bool Equals(object obj)
        {
            bool a = top == ((FVec4)obj).top;
            bool b = right == ((FVec4)obj).right;
            bool c = bottom == ((FVec4)obj).bottom;
            bool d = left == ((FVec4)obj).left;

            return a && b && c && d;
        }


        public override readonly int GetHashCode() => 0;


        public static bool operator ==(FVec4 left, FVec4 right)
        {
            bool a = left.top == right.top;
            bool b = left.right == right.right;
            bool c = left.bottom == right.bottom;
            bool d = left.left == right.left;

            return a && b && c && d;
        }


        public static bool operator !=(FVec4 left, FVec4 right)
        {
            return !(left == right);
        }
    }
}

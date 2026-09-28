using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public struct FVecParameter
    {
        // ===============================================
        // MARK: - ● Property ─────

        public EParamType type;
        public Vector2 parameter;


        // ===============================================
        // MARK: - ● Constructor ─────

        /// RATIO/POINTとパラメータで初期化
        /// - Parameters:
        ///   - type: RATIO or POINT
        ///   - parameter: パラメータ
        public FVecParameter(EParamType type, Vector2 parameter)
        {
            this.type = type;
            this.parameter = parameter;
        }



        // ===============================================
        // MARK: - ● Operator: Equatable ─────

        public override readonly bool Equals(object obj) => 
            type == ((FVecParameter)obj).type && parameter == ((FVecParameter)obj).parameter;


        public override readonly int GetHashCode() => 0;


        public static bool operator ==(FVecParameter left, FVecParameter right) =>
            (left.type == right.type) && (left.parameter == right.parameter);


        public static bool operator !=(FVecParameter left, FVecParameter right) =>
            !(left == right);
    }




    public struct FArea
    {
        // ===============================================
        // MARK: - ● Enum ─────

        public enum EAreaConstructorType
        {
            ORIGIN_SIZE,
            ORIGIN_ENDPOINT,
            CENTER_SIZE
        }


        // ===============================================
        // MARK: - ● Property

        /// 原点
        public Vector2 origin;

        /// サイズ
        public Vector2 size;



        /// 中央点
        public Vector2 CenterPoint => new(origin.x + (size.x / 2), origin.y + (size.y / 2));

        /// 上の点
        public Vector2 TopPoint => new(origin.x + (size.x / 2), origin.y);

        /// 右上の点
        public Vector2 TopRightPoint => new(origin.x + size.x, origin.y);

        /// 右の点
        public Vector2 RightPoint => new(origin.x + size.x, origin.y + (size.y / 2));

        /// 左下の点
        public Vector2 BottomLeftPoint => new(origin.x, origin.y + size.y);

        /// 左の点
        public Vector2 LeftPoint => new(origin.x, origin.y + (size.y / 2));

        /// 左上の点
        public Vector2 TopLeftPoint => origin;

        /// 下の点
        public Vector2 BottomPoint => new(origin.x + (size.x / 2), origin.y + size.y);

        /// 終点
        public Vector2 EndPoint => new(origin.x + size.x, origin.y + size.y);


        /// 文字列化
        public override readonly string ToString()
        {
            return "origin(" + origin.x.ToString() + ", " + origin.y.ToString() + "), size(" + size.x.ToString() + ", " + size.y.ToString() + ")";
        }


        /// Y軸対称で変換したものを取得
        public FArea AsYReversed()
        {
            Vector2 newCenterPoint = CenterPoint.AsYReversed();
            return new FArea(p1: newCenterPoint, p2: size, areaConstructorType: EAreaConstructorType.CENTER_SIZE);
        }



        // ===============================================
        // MARK: - ● Constant

        /// ZERO
        public static FArea ZERO = new(origin: new Vector2(0, 0), size: new Vector2(0, 0));



        // ===============================================
        // MARK: - ● Constructor

        /// 原点とサイズで初期化
        /// - Parameters:
        ///   - origin: 原点位置
        ///   - size: サイズ
        public FArea(Vector2 origin, Vector2 size)
        {
            this.origin = origin;
            this.size = size;
        }


        /// 計算方法を指定して初期化
        /// - Parameters:
        ///   - p1: パラメータ1
        ///   - p2: パラメータ2
        ///   - areaConstructorType: 計算方法
        /// - 座標の裏返りはチェックしていない
        public FArea(Vector2 p1, Vector2 p2, EAreaConstructorType areaConstructorType)
        {
            switch (areaConstructorType)
            {
                case EAreaConstructorType.ORIGIN_SIZE:
                    origin = p1;
                    size = p2;
                    break;

                case EAreaConstructorType.ORIGIN_ENDPOINT:
                    origin = p1;
                    size = p2 - p1;
                    break;

                case EAreaConstructorType.CENTER_SIZE:
                    origin = p1 - (p2 / 2);
                    size = p2;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }


        /// エリアで初期化
        /// - Parameters:
        ///   - origin: 原点位置
        ///   - size: サイズ
        public FArea(FArea area)
        {
            origin = area.origin;
            size = area.size;
        }


        /// Rect で初期化（座標系の違いに注意）
        /// - Parameters:
        ///   - rect: Unity Rect
        public FArea(Rect rect)
        {
            origin = new Vector2(x: rect.xMin, y: rect.yMin);
            size = rect.size;
        }


        /// 移動
        public void Translate(Vector2 translation)
        {
            origin += translation;
        }



        // ===============================================
        // MARK: - ● Get Point / Partial Area

        /// アンカーポイントから指定の量・割合をずらしたポイントを取得する
        public readonly Vector2 GetPoint(Vector2 anchorRatio, FVecParameter translationParameter)
        {
            return translationParameter.type switch
            {
                EParamType.RATIO => origin + size * (anchorRatio + translationParameter.parameter),
                EParamType.VALUE => origin + size * anchorRatio + translationParameter.parameter,
                _ => throw new InvalidOperationException(),
            };
        }


        /// エリアの原点を基準とし、サイズに対する割合から座標を得る
        public readonly Vector2 GetPointByRatioFromOrigin(Vector2 ratio) =>
            origin + size * ratio;


        /// エリアから一部を切り出す(原点と終点を比率で指定)
        public readonly FArea GetPartialAreaByOriginAndEndPoint(Vector2 originRatio, Vector2 endPointRatio)
        {
            var newOrigin = GetPointByRatioFromOrigin(ratio: originRatio);
            var newEndPoint = GetPointByRatioFromOrigin(ratio: endPointRatio);

            return new FArea(p1: newOrigin, p2: newEndPoint, areaConstructorType: EAreaConstructorType.ORIGIN_ENDPOINT);
        }


        /// エリアから一部を切り出す(原点とサイズを比率で指定)
        public readonly FArea GetPartialAreaByOriginAndSize(Vector2 originRatio, Vector2 sizeRatio)
        {
            var newOrigin = GetPointByRatioFromOrigin(ratio: originRatio);
            var newEndPoint = newOrigin + size * sizeRatio;

            return new FArea(p1: newOrigin, p2: newEndPoint, areaConstructorType: EAreaConstructorType.ORIGIN_ENDPOINT);
        }


        /// <summary>
        /// ratio 比となる最大の内部エリアを取得
        /// </summary>
        public FArea GetPartialArea(Vector2 ratio) =>
            GetPartialArea(XtoYRatio: ratio.y / ratio.x);


        /// <summary>
        /// y/x 比となる最大の内部エリアを取得
        /// </summary>
        /// <param name="XtoYRatio"></param>
        /// <returns></returns>
        public FArea GetPartialArea(float XtoYRatio)
        {
            Assert.IsTrue(size.x * size.y != 0);

            float XtoYRatioOfSelf = size.y / size.x;

            if (XtoYRatioOfSelf <= XtoYRatio)
            {
                return new FArea(
                    p1: CenterPoint,
                    p2: new Vector2(x: size.y / XtoYRatio, y: size.y),
                    areaConstructorType: EAreaConstructorType.CENTER_SIZE);
            }
            else
            {
                return new FArea(
                    p1: CenterPoint,
                    p2: new Vector2(x: size.x, y: size.x * XtoYRatio),
                    areaConstructorType: EAreaConstructorType.CENTER_SIZE);
            }
        }



        // ===============================================
        // MARK: - ● Operator: Equatable

        public override readonly bool Equals(object obj) =>
            origin == ((FArea)obj).origin && size == ((FArea)obj).size;


        public override int GetHashCode() => 0;


        public static bool operator ==(FArea left, FArea right) =>
            (left.origin == right.origin) && (left.size == right.size);


        public static bool operator !=(FArea left, FArea right) =>
            !(left == right);



        // ===============================================
        // MARK: - ● Operator: with FSpace4

        public static FArea operator +(FArea left, FVec4 right)
        {
            Vector2 resultOrigin = left.origin - new Vector2(right.left, right.top);
            Vector2 resultSize = left.size + new Vector2(right.left + right.right, right.top + right.bottom);

            return new FArea(origin: resultOrigin, size: resultSize);
        }


        public static FArea operator -(FArea left, FVec4 right)
        {
            Vector2 resultOrigin = left.origin + new Vector2(right.left, right.top);
            Vector2 resultSize = left.size - new Vector2(right.left + right.right, right.top + right.bottom);

            return new FArea(origin: resultOrigin, size: resultSize);
        }
    }
}

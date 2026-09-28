using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public interface ILayoutSizeSetting
    {
        Vector2 Size { get; }
        Vector2 CalcAndUpdateSize(Vector2? maxSize, Vector2 originalSize);
    }


    public struct FLayoutSizeSettingByValue : ILayoutSizeSetting
    {
        private Vector2 _size;
        public Vector2 Size => _size;


        public FLayoutSizeSettingByValue(Vector2 size)
        {
            _size = size;
        }


        public Vector2 CalcAndUpdateSize(Vector2? maxSize, Vector2 originalSize)
        {
            // maxSize.hasValue でも判定可能
            if (maxSize != null)
            {
                // null許容型の場合、.Value で実際の値にアクセスできる
                _size = new Vector2(
                    Math.Min(_size.x, maxSize.Value.x),
                    Math.Min(_size.y, maxSize.Value.y));
            }

            return _size;
        }
    }



    public struct FLayoutSizeSettingByRatio : ILayoutSizeSetting
    {
        public Vector2 ratio;

        private Vector2 _size;
        public Vector2 Size => _size;

        public static FLayoutSizeSettingByRatio FULL_SIZE = new(ratio: new Vector2(1f, 1f));


        public FLayoutSizeSettingByRatio(Vector2 ratio)
        {
            this.ratio = ratio;
            _size = Vector2.zero;
        }


        public Vector2 CalcAndUpdateSize(Vector2? maxSize, Vector2 originalSize)
        {
            _size = originalSize * ratio;

            if (maxSize != null)
            {
                _size = new Vector2(
                    Math.Min(_size.x, maxSize.Value.x),
                    Math.Min(_size.y, maxSize.Value.y));
            }

            return _size;
        }
    }


    public struct FLayoutSizeSettingByRatioAndMinMax : ILayoutSizeSetting
    {
        public Vector2 ratio;
        public FMinMaxVec minMaxValue;

        private Vector2 _size;
        public Vector2 Size => _size;


        public FLayoutSizeSettingByRatioAndMinMax(Vector2 ratio, FMinMaxVec minMaxValue)
        {
            this.ratio = ratio;
            this.minMaxValue = minMaxValue;
            _size = Vector2.zero;
        }

        public Vector2 CalcAndUpdateSize(Vector2? maxSize, Vector2 originalSize)
        {
            _size = minMaxValue.Clamp(originalSize * ratio);

            if (maxSize != null)
            {
                _size = new Vector2(
                    Math.Min(_size.x, maxSize.Value.x),
                    Math.Min(_size.y, maxSize.Value.y));
            }

            return _size;
        }
    }


    /// <summary>
    /// Y/X の比率を指定して最大値を取るようにする
    /// </summary>
    public struct FLayoutSizeSettingByXYRatio : ILayoutSizeSetting
    {
        public float XYRatio;

        private Vector2 _size;
        public Vector2 Size => _size;


        public FLayoutSizeSettingByXYRatio(float XYRatio)
        {
            this.XYRatio = XYRatio;
            _size = Vector2.zero;
        }


        public Vector2 CalcAndUpdateSize(Vector2? maxSize, Vector2 originalSize)
        {
            float maxX;
            float maxY;

            if (maxSize != null)
            {
                maxX = maxSize.Value.x;
                maxY = maxSize.Value.y;
            }
            else
            {
                maxX = originalSize.x;
                maxY = originalSize.y;
            }


            if (maxY / maxX < XYRatio)
            {
                // maxY をそのまま用いる
                _size = new Vector2(
                    maxY / XYRatio,
                    maxY);
            }
            else
            {
                // maxX をそのまま用いる
                _size = new Vector2(
                    maxX,
                    maxX * XYRatio);
            }

            return _size;
        }
    }
}

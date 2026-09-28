using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;



namespace GFramework.Core
{
    /// <summary>
    /// ウィンドウ内の要素の位置・サイズ設定
    /// </summary>
    public struct FWindowElementArea
    {
        private FArea _area;
        public readonly FArea Area => _area;

        public readonly Vector2 Origin => Area.origin;
        public readonly Vector2 Size => Area.size;
        public readonly Vector2 EndPoint => Area.EndPoint;


        /// <summary>
        /// 開始点と終点で指定
        /// </summary>
        /// <param name="origin">開始点：(x,y) = (0f, 0f) - (1f, 1.6f)</param>
        /// <param name="endPoint">終点：(x,y) = (0f, 0f) - (1f, 1.6f)</param>
        public FWindowElementArea(
            Vector2 origin,
            Vector2 endPoint) : this(origin: origin, width: endPoint.x - origin.x, height: endPoint.y - origin.y) {}


        /// <summary>
        /// 開始点と幅、高さで指定
        /// </summary>
        /// <param name="origin">開始点：(x,y) = (0f, 0f) - (1f, 1.6f) or (0f, 0f) - (0.8f, 0.6f)</param>
        /// <param name="width">幅：0f - 1f or 0f - 0.8f</param>
        /// <param name="height">高さ：0f - 1.6f or 0f - 0.6f</param>
        public FWindowElementArea(
            Vector2 origin,
            float width,
            float height)
        {
            _area = new FArea(origin: origin, size: new Vector2(x: width, y: height));
        }


        public readonly void Validate(EWindowType windowType)
        {
            switch (windowType)
            {
                case EWindowType.TOP:
                    ValidateAreaAsWindowTop();
                    break;

                case EWindowType.NORMAL:
                    ValidateAreaAsWindowL();
                    break;

                case EWindowType.CONFIRMATION:
                    ValidateAreaAsWindowS();
                    break;

               case EWindowType.ERROR:
                    ValidateAreaAsWindowL();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }


        // TODO: 正しいチェックとなるように数値を変更する
        private readonly void ValidateAreaAsWindowTop()
        {
            var isValid = true;

            if ((Area.origin.x < 0) || (Area.origin.x > 1.0f)) { isValid = false; }
            if ((Area.origin.y < 0) || (Area.origin.y > 0.5f)) { isValid = false; }

            if ((Area.EndPoint.x < 0) || (Area.EndPoint.x > 1.0f)) { isValid = false; }
            if ((Area.EndPoint.y < 0) || (Area.EndPoint.y > 0.5f)) { isValid = false; }

            if (isValid == false)
            {
                Debug.Log("FWindowElementArea for Top window is not in the window UI area. (0f <= x <= 1f, 0f <= y <= 0.5f)");
            }
        }


        private readonly void ValidateAreaAsWindowL()
        {
            var isValid = true;

            if ((Area.origin.x < 0) || (Area.origin.x > 1.0f)) { isValid = false; }
            if ((Area.origin.y < 0) || (Area.origin.y > 1.6f)) { isValid = false; }

            if ((Area.EndPoint.x < 0) || (Area.EndPoint.x > 1.0f)) { isValid = false; }
            if ((Area.EndPoint.y < 0) || (Area.EndPoint.y > 1.6f)) { isValid = false; }

            if (isValid == false)
            {
                Debug.Log("FWindowElementArea for large size window is not in the window UI area. (0f <= x <= 1f, 0f <= y <= 1.6f)");
            }
        }


        private readonly void ValidateAreaAsWindowS()
        {
            var isValid = true;

            if ((Area.origin.x < 0) || (Area.origin.x > Config.WindowConfirmationSizeRatio.x)) { isValid = false; }
            if ((Area.origin.y < 0) || (Area.origin.y > Config.WindowConfirmationSizeRatio.y)) { isValid = false; }

            if ((Area.EndPoint.x < 0) || (Area.EndPoint.x > Config.WindowConfirmationSizeRatio.x)) { isValid = false; }
            if ((Area.EndPoint.y < 0) || (Area.EndPoint.y > Config.WindowConfirmationSizeRatio.y)) { isValid = false; }

            if (isValid == false)
            {
                var x = Config.WindowConfirmationSizeRatio.x;
                var y = Config.WindowConfirmationSizeRatio.y;

                Debug.Log("FWindowElementArea for small size window is not in the window UI area. (0f <= x <= " + x + "f, 0f <= y <= " + y + "f)");
            }
        }
    }
}
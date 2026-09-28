using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public struct FMinMax
    {
        private readonly float _min;
        private readonly float _max;

        private readonly bool _hasMin;
        private readonly bool _hasMax;

        public readonly float Min
        {
            get
            {
                Assert.IsTrue(_hasMin);
                return _min;
            }
        }


        public readonly float Max
        {
            get
            {
                Assert.IsTrue(_hasMax);
                return _max;
            }
        }

        public bool HasMin => _hasMin;
        public bool HasMax => _hasMax;


        public static FMinMax NONE => new(min: 0, max: 0, hasMin: false, hasMax: false);



        public FMinMax(float min, float max, bool hasMin, bool hasMax)
        {
            _min = min;
            _max = max;

            _hasMin = hasMin;
            _hasMax = hasMax;
        }


        public readonly float Clamp(float value)
        {
            if (_hasMin)
            {
                if (value < _min) { return _min; }
             }

            if (_hasMax)
            {
                if (value > _max) { return _max; }
            }

            return value;
        }
    }



    public struct FMinMaxVec
    {
        private readonly FMinMax _minMaxX;
        private readonly FMinMax _minMaxY;


        public FMinMax MinMaxX => _minMaxX;
        public FMinMax NinMaxY => _minMaxY;


        public static FMinMaxVec NONE => new(minMaxX: new FMinMax(min: 0, max: 0, hasMin: false, hasMax: false), minMaxY: new FMinMax(min: 0, max: 0, hasMin: false, hasMax: false));


        public FMinMaxVec(in FMinMax minMaxX, in FMinMax minMaxY)
        {
            _minMaxX = minMaxX;
            _minMaxY = minMaxY;
        }


        public readonly Vector2 Clamp(Vector2 value) =>
            new(_minMaxX.Clamp(value.x), _minMaxY.Clamp(value.y));
    }
}
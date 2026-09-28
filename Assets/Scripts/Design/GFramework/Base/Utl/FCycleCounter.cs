using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework.Core
{
    /// <summary>
    /// 最大値と最小値をループするサイクルカウンタ
    /// </summary>
    public struct FCycleCounter
    {
        public int _value;
        public int Value
        {
            set
            {
                Assert.IsTrue(_min <= value);
                Assert.IsTrue(value <= _max);
                _value = value;
            }
            get { return _value; }
        }

        private readonly int _min;
        public int Min => _min;

        private readonly int _max;
        public int Max => _max;

        public FCycleCounter(int count, int min, int max)
        {
            Assert.IsTrue(min <= max);

            _value = count;
            _min = min;
            _max = max;
        }


        public void Increment()
        {
            _value = WrapValue(_value + 1);

        }

        public void Decrement()
        {
            _value = WrapValue(_value - 1);
        }

        private readonly int WrapValue(int value)
        {
            if (value < _min) { return _max; }
            if (_max < value) { return _min; }

            return value;
        }
    }
}

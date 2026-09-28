using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CSwingCounter : CCounter
    {
        // ===============================================
        // MARK: - ● Property ─────

        private int _count;
        public override int Count => _count;

        private int _min;
        public override int Min => _min;

        private int _max;
        public override int Max => _max;

        private bool _isForwardDirection;
        public override bool IsForwardDirection => _isForwardDirection;

        public override string AsString => "{_count} / {_min}-{_max} forwardDirection: {_isForwardDirection}";

        public override CCounter Copy => new CSwingCounter(count: _count, min: _min, max: _max, isForwardDirection: _isForwardDirection);


        // ===============================================
        // MARK: - ● Constructor ─────

        public CSwingCounter(int count = 0, int min = 0, int max = int.MaxValue, bool isForwardDirection = true)
        {
            _count = count;
            _min = min;
            _max = max;
            _isForwardDirection = isForwardDirection;


            Assert.IsTrue(Utl.IsInRange(value: ref _count, min: ref _min, max: ref _max));
            Assert.IsTrue(_min < _max);
        }



        // ===============================================
        // MARK: - ● Method ─────

        public override void SetPresentCount(int value)
        {
            Assert.IsTrue(Utl.IsInRange(value: ref value, min: ref _min, max: ref _max));

            _count = value;
        }


        public override void SetIsForwardDirection(bool value)
        {
            _isForwardDirection = value;
        }


        public override void Up()
        {
            if (_isForwardDirection)
            {
                if (_count == _max)
                {
                    _count -= 1;
                    _isForwardDirection = false;
                }
                else
                {
                    _count += 1;
                }
            }
            else // case: (_isForwardDirection == false)
            {
                if (_count == _min)
                {
                    _count += 1;
                    _isForwardDirection = true;
                }
                else
                {
                    _count -= 1;
                }
            }
        }
        

        public override void Down()
        {
            if (_isForwardDirection)
            {
                if (_count == _min)
                {
                    _count += 1;
                    _isForwardDirection = true;
                }
                else
                {
                    _count -= 1;
                }

            }
            else // case: (_isForwardDirection == false)
            {
                if (_count == _max)
                {
                    _count -= 1;
                    _isForwardDirection = false;
                }
                else
                {
                    _count += 1;
                }
            }
        }
    }
}

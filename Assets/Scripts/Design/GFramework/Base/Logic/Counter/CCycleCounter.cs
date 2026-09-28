using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class CCycleCounter : CCounter
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

        public override string AsString => "{_count} / {_min}-{_max} cycle: {_cycle} forwardDirection: {_isForwardDirection}";

        public override CCounter Copy => new CCycleCounter(count: _count, min: _min, cycle: _cycle, isForwardDirection: _isForwardDirection);

        private int _cycle;
        public int Cycle => _cycle;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CCycleCounter(int count = 0, int min = 0, int cycle = int.MaxValue, bool isForwardDirection = true)
        {
            _count = count;
            _min = min;
            _max = cycle - 1;
            _cycle = cycle;
            _isForwardDirection = isForwardDirection;

            Assert.IsTrue(Utl.IsInRange(value: ref _count, min: ref _min, max: ref _max));
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
                    _count = _min;
                }
                else
                {
                    _count += 1;
                }
            }
            else
            {
                if (_count == _min)
                {
                    _count = _max;
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
                    _count = _max;
                }
                else
                {
                    _count -= 1;
                }
            }
            else
            {
                if (_count == _max)
                {
                    _count = _min;
                }
                else
                {
                    _count += 1;
                }
            }
        }
    }
}
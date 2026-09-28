using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public abstract class CCounter
    {
        // ===============================================
        // MARK: - ● Property ─────

        public abstract int Count { get; }
        public abstract int Min { get; }
        public abstract int Max { get; }
        public abstract bool IsForwardDirection { get; }
        public abstract string AsString { get; }
        public abstract CCounter Copy { get; }

        public bool IsMin => Count == Min;
        public bool IsMax => Count == Max;
        public int DeltaFromMin => Count - Min;
        public int DeltaToMax => Max - Count;
        public float Alpha => (float)(Count - Min) / (float)(Max - Min);
        public float AlphaReversed => 1f - Alpha;


        // ===============================================
        // MARK: - ● Method ─────

        public abstract void SetPresentCount(int value);
        public abstract void SetIsForwardDirection(bool value);
        public abstract void Up();
        public abstract void Down();
        

        public void SetToMin()
        {
            SetPresentCount(value: Min);
        }


        public void SetToMax()
        {
            SetPresentCount(value: Max);
        }


        public void ReverseDirection()
        {
            SetIsForwardDirection(value: !IsForwardDirection);
        }


        public bool IsMultiple(int of)
        {
            return Count % of == 0;
        }
    }
}

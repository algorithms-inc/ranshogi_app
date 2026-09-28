using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public struct FColor3
    {
        private float _r;
        private float _g;
        private float _b;

        public float R => _r;
        public float G => _g;
        public float B => _b;

        public FColor3(
            float r,
            float g,
            float b)
        {
            _r = r;
            _g = g;
            _b = b;
        }

        public FColor3(
            UnityEngine.Color systemColor)
        {
            _r = systemColor.r;
            _g = systemColor.g;
            _b = systemColor.b;
        }


        public UnityEngine.Color AsSystemColor()
        {
            return new UnityEngine.Color(r: _r, g: _g, b: _b, a: 1.0f);
        }

        public UnityEngine.Color AsSystemColor(float a)
        {
            return new UnityEngine.Color(r: _r, g: _g, b: _b, a: a);
        }

        public FColor4 AsColor4()
        {
            return new FColor4(r: _r, g: _g, b: _b, a: 1.0f);
        }

        public FColor4 AsColor4(float a)
        {
            return new FColor4(r: _r, g: _g, b: _b, a: a);
        }

        // ===============================================
        // MARK: - ● Operator: Equatable

        public override bool Equals(object obj)
        {
            return _r == ((FColor3)obj)._r && _g == ((FColor3)obj)._g && _b == ((FColor3)obj)._b; 
        }


        public override int GetHashCode()
        {
            return 0;
        }


        public static bool operator ==(FColor3 left, FColor3 right)
        {
            return (left._r == right._r) && (left._g == right._g) && (left._b == right._b);
        }


        public static bool operator !=(FColor3 left, FColor3 right)
        {
            return !(left == right);
        }



        // ===============================================
        // MARK: - ● Operator

        public static FColor3 operator +(FColor3 left, FColor3 right)
        {
            float newR = left._r + right._r;
            float newG = left._g + right._g;
            float newB = left._b + right._b;

            return new FColor3(r: newR, g: newG, b: newB);
        }


        public static FColor3 operator -(FColor3 left, FColor3 right)
        {
            float newR = left._r - right._r;
            float newG = left._g - right._g;
            float newB = left._b - right._b;

            return new FColor3(r: newR, g: newG, b: newB);
        }


        public static FColor3 operator *(FColor3 left, float right)
        {
            float newR = left._r * right;
            float newG = left._g * right;
            float newB = left._b * right;

            return new FColor3(r: newR, g: newG, b: newB);
        }
    }



    public struct FColor4
    {
        private float r;
        private float g;
        private float b;
        private float a;

        public float R => r;
        public float G => g;
        public float B => b;
        public float A => a;

        public FColor4(
            float r,
            float g,
            float b,
            float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public FColor4(
            FColor3 color,
            float a)
        {
            this.r = color.R;
            this.g = color.G;
            this.b = color.B;
            this.a = a;
        }


        public UnityEngine.Color AsSystemColor()
        {
            return new UnityEngine.Color(r: r, g: g, b: b, a: a);
        }


        // ===============================================
        // MARK: - ● Operator: Equatable

        public override bool Equals(object obj)
        {
            return r == ((FColor4)obj).r && g == ((FColor4)obj).g && b == ((FColor4)obj).b && a == ((FColor4)obj).a;
        }


        public override int GetHashCode()
        {
            return 0;
        }


        public static bool operator ==(FColor4 left, FColor4 right)
        {
            return (left.r == right.r) && (left.g == right.g) && (left.b == right.b) && (left.a == right.a);
        }


        public static bool operator !=(FColor4 left, FColor4 right)
        {
            return !(left == right);
        }



        // ===============================================
        // MARK: - ● Operator

        public static FColor4 operator +(FColor4 left, FColor4 right)
        {
            float newR = left.r + right.r;
            float newG = left.g + right.g;
            float newB = left.b + right.b;
            float newA = left.a + right.a;

            return new FColor4(r: newR, g: newG, b: newB, a: newA);
        }


        public static FColor4 operator -(FColor4 left, FColor4 right)
        {
            float newR = left.r - right.r;
            float newG = left.g - right.g;
            float newB = left.b - right.b;
            float newA = left.a - right.a;

            return new FColor4(r: newR, g: newG, b: newB, a: newA);
        }


        public static FColor4 operator *(FColor4 left, float right)
        {
            float newR = left.r * right;
            float newG = left.g * right;
            float newB = left.b * right;
            float newA = left.a * right;

            return new FColor4(r: newR, g: newG, b: newB, a: newA);
        }
    }
}
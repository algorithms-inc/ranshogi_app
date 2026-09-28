using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public static class Interpole
    {
        public const float PI = 3.1415f;

        // ===============================================
        // MARK: - ● Linear ─────

        public static float Linear(float begin, float end, float alpha) => (end - begin) * alpha + begin;

        public static Vector2 Linear(Vector2 begin, Vector2 end, float alpha) => (end - begin) * alpha + begin;

        public static Vector3 Linear(Vector3 begin, Vector3 end, float alpha) => (end - begin) * alpha + begin;

        public static FColor3 Linear(FColor3 begin, FColor3 end, float alpha) => (end - begin) * alpha + begin;

        // ===============================================
        // MARK: - ● SinCurveIn ─────

        public static float SinCurveIn(float begin, float end, float alpha) => (end - begin) * SinCurveInAlpha(alpha) + begin;

        public static Vector2 SinCurveIn(Vector2 begin, Vector2 end, float alpha) => (end - begin) * SinCurveInAlpha(alpha) + begin;

        public static Vector3 SinCurveIn(Vector3 begin, Vector3 end, float alpha) => (end - begin) * SinCurveInAlpha(alpha) + begin;

        public static FColor3 SinCurveIn(FColor3 begin, FColor3 end, float alpha) => (end - begin) * SinCurveInAlpha(alpha) + begin;

        public static float SinCurveInAlpha(float alpha) => 1 - Mathf.Cos((PI / 2) * alpha);

        // ===============================================
        // MARK: - ● SinCurveOut ─────

        public static float SinCurveOut(float begin, float end, float alpha) => (end - begin) * SinCurveOutAlpha(alpha) + begin;

        public static Vector2 SinCurveOut(Vector2 begin, Vector2 end, float alpha) => (end - begin) * SinCurveOutAlpha(alpha) + begin;

        public static Vector3 SinCurveOut(Vector3 begin, Vector3 end, float alpha) => (end - begin) * SinCurveOutAlpha(alpha) + begin;

        public static FColor3 SinCurveOut(FColor3 begin, FColor3 end, float alpha) => (end - begin) * SinCurveOutAlpha(alpha) + begin;

        public static float SinCurveOutAlpha(float alpha) => Mathf.Sin((PI / 2) * alpha);

        // ===============================================
        // MARK: - ● SinCurveInOut ─────

        public static float SinCurveInOut(float begin, float end, float alpha) => (end - begin) * SinCurveInOutAlpha(alpha) + begin;

        public static Vector2 SinCurveInOut(Vector2 begin, Vector2 end, float alpha) => (end - begin) * SinCurveInOutAlpha(alpha) + begin;

        public static Vector3 SinCurveInOut(Vector3 begin, Vector3 end, float alpha) => (end - begin) * SinCurveInOutAlpha(alpha) + begin;

        public static FColor3 SinCurveInOut(FColor3 begin, FColor3 end, float alpha) => (end - begin) * SinCurveInOutAlpha(alpha) + begin;

        public static float SinCurveInOutAlpha(float alpha)
        {
            float doubledAlpha = alpha * 2;
            return doubledAlpha < 1
                ? SinCurveInAlpha(doubledAlpha) / 2
                : (SinCurveOutAlpha(doubledAlpha - 1) + 1) / 2;
        }

        // ===============================================
        // MARK: - ● SinCurveInStrong ─────

        public static float SinCurveInStrong(float begin, float end, float alpha) => (end - begin) * SinCurveInStrongAlpha(alpha) + begin;

        public static Vector2 SinCurveInStrong(Vector2 begin, Vector2 end, float alpha) => (end - begin) * SinCurveInStrongAlpha(alpha) + begin;

        public static Vector3 SinCurveInStrong(Vector3 begin, Vector3 end, float alpha) => (end - begin) * SinCurveInStrongAlpha(alpha) + begin;

        public static FColor3 SinCurveInStrong(FColor3 begin, FColor3 end, float alpha) => (end - begin) * SinCurveInStrongAlpha(alpha) + begin;

        public static float SinCurveInStrongAlpha(float alpha) => (1 - Mathf.Cos((PI / 2) * alpha)) * alpha;

        // ===============================================
        // MARK: - ● SinCurveOutStrong ─────

        public static float SinCurveOutStrong(float begin, float end, float alpha) => (end - begin) * SinCurveOutStrongAlpha(alpha) + begin;

        public static Vector2 SinCurveOutStrong(Vector2 begin, Vector2 end, float alpha) => (end - begin) * SinCurveOutStrongAlpha(alpha) + begin;

        public static Vector3 SinCurveOutStrong(Vector3 begin, Vector3 end, float alpha) => (end - begin) * SinCurveOutStrongAlpha(alpha) + begin;

        public static FColor3 SinCurveOutStrong(FColor3 begin, FColor3 end, float alpha) => (end - begin) * SinCurveOutStrongAlpha(alpha) + begin;

        public static float SinCurveOutStrongAlpha(float alpha)
        {
            float calc = Mathf.Sin((PI / 2) * alpha);
            return 1 - (1 - calc) * (1 - alpha);
        }

        // ===============================================
        // MARK: - ● SinCurveInOutStrong ─────

        public static float SinCurveInOutStrong(float begin, float end, float alpha) => (end - begin) * SinCurveInOutAlphaStrong(alpha) + begin;

        public static Vector2 SinCurveInOutStrong(Vector2 begin, Vector2 end, float alpha) => (end - begin) * SinCurveInOutAlphaStrong(alpha) + begin;

        public static Vector3 SinCurveInOutStrong(Vector3 begin, Vector3 end, float alpha) => (end - begin) * SinCurveInOutAlphaStrong(alpha) + begin;

        public static FColor3 SinCurveInOutStrong(FColor3 begin, FColor3 end, float alpha) => (end - begin) * SinCurveInOutAlphaStrong(alpha) + begin;

        public static float SinCurveInOutAlphaStrong(float alpha)
        {
            float doubledAlpha = alpha * 2;
            return doubledAlpha < 1
                ? SinCurveInStrongAlpha(doubledAlpha) / 2
                : (SinCurveOutStrongAlpha(doubledAlpha - 1) + 1) / 2;
        }

        // ===============================================
        // MARK: - ● BounceUpWithDown ─────

        public static float BounceUpDown(float baseValue, float peakValue, float alpha) => (peakValue - baseValue) * BounceUpDownAlpha(alpha) + baseValue;

        public static Vector2 BounceUpDown(Vector2 baseValue, Vector2 peakValue, float alpha) => (peakValue - baseValue) * BounceUpDownAlpha(alpha) + baseValue;

        public static Vector3 BounceUpDown(Vector3 baseValue, Vector3 peakValue, float alpha) => (peakValue - baseValue) * BounceUpDownAlpha(alpha) + baseValue;

        public static FColor3 BounceUpDown(FColor3 baseValue, FColor3 peakValue, float alpha) => (peakValue - baseValue) * BounceUpDownAlpha(alpha) + baseValue;

        public static float BounceUpDownAlpha(float alpha) => 11 * alpha * (alpha - 0.731f) * (alpha - 1);

        // ===============================================
        // MARK: - ● Parabolic ─────

        public static float Parabolic(float baseValue, float peakValue, float alpha) => (peakValue - baseValue) * ParabolicAlpha(alpha) + baseValue;

        public static Vector2 Parabolic(Vector2 baseValue, Vector2 peakValue, float alpha) => (peakValue - baseValue) * ParabolicAlpha(alpha) + baseValue;

        public static Vector3 Parabolic(Vector3 baseValue, Vector3 peakValue, float alpha) => (peakValue - baseValue) * ParabolicAlpha(alpha) + baseValue;

        public static FColor3 Parabolic(FColor3 baseValue, FColor3 peakValue, float alpha) => (peakValue - baseValue) * ParabolicAlpha(alpha) + baseValue;

        public static float ParabolicAlpha(float alpha) => 4 * alpha * (1 - alpha);

        // ===============================================
        // MARK: - ● Circular ─────

        public static float Circular(float baseValue, float peakValue, float alpha) => (peakValue - baseValue) * CircularAlpha(alpha) + baseValue;

        public static Vector2 Circular(Vector2 baseValue, Vector2 peakValue, float alpha) => (peakValue - baseValue) * CircularAlpha(alpha) + baseValue;

        public static Vector3 Circular(Vector3 baseValuse, Vector3 peakValue, float alpha) => (peakValue - baseValuse) * CircularAlpha(alpha) + baseValuse;

        public static FColor3 Circular(FColor3 baseValue, FColor3 peakValue, float alpha) => (peakValue - baseValue) * CircularAlpha(alpha) + baseValue;

        public static float CircularAlpha(float alpha) => 2 * Mathf.Sqrt(alpha * (1 - alpha));
    }
}
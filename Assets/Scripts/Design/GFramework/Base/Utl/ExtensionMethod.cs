using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GFramework
{
    public static class SpriteRendererExt
    {
        // デフォルトのスプライトマテリアルは共有されているので、その場合は self.color の値を変更し、self.material.color を変更してしまうと、全てのスプライトで変更されてしまう。
        // 乱将棋ように用意したオリジナルのスプライトマテリアルを個別に使用している場合、self.color の変更は無効隣、self.material.color を変更する必要がある。
        public static void SetMaterialAlpha(this SpriteRenderer self, float alpha)
        {
            // 複製されないと思われるが念の為 sharedMaterial でアクセス
            var color = self.sharedMaterial.color;
            color.a = alpha;
            self.sharedMaterial.color = color;
        }

        public static float GetMaterialAlpha(this SpriteRenderer self)
        {
            // 複製されないと思われるが念の為 sharedMaterial でアクセス
            return self.sharedMaterial.color.a;
        }

        public static void SetMaterialColor(this SpriteRenderer self, Vector3 color)
        {
            // 複製されないと思われるが念の為 sharedMaterial でアクセス
            var materialAlpha = self.sharedMaterial.color.a;

            self.sharedMaterial.color = new UnityEngine.Color(r: color.x, g: color.y, b: color.z, materialAlpha);
        }
    }


    public static class ImageExt
    {
        // Image は UI用のデフォルトマテリアルを共有して使用している。self.material.color をいじってしまうと、全てのUIで色が変更されてしまう。この点スプライトと使用が異なる点に注意。
        public static void SetAlpha(this Image self, float alpha)
        {
            var color = self.color;
            color.a = alpha;
            self.color = color;
        }


        public static float GetAlpha(this Image self)
        {
            return self.color.a;
        }
    }


    public static class StringExt
    {
        public static string ReplaceAt(this string str, int index, char character)
        {
            return str.Remove(index, 1).Insert(index, character.ToString());
        }
    }


    public static class Vector2Ext
    {
        /// Y/X 比率を取得
        public static float GetXtoYRatio(this Vector2 vector)
        {
            return vector.y / vector.x;
        }

        /// Y軸対称で変換したものを取得
        public static Vector2 AsYReversed(this Vector2 vector)
        {
            return new Vector2(x: vector.x, y: vector.y * -1);
        }
    }
}

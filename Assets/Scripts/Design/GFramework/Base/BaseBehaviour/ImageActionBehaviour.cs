using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public abstract class ImageActionBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private protected float _defaultAlpha = 1f;
        public float DefaultAlpha => _defaultAlpha;

        public abstract float Alpha { get; }

        private protected Vector2 _baseImageSize = new Vector2(0f, 0f);
        public Vector2 BaseImageSize => _baseImageSize;

        public Vector2 LocalScaleOfWhole => gameObject.transform.localScale;

       
        // ===============================================
        // MARK: - ● Method ─────

        public abstract void SetAlpha(float alpha);


        public void SetDefaultAlpha(float alpha)
        {
            _defaultAlpha = alpha;
        }


        public void ResetAlpha()
        {
            SetAlpha(alpha: _defaultAlpha);
        }


        public void SetLocalScaleOfWhole(Vector2 localScale)
        {
            gameObject.transform.localScale = localScale;
        }

        
        public abstract void Show();

        public abstract void Hide();


        public abstract void FadeIn(Action actionOnDidFinish = null);

        public abstract void FadeOut(Action actionOnDidFinish = null);

        protected override void OnDestroy()
        {
            // オリジナルのマテリアルを使用しているため自前で削除を行う
            // 注意：
            // sharedMaterials を使用せず、materials を使用するとこのタイミングで複製が作成されるようで削除してもマテリアルがメモリ上に残る。（Utl.などのスプライトの色変更の際に material を使用していないため）
            // GetComponent<Renderer>() でなけばならない。GetComponent<SpriteRenderer>()の場合、materials は取得できても sharedMaterials は取得できない
            var renderer = gameObject.GetComponent<Renderer>();

            if (renderer != null && renderer.sharedMaterials != null)
            {
                foreach (var m in renderer.sharedMaterials)
                {
                    //DestroyImmediate(m); は推奨されないので、Destory(m)に変更
                    Destroy(m);
                }
            }

            base.OnDestroy();
        }
    }
}
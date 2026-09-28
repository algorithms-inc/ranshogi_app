using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;

namespace GFramework
{
    public enum ESpriteBlendMode
    {
        NORMAL,
        ADD,
    }


    public enum EScaleMode
    {
        /// 縦横比維持・余白あり
        FIT,
        /// 縦横比維持・余白なし（はみ出る） 
        FIT_AND_FILL,
        /// 埋める
        STRETCH, 
    }


    public static class UtlSprite
    {
        #region Add Sprite
        // 原則として、ImageActionBehaviour を継承したクラス内で使用すること。
        // それ以外の場合、マテリアルを削除をしなければメモリリークすることがある。

        public static void AddSpriteToGameObject(
            GameObject gameObject,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder)
        {
            // texture
            Texture2D texture = Locator.Instance.textureManager.LoadTexture(
                filePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: willCacheIfNeeded);

            // renderer
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(
                texture: texture,
                rect: new Rect(0.0f, 0.0f, texture.width, texture.height),
                pivot: new Vector2(0.5f, 0.5f),
                pixelsPerUnit: Config.PIXEL_PER_UNIT);
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 1.0f);

            // material
            spriteRenderer.material = CreateSpriteMaterial(spriteBlendMode: ESpriteBlendMode.NORMAL);
        }


        public static void AddSpriteToGameObjectWithResizableSprite(
            GameObject gameObject,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder,
            Vector4 border)
        {
            // texture
            Texture2D texture = Locator.Instance.textureManager.LoadTexture(
                filePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: willCacheIfNeeded);

            // renderer
            // extrude値：Sprite の周囲に追加されるピクセル数。これにより、Sprite を拡大すると、周囲のピクセルがクリッピングされることがなくなる。extrude の値が 0 の場合、Sprite の周囲に追加のピクセルが追加されない。
            // →他のパラメータの値を設定しているため、省略できないため記述している
            // generateFallbackPhysicsShape値：パラメータは、Sprite の物理形状を自動的に生成するかどうかを指定。→ ひとまず切っている
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.sprite = Sprite.Create(
                texture: texture,
                rect: new Rect(0.0f, 0.0f, texture.width, texture.height),
                pivot: new Vector2(0.5f, 0.5f),
                pixelsPerUnit: Config.PIXEL_PER_UNIT,
                extrude: 0,
                meshType: SpriteMeshType.FullRect,
                border: border,
                generateFallbackPhysicsShape: false);
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 1.0f);

            // material
            spriteRenderer.material = CreateSpriteMaterial(spriteBlendMode: ESpriteBlendMode.NORMAL);
        }
        #endregion

        // private static 



        #region Create GameObject with Sprite

        public static GameObject CreateGameObjectWithSprite(
            string name,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder,
            Transform transformParent)
        {
            GameObject resultGameObject = new GameObject(name);

            AddSpriteToGameObject(
                gameObject: resultGameObject,
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: willCacheIfNeeded,
                sortingOrder: sortingOrder);

            // transform
            resultGameObject.transform.parent = transformParent;
            resultGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

            return resultGameObject;
        }


        public static SingleImageActionBehaviour CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
            string name,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder,
            Transform transformParent,
            in FSingleImageActionBehaviourInitializer initializer)
        {
            var gameObject = CreateGameObjectWithSprite(
                    name: name,
                    texturePathUnderResources: texturePathUnderResources,
                    willCacheIfNeeded: willCacheIfNeeded,
                    sortingOrder: sortingOrder,
                    transformParent: transformParent);

            var singleImageActionBehaviour = gameObject.AddComponent<SingleImageActionBehaviour>();
            singleImageActionBehaviour.SetupComponent(initializer: initializer);
            singleImageActionBehaviour.SetBaseImageSizeUsingSpriteTextureSize();

            return singleImageActionBehaviour;
        }


        /// <summary>
        /// スプライトを有するSingleActionBehaviour を作成し、指定のエリアに移動・フィットさせる
        /// </summary>
        /// <param name="name"></param>
        /// <param name="texturePathUnderResources"></param>
        /// <param name="willCacheIfNeeded"></param>
        /// <param name="sortingOrder"></param>
        /// <param name="transformParent"></param>
        /// <param name="initializer"></param>
        /// <param name="area"></param>
        /// <param name="scaleMode"></param>
        /// <returns></returns>        
        public static SingleImageActionBehaviour CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
            string name,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder,
            Transform transformParent,
            in FSingleImageActionBehaviourInitializer initializer,
            FArea area,
            EScaleMode scaleMode)
        {
            var resultSingleImageActionBehaviour = CreateGameObjectWithSpriteAndSingleImageActionBehaviour(name: name, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: willCacheIfNeeded, sortingOrder: sortingOrder, transformParent: transformParent, initializer: initializer);

            FitGameObjectWithSingleImageActionBehaviourToArea(gameObject: resultSingleImageActionBehaviour.gameObject, area: area, scaleMode: scaleMode);

            return resultSingleImageActionBehaviour;
        }


        public static GameObject CreateGameObjectWithResizableSprite(
            string name,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder,
            Transform transformParent,
            Vector4 border) // 伸縮しない枠部分 new Vector4(32f, 32f, 32f, 32f) など
        {
            GameObject resultGameObject = new GameObject(name);

            AddSpriteToGameObjectWithResizableSprite(
                gameObject: resultGameObject,
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: willCacheIfNeeded,
                sortingOrder: sortingOrder,
                border: border);

            // transform
            resultGameObject.transform.parent = transformParent;
            resultGameObject.transform.localScale = new Vector3(1f, 1f, 1f);

            return resultGameObject;
        }


        public static SingleImageActionBehaviour CreateGameObjectWithResizableSpriteAndSingleImageBehaviour(
            string name,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder,
            Transform transformParent,
            Vector4 border, // 伸縮しない枠部分 new Vector4(32f, 32f, 32f, 32f) など
            in FSingleImageActionBehaviourInitializer initializer)
        {
            var gameObject = CreateGameObjectWithResizableSprite(
                    name: name,
                    texturePathUnderResources: texturePathUnderResources,
                    willCacheIfNeeded: willCacheIfNeeded,
                    sortingOrder: sortingOrder,
                    transformParent: transformParent,
                    border: border);

            var singleImageActionBehaviour = gameObject.AddComponent<SingleImageActionBehaviour>();
            singleImageActionBehaviour.SetupComponent(initializer: initializer);

            return singleImageActionBehaviour;
        }


        // 使用するとエラーがでる。
        // resultSingleImageActionBehaviour において、BaseImageSize が設定されていない（=初期値 0,0 ）のまま、FitGameObjectWithSingleImageActionBehaviourToArea が呼ばれているため、scale がもとめられないため
        // 特にしようされないようにここに残しておく。
        // 同じように使用したい場合には、パラメータ数の少ない CreateGameObjectWithResizableSpriteAndSingleImageBehaviour を読んでる箇所を参考にすること。
        public static SingleImageActionBehaviour CreateGameObjectWithResizableSpriteAndSingleImageBehaviour(
            string name,
            string texturePathUnderResources,
            bool willCacheIfNeeded,
            int sortingOrder,
            Transform transformParent,
            Vector4 border, // 伸縮しない枠部分 new Vector4(32f, 32f, 32f, 32f) など
            in FSingleImageActionBehaviourInitializer initializer,
            FArea area,
            EScaleMode scaleMode)
        {
            Debug.Log("この関数は使用できません");


            var resultSingleImageActionBehaviour = CreateGameObjectWithResizableSpriteAndSingleImageBehaviour(name: name, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: willCacheIfNeeded, sortingOrder: sortingOrder, transformParent: transformParent, border: border, initializer: initializer);

            FitGameObjectWithSingleImageActionBehaviourToArea(gameObject: resultSingleImageActionBehaviour.gameObject, area: area, scaleMode: scaleMode);

            return resultSingleImageActionBehaviour;
        }
        #endregion


        #region Fit GameObject to Area 

        // 与えられたエリア(グローバル値)にフィットするように表示物を拡大縮小する
        // - 親となる GameObject が拡大縮小されていても、グローバル座標で表示を調整する
        public static void FitGameObjectWithSingleImageActionBehaviourToArea(GameObject gameObject, FArea area, EScaleMode scaleMode)
        {
            // gameObject の縦横を取得する
            var singeImageActionBehaviour = gameObject.GetComponent<SingleImageActionBehaviour>();
            Assert.IsTrue(singeImageActionBehaviour != null);

            var imageSize = singeImageActionBehaviour.BaseImageSize;

            // 親となる GameObject の拡大縮小率を反映させる
            var presentGlobalScale = gameObject.transform.lossyScale;

            // 中央に移動する
            // - ここはUnitで移動する必要がある
            gameObject.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: area.CenterPoint) / presentGlobalScale;

            // 拡大縮小する

            Vector2 scale = new Vector2(0f, 0f);

            switch (scaleMode)
            {
                case EScaleMode.FIT:
                    var tempScaleX = area.size.x / imageSize.x;
                    var tempScaleY = area.size.y / imageSize.y;
                    if (tempScaleX < tempScaleY)
                    {
                        scale.x = tempScaleX;
                        scale.y = tempScaleX;
                    }
                    else
                    {
                        scale.x = tempScaleY;
                        scale.y = tempScaleY;
                    }
                    break;

                case EScaleMode.FIT_AND_FILL:
                    var tempScaleX2 = area.size.x / imageSize.x;
                    var tempScaleY2 = area.size.y / imageSize.y;
                    if (tempScaleX2 > tempScaleY2)
                    {
                        scale.x = tempScaleX2;
                        scale.y = tempScaleX2;
                    }
                    else
                    {
                        scale.x = tempScaleY2;
                        scale.y = tempScaleY2;
                    }
                    break;

                case EScaleMode.STRETCH:
                    scale.x = area.size.x / imageSize.x;
                    scale.y = area.size.y / imageSize.y;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            gameObject.transform.localScale = scale / presentGlobalScale;
        }


        #endregion


        #region Material

        private static Material CreateSpriteMaterial(ESpriteBlendMode spriteBlendMode)
        {
            string shaderName = spriteBlendMode switch
            {
                ESpriteBlendMode.NORMAL => "Custom/Sprites-Normal",
                ESpriteBlendMode.ADD => "Custom/Sprites-Add",
                _ => throw new InvalidOperationException()
            };

            // material
            return new Material(Shader.Find(shaderName));
        }

        #endregion


        #region Blend Mode

        public static void ChangeSpriteBlendMode(GameObject gameObject, ESpriteBlendMode spriteBlendMode)
        {
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            // メモリリークを防ぐため、自前で作成したマテリアルを削除する
            UnityEngine.Object.Destroy(spriteRenderer.sharedMaterial);

            spriteRenderer.material = CreateSpriteMaterial(spriteBlendMode: spriteBlendMode);
        }

        #endregion


        #region Alpha

        public static float GetSpriteAlpha(GameObject gameObject)
        {
            return gameObject.GetComponent<SpriteRenderer>().GetMaterialAlpha();
        }


        public static void SetSpriteAlpha(GameObject gameObject, float alpha)
        {
            gameObject.GetComponent<SpriteRenderer>().SetMaterialAlpha(alpha: alpha);
        }


        public static void SetSpriteAlphaByList(GameObject[] gameObjectWithSpriteList, float alpha)
        {
            // 配列は LINQ の ForEach を利用できない
            foreach (var i in gameObjectWithSpriteList)
            {
                SetSpriteAlpha(gameObject: i, alpha: alpha);
            }
        }

        #endregion


        #region Color

        public static void ChangeColor(GameObject gameObject, Vector3 color)
        {
            var renderer = gameObject.GetComponent<SpriteRenderer>();

            renderer.SetMaterialColor(color: color);
        }

        #endregion


        #region Visibility
        // 現在は特に使用していない。α値をゼロにすることで非表示にしているが、パフォーマンスに影響があるならば使用する。
        public static bool GetSpriteVisibility(GameObject gameObject)
        {
            // renderer
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            return spriteRenderer.enabled;
        }

        public static void SetSpriteVisibility(GameObject gameObject, bool value)
        {
            // renderer
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = value;
        }

        #endregion



        #region Sorting Order
        public static void SetSortingOrder(GameObject gameObject, int sortingOrder)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }
        #endregion


        #region Texture

        // sprite が immutable であるため、実際には sprite を生成して差し替えている
        public static void ChangeTexture(GameObject gameObject, string texturePathUnderResources, bool willCacheIfNeeded)
        {
            // texture
            Texture2D texture = Locator.Instance.textureManager.LoadTexture(filePathUnderResources: texturePathUnderResources, willCacheIfNeeded: willCacheIfNeeded);

            // renderer
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(
                texture: texture,
                rect: new Rect(0.0f, 0.0f, texture.width, texture.height),
                pivot: new Vector2(0.5f, 0.5f),
                pixelsPerUnit: Config.PIXEL_PER_UNIT);
        }


        // NOTE: 既存スプライトが9Sliceを利用しているかの判定がいまのところ、Border値の設定有無で擬似的に確認できるのみではないか。(SpriteMeshTypeはTextureImporterが用いる値であり、後から取得できない)
        // そのため、9sliceかどうかは判定せず、関数を分けている。
        public static void ChangeTextureForResizable(
            GameObject gameObject,
            string texturePathUnderResources,
            bool willCacheIfNeeded)
        {
            // texture
            Texture2D texture = Locator.Instance.textureManager.LoadTexture(filePathUnderResources: texturePathUnderResources, willCacheIfNeeded: willCacheIfNeeded);

            // renderer
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            var reservedBorder = spriteRenderer.sprite.border;

            spriteRenderer.sprite = Sprite.Create(
                texture: texture,
                rect: new Rect(0.0f, 0.0f, texture.width, texture.height),
                pivot: new Vector2(0.5f, 0.5f),
                pixelsPerUnit: Config.PIXEL_PER_UNIT,
                extrude: 0,
                meshType: SpriteMeshType.FullRect,
                border: reservedBorder,
                generateFallbackPhysicsShape: false);// TODO: これでいい？);                
        }


        public static Vector2 GetTextureSize(GameObject gameObject)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            Assert.IsTrue(spriteRenderer != null);

            return new Vector2(spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height);
        }

        #endregion
    }
}
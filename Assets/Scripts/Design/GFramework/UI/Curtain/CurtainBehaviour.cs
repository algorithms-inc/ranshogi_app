using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


/// <summary>
/// TODO: アニメーションでもって表示できる
/// TODO: アニメーション途中でも切り替えられるか確認する
/// </summary>
/// 
namespace GFramework
{
    public class CurtainBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        public enum ECurtainType
        {
            WINDOW_TOP,
            WINDOW_NORMAL,
            WINDOW_CONFIRMATION,
            WINDOW_ERROR,
            GAME_RESULT_WIN,
            GAME_RESULT_LOSE,
            GAME_RESULT_DRAW,
        }


        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;


        private bool _isShown = true;
        public bool IsShown => _isShown;

        public float Alpha => UtlSprite.GetSpriteAlpha(gameObject: _bodySingleImageActionBehaviour.gameObject);



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(ECurtainType curtainType)
        {
            // 必要？
            transform.position = new Vector3(0f, 0f, 0f);

            SetupGameObject(curtainType: curtainType);

            Hide();
        }


        private void SetupGameObject(ECurtainType curtainType)
        {
            SetupBody(curtainType: curtainType);

            SetLocalScale();
        }


        private void SetupBody(ECurtainType curtainType)
        {
            string texturePathUnderResources;
            int sortingOrder;

            switch (curtainType)
            {
                case ECurtainType.WINDOW_TOP:
                    texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.WHITE);
                    sortingOrder = Config.CURTAIN_TOP__LAYERED_ORDERID.SortingOrder;
                    break;

                case ECurtainType.WINDOW_NORMAL:
                    texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.WHITE);
                    sortingOrder = Config.CURTAIN_NORMAL__LAYERED_ORDERID.SortingOrder;
                    break;

                case ECurtainType.WINDOW_CONFIRMATION:
                    texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.WHITE);
                    sortingOrder = Config.CURTAIN_CONFIRMATION__LAYERED_ORDERID.SortingOrder;
                    break;

                case ECurtainType.WINDOW_ERROR:
                    texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.BLACK);
                    sortingOrder = Config.CURTAIN_ERROR__LAYERED_ORDERID.SortingOrder;
                    break;

                case ECurtainType.GAME_RESULT_WIN:
                    texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.BLUE);
                    sortingOrder = Config.CURTAIN_GAME_RESULT__LAYERED_ORDERID.SortingOrder;
                    break;

                case ECurtainType.GAME_RESULT_LOSE:
                    texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.RED);
                    sortingOrder = Config.CURTAIN_GAME_RESULT__LAYERED_ORDERID.SortingOrder;
                    break;

                    // TODO: ひとまず、勝利と同じ
                case ECurtainType.GAME_RESULT_DRAW:
                    texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.BLUE);
                    sortingOrder = Config.CURTAIN_GAME_RESULT__LAYERED_ORDERID.SortingOrder;
                    break;

                default:
                    throw new InvalidOperationException();
            }



            var gameObjectCurtainBody = UtlSprite.CreateGameObjectWithSprite(
                name: "CurtainBody",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform);

            _bodySingleImageActionBehaviour = gameObjectCurtainBody.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: Config.CURTAIN__ALPHA_DEFAULT,
                durationFadeIn: 0.5f,
                durationFadeOut: 0.5f);
            _bodySingleImageActionBehaviour.SetupComponent(initializer: initializer);
        }


        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            var bodyImageSize = Config.CURTAIN_BODY__IMAGE_SIZE;

            var bodyDisplaySize = Locator.Instance.layoutManager.AreaBackground.size;

            var value = bodyDisplaySize / bodyImageSize;

            transform.localScale = value;
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public void SetAlpha(float alpha)
        {
            UtlSprite.SetSpriteAlpha(gameObject: _bodySingleImageActionBehaviour.gameObject, alpha: alpha);
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void Show()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.Show();

            _isShown = true;
        }


        public void Hide()
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public void FadeIn()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.FadeIn();

            _isShown = true;
        }


        public void FadeOut()
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _bodySingleImageActionBehaviour = null;
        }
    }
}
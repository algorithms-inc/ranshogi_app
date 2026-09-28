using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class TitleLogoBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => UtlSprite.GetSpriteAlpha(gameObject: _singleImageActionBehaviourLogoBody.gameObject);

        private MenuStageLayouter _layouter;


        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _singleImageActionBehaviourLogoBody;
        public SingleImageActionBehaviour SingleImageActionBehaviourLogoBody => _singleImageActionBehaviourLogoBody;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(MenuStageLayouter layouter)
        {
            _layouter = layouter;

            transform.position = new Vector3(0f, 0f, 0f);

            SetupGameObject();
        }


        private void SetupGameObject()
        {
            SetupBody();
            SetupBaseImageSize();


            SetupPosition();

            Hide();
        }


        private void SetupBody()
        {
            _singleImageActionBehaviourLogoBody = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "TitleLogoBody",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForTitleLogo.BODY),
                willCacheIfNeeded: true,
                sortingOrder: Config.MENU_STAGE_TITLE_LOGO_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 1f,
                    durationFadeOut: 1f),
                area: _layouter.AreaTitleLogo,
                scaleMode: EScaleMode.FIT
                );
        }


        private void SetupBaseImageSize()
        {
            _baseImageSize = UtlSprite.GetTextureSize(gameObject: _singleImageActionBehaviourLogoBody.gameObject);
        }


        // ===============================================
        // MARK: - ● Get Local Position ─────

        public Vector2 GetLocalPosition()
        {
            return transform.localPosition;
        }


        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositon(float unitY)
        {
            var unit = new Vector2(transform.localPosition.x, unitY);
            Utl.SetLocalPosition(gameObject: gameObject, unit: unit);
        }

        public void SetLocalPositonByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: gameObject, baseImageSize: _baseImageSize, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Setup Position ─────

        private void SetupPosition()
        {
            Vector2 centerPoint = Locator.Instance.layoutManager.AreaBackground.CenterPoint;

            Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: centerPoint);

            transform.position = position;
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _singleImageActionBehaviourLogoBody.SetAlpha(alpha: alpha);
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void ShowWithAnimation(bool isFull)
        {
            if (_isShown) { return; }

            RegisterAction(action: CAction.CreateTitleLogoActionShowFull(titleLogoBehaviour: this, duration: 1.0f));

            _isShown = true;
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; }

            SetAlpha(alpha: 1f);

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            SetAlpha(alpha: 0f);

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }


            var action = CAction.CreateImageActionFade(targetBehaviour: this, fadeType: CAction.EFadeType.FADE_IN, duration: Config.ANIMATION_DURATION_3);
            RegisterAction(action: action);
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }


            var action = CAction.CreateImageActionFade(targetBehaviour: this, fadeType: CAction.EFadeType.FADE_OUT, duration: Config.ANIMATION_DURATION_3);
            RegisterAction(action: action);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _layouter = null; // 削除はMenuStageBehaviour で行う
            _singleImageActionBehaviourLogoBody = null;
        }
    }
}
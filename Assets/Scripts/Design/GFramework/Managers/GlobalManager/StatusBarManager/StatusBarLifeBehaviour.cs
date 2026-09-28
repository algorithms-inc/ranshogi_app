using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class StatusBarLifeBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private SingleImageActionBehaviour _singleImageActionBehaviourHole;
        private SingleImageActionBehaviour _singleImageActionBehaviourLifeBody;

        private bool _isLifeBodyOn = false;
        public bool IsLifeBodyOn => _isLifeBodyOn;

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _singleImageActionBehaviourHole.Alpha;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(FArea area)
        {
            SetupGameObject(area: area);
        }


        private void SetupGameObject(FArea area)
        {
            SetupBody(area: area);

            SetupPosition();

            Hide();
        }


        private void SetupBody(FArea area)
        {
            _singleImageActionBehaviourHole = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "Hole",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForStatusBar.HOLE),
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_HOLE__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f),
                area: area,
                scaleMode: EScaleMode.FIT
                );


            _singleImageActionBehaviourLifeBody = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "LifeBody",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForStatusBar.LIFE_BODY),
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_LIFE_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f),
                area: area,
                scaleMode: EScaleMode.FIT
                );
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
            _singleImageActionBehaviourHole.SetAlpha(alpha: alpha);

            if (_isLifeBodyOn)
            {
                _singleImageActionBehaviourLifeBody.SetAlpha(alpha: alpha);
            }
        }



        // ===============================================
        // MARK: - ● Set isLifeBodyOn ─────

        public void SetIsLifeBodyOn(bool value)
        {
            if (_isLifeBodyOn == value) { return; }

            _isLifeBodyOn = value;

            if (_isShown == false) { return; }

            if (_isLifeBodyOn)
            {
                _singleImageActionBehaviourLifeBody.FadeIn();
            }
            else
            {
                _singleImageActionBehaviourLifeBody.FadeOut();
            }
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; }

            _singleImageActionBehaviourHole.Show();

            if (_isLifeBodyOn)
            {
                _singleImageActionBehaviourLifeBody.Show();
            }

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _singleImageActionBehaviourHole.Hide();

            if (_isLifeBodyOn)
            {
                _singleImageActionBehaviourLifeBody.Hide();
            }

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● Animation ─────

        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }


            _singleImageActionBehaviourHole.FadeIn();

            if (_isLifeBodyOn)
            {
                _singleImageActionBehaviourLifeBody.FadeIn();
            }


            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }


            _singleImageActionBehaviourHole.FadeOut();

            if (_isLifeBodyOn)
            {
                _singleImageActionBehaviourLifeBody.FadeOut();
            }


            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _singleImageActionBehaviourHole = null;
            _singleImageActionBehaviourLifeBody = null;
        }
    }
}
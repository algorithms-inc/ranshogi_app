using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public enum ERippleType
    {
        A,
        B,
        C,
        D
    }


    public class RippleBehaviour : SingleImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private ERippleType _rippleType;

        public override float Alpha => UtlSprite.GetSpriteAlpha(gameObject: gameObject);



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(ERippleType rippleType, in FSingleImageActionBehaviourInitializer initializer)
        {
            _rippleType = rippleType;

            SetupBody();
            SetLocalScale();
            SetupPosition();

            SetDefaultAlpha(alpha: Config.BACKGROUND_RIPPLE__ALPHA_DEFAULT);


            base.SetupComponent(initializer: initializer);

            Hide();
        }



        private void SetupBody()
        {
            string texturePathUnderResources = _rippleType switch
            {
                ERippleType.A => Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForBackground.RIPPLE_A),
                ERippleType.B => Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForBackground.RIPPLE_B),
                ERippleType.C => Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForBackground.RIPPLE_C),
                ERippleType.D => Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForBackground.RIPPLE_D),
                _ => throw new InvalidOperationException(),
            };


            int sortingOrder = _rippleType switch
            {
                ERippleType.A => Config.BACKGROUND_RIPPLE_A__LAYERED_ORDERID.SortingOrder,
                ERippleType.B => Config.BACKGROUND_RIPPLE_B__LAYERED_ORDERID.SortingOrder,
                ERippleType.C => Config.BACKGROUND_RIPPLE_C__LAYERED_ORDERID.SortingOrder,
                ERippleType.D => Config.BACKGROUND_RIPPLE_D__LAYERED_ORDERID.SortingOrder,
                _ => throw new InvalidOperationException(),
            };


            UtlSprite.AddSpriteToGameObject(
                gameObject: gameObject,
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder);


            _baseImageSize = UtlSprite.GetTextureSize(gameObject: gameObject);
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
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            float bodyImageSizeY = Config.BACKGOURND_BODY__IMAGE_SIZE.y;

            float bodyDisplaySizeY = Locator.Instance.layoutManager.AreaBackground.size.y;

            float value = bodyDisplaySizeY / bodyImageSizeY;

            transform.localScale = new Vector2(value, value);
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
            UtlSprite.SetSpriteAlpha(gameObject: gameObject, alpha: alpha);
        }



        // ===============================================
        // MARK: - ● Change Player ─────

        public void ChangePlayer(EPlayerType toPlayerType)
        {
            throw new NotImplementedException();
        }


        // ===============================================
        // MARK: - ● Animation ─────

        public void DoNormalAnimation(
            CSwingCounter swingCounterForAlpha,
            COnewayCounter oneWayCounterHorizontalMove,
            CSwingCounter swingCounterScale)
        {
            var action = new CRippleActionNormal(
                rippleBehaviour: this,
                plusMinusDeltaAlpha: 0.050f,
                plusMinusDeltaVerticalMove: 100f,
                plusMinusScale: 0.5f,

                swingCounterForAlpha: swingCounterForAlpha,
                oneWayCounterForVerticalMove: oneWayCounterHorizontalMove,
                swingCounterForScale: swingCounterScale,

                duration: 100f);

            RegisterAction(action: action);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
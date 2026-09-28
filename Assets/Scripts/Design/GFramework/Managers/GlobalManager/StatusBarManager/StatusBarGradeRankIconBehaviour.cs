using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public class StatusBarGradeRankIconBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private EPlayerType _playerType = EPlayerType.PLAYER1;
        private EMoveOrderType _moveOrderType = EMoveOrderType.FIRST;
        public EMoveOrderType MoveOrderType => _moveOrderType;
        private EGradeRankType _gradeRankType = EGradeRankType.NONE;
        public EGradeRankType GradeRankType => _gradeRankType;
        private EBoardType _boardType;

        private bool _isShown = true;
        public bool IsShown => _isShown;

        private bool _isOn = true;
        public bool IsOn => _isOn;

        public override float Alpha => _singleImageActionBehaviourMoveOrderOn.Alpha;


        private SingleImageActionBehaviour _singleImageActionBehaviourMoveOrderDefault;
        private SingleImageActionBehaviour _singleImageActionBehaviourMoveOrderOn;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(EPlayerType playerType, EBoardType boardType, bool isOn)
        {
            _playerType = playerType;
            _boardType = boardType;

            _isOn = isOn;

            SetupGameObject();
            Hide();
        }


        private void SetupGameObject()
        {
            SetupBody();

            SetupBaseImageSize();


            SetLocalScale();

            SetupPosition();


            SetGradeRankIcon(newMoveOrderType: EMoveOrderType.SECOND, newGradeRankType: EGradeRankType.NONE, boardType: _boardType);
        }


        private void SetupBody()
        {
            var filePathOfGradeRankIconDefault = Locator.Instance.textureManager.GetFilePathOfGradeRankIcon(gradeRankType: _gradeRankType, boardType: _boardType, isDefault: true);

            _singleImageActionBehaviourMoveOrderDefault = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "MoveOrderDefault",
                texturePathUnderResources: filePathOfGradeRankIconDefault,
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_MOVE_ORDER_DEFAULT__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f),
                area: Locator.Instance.statusBarManager.Layouter.GetAreaMoveOrder(playerType: _playerType).AsYReversed(),
                scaleMode: EScaleMode.FIT
                );


            var filePathOfGradeRankIconOn = Locator.Instance.textureManager.GetFilePathOfGradeRankIcon(gradeRankType: _gradeRankType, boardType: _boardType, isDefault: false);

            _singleImageActionBehaviourMoveOrderOn = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "MoveOrderOn",
                texturePathUnderResources: filePathOfGradeRankIconOn,
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_MOVE_ORDER_ON__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f),
                area: Locator.Instance.statusBarManager.Layouter.GetAreaMoveOrder(playerType: _playerType).AsYReversed(),
                scaleMode: EScaleMode.FIT
                );


            Show();
            SetOn();
        }

        private void SetupBaseImageSize()
        {
            _baseImageSize = UtlSprite.GetTextureSize(gameObject: _singleImageActionBehaviourMoveOrderDefault.gameObject);
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
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            // do nothing
        }



        // ===============================================
        // MARK: - ● Setup Position ─────

        private void SetupPosition()
        {
            //Vector2 centerPoint = Locator.Instance.statusBarManager.Layouter.GetAreaMoveOrder(playerType: _playerType).CenterPoint;

            //Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: centerPoint);

            //transform.position = position;
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            if (_isOn)
            {
                _singleImageActionBehaviourMoveOrderOn.SetAlpha(alpha: alpha);
            }

            _singleImageActionBehaviourMoveOrderDefault.SetAlpha(alpha: alpha);
        }



        // ===============================================
        // MARK: - ●  ─────

        public void SetGradeRankIcon(EMoveOrderType newMoveOrderType, EGradeRankType newGradeRankType, EBoardType boardType)
        {
            if ((_moveOrderType == newMoveOrderType) && (_gradeRankType == newGradeRankType) && (_boardType == boardType)) { return; }

            _moveOrderType = newMoveOrderType;
            _gradeRankType = newGradeRankType;
            _boardType = boardType;

            var filePathOfGradeRankIconDefault = Locator.Instance.textureManager.GetFilePathOfGradeRankIcon(gradeRankType: _gradeRankType, boardType: _boardType, isDefault: true);

            UtlSprite.ChangeTexture(
                gameObject: _singleImageActionBehaviourMoveOrderDefault.gameObject,
                texturePathUnderResources: filePathOfGradeRankIconDefault,
                willCacheIfNeeded: true);

            var filePathOfGradeRankIconOn = Locator.Instance.textureManager.GetFilePathOfGradeRankIcon(gradeRankType: _gradeRankType, boardType: _boardType, isDefault: false);

            UtlSprite.ChangeTexture(
                gameObject: _singleImageActionBehaviourMoveOrderOn.gameObject,
                texturePathUnderResources: filePathOfGradeRankIconOn,
                willCacheIfNeeded: true);
        }


        // ===============================================
        // MARK: - ●  ─────

        public void SetOn()
        {
            if (_isOn) { return; }

            if (_isShown)
            {
                _singleImageActionBehaviourMoveOrderOn.FadeIn();
            }

            _isOn = true;
        }

        public void SetOff()
        {
            if (_isOn == false) { return; }

            _singleImageActionBehaviourMoveOrderOn.FadeOut();

            _isOn = false;
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; }

            if (_isOn)
            {
                _singleImageActionBehaviourMoveOrderOn.Show();
            }

            _singleImageActionBehaviourMoveOrderDefault.Show();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            if (_isOn)
            {
                _singleImageActionBehaviourMoveOrderOn.Hide();
            }

            _singleImageActionBehaviourMoveOrderDefault.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if(_isShown) { return; }

            if (_isOn)
            {
                _singleImageActionBehaviourMoveOrderOn.FadeIn();
            }

            _singleImageActionBehaviourMoveOrderDefault.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            if (_isOn)
            {
                _singleImageActionBehaviourMoveOrderOn.FadeOut();
            }

            _singleImageActionBehaviourMoveOrderDefault.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _singleImageActionBehaviourMoveOrderDefault = null;
            _singleImageActionBehaviourMoveOrderOn = null;
        }
    }
}
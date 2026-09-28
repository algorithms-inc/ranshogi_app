using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;



// 場所
// - board / Waiting Space(駒を置く場合などの表示…ひとまずしない？？？)

// 表示状態
// - 非表示
// - 移動可能場所
// - 敵の移動（ひとまずしない？？？）


// 駒はアニメーションするので、セルで一括してタップ判定を受け取るようにする。

namespace GFramework
{
    public class CellBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Enum ─────

        public enum ECellType
        {
            DEFAULT,
            PLACE_TARGET,
            LAST_MOVED
        }


        // ===============================================
        // MARK: - ● Property ─────

        private BoardSetBehaviour _boardSetBehaviour = null;

        private ECellType _cellType = ECellType.DEFAULT;
        public ECellType CellType => _cellType;

        private bool _isShown = true;
        public bool IsShown => _isShown;

        private FBoardCrd _boardCrd;


        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;


        public override float Alpha => _bodySingleImageActionBehaviour.Alpha;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(BoardSetBehaviour boardSetBehaviour, FBoardCrd boardCrd)
        {
            _boardSetBehaviour = boardSetBehaviour;
            _boardCrd = boardCrd;
            transform.position = new Vector3(0f, 0f, 0f);

            SetupGameObject();

            Hide();
        }


        private void SetupGameObject()
        {
            SetupBody();
            SetupBaseImageSize();

            SetLocalScale();

            SetLocalScaleOfSingeImageObject(scale: new Vector2(1, 1));
        }


        private void SetupBaseImageSize()
        {
            _baseImageSize = Config.CELL__IMAGE_SIZE;
        }


        private void SetupBody()
        {
            // 便宜上 選択時画像をロードしておく
            _bodySingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "CellBody",
                texturePathUnderResources: _boardSetBehaviour.BoardImageSetManager.GetFilePathOfImageCell(cellType: ECellType.PLACE_TARGET),
                willCacheIfNeeded: true,
                sortingOrder: Config.CELL__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 1,
                    durationFadeOut: 1)
                );


            Vector2 positionByPixel = _boardSetBehaviour.GetAreaBoardCell(boardCrd: _boardCrd).CenterPoint;
            transform.position = Utl.ConvertImageSizeToUnit(imageSize: positionByPixel);
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositon(Vector2 unit)
        {
            Utl.SetLocalPosition(gameObject: _bodySingleImageActionBehaviour.gameObject, unit: unit);
        }


        public void SetLocalPositonByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: _bodySingleImageActionBehaviour.gameObject, baseImageSize: _baseImageSize, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            transform.localScale = _boardSetBehaviour.GetCellImageScaleFromOriginalToOnBoard();
        }


        public void SetLocalScaleOfSingeImageObject(Vector2 scale)
        {
            _bodySingleImageActionBehaviour.transform.localScale = scale;
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _bodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }


        // ===============================================
        // MARK: - ●  ─────

        public void SetCellType(ECellType cellType)
        {
            if (_cellType == cellType) { return; }

            _cellType = cellType;

            if (cellType != ECellType.DEFAULT)
            {
                ChangeTexture(cellType: cellType);
                
            }
        }

        private void ChangeTexture(ECellType cellType)
        {
            string texturePathUnderResources = _boardSetBehaviour.BoardImageSetManager.GetFilePathOfImageCell(cellType: cellType);
            UtlSprite.ChangeTexture(gameObject: _bodySingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
        }



        // ===============================================
        // MARK: - ● Show and Hide ─────

        public override void Show()
        {
            if (_isShown) { return; };

            _bodySingleImageActionBehaviour.Show();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; };

            _bodySingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; };

            _bodySingleImageActionBehaviour.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; };

            _bodySingleImageActionBehaviour.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ●  ─────

        public void ExecuteActionChangeToSelectTarget()
        {
            var newAction = CAction.CreateCellActionChangeDisplayToPlaceTarget(cellBehaviour: this);

            RegisterAction(action: newAction);
        }

        public void ExecuteActionChangeToLastMoved()
        {
            var newAction = CAction.CreateCellActionChangeDisplayToLastMoved(cellBehaviour: this);

            RegisterAction(action: newAction);
        }

        public void ExecuteActionChengeToDefault()
        {
            var newAction = CAction.CreateCellActionChangeDisplayToDefault(cellBehaviour: this);

            RegisterAction(action: newAction);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _boardSetBehaviour = null;
            _bodySingleImageActionBehaviour = null;
        }
    }
}
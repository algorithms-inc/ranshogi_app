using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public class BoardBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private EBoardType _boardType;

        private BoardSetBehaviour _boardSetBehaviour = null;

        private CellBehaviour[] _cellBehaviourList;

        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;
        private SingleImageActionBehaviour _shadowSingleImageActionBehaviour;


        private bool _isShown = false;
        public bool IsShown => _isShown;

        public override float Alpha => _bodySingleImageActionBehaviour.Alpha;


        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(EBoardType boardType, BoardSetBehaviour boardSetBahavirour)
        {
            _boardType = boardType;
            _boardSetBehaviour = boardSetBahavirour;

            switch (_boardType)
            {
                case EBoardType.BOARD_66:
                    _cellBehaviourList = new CellBehaviour[36];
                    break;
                case EBoardType.BOARD_99:
                    _cellBehaviourList = new CellBehaviour[81];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_boardType), _boardType, null);
            }

            transform.position = new Vector3(0f, 0f, 0f);

            SetupGameObject();

            Hide();
        }


        private void SetupGameObject()
        {
            SetupBody();
            SetupShadow();
            SetLocalScale();
            SetupPosition();
            SetupCell();
        }


        private Vector2 GetBaseImageSize()
        {
            return UtlSprite.GetTextureSize(gameObject: _bodySingleImageActionBehaviour.gameObject);
        }


        private string GetFilePathOfBoard()
        {
            string result = "";

            if (Locator.Instance.designManager.IsShowNumOnBoard() == false)
            {
                result = Locator.Instance.textureManager.GetFilePath(boardType: _boardType, textureType: ETextureTypeForBoard.BODY);
            }
            else
            {
                if (_boardSetBehaviour.MoveOrderTypeOfPlayer1 == EMoveOrderType.FIRST)
                {
                    result = Locator.Instance.textureManager.GetFilePath(boardType: _boardType, textureType: ETextureTypeForBoard.BODY_NUM_BLACK);
                }
                else
                {
                    result = Locator.Instance.textureManager.GetFilePath(boardType: _boardType, textureType: ETextureTypeForBoard.BODY_NUM_WHITE);
                }
            }

            return result;
        }

        private void SetupBody()
        {
            string texturePathUnderResources = GetFilePathOfBoard();


            var gameObjectBody = UtlSprite.CreateGameObjectWithSprite(
                name: "BoardBody",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: Config.BOARD_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform);

            _bodySingleImageActionBehaviour = gameObjectBody.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);
            _bodySingleImageActionBehaviour.SetupComponent(initializer: initializer);
        }


        private void SetupShadow()
        {
            string texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(boardType: _boardType, textureType: ETextureTypeForBoard.SHADOW);

            var gameObjectShadow = UtlSprite.CreateGameObjectWithSprite(
                name: "BoardShadow",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: Config.BOARD_SHADOW__LAYERED_ORDERID.SortingOrder,
                transformParent: transform);

            _shadowSingleImageActionBehaviour = gameObjectShadow.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: Config.BOARD_SHADOW__ALPHA__DEFAULT,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);
            _shadowSingleImageActionBehaviour.SetupComponent(initializer: initializer);


            Utl.SetLocalPositionByRatio(gameObject: gameObjectShadow, baseImageSize: GetBaseImageSize(), ratio: Config.BOARD_SHADOW__LOCAL_POSITION_RATIO__DEFAULT);
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositonOfWholeBoard(Vector2 unit)
        {
            Utl.SetLocalPosition(gameObject: transform.parent.gameObject, unit: unit);
        }

        public void SetLocalPositonOfWholeBoardByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: transform.parent.gameObject, baseImageSize: GetBaseImageSize(), ratio: ratio);
        }


        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            float bodyImageSizeX = Config.BOARD_BODY__IMAGE_SIZE.x;
            float bodyDisplaySizeX = _boardSetBehaviour.GetAreaBoard().size.x;

            float value = bodyDisplaySizeX / bodyImageSizeX;

            transform.localScale = new Vector2(value, value);
        }



        // ===============================================
        // MARK: - ● Setup Position ─────

        private void SetupPosition()
        {
            Vector2 centerPoint = _boardSetBehaviour.GetAreaBoard().CenterPoint;

            Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: centerPoint);

            transform.position = position;
        }




        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            _bodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
            _shadowSingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }


        public override void Show()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.Hide();
            _shadowSingleImageActionBehaviour.Hide();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.Hide();
            _shadowSingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.FadeIn();
            _shadowSingleImageActionBehaviour.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            ClearAllCellDisplayToDefault();
            _bodySingleImageActionBehaviour.FadeOut();
            _shadowSingleImageActionBehaviour.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● Cell ─────

        private void SetupCell()
        {
            int index = 0;

            var maxLineIndex = _boardType.GetMaxLineIndex();

            for (int y = 0; y <= maxLineIndex; y++)
            {
                for (int x = 0; x <= maxLineIndex; x++)
                {
                    FBoardCrd boardCrd = new FBoardCrd(x: x + 1, y: y + 1);
                    GameObject newCell = new GameObject("Cell" + boardCrd.ToString());
                    newCell.transform.parent = transform;


                    CellBehaviour newCellBehaviour = newCell.AddComponent<CellBehaviour>();
                    newCellBehaviour.SetupComponent(boardSetBehaviour: _boardSetBehaviour, boardCrd: boardCrd);

                    _cellBehaviourList[index] = newCellBehaviour;

                    index++;
                }
            }
        }



        // ===============================================
        // MARK: - ● Change Cell Display ─────

        public void ChangeCellDisplayToPlaceTarget(List<FBoardCrd> boardCrdList)
        {
            boardCrdList.ForEach(i => _cellBehaviourList[i.AsIndex(boardType: _boardType)].ExecuteActionChangeToSelectTarget());
        }


        public void ChangeCellDisplayToLastMoved(FBoardCrd boardCrd)
        {
            _cellBehaviourList[boardCrd.AsIndex(boardType: _boardType)].ExecuteActionChangeToLastMoved();
        }


        public void ChangePlaceTargetCellDisplayToDefault()
        {
            List<CellBehaviour> placeTargetCellBehaviourList = GetPlaceTargetCellBehaviour();

            placeTargetCellBehaviourList.ForEach(i => i.ExecuteActionChengeToDefault());
        }


        public void ChangeCellDisplayFromLastMovedToDefaultIfNeeded(FBoardCrd boardCrd)
        {
            var isNeeded = _cellBehaviourList[boardCrd.AsIndex(boardType: _boardType)].CellType == CellBehaviour.ECellType.LAST_MOVED;

            if (isNeeded)
            {
                _cellBehaviourList[boardCrd.AsIndex(boardType: _boardType)].ExecuteActionChengeToDefault();
            }
        }



        // ===============================================
        // MARK: - ● Clear All Cell Display ─────

        // シーン遷移の際に、全てのセルの表示をデフォルトに戻す
        private void ClearAllCellDisplayToDefault()
        {
            foreach (CellBehaviour i in _cellBehaviourList)
            {
                if (i.CellType != CellBehaviour.ECellType.DEFAULT)
                {
                    i.ExecuteActionChengeToDefault();
                }
            }
        }
         


        // ===============================================
        // MARK: - ● Search ─────

        private List<CellBehaviour> GetPlaceTargetCellBehaviour()
        {
            List<CellBehaviour> resultList = new List<CellBehaviour>();

            foreach (CellBehaviour i in _cellBehaviourList)
            {
                if (i.CellType == CellBehaviour.ECellType.PLACE_TARGET)
                {
                    resultList.Add(i);
                }
            }

            return resultList;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _boardSetBehaviour = null;
            _bodySingleImageActionBehaviour = null;
            _shadowSingleImageActionBehaviour = null;
        }
    }
}
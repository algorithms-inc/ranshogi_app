using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class PieceBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property & Enum ─────

        // ボードセットのへの参照
        private BoardSetBehaviour _boardSetBehaviour = null;
        public BoardSetBehaviour BoardSetBehaviour => _boardSetBehaviour;


        // for Piece State

        private int _pieceID;
        public int PieceID => _pieceID;
        
        private FPieceState _pieceState;
        public FPieceState PieceState => _pieceState;

        private FBoardCrd _boardCrd;
        public FBoardCrd BoardCrd => _boardCrd;

        public enum ESelectType
        {
            UNSELECTED,
            SELECTED
        }

        private ESelectType _selectType;

        public ESelectType SelectType
        {
            get => _selectType;

            set
            {
                Assert.IsTrue(_selectType != value);

                _selectType = value;
            }
        }


        private bool _isShown = true;
        public bool IsShown => _isShown;

        public float AlphaOfBody => _bodySingleImageActionBehaviour.Alpha;

        public float AlphaOfShadow => _shadowSingleImageActionBehaviour.Alpha;


        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;
        public SingleImageActionBehaviour BodySingleImageActionBehaviour => _bodySingleImageActionBehaviour;

        private SingleImageActionBehaviour _shadowSingleImageActionBehaviour;
        public SingleImageActionBehaviour ShadowSingleImageActionBehaviour => _shadowSingleImageActionBehaviour;

        public override float Alpha => _bodySingleImageActionBehaviour.Alpha;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(int pieceID, BoardSetBehaviour boardSetBehaviour, FPieceState pieceState, FBoardCrd boardCrd)
        {
            _pieceID = pieceID;
            _boardSetBehaviour = boardSetBehaviour;
            _pieceState = pieceState;
            transform.position = new Vector3(0f, 0f, 0f);

            _boardCrd = boardCrd;
            _selectType = ESelectType.UNSELECTED;

            SetupGameObject();

            Hide();
        }


        private void SetupGameObject()
        {
            SetupBody();
            SetupShadow();

            SetLocalScale();

            SetLocalScaleOfBody(scale: new Vector2(1, 1));
            SetLocalScaleOfShadow(scale: new Vector2(1, 1));
        }


        public Vector2 GetBaseImageSize()
        {
            return Config.PIECE_BODY__IMAGE_SIZE;
        }


        private void SetupBody()
        {
            string texturePathUnderResources = _boardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceBody(pieceState: _pieceState);

            var gameObjectBody = UtlSprite.CreateGameObjectWithSprite(
                name: "PieceBody",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: Config.PIECE_BODY__LAYERED_ORDERID__DEFAULT.SortingOrder,
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
            string texturePathUnderResources = _boardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceShadow(pieceState: _pieceState);

            var gameObjectShadow = UtlSprite.CreateGameObjectWithSprite(
                name: "PieceShadow",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: Config.PIECE_SHADOW__LAYERED_ORDERID__DEFAULT.SortingOrder,
                transformParent: transform);

            _shadowSingleImageActionBehaviour = gameObjectShadow.AddComponent<SingleImageActionBehaviour>();

            var initializer = new FSingleImageActionBehaviourInitializer(
                defaultAlpha: Config.PIECE_SHADOW__ALPHA__DEFAULT,
                durationFadeIn: 0.3f,
                durationFadeOut: 0.3f);

            _shadowSingleImageActionBehaviour.SetupComponent(initializer: initializer);


            SetLocalPositonOfShadowByRatio(ratio: Config.PIECE_SHADOW__LOCAL_POSITION_RATIO__DEFAULT);
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositonOfBody(Vector2 unit)
        {
            Utl.SetLocalPosition(gameObject: _bodySingleImageActionBehaviour.gameObject, unit: unit);
        }

        public void SetLocalPositonOfBodyByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: _bodySingleImageActionBehaviour.gameObject, baseImageSize: GetBaseImageSize(), ratio: ratio);
        }


        public void SetLocalPositonOfShadow(Vector2 unit)
        {
            Utl.SetLocalPosition(gameObject: _shadowSingleImageActionBehaviour.gameObject, unit: unit);
        }

        public void SetLocalPositonOfShadowByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: _shadowSingleImageActionBehaviour.gameObject, baseImageSize: GetBaseImageSize(), ratio: ratio);
        }


        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            if (_boardCrd.IsOnBoard(boardType: _boardSetBehaviour.BoardType))
            {
                transform.localScale = _boardSetBehaviour.GetPieceImageScaleFromOriginalToOnBoard();
            }
            else
            {
                // case: onWaitingSpace
                transform.localScale = _boardSetBehaviour.GetPieceImageScaleFromOriginalToOnWaitingSpace();
            }
        }


        public void SetLocalScaleOfBody(Vector2 scale)
        {
            _bodySingleImageActionBehaviour.gameObject.transform.localScale = scale * Config.PIECE_BODY__SCALE_RATIO__DEFAULT;
        }


        public void SetLocalScaleOfShadow(Vector2 scale)
        {
            _shadowSingleImageActionBehaviour.gameObject.transform.localScale = scale * Config.PIECE_SHADOW__SCALE_RATIO__DEFAULT;
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public void SetAlphaOfBody(float alpha)
        {
            _bodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }


        public void SetAlphaOfShadow(float alpha)
        {
            _shadowSingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }



        // ===============================================
        // MARK: - ● Set Sorting Order ─────

        public void SetSortingOrder(ESelectType selectType)
        {
            switch (selectType)
            {
                case ESelectType.SELECTED:
                    SetSortingOrderToSelected();
                    break;

                case ESelectType.UNSELECTED:
                    SetSortingOrderToDefault();
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        public void SetSortingOrderToDefault()
        {
            UtlSprite.SetSortingOrder(gameObject: _bodySingleImageActionBehaviour.gameObject, sortingOrder: Config.PIECE_BODY__LAYERED_ORDERID__DEFAULT.SortingOrder);
            UtlSprite.SetSortingOrder(gameObject: _shadowSingleImageActionBehaviour.gameObject, sortingOrder: Config.PIECE_SHADOW__LAYERED_ORDERID__DEFAULT.SortingOrder);
        }

        public void SetSortingOrderToMovingToWaitingSpace()
        {
            UtlSprite.SetSortingOrder(gameObject: _bodySingleImageActionBehaviour.gameObject, sortingOrder: Config.PIECE_BODY__LAYERED_ORDERID__MOVINGTOWAITINGSPACE.SortingOrder);
            UtlSprite.SetSortingOrder(gameObject: _shadowSingleImageActionBehaviour.gameObject, sortingOrder: Config.PIECE_SHADOW__LAYERED_ORDERID__MOVINGTOWAITINGSPACE.SortingOrder);
        }

        public void SetSortingOrderToSelected()
        {
            UtlSprite.SetSortingOrder(gameObject: _bodySingleImageActionBehaviour.gameObject, sortingOrder: Config.PIECE_BODY__LAYERED_ORDERID__SELECTED.SortingOrder);
            UtlSprite.SetSortingOrder(gameObject: _shadowSingleImageActionBehaviour.gameObject, sortingOrder: Config.PIECE_SHADOW__LAYERED_ORDERID__SELECTED.SortingOrder);
        }



        // ? なんの関数？
        // TODO: 初期位置のみ設定しておき、最初はアニメーションで表示させるようにしたい
        public void TransformAccordingToBoardCrd(FBoardCrd boardCrd)
        {
            if (boardCrd.IsOnBoard(boardType: _boardSetBehaviour.BoardType))
            {
                Vector2 imageSize = _boardSetBehaviour.GetAreaBoardCell(boardCrd: boardCrd).CenterPoint;

                transform.position = Utl.ConvertImageSizeToUnit(imageSize: imageSize);
            }
            else
            {
                var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: _pieceState.PieceType);

                Vector2 imageSize = _boardSetBehaviour.GetAreaWaitingSpaceCell(playerType: _pieceState.PlayerType, index: index).CenterPoint;

                transform.position = Utl.ConvertImageSizeToUnit(imageSize: imageSize);
            }
        }


        // ===============================================
        // MARK: - ●  ─────

        public void MoveBoardCrdOnBoard(FBoardCrd newBoardCrd)
        {
            Assert.IsTrue(_boardCrd.IsOnBoard(boardType: _boardSetBehaviour.BoardType));
            Assert.IsTrue(_boardCrd != newBoardCrd);

            _boardCrd = newBoardCrd;
        }

        // ===============================================
        // MARK: - ●  ─────

        public void ChangePlayerTypeAndMoveToWaitingSpace()
        {
            _pieceState.ChangePlayerTypeToOppositeTypeAndSetNotNariIfNeeded();


            ChangeParentFromBoardToWaitingSpace();

            RegisterAction(action: CAction.CreatePieceActionMoveToWaitingSpace(pieceBehaviour: this, duration: 0.5f));
        }



        // ===============================================
        // MARK: - ● Change Parent Between Board And Waiting Space ─────

        // 駒を盤に設置する時の処理
        public void ChangeParentFromWaitingSpaceToBoard(FBoardCrd boardCrd)
        {
            Assert.IsTrue(_boardCrd.IsOnBoard(boardType: _boardSetBehaviour.BoardType) == false);

            _boardCrd = boardCrd;

            gameObject.transform.parent = _boardSetBehaviour.GetBoardBehaviour().transform;

            // TODO: ここでサイズ調整が必要か？
            // → アニメーションで、gameObject.transform.parent のスケールを変更すること。
            // → 影の表示レイヤーも調整必要
        }


        // 駒が取られた時の処理
        private void ChangeParentFromBoardToWaitingSpace()
        {
            Assert.IsTrue(_boardCrd.IsOnBoard(boardType: _boardSetBehaviour.BoardType));

            _boardCrd = FBoardCrd.AsOnWaitingSpace;

            gameObject.transform.parent = _boardSetBehaviour.GetWaitingSpaceBehaviour(playerType: _pieceState.PlayerType).transform;

            // TODO: ここでサイズ調整が必要か？
            // → アニメーションで、gameObject.transform.parent のスケールを変更すること。
            // → 影の表示レイヤーも調整必要
        }




        // ===============================================
        // MARK: - ●  ─────

        // 現在の pieceState に合わせて絵柄を変更する
        // - 駒が移動したのち、絵柄を変更する
        // - 駒が取られて駒台に移動したのち、絵柄を変更する
        public void UpdateTexture()
        {
            string texturePathUnderResources = _boardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceBody(pieceState: _pieceState);

            UtlSprite.ChangeTexture(gameObject: _bodySingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);


            texturePathUnderResources = _boardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceShadow(pieceState: _pieceState);

            UtlSprite.ChangeTexture(gameObject: _shadowSingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
        }




        // ===============================================
        // MARK: - ●  ─────

        // 外部から変更する場合、pieceState の ChangeToNari() を直接よんではいけない。（構造体であるため、複製されてしまう）
        public void ChangeToNari()
        {
            _pieceState.ChangeToNari();
        }


        // ===============================================
        // MARK: - ●  ─────

        public WaitingSpaceBehaviour GetWaitingSpaceBehaviour()
        {
            return _boardSetBehaviour.GetWaitingSpaceBehaviour(playerType: PieceState.PlayerType);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }


        public override void Show()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.Show();
            _shadowSingleImageActionBehaviour.Show();

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

            _bodySingleImageActionBehaviour.FadeOut();
            _shadowSingleImageActionBehaviour.FadeOut();

            _isShown = false;
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
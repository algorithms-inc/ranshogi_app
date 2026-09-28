using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class PromotionSelectorBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private GameStageBehaviour _gameStageBehaviour = null;

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;


        private SingleImageActionBehaviour _pieceBodyPromoteSingleImageActionBehaviour;
        private SingleImageActionBehaviour _pieceShadowPromoteSingleImageActionBehaviour;
        private SingleImageActionBehaviour _pieceBodyNotPromoteSingleImageActionBehaviour;
        private SingleImageActionBehaviour _pieceShadowNotPromoteSingleImageActionBehaviour;

        private ButtonBehaviour _buttonBehaviourPromote;
        private ButtonBehaviour _buttonBehaviourNotPromote;

        private Canvas _canvas;


        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _pieceBodyPromoteSingleImageActionBehaviour.Alpha;

        private EPieceType _pieceType;

        private PromotionSelectorLayouter _layouter;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(GameStageBehaviour gameStageBehaviour)
        {
            _gameStageBehaviour = gameStageBehaviour;

            _layouter = new PromotionSelectorLayouter(area: _gameStageBehaviour.BoardSetBehaviour.GetAreaSelectorOnGameStage());


            SetupGameObject();

            Hide();
        }



        // ===============================================
        // MARK: - ● Setup Game Object ─────

        private void SetupGameObject()
        {
            SetupCanvas();

            SetupBody();
            SetupButton();
            SetupPiece();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "InputCanvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.PROMOTION_SELECTOR_INPUT__LAYERED_ORDERID__DEFAULT.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupBody()
        {
            string texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForPromotionSelector.BODY);

            var gameObjectPromotionSelectorBody = UtlSprite.CreateGameObjectWithSprite(
                name: "PromotionSelectorBody",
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: Config.PROMOTION_SELECTOR_BODY__LAYERED_ORDERID__DEFAULT.SortingOrder,
                transformParent: transform);

            _bodySingleImageActionBehaviour = gameObjectPromotionSelectorBody.AddComponent<SingleImageActionBehaviour>();
            var initializer = new FSingleImageActionBehaviourInitializer(defaultAlpha: 1, durationFadeIn: 0.15f, durationFadeOut: 0.15f);
            _bodySingleImageActionBehaviour.SetupComponent(initializer: initializer);


            FArea resignSelectorArea = _layouter.AreaPromotionSelector;

            gameObjectPromotionSelectorBody.transform.position = Utl.ConvertImageSizeToUnit(imageSize: resignSelectorArea.CenterPoint);

            var localScale = resignSelectorArea.size / Config.PROMOTION_SELECTOR_BODY__IMAGE_SIZE;
            gameObjectPromotionSelectorBody.transform.localScale = localScale;
        }


        private void SetupButton()
        {
            ButtonBehaviour support(string name, EButtonImageType buttonImageType, FArea buttonArea, Action actionOnFire)
            {
                GameObject gameObjectButton = new GameObject(name: name);
                var buttonBehaviour = gameObjectButton.AddComponent<ButtonBehaviour>();

                gameObjectButton.transform.parent = gameObject.transform;

                var buttonInitializer = new FButtonInitializer(
                    targetCanvas: _canvas,
                    buttonImageType: buttonImageType,
                    area: buttonArea,
                    tapJudgeType: ETapJudgeType.CLICK,
                    isPlaySE: true,
                    isEnabled: true,
                    sortingOrder: Config.PROMOTION_SELECTOR_BUTTON__LAYERED_ORDERID__DEFAULT.SortingOrder,
                    action: actionOnFire);

                buttonBehaviour.SetupComponent(initializer: buttonInitializer);

                // 通常とは異なる短い非表示／非表示アニメーションを設定している
                buttonBehaviour.SetFadeDuration(durationFadeIn: 0.15f, durationFadeOut: 0.15f);

                return buttonBehaviour;
            }

            _buttonBehaviourPromote = support(
                name: "ButtonPromote",
                buttonImageType: EButtonImageType.BUTTON_UNRESIZABLE_PROMOTION_SELECTOR,
                buttonArea: _layouter.AreaButtonPromote,
                actionOnFire: () => { OnButtonPromoteDown(); });

            _buttonBehaviourNotPromote = support(
                name: "ButtonNotPromote",
                buttonImageType: EButtonImageType.BUTTON_UNRESIZABLE_PROMOTION_SELECTOR,
                buttonArea: _layouter.AreaButtonNotPromote,
                actionOnFire: () => { OnButtonNotPromoteDown(); });
        }


        private void SetupPiece()
        {
            SingleImageActionBehaviour support(string name, bool isBody)
            {
                var dummyPieceState = new FPieceState(playerType: EPlayerType.PLAYER1, pieceType: EPieceType.HISHA, isGyokushou: false, isNari: false);


                string texturePathUnderResources;

                if (isBody)
                {
                    texturePathUnderResources = _gameStageBehaviour.BoardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceBody(pieceState: dummyPieceState);
                }
                else
                {
                    texturePathUnderResources = _gameStageBehaviour.BoardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceShadow(pieceState: dummyPieceState);
                }


                int sortingOrder;

                if (isBody)
                {
                    sortingOrder = Config.PROMOTION_SELECTOR_PIECE_BODY__LAYERED_ORDERID__DEFAULT.SortingOrder;
                }
                else
                {
                    sortingOrder = Config.PROMOTION_SELECTOR_PIECE_SHADOW__LAYERED_ORDERID__DEFAULT.SortingOrder;
                }


                var newGameObject = UtlSprite.CreateGameObjectWithSprite(
                        name: name,
                        texturePathUnderResources: texturePathUnderResources,
                        willCacheIfNeeded: true,
                        sortingOrder: sortingOrder,
                        transformParent: transform);

                var singleImageActionBehaviour = newGameObject.AddComponent<SingleImageActionBehaviour>();

                float defaultAlpha = isBody ? 1 : Config.PIECE_SHADOW__ALPHA__DEFAULT;

                var initializer = new FSingleImageActionBehaviourInitializer(defaultAlpha: defaultAlpha, durationFadeIn: 0.15f, durationFadeOut: 0.15f);

                singleImageActionBehaviour.SetupComponent(initializer: initializer);


                return singleImageActionBehaviour;
            }

            _pieceBodyPromoteSingleImageActionBehaviour = support(name: "PieceBodyPromote", isBody: true);
            _pieceShadowPromoteSingleImageActionBehaviour = support(name: "PieceShadowPromote", isBody: false);
            _pieceBodyNotPromoteSingleImageActionBehaviour = support(name: "PieceBodyNotPromote", isBody: true);
            _pieceShadowNotPromoteSingleImageActionBehaviour = support(name: "PieceShadowNotPromote", isBody: false);


            _pieceShadowPromoteSingleImageActionBehaviour.gameObject.transform.parent = _pieceBodyPromoteSingleImageActionBehaviour.gameObject.transform;
            _pieceShadowNotPromoteSingleImageActionBehaviour.gameObject.transform.parent = _pieceBodyNotPromoteSingleImageActionBehaviour.gameObject.transform;


            _pieceBodyPromoteSingleImageActionBehaviour.gameObject.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: _layouter.AreaPiecePromote.CenterPoint);
            _pieceBodyNotPromoteSingleImageActionBehaviour.gameObject.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: _layouter.AreaPieceNotPromote.CenterPoint);

            // TODO: ここを計算で算出する
            _pieceShadowPromoteSingleImageActionBehaviour.gameObject.transform.localPosition = new Vector2(0.2f, -0.25f);
            _pieceShadowNotPromoteSingleImageActionBehaviour.gameObject.transform.localPosition = new Vector2(0.2f, -0.25f);


            var height = _layouter.AreaPiecePromote.size.y;
            var pieceImageSizeHegiht = Config.PIECE_BODY__IMAGE_SIZE.y;
            var scaleY = height / pieceImageSizeHegiht;

            _pieceBodyPromoteSingleImageActionBehaviour.gameObject.transform.localScale = new Vector2(scaleY, scaleY);
            _pieceBodyNotPromoteSingleImageActionBehaviour.gameObject.transform.localScale = new Vector2(scaleY, scaleY);
        }



        // ===============================================
        // MARK: - ●  ─────

        public void ShowLayoutArea()
        {

        }



        // ===============================================
        // MARK: - ●  ─────

        private void ChangePiece(in FPieceState pieceState)
        {
            Assert.IsTrue(Utl.CanBeNari(pieceType: pieceState.PieceType));

            if (_pieceType == pieceState.PieceType) { return; }

            _pieceType = pieceState.PieceType;

            FPieceState dummyPieceState;
            string texturePathUnderResources;

            dummyPieceState = new FPieceState(playerType: EPlayerType.PLAYER1, pieceType: _pieceType, isGyokushou: false, isNari: true);
            texturePathUnderResources = _gameStageBehaviour.BoardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceBody(pieceState: dummyPieceState);

            UtlSprite.ChangeTexture(gameObject: _pieceBodyPromoteSingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);


            dummyPieceState = new FPieceState(playerType: EPlayerType.PLAYER1, pieceType: _pieceType, isGyokushou: false, isNari: false);
            texturePathUnderResources = _gameStageBehaviour.BoardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceBody(pieceState: dummyPieceState);

            UtlSprite.ChangeTexture(gameObject: _pieceBodyNotPromoteSingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);


            texturePathUnderResources = _gameStageBehaviour.BoardSetBehaviour.PieceImageSetManager.GetFilePathOfImagePieceShadow(pieceState: dummyPieceState);

            UtlSprite.ChangeTexture(gameObject: _pieceShadowPromoteSingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
            UtlSprite.ChangeTexture(gameObject: _pieceShadowNotPromoteSingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }


        public void Show(in FPieceState pieceState)
        {
            if (_isShown) { return; }


            ChangePiece(pieceState: pieceState);
            Show();
        }


        public override void Show()
        {
            if (_isShown) { return; }


            _buttonBehaviourNotPromote.Show();
            _buttonBehaviourPromote.Show();

            _pieceBodyPromoteSingleImageActionBehaviour.Show();
            _pieceBodyNotPromoteSingleImageActionBehaviour.Show();
            _pieceShadowPromoteSingleImageActionBehaviour.Show();
            _pieceShadowNotPromoteSingleImageActionBehaviour.Show();

            _bodySingleImageActionBehaviour.Show();


            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }


            _buttonBehaviourNotPromote.Hide();
            _buttonBehaviourPromote.Hide();

            _pieceBodyPromoteSingleImageActionBehaviour.Hide();
            _pieceBodyNotPromoteSingleImageActionBehaviour.Hide();
            _pieceShadowPromoteSingleImageActionBehaviour.Hide();
            _pieceShadowNotPromoteSingleImageActionBehaviour.Hide();

            _bodySingleImageActionBehaviour.Hide();


            _isShown = false;
        }


        public void FadeIn(in FPieceState pieceState)
        {
            if (_isShown) { return; }


            ChangePiece(pieceState: pieceState);
            FadeIn();
        }



        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }


            _buttonBehaviourNotPromote.FadeIn();
            _buttonBehaviourPromote.FadeIn();

            _pieceBodyPromoteSingleImageActionBehaviour.FadeIn();
            _pieceBodyNotPromoteSingleImageActionBehaviour.FadeIn();
            _pieceShadowPromoteSingleImageActionBehaviour.FadeIn();
            _pieceShadowNotPromoteSingleImageActionBehaviour.FadeIn();

            _bodySingleImageActionBehaviour.FadeIn();


            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }


            _buttonBehaviourNotPromote.FadeOut();
            _buttonBehaviourPromote.FadeOut();

            _pieceBodyPromoteSingleImageActionBehaviour.FadeOut();
            _pieceBodyNotPromoteSingleImageActionBehaviour.FadeOut();
            _pieceShadowPromoteSingleImageActionBehaviour.FadeOut();
            _pieceShadowNotPromoteSingleImageActionBehaviour.FadeOut();

            _bodySingleImageActionBehaviour.FadeOut();


            _isShown = false;
        }



        // ===============================================
        // MARK: - ●  ─────

        private void OnButtonNotPromoteDown()
        {
            _gameStageBehaviour.OnDidSelectPromotion(isPromotionSelected: false);

            FadeOut();
        }

        private void OnButtonPromoteDown()
        {
            _gameStageBehaviour.OnDidSelectPromotion(isPromotionSelected: true);

            FadeOut();
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            ReleaseReference();

            _layouter?.Destroy();
        }


        private void ReleaseReference()
        {
            _gameStageBehaviour = null;
            _bodySingleImageActionBehaviour = null;
            _pieceBodyPromoteSingleImageActionBehaviour = null;
            _pieceShadowPromoteSingleImageActionBehaviour = null;
            _pieceBodyNotPromoteSingleImageActionBehaviour = null;
            _pieceShadowNotPromoteSingleImageActionBehaviour = null;
            _buttonBehaviourPromote = null;
            _buttonBehaviourNotPromote = null;
            _canvas = null;
        }
    }
}
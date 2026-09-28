using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// 移動開始時点で、WaitingSpace を親とした状態になっている
// 移動開始時点で、Pieceは取った側のPlayerに所属している
// バラバラにおこなっている CirclarAlpha の計算をまとめることは可能なので後に検討

// 移動完了後、駒の向きを上下逆さにする必要がある(PlayerTypeに応じたテクスチャを再度読み込む(呼び出し元で行う)
// 必要に応じて、駒を消す。カウントを修正する
// 必要に応じて、成っているのをもとに戻す

namespace GFramework
{
    public class CPieceActionMoveToWaitingSpace : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private PieceBehaviour _pieceBehaviour;


        private Vector2 _beginOfPiecePosition;

        private Vector2 _endOfPiecePosition;

        private Vector2 _beginOfBodyLocalPosition;

        private Vector2 _beginOfShadowLocalPosition;

        private float _beginOfShadowAlpha;

        private Vector2 _beginOfWholePieceLocalScale;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CPieceActionMoveToWaitingSpace(
            ActionBehaviour actionImageBehaviour,
            float duration) : base(actionImageBehaviour)
        {
            _pieceBehaviour = actionImageBehaviour as PieceBehaviour;
            Assert.IsTrue(_pieceBehaviour != null);

            _duration = duration;



            // 移動先の計算
            int index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: _pieceBehaviour.PieceState.PieceType);

            Vector2 positionByPixel = _pieceBehaviour.BoardSetBehaviour.GetAreaWaitingSpaceCell(playerType: _pieceBehaviour.PieceState.PlayerType, index: index).CenterPoint;

            _endOfPiecePosition = Utl.ConvertImageSizeToUnit(imageSize: positionByPixel);
        }

        

        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            _beginOfPiecePosition       = _pieceBehaviour.transform.position;
            _beginOfBodyLocalPosition   = _pieceBehaviour.BodySingleImageActionBehaviour.gameObject.transform.localPosition;
            _beginOfShadowLocalPosition = _pieceBehaviour.ShadowSingleImageActionBehaviour.gameObject.transform.localPosition;

            _beginOfShadowAlpha = _pieceBehaviour.AlphaOfShadow;

            _beginOfWholePieceLocalScale = _pieceBehaviour.gameObject.transform.localScale;

            _pieceBehaviour.SetSortingOrderToMovingToWaitingSpace();
        }


        protected override void ProcessMain()
        {
            float elapsedTimeAlpha = GetElapsedTimeAlpha();

            MoveWholePiece(elapsedTimeAlpha: elapsedTimeAlpha);
            MoveBody(elapsedTimeAlpha: elapsedTimeAlpha);
            MoveShadow(elapsedTimeAlpha: elapsedTimeAlpha);
            FadeShadow(elapsedTimeAlpha: elapsedTimeAlpha);

            ChangeScaleOfWholePiece(elapsedTimeAlpha: elapsedTimeAlpha);
            ChangeScaleOfBody(elapsedTimeAlpha: elapsedTimeAlpha);
            ChangeScaleOfShadow(elapsedTimeAlpha: elapsedTimeAlpha);
        }


        private void MoveWholePiece(float elapsedTimeAlpha)
        {
            Vector2 valueByUnit = Interpole.SinCurveOut(_beginOfPiecePosition, _endOfPiecePosition, elapsedTimeAlpha);

            _pieceBehaviour.transform.position = valueByUnit;
        }


        private void MoveBody(float elapsedTimeAlpha)
        {
            Vector2 peakValue = Utl.ConvertRatioToUnit(baseImageSize: _pieceBehaviour.GetBaseImageSize(), ratio: Config.PIECE_BODY__LOCAL_POSITION_RATIO__MOVETOWAITINGSPACE_PEAK);
            Vector2 valueByUnit = Interpole.Circular(baseValue: _beginOfBodyLocalPosition, peakValue: peakValue, alpha: elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfBody(unit: valueByUnit);
        }


        private void MoveShadow(float elapsedTimeAlpha)
        {
            Vector2 peakValue = Utl.ConvertRatioToUnit(baseImageSize: _pieceBehaviour.GetBaseImageSize(), ratio: Config.PIECE_SHADOW__LOCAL_POSITION_RATIO__MOVETOWAITINGSPACE_PEAK);
            Vector2 valueByUnit = Interpole.Circular(baseValue: _beginOfShadowLocalPosition, peakValue: peakValue, alpha: elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfShadow(unit: valueByUnit);
        }


        private void FadeShadow(float elapsedTimeAlpha)
        {
            float alpha = Interpole.Circular(baseValue: _beginOfShadowAlpha, peakValue: Config.PIECE_SHADOW__ALPHA__MOVETOWAITINGSPACE_PEAK, elapsedTimeAlpha);

            _pieceBehaviour.SetAlphaOfShadow(alpha: alpha);
        }


        private void ChangeScaleOfWholePiece(float elapsedTimeAlpha)
        {
            Vector2 scale = Interpole.SinCurveOut(
                _beginOfWholePieceLocalScale,
                _pieceBehaviour.BoardSetBehaviour.GetPieceImageScaleFromOriginalToOnWaitingSpace(),
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalScaleOfWhole(localScale: scale);
        }


        private void ChangeScaleOfBody(float elapsedTimeAlpha)
        {
            float alphaByCircular = Interpole.Circular(baseValue: 1f, peakValue: 1.1f, alpha: elapsedTimeAlpha);

            // TODO: new Vector2(1, 1) をレイアウトから引っ張ってくる
            Vector2 scale = new Vector2(1f, 1f) * alphaByCircular;

            _pieceBehaviour.SetLocalScaleOfBody(scale: scale * alphaByCircular);
        }


        private void ChangeScaleOfShadow(float elapsedTimeAlpha)
        {
            float alphaByCircular = Interpole.Circular(baseValue: 1f, peakValue: 1.1f, alpha: elapsedTimeAlpha);

            Vector2 scale = new Vector2(1, 1) * alphaByCircular;

            _pieceBehaviour.SetLocalScaleOfShadow(scale: scale * alphaByCircular);
        }


        protected override void ProcessOnLastFrame()
        {
            _pieceBehaviour.UpdateTexture();
            _pieceBehaviour.GetWaitingSpaceBehaviour().RegisterPiece(pieceBehaviour: _pieceBehaviour);
            _pieceBehaviour.SetSortingOrderToDefault();

            _pieceBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}


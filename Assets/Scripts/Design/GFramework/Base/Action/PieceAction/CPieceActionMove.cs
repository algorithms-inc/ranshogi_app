using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

// TODO: 最後に ZOrder を修正すること
// TODO: 速度を２段階ぐらいにしたい
// TODO: 速度なしも実現したい
// TODO: 距離によって速度を変えたい
// TODO: 非選択状態でも高く上がるようにしたい
// TODO: 桂馬の場合、より高く、そしてカーブを描きつつ移動したい

namespace GFramework
{
    public class CPieceActionMove : CAction
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
        private Vector2 _beginOfBodyLocalScale;
        private Vector2 _beginOfShadowLocalScale;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CPieceActionMove(
            ActionBehaviour actionImageBehaviour,
            float duration,
            Vector2 endOfPiecePosition) : base(actionImageBehaviour)
        {
            _pieceBehaviour = actionImageBehaviour as PieceBehaviour;
            Assert.IsTrue(_pieceBehaviour != null);

            _duration = duration;

            _endOfPiecePosition = endOfPiecePosition;
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame()
        {
            _beginOfPiecePosition = _pieceBehaviour.transform.position;

            _beginOfBodyLocalPosition = _pieceBehaviour.BodySingleImageActionBehaviour.gameObject.transform.localPosition;

            _beginOfShadowLocalPosition = _pieceBehaviour.ShadowSingleImageActionBehaviour.gameObject.transform.localPosition;

            _beginOfShadowAlpha = _pieceBehaviour.AlphaOfShadow;

            _beginOfWholePieceLocalScale = _pieceBehaviour.gameObject.transform.localScale;
            _beginOfBodyLocalScale       = _pieceBehaviour.BodySingleImageActionBehaviour.gameObject.transform.localScale;
            _beginOfShadowLocalScale     = _pieceBehaviour.ShadowSingleImageActionBehaviour.gameObject.transform.localScale;

            _pieceBehaviour.SetSortingOrderToSelected();
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
            Vector2 endOfBodyLocalPosition = Utl.ConvertRatioToUnit(baseImageSize: _pieceBehaviour.GetBaseImageSize(), ratio: Config.PIECE_BODY__LOCAL_POSITION_RATIO__DEFAULT);
            Vector2 valueByUnit = Interpole.SinCurveOut(_beginOfBodyLocalPosition, endOfBodyLocalPosition, elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfBody(unit: valueByUnit);
        }


        private void MoveShadow(float elapsedTimeAlpha)
        {
            Vector2 endOfShadowLocalPosition = Utl.ConvertRatioToUnit(baseImageSize: _pieceBehaviour.GetBaseImageSize(), ratio: Config.PIECE_SHADOW__LOCAL_POSITION_RATIO__DEFAULT);
            Vector2 valueByUnit = Interpole.SinCurveOut(_beginOfShadowLocalPosition, endOfShadowLocalPosition, elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfShadow(unit: valueByUnit);
        }


        private void FadeShadow(float elapsedTimeAlpha)
        {
            float alpha = Interpole.SinCurveInStrong(_beginOfShadowAlpha, Config.PIECE_SHADOW__ALPHA__DEFAULT, elapsedTimeAlpha);

            _pieceBehaviour.SetAlphaOfShadow(alpha: alpha);
        }


        private void ChangeScaleOfWholePiece(float elapsedTimeAlpha)
        {
            Vector2 targetScale = _pieceBehaviour.BoardSetBehaviour.GetPieceImageScaleFromOriginalToOnBoard();

            _pieceBehaviour.SetLocalScaleOfWhole(localScale: targetScale);
        }


        private void ChangeScaleOfBody(float elapsedTimeAlpha)
        {
            Vector2 scale = Interpole.SinCurveInStrong(
                _beginOfBodyLocalScale,
                Config.PIECE_BODY__SCALE_RATIO__DEFAULT,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalScaleOfBody(scale: scale);
        }


        private void ChangeScaleOfShadow(float elapsedTimeAlpha)
        {
            Vector2 scale = Interpole.SinCurveInStrong(
                _beginOfShadowLocalScale,
                Config.PIECE_SHADOW__SCALE_RATIO__DEFAULT,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalScaleOfShadow(scale: scale);
        }


        protected override void ProcessOnLastFrame()
        {
            // 選択してから移動した駒である場合、ステータスを非選択にする

            if (_pieceBehaviour.SelectType == PieceBehaviour.ESelectType.SELECTED)
            {
                _pieceBehaviour.SelectType = PieceBehaviour.ESelectType.UNSELECTED;
            }

            // TODO: 成り選択が発生する場合には、音は鳴らさないように修正する
            Locator.Instance.audioManager.Play(SEType: ESEType.PIECE_PLACE);

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
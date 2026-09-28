using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public class CPieceActionSelect : CAction
    {
        // ===============================================
        // MARK: - ● Enum ─────

        public enum ESelectActionType
        {
            SELECT,
            DESELECT
        }


        // ===============================================
        // MARK: - ● Property ─────

        private PieceBehaviour _pieceBehaviour;

        private readonly ESelectActionType _selectActionType;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CPieceActionSelect(
            ActionBehaviour actionImageBehaviour,
            ESelectActionType selectActionType,
            float duration) : base(actionImageBehaviour)
        {
            _pieceBehaviour = actionImageBehaviour as PieceBehaviour;
            Assert.IsTrue(_pieceBehaviour != null);

            _selectActionType = selectActionType;
            _duration = duration;

            Setup();
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void Setup()
        {
            // アクションは開始していないが、この時点で完了時の状態である SELECTED/UNSELECTED にしておくこと
            _pieceBehaviour.SelectType = _selectActionType switch
            {
                ESelectActionType.SELECT => PieceBehaviour.ESelectType.SELECTED,
                ESelectActionType.DESELECT => PieceBehaviour.ESelectType.UNSELECTED,
                _ => throw new InvalidOperationException(),
            };
        }



        // ===============================================
        // MARK: - ● Convert ─────

        private PieceBehaviour.ESelectType ConvertSelectActionTypeToSelectType(ESelectActionType selectActionType)
        {
            return selectActionType switch
            {
                ESelectActionType.SELECT => PieceBehaviour.ESelectType.SELECTED,
                ESelectActionType.DESELECT => PieceBehaviour.ESelectType.UNSELECTED,
                _ => throw new InvalidOperationException()
            };
        }



        // ===============================================
        // MARK: - ● Process On First Frame ─────

        protected override void ProcessOnFirstFrame()
        {
            // 選択時は最初のフレームで影や駒の配置を上にあげる
            if (_selectActionType == ESelectActionType.SELECT)
            {
                var selectType = ConvertSelectActionTypeToSelectType(selectActionType: _selectActionType);
                _pieceBehaviour.SetSortingOrder(selectType: selectType);

                Locator.Instance.audioManager.Play(SEType: ESEType.PIECE_SELECT);
            }
            else
            {
                Locator.Instance.audioManager.Play(SEType: ESEType.PIECE_DESELECT);
            }
        }



        // ===============================================
        // MARK: - ● Process Main ─────

        protected override void ProcessMain()
        {
            float elapsedTimeAlpha = GetElapsedTimeAlpha();

            switch (_selectActionType)
            {
                case ESelectActionType.SELECT:
                    MoveBodyForSelect(elapsedTimeAlpha: elapsedTimeAlpha);
                    MoveShadowForSlect(elapsedTimeAlpha: elapsedTimeAlpha);

                    FadeShadowForSelect(elapsedTimeAlpha: elapsedTimeAlpha);

                    ChangeScaleOfBodyForSelect(elapsedTimeAlpha: elapsedTimeAlpha);
                    ChangeScaleOfShadowForSlect(elapsedTimeAlpha: elapsedTimeAlpha);
                    break;

                case ESelectActionType.DESELECT:
                    MoveBodyForDeselect(elapsedTimeAlpha: elapsedTimeAlpha);
                    MoveShadowForDeselect(elapsedTimeAlpha: elapsedTimeAlpha);

                    FadeShadowForDeselect(elapsedTimeAlpha: elapsedTimeAlpha);

                    ChangeScaleOfBodyForDeselect(elapsedTimeAlpha: elapsedTimeAlpha);
                    ChangeScaleOfShadowForDeselect(elapsedTimeAlpha: elapsedTimeAlpha);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ● for Select ─────

        private void MoveBodyForSelect(float elapsedTimeAlpha)
        {
            Vector2 ratio = Interpole.SinCurveOut(
                Config.PIECE_BODY__LOCAL_POSITION_RATIO__DEFAULT,
                Config.PIECE_BODY__LOCAL_POSITION_RATIO__SELECTED,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfBodyByRatio(ratio: ratio);
        }


        private void MoveShadowForSlect(float elapsedTimeAlpha)
        {
            Vector2 ratio = Interpole.SinCurveOut(
                Config.PIECE_SHADOW__LOCAL_POSITION_RATIO__DEFAULT,
                Config.PIECE_SHADOW__LOCAL_POSITION_RATIO__SELECTED,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfShadowByRatio(ratio: ratio);
        }


        private void FadeShadowForSelect(float elapsedTimeAlpha)
        {
            float alpha = Interpole.SinCurveOut(
                Config.PIECE_SHADOW__ALPHA__DEFAULT,
                Config.PIECE_SHADOW__ALPHA__SELECTED,
                elapsedTimeAlpha);

            _pieceBehaviour.SetAlphaOfShadow(alpha: alpha);
        }


        private void ChangeScaleOfBodyForSelect(float elapsedTimeAlpha)
        {
            Vector2 scale = Interpole.SinCurveOut(
                Config.PIECE_BODY__SCALE_RATIO__DEFAULT,
                Config.PIECE_BODY__SCALE_RATIO__SELECTED,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalScaleOfBody(scale: scale);
        }


        private void ChangeScaleOfShadowForSlect(float elapsedTimeAlpha)
        {
            Vector2 scale = Interpole.SinCurveOut(
                Config.PIECE_SHADOW__SCALE_RATIO__DEFAULT,
                Config.PIECE_SHADOW__SCALE_RATIO__SELECTED,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalScaleOfShadow(scale: scale);
        }



        // ===============================================
        // MARK: - ● for Deselect ─────

        private void MoveBodyForDeselect(float elapsedTimeAlpha)
        {
            Vector2 ratio = Interpole.SinCurveIn(
                Config.PIECE_BODY__LOCAL_POSITION_RATIO__SELECTED,
                Config.PIECE_BODY__LOCAL_POSITION_RATIO__DEFAULT,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfBodyByRatio(ratio: ratio);
        }


        private void MoveShadowForDeselect(float elapsedTimeAlpha)
        {
            Vector2 ratio = Interpole.SinCurveIn(
                Config.PIECE_SHADOW__LOCAL_POSITION_RATIO__SELECTED,
                Config.PIECE_SHADOW__LOCAL_POSITION_RATIO__DEFAULT,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalPositonOfShadowByRatio(ratio: ratio);
        }


        private void FadeShadowForDeselect(float elapsedTimeAlpha)
        {
            float alpha = Interpole.SinCurveIn(
                Config.PIECE_SHADOW__ALPHA__SELECTED,
                Config.PIECE_SHADOW__ALPHA__DEFAULT,
                elapsedTimeAlpha);

            _pieceBehaviour.SetAlphaOfShadow(alpha: alpha);
        }


        private void ChangeScaleOfBodyForDeselect(float elapsedTimeAlpha)
        {
            Vector2 scale = Interpole.SinCurveIn(
                Config.PIECE_BODY__SCALE_RATIO__SELECTED,
                Config.PIECE_BODY__SCALE_RATIO__DEFAULT,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalScaleOfBody(scale: scale);
        }


        private void ChangeScaleOfShadowForDeselect(float elapsedTimeAlpha)
        {
            Vector2 scale = Interpole.SinCurveIn(
                Config.PIECE_SHADOW__SCALE_RATIO__SELECTED,
                Config.PIECE_SHADOW__SCALE_RATIO__DEFAULT,
                elapsedTimeAlpha);

            _pieceBehaviour.SetLocalScaleOfShadow(scale: scale);
        }



        // ===============================================
        // MARK: - ● Process On Last Frame ─────

        protected override void ProcessOnLastFrame()
        {
            // 選択解除時は最後のフレームで影や駒の配置を下にさげる
            if (_selectActionType == ESelectActionType.DESELECT)
            {
                var selectType = ConvertSelectActionTypeToSelectType(selectActionType: _selectActionType);
                _pieceBehaviour.SetSortingOrder(selectType: selectType);
            }

            _pieceBehaviour = null;
        }



        // ===============================================
        // MARK: - ● Forced Finish Action ─────

        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

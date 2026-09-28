using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CCellActionChangeDisplay : CAction
    {
        // ===============================================
        // MARK: - ● Enum ─────

        public enum EActionType
        {
            CHANGE_TO_PLACE_TARGET,
            CHANGE_TO_LAST_MOVED,
            CHANGE_TO_DEFAULT
        }


        // ===============================================
        // MARK: - ● Property ─────

        private CellBehaviour _cellBehaviour;
        private readonly EActionType _actionType;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CCellActionChangeDisplay(
            ActionBehaviour actionBehaviour,
            EActionType actionType,
            float duration) : base(actionBehaviour)
        {
            _cellBehaviour = actionBehaviour as CellBehaviour;
            Assert.IsTrue(_cellBehaviour != null);

            _actionType = actionType;
            _duration = duration;

            Setup();
        }



        // ===============================================
        // MARK: - ● Setup ─────

        // アクションは開始していないが、この時点で完了時の状態である SHOW/HIDE にしておくこと
        private void Setup() =>
            _cellBehaviour.SetCellType(_actionType switch
            {
                EActionType.CHANGE_TO_PLACE_TARGET => CellBehaviour.ECellType.PLACE_TARGET,
                EActionType.CHANGE_TO_LAST_MOVED   => CellBehaviour.ECellType.LAST_MOVED,
                EActionType.CHANGE_TO_DEFAULT      => CellBehaviour.ECellType.DEFAULT,
                _ => throw new InvalidOperationException()
            });



        // ===============================================
        // MARK: - ● Process On First Frame ─────

        protected override void ProcessOnFirstFrame()
        {
            // do nothing

            // 選択状態でないかのチェックのための、Assert を入れること
            // TODO: 非表示にする場合には、この時点でのアルファを拾ったほうがいいかもしれない。
        }



        // ===============================================
        // MARK: - ● Process Main ─────

        protected override void ProcessMain()
        {
            float elapsedTimeAlpha = GetElapsedTimeAlpha();

            switch (_actionType)
            {
                case EActionType.CHANGE_TO_PLACE_TARGET:
                    FadeShadowForSelect(elapsedTimeAlpha: elapsedTimeAlpha);
                    break;

                case EActionType.CHANGE_TO_LAST_MOVED:
                    FadeShadowForSelect(elapsedTimeAlpha: elapsedTimeAlpha);
                    break;

                case EActionType.CHANGE_TO_DEFAULT:
                    FadeShadowForDeselect(elapsedTimeAlpha: elapsedTimeAlpha);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ● for Select ─────

        private void FadeShadowForSelect(float elapsedTimeAlpha) =>
            _cellBehaviour.SetAlpha(
                Interpole.SinCurveOut(begin: 0, end: Config.CELL__ALPHA__PLACE_TARGET, alpha: elapsedTimeAlpha));



        // ===============================================
        // MARK: - ● for Deselect ─────

        private void FadeShadowForDeselect(float elapsedTimeAlpha) =>
            _cellBehaviour.SetAlpha(
                Interpole.SinCurveIn(begin: Config.CELL__ALPHA__PLACE_TARGET, end: 0, alpha: elapsedTimeAlpha));



        // ===============================================
        // MARK: - ● Process On Last Frame ─────

        protected override void ProcessOnLastFrame() =>
            _cellBehaviour = null;



        // ===============================================
        // MARK: - ● Forced Finish Action ─────

        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

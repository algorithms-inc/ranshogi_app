using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

// TODO: Delayを用意したい

namespace GFramework
{
    public abstract class CAction
    {
        public enum EFadeType
        {
            FADE_IN,
            FADE_OUT
        }

        protected ActionBehaviour _actionBehaviour;

        protected bool _isFirstFrame;

        protected float _duration;
        protected float _elapsedTime;


        public CAction(ActionBehaviour actionBehaviour)
        {
            _actionBehaviour = actionBehaviour;
            _isFirstFrame = true;
            _duration = 0f;
            _elapsedTime = 0f;
        }


        public void Update()
        {
            UpdateElapsedTime();

            if (_isFirstFrame)
            {
                ProcessOnFirstFrame();
                _isFirstFrame = false;
            }


            ProcessMain();


            if (IsLastFrame())
            {
                ProcessOnLastFrame();
                CloseAnimation();
            }
        }

        #region Abstract Method
        protected abstract void ProcessOnFirstFrame();
        protected abstract void ProcessMain();
        protected abstract void ProcessOnLastFrame();
        public abstract void ForceFinishAction();
        #endregion

        private void UpdateElapsedTime()
        {
            _elapsedTime += Time.deltaTime;
        }


        private void CloseAnimation()
        {
            // 親から切り離してもらう
            _actionBehaviour.PurgeAction(this);

            // 念の為、親の参照を外す
            _actionBehaviour = null;
        }


        private bool IsLastFrame() => _duration <= _elapsedTime;


        protected float GetElapsedTimeAlpha()
        {
            float result = _elapsedTime / _duration;

            return (result < 1.0f) ? result : 1.0f;
        }


        protected float GetElapsedTimeAlphaBetween(float min, float max)
        {
            float result = (_elapsedTime - min) / (max - min);

            if (result < 0.0f)
            {
                result = 0.0f;
            }
            else if (result > 1.0f)
            {
                result = 1.0f;
            }

            return result;
        }


        protected void SetElapsedTimeMax() => _elapsedTime = _duration;


        // ===============================================
        // MARK: - ● Static Method ─────


        // ===============================================
        // MARK: - ● Common ─────

        #region Static Method
        public static CAction CreateActionWait(
            ActionBehaviour actionBehaviour,
            float duration) =>
                new CActionWait(actionBehaviour: actionBehaviour, duration: duration);


        public static CAction CreateActionFire(
            ActionBehaviour actionBehaviour,
            float duration,
            Action action) => 
                new CActionFire(actionBehaviour: actionBehaviour, duration: duration, action: action);


        public static CAction CreateActionFade(
            EFadeType fadeType,
            ActionBehaviour actionImageBehaviour,
            GameObject gameObjectWithSprite,
            float baseAlpha,
            float duration)
        {
            var (beginAlpha, endAlpha) = fadeType switch
            {
                EFadeType.FADE_IN  => (0f, baseAlpha),
                EFadeType.FADE_OUT => (baseAlpha, 0f),
                _ => throw new InvalidOperationException()
            };

            return new CActionFade(
                actionImageBehaviour: actionImageBehaviour,
                actionData: new FActionData(gameObjectWithSprite: gameObjectWithSprite, beginValue: beginAlpha, endValue: endAlpha),
                duration);
        }


        public static CAction CreateActionFade(
            EFadeType fadeType,
            ActionBehaviour actionBehaviour,
            List<(GameObject, float)> gameObjectWithSpriteAndBaseAlphaList,
            float duration)
        {
            List<FActionData> actionDataList = new();

            switch (fadeType)
            {
                case EFadeType.FADE_IN:
                    foreach (var i in gameObjectWithSpriteAndBaseAlphaList)
                    {
                        var actionData = new FActionData(gameObjectWithSprite: i.Item1, beginValue: 0f, endValue: i.Item2);
                        actionDataList.Add(actionData);
                    }
                    break;

                case EFadeType.FADE_OUT:
                    foreach (var i in gameObjectWithSpriteAndBaseAlphaList)
                    {
                        var actionData = new FActionData(gameObjectWithSprite: i.Item1, beginValue: i.Item2, endValue: 0f);
                        actionDataList.Add(actionData);
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return new CActionFadeMulti(
                actionBehaviour: actionBehaviour,
                actionDataList: actionDataList,
                duration);
        }



        // ===============================================
        // MARK: - ● Image Text ─────

        public static CAction CreateImageActionFade(
            ImageActionBehaviour targetBehaviour,
            EFadeType fadeType,
            float duration,
            Action actionOnDidFinish = null) =>
                new CImageActionFade(targetBehaviour: targetBehaviour, fadeType: fadeType, duration: duration, actionOnDidFinish: actionOnDidFinish);



        // ===============================================
        // MARK: - ● Board ─────

        public static CAction CreateBoardActionShake(
            BoardBehaviour boardBehaviour,
            Vector2 magnitudeRatio,
            float duration) =>
                new CBoardActionShake(boardBehaviour: boardBehaviour, magnitudeRatio: magnitudeRatio, duration: duration);



        // ===============================================
        // MARK: - ● Piece ─────

        public static CAction CreatePieceActionSelect(
            PieceBehaviour pieceBehaviour,
            float duration) =>
                new CPieceActionSelect(actionImageBehaviour: pieceBehaviour, selectActionType: CPieceActionSelect.ESelectActionType.SELECT, duration: duration);


        public static CAction CreatePieceActionDeselect(
            PieceBehaviour pieceBehaviour,
            float duration) => 
                new CPieceActionSelect(actionImageBehaviour: pieceBehaviour, selectActionType: CPieceActionSelect.ESelectActionType.DESELECT, duration: duration);


        public static CAction CreatePieceActionMove(
            PieceBehaviour pieceBehaviour,
            float duration,
            Vector2 endOfPieceLocalPosition) =>
                new CPieceActionMove(actionImageBehaviour: pieceBehaviour, duration: duration, endOfPiecePosition: endOfPieceLocalPosition);


        public static CAction CreatePieceActionMove(
            PieceBehaviour pieceBehaviour,
            float duration,
            FBoardCrd boardCrd)
        {
            Vector2 centerPointOfBoardCell = pieceBehaviour.BoardSetBehaviour.GetAreaBoardCell(boardCrd: boardCrd).CenterPoint;

            return CreatePieceActionMove(pieceBehaviour: pieceBehaviour, duration: duration, endOfPieceLocalPosition: centerPointOfBoardCell / Config.PIXEL_PER_UNIT);
        }


        public static CAction CreatePieceActionFlip(
            PieceBehaviour pieceBehaviour,
            float duration,
            float executionTimingAlpha) => 
                new CPieceActionFlip(
                    actionImageBehaviour: pieceBehaviour,
                    duration: duration,
                    executionTimingAlpha: executionTimingAlpha);


        public static CAction CreatePieceActionMoveToWaitingSpace(
            PieceBehaviour pieceBehaviour,
            float duration) =>
                new CPieceActionMoveToWaitingSpace(actionImageBehaviour: pieceBehaviour, duration: duration);



        // ===============================================
        // MARK: - ● Piece Count ─────

        public static CAction CreatePieceCountActionIncrement(
            PieceCountBehaviour pieceCountBehaviour,
            float duration) =>
                new CPieceCountActionChangeCount(
                    actionBehaviour: pieceCountBehaviour,
                    duration: duration,
                    changeCountType: CPieceCountActionChangeCount.EChangeCountType.INCREMENT);


        public static CAction CreatePieceCountActionDecrement(
            PieceCountBehaviour pieceCountBehaviour,
            float duration) => 
                new CPieceCountActionChangeCount(
                    actionBehaviour: pieceCountBehaviour,
                    duration: duration,
                    changeCountType: CPieceCountActionChangeCount.EChangeCountType.DECREMENT);



        // ===============================================
        // MARK: - ● Cell ─────

        public static CAction CreateCellActionChangeDisplayToPlaceTarget(
            CellBehaviour cellBehaviour) =>
                new CCellActionChangeDisplay(
                    actionBehaviour: cellBehaviour,
                    actionType: CCellActionChangeDisplay.EActionType.CHANGE_TO_PLACE_TARGET,
                    duration: 0.3f); // TODO: ここを定数にする

        public static CAction CreateCellActionChangeDisplayToLastMoved(
            CellBehaviour cellBehaviour) =>
                new CCellActionChangeDisplay(
                    actionBehaviour: cellBehaviour,
                    actionType: CCellActionChangeDisplay.EActionType.CHANGE_TO_LAST_MOVED,
                    duration: 0.5f); // TODO: ここを定数にする

        public static CAction CreateCellActionChangeDisplayToDefault(
            CellBehaviour cellBehaviour) =>
                new CCellActionChangeDisplay(
                    actionBehaviour: cellBehaviour,
                    actionType: CCellActionChangeDisplay.EActionType.CHANGE_TO_DEFAULT,
                    duration: 0.2f); // TODO: ここを定数にする



        // ===============================================
        // MARK: - ● Result ─────

        public static CAction CreateRateAction(
            GameResultRateInfoBehaviour gameResultRateInfoBehaviour,
            int beginRate,
            int endRate) => 
                new RateAction(gameResultRateInfoBehaviour: gameResultRateInfoBehaviour, beginNum: beginRate, endNum: endRate);


        public static CAction CreateGradeRankAction(
            GameResultGradeRankIconBehaviour gameResultGradeRankIconBehaviour,
            EGradeRankType previousGradeRankType,
            EGradeRankType nextGradeRankType) =>
                new GradeRankAction(gameResultGradeRankIconBehaviour: gameResultGradeRankIconBehaviour, previousGradeRankType: previousGradeRankType, nextGradeRankType: nextGradeRankType);



        // ===============================================
        // MARK: - ● UI ─────

        public static CAction CreateCheckBoxActionSelectOn(
            CheckBoxBehaviour checkBoxBehaviour) =>
                new CCheckBoxActionSelect(
                    checkBoxBehaviour: checkBoxBehaviour,
                    animationType: CCheckBoxActionSelect.ECheckBoxAnimationType.SELECT_ON,
                    duration: 0.25f);


        public static CAction CreateCheckBoxActionSelectOff(
            CheckBoxBehaviour checkBoxBehaviour) =>
                new CCheckBoxActionSelect(
                    checkBoxBehaviour: checkBoxBehaviour,
                    animationType: CCheckBoxActionSelect.ECheckBoxAnimationType.SELECT_OFF,
                    duration: 0.15f); // こちらは少し時間を早くしている


        public static CAction CreateAudioActionFade(
            AudioActionBehaviour audioActionBehaviour,
            float alphaTo,
            float duration) =>
                new CAudioActionFade(
                    audioActionBehaviour: audioActionBehaviour,
                    alphaTo: alphaTo,
                    duration: duration);


        public static CAction CreateTitleLogoActionShowFull(
            TitleLogoBehaviour titleLogoBehaviour,
            float duration) =>
                new CTitleLogoActionShowFull(
                    titleLogoBehaviour: titleLogoBehaviour,
                    duration: duration);

        #endregion
    }
}

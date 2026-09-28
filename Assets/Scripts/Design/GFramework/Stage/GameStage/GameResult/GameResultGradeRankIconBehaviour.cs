using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class GameResultGradeRankIconBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private EGameMode _gameMode = default!;
        private EBoardType _boardType = default!;
        private EGradeRankType _previousGradeRankType = default!;
        private EGradeRankType _nextGradeRankType = default!;


        // -----------------------------------------------
        // MARK: - Game Object

        private bool _isShown = false;
        public bool IsShown => _isShown;

        public override float Alpha => _iconPreviousBodySingleImageActionBehaviour.Alpha;

        private SingleImageActionBehaviour _iconPreviousBodySingleImageActionBehaviour;
        private SingleImageActionBehaviour _iconNextBodySingleImageActionBehaviour;

        private SingleImageActionBehaviour _characterBodySingleImageActionBehaviour;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(EGameMode gameMode, EBoardType boardType, EGradeRankType previousGradeRankType, EGradeRankType nextGradeRankType, FArea areaIcon, FArea areaCharacter)
        {
            _gameMode = gameMode;
            _boardType = boardType;
            _previousGradeRankType = previousGradeRankType;
            _nextGradeRankType = nextGradeRankType;

            SetupGameObject(areaIcon: areaIcon, areaCharacter: areaCharacter);

            Hide();
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void SetupGameObject(FArea areaIcon, FArea areaCharacter)
        {
            SetupIconBody(area: areaIcon);
            SetupCharacterBody(area: areaCharacter);
        }


        private void SetupIconBody(FArea area)
        {
            SingleImageActionBehaviour Support(string name, EGradeRankType gradeRankType, int sortingOrder)
            {
            string texturePath = Locator.Instance.textureManager.GetFilePathOfGradeRankIcon(gradeRankType: gradeRankType, boardType: _boardType, isDefault: true);

            return UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: name,
                texturePathUnderResources: texturePath,
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn:　0.3f,
                    durationFadeOut: 0.3f),
                area: area,
                scaleMode: EScaleMode.FIT
                );
            }

            _iconPreviousBodySingleImageActionBehaviour = Support(name: "IconPreviousGradeRank", gradeRankType: _previousGradeRankType, sortingOrder: Config.GAME_RESULT_GRADE_RANK_ICON_PREVIOUS__LAYERED_ORDERID.SortingOrder);
            _iconNextBodySingleImageActionBehaviour = Support(name: "IconNextGradeRank", gradeRankType: _nextGradeRankType, sortingOrder: Config.GAME_RESULT_GRADE_RANK_ICON_NEXT__LAYERED_ORDERID.SortingOrder);
        }


        private void SetupCharacterBody(FArea area)
        {
            SingleImageActionBehaviour Support(string name, EGradeRankType gradeRankType, int sortingOrder)
            {
            string texturePath = Locator.Instance.textureManager.GetFilePathOfGradeRankCharacter(gradeRankType: gradeRankType);
            
            return UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: name,
                texturePathUnderResources: texturePath,
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f),
                area: area,
                scaleMode: EScaleMode.FIT
                );
            }

            _characterBodySingleImageActionBehaviour = Support(name: "CharacterGradeRank", gradeRankType: _previousGradeRankType, sortingOrder: Config.GAME_RESULT_GRADE_RANK_ICON_PREVIOUS__LAYERED_ORDERID.SortingOrder);
        }


        // ===============================================
        // MARK: - ● for Animation ─────

        public float GetScaleX()
        {
            return _iconNextBodySingleImageActionBehaviour.transform.localScale.x;
        }


        public void ChangeRankGradeCharacterAsNext()
        {
            string texturePath = Locator.Instance.textureManager.GetFilePathOfGradeRankCharacter(gradeRankType: _nextGradeRankType);

            UtlSprite.ChangeTexture(gameObject: _characterBodySingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePath, willCacheIfNeeded: true);
        }

        public void UpdateAnimation(Vector2 scale, float alpha)
        {
            _iconNextBodySingleImageActionBehaviour.Show();
            _iconNextBodySingleImageActionBehaviour.transform.localScale = scale;

            _iconPreviousBodySingleImageActionBehaviour.SetAlpha(alpha: 1f - alpha);
            _iconNextBodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }

        public void ClearAnimation()
        {
            ForceFinishActionIfNeeded();
            ClearAction();
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            _iconPreviousBodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
            
        }

        public override void Show()
        {
            if (_isShown) { return; }

            _iconPreviousBodySingleImageActionBehaviour.Show();
            _iconNextBodySingleImageActionBehaviour.Show();
            _characterBodySingleImageActionBehaviour.Show();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _iconPreviousBodySingleImageActionBehaviour.Hide();
            _iconNextBodySingleImageActionBehaviour.Hide();
            _characterBodySingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _characterBodySingleImageActionBehaviour.FadeIn();
            _iconPreviousBodySingleImageActionBehaviour.Hide();
            _iconNextBodySingleImageActionBehaviour.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _iconPreviousBodySingleImageActionBehaviour.FadeOut();
            _iconPreviousBodySingleImageActionBehaviour.Hide();
            _iconNextBodySingleImageActionBehaviour.FadeOut();
            _characterBodySingleImageActionBehaviour.FadeOut();

            _isShown = false;
        }


        public void FadeInWithAnimation()
        {
            if (_isShown) { return; }


            _characterBodySingleImageActionBehaviour.FadeIn();

            _iconPreviousBodySingleImageActionBehaviour.Show();
            RegisterAction(CAction.CreateGradeRankAction(gameResultGradeRankIconBehaviour: this, previousGradeRankType: _previousGradeRankType, nextGradeRankType: _nextGradeRankType));


            _isShown = true;  
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _iconPreviousBodySingleImageActionBehaviour = null;
            _iconNextBodySingleImageActionBehaviour = null;
            _characterBodySingleImageActionBehaviour = null;
        }
    }
}
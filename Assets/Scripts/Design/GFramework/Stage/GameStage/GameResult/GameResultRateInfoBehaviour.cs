using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class GameResultRateInfoBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        public enum EGameResultRateArrowType
        {
            UP, DOWN, EVEN
        }


        // -----------------------------------------------
        // MARK: - Game Object

        private bool _isFirstShow = true;

        private bool _isShown = false;
        public bool IsShown => _isShown;

        public override float Alpha => _bodySingleImageActionBehaviour.Alpha;

        private int _previousRate = 0;
        private int _nextRate = 0;

        private EGameMode _gameMode = default!;
        private EBoardType _boardType = default!;
        private EGradeRankType _previousGradeRankType = default!;
        private EGradeRankType _nextGradeRankType = default!;

        private EGameResultRateArrowType _gameResultRateArrowType = default!;

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;
        private SingleImageActionBehaviour _arrowSingleImageActionBehaviour;

        private ImageTextBehaviour _nextRateImageTextBehaviour;
        private ImageTextBehaviour _deltaRateImageTextBehaviour;

        private GameResultGradeRankIconBehaviour _gameResultGradeRankIconBehaviour;


        // -----------------------------------------------
        // MARK: -

        private GameResultRateInfoLayouter _layouter;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(EGameMode gameMode, EBoardType boardType, int previousRate, int nextRate, EGradeRankType previousGradeRankType, EGradeRankType nextGradeRankType, FArea area)
        {
            _gameMode = gameMode;
            _boardType = boardType;

            _previousRate = previousRate;
            _nextRate = nextRate;

            _previousGradeRankType = previousGradeRankType;
            _nextGradeRankType = nextGradeRankType;

            _layouter = new GameResultRateInfoLayouter(area: area);


            _gameResultRateArrowType = DetermineGameResultRateArrowType(previousRate: _previousRate, nextRate: _nextRate);


            SetupGameObject();
        }


        private EGameResultRateArrowType DetermineGameResultRateArrowType(float previousRate, float nextRate)
        {
            if (previousRate < nextRate)
            {
                return EGameResultRateArrowType.UP;
            }
            else if (previousRate > nextRate)
            {
                return EGameResultRateArrowType.DOWN;
            }
            else
            {
                return EGameResultRateArrowType.EVEN;
            }
        }


        // ===============================================
        // MARK: - ● ShowFirst ─────

        private void SetupGameObject()
        {
            SetupBody();
            SetupArrow();
            SetupNextRate();
            SetupDeltaRate();
            SetupGradeRankIcon();
        }


        private void SetupBody()
        {
            _bodySingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "RateInfo",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePathOfGameResultRateBaseImage(boardType: _boardType),
                willCacheIfNeeded: true,
                sortingOrder: Config.GAME_RESULT_RATE_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f),
                area: _layouter.AreaRateInfo,
                scaleMode: EScaleMode.FIT
                );
        }


        private void SetupArrow()
        {
            _arrowSingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "RateArrow",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePathOfGameResultRateArrowImage(rateResultArrowType: _gameResultRateArrowType),
                willCacheIfNeeded: true,
                sortingOrder: Config.GAME_RESULT_RATE_MAIN__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f),
                area: _layouter.AreaArrow,
                scaleMode: EScaleMode.FIT
                );
        }


        private void SetupNextRate()
        {
            FImageTextInitializer imageTextInitializer = new FImageTextInitializer(
                designID: 0,
                maxLength: 4,
                spacingRatioList: new float[] { 0.07f, 0.07f, 0.07f },
                textAlign: ImageTextBehaviour.ETextAlign.LEFT,
                anchorDirection: EDirection.LEFT,
                sortingOrder: Config.GAME_RESULT_RATE_MAIN__LAYERED_ORDERID.SortingOrder);

            var gameObjectNextRate = new GameObject("NextRate");
            gameObjectNextRate.transform.parent = transform;
            _nextRateImageTextBehaviour = gameObjectNextRate.AddComponent<ImageTextBehaviour>();
            _nextRateImageTextBehaviour.SetupComponent(imageTextInitializer: imageTextInitializer);

            // Color の設定
            _nextRateImageTextBehaviour.MultipleColor(color3: new FColor3(0, 0, 0));

            // Scale の設定
            var scale = _layouter.AreaNextRate.size.y / Config.IMAGE_TEXT_CHAR__IMAGE_SIZE.y;
            gameObjectNextRate.transform.localScale = new Vector2(scale, scale);

            // Translation の設定
            Vector2 centerPoint = _layouter.AreaNextRate.LeftPoint;
            Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: centerPoint);
            gameObjectNextRate.transform.position = position;


            _nextRateImageTextBehaviour.SetText(text: _nextRate.ToString());
        }


        private void SetupDeltaRate()
        {
            FImageTextInitializer imageTextInitializer = new FImageTextInitializer(
                designID: 0,
                maxLength: 3,
                spacingRatioList: new float[] { 0.07f, 0.07f },
                textAlign: ImageTextBehaviour.ETextAlign.LEFT,
                anchorDirection: EDirection.LEFT,
                sortingOrder: Config.GAME_RESULT_RATE_MAIN__LAYERED_ORDERID.SortingOrder);

            var gameObjectDeltaRate = new GameObject("DeltaRate");
            gameObjectDeltaRate.transform.parent = transform;
            _deltaRateImageTextBehaviour = gameObjectDeltaRate.AddComponent<ImageTextBehaviour>();
            _deltaRateImageTextBehaviour.SetupComponent(imageTextInitializer: imageTextInitializer);

            // Color の設定

            FColor3 color3 = new FColor3(0, 0, 0);

            color3 = _gameResultRateArrowType switch
            {
                EGameResultRateArrowType.UP => new FColor3(0, 0.25f, 0),
                EGameResultRateArrowType.DOWN => new FColor3(0.25f, 0, 0),
                EGameResultRateArrowType.EVEN => new FColor3(0, 0, 0.5f),
                _ => throw new InvalidOperationException(),
            };
            _deltaRateImageTextBehaviour.MultipleColor(color3: color3);

            // Scale の設定
            var scale = _layouter.AreaDeltaRate.size.y / Config.IMAGE_TEXT_CHAR__IMAGE_SIZE.y;
            gameObjectDeltaRate.transform.localScale = new Vector2(scale, scale);

            // Translation の設定
            Vector2 centerPoint = _layouter.AreaDeltaRate.LeftPoint;
            Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: centerPoint);
            gameObjectDeltaRate.transform.position = position;


            var deltaRate = _nextRate - _previousRate;


            if (deltaRate == 0)
            {
                _deltaRateImageTextBehaviour.SetText(text: deltaRate.ToString());

                // ここでプラマイ表示を行う（勝ちならプラス、負けならマイナス、で表示すると複数デバイス起動時におかしくなる）
            }
            else if (deltaRate >= 1)
            {
                _deltaRateImageTextBehaviour.SetText(text: deltaRate.ToString());

                // ここでプラマイ表示を行う（勝ちならプラス、負けならマイナス、で表示すると複数デバイス起動時におかしくなる）
            }
            else // deltaRate <= -1 
            {
                _deltaRateImageTextBehaviour.SetText(text: (deltaRate * -1).ToString());

                // ここでプラマイ表示を行う（勝ちならプラス、負けならマイナス、で表示すると複数デバイス起動時におかしくなる）
            }
        }


        private void SetupGradeRankIcon()
        {
            FImageTextInitializer imageTextInitializer = new FImageTextInitializer(
                designID: 0,
                maxLength: 3,
                spacingRatioList: new float[] { 0.07f, 0.07f },
                textAlign: ImageTextBehaviour.ETextAlign.LEFT,
                anchorDirection: EDirection.LEFT,
                sortingOrder: Config.GAME_RESULT_RATE_MAIN__LAYERED_ORDERID.SortingOrder);

            var gameObjectGradeRank = new GameObject("GradeRankIcon");
            gameObjectGradeRank.transform.parent = transform;
            _gameResultGradeRankIconBehaviour = gameObjectGradeRank.AddComponent<GameResultGradeRankIconBehaviour>();
            _gameResultGradeRankIconBehaviour.SetupComponent(
                gameMode: _gameMode,
                boardType: _boardType,
                previousGradeRankType: _previousGradeRankType,
                nextGradeRankType: _nextGradeRankType,
                areaIcon: _layouter.AreaGradeRankIcon,
                areaCharacter: _layouter.AreaGradeRankCharacter);
        }



        // ===============================================
        // MARK: - ●  ─────

        public void ChangeRate(int rate)
        {
            _nextRateImageTextBehaviour.SetText(text: rate.ToString());
        }


        // ===============================================
        // MARK: - ● Clear Animation ─────


        // 盤面表示や次の対局に進む場合などでアニメーションを強制終了させる
        public void ClearAnimation()
        {
            _gameResultGradeRankIconBehaviour.ClearAnimation();
            _gameResultGradeRankIconBehaviour.ChangeRankGradeCharacterAsNext();
        }

        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            _bodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
            _arrowSingleImageActionBehaviour.SetAlpha(alpha: alpha);
            _nextRateImageTextBehaviour.SetAlpha(alpha: alpha);
            _deltaRateImageTextBehaviour.SetAlpha(alpha: alpha);
        }


        public override void Show()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.Show();
            _arrowSingleImageActionBehaviour.Show();
            _nextRateImageTextBehaviour.Show();
            _deltaRateImageTextBehaviour.Show();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.Hide();
            _arrowSingleImageActionBehaviour.Hide();
            _nextRateImageTextBehaviour.Hide();
            _deltaRateImageTextBehaviour.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.FadeIn();
            _arrowSingleImageActionBehaviour.FadeIn();
            _nextRateImageTextBehaviour.FadeIn();
            _deltaRateImageTextBehaviour.FadeIn();
            
            if (_isFirstShow && (_previousRate != _nextRate))
            {
                RegisterAction(CAction.CreateRateAction(gameResultRateInfoBehaviour: this, beginRate: _previousRate, endRate: _nextRate));

                _gameResultGradeRankIconBehaviour.FadeInWithAnimation();

                _isFirstShow = false;
            }
            else
            {
                _gameResultGradeRankIconBehaviour.FadeIn();
            }

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.FadeOut();
            _arrowSingleImageActionBehaviour.FadeOut();
            _nextRateImageTextBehaviour.FadeOut();
            _deltaRateImageTextBehaviour.FadeOut();
            _gameResultGradeRankIconBehaviour.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _bodySingleImageActionBehaviour = null;
            _arrowSingleImageActionBehaviour = null;

            _nextRateImageTextBehaviour = null;
            _deltaRateImageTextBehaviour = null;
        }
    }
}
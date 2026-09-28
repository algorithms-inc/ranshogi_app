using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class OpponentSearcherBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private GameStageBehaviour _gameStageBehaviour;

        private EGameMode _gameMode;


        // -----------------------------------------------
        // MARK: - Game Object

        private Canvas _canvas;

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _messageTextFiledBehaviour.Alpha;

        private TextFieldBehaviour _messageTextFiledBehaviour;
        private FrameAnimationBehaviour _waitingFrameAnimationBehaviour;
        private ButtonBehaviour _buttonBackBehaviour;



        // -----------------------------------------------
        // MARK: - Layout

        private OpponentSearcherLayouter _layouter;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(GameStageBehaviour gameStageBehaviour, EGameMode gameMode)
        {
            _gameStageBehaviour = gameStageBehaviour;
            _gameMode = gameMode;

            _layouter = new OpponentSearcherLayouter();

            SetupCanvas();

            SetupButton();

            SetupMessageText();

            SetupWaitignFrameAnimation();

            Hide();
        }


        private void SetupMessageText()
        {
            var gameObjectText = new GameObject(name: "Name");
            _messageTextFiledBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            string textString = _gameStageBehaviour.GameMode switch
            {
                EGameMode.NORMAL => "対局相手を探しています",
                EGameMode.WITH_FRIEND => "友達の入室を待っています",
                _ => throw new InvalidOperationException(),
            };

            FArea area = _layouter.AreaText;

            var textInitializer = new FTextFieldInitializer(
                text: textString,
                fontType: EFontType.SERIF,
                fontSize: _layouter.FontSize,
                alignment: EAlignment.MIDDLE_CENTER,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _messageTextFiledBehaviour.SetupComponent(textInitializer: textInitializer);
        }

        private void SetupWaitignFrameAnimation()
        {
            var gameObjectFrameAnimation = new GameObject(name: "WaitingFrameAnimation");
            _waitingFrameAnimationBehaviour = gameObjectFrameAnimation.AddComponent<FrameAnimationBehaviour>();
            gameObjectFrameAnimation.transform.parent = transform;

            FArea area = _layouter.AreaAnimation;

            _waitingFrameAnimationBehaviour.SetupComponent(new FFrameAnimationInitializer(
                singleImageActionBehaviourInitializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1f,
                    durationFadeIn: Config.ANIMATION_DURATION_2,
                    durationFadeOut: Config.ANIMATION_DURATION_2),
                texturePathUnderResourcesBase: "UI/WaitingAnimation/waiting_animation_",
                animationCounter: new CCycleCounter(count: 0, min: 0, cycle: 10),
                animationWait: 3,
                sortingOrder: Config.OPPONENT_SEARCHER_ANIMATION_LAYERED_ORDERID.SortingOrder,
                isAnimationOn: true,
                isShown: true));

            gameObjectFrameAnimation.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: area.CenterPoint);

            var textureSize = UtlSprite.GetTextureSize(gameObject: gameObjectFrameAnimation);
            var areaWidth = area.size.x;
            var scale = areaWidth * 0.7f / textureSize.x;

            gameObjectFrameAnimation.transform.localScale = new Vector2(x: scale, y: scale);

            _waitingFrameAnimationBehaviour.Hide();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.OPPONENT_SEARCHER_INPUT_CANVAS__LAYERED_ORDERID.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupButton()
        {
            ButtonBehaviour Support(string name, string buttonText, EButtonImageType buttonImageType, FArea buttonArea, Action action)
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
                    sortingOrder: Config.OPPONENT_SEARCHER_BUTTON__LAYERED_ORDERID.SortingOrder,
                    action: action);

                buttonBehaviour.SetupComponent(initializer: buttonInitializer);


                // テストでテキストを表示
                var buttonTextInitializer = new FButtonTextInitializer(
                    text: buttonText,
                    fontType: EFontType.SERIF,
                    fontSize: (int)(buttonArea.size.y * 0.5f),
                    alignment: EAlignment.MIDDLE_CENTER);
                buttonBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);


                return buttonBehaviour;
            }

            var buttonText = (_gameMode == EGameMode.NORMAL) ? "ホームへ戻る" : "友達対局メニューへ戻る";


            _buttonBackBehaviour = Support(
                name: "ButtonBack",
                buttonText: buttonText,
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonBack,
                action: () => { OnButtonBackDown(); });
        }

        private void OnButtonBackDown()
        {
            _gameStageBehaviour.OnTapButtonBackOfOpponentSearcher();
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositon(Vector2 unit)
        {
            // SetLocalPositon(gameObject: gameObjectWindowBody, unit: unit);
        }


        public void SetLocalPositonByRatio(Vector2 ratio)
        {
            // SetLocalPositonByRatio(gameObject: gameObjectWindowBody, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            transform.localScale = new Vector2(x: 1f, y: 1f);
        }


        // ===============================================
        // MARK: - ●  ─────


        private void SetupGameObject()
        {
            SetupBody();
            SetupBaseImageSize();

            SetLocalScale();
        }


        private void SetupBody()
        {
        }


        private void SetupBaseImageSize()
        {
            // TODO: ここを実装する
            // baseImageSize = Utl.ConvertImageSizeToUnit(imageSize);
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

            _buttonBackBehaviour.Show();
            _waitingFrameAnimationBehaviour.ShowAndOn();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _buttonBackBehaviour.Hide();
            _waitingFrameAnimationBehaviour.HideAndOff();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _buttonBackBehaviour.FadeIn();
            _messageTextFiledBehaviour.FadeIn();
            _waitingFrameAnimationBehaviour.SetIsAnimationOn(value: true);
            _waitingFrameAnimationBehaviour.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _buttonBackBehaviour.FadeOut();
            _messageTextFiledBehaviour.FadeOut();
            _waitingFrameAnimationBehaviour.FadeOut();


            var action = CAction.CreateActionFire(
                actionBehaviour: this,
                duration: Config.ANIMATION_DURATION_4,
                action: actionOnDidFinish);

            RegisterAction(action: action);

            _isShown = false;
        }


        public void FadeOutWithInform(bool isForGame)
        {
            if (_isShown == false) { return; }

            FadeOut(actionOnDidFinish: () => { _gameStageBehaviour.OnDidOpponentSearcherHideAnimation(isForGame: isForGame); });
        }


        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _layouter?.Destroy();

            _canvas = null;
        }
    }
}
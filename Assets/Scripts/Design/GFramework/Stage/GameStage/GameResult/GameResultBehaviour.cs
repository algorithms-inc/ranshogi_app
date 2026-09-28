using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class GameResultBehaviour : ActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private GameStageBehaviour _gameStageBehaviour = null;

        private EGameMode _gameMode;

        private FGameResultData _gameResultData;

        private bool _isShowBoardMode = false;



        private bool _isShown = false;
        public bool IsShown => _isShown;


        // -----------------------------------------------
        // MARK: - Game Object

        private Canvas _canvas;
        public Canvas Canvas => _canvas;

        private CurtainBehaviour _curtainBehaviour;

        private GameResultKeyVisualBehaviour _keyVisualBehaviour;
        private GameResultRateInfoBehaviour _gameResultRateInfoBehaviour;

        private readonly List<TextFieldBehaviour> _textFieldBehaviourList = new List<TextFieldBehaviour>();
        private List<ButtonBehaviour> _buttonBehaviourList = new();

        private TapInputBehaviour _tapInputFullScreenBehaviour;

        private ButtonBehaviour _buttonHistoryDetailBehaviour;
        private ButtonBehaviour _buttonHomeBehaviour;
        private ButtonBehaviour _buttonShowBoardBehaviour;
        private ButtonBehaviour _buttonNextGameBehaviour;
        private ButtonBehaviour _buttonDetailBehaviour;


        // -----------------------------------------------
        // MARK: - Layout

        private GameResultLayouter _layouter;

        private GameStageLayouter _layouter2;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(GameStageBehaviour gameStageBehaviour)
        {
            _gameStageBehaviour = gameStageBehaviour;

            _layouter = new GameResultLayouter();
            _layouter2 = new GameStageLayouter();

            SetupCanvas();
        }



        // ===============================================
        // MARK: - ● ShowFirst ─────

        public void ShowFirst(EGameMode gameMode, FGameResultData gameResultData)
        {
            _gameMode = gameMode;
            _gameResultData = gameResultData;

            _gameStageBehaviour.SetIsButtonSettingEnabled(value: false);

            SetupKeyVisual();

            SetupRate();

            SetupCurtain();

            SetupTapInputFullScreen();

            SetupButton();


            FadeIn();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.GAME_RESULT_INPUT_CANVAS__LAYERED_ORDERID.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupKeyVisual()
        {
            var newGameObject = new GameObject(name: "KeyVisual");
            _keyVisualBehaviour = newGameObject.AddComponent<GameResultKeyVisualBehaviour>();


            _keyVisualBehaviour.SetupComponent(
                gameMode: _gameMode,
                gameResultType: _gameResultData.GameResultType,
                consecutiveCount: _gameResultData.ConsecutiveCount,
                area: _layouter.AreaKeyVisual);
        }




        private void SetupRate()
        {
            var newGameObject = new GameObject(name: "NextRateInfo");
            _gameResultRateInfoBehaviour = newGameObject.AddComponent<GameResultRateInfoBehaviour>();

            _gameResultRateInfoBehaviour.SetupComponent(
                gameMode: _gameMode,
                boardType: _gameStageBehaviour.BoardType,
                previousRate: _gameResultData.PreviousRate,
                nextRate: _gameResultData.NextRate,
                previousGradeRankType: _gameResultData.PreviousGradeRankType,
                nextGradeRankType: _gameResultData.NextGradeRankType,
                area: _layouter.AreaRate.AsYReversed()); // TODO：.AsYReversed() をなくせないか？
        }


        private void SetupCurtain()
        {
            var gameObjectCurtain = new GameObject("Curtain");
            gameObjectCurtain.transform.parent = gameObject.transform;

            CurtainBehaviour.ECurtainType curtainType = _gameResultData.GameResultType switch
            {
                EGameResultType.WIN => CurtainBehaviour.ECurtainType.GAME_RESULT_WIN,
                EGameResultType.LOSE => CurtainBehaviour.ECurtainType.GAME_RESULT_LOSE,
                EGameResultType.DRAW => CurtainBehaviour.ECurtainType.GAME_RESULT_WIN, // TODO: ひとまず、引き分けのときにも勝利の色で設定
                _ => throw new InvalidOperationException(),
            };

            _curtainBehaviour = gameObjectCurtain.AddComponent<CurtainBehaviour>();
            _curtainBehaviour.SetupComponent(curtainType: curtainType);
        }


        private void SetupTapInputFullScreen()
        {
            // 対局中設定ボタンの判定の方を優先するため、BoardSet の Canvas に設定
            _tapInputFullScreenBehaviour = Utl.CreateTapInputFullScreen(
                canvas: _gameStageBehaviour.BoardSetBehaviour.GetBoardSetInputCanvas(),
                isEnabled: true,
                action: (ETapInputType tapInputType, int durationFrame) => { OnTapFullScreen(); });
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
                    sortingOrder: Config.GAME_RESULT_BUTTON__LAYERED_ORDERID.SortingOrder,
                    action: action);

                buttonBehaviour.SetupComponent(initializer: buttonInitializer);


                var buttonTextInitializer = new FButtonTextInitializer(
                    text: buttonText,
                    fontType: EFontType.SERIF,
                    fontSize: (int)(buttonArea.size.y * 0.5f),
                    alignment: EAlignment.MIDDLE_CENTER);
                buttonBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);


                return buttonBehaviour;
            }

            _buttonDetailBehaviour = Support(
                name: "ButtonDetail",
                buttonText: "詳細",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter2.AreaResignButton,
                action: () => { OnTapShowDetail(); });
            _buttonBehaviourList.Add(_buttonDetailBehaviour);

            _buttonHistoryDetailBehaviour = Support(
                name: "ButtonHistoryDetail",
                buttonText: "検討",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonHistoryDetail,
                action: () => { _gameStageBehaviour.OnTapButtonHistoryDetail(); });
            _buttonBehaviourList.Add(_buttonHistoryDetailBehaviour);

            _buttonShowBoardBehaviour = Support(
                name: "ButtonShowBoard",
                buttonText: "盤面",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonShowBoard,
                action: () => { OnTapShowBoard(); });
            _buttonBehaviourList.Add(_buttonShowBoardBehaviour);

            _buttonNextGameBehaviour = Support(
                name: "ButtonNextGame",
                buttonText: _gameStageBehaviour.NextGameButtonText,
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                buttonArea: _layouter.AreaButtonNextGame,
                action: () => { _gameStageBehaviour.OnTapButtonNextGameOnGameResult(); });
            _buttonBehaviourList.Add(_buttonNextGameBehaviour);

            _buttonHomeBehaviour = Support(
                name: "ButtonHome",
                buttonText: _gameStageBehaviour.HomeButtonText,
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonHome,
                action: () => { _gameStageBehaviour.OnTapButtonHomeOnGameResult(); });
            _buttonBehaviourList.Add(_buttonHomeBehaviour);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void FadeIn()
        {
            Assert.IsTrue(_isShown == false);

            _keyVisualBehaviour.FadeIn();
            _gameResultRateInfoBehaviour.FadeIn();
            _curtainBehaviour.FadeIn();
            FadeInText();
            FadeInButton();

            _isShown = true;
        }


        private void FadeInText()
        {
            _textFieldBehaviourList.ForEach(i => i.Show());
        }


        private void FadeInButton()
        {
            _buttonBehaviourList.ForEach(i => i.Show());
        }


        public void FadeOut()
        {
            Assert.IsTrue(_isShown);

            _keyVisualBehaviour.FadeOut();
            _gameResultRateInfoBehaviour.FadeOut();
            _curtainBehaviour.FadeOut();
            FadeOutText();
            FadeOutButton();

            _isShown = false;
        }


        private void FadeOutText()
        {
            _textFieldBehaviourList.ForEach(i => i.FadeOut());
        }


        private void FadeOutButton()
        {
            _buttonBehaviourList.ForEach(i => i.FadeOut());
        }



        // ===============================================
        // MARK: - ● On Event ─────

        private void OnTapFullScreen()
        {
            if (_isShowBoardMode == false) { return; }

            _gameStageBehaviour.SetIsButtonSettingEnabled(value: false);

            _isShowBoardMode = false;

            _keyVisualBehaviour.FadeIn();
            _gameResultRateInfoBehaviour.FadeIn();
            FadeInButton();
            FadeInText();
            _curtainBehaviour.FadeIn();
        }


        private void OnTapShowBoard()
        {
            Assert.IsTrue(_isShowBoardMode == false);

            _gameStageBehaviour.SetIsButtonSettingEnabled(value: true);

            _isShowBoardMode = true;

            _keyVisualBehaviour.FadeOut();
            _gameResultRateInfoBehaviour.ClearAnimation();
            _gameResultRateInfoBehaviour.FadeOut();
            FadeOutButton();
            FadeOutText();
            _curtainBehaviour.FadeOut();
        }


        // ===============================================
        // MARK: - ● On Tap Show Detail ─────

        private void OnTapShowDetail()
        {
            AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldTitle",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.0f),
                    endPoint: new Vector2(1f, 0.15f)),
                text: "対局結果詳細",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_CENTER,
                actionOnTap: null);


            void Support2(
                String name,
                int lineIndex,
                String textSubheading,
                String textContent)
            {
                var areaSubheading = new FWindowElementArea(
                    origin: new Vector2(0f, 0.2f + (0.125f * lineIndex)),
                    endPoint: new Vector2(0.35f, 0.35f + (0.125f * lineIndex)));

                AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                    name: name + "Subheading",
                    area: areaSubheading,
                    text: textSubheading,
                    fontScale: 0.8f,
                    fontType: EFontType.SERIF,
                    textColorType: EWindowTextColorType.NORMAL,
                    alignment: EAlignment.TOP_RIGHT,
                    actionOnTap: null);

                var areaContent = new FWindowElementArea(
                    origin: new Vector2(0.37f, 0.2f + (0.125f * lineIndex)),
                    endPoint: new Vector2(1f, 0.35f + (0.125f * lineIndex)));

                AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                    name: name,
                    area: areaContent,
                    text: textContent,
                    fontScale: 0.8f,
                    fontType: EFontType.SERIF,
                    textColorType: EWindowTextColorType.NORMAL,
                    alignment: EAlignment.TOP_LEFT,
                    actionOnTap: null);
            }


            Support2(
                name: "textFieldBoardType",
                lineIndex: 0,
                textSubheading: "盤：",
                textContent: _gameStageBehaviour.BoardType.ConvertToString());


            var (nameBlack, nameWhite) = _gameResultData.MoveOrderTypePlayer1 == EMoveOrderType.FIRST 
                ? (_gameResultData.NamePlayer1, _gameResultData.NamePlayer2) 
                : (_gameResultData.NamePlayer2, _gameResultData.NamePlayer1);

            Support2(
                name: "textFieldPlayerBlack",
                lineIndex: 1,
                textSubheading: "先手：",
                textContent: nameBlack);

            Support2(
                name: "textFieldPlayerWhite",
                lineIndex: 2,
                textSubheading: "後手：",
                textContent: nameWhite);

            // Support2(
            //     name: "textFieldMoves",
            //     lineIndex: 2,
            //     textSubheading: "手数：",
            //     textContent: "N/Y 手数");

            Support2(
                name: "textFieldResult",
                lineIndex: 3,
                textSubheading: "結果：",
                textContent: GetWinLoseResult());

            Support2(
                name: "textFieldReason",
                lineIndex: 4,
                textSubheading: "決まり手：",
                textContent: GetWinLoseReasonDetail());

            string rateString = "";
            if (_gameResultData.PreviousRate == _gameResultData.NextRate)
            {
                rateString = _gameResultData.PreviousRate.ToString() + " (±0)";
            }
            else
            {
                var deltaRate = _gameResultData.GetDeltaRate();
                var deltaRateString = deltaRate > 0 ? "+" + deltaRate.ToString() : deltaRate.ToString();

                rateString = _gameResultData.PreviousRate.ToString() + " → " + _gameResultData.NextRate.ToString() + " (" + deltaRateString + ")";
            }

            Support2(
                name: "textFieldPreviousRate",
                lineIndex: 5,
                textSubheading: "レート：",
                textContent: rateString);


            string rankGradeSting = "";

            if (_gameResultData.PreviousGradeRankType == _gameResultData.NextGradeRankType)
            {
                rankGradeSting = _gameResultData.PreviousGradeRankType.ToJapaneseName();
            }
            else
            {
                rankGradeSting = _gameResultData.PreviousGradeRankType.ToJapaneseName() + " → " + _gameResultData.NextGradeRankType.ToJapaneseName();
            }

            Support2(
                name: "textFieldGradeRank",
                lineIndex: 6,
                textSubheading: "段位：",
                textContent: rankGradeSting);


            AppManager.Instance.UICreator.WindowNormal.Button.Add(
                name: "buttonClose",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 1.425f),
                    endPoint: new Vector2(1f, 1.6f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "閉じる",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
                });


            AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
        }


        private String GetWinLoseResult()
        {
            switch (_gameResultData.GameResultType)
            {
                case EGameResultType.WIN:
                    return "勝ち";

                case EGameResultType.LOSE:
                    return "負け";

                case EGameResultType.DRAW:
                    return "引き分け";

                default:
                    throw new InvalidOperationException();
            }
        }

        private String GetWinLoseReasonDetail()
        {
            return _gameResultData.GameResultReasonType.ToString(gameResultType: _gameResultData.GameResultType);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unity Editor での表示の場合停止タイミングで _layouter が null となるためチェックして破壊
            _layouter?.Destroy();
            _layouter = null;


            _gameStageBehaviour = null;

            _canvas = null;

            _curtainBehaviour = null;

            _keyVisualBehaviour = null;
            _gameResultRateInfoBehaviour = null;

            _textFieldBehaviourList.Clear();
            _buttonBehaviourList.Clear();

            _tapInputFullScreenBehaviour = null;

            _buttonHistoryDetailBehaviour = null;
            _buttonHomeBehaviour = null;
            _buttonShowBoardBehaviour = null;
            _buttonNextGameBehaviour = null;
            _buttonDetailBehaviour = null;
        }
    }
}
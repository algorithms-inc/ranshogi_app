using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;
using GFramework;
using System.Linq.Expressions;

// TODO: 外部からの設定データの変換をHistoryDetailでも利用したいので、Staticにしたい
namespace GFramework
{
    public enum EGameMode
    {
        NORMAL,
        WITH_FRIEND
    }

    public enum EGameStartMode
    {
        NEW_GAME,
        RESUME
    }

    public enum EGameStageState
    {
        PRE_OPPONENT_SEARCHING,
        OPPONENT_SEARCHING,
        PRE_GAME,
        ON_GAME,
        GAME_RESULT
    }


    public class GameStageBehaviour : ActionBehaviour, IGameStage
    {
        // ===============================================
        // MARK: - ● Property ─────

        // GameManager
        private IGameManager _gameManager;

        // プレイヤーの番手
        private EMoveOrderType _moveOrderTypeOfPlayer1 = default!;


        // General

        private EGameMode _gameMode;
        public EGameMode GameMode => _gameMode;

        private EBoardType _boardType = default!;
        public EBoardType BoardType => _boardType;

        private EGameStartMode _gameStartMode;
        public EGameStartMode GameStartMode => _gameStartMode;

        private EGameStageState _gameStageState;
        public EGameStageState GameStageState => _gameStageState;

        private string _nextGameButtonText = "次の対局へ";
        private string _homeButtonText = "ホームへ";
        public string NextGameButtonText => _nextGameButtonText;

        public string HomeButtonText => _homeButtonText;


        // Opponent Searcher

        private OpponentSearcherBehaviour _opponentSearcherBehaviour;



        // Board Set

        private BoardSetBehaviour _boardSetBehaviour;
        public BoardSetBehaviour BoardSetBehaviour => _boardSetBehaviour;


        // 成り選択ウィンドウ
        private PromotionSelectorBehaviour _promotionSelectorBehaviour;

        private PieceBehaviour _promotionSelectingPieceBehaviour;


        // 設定ボタン
        private ButtonBehaviour _buttonSettingBehaviour;

        // 投了ボタン
        private ButtonBehaviour _buttonResignBehaviour;

        // AI対戦だけに出す「待った」「戻る」ボタン
        private ButtonBehaviour _buttonUndoBehaviour;
        private ButtonBehaviour _buttonBackBehaviour;
        private bool _isAiGameButtonsShown = false;

        // 投了選択ウィンドウ
        private ResignSelectorBehaviour _resignSelectorBehaviour;

        
        // シーン切り替え
        private string _nextSceneName = "";



        // Text Balloon
        public enum ETextBalloonType
        {
            NORMAL,
            ERROR,
        }

        private TextBalloonBehaviour _textBalloonNormalBehaviour;
        private TextBalloonBehaviour _textBalloonErrorBehaviour;

        // Game Result

        private GameResultBehaviour _gameResultBehaviour;



        // -----------------------------------------------
        // MARK: - Layout

        private GameStageLayouter _layouter;


        #region  Setup Component
        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(
            IGameManager gameManager,
            EGameMode gameMode,
            EBoardType boardType,
            EGameStartMode gameStartMode)
        {
            _gameManager = gameManager;
            _gameMode = gameMode;
            _boardType = boardType;
            _gameStartMode = gameStartMode;


            _gameStageState = _gameStartMode switch
            {
                EGameStartMode.NEW_GAME => EGameStageState.PRE_OPPONENT_SEARCHING,
                EGameStartMode.RESUME => EGameStageState.PRE_GAME,
                _ => throw new NotImplementedException(),
            };


            _layouter = new GameStageLayouter();

            SetupOpponentSearcher();


            Locator.Instance.audioManager.PlayDefaultStageBGM();
        }
        #endregion


        #region Setup Opponent Searcher
        // ===============================================
        // MARK: - ● Setup Opponent Searcher ─────

        private void SetupOpponentSearcher()
        {
            var gameObjectOpponentSearcher = new GameObject("OpponentSearcher");
            gameObjectOpponentSearcher.transform.parent = transform;
            _opponentSearcherBehaviour = gameObjectOpponentSearcher.AddComponent<OpponentSearcherBehaviour>();
            _opponentSearcherBehaviour.SetupComponent(gameStageBehaviour: this, gameMode: _gameMode);
        }
        #endregion


        #region Setup Parts For Game
        // ===============================================
        // MARK: - ● Setup Parts For Game ─────

        private void SetupPartsForGame(in FGameStageInitializer gameStageInitializer)
        {
            transform.position = new Vector3(0f, 0f, 0f);


            SetupBoardSet(gameStageInitializer: gameStageInitializer);


            

            

            SetupButtonResign();

            

            SetupPlayerInfo(stageInitializer: gameStageInitializer);

            SetupPromotionSelector();

            SetupResignSelector();

            SetupTextBalloon();

            SetupGameResult();

            SetupButtonSetting();
        }


        #endregion


        // ===============================================
        // MARK: - ● OpponentSearcherBehaviour からの通知 ─────

        // OpponentSearcher でバックボタンが押された
        public void OnTapButtonBackOfOpponentSearcher()
        {
            Assert.IsTrue(_gameStageState == EGameStageState.OPPONENT_SEARCHING);


            _gameManager.OnTapBackOnOpponentSearcher();
        }


        // OpponentSearcher の非表示が終わった
        public void OnDidOpponentSearcherHideAnimation(bool isForGame)
        {
            Assert.IsTrue(_gameStageState == EGameStageState.OPPONENT_SEARCHING);

            if (isForGame)
            {
                _gameStageState = EGameStageState.PRE_GAME;

                _gameManager.OnDidHideOpponentSearcherForGame();
            }
            else
            {
                AppManager.Instance.DestroyStageAndLoadScene(nextSceneName: _nextSceneName);
            }
        }



        // ===============================================
        // MARK: - ● Implement Interface ─────

        // ===============================================
        // MARK: - ● Opponent Searcher ─────

        public void ShowOpponentSearcher()
        {
            Assert.IsTrue(_gameStageState == EGameStageState.PRE_OPPONENT_SEARCHING);


            _gameStageState = EGameStageState.OPPONENT_SEARCHING;

            _opponentSearcherBehaviour.FadeIn();
        }


        // 待機画面を非表示 (戻る操作のために)してステージを遷移
        public void HideOpponentSearcherForBackAndChangeScene(string nextSceneName, EStageType nextStageType)
        {
            Assert.IsTrue(_gameStageState == EGameStageState.OPPONENT_SEARCHING);


            _nextSceneName = nextSceneName;

            _opponentSearcherBehaviour.FadeOutWithInform(isForGame: false);

            // 次のページと曲が異なるならば、曲をフェードする
            Locator.Instance.audioManager.FadeOutBGMIfNeeded(nextStageType: nextStageType, duration: Config.ANIMATION_DURATION_3);
        }


        // 待機画面を非表示 (対局に進むため)
        public void HideOpponentSearcherForGame()
        {
            Assert.IsTrue(_gameStageState == EGameStageState.OPPONENT_SEARCHING);


            _opponentSearcherBehaviour.FadeOutWithInform(isForGame: true);

            // チャイムを鳴らす
            Locator.Instance.audioManager.Play(SEType: ESEType.GAME_STAGE_OPPONENT_FOUND);
        }



        // ===============================================
        // MARK: - ● Enabled / Disabled Setting Button ─────

        public void SetIsButtonSettingEnabled(bool value)
        {
            _buttonSettingBehaviour.SetIsEnabled(value: value, willChangeButtonTexture: true);
        }



        // ===============================================
        // MARK: - ● Show Button ─────

        private void ShowButtonSetting()
        {
            _buttonSettingBehaviour.FadeIn();
        }


        private void ShowButtonResign()
        {
            _buttonResignBehaviour.FadeIn();
        }



        // ===============================================
        // MARK: - ● Setup Board Set ─────

        public void SetupBoardSet(in FGameStageInitializer gameStageInitializer)
        {
            var gameObjectBoardSet = new GameObject("BoardSet");
            gameObjectBoardSet.transform.parent = transform;
            _boardSetBehaviour = gameObjectBoardSet.AddComponent<BoardSetBehaviour>();
            _boardSetBehaviour.SetupComponentForAsGameStage(
                gameStageBehaviour: this,
                gameStageInitializer: gameStageInitializer,
                moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1);
        }



        // ===============================================
        // MARK: - ● Setup Player Info ─────

        private void SetupPlayerInfo(FGameStageInitializer stageInitializer)
        {
            SetupPlayerInfoPlayer2(stageInitializer: stageInitializer);
        }


        private void SetupPlayerInfoPlayer2(FGameStageInitializer stageInitializer)
        {
            Locator.Instance.statusBarManager.SetupGameObjectForPlayer2(
                parentGameObject: this.gameObject,
                playerName: stageInitializer.PlayerData2.PlayerName,
                moveOrderType: stageInitializer.PlayerData2.MoveOrderType,
                gradeRankType: stageInitializer.PlayerData2.GradeRankType,
                boardType: _boardType);
        }



        // ===============================================
        // MARK: - ● Status Bar ─────

        private void ChangeStatusBarModeToOnGameForPlayer1(string playerName, int rate, EGradeRankType gradeRankType, EMoveOrderType moveOrderType)
        {
            Locator.Instance.statusBarManager.SetPlayerNameForPlayer1(playerName: playerName);
            Locator.Instance.statusBarManager.SetRate(rate: rate);
            Locator.Instance.statusBarManager.SetGradeRankTypeForPlayer1(moveOrderType: moveOrderType, gradeRankType: gradeRankType, boardType: _boardType);
            Locator.Instance.statusBarManager.ChangeStatusBarModeForPlayer1(statusBarMode: StatusBarBehaviour.EStatusBarMode.ON_GAME);
        }

        private void ShowStatusBarPlayer2()
        {
            Locator.Instance.statusBarManager.Show(playerType: EPlayerType.PLAYER2);
        }



        // ===============================================
        // MARK: - ● Setup Button Setting ─────

        /// <summary>
        /// 設定ボタンのセットアップ
        /// - GameResult の Canvas を利用するため、GameResult のセットアップ後に呼び出すこと
        /// </summary>
        private void SetupButtonSetting()
        {
            var gameObjectButtonSetting = new GameObject(name: "ButtonSetting");
            _buttonSettingBehaviour = gameObjectButtonSetting.AddComponent<ButtonBehaviour>();

            gameObjectButtonSetting.transform.parent = transform;

            var buttonInitializer = new FButtonInitializer(
                targetCanvas: _gameResultBehaviour.Canvas,
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                area: _layouter.AreaSettingButton,
                tapJudgeType: ETapJudgeType.CLICK,
                isPlaySE: true,
                isEnabled: true,
                sortingOrder: Config.ON_GAME_BUTTON_SETTING__LAYERED_ORDERID.SortingOrder,
                action: () => { OnButtonSettingDown(); }); ;


            _buttonSettingBehaviour.SetupComponent(initializer: buttonInitializer);

            _buttonSettingBehaviour.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.SETTING),
                area: _layouter.AreaCompanionImageForSettingButton);

            ShowButtonSetting();
        }

        private void OnButtonSettingDown()
        {
            _buttonSettingBehaviour.SetIsEnabled(value: false, willChangeButtonTexture: true);

            bool isMuteMaster = AppManager.Instance.AudioController.IsMuteMaster();

            AppManager.Instance.UICreator.WindowTop.CheckBox.Add(
                name: "checkBoxPlayAudio",
                area: new FWindowElementArea(
                    origin: new Vector2(0.2f, 0.055f),
                    width: 0.09f, height: 0.09f),
                actionOnTap: () => { 
                        var isOn = AppManager.Instance.UICreator.WindowTop.CheckBox.IsOn(name: "checkBoxPlayAudio");
                        AppManager.Instance.AudioController.SetIsMuteMaster(value: !isOn);
                        _gameManager.OnTapPlayAudio(isOn: isOn);
                    },
                isOn: !isMuteMaster,
                hasError: false,
                isEnabled: true,
                companionTextFieldSetting: new FCompanionTextFieldSetting(
                    windowElementArea: new FWindowElementArea(
                        origin: new Vector2(0.325f, 0.05f),
                        endPoint: new Vector2(0.9f, 0.15f)),
                    text: "音を鳴らす",
                    fontType: EFontType.SERIF,
                    fontScale: 0.9f,
                    alignment: EAlignment.MIDDLE_LEFT));

            AppManager.Instance.UICreator.WindowTop.CheckBox.Add(
                name: "checkBoxShowNameP2",
                area: new FWindowElementArea(
                    origin: new Vector2(0.2f, 0.205f),
                    width: 0.09f, height: 0.09f),
                actionOnTap: () => { 
                    var isOn = AppManager.Instance.UICreator.WindowTop.CheckBox.IsOn(name: "checkBoxShowNameP2");
                    if (isOn) {
                        Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER2);
                    } else {
                        Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER2);
                    }
                    _gameManager.OnTapShowNamePlayer2(isOn: isOn);},
                isOn: Locator.Instance.statusBarManager.IsNameShown(playerType: EPlayerType.PLAYER2),
                hasError: false,
                isEnabled: true,
                companionTextFieldSetting: new FCompanionTextFieldSetting(
                    windowElementArea: new FWindowElementArea(
                        origin: new Vector2(0.325f, 0.2f),
                        endPoint: new Vector2(0.9f, 0.3f)),
                    text: "相手の名前を表示",
                    fontType: EFontType.SERIF,
                    fontScale: 0.9f,
                    alignment: EAlignment.MIDDLE_LEFT));

            AppManager.Instance.UICreator.WindowTop.CheckBox.Add(
                name: "checkBoxShowNameP1",
                area: new FWindowElementArea(
                    origin: new Vector2(0.2f, 0.355f),
                    width: 0.09f, height: 0.09f),
                actionOnTap: () => { 
                    var isOn = AppManager.Instance.UICreator.WindowTop.CheckBox.IsOn(name: "checkBoxShowNameP1");
                    if (isOn) {
                        Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER1);
                    } else {
                        Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER1);
                    }
                    _gameManager.OnTapShowNamePlayer1(isOn: isOn);},
                isOn: Locator.Instance.statusBarManager.IsNameShown(playerType: EPlayerType.PLAYER1),
                hasError: false,
                isEnabled: true,
                companionTextFieldSetting: new FCompanionTextFieldSetting(
                    windowElementArea: new FWindowElementArea(
                        origin: new Vector2(0.325f, 0.35f),
                        endPoint: new Vector2(0.9f, 0.45f)),
                    text: "自分の名前を表示",
                    fontType: EFontType.SERIF,
                    fontScale: 0.9f,
                    alignment: EAlignment.MIDDLE_LEFT));

            AppManager.Instance.UICreator.WindowTop.Button.Add(
                name: "buttonClose",
                area: new FWindowElementArea(
                    origin: new Vector2(0.0f, 0.01f),
                    width: 0.09f, height: 0.09f),
                buttonDesignType: EUIButtonDesignType.WINDOW_CLOSE,
                text: "",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    AppManager.Instance.UICreator.WindowTop.Hide(onDidHideAction: () => {
                        _buttonSettingBehaviour.SetIsEnabled(value: true, willChangeButtonTexture: true);
                    });
                });

            AppManager.Instance.UICreator.WindowTop.Show();
        }



        // ===============================================
        // MARK: - ● Setup Button Resign ─────

        private void SetupButtonResign()
        {
            var gameObjectButtonResign = new GameObject(name: "ButtonResign");
            _buttonResignBehaviour = gameObjectButtonResign.AddComponent<ButtonBehaviour>();

            gameObjectButtonResign.transform.parent = transform;

            var buttonInitializer = new FButtonInitializer(
                targetCanvas: _boardSetBehaviour.GetBoardSetInputCanvas(),
                buttonImageType: EButtonImageType.BUTTON_UNRESIZABLE_RESIGN,
                area: _layouter.AreaResignButton,
                tapJudgeType: ETapJudgeType.CLICK,
                isPlaySE: true,
                isEnabled: true,
                sortingOrder: Config.ON_GAME_BUTTON_RESIGN__LAYERED_ORDERID.SortingOrder,
                action: () => { OnButtonResignDown(); }); ;


            _buttonResignBehaviour.SetupComponent(initializer: buttonInitializer);


            var fontSize = (int)(_layouter.AreaResignButton.size.y * 0.55f);

            var buttonTextInitializer = new FButtonTextInitializer(
                text: "投了",
                fontType: EFontType.SERIF,
                fontSize: fontSize,
                alignment: EAlignment.MIDDLE_CENTER);
            _buttonResignBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);

            ShowButtonResign();

            if (_isAiGameButtonsShown)
            {
                SetupButtonsForAiGame();
            }
        }


        private void OnButtonResignDown()
        {
            _buttonResignBehaviour.SetIsEnabled(value: false, willChangeButtonTexture: true);

            _resignSelectorBehaviour.FadeIn();
        }


        // ヘッダーのボタン1つに収まる文字の大きさ(高さにも幅にも収まるようにする)。
        // 投了ボタン単独のとき(高さ×0.55)と同じくらいの大きさになるようにしている。
        private static int CalcFontSizeToFit(FArea area, string text)
        {
            return (int)Mathf.Min(area.size.y * 0.55f, area.size.x / (text.Length + 0.6f));
        }


        // AI対戦のヘッダーに並ぶ「戻る」「待った」「投了」の文字サイズ。
        // ボタンごとに幅も文字数も違うので、3つとも収まる一番小さい大きさに揃える。
        private int CalcFontSizeForAiGameHeaderButtons()
        {
            return Mathf.Min(
                CalcFontSizeToFit(area: _layouter.AreaBackButtonInAiGame, text: TextBackButtonInAiGame),
                Mathf.Min(
                    CalcFontSizeToFit(area: _layouter.AreaUndoButtonInAiGame, text: TextUndoButtonInAiGame),
                    CalcFontSizeToFit(area: _layouter.AreaResignButton, text: TextResignButton)));
        }


        private const string TextBackButtonInAiGame = "戻る";
        private const string TextUndoButtonInAiGame = "待った";
        private const string TextResignButton = "投了";


        // AI対戦で、投了ボタンの左に「戻る」「待った」ボタンを置く(相手の手番でも押せる)
        private void SetupButtonsForAiGame()
        {
            if (_buttonUndoBehaviour != null) { return; }

            var fontSize = CalcFontSizeForAiGameHeaderButtons();

            ButtonBehaviour Support(string name, string text, FArea area, Action action)
            {
                var gameObjectButton = new GameObject(name: name);
                var buttonBehaviour = gameObjectButton.AddComponent<ButtonBehaviour>();

                gameObjectButton.transform.parent = transform;

                buttonBehaviour.SetupComponent(initializer: new FButtonInitializer(
                    targetCanvas: _boardSetBehaviour.GetBoardSetInputCanvas(),
                    buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                    area: area,
                    tapJudgeType: ETapJudgeType.CLICK,
                    isPlaySE: true,
                    isEnabled: true,
                    sortingOrder: Config.ON_GAME_BUTTON_RESIGN__LAYERED_ORDERID.SortingOrder,
                    action: action));

                // 隣に並ぶ投了ボタンと文字の大きさを揃える
                buttonBehaviour.SetupText(buttonTextInitializer: new FButtonTextInitializer(
                    text: text,
                    fontType: EFontType.SERIF,
                    fontSize: fontSize,
                    alignment: EAlignment.MIDDLE_CENTER));

                buttonBehaviour.FadeIn();
                return buttonBehaviour;
            }

            _buttonBackBehaviour = Support(
                name: "ButtonBackInAiGame",
                text: TextBackButtonInAiGame,
                area: _layouter.AreaBackButtonInAiGame,
                action: () => { if (!IsAnySelectorShown()) { _gameManager.OnTapButtonBackInAiGame(); } });

            _buttonUndoBehaviour = Support(
                name: "ButtonUndoInAiGame",
                text: TextUndoButtonInAiGame,
                area: _layouter.AreaUndoButtonInAiGame,
                action: () => { if (!IsAnySelectorShown()) { _gameManager.OnTapButtonUndoInAiGame(); } });

            // 投了ボタンは単独では大きめの文字で作られているので、ここで同じ大きさに揃える
            _buttonResignBehaviour.SetFontSize(size: fontSize);

            // ヘッダーがボタンで埋まるので、エラーの吹き出しは盤の上側へ移す
            if (_textBalloonErrorBehaviour != null) {
                SetupTextBalloonError(area: _layouter.AreaTextBalloonErrorInAiGame);
            }
        }


        private void HideButtonsForAiGame()
        {
            _buttonUndoBehaviour?.Hide();
            _buttonBackBehaviour?.Hide();
        }



        // ===============================================
        // MARK: - ● Setup Promotion Selector ─────

        // for Promotion Selector
        private void SetupPromotionSelector()
        {
            var gameObjectPromotionSelector = new GameObject(name: "PromotionSelector");
            _promotionSelectorBehaviour = gameObjectPromotionSelector.AddComponent<PromotionSelectorBehaviour>();

            gameObjectPromotionSelector.transform.parent = transform;

            _promotionSelectorBehaviour.SetupComponent(gameStageBehaviour: this);
        }



        // ===============================================
        // MARK: - ● Setup Text Balloon ─────

        // 吹き出しを1つ作る。area の少し内側に収める
        private static TextBalloonBehaviour CreateTextBalloon(string name, FArea area, int sortingOrder)
        {
            var gameObject = new GameObject(name: name);
            var resultTextBalloonBehaviour = gameObject.AddComponent<TextBalloonBehaviour>();

            var balloonArea = new FArea(
                p1: area.CenterPoint,
                p2: new Vector2(x: area.size.x * 0.95f, y: area.size.y * 0.8f),
                areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);

            resultTextBalloonBehaviour.SetupComponent(area: balloonArea, sortingOrder: sortingOrder);

            return resultTextBalloonBehaviour;
        }


        public void SetupTextBalloon()
        {
            _textBalloonNormalBehaviour = CreateTextBalloon(
                name: "TextBalloonNormal",
                area: Locator.Instance.layoutManager.AreaGameStageBoardControlAndMessage,
                sortingOrder: Config.GAME_STAGE_TEXT_BALLOON__LAYERED_ORDERID.SortingOrder);

            // AI対戦のボタンを先に作っていたときは、そちらに合わせた場所に出す
            SetupTextBalloonError(area: _isAiGameButtonsShown
                ? _layouter.AreaTextBalloonErrorInAiGame
                : _layouter.AreaTextBalloonError);
        }


        private void SetupTextBalloonError(FArea area)
        {
            if (_textBalloonErrorBehaviour != null) {
                Destroy(_textBalloonErrorBehaviour.gameObject);
            }

            _textBalloonErrorBehaviour = CreateTextBalloon(
                name: "TextBalloonError",
                area: area,
                sortingOrder: Config.GAME_STAGE_TEXT_BALLOON__LAYERED_ORDERID.SortingOrder);

            _textBalloonErrorBehaviour.SetColor(color: new Vector3(x: 0.8f, y: 0.0f, z: 0.0f));
        }



        // ===============================================
        // MARK: - ● Setup ResignSelector ─────

        public void SetupResignSelector()
        {
            var gameObjectResignSelector = new GameObject(name: "ResignSelector");
            _resignSelectorBehaviour = gameObjectResignSelector.AddComponent<ResignSelectorBehaviour>();

            gameObjectResignSelector.transform.parent = transform;

            _resignSelectorBehaviour.SetupComponent(gameStageBehaviour: this);
        }



        // ===============================================
        // MARK: - ● Setup Game Result ─────

        public void SetupGameResult()
        {
            var gameObjectGameResult = new GameObject(name: "GameResult");
            _gameResultBehaviour = gameObjectGameResult.AddComponent<GameResultBehaviour>();

            gameObjectGameResult.transform.parent = transform;

            _gameResultBehaviour.SetupComponent(gameStageBehaviour: this);
        }


        // ===============================================
        // MARK: - ● Text Balloon ─────

        private TextBalloonBehaviour GetTextBalloon(ETextBalloonType textBalloonType)
        {
            return (textBalloonType) switch
            {
                ETextBalloonType.NORMAL => _textBalloonNormalBehaviour,
                ETextBalloonType.ERROR => _textBalloonErrorBehaviour,
                _ => throw new InvalidOperationException(),
            };
        }


        private void ShowTextBalloon(ETextBalloonType textBalloonType, string text)
        {
            var textBalloonBehaviour = GetTextBalloon(textBalloonType: textBalloonType);

            // ボードが表示される前に呼び出された場合には何もしない
            if (textBalloonBehaviour == null) { return; }

            textBalloonBehaviour.SetTextAndFadeInIfNeeded(text: text);
        }


        public void ShowTextBalloonNormal(string text)
        {
            ShowTextBalloon(textBalloonType: ETextBalloonType.NORMAL, text: text);
        }


        public void ShowTextBalloonError(string text)
        {
            ShowTextBalloon(textBalloonType: ETextBalloonType.ERROR, text: text);
        }


        private void HideTextBalloon(ETextBalloonType textBalloonType)
        {
            var textBalloonBehaviour = GetTextBalloon(textBalloonType: textBalloonType);

            // ボードが表示される前に呼び出された場合には何もしない
            if (textBalloonBehaviour == null) { return; }

            textBalloonBehaviour.FadeOut();
        }


        public void HideTextBalloonNormal()
        {
            HideTextBalloon(textBalloonType: ETextBalloonType.NORMAL);
        }


        public void HideTextBalloonError()
        {
            HideTextBalloon(textBalloonType: ETextBalloonType.ERROR);
        }



        // ===============================================
        // MARK: - ● Start Game ─────

        public void StartGame()
        {
            Assert.IsTrue(_gameStageState == EGameStageState.PRE_GAME);


            _gameStageState = EGameStageState.ON_GAME;
        }




        // ===============================================
        // MARK: - ● Show Promotion Selector ─────

        public void ShowPromotionSelector(PieceBehaviour promotionSelectingPieceBehaviour)
        {
            _promotionSelectingPieceBehaviour = promotionSelectingPieceBehaviour;
            _promotionSelectorBehaviour.FadeIn(pieceState: _promotionSelectingPieceBehaviour.PieceState);
        }



        // ===============================================
        // MARK: - ●  ─────

        public void OnDidSelectPromotion(bool isPromotionSelected)
        {
            void Support(PieceBehaviour pieceBehaviour, EPromotionResultType promotionResultType)
            {
                Assert.IsTrue(pieceBehaviour.PieceState.PlayerType == EPlayerType.PLAYER1);
                Assert.IsTrue(Utl.CanBeNari(pieceType: pieceBehaviour.PieceState.PieceType));

                var piece = Utl.ConvertPieceTypeFrameworkToPieceMain(
                    playerType: pieceBehaviour.PieceState.PlayerType,
                    moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1,
                    pieceType: pieceBehaviour.PieceState.PieceType,
                    isNari: true);

                var sq = pieceBehaviour.BoardCrd.ConvertToSquare(boardType: _boardType, moveOrderType: _moveOrderTypeOfPlayer1);

                _gameManager.OnDidSelectPromotion(piece: piece, sq: sq, promotionResultType: promotionResultType);
            }


            if (isPromotionSelected)
            {
                // pieceBehaviour.pieceState.ChangeToNari() を直接呼ばないこと
                _promotionSelectingPieceBehaviour.ChangeToNari();

                _promotionSelectingPieceBehaviour.RegisterAction(action: CAction.CreatePieceActionFlip(
                    pieceBehaviour: _promotionSelectingPieceBehaviour,
                    duration: 0f,
                    executionTimingAlpha: 0f));

                Support(pieceBehaviour: _promotionSelectingPieceBehaviour, promotionResultType: EPromotionResultType.PROMOTED);
            }
            else
            {
                Support(pieceBehaviour: _promotionSelectingPieceBehaviour, promotionResultType: EPromotionResultType.NOT_PROMOTED);
            }


            _promotionSelectingPieceBehaviour = null;
        }



        // ===============================================
        // MARK: - ● Check Promotion Selector / Resign Selector is Active ─────


        // 駒選択ウィンドウが表示されているか
        public bool IsPromotionSelectorShown()
        {
            return _promotionSelectingPieceBehaviour.IsShown;
        }


        // 投了確認ウィンドウが表示されているか
        public bool IsResignSelectorShown()
        {
            return _resignSelectorBehaviour.IsShown;
        }


        // 何らかの選択ウィンドウが表示されているか
        private bool IsAnySelectorShown()
        {
            return _promotionSelectorBehaviour.IsShown || _resignSelectorBehaviour.IsShown;
        }



        // ===============================================
        // MARK: - ● Pass Input To GameStage ─────

        public void OnTapBoard(FBoardCrd boardCrd)
        {
            // 何らかのセレクターウィンドウが開いている場合、タップを無効化
            if (IsAnySelectorShown()) { return; }

            Square square = boardCrd.ConvertToSquare(boardType: _boardType, moveOrderType: _moveOrderTypeOfPlayer1);
            _gameManager.OnTap(sq: square);
        }


        public void OnTapOnWaitingSpace(Piece piece)
        {
            // 何らかのセレクターウィンドウが開いている場合、タップを無効化
            if (IsAnySelectorShown()) { return; }

            _gameManager.OnTapOnWaitingSpace(piece: piece);
        }


        public void OnTapOutOfBoard()
        {
            // 何らかのセレクターウィンドウが開いている場合、タップを無効化
            if (IsAnySelectorShown()) { return; }

            _gameManager.OnTapOutOfBoard();
        }


        // 駒がタップされたことを通知
        public void OnTapPiece(EPlayerType playerType, EPieceType pieceType, bool isNari, FBoardCrd boardCrd)
        {
            // 何らかのセレクターウィンドウが開いている場合、タップを無効化
            if (IsAnySelectorShown()) { return; }

            Piece piece = Utl.ConvertPieceTypeFrameworkToPieceMain(
                playerType: playerType,
                moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1,
                pieceType: pieceType,
                isNari: isNari);

                Square square = boardCrd.ConvertToSquare(boardType: _boardType, moveOrderType: _moveOrderTypeOfPlayer1);
                _gameManager.OnTap(piece: piece, sq: square);
        }


        public void OnDidSelectResign(ESelectResultType selectResultType)
        {
            switch (selectResultType)
            {
                case ESelectResultType.CANSEL:
                    _buttonResignBehaviour.SetIsEnabled(value: true, willChangeButtonTexture: true);
                    break;

                case ESelectResultType.OK:
                    _gameManager.OnTapButtonResign();
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }



        // 対局結果表示で「検討」押下
        public void OnTapButtonHistoryDetail()
        {
            Locator.Instance.statusBarManager.ChangeStatusBarModeForPlayer1(statusBarMode: StatusBarBehaviour.EStatusBarMode.NORMAL);
            _gameManager.OnTapButtonHistoryDetail();
        }


        // 対局結果表示で「次の対局へ／友達ともう一度対局する」押下
        public void OnTapButtonNextGameOnGameResult()
        {
            Locator.Instance.statusBarManager.ChangeStatusBarModeForPlayer1(statusBarMode: StatusBarBehaviour.EStatusBarMode.NORMAL);
            _gameManager.OnTapButtonNextGameOnGameResult();
        }


        // 対局結果表示で「HOME」押下
        public void OnTapButtonHomeOnGameResult()
        {
            Locator.Instance.statusBarManager.ChangeStatusBarModeForPlayer1(statusBarMode: StatusBarBehaviour.EStatusBarMode.NORMAL);
            _gameManager.OnTapButtonHomeOnGameResult();
        }



        // ===============================================
        // MARK: - ● Show Game Result ─────

        public void SetNextGameButtonText(string text)
        {
            _nextGameButtonText = text;
        }

        public void SetHomeButtonText(string text)
        {
            _homeButtonText = text;
        }

        public void SetAiGameButtonsShown(bool value)
        {
            _isAiGameButtonsShown = value;

            // 投了ボタンを作った後に呼ばれたときは、ここで作る
            if (value && _buttonResignBehaviour != null)
            {
                SetupButtonsForAiGame();
            }
        }

        public void ShowGameResult(FGameResultData gameResultData)
        {
            Assert.IsTrue(_gameStageState == EGameStageState.ON_GAME);


            _gameStageState = EGameStageState.GAME_RESULT;

            // 対局が終わったら「待った」「戻る」は使えないので隠す(結果画面に「戻る」がある)
            HideButtonsForAiGame();

            _gameResultBehaviour.ShowFirst(gameMode: _gameMode, gameResultData: gameResultData);
        }



        // ===============================================
        // MARK: - ● Change Present Player ─────

        public void ChangePresentPlayer(EPlayerType playerType)
        {
            Locator.Instance.statusBarManager.ChangePresentPlayer(playerType: playerType);
            Locator.Instance.backgroundManager.ChangePlayerColor(playerType: playerType);

            switch (playerType)
            {
                case EPlayerType.PLAYER1:
                    _boardSetBehaviour.SetBoardSetInputEnabledIfNeeded(value: true);
                    _buttonResignBehaviour.SetIsEnabled(value: true, willChangeButtonTexture: true);
                    break;

                case EPlayerType.PLAYER2:
                    _boardSetBehaviour.SetBoardSetInputEnabledIfNeeded(value: false);
                    _buttonResignBehaviour.SetIsEnabled(value: false, willChangeButtonTexture: true);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ● Show Board Set ─────

        public void ShowBoardSet(
            in FPlayerData player1Data,
            in List<FPieceAndSquare> player1PieceList,
            in FPlayerData player2Data,
            in List<FPieceAndSquare> player2PieceList,
            EBoardType boardType
            )
        {
            _moveOrderTypeOfPlayer1 = Utl.ConvertColorToMoveOrderType(color: player1Data.Color);

            FGameStageInitializer gameStageInitializer = CreateGameStageInitializer(
                moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1,
                boardType: boardType,
                player1Data: player1Data,
                player1PieceList: player1PieceList,
                player2Data: player2Data,
                player2PieceList: player2PieceList);

            ShowBoardSetMain(player1Color: player1Data.Color, gameStageInitializer: gameStageInitializer);
        }

        private void ShowBoardSetMain(Color player1Color, in FGameStageInitializer gameStageInitializer)
        {
            Utl.SafeDestroy(behaviour: _opponentSearcherBehaviour);
            _opponentSearcherBehaviour = null;

            SetupPartsForGame(gameStageInitializer: gameStageInitializer);

            ShowStatusBarPlayer2();

            ChangeStatusBarModeToOnGameForPlayer1(
                playerName: gameStageInitializer.PlayerData1.PlayerName,
                rate: gameStageInitializer.PlayerData1.Rate,
                gradeRankType: gameStageInitializer.PlayerData1.GradeRankType,
                moveOrderType: gameStageInitializer.PlayerData1.MoveOrderType);

            // ゲーム開始
            // - 駒表示のアニメーション表示が完了したのちに呼び出すようにしたい
            OnDidShowBoardSet();
        }


        // フレームワーク内部でのセットアップ用に、GameStageInitializer を生成
        // - HistoryDetailStage でも利用されるため、static 関数として公開
        static public FGameStageInitializer CreateGameStageInitializer(
            EMoveOrderType moveOrderTypeOfPlayer1,
            EBoardType boardType,
            in FPlayerData player1Data,
            in List<FPieceAndSquare> player1PieceList,
            in FPlayerData player2Data,
            in List<FPieceAndSquare> player2PieceList)
        {
            Assert.IsTrue(player1Data.Color != player2Data.Color);

            FPlayerDataForFramework support(
                in FPlayerData playerData,
                in List<FPieceAndSquare> playerPieceList)
            {
                var piecePlaceDataListForPlayer = Utl.ConvertPieceTypeMainListToGFrameworkList(
                    moveOrderTypeOfPlayer1: moveOrderTypeOfPlayer1,
                    playerPieceList: playerPieceList,
                    boardType: boardType);

                return new FPlayerDataForFramework(
                    playerName: playerData.PlayerName,
                    rate: playerData.Rate,
                    gradeRankType: playerData.GradeRankType,
                    piecePlaceDataList: piecePlaceDataListForPlayer,
                    moveOrderType: Utl.ConvertColorToMoveOrderType(color: playerData.Color));
            }


            return new FGameStageInitializer(
                boardType: boardType,
                playerData1: support(playerData: player1Data, playerPieceList: player1PieceList),
                playerData2: support(playerData: player2Data, playerPieceList: player2PieceList));
        }

        // 盤セットが表示されたことを通知
        private void OnDidShowBoardSet()
        {
            _gameManager.OnDidShowBoardSet();
        }



        // 残り時刻の表示 (プレイヤー両者)
        public void SetTimeLeft(int minute, int second)
        {
            Locator.Instance.statusBarManager.SetTimeLeft(playerType: EPlayerType.PLAYER1, minute: minute, second: second);
            Locator.Instance.statusBarManager.SetTimeLeft(playerType: EPlayerType.PLAYER2, minute: minute, second: second);
        }


        // 残り時刻の代わりに「--:--」を表示 (プレイヤー両者。時間制限のないAI対戦)
        public void SetTimeLeftUnknown()
        {
            Locator.Instance.statusBarManager.SetTimeLeftText(playerType: EPlayerType.PLAYER1, text: "--:--");
            Locator.Instance.statusBarManager.SetTimeLeftText(playerType: EPlayerType.PLAYER2, text: "--:--");
        }


        // 残り時刻の表示 (プレイヤー指定)
        public void SetTimeLeft(Color color, int minute, int second)
        {
            var moveOrderType = Utl.ConvertColorToMoveOrderType(color: color);

            if (moveOrderType == _moveOrderTypeOfPlayer1)
            {
                // player 1
                Locator.Instance.statusBarManager.SetTimeLeft(playerType: EPlayerType.PLAYER1, minute: minute, second: second);
            }
            else
            {
                // player 2
                Locator.Instance.statusBarManager.SetTimeLeft(playerType: EPlayerType.PLAYER2, minute: minute, second: second);
            }
        }


        public void ChangePresentPlayer(Color color)
        {
            var moveOrderType = Utl.ConvertColorToMoveOrderType(color: color);

            if (moveOrderType == _moveOrderTypeOfPlayer1)
            {
                // player 1
                ChangePresentPlayer(playerType: EPlayerType.PLAYER1);
            }
            else
            {
                // player 2
                ChangePresentPlayer(playerType: EPlayerType.PLAYER2);
            }
        }


        // 対局結果を表示
        public void ShowGameResult(EGameResultType gameResultType, EGameResultReasonType gameResultReasonType, int consecutiveCount, int previousRate, int nextRate, EGradeRankType previousGradeRankType, EGradeRankType nextGradeRankType)
        {
            Assert.IsTrue(_gameStageState == EGameStageState.ON_GAME);
        
            var gameResultData = new FGameResultData(
                namePlayer1: Locator.Instance.statusBarManager.GetPlayerName(playerType: EPlayerType.PLAYER1),
                namePlayer2: Locator.Instance.statusBarManager.GetPlayerName(playerType: EPlayerType.PLAYER2),
                moveOrderTypePlayer1: Locator.Instance.statusBarManager.GetMoveOrderType(playerType: EPlayerType.PLAYER1),
                gameResultType: gameResultType,
                gameResultReasonType: gameResultReasonType,
                consecutiveCount: consecutiveCount,
                previousRate: previousRate,
                nextRate: nextRate,
                previousGradeRankType: previousGradeRankType,
                nextGradeRankType: nextGradeRankType);

            ShowGameResult(gameResultData: gameResultData);

            PlayJINOnResult(gameResultType: gameResultType);
        }


        private void PlayJINOnResult(EGameResultType gameResultType)
        {
            Locator.Instance.audioManager.FadeBGM(alphaTo: 0.0f, duration: 0.25f);


            EJINType JINType = gameResultType switch
            {
                EGameResultType.WIN => EJINType.WIN,
                EGameResultType.DRAW => EJINType.LOSE,
                EGameResultType.LOSE => EJINType.LOSE,
                _ => throw new InvalidOperationException(),
            };

            Locator.Instance.audioManager.Play(JINType: JINType);
        }


        // 王手を表示
        public void ShowCheck(Color color)
        {
            Debug.Log(color.ToString() + "王手");
        }


        public void HideBoardSetAndChangeScene(string nextSceneName, EStageType nextStageType)
        {
            // 次のページと曲が異なるならば、曲をフェードする
            Locator.Instance.audioManager.FadeOutBGMIfNeeded(nextStageType: nextStageType, duration: Config.ANIMATION_DURATION_3);

            // 背景の色を戻す
            Locator.Instance.backgroundManager.ChangePlayerColor(playerType: EPlayerType.PLAYER1);

            Locator.Instance.statusBarManager.Hide(playerType: EPlayerType.PLAYER2);
            Locator.Instance.statusBarManager.ChangeStatusBarModeForPlayer1(statusBarMode: StatusBarBehaviour.EStatusBarMode.NORMAL);
            Locator.Instance.statusBarManager.SetStatusBarForBehaviourPlayer2NullWhenCloseGameStage();

            _buttonSettingBehaviour.Hide();
            _buttonResignBehaviour.Hide();
            HideButtonsForAiGame();

            // 結果画面の部品は結果を表示したときに作られる。対局中に画面を移る (AI対戦の「待った」「戻る」) ときは触らない
            if (_gameStageState == EGameStageState.GAME_RESULT)
            {
                _gameResultBehaviour.FadeOut();
            }

            _boardSetBehaviour.FadeOut();

            _textBalloonNormalBehaviour.Hide();
            _textBalloonErrorBehaviour.Hide();

            var action = CAction.CreateActionFire(
                actionBehaviour: this,
                duration: Config.ANIMATION_DURATION_3,
                action: () => { AppManager.Instance.DestroyStageAndLoadScene(nextSceneName: nextSceneName); });

            RegisterAction(action: action);
        }



        // ===============================================
        // MARK: - ● Action for Piece ─────

        // 駒の選択
        public void SelectPiece(
            Color color,
            Square squareOfPiece,
            in List<Square> squareListOfPlaceTarget)
        {
            Assert.IsTrue(squareOfPiece != Square.SQ_Invalid);

            FBoardCrd boardCrd = new FBoardCrd(boardType: _boardType, square: squareOfPiece, moveOrderType: _moveOrderTypeOfPlayer1);
            List<FBoardCrd> boardCrdListOfPlaceTarget = Utl.ConvertSquareListToBoardCrdList(boardType: _boardType, squareList: squareListOfPlaceTarget, moveOrderType: _moveOrderTypeOfPlayer1);

            SelectPieceOnBoardMain(
                color: color,
                boardCrd: boardCrd,
                boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);
        }

        private void SelectPieceOnBoardMain(
            Color color,
            FBoardCrd boardCrd,
            in List<FBoardCrd> boardCrdListOfPlaceTarget)
        {
            EPlayerType playerType = ConvertColorToPlayerType(color: color);

            _boardSetBehaviour.SelectPiece(
                playerType: playerType,
                boardCrd: boardCrd,
                boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);
        }


        // 駒の選択
        public void SelectPiece(
            Color color,
            PieceType pieceTypeOnWaitingSpace,
            in List<Square> squareListOfPlaceTarget)
        {
            List<FBoardCrd> boardCrdListOfPlaceTarget = Utl.ConvertSquareListToBoardCrdList(boardType: _boardType, squareList: squareListOfPlaceTarget, moveOrderType: _moveOrderTypeOfPlayer1);

            SelectPieceOnWaitingSpaceMain(
                color: color,
                pieceTypeOnWaitingSpace: pieceTypeOnWaitingSpace,
                boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);
        }


        private void SelectPieceOnWaitingSpaceMain(
            Color color,
            PieceType pieceTypeOnWaitingSpace,
            in List<FBoardCrd> boardCrdListOfPlaceTarget)
        {
            Assert.IsTrue(pieceTypeOnWaitingSpace != PieceType.No_Piece);

            EPlayerType playerType = ConvertColorToPlayerType(color: color);
            var pieceInfoOnWaitingSpace = Utl.ConvertPieceTypeMainToGFrameWork(pieceTypeMain: pieceTypeOnWaitingSpace);

            _boardSetBehaviour.SelectPiece(
                playerType: playerType,
                pieceInfoOnWaitingSpace: pieceInfoOnWaitingSpace,
                boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);
        }


        // 選択状態の駒を非選択状態にする
        public void DeselectPiece()
        {
            _boardSetBehaviour.DeselectPiece();
        }



        // ===============================================
        // MARK: - ● Move Piece ─────

        // 盤上の駒の移動
        public void MovePiece(
            Color color,
            Square squareFrom,
            Square squareTo,
            EPromotionAfterMoveType promotionAfterMoveType)
        {
            EPlayerType playerType = ConvertColorToPlayerType(color: color);
            FBoardCrd boardCrdFrom = new FBoardCrd(boardType: _boardType, square: squareFrom, moveOrderType: _moveOrderTypeOfPlayer1);
            FBoardCrd boardCrdTo = new FBoardCrd(boardType: _boardType, square: squareTo, moveOrderType: _moveOrderTypeOfPlayer1);

            _boardSetBehaviour.MovePiece(
                playerType: playerType,
                boardCrdFrom: boardCrdFrom,
                boardCrdTo: boardCrdTo,
                promotionAfterMoveType: promotionAfterMoveType);
        }

        // 駒台の駒の移動
        public void MovePiece(
            Color color,
            PieceType pieceTypeOnWaitingSpace,
            Square squareTo)
        {
            EPlayerType playerType = ConvertColorToPlayerType(color: color);
            FBoardCrd boardCrdTo = new FBoardCrd(boardType: _boardType, square: squareTo, moveOrderType: _moveOrderTypeOfPlayer1);

            _boardSetBehaviour.MovePiece(
                playerType: playerType,
                pieceTypeOnWaitingSpace: pieceTypeOnWaitingSpace,
                boardCrdTo: boardCrdTo);
        }


        // ===============================================
        // MARK: - ● Utl Converter ─────

        private EPlayerType ConvertColorToPlayerType(Color color)
        {
            var moveOrderType = Utl.ConvertColorToMoveOrderType(color: color);

            if (moveOrderType == _moveOrderTypeOfPlayer1)
            {
                return EPlayerType.PLAYER1;
            }
            else
            {
                return EPlayerType.PLAYER2;
            }
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unity Editor での表示の場合停止タイミングで _layouter が null となるためチェックして破壊
            _layouter?.Destroy();
            _layouter = null;

            _gameManager = null;

            _boardSetBehaviour = null;

            _opponentSearcherBehaviour = null;

            _buttonResignBehaviour = null;
            _buttonUndoBehaviour = null;
            _buttonBackBehaviour = null;

            _textBalloonNormalBehaviour = null;
            _textBalloonErrorBehaviour = null;

            _promotionSelectorBehaviour = null;
            _promotionSelectingPieceBehaviour = null;
            _resignSelectorBehaviour = null;
            _gameResultBehaviour = null;
        }
    }
}
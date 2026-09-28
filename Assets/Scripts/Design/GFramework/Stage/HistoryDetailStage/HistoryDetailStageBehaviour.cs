using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


// TODO: 99将棋のときはAI想定手順を表示しないようにする。あるいはデータが渡されなかった場合
// TODO: AI将棋のときには、ステータスバーの名前を変更する。段位アイコンを変更する
// TODO: AI将棋の棋譜データを渡してもらようにする

namespace GFramework.Core
{
    public enum EKifuDataType
    {
        // KIF形式
        KIF,
        // KI2形式
        KI2,
        // CSA形式
        CSA
    }
}

// TODO: Board Set を初期では非表示にする。その後、表示して、OnDidAnimationを呼び出す。

// ムーブが完了してから、次の入力を受け付けるようにするのが望ましい。アニメーションをつけるかもしれないため。
namespace GFramework
{
    public class HistoryDetailStageBehaviour : ImageActionBehaviour, IHistoryDetailStage
    {
        // ===============================================
        // MARK: - ● Property ─────

        private IHistoryDetailManager _historyDetailManager;


        // -----------------------------------------------
        // MARK: - Game Object

        private Canvas _canvas;

        private EBoardType _boardType;

        private bool _isShown = false;
        public bool IsShown => _isShown;

        public override float Alpha => _textMoveNumBehaviour.Alpha;


        private HistoryDetailStageLayouter _layouter;


        private TextFieldBehaviour _textMoveNumBehaviour;
        private int _presentMoveNum = 0;
        private int _maxMoveNum = 0;


        private readonly List<ButtonBehaviour> _buttonBehaviourList = new List<ButtonBehaviour>();

        private ButtonBehaviour _buttonAiEvaluation;
        private ButtonBehaviour _buttonAiMatch;

        // AI評価値の吹き出し(初めて出すときに作る)
        private GameObject _gameObjectAiEvaluationBalloon;
        private SingleImageActionBehaviour _aiEvaluationBalloonBase;
        private TextFieldBehaviour _aiEvaluationBalloonText;
        private ButtonBehaviour _buttonReplayAiVariation;
        private bool _isAiEvaluationBalloonShown = false;
        public bool IsAiEvaluationBalloonShown => _isAiEvaluationBalloonShown;
        private ButtonBehaviour _buttonBackToActualPlay;
        private ButtonBehaviour _buttonBack;
        private ButtonBehaviour _buttonNext;
        private ButtonBehaviour _buttonPrevious;
        private ButtonBehaviour _buttonToFirst;
        private ButtonBehaviour _buttonToLast;



        // flag

        private bool _isSetupGameInfo = false;

        private bool _isSetupBoardSet = false;


        // Info

        private FHistoryItemInitializer _historyDetailSetting;

        // Board Set

        private BoardSetBehaviour _boardSetBehaviour = null;


        private EMoveOrderType _moveOrderTypeOfPlayer1 = default!;
        public EMoveOrderType MoveOrderTypeOfPlayer1 => _moveOrderTypeOfPlayer1;


        private CBoardDiffCalcurator _boardDiffCalcurator = new CBoardDiffCalcurator();

        private EKifuDataType _selectedKifuDataType = EKifuDataType.KIF;

        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(IHistoryDetailManager historyDetailManager, EBoardType boardType)
        {
            _historyDetailManager = historyDetailManager;
            _boardType = boardType;

            SetupLayout();

            SetupCanvas();

            SetupTextMoveNum();

            SetupButton();


            _textMoveNumBehaviour.Hide();
            // Button は非表示にしておく
            HideButton();

            Locator.Instance.audioManager.PlayDefaultStageBGM();
        }

        
        public void SetupLayout()
        {
            _layouter = new HistoryDetailStageLayouter();

            // _layouter.ShowLayoutArea();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.BASE_LAYERED_ORDERID_HISTORY_DETAIL_STAGE.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupTextMoveNum()
        {
            var gameObjectText = new GameObject(name: "TextMove");
            _textMoveNumBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            var text = "-- / --";
            FArea area = _layouter.AreaTextMove;

            var textInitializer = new FTextFieldInitializer(
                text: text,
                fontType: EFontType.BLOCK,
                fontSize: 80,
                alignment: EAlignment.MIDDLE_CENTER,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textMoveNumBehaviour.SetupComponent(textInitializer: textInitializer);
        }


        private void SetupButton()
        {
            ButtonBehaviour Support(string name, string buttonText, EButtonImageType buttonImageType, FArea buttonArea, Action action)
            {
                return CreateButton(
                    name: name,
                    buttonText: buttonText,
                    buttonImageType: buttonImageType,
                    buttonArea: buttonArea,
                    canvas: _canvas,
                    sortingOrder: Config.BASE_LAYERED_ORDERID_HISTORY_DETAIL_STAGE.SortingOrder,
                    fontSize: (int)(buttonArea.size.y * 0.5f),
                    action: action);
            }


            if (_boardType == EBoardType.BOARD_66)
            {
                // 押すと今の局面をAIが読み、評価値と読み筋を吹き出しで出す。読み筋の再生中は「本譜に戻る」になる
                _buttonAiEvaluation = Support(
                    name: "ButtonAiEvaluation",
                    buttonText: "AI評価値",
                    buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                    buttonArea: _layouter.AreaButtonReplayMode,
                    action: () => { _historyDetailManager.OnTapButtonAiEvaluation(); });
                _buttonBehaviourList.Add(_buttonAiEvaluation);

                // AIエンジンが6x6専用のため、66のときのみ
                _buttonAiMatch = Support(
                    name: "ButtonAiMatch",
                    buttonText: "AIと対戦",
                    buttonImageType: EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                    buttonArea: _layouter.AreaButtonAiMatch,
                    // 対戦できる局面か(以前のルールの局面でないか)はマネージャ側で判定してからウィンドウを出す
                    action: () => { _historyDetailManager.OnTapButtonAiMatch(); });
                _buttonBehaviourList.Add(_buttonAiMatch);
            }


            // 駒を動かしたり読み筋を再生したりしてできた変化から本譜に戻る(本譜を表示している間は押せない)
            _buttonBackToActualPlay = Support(
                name: "ButtonBackToActualPlay",
                buttonText: "本譜",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonInfo,
                action: () => { _historyDetailManager.OnTapButtonBackToActualPlay(); });
            _buttonBehaviourList.Add(_buttonBackToActualPlay);


            _buttonBack = Support(
                name: "ButtonBack",
                buttonText: "戻る",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonBack,
                action: () => { OnTapButtonBack(); });
            _buttonBehaviourList.Add(_buttonBack);


            _buttonNext = Support(
                name: "ButtonNext",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonNext,
                action: () => { OnTapButtonNext(); });
            _buttonBehaviourList.Add(_buttonNext);

            _buttonNext.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.NEXT),
                area: _layouter.AreaButtonNextCompanionImage);

            _buttonPrevious = Support(
                name: "ButtonPrevious",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonPrevious,
                action: () => { OnTapButtonPrevious(); });
            _buttonBehaviourList.Add(_buttonPrevious);

            _buttonPrevious.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.PREVIOUS),
                area: _layouter.AreaButtonPreviousCompanionImage);


            _buttonToFirst = Support(
                name: "ButtonToFirst",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonToFirst,
                action: () => { OnTapButtonToFirst(); });
            _buttonBehaviourList.Add(_buttonToFirst);

            _buttonToFirst.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.TO_FIRST),
                area: _layouter.AreaButtonToFirstCompanionImage);


            _buttonToLast = Support(
                name: "ButtonToLast",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonToLast,
                action: () => { OnTapButtonToLast(); });
            _buttonBehaviourList.Add(_buttonToLast);

            _buttonToLast.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.TO_LAST),
                area: _layouter.AreaButtonToLastCompanionImage);


            // ページコントロール関係のボタンは当初は無効にしておく
            SetButtonStepForwardDisabled();
            SetButtonStepBackDisabled();
            SetButtonJumpToFirstDisabled();
            SetButtonJumpToLastDisabled();
            SetButtonBackToActualPlayEnabled(value: false);
        }


        private ButtonBehaviour CreateButton(string name, string buttonText, EButtonImageType buttonImageType, FArea buttonArea, Canvas canvas, int sortingOrder, int fontSize, Action action)
        {
            GameObject gameObjectButton = new GameObject(name: name);
            var buttonBehaviour = gameObjectButton.AddComponent<ButtonBehaviour>();

            gameObjectButton.transform.parent = gameObject.transform;

            var buttonInitializer = new FButtonInitializer(
                targetCanvas: canvas,
                buttonImageType: buttonImageType,
                area: buttonArea,
                tapJudgeType: ETapJudgeType.CLICK,
                isPlaySE: true,
                isEnabled: true,
                sortingOrder: sortingOrder,
                action: action);

            buttonBehaviour.SetupComponent(initializer: buttonInitializer);


            var buttonTextInitializer = new FButtonTextInitializer(
                text: buttonText,
                fontType: EFontType.SERIF,
                fontSize: fontSize,
                alignment: EAlignment.MIDDLE_CENTER);
            buttonBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);

            return buttonBehaviour;
        }


        // ===============================================
        // MARK: - ● Change Move Num And Piece ─────

        public void ChangeMoveNumAndPiece(int moveNum, in List<FPieceAndSquare> player1PieceList, in List<FPieceAndSquare> player2PieceList)
        {
            // ハイライトされているセルがあれば、解除する
            ClearHighlightedLastMovedCellIfNeeded();

            // 一手進む場合は、次の手をハイライトする
            bool shouldHighlightCell = _presentMoveNum + 1 == moveNum;


            ChangeMoveNum(moveNum: moveNum);

            var diffResult = _boardDiffCalcurator.CalcurateDiffAndProcessForBoard(
                player1PieceList: player1PieceList,
                player2PieceList: player2PieceList);

            // Debug.Log("Diff: Board");
            // Debug.Log(diffResult.ResultBoard.AsString());
            // Debug.Log("Diff: Waiting Space Player 1");
            // Debug.Log(diffResult.ResultWaitingSpacePlayer1.AsString());
            // Debug.Log("Diff: Waiting Space Player 2");
            // Debug.Log(diffResult.ResultWaitingSpacePlayer2.AsString());

            _boardSetBehaviour.UpdateUsingDiffData(
                diffResultForBoard: diffResult.ResultBoard,
                diffResultForWaitingSpacePlayer1: diffResult.ResultWaitingSpacePlayer1,
                diffResultForWaitingSpacePlayer2: diffResult.ResultWaitingSpacePlayer2);

            if (shouldHighlightCell)
            {
                HighlightLastMovedCell(addList: diffResult.ResultBoard.AddList); 
            }

            // Debug.Log("Diff: Diff DebugPrint Again");
            // _boardDiffCalcurator.DebugPrint();

            return;
        }


        private void ChangeMoveNum(int moveNum)
        {
            Assert.IsTrue(IsValidMoveNum(moveNum: moveNum));
            _presentMoveNum = moveNum;


            var text = _presentMoveNum.ToString() + " / " + _maxMoveNum.ToString();
            _textMoveNumBehaviour.SetText(text: text);
        }


		private bool IsValidMoveNum(int moveNum) => (0 <= moveNum) && (moveNum <= _maxMoveNum);



		// ===============================================
		// MARK: - ● Cell Hightlight ─────

		private void HighlightLastMovedCell(List<FBoardDiffBoardDataElement> addList)
        {
            foreach (var i in addList)
            {
                _boardSetBehaviour.HighlightLastMovedCellForHistoryDetailStage(boardCrd: i.Crd);
            }
        }


		// セルのハイライトを解除
		private void ClearHighlightedLastMovedCellIfNeeded() => _boardSetBehaviour.ClearHighlightedLastMovedCellForHistoryDetailStage();


		// 何らかのセルがハイライトされているか
		private bool HasHighlightedLastMovedCell() => _boardSetBehaviour.HasHighlightedLastMovedCellForHistoryDetailStage();



		// ===============================================
		// MARK: - ● Text Move ─────

		private void UpdateTextMove()
        {
            var text = _presentMoveNum.ToString() + "/" + _maxMoveNum.ToString();
            _textMoveNumBehaviour.SetText(text: text);
        }



        // ===============================================
        // MARK: - ●  Setup ─────

        public void SetupGameInfo(in FHistoryItemInitializer historyDetailSetting)
        {
            Assert.IsTrue(_isSetupGameInfo == false, "対局情報はすでに設定されています");

            _isSetupGameInfo = true;

            _historyDetailSetting = historyDetailSetting;

            SetupOpponentInfo();
        }


        private void SetupOpponentInfo()
        {
            var playerMoveOrderType = Utl.ConvertColorToMoveOrderType(color: _historyDetailSetting.Color);
            var opponentMoveOrderType = (playerMoveOrderType == EMoveOrderType.FIRST) ? EMoveOrderType.SECOND : EMoveOrderType.FIRST;

            Locator.Instance.statusBarManager.SetupGameObjectForPlayer2(
            parentGameObject: this.gameObject,
            playerName: _historyDetailSetting.OpponentName,
            moveOrderType: opponentMoveOrderType,
            gradeRankType: _historyDetailSetting.OpponentGradeRankType,
            boardType: _boardType);
        }


        public void SetupBoardSet(
            int moveNum,
            EBoardType boardType,
            in FPlayerData player1Data,
            in List<FPieceAndSquare> player1PieceList,
            in FPlayerData player2Data,
            in List<FPieceAndSquare> player2PieceList)
        {
            Assert.IsTrue(_isSetupGameInfo, "先に対局情報を設定してください");
            _isSetupBoardSet = true;

            _moveOrderTypeOfPlayer1 = Utl.ConvertColorToMoveOrderType(color: player1Data.Color);

            var gameStageInitializer = GameStageBehaviour.CreateGameStageInitializer(
                moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1,
                boardType: boardType,
                player1Data: player1Data,
                player1PieceList: player1PieceList,
                player2Data: player2Data,
                player2PieceList: player2PieceList);

            // BoardDiffCalculator のセットアップ
            _boardDiffCalcurator.Setup(
                historyDetailStageBehaviour: this,
                gameStageInitializer: gameStageInitializer);

            // 盤・駒を生成する
            var gameObject = new GameObject("BoardSet");
            _boardSetBehaviour = gameObject.AddComponent<BoardSetBehaviour>();
            _boardSetBehaviour.SetupComponentForHistoryDetailStage(
                historyDetailStageBehaviour: this,
                gameStageInitializer: gameStageInitializer,
                moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1);
        }


        // ===============================================
        // MARK: - ● Change Status Bar As AI Variation ─────

        private void ChangeStatusBarForPlayer2AsAIVariation()
        {
            // moveOrderType: EMoveOrderType.FIRST は表示には関係がないため固定値
            Locator.Instance.statusBarManager.SetGradeRankTypeForPlayer2(moveOrderType: EMoveOrderType.FIRST, gradeRankType: EGradeRankType.AI, boardType: _boardType);
            Locator.Instance.statusBarManager.SetPlayerNameForPlayer2(playerName: "AI");
		}

        private void ChangeStatusBarForPlayer2AsActualPlay()
        {
            // moveOrderType: EMoveOrderType.FIRST は表示には関係がないため固定値
            Locator.Instance.statusBarManager.SetGradeRankTypeForPlayer2(moveOrderType: EMoveOrderType.FIRST, gradeRankType: _historyDetailSetting.OpponentGradeRankType, boardType: _boardType);
            Locator.Instance.statusBarManager.SetPlayerNameForPlayer2(playerName: _historyDetailSetting.OpponentName);
		}


		// ===============================================
		// MARK: - ● as IHistoryStage ─────

		public void SetDefaultSelectedKifuDataType(EKifuDataType kifuDataType)
        {
            Assert.IsTrue(AppManager.Instance.UICreator.WindowConfirmation.IsShown() == false, "棋譜データ選択ウィンドウが表示する前に設定する必要があります");
            
            _selectedKifuDataType = kifuDataType;
        }


        private void UpdateSetSelectedKifuDataType()
        {
            var index = AppManager.Instance.UICreator.WindowConfirmation.RadioButtonGroup.GetIndexOfOn(name: "radioButtonForKifuDataType");

            _selectedKifuDataType = index switch
            {
                0 => EKifuDataType.KIF,
                1 => EKifuDataType.KI2,
                2 => EKifuDataType.CSA,
                _ => throw new InvalidOperationException(),
            };

            Debug.Log(_selectedKifuDataType);
        }


        private int ConvertSelectedKifuDataTypeToRadioButtonIndex()
        {
            return _selectedKifuDataType switch
            {
                EKifuDataType.KIF => 0,
                EKifuDataType.KI2 => 1,
                EKifuDataType.CSA => 2,
                _ => throw new InvalidOperationException(),
            };
        }



        // ===============================================
        // MARK: - ● as IHistoryStage ─────

        public void SetPresentMoveNum(int value)
        {
            _presentMoveNum = value;

            UpdateTextMove();
        }


        public void SetMaxMoveNum(int value)
        {
            _maxMoveNum = value;

            UpdateTextMove();
        }


		public void SetButtonStepForwardEnabled() => _buttonNext.SetIsEnabled(value: true, willChangeButtonTexture: true);

		public void SetButtonStepForwardDisabled() => _buttonNext.SetIsEnabled(value: false, willChangeButtonTexture: true);


		public void SetButtonStepBackEnabled() => _buttonPrevious.SetIsEnabled(value: true, willChangeButtonTexture: true);

		public void SetButtonStepBackDisabled() => _buttonPrevious.SetIsEnabled(value: false, willChangeButtonTexture: true);

		public void SetButtonJumpToFirstEnabled() => _buttonToFirst.SetIsEnabled(value: true, willChangeButtonTexture: true);

		public void SetButtonJumpToFirstDisabled() => _buttonToFirst.SetIsEnabled(value: false, willChangeButtonTexture: true);


		public void SetButtonJumpToLastEnabled() => _buttonToLast.SetIsEnabled(value: true, willChangeButtonTexture: true);

		public void SetButtonJumpToLastDisabled() => _buttonToLast.SetIsEnabled(value: false, willChangeButtonTexture: true);



		// ===============================================
		// MARK: - ● History Manager への接続 ─────

		// 表示アニメーションが完了
		private void OnDidShowAnimation() => _historyDetailManager.OnDidShowAnimation();

		// 戻るボタンを押下
		private void OnTapButtonBack() => _historyDetailManager.OnTapButtonBack();

		// 次のページへボタンを押下
		private void OnTapButtonNext() => _historyDetailManager.OnTapButtonStepForward();

		// 前のページへボタンを押下
		private void OnTapButtonPrevious() => _historyDetailManager.OnTapButtonStepBack();

		// 最初ページへボタンを押下
		private void OnTapButtonToFirst() => _historyDetailManager.OnTapButtonJumpToFirst();

		// 最後のページへボタンを押下
		private void OnTapButtonToLast() => _historyDetailManager.OnTapButtonJumpToLast();


		// ===============================================
		// MARK: - ● Alpha ─────

		public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void ShowWithAnimation()
        {
            Assert.IsTrue(_isSetupGameInfo, "ゲーム情報が設定されていません");
            Assert.IsTrue(_isSetupBoardSet, "盤が設定されていません");


            FadeIn();

            // TODO: この数字を決めうちにしないこと
            var waitDuration = 0.5f;

            var action = CAction.CreateActionFire(actionBehaviour: this, duration: waitDuration, action: () => { OnDidShowAnimation(); });
            RegisterAction(action: action);
        }


        public override void Show()
        {
            Assert.IsTrue(_isShown == false);

            // 手数表示は一旦出さない (その位置に AIと対戦ボタンを置いている)
            ShowButton();
            Locator.Instance.statusBarManager.Show(playerType: EPlayerType.PLAYER2);

            _isShown = true;
        }


        private void ShowButton()
        {
            _buttonBehaviourList.ForEach(i => i.Show());
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown == false);

            // 手数表示は一旦出さない (その位置に AIと対戦ボタンを置いている)
            ShowButtonWithAnimation();
            Locator.Instance.statusBarManager.Show(playerType: EPlayerType.PLAYER2);

            _isShown = true;
        }


        private void ShowButtonWithAnimation()
        {
            _buttonBehaviourList.ForEach(i => i.FadeIn());

            Locator.Instance.statusBarManager.Show(playerType: EPlayerType.PLAYER2);
        }


        public void HideWithAnimationAndChangeScene(string nextSceneName, EStageType nextStageType)
        {
            Assert.IsTrue(_isShown);

            FadeOut();

            // 次のページと曲が異なるならば、曲をフェードする
            Locator.Instance.audioManager.FadeOutBGMIfNeeded(nextStageType: nextStageType, duration: Config.ANIMATION_DURATION_3);

            var action = CAction.CreateActionFire(
                actionBehaviour: this,
                duration: Config.ANIMATION_DURATION_3,
                action: () => { 
                    AppManager.Instance.DestroyStageAndLoadScene(nextSceneName: nextSceneName);
                 });

            RegisterAction(action: action);
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown);

            _buttonBehaviourList.ForEach(i => i.FadeOut());
            _boardSetBehaviour.FadeOut();
            FadeOutButton();
            HideAiEvaluationBalloon();
            Locator.Instance.statusBarManager.Hide(playerType: EPlayerType.PLAYER2);

            _isShown = false;
        }



        public override void Hide()
        {
            Assert.IsTrue(_isShown);

            HideButton();
            Locator.Instance.statusBarManager.Hide(playerType: EPlayerType.PLAYER2);

            _isShown = false;
        }


        private void HideButton()
        {
            _buttonBehaviourList.ForEach(i => i.Hide());
        }


        private void FadeOutButton()
        {
            _buttonBehaviourList.ForEach(i => i.FadeOut());
        }


        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unity Editor での表示の場合停止タイミングで _layouter が null となるためチェックして破壊
            _layouter?.Destroy();
            _layouter = null;

            _textMoveNumBehaviour = null;

            Utl.SafeDestroy(behaviourList: _buttonBehaviourList);
            _buttonBehaviourList.Clear();

            if (_gameObjectAiEvaluationBalloon != null)
            {
                Destroy(_gameObjectAiEvaluationBalloon);
            }
            _gameObjectAiEvaluationBalloon = null;
            _aiEvaluationBalloonBase = null;
            _aiEvaluationBalloonText = null;
            _buttonReplayAiVariation = null;

            _historyDetailManager = null;
            _canvas = null;

            _boardSetBehaviour = null;
        }



        // ===============================================
        // MARK: - ● Board Input ─────

        // ウィンドウを出している間や、画面を閉じている間は盤の操作を受け付けない
        private bool IsBoardInputBlocked()
        {
            return _isShown == false
                || AppManager.Instance.UICreator.WindowNormal.IsShown()
                || AppManager.Instance.UICreator.WindowConfirmation.IsShown();
        }


        public void OnTapBoard(FBoardCrd boardCrd)
        {
            if (IsBoardInputBlocked()) { return; }

            Square square = boardCrd.ConvertToSquare(boardType: _boardType, moveOrderType: _moveOrderTypeOfPlayer1);
            _historyDetailManager.OnTap(sq: square);
        }


        public void OnTapPiece(EPlayerType playerType, EPieceType pieceType, bool isNari, FBoardCrd boardCrd)
        {
            if (IsBoardInputBlocked()) { return; }

            Piece piece = Utl.ConvertPieceTypeFrameworkToPieceMain(
                playerType: playerType,
                moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1,
                pieceType: pieceType,
                isNari: isNari);

            Square square = boardCrd.ConvertToSquare(boardType: _boardType, moveOrderType: _moveOrderTypeOfPlayer1);
            _historyDetailManager.OnTap(piece: piece, sq: square);
        }


        public void OnTapWaitingSpace(EPlayerType playerType, int index)
        {
            if (IsBoardInputBlocked()) { return; }

            // 相手の駒台は吹き出しの下にあるので、吹き出しを出している間はタップで吹き出しを閉じるだけにする
            if (playerType == EPlayerType.PLAYER2 && _isAiEvaluationBalloonShown)
            {
                HideAiEvaluationBalloon();
                return;
            }

            Piece piece = Utl.ConvertPieceTypeFrameworkToPieceMain(
                playerType: playerType,
                moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1,
                pieceType: Utl.ConvertIndexOnWaitingSpaceToPieceType(index: index),
                isNari: false);

            _historyDetailManager.OnTapOnWaitingSpace(piece: piece);
        }


        public void OnTapOutOfBoard()
        {
            if (IsBoardInputBlocked()) { return; }

            _historyDetailManager.OnTapOutOfBoard();
        }


        public void SelectPiece(Color color, Square squareOfPiece, in List<Square> squareListOfPlaceTarget)
        {
            var boardCrd = new FBoardCrd(boardType: _boardType, square: squareOfPiece, moveOrderType: _moveOrderTypeOfPlayer1);
            var boardCrdListOfPlaceTarget = Utl.ConvertSquareListToBoardCrdList(boardType: _boardType, squareList: squareListOfPlaceTarget, moveOrderType: _moveOrderTypeOfPlayer1);

            _boardSetBehaviour.SelectPiece(
                playerType: ConvertColorToPlayerType(color: color),
                boardCrd: boardCrd,
                boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);
        }


        public void SelectPiece(Color color, PieceType pieceTypeOnWaitingSpace, in List<Square> squareListOfPlaceTarget)
        {
            var boardCrdListOfPlaceTarget = Utl.ConvertSquareListToBoardCrdList(boardType: _boardType, squareList: squareListOfPlaceTarget, moveOrderType: _moveOrderTypeOfPlayer1);

            _boardSetBehaviour.SelectPiece(
                playerType: ConvertColorToPlayerType(color: color),
                pieceInfoOnWaitingSpace: Utl.ConvertPieceTypeMainToGFrameWork(pieceTypeMain: pieceTypeOnWaitingSpace),
                boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);
        }


        public void DeselectPiece() => _boardSetBehaviour.DeselectPiece();


        private EPlayerType ConvertColorToPlayerType(Color color)
        {
            return Utl.ConvertColorToMoveOrderType(color: color) == _moveOrderTypeOfPlayer1 ? EPlayerType.PLAYER1 : EPlayerType.PLAYER2;
        }


        public void ShowWindowPromotion(Action<bool> onSelect)
        {
            var window = AppManager.Instance.UICreator.WindowConfirmation;

            window.TextField.Add(
                name: "textFieldTitleOnPromotionWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.1f),
                    endPoint: new Vector2(0.9f, 0.4f)),
                text: "成りますか？",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.MIDDLE_CENTER,
                actionOnTap: null);

            window.Button.Add(
                name: "buttonNotPromoteOnPromotionWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.5f),
                    endPoint: new Vector2(0.43f, 0.7f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "成らない",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    window.Hide(onDidHideAction: () => { onSelect(false); });
                });

            window.Button.Add(
                name: "buttonPromoteOnPromotionWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0.47f, 0.5f),
                    endPoint: new Vector2(0.9f, 0.7f)),
                buttonDesignType: EUIButtonDesignType.POSITIVE,
                text: "成る",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    window.Hide(onDidHideAction: () => { onSelect(true); });
                });

            window.Show();
        }


        // ===============================================
        // MARK: - ● AI Evaluation ─────

        public void SetButtonBackToActualPlayEnabled(bool value)
        {
            _buttonBackToActualPlay.SetIsEnabled(value: value, willChangeButtonTexture: true);
        }


        public void SetButtonAiEvaluationEnabled(bool value)
        {
            _buttonAiEvaluation?.SetIsEnabled(value: value, willChangeButtonTexture: true);
        }


        public void ShowAiEvaluationBalloon(string text, bool canReplay)
        {
            SetupAiEvaluationBalloonIfNeeded();

            _aiEvaluationBalloonText.SetText(text: text);
            _aiEvaluationBalloonBase.Show();
            _aiEvaluationBalloonText.Show();
            if (canReplay)
            {
                _buttonReplayAiVariation.Show();
            }
            else
            {
                _buttonReplayAiVariation.Hide();
            }

            _isAiEvaluationBalloonShown = true;
        }


        public void HideAiEvaluationBalloon()
        {
            _isAiEvaluationBalloonShown = false;

            if (_gameObjectAiEvaluationBalloon == null) { return; }

            _aiEvaluationBalloonBase.Hide();
            _aiEvaluationBalloonText.Hide();
            _buttonReplayAiVariation.Hide();
        }


        // 吹き出しは相手(Player2)の名前と持ち駒の上に重ねる。盤ができてから位置が決まるので、初めて出すときに作る
        private void SetupAiEvaluationBalloonIfNeeded()
        {
            if (_gameObjectAiEvaluationBalloon != null) { return; }

            // 相手のステータスバーと駒台を合わせた範囲。上下の向きによらないよう、両端の最小・最大から求める
            var statusBar = Locator.Instance.layoutManager.AreaStatusBarPlayer2;
            var waitingSpace = _boardSetBehaviour.GetAreaWaitingSpace(playerType: EPlayerType.PLAYER2);
            float minY = Mathf.Min(statusBar.CenterPoint.y - statusBar.size.y / 2f, waitingSpace.CenterPoint.y - waitingSpace.size.y / 2f);
            float maxY = Mathf.Max(statusBar.CenterPoint.y + statusBar.size.y / 2f, waitingSpace.CenterPoint.y + waitingSpace.size.y / 2f);
            var area = new FArea(
                p1: new Vector2(statusBar.CenterPoint.x, (minY + maxY) / 2f),
                p2: new Vector2(statusBar.size.x, maxY - minY),
                areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);

            // 盤・駒・ステータスバーより前、ウィンドウより後ろ。Config の値は Increment() で書き換わるので直接作る
            int sortingOrder = new FLayeredOrderID(layerType: ELayerType.HISTORY_DETAIL_STAGE, orderID: 10).SortingOrder;

            // 対局画面の吹き出し(TextBalloonBehaviour)と同じく、ルートは原点に置き、中身を画面中央からの位置で置く
            _gameObjectAiEvaluationBalloon = new GameObject(name: "AiEvaluationBalloon");

            _aiEvaluationBalloonBase = UtlSprite.CreateGameObjectWithResizableSpriteAndSingleImageBehaviour(
                name: "AiEvaluationBalloonBase",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePathOfTextBalloonBase(),
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: _gameObjectAiEvaluationBalloon.transform,
                border: new Vector4(x: 64f, y: 64f, z: 64f, w: 64f),
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f));
            _aiEvaluationBalloonBase.gameObject.GetComponent<SpriteRenderer>().size = Utl.ConvertImageSizeToUnit(imageSize: area.size);
            _aiEvaluationBalloonBase.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: area.CenterPoint);

            var canvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: _gameObjectAiEvaluationBalloon.transform,
                sortingOrder: sortingOrder + 1).GetComponent<Canvas>();

            // 左に評価値と読み筋(収まらなければ文字を小さくする)、右に「手順を再生」
            var textArea = area.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.05f, 0.1f), endPointRatio: new Vector2(0.72f, 0.9f));
            var gameObjectText = new GameObject(name: "AiEvaluationBalloonText");
            gameObjectText.transform.parent = _gameObjectAiEvaluationBalloon.transform;
            _aiEvaluationBalloonText = gameObjectText.AddComponent<TextFieldBehaviour>();
            _aiEvaluationBalloonText.SetupComponent(textInitializer: new FTextFieldInitializer(
                text: "",
                fontType: EFontType.SERIF,
                fontSize: (int)(textArea.size.y * 0.32f),
                alignment: EAlignment.MIDDLE_LEFT,
                area: textArea,
                lineSpacing: 1f,
                isOverFlowHorizontal: false,
                isOverFlowVertical: false,
                isResizeTextForBestFit: true,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: canvas));

            var buttonText = "手順を再生";
            var buttonArea = area.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.74f, 0.25f), endPointRatio: new Vector2(0.96f, 0.75f));
            _buttonReplayAiVariation = CreateButton(
                name: "ButtonReplayAiVariation",
                buttonText: buttonText,
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                buttonArea: buttonArea,
                canvas: canvas,
                sortingOrder: sortingOrder + 1,
                fontSize: (int)Mathf.Min(buttonArea.size.y * 0.5f, buttonArea.size.x / (buttonText.Length + 1)),
                action: () => { _historyDetailManager.OnTapButtonReplayAiVariation(); });

            _aiEvaluationBalloonBase.Hide();
            _aiEvaluationBalloonText.Hide();
            // ボタンは生成時点で非表示扱いだが、SetupText が文字の透明度を戻すので文字だけ見えている。
            // 一度表示してから隠し、状態と見た目をそろえる(そろえないと以後の Hide() が何もしない)
            _buttonReplayAiVariation.Show();
            _buttonReplayAiVariation.Hide();
        }


        // ===============================================
        // MARK: - ● Window AI Match ─────

        public void ShowWindowAiMatch()
        {
            AiMatchWindow.Show(
                note: "表示中の局面から対局します",
                onStart: (playerColor, level) => {
                    _historyDetailManager.OnTapButtonStartAiMatch(playerColor: playerColor, level: level);
                });
        }


        // ===============================================
        // MARK: - ● Window Info ─────

        private void ShowWindowInfo()
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


            void Support(
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


            Support(
                name: "textFieldBoardType",
                lineIndex: 0,
                textSubheading: "番：",
                textContent: _historyDetailSetting.BoardType.ConvertToString());

            Support(
                name: "textFieldPlayerBlack",
                lineIndex: 1,
                textSubheading: "先手：",
                textContent: _historyDetailSetting.PlayerBlackName);

            Support(
                name: "textFieldPlayerWhite",
                lineIndex: 2,
                textSubheading: "後手：",
                textContent: _historyDetailSetting.PlayerWhiteName);

            Support(
                name: "textFieldResult",
                lineIndex: 3,
                textSubheading: "結果：",
                textContent: GetWinLoseResult());

            Support(
                name: "textFieldReason",
                lineIndex: 4,
                textSubheading: "決まり手：",
                textContent: GetWinLoseReasonDetail());

            string rateString = "";
            if (_historyDetailSetting.PreviousRate == _historyDetailSetting.NextRate)
            {
                rateString = _historyDetailSetting.PreviousRate.ToString() + " (±0)";
            }
            else
            {
                var deltaRate = _historyDetailSetting.GetDeltaRate();
                var deltaRateString = deltaRate > 0 ? "+" + deltaRate.ToString() : deltaRate.ToString();

                rateString = _historyDetailSetting.PreviousRate.ToString() + " → " + _historyDetailSetting.NextRate.ToString() + " (" + deltaRateString + ")";
            }

            Support(
                name: "textFieldRate",
                lineIndex: 5,
                textSubheading: "レート：",
                textContent: rateString);

            string rankGradeSting = "";

            if (_historyDetailSetting.PreviousGradeRankType == _historyDetailSetting.NextGradeRankType)
            {
                rankGradeSting = _historyDetailSetting.PreviousGradeRankType.ToJapaneseName();
            }
            else
            {
                rankGradeSting = _historyDetailSetting.PreviousGradeRankType.ToJapaneseName() + " → " + _historyDetailSetting.NextGradeRankType.ToJapaneseName();
            }

            Support(
                name: "textFieldGradeRank",
                lineIndex: 6,
                textSubheading: "段位：",
                textContent: rankGradeSting);

            if (_historyDetailSetting.CanExportData)
            {
                AppManager.Instance.UICreator.WindowNormal.Button.Add(
                    name: "buttonExport",
                    area: new FWindowElementArea(
                        origin: new Vector2(0f, 1.225f),
                        endPoint: new Vector2(1f, 1.4f)),
                    buttonDesignType: EUIButtonDesignType.DEFAULT,
                    text: "棋譜データ出力",
                    isPlaySE: true,
                    isEnabled: true,
                    actionOnTap: () => {
                        ShowDataExportWindow();
                    });
            }


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
			return _historyDetailSetting.HistoryItemResult switch
			{
				EHistoryItemResult.WIN => "勝ち",
				EHistoryItemResult.LOSE => "負け",
				EHistoryItemResult.DRAW => "引き分け",
				EHistoryItemResult.UNKNOWN => "不明",
				_ => throw new InvalidOperationException(),
			};
		}


        private String GetWinLoseReasonDetail()
        {
            EGameResultType gameResultType;

            switch (_historyDetailSetting.HistoryItemResult)
            {
                case EHistoryItemResult.WIN:
                    gameResultType = EGameResultType.WIN;
                    break;
                case EHistoryItemResult.LOSE:
                    gameResultType = EGameResultType.LOSE;
                    break;
                case EHistoryItemResult.DRAW:
                    gameResultType = EGameResultType.DRAW;
                    break;
                case EHistoryItemResult.UNKNOWN:
                    return "";
                default:
                    throw new InvalidOperationException();
            }

            return _historyDetailSetting.GameResultReasonType.ToString(gameResultType: gameResultType);
        }


        private void ShowDataExportWindow()
        {
            AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
                name: "textFieldTitleOnDataExportWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.0f),
                    endPoint: new Vector2(0.9f, 0.15f)),
                text: "棋譜データ出力",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_CENTER,
                actionOnTap: null);

            AppManager.Instance.UICreator.WindowConfirmation.RadioButtonGroup.Add(
                name: "radioButtonForKifuDataType",
                areaList: new List<FWindowElementArea> {
                        new(origin: new Vector2(0.05f, 0.15f),  endPoint: new Vector2(0.175f, 0.275f)),
                        new(origin: new Vector2(0.05f, 0.3f),   endPoint: new Vector2(0.175f, 0.425f)),
                        new(origin: new Vector2(0.05f, 0.45f),  endPoint: new Vector2(0.175f, 0.575f)),
                        },
                companionTextFieldSettingList: new List<FCompanionTextFieldSetting> {
                        new(
                            windowElementArea: new FWindowElementArea(origin: new Vector2(0.21f, 0.15f), endPoint: new Vector2(1.0f, 0.27f)),
                            text: "KIF形式",
                            fontType: EFontType.SERIF,
                            fontScale: 1.0f,
                            alignment: EAlignment.MIDDLE_LEFT),
                        new(
                            windowElementArea: new FWindowElementArea(origin: new Vector2(0.21f, 0.3f), endPoint: new Vector2(1.0f, 0.42f)),
                            text: "KI2形式",
                            fontType: EFontType.SERIF,
                            fontScale: 1.0f,
                            alignment: EAlignment.MIDDLE_LEFT),
                        new(
                            windowElementArea: new FWindowElementArea(origin: new Vector2(0.21f, 0.45f), endPoint: new Vector2(1.0f, 0.57f)),
                            text: "CSA形式",
                            fontType: EFontType.SERIF,
                            fontScale: 1.0f,
                            alignment: EAlignment.MIDDLE_LEFT),
                },
                indexOfOn: ConvertSelectedKifuDataTypeToRadioButtonIndex(),
                actionOnTap: () => { UpdateSetSelectedKifuDataType(); },
                isEnabled: true);

            AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
                name: "buttonCloseOnDataExportWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.625f),
                    endPoint: new Vector2(0.39f, 0.8f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "閉じる",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: null);
                });

            AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
                name: "buttonExportOnDataExportWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0.43f, 0.625f),
                    endPoint: new Vector2(0.9f, 0.8f)),
                buttonDesignType: EUIButtonDesignType.POSITIVE,
                text: "保存する",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    _historyDetailManager.OnTapExportData(kifuDataType: _selectedKifuDataType);
                    AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: null);
                });

            AppManager.Instance.UICreator.WindowConfirmation.Show();
        }
    }
}


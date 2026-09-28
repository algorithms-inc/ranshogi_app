using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;
using System.Threading;

// TODO: GameStage と HistoryDetail では配置のYが異なるだけ（縦画面なら）
namespace GFramework
{
    public class BoardSetBehaviour : ImageActionBehaviour
    {
        public enum EStageType
        {
            GAME_STAGE,
            HISTORY_DETAIL_STAGE
        }


        // ステージタイプ
        private EStageType _stageType = default!;

        public bool IsGameStage => _stageType == EStageType.GAME_STAGE;
        public bool IsHistoryDetailStage => _stageType == EStageType.HISTORY_DETAIL_STAGE;

        // 盤の種類
        private EBoardType _boardType = default!;
        public EBoardType BoardType => _boardType;

        // アニメーションの時間
        private readonly float _durationPieceSelect = 0.1f;
        private readonly float _durationPieceMove = 0.2f;

        // ボードセットの設定
        private FBoardSetSetting _boardSetSetting = default!;

        // 親となるBehaviour
        private GameStageBehaviour _gameStageBehaviour = null;
        private HistoryDetailStageBehaviour _historyDetailStageBehaviour = null;

        // プレイヤーの番手
        private EMoveOrderType _moveOrderTypeOfPlayer1 = default!;
        public EMoveOrderType MoveOrderTypeOfPlayer1 => _moveOrderTypeOfPlayer1;

        // 玉将のプレイヤー
        private EPlayerType _gyokushouPlayer = default!;
        public EPlayerType GyokushouPlayer => _gyokushouPlayer;


        // 関連する Local Manager

        private BoardImageSetManager _boardImageSetManager = null;
        public BoardImageSetManager BoardImageSetManager => _boardImageSetManager;

        private PieceImageSetManager _pieceImageSetManager = null;
        public PieceImageSetManager PieceImageSetManager => _pieceImageSetManager;

        private BoardSetInputManager _boardSetInputManager = null;

        // 最終移動セル
        private FBoardCrd _lastMovedBoardCrd = new FBoardCrd(x: 1, y: 1); // 便宜上の初期値。動作に問題はない

        // 子として保有するBehaviour

        // 駒のID管理
        private CIndexManager _pieceIDManager = new CIndexManager();
    
        // 駒
        private List<PieceBehaviour> _pieceBehaviourList = new List<PieceBehaviour>();

        // 盤
        private BoardBehaviour _boardBehaviour;
        public BoardBehaviour BoardBehaviour => _boardBehaviour;

        // 駒台
        private WaitingSpaceBehaviour _waitingSpaceBehaviourPlayer1;
        private WaitingSpaceBehaviour _waitingSpaceBehaviourPlayer2;


        private BoardSetLayouter _layouter = null;

        // Interface (ImageActionBehaviour)
        public override float Alpha => _boardBehaviour.Alpha;

        private bool _isShown = true;
        public bool IsShown => _isShown;



        // for History Detail Stage

        private List<FBoardCrd> _highlightedBoardCrdListForHistoryDetailStage = new List<FBoardCrd>();


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponentForAsGameStage(
            GameStageBehaviour gameStageBehaviour,
            FGameStageInitializer gameStageInitializer,
            EMoveOrderType moveOrderTypeOfPlayer1)
        {
            Assert.IsTrue(_gameStageBehaviour == null);
            Assert.IsTrue(_historyDetailStageBehaviour == null);

            _stageType = EStageType.GAME_STAGE;
            _boardType = gameStageInitializer.BoardType;

            _gameStageBehaviour = gameStageBehaviour;
            _moveOrderTypeOfPlayer1 = moveOrderTypeOfPlayer1;
            _gyokushouPlayer = gameStageInitializer.GyokushouPlayer;


            CommonSetup(gameStageInitializer: gameStageInitializer);

            // CommonSetup() の後で呼び出すこと
            SetupBoardSetInputManager();
        }


        public void SetupComponentForHistoryDetailStage(
            HistoryDetailStageBehaviour historyDetailStageBehaviour,
            FGameStageInitializer gameStageInitializer,
            EMoveOrderType moveOrderTypeOfPlayer1)
        {
            Assert.IsTrue(_gameStageBehaviour == null);
            Assert.IsTrue(_historyDetailStageBehaviour == null);

            _stageType = EStageType.HISTORY_DETAIL_STAGE;
            _boardType = gameStageInitializer.BoardType;

            _historyDetailStageBehaviour = historyDetailStageBehaviour;
            _moveOrderTypeOfPlayer1 = moveOrderTypeOfPlayer1;


            CommonSetup(gameStageInitializer: gameStageInitializer);

            // 検討画面でも駒を動かせるようにする(動かせるかの判定は HistoryDetailViewController が行う)
            _boardSetInputManager = new BoardSetInputManager(boardSetBehaviour: this);
            _boardSetInputManager.SetupInputForHistoryDetail();
        }


        private void CommonSetup(in FGameStageInitializer gameStageInitializer)
        {
            // これ必要？
            transform.position = new Vector3(0f, 0f, 0f);

            SetupBoardSetSetting();

            SetupLayout();

            SetupBoardImageSetManager();
            SetupPieceImageSetManager();

            SetupBoard();

            ShowBoard();

            SetupWaitingSpace();

            ShowWaitingSpace();

            CreatePieceForPrepareBoardSet(stageInitializer: gameStageInitializer);

            ShowCountOnWaitingSpaceOnGameBegin();
        }


        private void SetupBoardSetSetting()
        {
            _boardSetSetting = new FBoardSetSetting(
                boardDesignType: 0,              // 盤のデザイン       現在 0 のみ
                pieceDesignType: 0,              // 駒のデザイン       現在 0 のみ
                pieceFrontType: AppManager.Instance.DesignController.GetDesignIDOfPiceFront(),
                pieceBackTypeForRookGroup: AppManager.Instance.DesignController.GetDesignIDOfPieceBackForGroupRook(),
                pieceBackTypeForSilverGroup: AppManager.Instance.DesignController.GetDesignIDOfPieceBackForGroupSilver(),
                pieceBackTypeForPawn: 0);        // 駒裏タイプ(歩)     現在 0「と」のみ
        }


        private void SetupLayout()
        {
            _layouter = new BoardSetLayouter(boardType: _boardType);
        }


        private void SetupBoardSetInputManager()
        {
            _boardSetInputManager = new BoardSetInputManager(boardSetBehaviour: this);

            // 生成時に一気に実行してはだめ？？
            _boardSetInputManager.SetupInputForGame();
        }


        // 盤関連画像セットマネジャのセットアップ
        private void SetupBoardImageSetManager()
        {
            _boardImageSetManager = new BoardImageSetManager();
            _boardImageSetManager.SetBoardImageSetSetting(boardImageSetSetting: _boardSetSetting.boardImageSetSetting);
        }


        // 駒画像セットマネジャのセットアップ
        private void SetupPieceImageSetManager()
        {
            _pieceImageSetManager = new PieceImageSetManager();
            _pieceImageSetManager.SetPieceImageSetSetting(pieceImageSetSetting: _boardSetSetting.pieceImageSetSetting);
        }


        private void SetupBoard()
        {
            var gameObjectBoard = new GameObject("Board");
            gameObjectBoard.transform.parent = transform;
            _boardBehaviour = gameObjectBoard.AddComponent<BoardBehaviour>();
            _boardBehaviour.SetupComponent(boardType: _boardType, boardSetBahavirour: this);
        }


        private void SetupWaitingSpace()
        {
            // TODO: 状況に応じて出し分けること
            EWaitingSpaceType waitingSpaceType = EWaitingSpaceType.HORIZONTAL;

            // for Player1
            var gameObjectWaitingSpacePlayer1 = new GameObject("WaitingSpacePlayer1");
            gameObjectWaitingSpacePlayer1.transform.parent = transform;
            _waitingSpaceBehaviourPlayer1 = gameObjectWaitingSpacePlayer1.AddComponent<WaitingSpaceBehaviour>();
            _waitingSpaceBehaviourPlayer1.SetupComponent(
                boardSetBehaviour: this,
                waitingSpaceType: waitingSpaceType,
                playerType: EPlayerType.PLAYER1);

            // for Player2
            var gameObjectWaitingSpacePlayer2 = new GameObject("WaitingSpacePlayer2");
            gameObjectWaitingSpacePlayer2.transform.parent = transform;
            _waitingSpaceBehaviourPlayer2 = gameObjectWaitingSpacePlayer2.AddComponent<WaitingSpaceBehaviour>();
            _waitingSpaceBehaviourPlayer2.SetupComponent(
                boardSetBehaviour: this,
                waitingSpaceType: waitingSpaceType,
                playerType: EPlayerType.PLAYER2);
        }



        private void CreatePieceForPrepareBoardSet(in FGameStageInitializer stageInitializer)
        {
            CreatePieceForSetup(
                playerType: EPlayerType.PLAYER1,
                isGyokushouPlayer: _gyokushouPlayer == EPlayerType.PLAYER1,
                PiecePlaceDataList: stageInitializer.PlayerData1.PiecePlaceDataList);

            CreatePieceForSetup(
                playerType: EPlayerType.PLAYER2,
                isGyokushouPlayer: _gyokushouPlayer == EPlayerType.PLAYER2,
                PiecePlaceDataList: stageInitializer.PlayerData2.PiecePlaceDataList);
        }


        // 最初の駒配置用
        // HistoryDetailStageにおいて、駒の生成を適時行うときには別関数を利用すること
        private void CreatePieceForSetup(EPlayerType playerType, bool isGyokushouPlayer, in List<FPiecePlaceData> PiecePlaceDataList)
        {
            var coutListOfPieceOnWaitingSpace = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            
            foreach (FPiecePlaceData i in PiecePlaceDataList)
            {
                var newPieceBehaviour = CreateGameObjectForPiece(playerType: playerType, isGyokushouPlayer: isGyokushouPlayer, i);
                
                // 駒台の数値はこの時点では取得できないので、手前でカウントする
                if (i.BoardCrd.IsOnBoard(boardType: _boardType))
                {
                    // case: OnBoard
                    newPieceBehaviour.FadeIn();
                }
                else
                {
                    // case: OnWaitingSpace

                    int indexOnWaitingSpace = Utl.ConvertPieceTypeToIndexOnWaitingSpace(i.PieceType);
                    coutListOfPieceOnWaitingSpace[indexOnWaitingSpace] += 1;

                    if (coutListOfPieceOnWaitingSpace[indexOnWaitingSpace] == 1)
                    {
                        newPieceBehaviour.FadeIn();
                    } 
                }
            }
        }


        private PieceBehaviour CreateGameObjectForPiece(EPlayerType playerType, bool isGyokushouPlayer, FPiecePlaceData i)
        {
            FPieceState pieceState = new FPieceState(
                playerType: playerType,
                pieceType: i.PieceType,
                isGyokushou: isGyokushouPlayer && (i.PieceType == EPieceType.OUSHOU),
                isNari: i.IsNari);

            var pieceID = _pieceIDManager.GenerateNextIndex();

            GameObject newGameObjectPiece = new GameObject("Piece" + pieceID + "_" + pieceState.PieceType.ToString());

            if (i.BoardCrd.IsOnBoard(boardType: _boardType))
            {
                newGameObjectPiece.transform.parent = _boardBehaviour.gameObject.transform;
            }
            else
            {
                // case = OnWaitingSpace
                newGameObjectPiece.transform.parent = GetWaitingSpaceBehaviour(playerType: playerType).transform;
            }


            PieceBehaviour newPieceBehaviour = newGameObjectPiece.AddComponent<PieceBehaviour>();

            newPieceBehaviour.SetupComponent(pieceID: pieceID, boardSetBehaviour: this, pieceState: pieceState, boardCrd: i.BoardCrd);

            _pieceBehaviourList.Add(newPieceBehaviour);

            newPieceBehaviour.TransformAccordingToBoardCrd(boardCrd: i.BoardCrd);

            return newPieceBehaviour;
        }


        private void ShowCountOnWaitingSpaceOnGameBegin()
        {
            int[] countListPlayer1 = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            int[] countListPlayer2 = new int[7] { 0, 0, 0, 0, 0, 0, 0 };

            foreach (var i in _pieceBehaviourList)
            {
                if (i.BoardCrd.IsOnWaitingSpace(boardType: _boardType))
                {
                    var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: i.PieceState.PieceType);

                    switch (i.PieceState.PlayerType)
                    {
                        case EPlayerType.PLAYER1:
                            countListPlayer1[index]++;
                            break;
                        case EPlayerType.PLAYER2:
                            countListPlayer2[index]++;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            _waitingSpaceBehaviourPlayer1.ShowCountOnGameBegin(countList: countListPlayer1);
            _waitingSpaceBehaviourPlayer2.ShowCountOnGameBegin(countList: countListPlayer2);
        }



        // ===============================================
        // MARK: - ● Get Area ─────

        // 外に出す or それぞれに埋め込む
        public FArea GetAreaWaitingSpace(EPlayerType playerType)
        {
            return _layouter.GetAreaWaitingSpace(playerType: playerType);
        }

        // 外に出す or それぞれに埋め込む
        public FArea GetAreaBoard()
        {
            return _layouter.GetAreaBoard();
        }

        // 外に出す or それぞれに埋め込む
        public FArea GetAreaBoardCell(in FBoardCrd boardCrd)
        {
            return _layouter.GetAreaBoardCell(boardType: _boardType, boardCrd: boardCrd);
        }

        // 外に出す or それぞれに埋め込む
        public FArea GetAreaWaitingSpaceCell(EPlayerType playerType, int index)
        {
            return _layouter.GetAreaWaitingSpaceCell(playerType: playerType, index: index);
        }


        // 外に出す
        public FArea GetAreaPieceCount(EPlayerType playerType, int index)
        {
            return _layouter.GetAreaPieceCount(playerType: playerType, index: index);
        }




        public FArea GetAreaSelectorOnGameStage()
        {
            return _layouter.GetAreaSelectorOnGameStage();
        }



        // ===============================================
        // MARK: - ● Get Image Scale ─────

        public Vector2 GetPieceImageScaleFromOriginalToOnBoard()
        {
            switch (_boardType)
            {
                case EBoardType.BOARD_66:
                    return Config.BOARD_BODY__PIECE__SIZE_66 / Config.PIECE_BODY__IMAGE_SIZE;

                case EBoardType.BOARD_99:
                    return Config.BOARD_BODY__PIECE__SIZE_99 / Config.PIECE_BODY__IMAGE_SIZE;

                default:
                    throw new InvalidOperationException();
            }
        }


        public Vector2 GetPieceImageScaleFromOriginalToOnWaitingSpace()
        {
            return Locator.Instance.layoutManager.GetWaitingSpaceType() switch
            {
                EWaitingSpaceType.HORIZONTAL => Config.WAITING_SPACE_HORIZONTAL__PIECE_SIZE / Config.PIECE_BODY__IMAGE_SIZE,

                EWaitingSpaceType.SQUARE => Config.WAITING_SPACE_SQUARE__PIECE_SIZE / Config.PIECE_BODY__IMAGE_SIZE,

                _ => throw new InvalidOperationException(),
            };
        }


        public Vector2 GetPieceCountImageScaleFromOriginalToOnBoard()
        {
            return Locator.Instance.layoutManager.GetWaitingSpaceType() switch
            {
                EWaitingSpaceType.HORIZONTAL => Config.WAITING_SPACE_HORIZONTAL__PIECE_COUNT__SIZE / Config.PIECE_COUNT__IMAGE_SIZE,

                EWaitingSpaceType.SQUARE => Config.WAITING_SPACE_SQUARE__PIECE_COUNT__SIZE / Config.PIECE_COUNT__IMAGE_SIZE,

                _ => throw new InvalidOperationException(),
            };
        }


        public Vector2 GetCellImageScaleFromOriginalToOnBoard()
        {
            switch (_boardType)
            {
                case EBoardType.BOARD_66:
                    return Config.BOARD__CELL__SIZE_66 / Config.CELL__IMAGE_SIZE;

                case EBoardType.BOARD_99:
                    return Config.BOARD__CELL__SIZE_99 / Config.CELL__IMAGE_SIZE;

                default:
                    throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ●  ─────

        private void ShowBoard()
        {
            _boardBehaviour.FadeIn();
        }


        private void ShowWaitingSpace()
        {
            _waitingSpaceBehaviourPlayer1.FadeIn();
            _waitingSpaceBehaviourPlayer2.FadeIn();
        }



        // ===============================================
        // MARK: - ●  ─────

        public BoardBehaviour GetBoardBehaviour()
        {
            return _boardBehaviour;
        }



        public WaitingSpaceBehaviour GetWaitingSpaceBehaviour(EPlayerType playerType)
        {
            return (playerType) switch
            {
                EPlayerType.PLAYER1 => _waitingSpaceBehaviourPlayer1,
                EPlayerType.PLAYER2 => _waitingSpaceBehaviourPlayer2,
                _ => throw new InvalidOperationException(),
            };
        }


        // ===============================================
        // MARK: - ●  ─────

        public Canvas GetBoardSetInputCanvas()
        {
            return _boardSetInputManager.Canvas;
        }

        public void SetBoardSetInputEnabledIfNeeded(bool value)
        {
            _boardSetInputManager.SetIsEnabledIfNeeded(value: value);
        }




        // ===============================================
        // MARK: - ●  ─────

        private bool IsAnyPiece(EPlayerType playerType, FBoardCrd boardCrd)
        {
            return GetPieceBehaviourOnBoard(playerType: playerType, boardCrd: boardCrd) != null;
        }



        /// 駒を取る場合など、同じマスに２つの駒が存在することがありうるため、Playerを指定する
        private PieceBehaviour GetPieceBehaviourOnBoard(EPlayerType playerType, FBoardCrd boardCrd)
        {
            Assert.IsTrue(boardCrd.IsOnBoard(boardType: _boardType));

            foreach (PieceBehaviour i in _pieceBehaviourList)
            {
                if (i.BoardCrd == boardCrd && i.PieceState.PlayerType == playerType)
                {
                    return i;
                }
            }

            // 指定の座標に指定プレイヤーの駒がなかった場合
            return null;
        }


        /// 特定のプレイヤーを優先して、指定のマスに存在する駒の数を取得する
        private PieceBehaviour GetPieceBehaviourOnBoardPrioritizingPlayer(EPlayerType playerType, FBoardCrd boardCrd)
        {
            Assert.IsTrue(boardCrd.IsOnBoard(boardType: _boardType));

            var list = new List<PieceBehaviour>();

            foreach (PieceBehaviour i in _pieceBehaviourList)
            {
                if (i.BoardCrd == boardCrd)
                {
                    list.Add(i);
                }
            }


            if (list.Count == 0) { return null; }

            if (list.Count == 1) { return list[0]; }

            if (list.Count >= 3) { throw new InvalidOperationException(); }

            // case list.Count == 2

            if (list[0].PieceState.PlayerType == list[1].PieceState.PlayerType) { throw new InvalidOperationException(); }

            if (list[0].PieceState.PlayerType == playerType)
            {
                return list[0];
            }
            else
            {
                return list[1];
            }
        }

        // 
        private PieceBehaviour GetPieceBihaviourOnWaitingSpace(EPlayerType playerType, EPieceType pieceType)
        {
            Assert.IsTrue(Utl.CanBeOnWaitingSpace(pieceType: pieceType));

            foreach (PieceBehaviour i in _pieceBehaviourList)
            {
                if (i.BoardCrd.IsOnWaitingSpace(boardType: _boardType) &&
                    i.PieceState.PlayerType == playerType &&
                    i.PieceState.PieceType == pieceType &&
                    i.IsShown == true)
                {
                    return i;
                }
            }

            // 駒台に指定の駒がなかった場合
            return null;
        }


        public PieceBehaviour GetNextHiddenPieceBehaviourOnWaitingSpace(EPlayerType playerType, EPieceType pieceType)
        {
            Assert.IsTrue(Utl.CanBeOnWaitingSpace(pieceType: pieceType));

            foreach (PieceBehaviour i in _pieceBehaviourList)
            {
                if (i.BoardCrd.IsOnWaitingSpace(boardType: _boardType) &&
                    i.PieceState.PlayerType == playerType &&
                    i.PieceState.PieceType == pieceType &&
                    i.IsShown == false)
                {
                    return i;
                }
            }

            // 駒台に指定の駒がなかった場合
            return null;
        }


        // ===============================================
        // MARK: - ● Action for Piece ─────

        // 盤上の駒を選択
        private void ActionSelectPiece(EPlayerType playerType, FBoardCrd boarCrd)
        {
            PieceBehaviour targetPieceBehaviour = GetPieceBehaviourOnBoard(playerType: playerType, boardCrd: boarCrd);

            Assert.IsTrue(targetPieceBehaviour != null);
            Assert.IsTrue(targetPieceBehaviour.SelectType == PieceBehaviour.ESelectType.UNSELECTED);

            targetPieceBehaviour.RegisterAction(action: CAction.CreatePieceActionSelect(
                pieceBehaviour: targetPieceBehaviour,
                duration: _durationPieceSelect));
        }


        // 盤上の駒で選択されているものを自分で探して、非選択にする
        private void ActionDeselectPiece()
        {
            foreach (var i in _pieceBehaviourList)
            {
                if (i.SelectType == PieceBehaviour.ESelectType.SELECTED)
                {
                    i.RegisterAction(action: CAction.CreatePieceActionDeselect(
                    pieceBehaviour: i,
                    duration: _durationPieceSelect));
                }
            }
        }


        // 持ち駒台の駒を選択
        private void ActionSelectPiece(EPlayerType playerType, EPieceType pieceType)
        {
            PieceBehaviour targetPieceBehaviour = GetPieceBihaviourOnWaitingSpace(playerType: playerType, pieceType: pieceType);

            // 駒台に表示中の駒が見つからないときは、選択の見た目を出さないだけにする
            if (targetPieceBehaviour == null) { return; }
            Assert.IsTrue(targetPieceBehaviour.SelectType == PieceBehaviour.ESelectType.UNSELECTED);

            targetPieceBehaviour.RegisterAction(action: CAction.CreatePieceActionSelect(
                pieceBehaviour: targetPieceBehaviour,
                duration: _durationPieceSelect));
        }



        // 盤上の駒を盤上で動かす
        // TODO: Board に任せたほうがいいかもしれない
        private void ActionMovePiece(
            EPlayerType playerType,
            FBoardCrd boarCrdFrom,
            FBoardCrd boardCrdTo,
            EPromotionAfterMoveType promotionAfterMoveType)
        {
            PieceBehaviour targetPieceBehaviour = GetPieceBehaviourOnBoard(playerType: playerType, boardCrd: boarCrdFrom);
            Assert.IsTrue(targetPieceBehaviour != null);

            // 同じプレイヤーの駒がないことを確認
            Assert.IsTrue(IsAnyPiece(playerType: playerType, boardCrd: boardCrdTo) == false);

            // 横持ちの場合のレイアウト、駒台処理をする
            // All Action Forced End を実装する
            // TODO をまとめる
            // まずは画面再描画をアクション途中でもできるようにする
            // Waitはつくる必要ある？
            //   RegisterAction(action: Action.CreateActionWait(actionBehaviour: this, duration: 0.5f));

            targetPieceBehaviour.MoveBoardCrdOnBoard(newBoardCrd: boardCrdTo);

            targetPieceBehaviour.RegisterAction(action: CAction.CreatePieceActionMove(
                pieceBehaviour: targetPieceBehaviour, duration: _durationPieceMove,
                boardCrd: boardCrdTo));

            switch (promotionAfterMoveType)
            {
                case EPromotionAfterMoveType.NOT_PROMOTE:
                    break;

                case EPromotionAfterMoveType.PROMOTE:
                    Assert.IsTrue(Utl.CanBeNari(pieceType: targetPieceBehaviour.PieceState.PieceType));
                    Assert.IsTrue(targetPieceBehaviour.PieceState.IsNari == false);

                    // pieceBehaviour.pieceState.ChangeToNari() を直接呼ばないこと
                    targetPieceBehaviour.ChangeToNari();

                    targetPieceBehaviour.RegisterAction(action: CAction.CreatePieceActionFlip(
                        pieceBehaviour: targetPieceBehaviour,
                        duration: 0f,
                        executionTimingAlpha: 0f));

                    break;

                case EPromotionAfterMoveType.ASK_PROMOTE:
                    Assert.IsTrue(Utl.CanBeNari(pieceType: targetPieceBehaviour.PieceState.PieceType));
                    Assert.IsTrue(targetPieceBehaviour.PieceState.IsNari == false);

                    _gameStageBehaviour.ShowPromotionSelector(promotionSelectingPieceBehaviour: targetPieceBehaviour);

                    break;


                default:
                    throw new InvalidOperationException();
            }



            // もし相手の駒がある場合、移動

            var oppositePlayerType = Utl.ConvertToOppositePlayerType(playerType: playerType);

            if (IsAnyPiece(playerType: oppositePlayerType, boardCrd: boardCrdTo))
            {
                var pieceBehaviour = GetPieceBehaviourOnBoard(playerType: oppositePlayerType, boardCrd: boardCrdTo);

                pieceBehaviour.ChangePlayerTypeAndMoveToWaitingSpace();
            }
        }


        // ===============================================
        // MARK: - ●  ─────


        // 駒台の駒を盤上に動かす
        // TODO: WatingSpace に任せたほうがいいかもしれない
        private void ActionMovePiece(EPlayerType playerType, EPieceType pieceType, FBoardCrd boardCrdTo)
        {
            PieceBehaviour targetPieceBehaviour = GetPieceBihaviourOnWaitingSpace(playerType: playerType, pieceType: pieceType);
            Assert.IsTrue(targetPieceBehaviour != null);

            Assert.IsTrue(IsAnyPiece(playerType: EPlayerType.PLAYER1, boardCrd: boardCrdTo) == false);

            // TODO:
            // 対象のマスに駒があればそいつの属性をかえて駒台に移動
            // 持ち駒台自身の移動について、ちゃんと歩なら歩のマス目に移動させる
            // --
            // Gitのソースコードを確認する
            // 横持ちの場合のレイアウト、駒台処理をする
            // All Action Forced End を実装する
            // TODO をまとめる
            // まずは画面再描画をアクション途中でもできるようにする
            // Waitはつくる必要ある？
            //   RegisterAction(action: Action.CreateActionWait(actionBehaviour: this, duration: 0.5f));

            targetPieceBehaviour.ChangeParentFromWaitingSpaceToBoard(boardCrd: boardCrdTo);

            targetPieceBehaviour.RegisterAction(action: CAction.CreatePieceActionMove(
                pieceBehaviour: targetPieceBehaviour, duration: _durationPieceMove,
                boardCrd: boardCrdTo));

            // 駒台に複数の駒がある場合、次の駒を表示し、カウンタを変更する
            WaitingSpaceBehaviour waitingSpaceBehaviour = GetWaitingSpaceBehaviour(playerType: playerType);

            _ = waitingSpaceBehaviour.ShowNextHiddenPieceAndDecrementCountIfNeeded(pieceType: pieceType);
        }


        private void ActionChangeCellDisplayToPlaceTarget(List<FBoardCrd> boardCrdList)
        {
            _boardBehaviour.ChangeCellDisplayToPlaceTarget(boardCrdList: boardCrdList);
        }

        private void ActionChangeCellDisplayToDefaultAll()
        {
            _boardBehaviour.ChangePlaceTargetCellDisplayToDefault();
        }

        private void ActionChangeCellDisplayFromLastMovedToDefaultIfNeeded()
        {
            _boardBehaviour.ChangeCellDisplayFromLastMovedToDefaultIfNeeded(boardCrd: _lastMovedBoardCrd);
        }

        private void ActionChangeCellDisplayToLastMoved(FBoardCrd boardCrd)
        {
            _boardBehaviour.ChangeCellDisplayToLastMoved(boardCrd: boardCrd);
        }

        private void ActionChangeCellDisplayFromLastMovedToDefaultForHistoryDetailStage(FBoardCrd boardCrd)
        {
            _boardBehaviour.ChangeCellDisplayFromLastMovedToDefaultIfNeeded(boardCrd: boardCrd);
        }


        // ===============================================
        // MARK: - ●  ─────

        private bool IsAnyPieceBehaviourOnWaitingSpacePlayer1(int index)
        {
            var pieceType = Utl.ConvertIndexOnWaitingSpaceToPieceType(index: index);

            foreach (PieceBehaviour i in _pieceBehaviourList)
            {
                if ((i.PieceState.PlayerType == EPlayerType.PLAYER1)
                    && (i.PieceState.PieceType == pieceType)
                    && i.BoardCrd.IsOnWaitingSpace(boardType: _boardType))
                {
                    return true;
                }
            }

            return false;
        }


        // ===============================================
        // MARK: - ●  ─────

        public void OnTapBoard(FBoardCrd boardCrd)
        {
            /// TODO: 1ここで指定の座標にコマがいるかどうか、分岐する
            /// TODO: 2WaitingSpace のコマについても対応すること     

            var pieceBehaviour = GetPieceBehaviourOnBoardPrioritizingPlayer(playerType: EPlayerType.PLAYER1, boardCrd: boardCrd);

            if (IsHistoryDetailStage)
            {
                if (pieceBehaviour == null)
                {
                    _historyDetailStageBehaviour.OnTapBoard(boardCrd: boardCrd);
                }
                else
                {
                    _historyDetailStageBehaviour.OnTapPiece(
                        playerType: pieceBehaviour.PieceState.PlayerType,
                        pieceType: pieceBehaviour.PieceState.PieceType,
                        isNari: pieceBehaviour.PieceState.IsNari,
                        boardCrd: boardCrd);
                }
                return;
            }

            if (pieceBehaviour == null)
            {
                _gameStageBehaviour.OnTapBoard(boardCrd: boardCrd);
            }
            else
            {
                _gameStageBehaviour.OnTapPiece(
                    playerType: pieceBehaviour.PieceState.PlayerType,
                    pieceType: pieceBehaviour.PieceState.PieceType,
                    isNari: pieceBehaviour.PieceState.IsNari,
                    boardCrd: boardCrd);
            }
        }


        // 駒台をタップ(Player2 の駒台の入力は検討画面にだけある)
        public void OnTapWaitingSpace(EPlayerType playerType, int index)
        {
            if (IsHistoryDetailStage)
            {
                // 持ち駒があるかは局面を持っている側で判定する
                _historyDetailStageBehaviour.OnTapWaitingSpace(playerType: playerType, index: index);
                return;
            }

            if (playerType == EPlayerType.PLAYER1)
            {
                OnTapWaitingSpace(index: index);
            }
        }


        public void OnTapWaitingSpace(int index)
        {
            if (IsHistoryDetailStage)
            {
                OnTapWaitingSpace(playerType: EPlayerType.PLAYER1, index: index);
                return;
            }

            if (IsAnyPieceBehaviourOnWaitingSpacePlayer1(index: index))
            {
                EPieceType pieceType = Utl.ConvertIndexOnWaitingSpaceToPieceType(index: index);

                Piece piece = Utl.ConvertPieceTypeFrameworkToPieceMain(
                    playerType: EPlayerType.PLAYER1,
                    moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1,
                    pieceType: pieceType,
                    isNari: false);

                _gameStageBehaviour.OnTapOnWaitingSpace(piece: piece);
            }
            else
            {
                // 駒台に指定の駒がなかった場合、盤外をタップしたときと同じ動作をする
                OnTapOutOfBoard();
            }
        }


        public void OnTapOutOfBoard()
        {
            if (IsHistoryDetailStage)
            {
                _historyDetailStageBehaviour.OnTapOutOfBoard();
                return;
            }

            _gameStageBehaviour.OnTapOutOfBoard();
        }



        // ===============================================
        // MARK: - ● Action for Piece ─────

        // 駒の選択(盤上)
        public void SelectPiece(
            EPlayerType playerType,
            FBoardCrd boardCrd,
            in List<FBoardCrd> boardCrdListOfPlaceTarget)
        {
            // 駒の選択
            ActionSelectPiece(playerType: playerType, boarCrd: boardCrd);

            // 最終移動セルの表示を戻す
            ActionChangeCellDisplayFromLastMovedToDefaultIfNeededWhenSelectPiece(boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);

            // セルの表示を変える
            ActionChangeCellDisplayToPlaceTarget(boardCrdList: boardCrdListOfPlaceTarget);
        }


        private void ActionChangeCellDisplayFromLastMovedToDefaultIfNeededWhenSelectPiece(in List<FBoardCrd> boardCrdListOfPlaceTarget)
        {
            // 設定が OFF になっている場合、何もしない
            if (AppManager.Instance.DesignController.IsShowLastMove() == false) { return; }

            // 選択表示マスと、最終移動マスが同じ場合、アニメーションが続くと見ためがおかしいため、何もしない
            if (boardCrdListOfPlaceTarget.Contains(_lastMovedBoardCrd)) { return; }

            ActionChangeCellDisplayFromLastMovedToDefaultIfNeeded();
        }


        public void SelectPiece(
            EPlayerType playerType,
            (EPieceType pieceType, bool isNari) pieceInfoOnWaitingSpace,
            in List<FBoardCrd> boardCrdListOfPlaceTarget)
        {
            Assert.IsTrue(pieceInfoOnWaitingSpace.isNari != true);

            // TODO: 駒を持っているか、ASSERTを設置する

            ActionSelectPiece(playerType: playerType, pieceType: pieceInfoOnWaitingSpace.pieceType);

            // 最終移動セルの表示を戻す
            ActionChangeCellDisplayFromLastMovedToDefaultIfNeededWhenSelectPiece(boardCrdListOfPlaceTarget: boardCrdListOfPlaceTarget);

            // セルの表示を変える
            ActionChangeCellDisplayToPlaceTarget(boardCrdList: boardCrdListOfPlaceTarget);
        }



        // 選択状態の駒を非選択状態にする
        public void DeselectPiece()
        {
            // 選択状態の駒を非選択状態にする
            ActionDeselectPiece();

            // 選択状態のセルを元にもどす
            ActionChangeCellDisplayToDefaultAll();
        }


        // 盤上の駒の移動
        public void MovePiece(
            EPlayerType playerType,
            FBoardCrd boardCrdFrom,
            FBoardCrd boardCrdTo,
            EPromotionAfterMoveType promotionAfterMoveType)
        {
            ActionMovePiece(playerType: playerType, boarCrdFrom: boardCrdFrom, boardCrdTo: boardCrdTo, promotionAfterMoveType: promotionAfterMoveType);

            // セルの表示を変える
            ActionChangeCellDisplayToDefaultAll();

            // 最終移動セルの表示の更新
            ChangeLastMovedCellDisplayIfNeeded(playerType: playerType, boardCrdTo: boardCrdTo);

            // 最終移動セルをアップデート
            UpdateLastMovedBoardCrd(boardCrdTo: boardCrdTo);
        }


        // 駒台の駒の移動
        public void MovePiece(
            EPlayerType playerType,
            PieceType pieceTypeOnWaitingSpace,
            FBoardCrd boardCrdTo)
        {
            var pieceInfo = Utl.ConvertPieceTypeMainToGFrameWork(pieceTypeMain: pieceTypeOnWaitingSpace);
            Assert.IsTrue(pieceInfo.isNari != true);

            ActionMovePiece(playerType: playerType, pieceType: pieceInfo.pieceType, boardCrdTo: boardCrdTo);

            // セルの表示を変える
            ActionChangeCellDisplayToDefaultAll();

            // 最終移動セルの表示の更新
            ChangeLastMovedCellDisplayIfNeeded(playerType: playerType, boardCrdTo: boardCrdTo);

            // 最終移動セルをアップデート
            UpdateLastMovedBoardCrd(boardCrdTo: boardCrdTo);
        }


        private void ChangeLastMovedCellDisplayIfNeeded(
            EPlayerType playerType,
            FBoardCrd boardCrdTo)
        {
            // 設定でOFFになっていれば何もしない
            if (AppManager.Instance.DesignController.IsShowLastMove() == false) { return; }

            // 最終移動セルの表示を戻す
            ActionChangeCellDisplayFromLastMovedToDefaultIfNeeded();

            // Player1の場合は何もしない
            var isNeeded = playerType == EPlayerType.PLAYER2;
            if (isNeeded)
            {
                // セルの表示を最終移動に変える
                ActionChangeCellDisplayToLastMoved(boardCrd: boardCrdTo);
            }
        }


        private void UpdateLastMovedBoardCrd(FBoardCrd boardCrdTo)
        {
            _lastMovedBoardCrd = boardCrdTo;
        }



        // ===============================================
        // MARK: - ● History Detail ─────

        public void HighlightLastMovedCellForHistoryDetailStage(FBoardCrd boardCrd)
        {
            Assert.IsTrue(_stageType == EStageType.HISTORY_DETAIL_STAGE);

            // 同じセルをハイライトしている場合は何もしない
            if (_highlightedBoardCrdListForHistoryDetailStage.Contains(boardCrd)) { return; }

            ActionChangeCellDisplayToLastMoved(boardCrd: boardCrd);

            _highlightedBoardCrdListForHistoryDetailStage.Add(boardCrd);
        }


        public void ClearHighlightedLastMovedCellForHistoryDetailStage()
        {
            Assert.IsTrue(_stageType == EStageType.HISTORY_DETAIL_STAGE);

            foreach (var i in _highlightedBoardCrdListForHistoryDetailStage)
            {
                ActionChangeCellDisplayFromLastMovedToDefaultForHistoryDetailStage(boardCrd: i);
            }

            _highlightedBoardCrdListForHistoryDetailStage.Clear();
        }


        public bool HasHighlightedLastMovedCellForHistoryDetailStage()
        {
            Assert.IsTrue(_stageType == EStageType.HISTORY_DETAIL_STAGE);

            return _highlightedBoardCrdListForHistoryDetailStage.Count > 0;
        }


        // ===============================================
        // MARK: - ● for History Detail Stage ─────

        public void UpdateUsingDiffData(CBoardDiffResultForBoard diffResultForBoard, CBoardDiffResultForWaitingSpace diffResultForWaitingSpacePlayer1, CBoardDiffResultForWaitingSpace diffResultForWaitingSpacePlayer2)
        {
            UpdateUsingDiffBoardData(diffResultForBoard: diffResultForBoard);
            UpdateUsingDiffWaitingSpaceData(diffResultForWaitingSpacePlayer1: diffResultForWaitingSpacePlayer1, diffResultForWaitingSpacePlayer2: diffResultForWaitingSpacePlayer2);
        }


        private void UpdateUsingDiffBoardData(CBoardDiffResultForBoard diffResultForBoard)
        {
            foreach (var i in diffResultForBoard.RemoveList)
            {
                RemovePieceOnBoardForHistoryDetail(playerType: i.PlayerType, boardCrd: i.Crd);
            }

            foreach (var i in diffResultForBoard.AddList)
            {
                AddPieceOnBoardForHistoryDetail(
                    playerType: i.PlayerType,
                    pieceType: i.PieceType,
                    isNari: i.IsNari,
                    boardCrd: i.Crd);
            }
        }


        private void RemovePieceOnBoardForHistoryDetail(EPlayerType playerType, FBoardCrd boardCrd)
        {
            Assert.IsTrue(boardCrd.IsOnBoard(boardType: _boardType));

            var pieceBehaviour = GetPieceBehaviourOnBoard(playerType: playerType, boardCrd: boardCrd);

            pieceBehaviour.Hide();

            RemoveAndDestroyPiece(pieceBehaviour: pieceBehaviour);
        }


        private void AddPieceOnBoardForHistoryDetail(EPlayerType playerType, EPieceType pieceType, bool isNari, FBoardCrd boardCrd)
        {
            var isGyokushouPlayer = _gyokushouPlayer == playerType;

            var pieceBehaviour = CreateGameObjectForPiece(
                playerType: playerType,
                isGyokushouPlayer: isGyokushouPlayer,
                new FPiecePlaceData(
                    pieceType: pieceType,
                    isNari: isNari,
                    boardType: _boardType,
                    boardCrd: boardCrd));

            pieceBehaviour.Show();
        }


        private void UpdateUsingDiffWaitingSpaceData(CBoardDiffResultForWaitingSpace diffResultForWaitingSpacePlayer1, CBoardDiffResultForWaitingSpace diffResultForWaitingSpacePlayer2)
        {
            void Support(EPlayerType playerType, CBoardDiffResultForWaitingSpace diffResult)
            {
                var waitingSpaceBehaviour = GetWaitingSpaceBehaviour(playerType: playerType);
                if (diffResult.DataList.Count == 0) { return; }

                
                // Count = 0 になるとき、１になるとき、２以上のとき
                foreach (var i in diffResult.DataList)
                {
                    var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: i.PieceType);
                    var presentCount = waitingSpaceBehaviour.GetCountOfPiece(pieceType: i.PieceType);
                    var newCount = presentCount + i.Delta;

                    waitingSpaceBehaviour.ChangeChangePieceCountForHistoryDetail(pieceType: i.PieceType, newCount: newCount);

                    if (newCount < presentCount)
                    {
                        // case: 減る場合
                        // 減る場合
                        // 減る数だけ非表示のコマを削除する
                        RemovePieceOnWaitingSpace(playerType: playerType, pieceType: i.PieceType, count: presentCount - newCount);
                    }
                    else
                    {
                        // case: 増える場合
                        // 増える数だけコマを足す
                        // 1つに増えた時には。表示する
                        // 2つに増える時には非表示にする
                        AddPieceOnWaitingSpace(playerType: playerType, pieceType: i.PieceType, count: newCount - presentCount);
                    }
                }
            }

            // 駒台の数値の変更
            Support(EPlayerType.PLAYER1, diffResultForWaitingSpacePlayer1);
            Support(EPlayerType.PLAYER2, diffResultForWaitingSpacePlayer2);
        }


        private void RemoveAndDestroyPiece(PieceBehaviour pieceBehaviour)
        {
            _pieceBehaviourList.Remove(pieceBehaviour);

            var pieceID = pieceBehaviour.PieceID;
            _pieceIDManager.RemoveIndex(index: pieceID);

            Utl.SafeDestroy(pieceBehaviour);
        }


        private void RemovePieceOnWaitingSpace(EPlayerType playerType, EPieceType pieceType, int count)
        {
            Assert.IsTrue(count > 0);

            for (int i = 0; i < count; i++)
            {
                var targetPieceBehaviour = GetNextHiddenPieceBehaviourOnWaitingSpace(playerType: playerType, pieceType: pieceType);

                if (targetPieceBehaviour != null)
                {
                    // case: 駒台に隠れている駒がある場合
                    RemoveAndDestroyPiece(pieceBehaviour: targetPieceBehaviour);
                }
                else
                {
                    // case: 駒台に隠れている駒がない場合 = 表示されている駒
                    targetPieceBehaviour = GetPieceBihaviourOnWaitingSpace(playerType: playerType, pieceType: pieceType);
                    targetPieceBehaviour.Hide();
                    RemoveAndDestroyPiece(pieceBehaviour: targetPieceBehaviour);
                }  
            }
        }


        private void AddPieceOnWaitingSpace(EPlayerType playerType, EPieceType pieceType, int count)
        {
            Assert.IsTrue(count > 0);

            // 現在何も表示されていない場合、最初の一つを表示する必要がある
            var isNeedShowFirstPiece = GetWaitingSpaceBehaviour(playerType: playerType).GetCountOfPiece(pieceType: pieceType) - count == 0;

            for (int i = 0; i < count; i++)
            {
                // あたらしくコマを生成して、駒台に配置する

                // 実際には王将を生成はしないので無意味
                var isGyokushouPlayer = _gyokushouPlayer == playerType;
    
                // 駒台上を示す
                var boardCrd = FBoardCrd.AsOnWaitingSpace;


                var pieceBehaviour = CreateGameObjectForPiece(
                    playerType: playerType,
                    isGyokushouPlayer: isGyokushouPlayer,
                    new FPiecePlaceData(
                        pieceType: pieceType,
                        isNari: false,
                        boardType: _boardType,
                        boardCrd: boardCrd));

                // 最初の一つであれば表示して、そうでない場合には非表示とする
                if (isNeedShowFirstPiece && i == 0)
                {
                    pieceBehaviour.Show();
                }
            }
        }
        

        // ===============================================
        // MARK: - ● Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _boardBehaviour.SetAlpha(alpha: alpha);
            _pieceBehaviourList.ForEach(i => i.SetAlpha(alpha: alpha));
            _waitingSpaceBehaviourPlayer1.SetAlpha(alpha: alpha);
            _waitingSpaceBehaviourPlayer2.SetAlpha(alpha: alpha);
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            Assert.IsTrue(_isShown == false);

            _boardBehaviour.Show();
            _pieceBehaviourList.ForEach(i => i.Show());
            _waitingSpaceBehaviourPlayer1.Show();
            _waitingSpaceBehaviourPlayer2.Show();

            _isShown = true;
        }

        public override void FadeIn(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown == false);

            _boardBehaviour.FadeIn();
            _pieceBehaviourList.ForEach(i => i.FadeIn());
            _waitingSpaceBehaviourPlayer1.FadeIn();
            _waitingSpaceBehaviourPlayer2.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown == true);

            _boardBehaviour.FadeOut();
            _pieceBehaviourList.ForEach(i => i.FadeOut());
            _waitingSpaceBehaviourPlayer1.FadeOut();
            _waitingSpaceBehaviourPlayer2.FadeOut();

            _isShown = false;
        }

        public override void Hide()
        {
            Assert.IsTrue(_isShown == true);

            _boardBehaviour.Hide();
            _pieceBehaviourList.ForEach(i => i.Hide());
            _waitingSpaceBehaviourPlayer1.Hide();
            _waitingSpaceBehaviourPlayer2.Hide();

            _isShown = false;
        }


        // ===============================================
        // MARK: - ● Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _gameStageBehaviour = null;
            _historyDetailStageBehaviour = null;


            _boardImageSetManager = null;
            _pieceImageSetManager = null;
            _boardSetInputManager = null;

            // Unity Editor での表示の場合停止タイミングで _layouter が null となるためチェックして破壊
            _layouter?.Destroy();
            _layouter = null;


            _boardBehaviour = null;
            _pieceBehaviourList.Clear();
            _waitingSpaceBehaviourPlayer1 = null;
            _waitingSpaceBehaviourPlayer2 = null;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public class BoardSetLayouter : PLayouter
    {
        // ===============================================
        // MARK: - ● Property ─────

        // 盤の種類
        private EBoardType _boardType;


        /// <summary>
        /// ボードセット
        /// </summary>
        private LayoutRoot _layoutStageBoardSet;

        /// <summary>
        /// ステージの駒台プレイヤー1(最大)
        /// </summary>
        private LayoutFrame _frameStageWaitingSpacePlayer1Max;

        /// <summary>
        /// ステージの駒台プレイヤー2(最大)
        /// </summary>
        private LayoutFrame _frameStageWaitingSpacePlayer2Max;


        /// <summary>
        /// ステージの駒台プレイヤー1
        /// </summary>
        private LayoutFrame _frameStageWaitingSpacePlayer1;

        /// <summary>
        /// ステージの駒台プレイヤー2
        /// </summary>
        private LayoutFrame _frameStageWaitingSpacePlayer2;

        /// <summary>
        /// ステージの盤とスペース
        /// </summary>
        private LayoutFrame _frameStageBoardMax;

        /// <summary>
        /// ステージの盤
        /// </summary>
        private LayoutFrame _frameStageBoard;

        /// <summary>
        /// ステージの盤のグリッド(6×6)
        /// </summary>
        private LayoutGrid _gridStageBoardCell;

        /// <summary>
        /// ステージの駒台プレイヤー1のグリッド(水平並び)
        /// </summary>
        private LayoutGrid _gridStageWaitingSpaceHorizontalPlayer1Cell;

        /// <summary>
        /// ステージの駒台プレイヤー2のグリッド(水平並び)
        /// </summary>
        private LayoutGrid _gridStageWaitingSpaceHorizontalPlayer2Cell;


        /// <summary>
        /// 投了／成り選択表示
        /// </summary>
        private LayoutFrame _frameStageSelector;


        // ===============================================
        // MARK: - ● Constructor ─────

        public BoardSetLayouter(EBoardType boardType)
        {
            _boardType = boardType;

            Setup();
            //ShowLayoutArea();
        }


        // ===============================================
        // MARK: - ● Setup Layout ─────

        private void Setup()
        {
            var stageBoardSetArea = Locator.Instance.layoutManager.AreaGameStageBoardSet.AsYReversed();
            _layoutStageBoardSet = new LayoutRoot(area: stageBoardSetArea, spacing: FVec4.ZERO);

            _frameStageWaitingSpacePlayer2Max = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.95f, 0.15f)),
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageWaitingSpacePlayer1Max = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.95f, 0.15f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageBoardMax = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.95f, 0.7f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);


            _layoutStageBoardSet.RegisterChildByList(newChildList: new List<PLayout> {
                _frameStageWaitingSpacePlayer2Max,
                _frameStageWaitingSpacePlayer1Max,
                _frameStageBoardMax });

            // 一度計算しておく
            _layoutStageBoardSet.Update();


            //-----------------------

            float XtoYRatioOfBoardArea = Config.BOARD_BODY__IMAGE_SIZE.GetXtoYRatio();
            FArea boardArea = _frameStageBoardMax.Area.GetPartialArea(XtoYRatio: XtoYRatioOfBoardArea);

            FVec4 boardSpacing;

            switch (_boardType)
            {
                case EBoardType.BOARD_66:
                    boardSpacing = new FVec4(
                        boardArea.size.y * Config.BOARD_BODY__INNER_SPACE4_RATIO_66.top,
                        boardArea.size.x * Config.BOARD_BODY__INNER_SPACE4_RATIO_66.right,
                        boardArea.size.y * Config.BOARD_BODY__INNER_SPACE4_RATIO_66.bottom,
                        boardArea.size.x * Config.BOARD_BODY__INNER_SPACE4_RATIO_66.left);
                    break;

                case EBoardType.BOARD_99:
                    boardSpacing = new FVec4(
                        boardArea.size.y * Config.BOARD_BODY__INNER_SPACE4_RATIO_99.top,
                        boardArea.size.x * Config.BOARD_BODY__INNER_SPACE4_RATIO_99.right,
                        boardArea.size.y * Config.BOARD_BODY__INNER_SPACE4_RATIO_99.bottom,
                        boardArea.size.x * Config.BOARD_BODY__INNER_SPACE4_RATIO_99.left);
                    break;

                default:
                    throw new InvalidOperationException();
            }


            _frameStageBoard = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: boardArea.size),
                align: EDirection.STAY,
                priority: 0,
                spacing: boardSpacing,
                offset: Vector2.zero);

            _frameStageBoardMax.RegisterChildByList(newChildList: new List<PLayout> { _frameStageBoard });


            switch (_boardType)
            {
                case EBoardType.BOARD_66:
                    _gridStageBoardCell = new LayoutGrid(
                    sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                    align: EDirection.STAY,
                    priority: 0,
                    spacing: FVec4.ZERO,
                    innerSpacingXY: new Vector2(0, 0),
                    cellCount: new Vector2Int(6, 6),
                    cellPadding: FVec4.ZERO,
                    offset: Vector2.zero);
                    break;

                case EBoardType.BOARD_99:
                    _gridStageBoardCell = new LayoutGrid(
                    sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                    align: EDirection.STAY,
                    priority: 0,
                    spacing: FVec4.ZERO,
                    innerSpacingXY: new Vector2(0, 0),
                    cellCount: new Vector2Int(9, 9),
                    cellPadding: FVec4.ZERO,
                    offset: Vector2.zero);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            
            _frameStageBoard.RegisterChildByList(newChildList: new List<PLayout> { _gridStageBoardCell });


            // 持ち駒台のエリアを追加

            float XtoYRatioOfWaitingSpace = Config.WAITING_SPACE_HORIZONTAL__IMAGE_SIZE.GetXtoYRatio();
            FArea waitingSpacePlayer1Area = _frameStageWaitingSpacePlayer1Max.Area.GetPartialArea(XtoYRatio: XtoYRatioOfWaitingSpace);
            FVec4 waitingSpaceHorizontalPlayer1Spacing = new FVec4(
                waitingSpacePlayer1Area.size.y * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER1.top,
                waitingSpacePlayer1Area.size.x * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER1.right,
                waitingSpacePlayer1Area.size.y * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER1.bottom,
                waitingSpacePlayer1Area.size.x * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER1.left);

            _frameStageWaitingSpacePlayer1 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: waitingSpacePlayer1Area.size),
                align: EDirection.DOWN,
                priority: 0,
                spacing: waitingSpaceHorizontalPlayer1Spacing,
                offset: Vector2.zero);

            _frameStageWaitingSpacePlayer1Max.RegisterChildByList(newChildList: new List<PLayout> { _frameStageWaitingSpacePlayer1 });


            _gridStageWaitingSpaceHorizontalPlayer1Cell = new LayoutGrid(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                innerSpacingXY: new Vector2(0, 0),
                cellCount: new Vector2Int(7, 1),
                cellPadding: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageWaitingSpacePlayer1.RegisterChildByList(newChildList: new List<PLayout> { _gridStageWaitingSpaceHorizontalPlayer1Cell });


            FArea waitingSpacePlayer2Area = _frameStageWaitingSpacePlayer1Max.Area.GetPartialArea(XtoYRatio: XtoYRatioOfWaitingSpace);
            FVec4 waitingSpaceHorizontalPlayer2Spacing = new FVec4(
                waitingSpacePlayer2Area.size.y * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER2.top,
                waitingSpacePlayer2Area.size.x * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER2.right,
                waitingSpacePlayer2Area.size.y * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER2.bottom,
                waitingSpacePlayer2Area.size.x * Config.WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER2.left);

            _frameStageWaitingSpacePlayer2 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: waitingSpacePlayer2Area.size),
                align: EDirection.UP,
                priority: 0,
                spacing: waitingSpaceHorizontalPlayer2Spacing,
                offset: Vector2.zero);

            _frameStageWaitingSpacePlayer2Max.RegisterChildByList(newChildList: new List<PLayout> { _frameStageWaitingSpacePlayer2 });


            _gridStageWaitingSpaceHorizontalPlayer2Cell = new LayoutGrid(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                innerSpacingXY: new Vector2(0, 0),
                cellCount: new Vector2Int(7, 1),
                cellPadding: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageWaitingSpacePlayer2.RegisterChildByList(newChildList: new List<PLayout> { _gridStageWaitingSpaceHorizontalPlayer2Cell });


            // 投了／成り選択表示

            var ratioResignSelectorPerBoardBody = Config.RESIGN_SELECTOR_BODY__IMAGE_SIZE / Config.BOARD_BODY__IMAGE_SIZE;

            _frameStageSelector = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: ratioResignSelectorPerBoardBody),
                align: EDirection.STAY,
                priority: 0,
                spacing: boardSpacing,
                offset: Vector2.zero);

            _frameStageBoard.RegisterChildByList(newChildList: new List<PLayout> { _frameStageSelector });


            _layoutStageBoardSet.Update();
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutStageBoardSet?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("FrameStageWaitingSpacePlayer1Max", _frameStageWaitingSpacePlayer1Max.Area.AsYReversed()),
                ("FrameStageWaitingSpacePlayer2Max", _frameStageWaitingSpacePlayer2Max.Area.AsYReversed()),
                ("FrameStageBoardAndSpace", _frameStageBoardMax.Area.AsYReversed()),
                ("FrameStageWaitingSpacePlayer2", _frameStageWaitingSpacePlayer2.Area.AsYReversed()),
                ("FrameStageWaitingSpacePlayer1", _frameStageWaitingSpacePlayer1.Area.AsYReversed()),
                ("FrameStageBoard", _frameStageBoard.Area.AsYReversed()),
                ("GridStageBoardCell", _gridStageBoardCell.Area.AsYReversed()),
                ("WaitingSpaceCellAreaForHorizontal(P1)", GetAreaWaitingSpaceCellForHorizontal(playerType: EPlayerType.PLAYER1, index: 0)),
                ("WaitingSpaceCellAreaForHorizontal(P2)", GetAreaWaitingSpaceCellForHorizontal(playerType: EPlayerType.PLAYER2, index: 0)),
                ("PieceCountArea(P1:0)", GetAreaPieceCount(playerType: EPlayerType.PLAYER1, index: 0)),
                ("PieceCountArea(P1:6)", GetAreaPieceCount(playerType: EPlayerType.PLAYER1, index: 6)),
                ("PieceCountArea(P2:0)", GetAreaPieceCount(playerType: EPlayerType.PLAYER2, index: 0)),
                ("PieceCountArea(P2:6)", GetAreaPieceCount(playerType: EPlayerType.PLAYER2, index: 6))
            });

            switch (_boardType)
            {
                case EBoardType.BOARD_66:
                    Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                        ("Board66CellArea(1,1)", GetAreaBoardCell(boardType: EBoardType.BOARD_66, boardCrd: new FBoardCrd(1, 1))),
                        ("Board66CellArea(3,3)", GetAreaBoardCell(boardType: EBoardType.BOARD_66, boardCrd: new FBoardCrd(3, 3))),
                        ("Board66CellArea(6,6)", GetAreaBoardCell(boardType: EBoardType.BOARD_66, boardCrd: new FBoardCrd(6, 6)))
                    });
                    break;

                case EBoardType.BOARD_99:
                    Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                        ("Board99CellArea(1,1)", GetAreaBoardCell(boardType: EBoardType.BOARD_99, boardCrd: new FBoardCrd(1, 1))),
                        ("Board99CellArea(3,3)", GetAreaBoardCell(boardType: EBoardType.BOARD_99, boardCrd: new FBoardCrd(3, 3))),
                        ("Board99CellArea(6,6)", GetAreaBoardCell(boardType: EBoardType.BOARD_99, boardCrd: new FBoardCrd(6, 6))),
                        ("Board99CellArea(9,9)", GetAreaBoardCell(boardType: EBoardType.BOARD_99, boardCrd: new FBoardCrd(9, 9)))
                    });
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }



        // ===============================================
        // MARK: - ● Get Area ─────

        public FArea GetAreaWaitingSpace(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => _frameStageWaitingSpacePlayer1.Area.AsYReversed(),
                EPlayerType.PLAYER2 => _frameStageWaitingSpacePlayer2.Area.AsYReversed(),
                _ => throw new InvalidOperationException(),
            };
        }


        public FArea GetAreaBoard()
        {
            return _frameStageBoard.Area.AsYReversed();
        }


        public FArea GetAreaBoardCell(EBoardType boardType, FBoardCrd boardCrd)
        {
            Assert.IsTrue(boardCrd.IsOnBoard(boardType: boardType));

            return _gridStageBoardCell.GetCellAreaWithPadding(crd: boardCrd.AsCrdTopLeftOrigin(boardType: boardType)).AsYReversed();
        }


        public FArea GetAreaWaitingSpaceCell(EPlayerType playerType, int index)
        {
            Assert.IsTrue(0 <= index && index <= 6);

            return Locator.Instance.layoutManager.GetWaitingSpaceType() switch
            {
                EWaitingSpaceType.HORIZONTAL => GetAreaWaitingSpaceCellForHorizontal(playerType: playerType, index: index),

                EWaitingSpaceType.SQUARE => GetAreaWaitingSpaceCellForSquare(playerType: playerType, index: index),

                _ => throw new InvalidOperationException(),
            };
        }


        private FArea GetAreaWaitingSpaceCellForHorizontal(EPlayerType playerType, int index)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => _gridStageWaitingSpaceHorizontalPlayer1Cell.GetCellAreaWithPadding(crd: new Vector2Int(6 - index, 0)).AsYReversed(),

                EPlayerType.PLAYER2 => _gridStageWaitingSpaceHorizontalPlayer2Cell.GetCellAreaWithPadding(crd: new Vector2Int(index, 0)).AsYReversed(),

                _ => throw new InvalidOperationException(),
            };
        }


        // TODO: 未実装
        public FArea GetAreaWaitingSpaceCellForSquare(EPlayerType playerType, int index)
        {
            throw new System.NotImplementedException();
        }


        public FArea GetAreaPieceCount(EPlayerType playerType, int index)
        {
            Assert.IsTrue(0 <= index && index <= 6);

            return Locator.Instance.layoutManager.GetWaitingSpaceType() switch
            {
                EWaitingSpaceType.HORIZONTAL => GetAreaWaitingSpacePieceCountForHorizontal(playerType: playerType, index: index),

                EWaitingSpaceType.SQUARE => GetAreaWaitingSpacePieceCountForSquare(playerType: playerType, index: index),

                _ => throw new InvalidOperationException(),
            };
        }


        private FArea GetAreaWaitingSpacePieceCountForHorizontal(EPlayerType playerType, int index)
        {
            Vector2 center = GetAreaWaitingSpaceCellForHorizontal(playerType: playerType, index: index).TopPoint;

            Vector2 ratio = Config.WAITING_SPACE_HORIZONTAL__PIECE_COUNT__SIZE / Config.WAITING_SPACE_HORIZONTAL__IMAGE_SIZE;
            Vector2 size = GetAreaWaitingSpace(playerType: playerType).size * ratio;

            return new FArea(p1: center, p2: size, areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);
        }


        public FArea GetAreaWaitingSpacePieceCountForSquare(EPlayerType playerType, int index)
        {
            Vector2 center = GetAreaWaitingSpaceCellForSquare(playerType: playerType, index: index).TopPoint;

            Vector2 ratio = Config.WAITING_SPACE_SQUARE__PIECE_COUNT__SIZE / Config.WAITING_SPACE_SQUARE__IMAGE_SIZE;
            Vector2 size = GetAreaWaitingSpace(playerType: playerType).size * ratio;

            return new FArea(p1: center, p2: size, areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);
        }


        public FArea GetAreaSelectorOnGameStage()
        {
            return _frameStageSelector.Area;
        }
    }
}
using System;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public struct FBoardDiffSupportData
    {
        public EPieceType PieceType;
        public bool IsNari;
        public FBoardCrd BoardCrd;

        public FBoardDiffSupportData(
            EPieceType pieceType,
            bool isNari,
            FBoardCrd boardCrd)
        {
            PieceType = pieceType;
            IsNari = isNari;
            BoardCrd = boardCrd;
        }
    }

    public class CBoardDiffCalcurator
    {
        private HistoryDetailStageBehaviour _historyDetailStageBehaviour = null;
        private EMoveOrderType _moveOrderTypeOfPlayer1 = default!;
        private EPlayerType _gyokushouPlayer = default!;
        private EBoardType _boardType = default!;

        private CBoardDiffData _presentData = new CBoardDiffData();
        private CBoardDiffData _nextData = new CBoardDiffData();

        public CBoardDiffCalcurator()
        {
            // do nothing
        }

        public void DebugPrint()
        {
            var textList = _presentData.BoardData.AsStringList();
            textList.Add(_presentData.WaitingSpaceDataPlayer1.AsString());
            textList.Add(_presentData.WaitingSpaceDataPlayer2.AsString());

            Debug.Log(string.Join("\n", textList));
        }



        // リストを受け取ってセットアップする
        public void Setup(
            HistoryDetailStageBehaviour historyDetailStageBehaviour,
            in FGameStageInitializer gameStageInitializer)
        {
            _historyDetailStageBehaviour = historyDetailStageBehaviour;
            _moveOrderTypeOfPlayer1 = _historyDetailStageBehaviour.MoveOrderTypeOfPlayer1;
            _gyokushouPlayer = gameStageInitializer.GyokushouPlayer;
            _boardType = gameStageInitializer.BoardType;

            RegisterData(playerData: gameStageInitializer.PlayerData1); 
            RegisterData(playerData: gameStageInitializer.PlayerData2); 
        }


        private void RegisterData(in FPlayerDataForFramework playerData)
        {
            var playerType = playerData.MoveOrderType == _moveOrderTypeOfPlayer1 ? EPlayerType.PLAYER1 : EPlayerType.PLAYER2;


            foreach (FPiecePlaceData i in playerData.PiecePlaceDataList)
            {
                if (i.BoardCrd.IsOnBoard(boardType: _boardType))
                {
                    // case: OnBoard

                    var dataElement = new FBoardDiffBoardDataElement(
                        boardType: _boardType,
                        playerType: playerType,
                        crd: i.BoardCrd,
                        pieceType: i.PieceType,
                        isNari: i.IsNari
                    );

                    _presentData.BoardData.RegisterData(dataElement);
                }
                else
                {
                    // case: OnWaitingSpace

                    switch (playerType)
                    {
                        case EPlayerType.PLAYER1:
                            _presentData.WaitingSpaceDataPlayer1.Increment(pieceType: i.PieceType);
                            break;
                        case EPlayerType.PLAYER2:
                            _presentData.WaitingSpaceDataPlayer2.Increment(pieceType: i.PieceType);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
        }





        private List<FBoardDiffSupportData> CreateBoardDiffSupportDataListForBoard(List<FPieceAndSquare> playerPieceList, EMoveOrderType moveOrderTypeOfPlayer1)
        {
            var result = new List<FBoardDiffSupportData>();

            foreach (var i in playerPieceList)
            {
                var pieceTypeAndIsNari = Utl.ConvertPieceTypeMainToGFrameWork(pieceTypeMain: i.PieceType);
                var boardCrd = new FBoardCrd(boardType: _boardType, square: i.Square, moveOrderType: moveOrderTypeOfPlayer1);

                result.Add(new FBoardDiffSupportData(
                    pieceType: pieceTypeAndIsNari.pieceType,
                    isNari: pieceTypeAndIsNari.isNari,
                    boardCrd: boardCrd
                ));
            }

            return result;
        }

        public CBoardDiffResult CalcurateDiffAndProcessForBoard(in List<FPieceAndSquare> player1PieceList, in List<FPieceAndSquare> player2PieceList)
        {
            SetNextData(
                player1PieceListAsSupportData: CreateBoardDiffSupportDataListForBoard(playerPieceList: player1PieceList, moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1),
                player2PieceListAsSupportData: CreateBoardDiffSupportDataListForBoard(playerPieceList: player2PieceList, moveOrderTypeOfPlayer1: _moveOrderTypeOfPlayer1)
            );

            return CalcurateDiffAndProcessMain();
        }


        private CBoardDiffResult CalcurateDiffAndProcessMain()
        {
            var resultBoard = _presentData.BoardData.CompareAndCopy(other: _nextData.BoardData);
            var resultWaitingSpacePlayer1 = _presentData.WaitingSpaceDataPlayer1.CompareAndCopy(other: _nextData.WaitingSpaceDataPlayer1);
            var resultWaitingSpacePlayer2 = _presentData.WaitingSpaceDataPlayer2.CompareAndCopy(other: _nextData.WaitingSpaceDataPlayer2);

            // 次に備えてデータをクリアする
            _presentData.BoardData.ClearCheckList();
            _nextData.Clear();

            return new CBoardDiffResult(
                resultBoard: resultBoard,
                resultWaitingSpacePlayer1: resultWaitingSpacePlayer1,
                resultWaitingSpacePlayer2: resultWaitingSpacePlayer2
            );
        }




        // リストを受け取ってDiffを取り、処理する。最終的にデータを入れる
        private void SetNextData(
            in List<FBoardDiffSupportData> player1PieceListAsSupportData,
            in List<FBoardDiffSupportData> player2PieceListAsSupportData)
        {
            void Support(
                EPlayerType playerType,
                in List<FBoardDiffSupportData> playerPieceListAsSupportData)
            {
                foreach (var i in playerPieceListAsSupportData)
                {
                    if (i.BoardCrd.IsOnBoard(boardType: _boardType))
                    {
                        // case: OnBoard

                        var dataElement = new FBoardDiffBoardDataElement(
                            boardType: _boardType,
                            playerType: playerType,
                            crd: i.BoardCrd,
                            pieceType: i.PieceType,
                            isNari: i.IsNari
                        );

                        _nextData.BoardData.RegisterData(dataElement);
                    }
                    else
                    {
                        // case: OnWaitingSpace

                        switch (playerType)
                        {
                            case EPlayerType.PLAYER1:
                                _nextData.WaitingSpaceDataPlayer1.Increment(pieceType: i.PieceType);
                                break;
                            case EPlayerType.PLAYER2:
                                _nextData.WaitingSpaceDataPlayer2.Increment(pieceType: i.PieceType);
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                }
            }
            

            Support(
                playerType: EPlayerType.PLAYER1,
                playerPieceListAsSupportData: player1PieceListAsSupportData
            );
 
            Support(
                playerType: EPlayerType.PLAYER2,
                playerPieceListAsSupportData: player2PieceListAsSupportData
            );
        }
    }
}
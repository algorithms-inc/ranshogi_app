using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;

namespace GFramework
{
    public sealed class BoardSetInputManager : IBoardSetInputManager
    {
        // ===============================================
        // MARK: - ● Property ─────

        private BoardSetBehaviour _boardSetBehaviour = null;


        private Canvas _canvas;

        private TapInputBehaviour _tapInputWholeScreen = null;
        private BoardInputBehaviour[] _boardInputBehaviourList;
        private WaitingSpaceInputBehaviour[] _waitingSpaceInputBehaviourList = new WaitingSpaceInputBehaviour[7];
        // 相手(Player2)の駒台。検討画面では両者の駒を動かすので作る
        private WaitingSpaceInputBehaviour[] _waitingSpaceInputBehaviourListPlayer2 = null;

        private bool _isEnabled = true;
        public bool IsBoardInputEnabled => _isEnabled;

        public Canvas Canvas
        {
            get
            {
                Assert.IsTrue(_canvas != null);

                return _canvas;
            }
        }


        // ===============================================
        // MARK: - ● Constructor ─────

        public BoardSetInputManager(BoardSetBehaviour boardSetBehaviour)
        {
            _boardSetBehaviour = boardSetBehaviour;


            switch (_boardSetBehaviour.BoardType)
            {
                case EBoardType.BOARD_66:
                    _boardInputBehaviourList = new BoardInputBehaviour[36];
                    break;
                case EBoardType.BOARD_99:
                    _boardInputBehaviourList = new BoardInputBehaviour[81];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_boardSetBehaviour.BoardType), _boardSetBehaviour.BoardType, null);
            }

            SetupInputCanvas();
        }



        private void SetupInputCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "InputCanvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: _boardSetBehaviour.gameObject.transform,  // 元は gameStageBehaviour.gameObject.transform, だった。調整が必要かも
                sortingOrder: Config.STAGE_INPUT__LAYERED_ORDERID.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }



        // ===============================================
        // MARK: - ● Setup for Game ─────

        public void SetupInputForGame()
        {
            SetupTapInputWholeScreen();
            SetupBoardInput();
            SetupWaitingSpaceInput();

            // 最初は入力不可に
            SetIsEnabled(value: false);
        }


        // 検討画面用。両者の駒台に入力を置き、最初から入力できるようにしておく
        public void SetupInputForHistoryDetail()
        {
            SetupTapInputWholeScreen();
            SetupBoardInput();
            SetupWaitingSpaceInput();
            SetupWaitingSpaceInputPlayer2();
        }


        private void SetupWaitingSpaceInputPlayer2()
        {
            var canvasTransform = _canvas.transform;
            _waitingSpaceInputBehaviourListPlayer2 = new WaitingSpaceInputBehaviour[7];

            for (int index = 0; index < 7; index++)
            {
                GameObject newGameObject = new GameObject(name: "WaitingSpaceInputPlayer2_" + index.ToString());
                newGameObject.transform.parent = canvasTransform;

                WaitingSpaceInputBehaviour newWaitingSpaceInputBehaviour = newGameObject.AddComponent<WaitingSpaceInputBehaviour>();

                newWaitingSpaceInputBehaviour.SetupComponent(
                    boardSetBehaviour: _boardSetBehaviour,
                    index: index,
                    isEnabled: _isEnabled,
                    actionOnPointerDown: (int index) => {
                        _boardSetBehaviour.OnTapWaitingSpace(playerType: EPlayerType.PLAYER2, index: index);
                    },
                    playerType: EPlayerType.PLAYER2);

                _waitingSpaceInputBehaviourListPlayer2[index] = newWaitingSpaceInputBehaviour;
            }
        }


        private void SetupTapInputWholeScreen()
        {
            _tapInputWholeScreen = Utl.CreateTapInputFullScreen(
                canvas: _canvas,
                isEnabled: true,
                action: (ETapInputType tapInputType, int durationFrame) => {
                    _boardSetBehaviour.OnTapOutOfBoard();
                });
        }


        private void SetupBoardInput()
        {
            var canvasTransform = _canvas.transform;
            int index = 0;

            var lineCount = _boardSetBehaviour.BoardType.GetLineCount();

            for (int y = 1; y <= lineCount; y++)
            {
                for (int x = 1; x <= lineCount; x++)
                {
                    FBoardCrd boardCrd = new FBoardCrd(x: x, y: y);

                    GameObject newGameObject = new GameObject(name: "BoardInput" + boardCrd.ToString());
                    newGameObject.transform.parent = canvasTransform;

                    BoardInputBehaviour newBoardInputBehaviour = newGameObject.AddComponent<BoardInputBehaviour>();

                    newBoardInputBehaviour.SetupComponent(
                        boardSetBehaviour: _boardSetBehaviour,
                        boardCrd: boardCrd,
                        isEnabled: _isEnabled,
                        actionOnPointerDown: (FBoardCrd boardCrd) => {
                            _boardSetBehaviour.OnTapBoard(boardCrd: boardCrd);
                        },
                        actionOnPointerUp: null,
                        actionOnPointerClick: null,
                        actionOnPointerEnter: null,
                        actionOnPointerExit: null);

                    _boardInputBehaviourList[index] = newBoardInputBehaviour;

                    index++;
                }
            }
        }


        private void SetupWaitingSpaceInput()
        {
            var canvasTransform = _canvas.transform;

            for (int index = 0; index < 7; index++)
            {
                GameObject newGameObject = new GameObject(name: "WaitingSpaceInput" + index.ToString());
                newGameObject.transform.parent = canvasTransform;

                WaitingSpaceInputBehaviour newWaitingSpaceInputBehaviour = newGameObject.AddComponent<WaitingSpaceInputBehaviour>();

                newWaitingSpaceInputBehaviour.SetupComponent(
                    boardSetBehaviour: _boardSetBehaviour,
                    index: index,
                    isEnabled: _isEnabled,
                    actionOnPointerDown: (int index) => {
                        _boardSetBehaviour.OnTapWaitingSpace(index: index);
                    },
                    actionOnPointerUp: null,
                    actionOnPointerClick: null,
                    actionOnPointerEnter: null,
                    actionOnPointerExit: null);

                _waitingSpaceInputBehaviourList[index] = newWaitingSpaceInputBehaviour;
            }
        }


        // ===============================================
        // MARK: - ● Enability ─────

        public void SetIsEnabledIfNeeded(bool value)
        {
            if (_isEnabled == value) { return; }

            SetIsEnabled(value: value);
        }


        public void SetIsEnabled(bool value)
        {
            Assert.IsTrue(_isEnabled != value);


            _isEnabled = value;


            _tapInputWholeScreen.SetIsEnabled(value: _isEnabled);

            // 配列は LINQ の ForEach を使用できない
            foreach (var i in _boardInputBehaviourList)
            {
                i.SetIsEnabled(value: _isEnabled);
            }

            // 配列は LINQ の ForEach を使用できない
            foreach (var i in _waitingSpaceInputBehaviourList)
            {
                i.SetIsEnabled(value: _isEnabled);
            }

            if (_waitingSpaceInputBehaviourListPlayer2 != null)
            {
                foreach (var i in _waitingSpaceInputBehaviourListPlayer2)
                {
                    i.SetIsEnabled(value: _isEnabled);
                }
            }
        }


        public void Destroy()
        {
            _boardSetBehaviour = null;

            _tapInputWholeScreen = null;
            _waitingSpaceInputBehaviourListPlayer2 = null;

            for (int index = 0; index < _boardInputBehaviourList.Length; index++)
            {
                _boardInputBehaviourList[index] = null;
            }

            for (int index = 0; index < _waitingSpaceInputBehaviourList.Length; index++)
            {
                _waitingSpaceInputBehaviourList[index] = null;
            }
        }
    }
}


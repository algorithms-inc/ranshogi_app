using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



/// ★★★
/// 影の表示位置を修正すること？

namespace GFramework
{
    public class WaitingSpaceBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private BoardSetBehaviour _boardSetBehaviour = null;

        private EWaitingSpaceType _waitingSpaceType;
        private EPlayerType _playerType;

        private readonly PieceCountBehaviour[] _pieceCounterList = new PieceCountBehaviour[7];


        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _bodySingleImageActionBehaviour;
        private SingleImageActionBehaviour _shadowSingleImageActionBehaviour;


        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _bodySingleImageActionBehaviour.Alpha;


        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(BoardSetBehaviour boardSetBehaviour, EWaitingSpaceType waitingSpaceType, EPlayerType playerType)
        {
            _boardSetBehaviour = boardSetBehaviour;
            _waitingSpaceType = waitingSpaceType;
            _playerType = playerType;

            SetupGameObject();
            SetupPieceCount();

            Hide();
        }


        private void SetupGameObject()
        {
            SetupBody();
            SetupShadow();

            SetLocalScale();

            SetupPositionOfWholeWaitingSpace();
        }



        private void SetupBody()
        {
            _bodySingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: (_playerType == EPlayerType.PLAYER1) ? "WaitingSpacePlayer1Body" : "WaitingSpacePlayer2Body",
                texturePathUnderResources: _boardSetBehaviour.BoardImageSetManager.GetFilePathOfImageWaitingSpaceBody(waitingSpaceType: _waitingSpaceType, playerType: _playerType),
                willCacheIfNeeded: true,
                sortingOrder: Config.WAITING_SPACE_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f)
                );
        }


        private void SetupShadow()
        {
            _shadowSingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: (_playerType == EPlayerType.PLAYER1) ? "WaitingSpacePlayer1Shadow" : "WaitingSpacePlayer2Shadow",
                texturePathUnderResources: _boardSetBehaviour.BoardImageSetManager.GetFilePathOfImageWaitingSpaceShadow(waitingSpaceType: _waitingSpaceType, playerType: _playerType),
                willCacheIfNeeded: true,
                sortingOrder: Config.WAITING_SPACE_SHADOW__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: Config.WAITING_SPACE_SHADOW__ALPHA__DEFAULT,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f)
                );

            SetLocalPositonOfShadowByRatio(ratio: Config.WAITING_SPACE_SHADOW__LOCAL_POSITION_RATIO__DEFAULT);
        }


        private void SetupPieceCount()
        {
            for (int index = 0; index <= 6; index++)
            {
                GameObject newGameObjectPiceCounter = new GameObject("PieceCount" + index.ToString());
                newGameObjectPiceCounter.transform.parent = transform;

                PieceCountBehaviour newPieceCountBehaviour = newGameObjectPiceCounter.AddComponent<PieceCountBehaviour>();
                newPieceCountBehaviour.SetupComponent(boardSetBehaviour: _boardSetBehaviour, playerType: _playerType, index: index);

                _pieceCounterList[index] = newPieceCountBehaviour;
            }
        }


        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositonOfShadowByRatio(Vector2 ratio)
        {
            Vector2 baseImageSize = UtlSprite.GetTextureSize(gameObject: _bodySingleImageActionBehaviour.gameObject);
            Utl.SetLocalPositionByRatio(gameObject: _shadowSingleImageActionBehaviour.gameObject, baseImageSize: baseImageSize, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            // TODO: Locator からHORIZONTALかSQUAREなのかを取得するように変更すること
            float bodyImageSizeX = Config.WAITING_SPACE_HORIZONTAL__IMAGE_SIZE.x;
            float bodyDisplaySizeX = _boardSetBehaviour.GetAreaWaitingSpace(playerType: _playerType).size.x;

            float value = bodyDisplaySizeX / bodyImageSizeX;

            transform.localScale = new Vector2(value, value);
        }



        // ===============================================
        // MARK: - ● Setup Position ─────

        private void SetupPositionOfWholeWaitingSpace()
        {
            Vector2 centerPoint = _boardSetBehaviour.GetAreaWaitingSpace(playerType: _playerType).CenterPoint;

            Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: centerPoint);

            transform.position = position;
        }



        // ===============================================
        // MARK: - ●  ─────

        public void ShowCountOnGameBegin(int[] countList)
        {
            Assert.IsTrue(countList.Length == 7);

            var index = 0;

            foreach (var count in countList)
            {
                _pieceCounterList[index].SetCountOnGameBegin(count: count);

                index++;
            }
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void SetAlpha(float alpha)
        {
            _bodySingleImageActionBehaviour.SetAlpha(alpha: alpha);
            _shadowSingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }


        public override void Show()
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.Show();
            _shadowSingleImageActionBehaviour.Show();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.Hide();
            _shadowSingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _bodySingleImageActionBehaviour.FadeIn();
            _shadowSingleImageActionBehaviour.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _bodySingleImageActionBehaviour.FadeOut();
            _shadowSingleImageActionBehaviour.FadeOut();

            // 配列は LINQ の ForEach を利用できない
            foreach (var i in _pieceCounterList)
            {
                i.FadeOut();
            }

            _isShown = false;
        }



        // ===============================================
        // MARK: - ●  ─────

        // 駒をとった場合の処理
        // - 呼ばれている時点で、PieceBehaviour の属する WaitingSpace が一致すること
        // TODO: 初期配置の数表示は別関数で行う
        public void RegisterPiece(PieceBehaviour pieceBehaviour)
        {
            Assert.IsTrue(pieceBehaviour.PieceState.PlayerType == _playerType);

            var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: pieceBehaviour.PieceState.PieceType);

            // すでに同じ駒を持っていれば、駒を非表示にする
            if (_pieceCounterList[index].Count >= 1)
            {
                pieceBehaviour.Hide();
            }

            _pieceCounterList[index].IncrementCount();
        }

        // TODO: 未使用なので、正しい操作でこちらを呼ぶ
        public void UnRegisterPiece(EPieceType pieceType)
        {
            Debug.Log("未実装");
            Assert.IsTrue(false);

            // 駒を取る？？取って設置するところまで管理するのは範囲が広そう
            // 誰がどう責任を担うかが不明。駒が責任を追うほうが良さそう

            // リストで管理する
            // ２こ以上になれば、カウントを変更する
            // 駒にアニメーションさせる（なしにもできるようにする）

            var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: pieceType);
            _pieceCounterList[index].DecrementCount();
        }


        // 必要があれば、次の駒をアニメーションで表示し、カウントを更新する
        public bool ShowNextHiddenPieceAndDecrementCountIfNeeded(EPieceType pieceType)
        {
            var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: pieceType);

            Assert.IsTrue(_pieceCounterList[index].Count != 0);


            _pieceCounterList[index].DecrementCount();

            if (_pieceCounterList[index].Count == 0)
            {
                return false;
            }
            else
            {
                // case Count >= 1
                PieceBehaviour pieceBehaviour = _boardSetBehaviour.GetNextHiddenPieceBehaviourOnWaitingSpace(playerType: _playerType, pieceType: pieceType);

                Assert.IsTrue(pieceBehaviour != null);

                pieceBehaviour.FadeIn();

                return true;
            }
        }



        // 棋譜再生の際、一度に駒数を削除する時に使用
        public void HideAllPieceCount()
        {
            for (int i = 0; i < 7; i++)
            {
                _pieceCounterList[i].SetZero_deprecated();
            }
        }



        // ===============================================
        // MARK: - ● for Debug ─────

        public void DebugPrintPieceCountList()
        {
            var index = 0;

            // 配列は LINQ の ForEach を利用できない
            foreach (var i in _pieceCounterList)
            {
                Debug.Log(index.ToString() + ": " + i.Count.ToString());
                index += 1;
            }
        }


        // ===============================================
        // MARK: - ● GetCountOfPiece ─────

        public int GetCountOfPiece(EPieceType pieceType)
        {
            var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: pieceType);
            return _pieceCounterList[index].Count;
        }



        // ===============================================
        // MARK: - ● Change Piece Count according to Diff Result ─────

        public void ChangeChangePieceCountForHistoryDetail(EPieceType pieceType, int newCount)
        {
                var index = Utl.ConvertPieceTypeToIndexOnWaitingSpace(pieceType: pieceType);
                var targetPieceCounter = _pieceCounterList[index];
                targetPieceCounter.SetCountForHistoryDetailStage(newCount: newCount);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        new void OnDestroy()
        {
            base.OnDestroy();

            _boardSetBehaviour = null;
            _bodySingleImageActionBehaviour = null;
            _shadowSingleImageActionBehaviour = null;
        }
    }
}

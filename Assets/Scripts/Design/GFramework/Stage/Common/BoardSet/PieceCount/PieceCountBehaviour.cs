using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



// 初期配置など、いきなり２となるばあいなどを考える必要がある
// Contup Count Down で自動的にアニメーションする


namespace GFramework
{
    public class PieceCountBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private BoardSetBehaviour _boardSetBehaviour = null;

        private EPlayerType _playerType;

        private int _presentIndex;
        public int PresentIndex => _presentIndex;

        private int _count = 0;
        public int Count => _count;

        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _pieceCountSingleImageActionBehaviour.Alpha;


        // -----------------------------------------------
        // MARK: - Game Object

        public GameObject GameObjectPieceCount => _pieceCountSingleImageActionBehaviour.gameObject;

        private SingleImageActionBehaviour _pieceCountSingleImageActionBehaviour;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(BoardSetBehaviour boardSetBehaviour, EPlayerType playerType, int index)
        {
            _boardSetBehaviour = boardSetBehaviour;
            _playerType = playerType;
            _presentIndex = index;
            transform.position = new Vector3(0f, 0f, 0f);

            SetupGameObject();
        }


        private void SetupGameObject()
        {
            SetupBody();
            SetupBaseImageSize();

            // CHECK: 次との違いは？
            SetLocalScale();

            // CHECK: 上との違いは？
            SetLocalScaleOfTestName(scale: new Vector2(1, 1));
        }


        private void SetupBaseImageSize()
        {
            _baseImageSize = Config.PIECE_COUNT__IMAGE_SIZE;
        }


        private void SetupBody()
        {
            // 便宜上 Blank 画像をロードしておく

            _pieceCountSingleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "PieceCountBody",
                texturePathUnderResources: _boardSetBehaviour.BoardImageSetManager.GetFilePathOfImagePieceCount(count: 0),
                willCacheIfNeeded: true,
                sortingOrder: Config.PIECE_COUNT__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.3f,
                    durationFadeOut: 0.3f)
                );


            // 見えなくしておく
            Hide();

            transform.localScale = _boardSetBehaviour.GetPieceCountImageScaleFromOriginalToOnBoard();

            Vector2 positionByPixel = _boardSetBehaviour.GetAreaPieceCount(playerType: _playerType, index: _presentIndex).CenterPoint;
            transform.position = Utl.ConvertImageSizeToUnit(imageSize: positionByPixel);
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositon(Vector2 unit)
        {
            Utl.SetLocalPosition(gameObject: _pieceCountSingleImageActionBehaviour.gameObject, unit: unit);
        }


        public void SetLocalPositonByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: _pieceCountSingleImageActionBehaviour.gameObject, baseImageSize: _baseImageSize, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Set Local Scale ─────

        private void SetLocalScale()
        {
            transform.localScale = _boardSetBehaviour.GetPieceCountImageScaleFromOriginalToOnBoard();
        }


        // 名称がかぶっている
        public void SetLocalScaleOfTestName(Vector2 scale)
        {
            _pieceCountSingleImageActionBehaviour.transform.localScale = scale * Config.PIECE_COUNT__SCALE_RATIO__DEFAULT;
        }



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            _pieceCountSingleImageActionBehaviour.SetAlpha(alpha: alpha);
        }


        public void SetAlphaZeroOfBodyAndShadow()
        {
            SetAlpha(alpha: 0);
        }



        // ===============================================
        // MARK: - ●  ─────

        public void SetCountOnGameBegin(int count)
        {
            _count = count;

            if (ShouldShow(count: count))
            {
                ChangeTexture(count: count);

                FadeIn();
            }
        }


        // ===============================================
        // MARK: - ● Validation  ─────

        private bool IsValidCount(int count)
        {
            return 0 <= count && count <= 12;
        }

        private bool CanIncrement()
        {
            return _count <= 11;
        }

        private bool CanDecrement()
        {
            return 1 <= _count;
        }

        private bool ShouldShow(int count)
        {
            return !ShouldHide(count: count);
        }

        private bool ShouldHide(int count)
        {
            return count == 0 || count == 1;
        }



        // ===============================================
        // MARK: - ●  ─────

        private void ChangeTexture(int count)
        {
            Assert.IsTrue(IsValidCount(count: count));

            string texturePathUnderResources = _boardSetBehaviour.BoardImageSetManager.GetFilePathOfImagePieceCount(count: count);
            UtlSprite.ChangeTexture(gameObject: _pieceCountSingleImageActionBehaviour.gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
        }


        public void IncrementCount()
        {
            Assert.IsTrue(IsValidCount(count: _presentIndex));
            Assert.IsTrue(CanIncrement());

            _count += 1;

            ChangeTexture(count: _count);

            // 1 -> 2 の際には表示する
            if (ShouldShow(count: _count) && _isShown == false)
            {
                FadeIn();
            }
            else if (_count >= 2)
            {
                RegisterAction(action: CAction.CreatePieceCountActionIncrement(pieceCountBehaviour: this, duration: 0.6f));
            }
        }


        public void DecrementCount()
        {
            Assert.IsTrue(IsValidCount(count: _presentIndex));
            Assert.IsTrue(CanDecrement());

            _count -= 1;

            // 2 -> 1 の際には非表示にする
            // TODO: これでは Change Texture が 1(非表示)になってから消えてしまう。アニメーション処理などが必要
            if (ShouldHide(count: _count) && _isShown)
            {
                if (_count == 0)
                {
                    // Do Nothing
                }
                else
                {
                    // case count == 1
                    FadeOut();
                }
            }
            else
            {
                // case count >= 2
                ChangeTexture(count: _count);

                RegisterAction(action: CAction.CreatePieceCountActionDecrement(pieceCountBehaviour: this, duration: 0.6f));
            }
        }


        // ===============================================
        // MARK: - ● for History Detail Stage ─────

        // 棋譜再現の際に全てをゼロに設定する場合に使用
        public void SetZero_deprecated()
        {
            _count = 0;
            // テクスチャは変更しない。zeroがないから。次に表示する際に設定すること
            Hide();
        }

        public void SetCountForHistoryDetailStage(int newCount)
        {
            if (_count == newCount) { return; }

            _count = newCount;

            if (ShouldShow(count: newCount))
            {
                ChangeTexture(count: newCount);

                Show();
            }
            else
            {
                Hide();
            }
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; }

            _pieceCountSingleImageActionBehaviour.Show();

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; }

            _pieceCountSingleImageActionBehaviour.Hide();

            _isShown = false;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; }

            _pieceCountSingleImageActionBehaviour.FadeIn();

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; }

            _pieceCountSingleImageActionBehaviour.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _boardSetBehaviour = null;
            _pieceCountSingleImageActionBehaviour = null;
        }
    }
}
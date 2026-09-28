using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

// TODO: 事前にテクスチャをキャッシュする
// TODO: テクスチャの参照を直接持つ

// SingleImageActionBehaviour を継承したい

namespace GFramework
{
    public readonly struct FFrameAnimationInitializer
    {
        public FSingleImageActionBehaviourInitializer SingleImageActionBehaviourInitializer { get; }
        public string TexturePathUnderResourcesBase { get; }
        public CCounter AnimationCounter { get; }
        public int AnimationWait { get; }
        public int SortingOrder { get; }
        public bool IsAnimationOn { get; }
        public bool IsShown { get; }

        public FFrameAnimationInitializer(
            FSingleImageActionBehaviourInitializer singleImageActionBehaviourInitializer,
            string texturePathUnderResourcesBase,
            CCounter animationCounter,
            int animationWait,
            int sortingOrder,
            bool isAnimationOn,
            bool isShown)
        {
            SingleImageActionBehaviourInitializer = singleImageActionBehaviourInitializer;
            TexturePathUnderResourcesBase = texturePathUnderResourcesBase;
            AnimationCounter = animationCounter; // 必要に応じて .Copy を呼び出す
            AnimationWait = animationWait;
            SortingOrder = sortingOrder;
            IsAnimationOn = isAnimationOn;
            IsShown = isShown;
        }
    }


    public class FrameAnimationBehaviour : SingleImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private CCounter _animationCounter;

        private int _animationWait;
        public int AnimationWait => _animationWait;

        private int _animationWaitCount = 0;

        private bool _isAnimationOn = true;
        public bool IsAnimationOn => _isAnimationOn;

        /// テクスチャのパス
        /// TODO: 将来的にはテクスチャへの参照を直接持ちたい
        public List<string> texturePathUnderResourcesList = new();


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(FFrameAnimationInitializer frameAnimationInitializer)
        {
            _animationCounter = frameAnimationInitializer.AnimationCounter;

            Assert.IsTrue(_animationCounter.Max >= 1);

            _animationWait = frameAnimationInitializer.AnimationWait;

            SetupTexturePathUnderResourcesList(texturePathUnderResourcesBase: frameAnimationInitializer.TexturePathUnderResourcesBase);

            UtlSprite.AddSpriteToGameObject(
                gameObject: gameObject,
                texturePathUnderResources: texturePathUnderResourcesList[_animationCounter.Count],
                willCacheIfNeeded: true,
                sortingOrder: frameAnimationInitializer.SortingOrder);

            SetupComponent(initializer: frameAnimationInitializer.SingleImageActionBehaviourInitializer);

            SetIsAnimationOn(value: frameAnimationInitializer.IsAnimationOn);

            if (frameAnimationInitializer.IsShown)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }


        private void SetupTexturePathUnderResourcesList(string texturePathUnderResourcesBase)
        {
            for (int i = 0; i <= _animationCounter.Max; i++)
            {
                texturePathUnderResourcesList.Add($"{texturePathUnderResourcesBase}{i}.png");
            }
        }



        // ===============================================
        // MARK: - ● Validation ─────

        private bool IsValidIndex(int index)
        {
            return (0 <= index && index <= _animationCounter.Max);
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void ShowAndOn()
        {
            Show();
            SetIsAnimationOn(value: true);
        }


        public void HideAndOff()
        {
            Hide();
            SetIsAnimationOn(value: false);
        }



        // ===============================================
        // MARK: - ● SetIsAnimationOn ─────

        public void SetIsAnimationOn(bool value)
        {
            _isAnimationOn = value;
        }


        // ===============================================
        // MARK: - ● Change Animation Wait ─────

        public void ChangeAnimationWait(int animationWait)
        {
            _animationWait = animationWait;
        }


        // ===============================================
        // MARK: - ● Update ─────

        public new void Update()
        {
            // これを記述しないと Action が実行されない。
            base.Update();

            if (_isAnimationOn)
            {
                // アニメーション中に animationWait が変更される可能性があるため、>= で判定する
                if (_animationWaitCount >= _animationWait)
                {
                    UpdateAnimation();

                    _animationWaitCount = 0;
                }
                else
                {
                    _animationWaitCount += 1;
                }
            }
        }


        private void UpdateAnimation()
        {
            _animationCounter.Up();

            UtlSprite.ChangeTexture(
                gameObject: gameObject,
                texturePathUnderResources: texturePathUnderResourcesList[_animationCounter.Count],
                willCacheIfNeeded: true);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();
            _animationCounter = null;

            texturePathUnderResourcesList.Clear();
        }
    }
}
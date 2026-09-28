using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public enum ETapAndSwipeInputType
    {
        POINTER_DOWN,
        POINTER_UP,
        POINTER_SWIPE
    }


    /// <summary>
    /// タップとスワイプを取得するBehaviour
    /// </summary>
    public class TapAndSwipeInputBehaviour : AInputBehaviour
    {
        // その場であげる→タップ
        // スワイプを一度でもすると、その場に戻ってもタップ判定はしない

        // ===============================================
        // MARK: - ● Property ─────

        // for Action
        private Action<ETapAndSwipeInputType, Vector2> _actionOnPointerDown;
        private Action<ETapAndSwipeInputType, Vector2> _actionOnPointerUp;
        private Action<ETapAndSwipeInputType, Vector2> _actionOnPointerSwipe; // 動いた時に発生。動かして止まっている時には発生しない。

        // state

        // ポインターが押された状態か
        private bool _isKeepingPointerDown = false;


        // position
        private Vector2 _lastExistingPosition = new Vector2(0, 0);



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(
            Vector2 size,
            Vector2 position,
            bool isEnabled,
            Action<ETapAndSwipeInputType, Vector2> actionOnPointerDown = null,
            Action<ETapAndSwipeInputType, Vector2> actionOnPointerUp = null,
            Action<ETapAndSwipeInputType, Vector2> actionOnPointerSwipe = null)
        {
            _actionOnPointerDown = actionOnPointerDown;
            _actionOnPointerUp = actionOnPointerUp;
            _actionOnPointerSwipe = actionOnPointerSwipe;

            transform.localScale = new Vector2(1f, 1f);


            SetupBase(size: size, position: position, isEnabled: isEnabled);
        }



        // ===============================================
        // MARK: - ● Update ─────

        // Update is called once per frame
        void Update()
        {
            if (_isKeepingPointerDown == false) { return; }


            var presentPosition = Utl.GetTapPosition();

            if (presentPosition != _lastExistingPosition)
            {
                _lastExistingPosition = presentPosition;

                _actionOnPointerSwipe?.Invoke(ETapAndSwipeInputType.POINTER_SWIPE, _lastExistingPosition);
            }
        }


        // ===============================================
        // MARK: - ● Main On Event ─────

        protected override void MainOnPointerDown()
        {
            _isKeepingPointerDown = true;
            _lastExistingPosition = Utl.GetTapPosition();

            _actionOnPointerDown?.Invoke(ETapAndSwipeInputType.POINTER_DOWN, _lastExistingPosition);
        }


        protected override void MainOnPointerUp()
        {
            // 強制ストップがかけられている場合、UPは発生しない。
            if (_isKeepingPointerDown == false) { return; }


            _isKeepingPointerDown = false;

            _actionOnPointerUp?.Invoke(ETapAndSwipeInputType.POINTER_UP, _lastExistingPosition);
        }


        protected override void MainOnPointerClick()
        {
            // Do Nothing
        }

        protected override void MainOnPointerEnter()
        {
            // Do Nothing
        }

        protected override void MainOnPointerExit()
        {
            // Do Nothing
        }



        // ===============================================
        // MARK: - ● Set Action ─────

        public void SetActionOnPointerDown(Action<ETapAndSwipeInputType, Vector2> action)
        {
            _actionOnPointerDown = action;
        }

        public void SetActionOnPointerUp(Action<ETapAndSwipeInputType, Vector2> action)
        {
            _actionOnPointerUp = action;
        }


        public void SetActionOnPointerSwipe(Action<ETapAndSwipeInputType, Vector2> action)
        {
            _actionOnPointerSwipe = action;
        }



        // ===============================================
        // MARK: - ● Force Stop ─────

        // 強制的にストップをかける
        // → この場合、再度 pointerDown が発生しないと解除されない = 再度タップが必要
        public void ForceStop()
        {
            _isKeepingPointerDown = false;
        }


        // ===============================================
        // MARK: - ● On Destroy ─────

        public void OnDestroy()
        {
            ReleaseReference();
        }


        private void ReleaseReference()
        {
            _actionOnPointerDown = null;
            _actionOnPointerUp = null;
            _actionOnPointerSwipe = null;
        }
    }
}
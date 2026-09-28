using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public enum ETapInputType
    {
        POINTER_DOWN,
        POINTER_UP,
        POINTER_CLICK,
        POINTER_ENTER,
        POINTER_EXIT
    }



    public class TapInputBehaviour : AInputBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        // for Action
        private Action<ETapInputType, int> _actionOnPointerDown;
        private Action<ETapInputType, int> _actionOnPointerUp;
        private Action<ETapInputType, int> _actionOnPointerClick;
        private Action<ETapInputType, int> _actionOnPointerEnter;
        private Action<ETapInputType, int> _actionOnPointerExit;

        // count

        // フレームカウントが必要かどうか
        private bool _sholdCountFrame = false;

        // フレームカウント
        private int _frameCount = 0;


        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(
            Vector2 size,
            Vector2 position,
            bool isEnabled,
            Action<ETapInputType, int> actionOnPointerDown = null,
            Action<ETapInputType, int> actionOnPointerUp = null,
            Action<ETapInputType, int> actionOnPointerClick = null,
            Action<ETapInputType, int> actionOnPointerEnter = null,
            Action<ETapInputType, int> actionOnPointerExit = null)
        {
            _actionOnPointerDown = actionOnPointerDown;
            _actionOnPointerUp = actionOnPointerUp;
            _actionOnPointerClick = actionOnPointerClick;
            _actionOnPointerEnter = actionOnPointerEnter;
            _actionOnPointerExit = actionOnPointerExit;

            transform.localScale = new Vector2(1f, 1f);


            SetupBase(size: size, position: position, isEnabled: isEnabled);
        }


        public void SetupComponentForFullScreen(
            bool isEnabled,
            Action<ETapInputType, int> actionOnPointerDown = null,
            Action<ETapInputType, int> actionOnPointerClick = null)
        {
            var area = Locator.Instance.layoutManager.AreaUICanvas;

            SetupComponent(
            size: area.size,
            position: area.CenterPoint,
            isEnabled: isEnabled,
            actionOnPointerDown: actionOnPointerDown,
            actionOnPointerUp: null,
            actionOnPointerClick: actionOnPointerClick,
            actionOnPointerEnter: null,
            actionOnPointerExit: null);
        }


        // ===============================================
        // MARK: - ● Update ─────

        // Update is called once per frame
        void Update()
        {
            if (_sholdCountFrame)
            {
                if (_frameCount != int.MaxValue)
                {
                    _frameCount++;
                }
            }
        }



        // ===============================================
        // MARK: - ● Count ─────

        private void SetShouldCountFrame()
        {
            _sholdCountFrame = true;
        }


        private void SetIsNotCountFrameAndResetFrameCount()
        {
            _sholdCountFrame = false;
            _frameCount = 0;
        }



        // ===============================================
        // MARK: - ● Main On Event ─────

        protected override void MainOnPointerDown()
        {
            _actionOnPointerDown?.Invoke(ETapInputType.POINTER_DOWN, _frameCount);

            SetShouldCountFrame();
        }

        protected override void MainOnPointerUp()
        {
            _actionOnPointerUp?.Invoke(ETapInputType.POINTER_UP, _frameCount);

            SetIsNotCountFrameAndResetFrameCount();
        }

        protected override void MainOnPointerClick()
        {
            _actionOnPointerClick?.Invoke(ETapInputType.POINTER_CLICK, _frameCount);

            SetIsNotCountFrameAndResetFrameCount();
        }

        protected override void MainOnPointerEnter()
        {
            _actionOnPointerEnter?.Invoke(ETapInputType.POINTER_ENTER, _frameCount);
        }

        protected override void MainOnPointerExit()
        {
            _actionOnPointerExit?.Invoke(ETapInputType.POINTER_EXIT, _frameCount);

            SetIsNotCountFrameAndResetFrameCount();
        }



        // ===============================================
        // MARK: - ● Set Action ─────

        public void SetActionOnPointerDown(Action<ETapInputType, int> action)
        {
            _actionOnPointerDown = action;
        }

        public void SetActionOnPointerUp(Action<ETapInputType, int> action)
        {
            _actionOnPointerUp = action;
        }

        public void SetActionOnPointerClick(Action<ETapInputType, int> action)
        {
            _actionOnPointerClick = action;
        }

        public void SetActionOnPointerEnter(Action<ETapInputType, int> action)
        {
            _actionOnPointerEnter = action;
        }

        public void SetActionOnPointerExit(Action<ETapInputType, int> action)
        {
            _actionOnPointerExit = action;
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
            _actionOnPointerClick = null;
            _actionOnPointerEnter = null;
            _actionOnPointerExit = null;
        }
    }
}
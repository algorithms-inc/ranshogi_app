using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class WaitingSpaceInputBehaviour : AInputBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private BoardSetBehaviour _boardSetBehaviour = null;
        
        private int _index;

        // for Action
        private Action<int> _actionOnPointerDown;
        private Action<int> _actionOnPointerUp;
        private Action<int> _actionOnPointerClick;
        private Action<int> _actionOnPointerEnter;
        private Action<int> _actionOnPointerExit;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(
            BoardSetBehaviour boardSetBehaviour,
            int index,
            bool isEnabled,
            Action<int> actionOnPointerDown = null,
            Action<int> actionOnPointerUp = null,
            Action<int> actionOnPointerClick = null,
            Action<int> actionOnPointerEnter = null,
            Action<int> actionOnPointerExit = null,
            EPlayerType playerType = EPlayerType.PLAYER1)
        {
            _boardSetBehaviour = boardSetBehaviour;

            _index = index;

            _actionOnPointerDown = actionOnPointerDown;
            _actionOnPointerUp = actionOnPointerUp;
            _actionOnPointerClick = actionOnPointerClick;
            _actionOnPointerEnter = actionOnPointerEnter;
            _actionOnPointerExit = actionOnPointerExit;

            transform.localScale = new Vector2(1f, 1f);

            FArea area = _boardSetBehaviour.GetAreaWaitingSpaceCell(playerType: playerType, index: index);
            Vector2 size = area.size;
            Vector2 position = area.CenterPoint;

            SetupBase(size: size, position: position, isEnabled: isEnabled);
        }



        // ===============================================
        // MARK: - ● Main On Event ─────

        protected override void MainOnPointerDown()
        {
            _actionOnPointerDown?.Invoke(_index);
        }

        protected override void MainOnPointerUp()
        {
            _actionOnPointerUp?.Invoke(_index);
        }

        protected override void MainOnPointerClick()
        {
            _actionOnPointerClick?.Invoke(_index);
        }

        protected override void MainOnPointerEnter()
        {
            _actionOnPointerEnter?.Invoke(_index);
        }

        protected override void MainOnPointerExit()
        {
            _actionOnPointerExit?.Invoke(_index);
        }



        // ===============================================
        // MARK: - ● Set Action ─────

        public void SetActionOnPointerDown(Action<int> actionOnPointerDown)
        {
            _actionOnPointerDown = actionOnPointerDown;
        }

        public void SetActionOnPointerUp(Action<int> actionOnPointerUp)
        {
            _actionOnPointerUp = actionOnPointerUp;
        }

        public void SetActionOnPointerClick(Action<int> actionOnPointerClick)
        {
            _actionOnPointerClick = actionOnPointerClick;
        }

        public void SetActionOnPointerEnter(Action<int> actionOnPointerEnter)
        {
            _actionOnPointerEnter = actionOnPointerEnter;
        }

        public void SetActionOnPointerExit(Action<int> actionOnPointerExit)
        {
            _actionOnPointerExit = actionOnPointerExit;
        }


        public void OnDestroy()
        {
            _boardSetBehaviour = null;
        }
    }
}
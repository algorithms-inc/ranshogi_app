using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class BoardInputBehaviour : AInputBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private BoardSetBehaviour _boardSetBehaviour = null;

        private FBoardCrd _boardCrd;

        // for Action
        private Action<FBoardCrd> _actionOnPointerDown;
        private Action<FBoardCrd> _actionOnPointerUp;
        private Action<FBoardCrd> _actionOnPointerClick;
        private Action<FBoardCrd> _actionOnPointerEnter;
        private Action<FBoardCrd> _actionOnPointerExit;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(
            BoardSetBehaviour boardSetBehaviour,
            FBoardCrd boardCrd,
            bool isEnabled,
            Action<FBoardCrd> actionOnPointerDown  = null,
            Action<FBoardCrd> actionOnPointerUp    = null,
            Action<FBoardCrd> actionOnPointerClick = null,
            Action<FBoardCrd> actionOnPointerEnter = null,
            Action<FBoardCrd> actionOnPointerExit  = null)
        {
            _boardSetBehaviour = boardSetBehaviour;

            _boardCrd = boardCrd;

            _actionOnPointerDown  = actionOnPointerDown;
            _actionOnPointerUp    = actionOnPointerUp;
            _actionOnPointerClick = actionOnPointerClick;
            _actionOnPointerEnter = actionOnPointerEnter;
            _actionOnPointerExit  = actionOnPointerExit;

            transform.localScale = new Vector2(1f, 1f);

            FArea area = _boardSetBehaviour.GetAreaBoardCell(boardCrd: boardCrd);
            Vector2 size = area.size;
            Vector2 position = area.CenterPoint;


            SetupBase(size: size, position: position, isEnabled: isEnabled);
        }



        // ===============================================
        // MARK: - ● Main On Event ─────

        protected override void MainOnPointerDown()
        {
            _actionOnPointerDown?.Invoke(_boardCrd);
        }

        protected override void MainOnPointerUp()
        {
            _actionOnPointerUp?.Invoke(_boardCrd);
        }

        protected override void MainOnPointerClick()
        {
            _actionOnPointerClick?.Invoke(_boardCrd);
        }

        protected override void MainOnPointerEnter()
        {
            _actionOnPointerEnter?.Invoke(_boardCrd);
        }

        protected override void MainOnPointerExit()
        {
            _actionOnPointerExit?.Invoke(_boardCrd);
        }



        // ===============================================
        // MARK: - ● Set Action ─────

        public void SetActionOnPointerDown(Action<FBoardCrd> actionOnPointerDown)
        {
            _actionOnPointerDown = actionOnPointerDown;
        }

        public void SetActionOnPointerUp(Action<FBoardCrd> actionOnPointerUp)
        {
            _actionOnPointerUp = actionOnPointerUp;
        }

        public void SetActionOnPointerClick(Action<FBoardCrd> actionOnPointerClick)
        {
            _actionOnPointerClick = actionOnPointerClick;
        }

        public void SetActionOnPointerEnter(Action<FBoardCrd> actionOnPointerEnter)
        {
            _actionOnPointerEnter = actionOnPointerEnter;
        }

        public void SetActionOnPointerExit(Action<FBoardCrd> actionOnPointerExit)
        {
            _actionOnPointerExit = actionOnPointerExit;
        }


        // ===============================================
        // MARK: - ● On Destroy ─────

        public void OnDestroy()
        {
            ReleaseReference();
        }


        private void ReleaseReference()
        {
            _boardSetBehaviour = null;
            _actionOnPointerDown = null;
            _actionOnPointerUp = null;
            _actionOnPointerClick = null;
            _actionOnPointerEnter = null;
            _actionOnPointerExit = null;
        }
    }
}
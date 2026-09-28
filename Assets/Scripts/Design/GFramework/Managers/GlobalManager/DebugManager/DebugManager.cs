using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEditor;
using UnityEngine.Assertions;


namespace GFramework
{
    public sealed class DebugManager : IDebugManager
    {
        private EBoardType _boardType = EBoardType.BOARD_66;
        public EBoardType BoardType
        {
            get => _boardType;
            set { _boardType = value;}
        }

        private bool _isWithFriendMode = false;
        public bool IsWithFriendMode
        {
            get => _isWithFriendMode;
            set { _isWithFriendMode = value;}
        }

        private bool _isQuickFinishMatchMode = false;
        public bool IsQuickFinishMatchMode
        {
            get => _isQuickFinishMatchMode;
            set { _isQuickFinishMatchMode = value;}
        }

        public DebugManager()
        {
            Assert.IsTrue(AppManager.Instance.IsDebugMode);
        }



        public void Destroy()
        {
            // do nothing
        }
    }
}
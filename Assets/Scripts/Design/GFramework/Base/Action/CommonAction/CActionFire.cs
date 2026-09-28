using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CActionFire : CAction
    {
        private Action _action;


        public CActionFire(ActionBehaviour actionBehaviour, float duration, Action action) : base(actionBehaviour)
        {
            _duration = duration;
            _action = action;
        }


        protected override void ProcessOnFirstFrame()
        {
            // do nothind
        }


        protected override void ProcessMain()
        {
            // do nothind
        }


        protected override void ProcessOnLastFrame()
        {
            _action.Invoke();

            _action = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

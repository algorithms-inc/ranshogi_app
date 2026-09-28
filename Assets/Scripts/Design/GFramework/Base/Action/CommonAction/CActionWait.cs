using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class CActionWait : CAction
    {
        public CActionWait(ActionBehaviour actionBehaviour, float duration) : base(actionBehaviour)
        {
            _duration = duration;
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
            // do nothing
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

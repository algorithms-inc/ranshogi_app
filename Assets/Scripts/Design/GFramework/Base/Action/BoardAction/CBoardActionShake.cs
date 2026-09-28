using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO: 秒間揺れ回数を決める必要がある。そこから一定の周期で揺れるようにする。あるいは sine カーブなどで周期を描くようにするといい？
// TODO: イージングをかけて、最後は収束するようにしたい
// TODO: 駒も必要かも。その場合、CommonAction にできるかも

namespace GFramework
{
    public class CBoardActionShake : CAction
    {
        private BoardBehaviour _boardBehaviour;
        private Vector2 _magnitudeRatio;
        private Vector2 _originalPosition;


        public CBoardActionShake(
            BoardBehaviour boardBehaviour,
            Vector2 magnitudeRatio, float duration) : base(boardBehaviour)
        {
            _duration = duration;
            _magnitudeRatio = magnitudeRatio;
        }


        protected override void ProcessOnFirstFrame()
        {
            _boardBehaviour = _actionBehaviour as BoardBehaviour;
            Assert.IsTrue(_boardBehaviour != null);

            _originalPosition = _boardBehaviour.transform.position;
        }


        protected override void ProcessMain()
        {
            float elapsedTimeAlpha = GetElapsedTimeAlpha();

            MoveBody(elapsedTimeAlpha: elapsedTimeAlpha);
        }


        private void MoveBody(float elapsedTimeAlpha) =>
            _boardBehaviour.SetLocalPositonOfWholeBoardByRatio(
                ratio: new(
                    x: Random.Range(_magnitudeRatio.x * -1, _magnitudeRatio.x),
                    y: Random.Range(_magnitudeRatio.y * -1, _magnitudeRatio.y)));


        protected override void ProcessOnLastFrame()
        {
            _boardBehaviour.transform.position = _originalPosition;

            _boardBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}
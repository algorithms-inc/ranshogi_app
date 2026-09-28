using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CAudioActionFade : CAction
    {
        // ===============================================
        // MARK: - ● Property ─────

        private AudioActionBehaviour _audioActionBehaviour;
        private readonly float _alphaTo;

        private float _alphaFrom = 0.0f;
        private readonly EVolumeType _volumeType;



        // ===============================================
        // MARK: - ● Constructor ─────

        public CAudioActionFade(
            AudioActionBehaviour audioActionBehaviour,
            float alphaTo,
            float duration) : base(audioActionBehaviour)
        {
            Assert.IsTrue(0.0f <= alphaTo);
            Assert.IsTrue(alphaTo <= 1.0f);

            _audioActionBehaviour = audioActionBehaviour;
            _alphaTo = alphaTo;
            _duration = duration;

            _volumeType = _audioActionBehaviour.AudioActionBehaviourType switch
            {
                EAudioActionBehaviourType.BGM => EVolumeType.VOLUME_BGM,
                EAudioActionBehaviourType.JIN => EVolumeType.VOLUME_JIN,
                EAudioActionBehaviourType.SE => EVolumeType.VOLUME_SE,
                _ => throw new InvalidOperationException(),
            };
        }



        // ===============================================
        // MARK: - ● Method ─────

        protected override void ProcessOnFirstFrame() =>
            _alphaFrom = Locator.Instance.audioManager.GetVolumeForSystem(_volumeType);


        protected override void ProcessMain() =>
            Locator.Instance.audioManager.SetVolumeForSystem(
                _volumeType,
                (_alphaTo - _alphaFrom) * GetElapsedTimeAlpha() + _alphaFrom);


        protected override void ProcessOnLastFrame()
        {
            // 音量を目標の値にしておく
            Locator.Instance.audioManager.SetVolumeForSystem(volumeType: _volumeType, value: _alphaTo);

            _audioActionBehaviour = null;
        }


        public override void ForceFinishAction()
        {
            SetElapsedTimeMax();
            Update();
        }
    }
}

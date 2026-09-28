using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    // TODO: プリロード
    // TODO: コールバックがほしい

    /// <summary>
    /// オーディオ・マネジャのインターフェース
    /// </summary>
    public interface IAudioManager : IAudioController
    {
        AudioClip LoadAudioClip(string filenameWithoutExtension);

        void PlayDefaultStageBGM();
        void Play(EJINType JINType);
        void Play(ESEType SEType);

        void SetIsMuteForUser(EVolumeType volumeType, bool value);
        void SetIsMuteForSystem(EVolumeType volumeType, bool value);

        void SetVolumeForSystem(EVolumeType volumeType, float value);
        void SetVolumeForUser(EVolumeType volumeType, float value);

        bool GetIsMuteForSystem(EVolumeType volumeType);
        bool GetIsMuteForUser(EVolumeType volumeType);

        float GetVolumeForSystem(EVolumeType volumeType);
        float GetVolumeForUser(EVolumeType volumeType);

        float CalcVolume(float childValue);


        void FadeBGM(float alphaTo, float duration);
        void FadeOutBGMIfNeeded(EStageType nextStageType, float duration);

        void Destroy();
    }
}
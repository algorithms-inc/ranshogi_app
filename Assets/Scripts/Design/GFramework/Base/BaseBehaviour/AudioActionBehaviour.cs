using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace GFramework
{
    public enum EAudioActionBehaviourType
    {
        BGM, // loop = true  , 重複不可(同じ曲なら成り続ける) , 成り終えることはない     , Stop / FadeStop 可能 , FadeStop のAction可能
        JIN, // loop = false , 重複不可(同じ曲なら成り続ける) , 成り終えた後のAction可能 , Stop / FadeStop 不可
        SE,  // loop = false , 重複可能                       , 成り終えた後のAction不可 , Stop / FadeStop 不可
    }


    public class AudioActionBehaviour : ActionBehaviour
    {

        // ===============================================
        // MARK: - ● Property ─────

        private EAudioActionBehaviourType _audioActionBehaviourType = EAudioActionBehaviourType.BGM; // 便宜上設定
        public EAudioActionBehaviourType AudioActionBehaviourType => _audioActionBehaviourType;

        private AudioSource _audioSource;

        private FVolumeSetting _volumeSetting = new(
            isMuteForSystem: false,
            isMuteForUser: false,
            volumeForSystem: 1.0f,
            volumeForUser: 1.0f);

        public bool IsMuteForSystem => _volumeSetting.IsMuteForSystem;
        public bool IsMuteForUser => _volumeSetting.IsMuteForUser;

        public float VolumeForSystem => _volumeSetting.VolumeForSystem;
        public float VolumeForUser => _volumeSetting.VolumeForUser;

        public bool IsPlaying => _audioSource.isPlaying;
        public bool IsLoop
        {
            set { _audioSource.loop = value; }
            get { return _audioSource.loop; }
        }

        private string _fileNameWithoutExtension;



        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(EAudioActionBehaviourType audioActionBehaviourType)
        {
            _audioActionBehaviourType = audioActionBehaviourType;

            _audioSource = gameObject.AddComponent<AudioSource>();
        }



        // ===============================================
        // MARK: - ● Method ─────

        public void SetIsMuteForSystem(bool value)
        {
            _volumeSetting.IsMuteForSystem = value;
            UpdateVolume();
        }

        public void SetVolumeForSystem(float value)
        {
            _volumeSetting.VolumeForSystem = value;
            UpdateVolume();
        }

        public void SetIsMuteForUser(bool value)
        {
            _volumeSetting.IsMuteForUser = value;
            UpdateVolume();
        }

        public void SetVolumeForUser(float value)
        {
            _volumeSetting.VolumeForUser = value;
            UpdateVolume();
        }



        public void Play(string fileNameWithoutExtension)
        {
            // AudioSource.Play()：同時再生不可。再生中に再度Playされると前のものは終了する
            // AudioSource.PlayOneShot(audoClip)：重複再生可能。効果音むき

            switch (_audioActionBehaviourType)
            {
                case EAudioActionBehaviourType.BGM:
                    if ((_fileNameWithoutExtension == fileNameWithoutExtension) && (IsPlaying)) { return; }

                    _audioSource.clip = LoadAudioClip(fileNameWithoutExtension: fileNameWithoutExtension);
                    _audioSource.volume = CalcVolume();
                    _audioSource.loop = true;
                    _audioSource.spatialBlend = 0f;
                    _audioSource.Play();

                    return;

                case EAudioActionBehaviourType.JIN:
                    if ((_fileNameWithoutExtension == fileNameWithoutExtension) && (IsPlaying)) { return; }

                    _audioSource.clip = LoadAudioClip(fileNameWithoutExtension: fileNameWithoutExtension);
                    _audioSource.volume = CalcVolume();
                    _audioSource.loop = false;
                    _audioSource.spatialBlend = 0f;
                    _audioSource.Play();

                    return;

                case EAudioActionBehaviourType.SE:

                    _audioSource.volume = CalcVolume();
                    _audioSource.loop = false;
                    _audioSource.spatialBlend = 0f;
                    _audioSource.PlayOneShot(LoadAudioClip(fileNameWithoutExtension: fileNameWithoutExtension));

                    return;

                default:
                    throw new InvalidOperationException();
            }
        }



        private AudioClip LoadAudioClip(string fileNameWithoutExtension)
        {
            _fileNameWithoutExtension = fileNameWithoutExtension;

            return Locator.Instance.audioManager.LoadAudioClip(filenameWithoutExtension: fileNameWithoutExtension);
        }


        public bool IsPlayingSameAudio(string fileNameWithoutExtension) =>
            IsPlaying && fileNameWithoutExtension == _fileNameWithoutExtension;


        public void PlayWithAction(string fileNameWithoutExtension)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void FadeAndStop(float duration, Action actionOnFinish = null)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void UnPause()
        {
            throw new NotImplementedException();
        }


        private float CalcVolume() =>
            Locator.Instance.audioManager.CalcVolume(childValue: _volumeSetting.Volume);


        public void UpdateVolume() => _audioSource.volume = CalcVolume();
    }
}
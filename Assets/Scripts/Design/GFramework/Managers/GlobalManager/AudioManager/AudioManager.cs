using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    public enum EBGMType
    {
        MENU = 0,
        GAME = 1,
    }

    public enum EBGMSourceType
    {
        BGM_00 = 0,
        BGM_01 = 1,
    }

    public enum EJINType
    {
        WIN = 0,
        LOSE = 1
    }

    public enum EJINSourceType
    {
        JIN_00 = 0,
        JIN_01 = 1,
    }

    public enum ESEType
    {
        TAP_NORMAL = 0,
        TAP_NEGATIVE = 1,
        PIECE_SELECT = 2,
        PIECE_DESELECT = 3,
        PIECE_PLACE = 4,
        PIECE_GET = 5,
        CHECKBOX_ON = 6,
        CHECKBOX_OFF = 7,
        SLIDER_CHANGE_VALUE = 8,
        GAME_STAGE_OPPONENT_FOUND = 9,
    }

    public enum ESESourceType
    {
        SE_00 = 0,
        SE_01 = 1,
        SE_02 = 2,
        SE_03 = 3,
        SE_04 = 4,
        SE_05 = 5,
        SE_06 = 6,
        SE_07 = 7,
        SE_08 = 8,
        SE_09 = 9,
        SE_10 = 10,
        SE_11 = 11,
        SE_12 = 12,
        SE_13 = 13,
        SE_14 = 14,
        SE_15 = 15,
        SE_16 = 16,
        SE_17 = 17,
        SE_18 = 18,
        SE_19 = 19,
        SE_20 = 20,
        SE_21 = 21,
        SE_22 = 22,
        SE_23 = 23,
        SE_24 = 24,
        SE_25 = 25,
        SE_26 = 26,
        SE_27 = 27,
        SE_28 = 28,
        SE_29 = 29,
        SE_30 = 30,
        SE_31 = 31,
        SE_32 = 32,
        SE_33 = 33,
        SE_34 = 34,
        SE_35 = 35,
        SE_36 = 36,
        SE_37 = 37,
        SE_38 = 38,
        SE_39 = 39,
        SE_40 = 40,
        SE_41 = 41,
        SE_42 = 42,
        SE_43 = 43,
    }
}


namespace GFramework
{
    public enum EVolumeType
    {
        VOLUME_MASTER,
        VOLUME_BGM,
        VOLUME_JIN,
        VOLUME_SE
    }

    public enum EAudioType
    {
        BGM,
        JIN,
        SE
    }


    public sealed class AudioManager : IAudioManager
    {
        // GameObject
        private GameObject _gameObjectAudioPlayer;

        // AudioActionBehaviour
        private AudioActionBehaviour _audioActionBehaviourBGM;
        private AudioActionBehaviour _audioActionBehaviourJIN;
        private AudioActionBehaviour _audioActionBehaviourSE;

        // Mute: Master
        private bool _isMuteMasterForSystem = false;
        private bool _isMuteMasterForUser = false;
        public bool IsMuteMaster => _isMuteMasterForSystem || _isMuteMasterForUser;

        // Volume: Master
        private float _volumeMasterForSystem = 1.0f;
        private float _volumeMasterForUser = 1.0f;
        public float VolumeMaster => _volumeMasterForSystem * _volumeMasterForUser;

        private AudioBinder _audioBinder = new AudioBinder();
        private AudioClipStorage _audioClipStorage = new AudioClipStorage();



        // ===============================================
        // MARK: - ● Constructor ─────

        public AudioManager()
        {
            _gameObjectAudioPlayer = new GameObject("AudioPlayer");

            Utl.SetDontDestroyOnLoad(targetObject: _gameObjectAudioPlayer);

            _audioActionBehaviourBGM = _gameObjectAudioPlayer.AddComponent<AudioActionBehaviour>();
            _audioActionBehaviourBGM.SetupComponent(audioActionBehaviourType: EAudioActionBehaviourType.BGM);

            _audioActionBehaviourJIN = _gameObjectAudioPlayer.AddComponent<AudioActionBehaviour>();
            _audioActionBehaviourJIN.SetupComponent(audioActionBehaviourType: EAudioActionBehaviourType.JIN);

            _audioActionBehaviourSE = _gameObjectAudioPlayer.AddComponent<AudioActionBehaviour>();
            _audioActionBehaviourSE.SetupComponent(audioActionBehaviourType: EAudioActionBehaviourType.SE);

            SetupAudioBinder();
        }


        private void SetupAudioBinder()
        {
            // for BGM
            _audioBinder.SetSoruceType(BGMType: EBGMType.MENU, BGMSourceType: EBGMSourceType.BGM_00);
            _audioBinder.SetSoruceType(BGMType: EBGMType.GAME, BGMSourceType: EBGMSourceType.BGM_01);

            _audioBinder.SetFilePath(BGMSourceType: EBGMSourceType.BGM_00, filePathWithoutExtension: "audiostock_146015");
            _audioBinder.SetFilePath(BGMSourceType: EBGMSourceType.BGM_01, filePathWithoutExtension: "audiostock_169316");

            // for JIN
            _audioBinder.SetSoruceType(JINType: EJINType.WIN, JINSourceType: EJINSourceType.JIN_00);
            _audioBinder.SetSoruceType(JINType: EJINType.LOSE, JINSourceType: EJINSourceType.JIN_01);

            _audioBinder.SetFilePath(JINSourceType: EJINSourceType.JIN_00, filePathWithoutExtension: "audiostock_1178725");
            _audioBinder.SetFilePath(JINSourceType: EJINSourceType.JIN_01, filePathWithoutExtension: "audiostock_990251");

            // for SE
            _audioBinder.SetSoruceType(SEType: ESEType.TAP_NORMAL, SESourceType: ESESourceType.SE_00);
            _audioBinder.SetSoruceType(SEType: ESEType.TAP_NEGATIVE, SESourceType: ESESourceType.SE_01);
            _audioBinder.SetSoruceType(SEType: ESEType.PIECE_SELECT, SESourceType: ESESourceType.SE_05);
            _audioBinder.SetSoruceType(SEType: ESEType.PIECE_DESELECT, SESourceType: ESESourceType.SE_06);
            _audioBinder.SetSoruceType(SEType: ESEType.PIECE_PLACE, SESourceType: ESESourceType.SE_04);
            _audioBinder.SetSoruceType(SEType: ESEType.PIECE_GET, SESourceType: ESESourceType.SE_00);
            _audioBinder.SetSoruceType(SEType: ESEType.CHECKBOX_ON, SESourceType: ESESourceType.SE_00);
            _audioBinder.SetSoruceType(SEType: ESEType.CHECKBOX_OFF, SESourceType: ESESourceType.SE_01);
            _audioBinder.SetSoruceType(SEType: ESEType.SLIDER_CHANGE_VALUE, SESourceType: ESESourceType.SE_02);
            _audioBinder.SetSoruceType(SEType: ESEType.GAME_STAGE_OPPONENT_FOUND, SESourceType: ESESourceType.SE_41);

            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_00, filePathWithoutExtension: "se_00");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_01, filePathWithoutExtension: "se_01");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_02, filePathWithoutExtension: "se_02");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_03, filePathWithoutExtension: "se_03");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_04, filePathWithoutExtension: "se_04");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_05, filePathWithoutExtension: "se_05");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_06, filePathWithoutExtension: "se_06");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_07, filePathWithoutExtension: "se_07");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_08, filePathWithoutExtension: "se_08");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_09, filePathWithoutExtension: "se_09");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_10, filePathWithoutExtension: "se_10");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_11, filePathWithoutExtension: "se_11");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_12, filePathWithoutExtension: "se_12");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_13, filePathWithoutExtension: "se_13");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_14, filePathWithoutExtension: "se_14");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_15, filePathWithoutExtension: "se_15");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_16, filePathWithoutExtension: "se_16");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_17, filePathWithoutExtension: "se_17");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_18, filePathWithoutExtension: "se_18");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_19, filePathWithoutExtension: "se_19");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_20, filePathWithoutExtension: "se_20");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_21, filePathWithoutExtension: "se_21");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_22, filePathWithoutExtension: "se_22");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_23, filePathWithoutExtension: "se_23");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_24, filePathWithoutExtension: "se_24");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_25, filePathWithoutExtension: "se_25");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_26, filePathWithoutExtension: "se_26");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_27, filePathWithoutExtension: "se_27");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_28, filePathWithoutExtension: "se_28");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_29, filePathWithoutExtension: "se_29");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_30, filePathWithoutExtension: "se_30");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_31, filePathWithoutExtension: "se_31");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_32, filePathWithoutExtension: "se_32");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_33, filePathWithoutExtension: "se_33");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_34, filePathWithoutExtension: "se_34");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_35, filePathWithoutExtension: "se_35");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_36, filePathWithoutExtension: "se_36");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_37, filePathWithoutExtension: "se_37");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_38, filePathWithoutExtension: "se_38");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_39, filePathWithoutExtension: "se_39");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_40, filePathWithoutExtension: "se_40");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_41, filePathWithoutExtension: "se_41");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_42, filePathWithoutExtension: "se_42");
            _audioBinder.SetFilePath(SESourceType: ESESourceType.SE_43, filePathWithoutExtension: "se_43");
        }


        // ===============================================
        // MARK: - ● Load AudioClip ─────

        public AudioClip LoadAudioClip(string fileNameWithoutExtension)
        {
            return _audioClipStorage.LoadAudioClip(filePathWithoutExtension: fileNameWithoutExtension, willCachedIfNeeded: true);
        }



        // ===============================================
        // MARK: - ● Control Audio ─────

        public void PlayDefaultStageBGM()
        {
            var presentStageType = AppManager.Instance.PresentStageType;

            EBGMType BGMType = GetDefaultStageBGMType(stageType: presentStageType);

            Play(BGMType: BGMType);
        }


        // TODO: AudioBinder に移行したい。設定で変更するかもしれないので
        private EBGMType GetDefaultStageBGMType(EStageType stageType)
        {
            EBGMType result = stageType switch
            {
                EStageType.GAME => EBGMType.GAME,
                EStageType.HISTORY_DETAIL => EBGMType.GAME,
                _ => EBGMType.MENU,
            };

            return result;
        }


        private void Play(EBGMType BGMType)
        {
            // アクションが残っている場合、FadeOutの処理が残ってしまっていることが考えられる。
            if (_audioActionBehaviourBGM.HasAction)
            {
                Debug.Log("BGMのフェードアウトが完了する前に、次のシーンが表示されました");
            }


            // 音量を元に戻す
            _audioActionBehaviourBGM.SetVolumeForSystem(value: 1.0f);

            

            string fileNameWithoutExtension = _audioBinder.GetFilePath(BGMType: BGMType);

            _audioActionBehaviourBGM.Play(fileNameWithoutExtension: fileNameWithoutExtension);
        }


        public void Play(EJINType JINType)
        {
            // 音量を元に戻す
            _audioActionBehaviourJIN.SetVolumeForSystem(value: 1.0f);

            string filenameWithoutExtension = _audioBinder.GetFilePath(JINType: JINType);

            _audioActionBehaviourJIN.Play(fileNameWithoutExtension: filenameWithoutExtension);
        }


        public void Play(ESEType SEType)
        {
            // 音量を元に戻す
            _audioActionBehaviourSE.SetVolumeForSystem(value: 1.0f);

            string filenameWithoutExtension = _audioBinder.GetFilePath(SEType: SEType);

            _audioActionBehaviourSE.Play(fileNameWithoutExtension: filenameWithoutExtension);
        }


        private void Play(EBGMSourceType BGMSourceType)
        {
            string filenameWithoutExtension = _audioBinder.GetFilePath(BGMSourceType: BGMSourceType);

            _audioActionBehaviourBGM.Play(fileNameWithoutExtension: filenameWithoutExtension);
        }

        private void Play(EJINSourceType JINSourceType)
        {
            string filenameWithoutExtension = _audioBinder.GetFilePath(JINSourceType: JINSourceType);

            _audioActionBehaviourJIN.Play(fileNameWithoutExtension: filenameWithoutExtension);
        }

        private void Play(ESESourceType SESourceType)
        {
            string filenameWithoutExtension = _audioBinder.GetFilePath(SESourceType: SESourceType);

            _audioActionBehaviourSE.Play(fileNameWithoutExtension: filenameWithoutExtension);
        }


        private ESESourceType ConvertSETypeToSESource(ESEType SEType)
        {
            ESESourceType result = SEType switch
            {
                ESEType.TAP_NORMAL => ESESourceType.SE_00,
                ESEType.TAP_NEGATIVE => ESESourceType.SE_00,
                ESEType.PIECE_SELECT => ESESourceType.SE_00,
                ESEType.PIECE_DESELECT => ESESourceType.SE_00,
                ESEType.PIECE_PLACE => ESESourceType.SE_00,
                ESEType.PIECE_GET => ESESourceType.SE_00,
                _ => throw new InvalidOperationException(),
            };

            return result;
        }


        public void SetIsMuteForSystem(EVolumeType volumeType, bool value)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    _isMuteMasterForSystem = value;
                    UpdateVolume();
                    return;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    GetTargetAudioActionBehaviour(volumeType: volumeType).SetIsMuteForSystem(value: value);
                    return;

                default:
                    throw new InvalidOperationException();
            }
        }

        public void SetIsMuteForUser(EVolumeType volumeType, bool value)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    _isMuteMasterForUser = value;
                    UpdateVolume();
                    return;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    GetTargetAudioActionBehaviour(volumeType: volumeType).SetIsMuteForUser(value: value);
                    return;

                default:
                    throw new InvalidOperationException();
            }
        }



        public void SetVolumeForSystem(EVolumeType volumeType, float value)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    _volumeMasterForSystem = ValidateVolumeValue(value: value);
                    UpdateVolume();
                    return;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    GetTargetAudioActionBehaviour(volumeType: volumeType).SetVolumeForSystem(value: value);
                    return;

                default:
                    throw new InvalidOperationException();
            }
        }


        public void SetVolumeForUser(EVolumeType volumeType, float value)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    _volumeMasterForUser = ValidateVolumeValue(value: value);
                    UpdateVolume();
                    return;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    GetTargetAudioActionBehaviour(volumeType: volumeType).SetVolumeForUser(value: value);
                    return;

                default:
                    throw new InvalidOperationException();
            }
        }


        public bool GetIsMuteForSystem(EVolumeType volumeType)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    return _isMuteMasterForSystem;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    return GetTargetAudioActionBehaviour(volumeType: volumeType).IsMuteForSystem;

                default:
                    throw new InvalidOperationException();
            }
        }

        public bool GetIsMuteForUser(EVolumeType volumeType)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    return _isMuteMasterForUser;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    return GetTargetAudioActionBehaviour(volumeType: volumeType).IsMuteForUser;

                default:
                    throw new InvalidOperationException();
            }
        }

        public float GetVolumeForSystem(EVolumeType volumeType)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    return _volumeMasterForSystem;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    return GetTargetAudioActionBehaviour(volumeType: volumeType).VolumeForSystem;

                default:
                    throw new InvalidOperationException();
            }
        }

        public float GetVolumeForUser(EVolumeType volumeType)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    return _volumeMasterForUser;

                case EVolumeType.VOLUME_BGM:
                case EVolumeType.VOLUME_JIN:
                case EVolumeType.VOLUME_SE:
                    return GetTargetAudioActionBehaviour(volumeType: volumeType).VolumeForUser;

                default:
                    throw new InvalidOperationException();
            }
        }




        private AudioActionBehaviour GetTargetAudioActionBehaviour(EAudioType audioType)
        {
            switch (audioType)
            {
                case EAudioType.BGM:
                    return _audioActionBehaviourBGM;

                case EAudioType.JIN:
                    return _audioActionBehaviourJIN;

                case EAudioType.SE:
                    return _audioActionBehaviourSE;

                default:
                    throw new InvalidOperationException();
            }
        }

        private AudioActionBehaviour GetTargetAudioActionBehaviour(EVolumeType volumeType)
        {
            switch (volumeType)
            {
                case EVolumeType.VOLUME_MASTER:
                    throw new InvalidOperationException();

                case EVolumeType.VOLUME_BGM:
                    return _audioActionBehaviourBGM;

                case EVolumeType.VOLUME_JIN:
                    return _audioActionBehaviourJIN;

                case EVolumeType.VOLUME_SE:
                    return _audioActionBehaviourSE;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void UpdateVolume()
        {
            _audioActionBehaviourBGM.UpdateVolume();
            _audioActionBehaviourJIN.UpdateVolume();
            _audioActionBehaviourSE.UpdateVolume();
        }


        private float ValidateVolumeValue(float value)
        {
            if (value < 0.0f) { return 0.0f; }
            if (1.0f < value) { return 1.0f; }
            return value;
        }


        public float CalcVolume(float childValue)
        {
            if (IsMuteMaster) { return 0.0f; }

            return childValue * VolumeMaster;
        }


        public void Destroy()
        {
            _audioClipStorage.ClearCache();
        }


        // ------------------------------------
        // Core に公開されているもの

        public float GetVolumeMaster()
        {
            return _volumeMasterForUser;
        }

        public void SetVolumeMaster(float value)
        {
            _volumeMasterForUser = value;
            UpdateVolume();
        }

        // 名称が重複している時の書き方
        bool IAudioController.IsMuteMaster()
        {
            return _isMuteMasterForUser;
        }

        public bool IsMuteBGM()
        {
            return _audioActionBehaviourBGM.IsMuteForUser;
        }

        public bool IsMuteJIN()
        {
            return _audioActionBehaviourJIN.IsMuteForUser;
        }

        public bool IsMuteSE()
        {
            return _audioActionBehaviourSE.IsMuteForUser;
        }

        public void SetIsMuteMaster(bool value)
        {
            _isMuteMasterForUser = value;
            UpdateVolume();
        }

        public void SetIsMuteBGM(bool value)
        {
            _audioActionBehaviourBGM.SetIsMuteForUser(value: value);
        }

        public void SetIsMuteJIN(bool value)
        {
            _audioActionBehaviourJIN.SetIsMuteForUser(value: value);
        }

        public void SetIsMuteSE(bool value)
        {
            _audioActionBehaviourSE.SetIsMuteForUser(value: value);
        }


        public void FadeBGM(float alphaTo, float duration)
        {
            var action = CAction.CreateAudioActionFade(
                audioActionBehaviour: _audioActionBehaviourBGM,
                alphaTo: alphaTo,
                duration: duration);

            _audioActionBehaviourBGM.RegisterAction(action: action);
        }


        public void FadeOutBGMIfNeeded(EStageType nextStageType, float duration)
        {
            // 次のステージでデフォルトで鳴るBGMと同じであれば、何もしない
            var nextBGMType = GetDefaultStageBGMType(stageType: nextStageType);
            var nextBGMSourceType = _audioBinder.GetSourceType(BGMType: nextBGMType);
            var nextBGMfilePath = _audioBinder.GetFilePath(BGMSourceType: nextBGMSourceType);
            if (_audioActionBehaviourBGM.IsPlayingSameAudio(fileNameWithoutExtension: nextBGMfilePath)) { return; }


            FadeBGM(alphaTo: 0f, duration: duration);
        }


        public EBGMSourceType GetSourceType(EBGMType BGMType)
        {
            return _audioBinder.GetSourceType(BGMType: BGMType);
        }

        public EJINSourceType GetSourceType(EJINType JINType)
        {
            return _audioBinder.GetSourceType(JINType: JINType);
        }

        public ESESourceType GetSourceType(ESEType SEType)
        {
            return _audioBinder.GetSourceType(SEType: SEType);
        }


        public string GetFilePath(EBGMSourceType BGMSourceType)
        {
            return _audioBinder.GetFilePath(BGMSourceType: BGMSourceType);
        }

        public string GetFilePath(EJINSourceType JINSourceType)
        {
            return _audioBinder.GetFilePath(JINSourceType: JINSourceType);
        }

        public string GetFilePath(ESESourceType SESourceType)
        {
            return _audioBinder.GetFilePath(SESourceType: SESourceType);
        }


        public void SetSoruceType(EBGMType BGMType, EBGMSourceType BGMSourceType)
        {
            _audioBinder.SetSoruceType(BGMType: BGMType, BGMSourceType: BGMSourceType);
        }

        public void SetSoruceType(EJINType JINType, EJINSourceType JINSourceType)
        {
            _audioBinder.SetSoruceType(JINType: JINType, JINSourceType: JINSourceType);
        }

        public void SetSoruceType(ESEType SEType, ESESourceType SESourceType)
        {
            _audioBinder.SetSoruceType(SEType: SEType, SESourceType: SESourceType);
        }


        public void TestPlay(EBGMType BGMType)
        {
            Play(BGMType: BGMType);
        }
        public void TestPlay(EJINType JINType)
        {
            Play(JINType: JINType);
        }
        public void TestPlay(ESEType SEType)
        {
            Play(SEType: SEType);
        }


        public void TestPlay(EBGMSourceType BGMSourceType)
        {
            Play(BGMSourceType: BGMSourceType);
        }

        public void TestPlay(EJINSourceType JINSourceType)
        {
            Play(JINSourceType: JINSourceType);
        }

        public void TestPlay(ESESourceType SESourceType)
        {
            Play(SESourceType: SESourceType);
        }
    }
}

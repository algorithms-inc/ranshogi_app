using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public struct FVolumeSetting
    {
        // Mute
        public bool IsMuteForSystem;
        public bool IsMuteForUser;

        public bool IsMute => IsMuteForSystem || IsMuteForUser;


        // Volume

        private float _volumeForSystem;
        public float VolumeForSystem
        {
            set { _volumeForSystem = validateValue(value: value); }
            get { return _volumeForSystem; }
        }

        private float _volumeForUser;
        public float VolumeForUser
        {
            set { _volumeForUser = validateValue(value: value); }
            get { return _volumeForUser; }
        }

        /// <summary>
        /// Mute 状況を考慮した Volume 値
        /// </summary>
        public float Volume
        {
            get
            {
                if (IsMute) { return 0f; }
                return VolumeForSystem * VolumeForUser;
            }
        }

        public FVolumeSetting(
            bool isMuteForSystem,
            float volumeForSystem,
            bool isMuteForUser,
            float volumeForUser)
        {
            IsMuteForSystem = isMuteForSystem;
            IsMuteForUser = isMuteForUser;

            // プロパティ設定前に初期化が必要
            _volumeForSystem = 0f;
            _volumeForUser = 0f;

            VolumeForSystem = volumeForSystem;
            VolumeForUser = volumeForUser;
        }

        private float validateValue(float value)
        {
            if (value < 0f) { return 0f; }
            if (1 < value) { return 1f; }
            return value;
        }
    }
}
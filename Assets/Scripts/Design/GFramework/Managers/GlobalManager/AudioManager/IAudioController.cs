using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using GFramework.Core;


namespace GFramework.Core
{
    /// <summary>
    /// オーディオ設定機能
    /// </summary>
    public interface IAudioController
    {
        float GetVolumeMaster();
        void SetVolumeMaster(float value);

        bool IsMuteMaster();
        bool IsMuteBGM();
        bool IsMuteJIN();
        bool IsMuteSE();

        void SetIsMuteMaster(bool value);
        void SetIsMuteBGM(bool value);
        void SetIsMuteJIN(bool value);
        void SetIsMuteSE(bool value);


        EBGMSourceType GetSourceType(EBGMType BGMType);
        EJINSourceType GetSourceType(EJINType JINType);
        ESESourceType GetSourceType(ESEType SEType);

        string GetFilePath(EBGMSourceType BGMSourceType);
        string GetFilePath(EJINSourceType JINSourceType);
        string GetFilePath(ESESourceType SESourceType);

        void SetSoruceType(EBGMType BGMType, EBGMSourceType BGMSourceType);
        void SetSoruceType(EJINType JINType, EJINSourceType JINSourceType);
        void SetSoruceType(ESEType SEType, ESESourceType SESourceType);

        void TestPlay(EBGMType BGMType);
        void TestPlay(EJINType JINType);
        void TestPlay(ESEType SEType);

        void TestPlay(EBGMSourceType BGMSourceType);
        void TestPlay(EJINSourceType JINSourceType);
        void TestPlay(ESESourceType SESourceType);
    }
}
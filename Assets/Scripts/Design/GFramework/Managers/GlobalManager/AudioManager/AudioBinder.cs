using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;



namespace GFramework
{
    // EBGMType -> EBGMSourceType -> ファイルパス の対応づけを管理する
    // EBGMType -> EBGMSourceType の対応づけはプログラム側から自由に変更することができる
    // EBGMSourceType -> ファイルパス の対応づけはプログラム側から自由に変更することができる
    public class AudioBinder
    {
        private readonly int _maxIndexOfBGMType = Enum.GetNames(typeof(EBGMType)).Length - 1;
        private readonly int _maxIndexOfBGMSourceType = Enum.GetNames(typeof(EBGMSourceType)).Length - 1;

        private readonly int _maxIndexOfJINType = Enum.GetNames(typeof(EBGMType)).Length - 1;
        private readonly int _maxIndexOfJINSourceType = Enum.GetNames(typeof(EBGMSourceType)).Length - 1;

        private readonly int _maxIndexOfSEType = Enum.GetNames(typeof(EBGMType)).Length - 1;
        private readonly int _maxIndexOfSESourceType = Enum.GetNames(typeof(EBGMSourceType)).Length - 1;

        private Dictionary<EBGMType, EBGMSourceType> _BGMTypeToBGMSourceTypeList = new Dictionary<EBGMType, EBGMSourceType>();
        private Dictionary<EJINType, EJINSourceType> _JINTypeToJINSourceTypeList = new Dictionary<EJINType, EJINSourceType>();
        private Dictionary<ESEType, ESESourceType> _SETypeToSESourceTypeList = new Dictionary<ESEType, ESESourceType>();

        private Dictionary<EBGMSourceType, string> _BGMSourceTypeToFilePathList = new Dictionary<EBGMSourceType, string>();
        private Dictionary<EJINSourceType, string> _JINSourceTypeToFilePathList = new Dictionary<EJINSourceType, string>();
        private Dictionary<ESESourceType, string> _SESourceTypeToFilePathList = new Dictionary<ESESourceType, string>();

        public AudioBinder()
        {
            Assert.IsTrue(0 <= _maxIndexOfBGMType);
            Assert.IsTrue(0 <= _maxIndexOfBGMSourceType);
            Assert.IsTrue(0 <= _maxIndexOfJINType);
            Assert.IsTrue(0 <= _maxIndexOfJINSourceType);
            Assert.IsTrue(0 <= _maxIndexOfSEType);
            Assert.IsTrue(0 <= _maxIndexOfSESourceType);

            SetupBGMTypeToBGMSourceTypeList();
            SetupJINTypeToJINSourceTypeList();
            SetupSETypeToSESourceTypeList();

            SetupBGMSoruceTypeToFilePathList();
            SetupJINSoruceTypeToFilePathList();
            SetupSESoruceTypeToFilePathList();
        }

        // --------------------------------------

        public void DebugPrint()
        {
            Debug.Log("BGMType => BGMSourceType");

            foreach (var pair in _SESourceTypeToFilePathList)
            {
                var result = pair.Key + " => " + pair.Value + "";
                Debug.Log(result); 
            }

            // そのほかのものは未作成
        }



        // --------------------------------------

        private void SetupBGMTypeToBGMSourceTypeList()
        {
            EBGMSourceType firstItem = (EBGMSourceType)Enum.ToObject(typeof(EBGMSourceType), 0);

            for (int i = 0; i <= _maxIndexOfBGMType; i++)
            {
                EBGMType enumValue = (EBGMType)Enum.ToObject(typeof(EBGMType), i);

                _BGMTypeToBGMSourceTypeList[enumValue] = firstItem;
            }
        }


        private void SetupJINTypeToJINSourceTypeList()
        {
            EJINSourceType firstItem = (EJINSourceType)Enum.ToObject(typeof(EJINSourceType), 0);

            for (int i = 0; i <= _maxIndexOfJINType; i++)
            {
                EJINType enumValue = (EJINType)Enum.ToObject(typeof(EJINType), i);

                _JINTypeToJINSourceTypeList[enumValue] = firstItem;
            }
        }


        private void SetupSETypeToSESourceTypeList()
        {
            ESESourceType firstItem = (ESESourceType)Enum.ToObject(typeof(ESESourceType), 0);

            for (int i = 0; i <= _maxIndexOfSEType; i++)
            {
                ESEType enumValue = (ESEType)Enum.ToObject(typeof(ESEType), i);

                _SETypeToSESourceTypeList[enumValue] = firstItem;
            }
        }


        // --------------------------------------

        private void SetupBGMSoruceTypeToFilePathList()
        {
            string defaultFileName = "";

            for (int i = 0; i <= _maxIndexOfBGMSourceType; i++)
            {
                EBGMSourceType enumValue = (EBGMSourceType)Enum.ToObject(typeof(EBGMSourceType), i);

                _BGMSourceTypeToFilePathList[enumValue] = defaultFileName;
            }
        }


        private void SetupJINSoruceTypeToFilePathList()
        {
            string defaultFileName = "";

            for (int i = 0; i <= _maxIndexOfSESourceType; i++)
            {
                ESESourceType enumValue = (ESESourceType)Enum.ToObject(typeof(ESESourceType), i);

                _SESourceTypeToFilePathList[enumValue] = defaultFileName;
            }
        }


        private void SetupSESoruceTypeToFilePathList()
        {
            string defaultFileName = "aaa";

            for (int i = 0; i <= _maxIndexOfSESourceType; i++)
            {
                ESESourceType enumValue = (ESESourceType)Enum.ToObject(typeof(ESESourceType), i);

                _SESourceTypeToFilePathList[enumValue] = defaultFileName;
            }
        }




        // --------------------------------------

        public string GetFilePath(EBGMType BGMType)
        {
            EBGMSourceType BGMSourceType = _BGMTypeToBGMSourceTypeList[BGMType];

            return _BGMSourceTypeToFilePathList[BGMSourceType];
        }

        public string GetFilePath(EJINType JINType)
        {
            EJINSourceType JINSourceType = _JINTypeToJINSourceTypeList[JINType];

            return _JINSourceTypeToFilePathList[JINSourceType];
        }

        public string GetFilePath(ESEType SEType)
        {
            ESESourceType SESourceType = _SETypeToSESourceTypeList[SEType];

            return _SESourceTypeToFilePathList[SESourceType];
        }


        // --------------------------------------

        public EBGMSourceType GetSourceType(EBGMType BGMType)
        {
            return _BGMTypeToBGMSourceTypeList[BGMType];
        }

        public EJINSourceType GetSourceType(EJINType JINType)
        {
            return _JINTypeToJINSourceTypeList[JINType];
        }

        public ESESourceType GetSourceType(ESEType SEType)
        {
            return _SETypeToSESourceTypeList[SEType];
        }

        // --------------------------------------

        public string GetFilePath(EBGMSourceType BGMSourceType)
        {
            return _BGMSourceTypeToFilePathList[BGMSourceType];
        }

        public string GetFilePath(EJINSourceType JINSourceType)
        {
            return _JINSourceTypeToFilePathList[JINSourceType];
        }

        public string GetFilePath(ESESourceType SESourceType)
        {
            return _SESourceTypeToFilePathList[SESourceType];
        }


        // --------------------------------------

        public void SetSoruceType(EBGMType BGMType, EBGMSourceType BGMSourceType)
        {
            _BGMTypeToBGMSourceTypeList[BGMType] = BGMSourceType;
        }


        public void SetSoruceType(EJINType JINType, EJINSourceType JINSourceType)
        {
            _JINTypeToJINSourceTypeList[JINType] = JINSourceType;
        }


        public void SetSoruceType(ESEType SEType, ESESourceType SESourceType)
        {
            _SETypeToSESourceTypeList[SEType] = SESourceType;
        }


        // --------------------------------------

        public void SetFilePath(EBGMSourceType BGMSourceType, string filePathWithoutExtension)
        {
            _BGMSourceTypeToFilePathList[BGMSourceType] = filePathWithoutExtension;
        }


        public void SetFilePath(EJINSourceType JINSourceType, string filePathWithoutExtension)
        {
            _JINSourceTypeToFilePathList[JINSourceType] = filePathWithoutExtension;
        }


        public void SetFilePath(ESESourceType SESourceType, string filePathWithoutExtension)
        {
            _SESourceTypeToFilePathList[SESourceType] = filePathWithoutExtension;
        }

    }
}
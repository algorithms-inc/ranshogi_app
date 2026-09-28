using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


// 未使用
namespace GFramework
{
    public struct FAudioPreset
    {
        private string _displayName;

        private List<int> _BGMTypeToSoundIDList;
        private List<int> _JINTypeToSoundIDList;
        private List<int> _SETypeToSoundIDList;

        public FAudioPreset(
            string displayName,
            List<int> BGMTypeToSoundIDList,
            List<int> JINTypeToSoundIDList,
            List<int> SETypeToSoundIDList)
        {
            _displayName = displayName;

            _BGMTypeToSoundIDList = new List<int>();
            _JINTypeToSoundIDList = new List<int>();
            _SETypeToSoundIDList = new List<int>();
        }

        public void Validate()
        {
            Assert.IsTrue(ValidateBGM());
            Assert.IsTrue(ValidateJIN());
            Assert.IsTrue(ValidateSE());
        }

        private bool ValidateBGM()
        {
            return _BGMTypeToSoundIDList.Count == Enum.GetValues(typeof(EBGMType)).Length;
        }

        private bool ValidateJIN()
        {
            return _JINTypeToSoundIDList.Count == Enum.GetValues(typeof(EJINType)).Length;
        }

        private bool ValidateSE()
        {
            return _SETypeToSoundIDList.Count == Enum.GetValues(typeof(ESEType)).Length;
        }
    }
}
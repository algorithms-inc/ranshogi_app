using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    /// ラジオボタングループの初期化構造体
    public struct FRadioButtonGroupInitializer
    {
        // 入力判定を設置する Canvas
        private readonly Canvas _targetCanvas;
        public Canvas TargetCanvas => _targetCanvas;

        private readonly List<FWindowElementArea> _areaList;
        public List<FWindowElementArea> AreaList => _areaList;

        private readonly List<FCompanionTextFieldSetting> _companionTextFieldSettingList;
        public List<FCompanionTextFieldSetting> CompanionTextFieldSettingList => _companionTextFieldSettingList;

        private readonly int _indexOfOn;
        public int IndexOfOn => _indexOfOn;

        private readonly bool _isPlaySE;
        public bool IsPlaySE => _isPlaySE;

        private readonly bool _isEnabled;
        public bool IsEnabled => _isEnabled;

        // 画像を設置する順(判定エリアは Canvas に置かれる)
        private readonly int _sortingOrder;
        public int SortingOrder => _sortingOrder;

        // 押下時の動作
        private Action _action;
        public Action Action => _action;


        public FRadioButtonGroupInitializer(
            Canvas targetCanvas,
            in List<FWindowElementArea> areaList,
            in List<FCompanionTextFieldSetting> companionTextFieldSettingList,
            int indexOfOn,
            bool isEnabled,
            int sortingOrder,
            Action action,
            bool isPlaySE = true)
        {
            _targetCanvas = targetCanvas;
            _areaList = areaList;
            _companionTextFieldSettingList = companionTextFieldSettingList;
            _indexOfOn = indexOfOn;
            _isPlaySE = isPlaySE;
            _isEnabled = isEnabled;
            _sortingOrder = sortingOrder;
            _action = action;

            Assert.IsTrue(0 <= _indexOfOn);
            Assert.IsTrue(0 <= _areaList.Count);
            Assert.IsTrue(_indexOfOn < _areaList.Count);
        }
    }
}
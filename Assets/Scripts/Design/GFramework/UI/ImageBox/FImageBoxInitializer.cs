using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    /// イメージボックスの初期化構造体
    public struct FImageBoxInitializer
    {
        // 入力判定を設置する Canvas
        private readonly Canvas _targetCanvas;
        public Canvas TargetCanvas => _targetCanvas;

        private readonly FArea _area;
        public FArea Area => _area;

        // 画像を設置する順(判定エリアは Canvas に置かれる)
        private readonly int _sortingOrder;
        public int SortingOrder => _sortingOrder;

        private readonly string _texturePathUnderResources;
        public string TexturePathUnderResources => _texturePathUnderResources;


        // 押下時の動作
        private Action _action;
        public Action Action => _action;


        public FImageBoxInitializer(
            Canvas targetCanvas,
            FArea area,
            string texturePathUnderResources,
            int sortingOrder,
            Action action)
        {
            _targetCanvas = targetCanvas;
            _area = area;
            _texturePathUnderResources = texturePathUnderResources;
            _sortingOrder = sortingOrder;
            _action = action;
        }
    }
}
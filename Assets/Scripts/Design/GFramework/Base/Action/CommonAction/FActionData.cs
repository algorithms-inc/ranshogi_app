using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public readonly struct FActionData
    {
        private readonly GameObject _gameObjectWithSprite;
        public readonly GameObject GameObjectWithSprite => _gameObjectWithSprite;

        private readonly float _beginValue;
        public readonly float BeginValue => _beginValue;

        private readonly float _endValue;
        public readonly float EndValue => _endValue;


        public FActionData(GameObject gameObjectWithSprite, float beginValue, float endValue)
        {
            _gameObjectWithSprite = gameObjectWithSprite;
            _beginValue = beginValue;
            _endValue = endValue;
        }
    }
}

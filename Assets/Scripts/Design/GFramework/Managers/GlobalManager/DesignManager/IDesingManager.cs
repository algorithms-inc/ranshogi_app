using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    /// <summary>
    /// デザイン・マネジャのインターフェース
    /// </summary>
    public interface IDesignManager : IDesignController
    {
        void Destroy();
    }
}
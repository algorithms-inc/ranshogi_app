using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;


namespace GFramework
{
    /// <summary>
    /// 色を管理するマネジャ
    /// </summary>
    public interface IColorManager
    {
        public FColor3 GetColor3(EColorPreset colorPreset);

        public FColor4 GetColor4(EColorPreset colorPreset);

        public FColor4 GetColor4(EColorPreset colorPreset, float a);
    }
}
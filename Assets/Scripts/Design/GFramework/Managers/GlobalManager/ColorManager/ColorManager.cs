using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;


namespace GFramework
{
    public sealed class ColorManager : IColorManager
    {
        public FColor3 GetColor3(EColorPreset colorPreset)
        {
            return colorPreset switch
            {
                EColorPreset.SYSTEM_WHITE => new FColor3(r: 0.5f, g: 0.5f, b: 0.5f),
                EColorPreset.SYSTEM_GRAY_LIGHT => new FColor3(r: 0.375f, g: 0.375f, b: 0.375f),
                EColorPreset.SYSTEM_GRAY => new FColor3(r: 0.25f, g: 0.25f, b: 0.25f),
                EColorPreset.SYSTEM_GRAY_DARK => new FColor3(r: 0.125f, g: 0.125f, b: 0.125f),
                EColorPreset.SYSTEM_BLACK => new FColor3(r: 0.0f, g: 0.0f, b: 0.0f),
                EColorPreset.SYSTEM_RED => new FColor3(r: 0.5f, g: 0.25f, b: 0.25f),
                EColorPreset.SYSTEM_GREEN => new FColor3(r: 0.25f, g: 0.5f, b: 0.25f),
                EColorPreset.SYSTEM_BLUE => new FColor3(r: 0.25f, g: 0.25f, b: 0.5f),
                EColorPreset.SYSTEM_YELLOW => new FColor3(r: 0.5f, g: 0.5f, b: 0.25f),
                EColorPreset.SYSTEM_CYAN => new FColor3(r: 0.25f, g: 0.5f, b: 0.5f),
                EColorPreset.SYSTEM_MAGENTA => new FColor3(r: 0.5f, g: 0.25f, b: 0.5f),
                _ => throw new InvalidOperationException(),
            };
        }



        public FColor4 GetColor4(EColorPreset colorPreset)
        {
            return GetColor3(colorPreset: colorPreset).AsColor4();
        }

        public FColor4 GetColor4(EColorPreset colorPreset, float a)
        {
            return GetColor3(colorPreset: colorPreset).AsColor4(a: a);
        }
    }
}
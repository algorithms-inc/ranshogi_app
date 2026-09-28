using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework.Core
{
    public interface IUIImageBoxCreator
    {
        void Add(
            string name,
            in FWindowElementArea area,
            string filePath,
            Action actionOnTap = null);
        void Remove(string name);
        void ChangeImage(string name, string filePath);
    }
}
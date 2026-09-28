using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public class UIImageBoxCreator : IUIImageBoxCreator
    {
        private WindowBehaviour _windowBehaviour;

        public UIImageBoxCreator(WindowBehaviour windowBehaviour)
        {
            _windowBehaviour = windowBehaviour;
        }

        public void Add(
            string name,
            in FWindowElementArea area,
            string filePath,
            Action actionOnTap = null)
        {
            area.Validate(windowType: _windowBehaviour.WindowType);

            _windowBehaviour.AddImageBox(
                name: name,
                windowElementArea: area,
                filePath: filePath,
                actionOnTap: actionOnTap);
        }

        public void Remove(string name)
        {
            _windowBehaviour.RemoveImageBox(name: name);
        }

        public void ChangeImage(string name, string filePath)
        {
            _windowBehaviour.ChangeImageBoxImage(name: name, filePath: filePath);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GFramework
{
    public interface ITestCase
    {
        public void ShowBeginLog();
        public void ShowEndLog();
        public void Execute();
    }
}
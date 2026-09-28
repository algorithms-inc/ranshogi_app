using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public interface IBoardSetInputManager
    {
        void SetupInputForGame();

        Canvas Canvas { get; }

        void SetIsEnabledIfNeeded(bool value);
        void SetIsEnabled(bool value);
    }
}


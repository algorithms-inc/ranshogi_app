using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public interface ILayoutManager
    {
        // ReLayout

        bool IsReLayoutable();
        void SetIsReLayoutable(bool value);
        bool ShouldReLayout(EScreenOrientation newScreenOrientation, Vector2 newScreenSize);


        // Waiting Space Type
        EWaitingSpaceType GetWaitingSpaceType();


        // Base Button Height

        float ButtonBaseHeight { get; }



        // Show Layout Area

        void ShowLayoutArea((string, FArea)[] areaList);
        void HideLayoutArea();


        // Background
        FArea AreaBackground { get; }

        // UI
        Vector2 ScaleUICanvas { get; }

        FArea AreaUICanvas { get; }

        // Window
        FArea AreaWindowTop { get; }
        FArea AreaWindow { get; }


        FArea AreaSafeAreaWithoutStatusBar { get; }

        FArea AreaMainContents { get; }
        FArea AreaMainContentsWithoutStatusBar { get; }

        FArea AreaGameStageHeader { get; }
        FArea AreaGameStageBoardSet { get; }
        FArea AreaGameStageBoardControlAndMessage { get; }
        FArea GetAreaStatusBar(EPlayerType playerType);
        FArea AreaStatusBarPlayer1 { get; }
        FArea AreaStatusBarPlayer2 { get; }



        FArea GetAreaOfRipple(ERippleType rippleType);
    }
}
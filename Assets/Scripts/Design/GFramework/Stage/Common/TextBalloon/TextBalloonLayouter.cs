using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class TextBalloonLayouter : PLayouter
    {
        private LayoutRoot _layoutRoot;
        public FArea BalloonMessageArea => _layoutRoot.Area.AsYReversed();


        // ===============================================
        // MARK: - ● Setup Component ─────

        public TextBalloonLayouter(FArea area)
        {
            Setup(area: area);
        }



        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup(FArea area)
        {
            _layoutRoot = new LayoutRoot(area: area, spacing: FVec4.ZERO);

            _layoutRoot.Update();
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutRoot?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("BalloonArea", _layoutRoot.Area.AsYReversed()),
            });
        }
    }
}
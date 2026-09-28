using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class GameResultKeyVisualLayouter : PLayouter
    {
        private LayoutRoot _layoutKeyVisualMax;
        private LayoutFrame _layoutKeyVisual;

        public FArea AreaKeyVisual => _layoutKeyVisual.Area.AsYReversed();
        public FArea AreaConsecutiveCount => CalcAreaConsecutiveCount();


        // ===============================================
        // MARK: - ● Setup Component ─────

        public GameResultKeyVisualLayouter(FArea area)
        {
            Setup(area: area);
        }

    

        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup(FArea area)
        {
            _layoutKeyVisualMax = new LayoutRoot(area: area, spacing: FVec4.ZERO);

            _layoutKeyVisual = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 1f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _layoutKeyVisualMax.RegisterChildByList(newChildList: new List<PLayout> {
                _layoutKeyVisual,
            });


            _layoutKeyVisualMax.Update();
        }



        // ===============================================
        // MARK: - ● Calc Area ─────

        private FArea CalcAreaConsecutiveCount()
        {
            return _layoutKeyVisual.Area.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.5f, 0f), endPointRatio: new Vector2(1.0f, 0.5f)).AsYReversed();
        }


        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutKeyVisualMax?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("KeyVisualArea", _layoutKeyVisualMax.Area.AsYReversed()),
                ("AreaKeyVisual", AreaKeyVisual),
                ("AreaConsecutiveCount", AreaConsecutiveCount)
            });
        }
    }
}
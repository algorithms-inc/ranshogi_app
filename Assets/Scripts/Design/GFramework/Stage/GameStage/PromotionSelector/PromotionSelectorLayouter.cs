using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class PromotionSelectorLayouter : PLayouter
    {
        private LayoutRoot _layoutPromotionSelector;
        public FArea AreaPromotionSelector => _layoutPromotionSelector.Area.AsYReversed();

        private LayoutFrame _frameButtonArea;

        private LayoutFrame _frameButtonL;
        private LayoutFrame _frameButtonR;

        private LayoutFrame _framePieceL;
        private LayoutFrame _framePieceR;

        public FArea AreaButtonPromote => CalcAreaButtonPromote();
        public FArea AreaButtonNotPromote => CalcAreaButtonNotPromote();

        public FArea AreaPiecePromote => CalcAreaPiecePromote();
        public FArea AreaPieceNotPromote => CalcAreaPieceNotPromote();



        // ===============================================
        // MARK: - ● Setup Component ─────

        public PromotionSelectorLayouter(FArea area)
        {
            Setup(area: area);

            //ShowLayoutArea();
        }



        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup(FArea area)
        {
            FArea areaPromotionSelector = area;
            _layoutPromotionSelector = new LayoutRoot(size: areaPromotionSelector.size, spacing: new FVec4(0, 0, 0, 0), position: areaPromotionSelector.origin);

            _frameButtonArea = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.85f, 0.8f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _layoutPromotionSelector.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonArea});


            _frameButtonL = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.47f, 1f)),
                align: EDirection.LEFT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonR = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.47f, 1f)),
                align: EDirection.RIGHT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonL,
                _frameButtonR});



            _framePieceL = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.7f, 0.7f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonL.RegisterChildByList(newChildList: new List<PLayout> {
                _framePieceL});


            _framePieceR = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.7f, 0.7f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonR.RegisterChildByList(newChildList: new List<PLayout> {
                _framePieceR});


            _layoutPromotionSelector.Update();
        }



        // ===============================================
        // MARK: - ● Calc Area ─────

        private FArea CalcAreaButtonPromote()
        {
            if (Config.PROMOTION_SELECTOR__LEFT_IS_PROMTE)
            {
                return _frameButtonL.Area.AsYReversed();
            }
            else
            {
                return _frameButtonR.Area.AsYReversed();
            }
        }


        private FArea CalcAreaButtonNotPromote()
        {
            if (Config.PROMOTION_SELECTOR__LEFT_IS_PROMTE)
            {
                return _frameButtonR.Area.AsYReversed();
            }
            else
            {
                return _frameButtonL.Area.AsYReversed();
            }
        }


        private FArea CalcAreaPiecePromote()
        {
            if (Config.PROMOTION_SELECTOR__LEFT_IS_PROMTE)
            {
                return _framePieceL.Area.AsYReversed();
            }
            else
            {
                return _framePieceR.Area.AsYReversed();
            }
        }


        private FArea CalcAreaPieceNotPromote()
        {
            if (Config.PROMOTION_SELECTOR__LEFT_IS_PROMTE)
            {
                return _framePieceR.Area.AsYReversed();
            }
            else
            {
                return _framePieceL.Area.AsYReversed();
            }
        }






        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutPromotionSelector?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("AreaPromotionSelector", AreaPromotionSelector),
                ("AreaButtonL", AreaButtonPromote),
                ("AreaButtonR", AreaButtonNotPromote),
                ("AreaPieceL", AreaPiecePromote),
                ("AreaPieceNotProAreaPieceNotRote", AreaPieceNotPromote)
            });
        }
    }
}
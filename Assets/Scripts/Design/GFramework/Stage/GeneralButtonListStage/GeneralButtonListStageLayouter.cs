using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class GeneralButtonListStageLayouter : PLayouter
    {
        private LayoutRoot _layoutRoot;

        private LayoutVStack _frameDemoMain;


        private LayoutFrame _frameTextTitle;
        public FArea AreaTextTitle => _frameTextTitle.Area.AsYReversed();

        private LayoutFrame _frameButton0;
        public FArea AreaButton0 => _frameButton0.Area.AsYReversed();

        private LayoutFrame _frameButton1;
        public FArea AreaButton1 => _frameButton1.Area.AsYReversed();

        private LayoutFrame _frameButton2;
        public FArea AreaButton2 => _frameButton2.Area.AsYReversed();

        private LayoutFrame _frameButton3;
        public FArea AreaButton3 => _frameButton3.Area.AsYReversed();

        private LayoutFrame _frameButton4;
        public FArea AreaButton4 => _frameButton4.Area.AsYReversed();

        private LayoutFrame _frameButton5;
        public FArea AreaButton5 => _frameButton5.Area.AsYReversed();

        private LayoutFrame _frameButton6;
        public FArea AreaButton6 => _frameButton6.Area.AsYReversed();

        private LayoutFrame _frameButton7;
        public FArea AreaButton7 => _frameButton7.Area.AsYReversed();

        private LayoutFrame _frameButton8;
        public FArea AreaButton8 => _frameButton8.Area.AsYReversed();

        private LayoutFrame _frameButton9;
        public FArea AreaButton9 => _frameButton9.Area.AsYReversed();



        // ===============================================
        // MARK: - ● Setup Component ─────

        public GeneralButtonListStageLayouter()
        {
            Setup();
        }


        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup()
        {
            float singleSpacing = Locator.Instance.layoutManager.AreaWindow.size.y * 0.05f;


            _layoutRoot = new LayoutRoot(area: Locator.Instance.layoutManager.AreaMainContentsWithoutStatusBar.AsYReversed(), spacing: new FVec4(0, singleSpacing, 0, singleSpacing)); ;


            _frameDemoMain = new LayoutVStack(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                innerSpacing: 0f,
                innerAlign: EDirection.UP,
                offset: new Vector2(0, 0));

            _layoutRoot.RegisterChildByList(
                newChildList: new List<PLayout> { _frameDemoMain });

            // 一度計算
            _layoutRoot.Update();


            // ----------

            _frameTextTitle = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton0 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton1 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton2 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton3 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton4 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton5 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton6 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton7 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton8 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameButton9 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.09f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameDemoMain.RegisterChildByList(
                newChildList: new List<PLayout> {
                    _frameTextTitle,
                    _frameButton0,
                    _frameButton1,
                    _frameButton2,
                    _frameButton3,
                    _frameButton4,
                    _frameButton5,
                    _frameButton6,
                    _frameButton7,
                    _frameButton8,
                    _frameButton9,
                });


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
                ("AreaMainMax", _layoutRoot.Area.AsYReversed()),
                ("AreaMain", _frameDemoMain.Area.AsYReversed()),
                ("AreaTextTitle", AreaTextTitle),
                ("AreaButton0", AreaButton0),
                ("AreaButton1", AreaButton1),
                ("AreaButton2", AreaButton2),
                ("AreaButton3", AreaButton3),
                ("AreaButton4", AreaButton4),
                ("AreaButton5", AreaButton5),
                ("AreaButton6", AreaButton6),
                ("AreaButton7", AreaButton7),
                ("AreaButton8", AreaButton8),
                ("AreaButton9", AreaButton9),
            });
        }
    }
}
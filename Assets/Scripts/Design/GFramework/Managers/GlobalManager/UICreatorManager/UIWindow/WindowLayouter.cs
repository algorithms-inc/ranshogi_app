using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;



namespace GFramework
{
    public class WindowLayouter : PLayouter
    {
        // for Window Top
        private LayoutRoot _layoutRootWindowTop;

        private LayoutFrame _layoutWindowTopBase;
        private FArea AreaWindowTopBase => _layoutWindowTopBase.Area.AsYReversed();

        private LayoutFrame _frameWindowTopMain;
        private FArea AreaWindowTopMain => _frameWindowTopMain.Area.AsYReversed();


        // for Window L
        private LayoutRoot _layoutRootWindowL;

        private LayoutFrame _layoutWindowLBase;
        private FArea AreaWindowLBase => _layoutWindowLBase.Area.AsYReversed();

        private LayoutFrame _frameWindowLMain;
        private FArea AreaWindowLMain => _frameWindowLMain.Area.AsYReversed();


        // for Window S
        private LayoutRoot _layoutRootWindowS;

        private LayoutFrame _layoutWindowSBase;
        private FArea AreaWindowSBase => _layoutWindowSBase.Area.AsYReversed();

        private LayoutFrame _frameWindowSMain;
        private FArea AreaWindowSMain => _frameWindowSMain.Area.AsYReversed();


        // ===============================================
        // MARK: - ● Constructor ─────

        public WindowLayouter()
        {
            SetupWindowTop();

            SetupWindowL();

            SetupWindowS();

            //ShowLayoutArea();
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void SetupWindowTop()
        {
            float singleSpacing = Locator.Instance.layoutManager.AreaWindowTop.size.x * 0.01f;


            _layoutRootWindowTop = new LayoutRoot(
                area: Locator.Instance.layoutManager.AreaWindowTop.AsYReversed(),
                spacing: FVec4.ZERO);

            _layoutWindowTopBase = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByXYRatio(XYRatio: 0.5f),
                align: EDirection.UP,
                priority: 0,
                spacing: new FVec4(singleSpacing),
                //spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _layoutRootWindowTop.RegisterChildByList(
                newChildList: new List<PLayout> { _layoutWindowTopBase });


            _frameWindowTopMain = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByXYRatio(XYRatio: 0.5f),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _layoutWindowTopBase.RegisterChildByList(
                newChildList: new List<PLayout> { _frameWindowTopMain });


            // 一度計算
            _layoutRootWindowTop.Update();
        }


        private void SetupWindowL()
        {
            float singleSpacing = Locator.Instance.layoutManager.AreaWindow.size.y * 0.05f;


            _layoutRootWindowL = new LayoutRoot(
                area: Locator.Instance.layoutManager.AreaWindow.AsYReversed(),
                spacing: FVec4.ZERO);

            _layoutWindowLBase = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByXYRatio(XYRatio: 1.6f),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(singleSpacing),
                offset: new Vector2(0, 0));

            _layoutRootWindowL.RegisterChildByList(
                newChildList: new List<PLayout> { _layoutWindowLBase });


            _frameWindowLMain = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByXYRatio(XYRatio: 1.6f),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _layoutWindowLBase.RegisterChildByList(
                newChildList: new List<PLayout> { _frameWindowLMain });


            // 一度計算
            _layoutRootWindowL.Update();
        }


        private void SetupWindowS()
        {
            float singleSpacing = Locator.Instance.layoutManager.AreaWindow.size.y * 0.05f;


            _layoutRootWindowS = new LayoutRoot(
                area: Locator.Instance.layoutManager.AreaWindow.AsYReversed(),
                spacing: FVec4.ZERO);

            var size = _frameWindowLMain.Area.size;
            var x = size.x / 10f * (Config.WindowConfirmationSizeRatio.x * 10f);
            var y = size.y / 16f * (Config.WindowConfirmationSizeRatio.y * 10f);

            _frameWindowSMain = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(new Vector2(x, y)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _layoutRootWindowS.RegisterChildByList(
                newChildList: new List<PLayout> { _frameWindowSMain });

            _layoutWindowSBase = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(new Vector2(x + singleSpacing * 2, y + singleSpacing * 2)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(singleSpacing),
                offset: new Vector2(0, 0));

            _layoutRootWindowS.RegisterChildByList(
                newChildList: new List<PLayout> { _layoutWindowSBase });


            // 一度計算
            _layoutRootWindowS.Update();
        }


        // ===============================================
        // MARK: - ● Destroy ─────

        public FArea GetAreaWindowBase(EWindowType windowType)
        {
            return windowType switch
            {
                EWindowType.TOP => AreaWindowTopBase,
                EWindowType.NORMAL => AreaWindowLBase,
                EWindowType.CONFIRMATION => AreaWindowSBase,
                EWindowType.ERROR => AreaWindowLBase,
                _ => throw new InvalidOperationException(),
            };
        }


        public FArea GetAreaWindowMain(EWindowType windowType)
        {
            return windowType switch
            {
                EWindowType.TOP => AreaWindowTopMain,
                EWindowType.NORMAL => AreaWindowLMain,
                EWindowType.CONFIRMATION => AreaWindowSMain,
                EWindowType.ERROR => AreaWindowLMain,
                _ => throw new NotImplementedException(),
            };
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutRootWindowTop?.Destroy();
            _layoutRootWindowL?.Destroy();
            _layoutRootWindowS?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("WindowTopMax", _layoutRootWindowTop.Area.AsYReversed()),
                ("WindowTopBase", AreaWindowTopBase),
                ("WindowTopMain", AreaWindowTopMain),

                // ("WindowLMax", _layoutRootWindowL.Area.AsYReversed()),
                // ("WindowLBase", AreaWindowLBase),
                // ("WindowLMain", AreaWindowLMain),

                // ("WindowSMax", _layoutRootWindowS.Area.AsYReversed()),
                // ("WindowSBase", AreaWindowSBase),
                // ("WindowSMain", AreaWindowSMain),
            });
        }
    }
}
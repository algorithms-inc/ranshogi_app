using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class OpponentSearcherLayouter : PLayouter
    {
        private LayoutRoot _layoutOpponentSearcher;

        private LayoutFrame _frameTextAndAnimationAreaMax;
        private LayoutFrame _frameButtonAreaMax;

        private LayoutFrame _frameText;
        public FArea AreaText => _frameText.Area.AsYReversed();

        private LayoutFrame _frameAnimation;
        public FArea AreaAnimation => _frameAnimation.Area.AsYReversed();

        private LayoutFrame _frameButtonBack;
        public FArea AreaButtonBack => _frameButtonBack.Area.AsYReversed();

        private int _fontSize = 0;
        public int FontSize => _fontSize;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public OpponentSearcherLayouter()
        {
            Setup();

            CalcFontSize();

            //ShowLayoutArea();
        }



        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup()
        {
            FArea areaOpponentSearcherMax = Locator.Instance.layoutManager.AreaMainContentsWithoutStatusBar.AsYReversed();
            _layoutOpponentSearcher = new LayoutRoot(area: areaOpponentSearcherMax, spacing: new FVec4(0, 0, 0, 0));

            _frameTextAndAnimationAreaMax = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 0.5f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonAreaMax = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.9f, 0.1f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _layoutOpponentSearcher.RegisterChildByList(newChildList: new List<PLayout> {
                _frameTextAndAnimationAreaMax,
                _frameButtonAreaMax});

            _layoutOpponentSearcher.Update();


            // ------------------


            _frameText = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 0.5f)),
                align: EDirection.UP,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameAnimation = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 0.5f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameTextAndAnimationAreaMax.RegisterChildByList(newChildList: new List<PLayout> {
                _frameText,
                _frameAnimation});

            _layoutOpponentSearcher.Update();


            // ------------------


            var buttonSize = new Vector2(_frameButtonAreaMax.Size.x, Locator.Instance.layoutManager.ButtonBaseHeight);

            _frameButtonBack = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSize),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));


            _frameButtonAreaMax.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonBack});

            _layoutOpponentSearcher.Update();
        }


        private void CalcFontSize()
        {
            var height = _frameText.Area.size.y;

            _fontSize = (int)(height * 0.15f);
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutOpponentSearcher?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("AreaOppnentSearcher", _layoutOpponentSearcher.Area.AsYReversed()),
                ("AreaTextAndAnimationAreaMax", _frameTextAndAnimationAreaMax.Area.AsYReversed()),
                ("AreaButtonAreaMax", _frameButtonAreaMax.Area.AsYReversed()),
                ("AreaText", AreaText),
                ("AreaAnimation", AreaAnimation),
                ("AreaButtonBack", AreaButtonBack),
            });
        }
    }
}
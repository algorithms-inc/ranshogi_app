using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace GFramework
{
    public class HistoryStageLayouter : PLayouter
    {
        private LayoutRoot _layoutRoot;

        private LayoutVStack _frameHistoyMain;


        private LayoutFrame _frameSpacing1;

        private LayoutFrame _frameHistoryItemList;
        public LayoutFrame FrameHistoryItemList => _frameHistoryItemList;

        private LayoutFrame _frameSpacing2;

        private LayoutFrame _frameBackButtonAndPageNavigator;
        public LayoutFrame FrameBackButtonAndPageNavigator => _frameBackButtonAndPageNavigator;


        private LayoutHStack _framePageNavigator;
        public LayoutHStack FramePageNavigator => _framePageNavigator;


        private LayoutFrame _frameTextPage;
        public FArea AreaTextPage => _frameTextPage.Area.AsYReversed();



        private LayoutFrame _frameButtonBack;
        public FArea AreaButtonBack => _frameButtonBack.Area.AsYReversed();


        private LayoutFrame _frameButtonNextPage;
        public FArea AreaButtonNextPage => _frameButtonNextPage.Area.AsYReversed();
        public FArea AreaButtonNextPageCompanionImage => _frameButtonNextPage.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();

        private LayoutFrame _frameButtonPreviousPage;
        public FArea AreaButtonPreviousPage => _frameButtonPreviousPage.Area.AsYReversed();
        public FArea AreaButtonPreviousPageCompanionImage => _frameButtonPreviousPage.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();

        private LayoutFrame _frameButtonFirstPage;
        public FArea AreaButtonFirstPage => _frameButtonFirstPage.Area.AsYReversed();
        public FArea AreaButtonFirstPageCompanionImage => _frameButtonFirstPage.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();

        private LayoutFrame _frameButtonLastPage;
        public FArea AreaButtonLastPage => _frameButtonLastPage.Area.AsYReversed();
        public FArea AreaButtonLastPageCompanionImage => _frameButtonLastPage.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();


        private readonly int _tempHistoryItemDisplayCount = 5;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public HistoryStageLayouter()
        {
            Setup();

            //ShowLayoutArea();
        }


        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup()
        {
            FArea areaMenuStage = Locator.Instance.layoutManager.AreaMainContentsWithoutStatusBar.AsYReversed();
            float singleSpacing = Locator.Instance.layoutManager.AreaWindow.size.y * 0.015f;

            _layoutRoot = new LayoutRoot(size: areaMenuStage.size, spacing: new FVec4(singleSpacing), position: areaMenuStage.origin);

            _frameHistoyMain = new LayoutVStack(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                innerSpacing: 0f,
                innerAlign: EDirection.UP,
                offset: new Vector2(0, 0));

            _layoutRoot.RegisterChildByList(
                newChildList: new List<PLayout> { _frameHistoyMain });

            // 一度計算
            _layoutRoot.Update();


            // ----------

            _frameTextPage = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.05f)),
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameSpacing1 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.01f)),
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameHistoryItemList = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.85f)),
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));


            _frameSpacing2 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.015f)),
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));


            _frameBackButtonAndPageNavigator = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.075f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));



            _frameHistoyMain.RegisterChildByList(
                newChildList: new List<PLayout> {
                    _frameTextPage,
                    _frameSpacing1,
                    _frameHistoryItemList,
                    _frameSpacing2,
                    _frameBackButtonAndPageNavigator });

            // 一度計算
            _layoutRoot.Update();

            // ----------



            _frameButtonBack = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.2f, 1f)),
                align: EDirection.LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _framePageNavigator = new LayoutHStack(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 1f)),
                align: EDirection.RIGHT,
                priority: 0,
                spacing: FVec4.ZERO,
                innerSpacing: 0f,
                innerAlign: EDirection.STAY,
                offset: new Vector2(0, 0));


            _frameBackButtonAndPageNavigator.RegisterChildByList(
                newChildList: new List<PLayout> {
                    _frameButtonBack,
                    _framePageNavigator
                });


            _layoutRoot.Update();


            _frameButtonFirstPage = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));



            _frameButtonPreviousPage = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_RIGHT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));


            _frameButtonNextPage = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));



            _frameButtonLastPage = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));


            _framePageNavigator.RegisterChildByList(
                newChildList: new List<PLayout> {
                    _frameButtonFirstPage,
                    _frameButtonPreviousPage,
                    _frameButtonNextPage,
                    _frameButtonLastPage,
                });

            _layoutRoot.Update();
        }

        // ===============================================
        // MARK: - ● Get Area ─────

        public FArea GetAreaForHistoryItem(int index)
        {
            Assert.IsTrue(index <= _tempHistoryItemDisplayCount - 1);

            float i = 1f / _tempHistoryItemDisplayCount;

            var originRatio = new Vector2(0, i * index);
            var endPointRatio = new Vector2(1, i * (index + 1f));

            return _frameHistoryItemList.Area.GetPartialAreaByOriginAndEndPoint(originRatio: originRatio, endPointRatio: endPointRatio);
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
                ("HistoryMax", _layoutRoot.Area.AsYReversed()),
                ("HistoryMain", _frameHistoyMain.Area.AsYReversed()),
                ("HistoryItemList", _frameHistoryItemList.Area.AsYReversed()),
                ("HistoryButtonBack", AreaButtonBack),
                ("HistoryButtonNextPage", AreaButtonNextPage),
                ("HistoryButtonPreviousPage", AreaButtonPreviousPage),
                ("HistoryButtonNextPage", AreaButtonFirstPage),
                ("HistoryButtonPreviousPage", AreaButtonLastPage),
                ("HistoryTextPage", AreaTextPage),
            });
        }
    }
}
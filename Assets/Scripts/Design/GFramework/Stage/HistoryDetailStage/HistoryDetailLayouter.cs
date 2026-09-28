using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class HistoryDetailStageLayouter : PLayouter
    {
        private LayoutRoot _layoutRootHeader;

        public FArea AreaButtonInfo => _layoutRootHeader.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.8f, 0.0f),
            endPointRatio: new Vector2(1f, 1f)).AsYReversed();

        public FArea AreaButtonReplayMode => _layoutRootHeader.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.40f, 0.0f),
            endPointRatio: new Vector2(0.8f, 1f)).AsYReversed();


        public FArea AreaTextMove => _layoutRootHeader.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.1f, 0.0f),
            endPointRatio: new Vector2(0.4f, 1f)).AsYReversed();

        // 手数表示の位置に置く (手数表示は現在非表示)
        public FArea AreaButtonAiMatch => AreaTextMove;


        private LayoutRoot _layoutRootButtonBackAndMoveNavigator;

        private LayoutFrame _frameButtonBackAndMoveNavigatorMain;

        private LayoutFrame _frameButtonBack;
        public FArea AreaButtonBack => _frameButtonBack.Area.AsYReversed();

        private LayoutHStack _frameMoveNavigator;


        private LayoutFrame _frameButtonNext;
        public FArea AreaButtonNext => _frameButtonNext.Area.AsYReversed();
        public FArea AreaButtonNextCompanionImage => _frameButtonNext.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();

        private LayoutFrame _frameButtonPrevious;
        public FArea AreaButtonPrevious => _frameButtonPrevious.Area.AsYReversed();
        public FArea AreaButtonPreviousCompanionImage => _frameButtonPrevious.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();

        private LayoutFrame _frameButtonToFirst;
        public FArea AreaButtonToFirst => _frameButtonToFirst.Area.AsYReversed();
        public FArea AreaButtonToFirstCompanionImage => _frameButtonToFirst.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();

        private LayoutFrame _frameButtonToLast;
        public FArea AreaButtonToLast => _frameButtonToLast.Area.AsYReversed();
        public FArea AreaButtonToLastCompanionImage => _frameButtonToLast.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.25f, 0.25f),
            endPointRatio: new Vector2(0.75f, 0.75f)).AsYReversed();



        // ===============================================
        // MARK: - ● Constructor ─────

        public HistoryDetailStageLayouter()
        {
            Setup();
            //ShowLayoutArea();
        }


        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup()
        {
            _layoutRootHeader = new LayoutRoot(
                area: Locator.Instance.layoutManager.AreaGameStageHeader.AsYReversed(),
                spacing: FVec4.ZERO);

            _layoutRootHeader.RegisterChildByList(
                newChildList: new List<PLayout> {});

            // 一度計算
            _layoutRootHeader.Update();

            // ----------


            _layoutRootButtonBackAndMoveNavigator = new LayoutRoot(
                area: Locator.Instance.layoutManager.AreaGameStageBoardControlAndMessage.AsYReversed(),
                spacing: FVec4.ZERO);

            _frameButtonBackAndMoveNavigatorMain = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.95f, 1f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _layoutRootButtonBackAndMoveNavigator.RegisterChildByList(
                newChildList: new List<PLayout> {
                    _frameButtonBackAndMoveNavigatorMain
                });

            _layoutRootButtonBackAndMoveNavigator.Update();

            _frameButtonBack = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.2f, 1f)),
                align: EDirection.LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));

            _frameMoveNavigator = new LayoutHStack(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 1f)),
                align: EDirection.RIGHT,
                priority: 0,
                spacing: FVec4.ZERO,
                innerSpacing: 0f,
                innerAlign: EDirection.STAY,
                offset: new Vector2(0, 0));


            _frameButtonBackAndMoveNavigatorMain.RegisterChildByList(
                newChildList: new List<PLayout> {
                    _frameButtonBack,
                    _frameMoveNavigator
                });


            _layoutRootButtonBackAndMoveNavigator.Update();


            _frameButtonToFirst = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));



            _frameButtonPrevious = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_RIGHT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));


            _frameButtonNext = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));



            _frameButtonToLast = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.25f, 1f)),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));


            _frameMoveNavigator.RegisterChildByList(
                newChildList: new List<PLayout> {
                    _frameButtonToFirst,
                    _frameButtonPrevious,
                    _frameButtonNext,
                    _frameButtonToLast,
                });

            _layoutRootButtonBackAndMoveNavigator.Update();
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutRootHeader?.Destroy();
            _layoutRootButtonBackAndMoveNavigator?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("HistoryDetailMax", _layoutRootHeader.Area.AsYReversed()),
                ("AreaButtonInfo", AreaButtonInfo),
                ("AreaButtonReplayMode", AreaButtonReplayMode),
                ("AreaButtonBackAndMoveNavigator", _layoutRootButtonBackAndMoveNavigator.Area.AsYReversed()),
                ("AreaTextMove", AreaTextMove),
                ("AreaMoveNavigator", _frameMoveNavigator.Area.AsYReversed()),
                ("AreaButtonBack", AreaButtonBack),
                ("AreaButtonStepForward", AreaButtonNext),
                ("AreaButtonStepBack", AreaButtonPrevious),
                ("AreaButtonJumpToFirst", AreaButtonToFirst),
                ("AreaButtonJumpToLast", AreaButtonToLast),
            });
        }
    }
}
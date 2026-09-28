using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class GameResultLayouter : PLayouter
    {
        private LayoutRoot _layoutGameResult;


        private LayoutVStack _vStackWhole;

        private LayoutFrame _frameKeyVisual;
        public FArea AreaKeyVisual => _frameKeyVisual.Area.AsYReversed();
        

        private LayoutFrame _frameRate;
        public FArea AreaRate => _frameRate.Area.AsYReversed();


        private LayoutFrame _frameButtonArea;

        private LayoutVStack _vStackButtonArea;
        private LayoutFrame _frameButtonArea1;
        private LayoutFrame _frameButtonArea2;
        private LayoutFrame _frameButtonArea3;


        private LayoutFrame _frameButtonShowBoard;
        public FArea AreaButtonShowBoard => _frameButtonShowBoard.Area.AsYReversed();

        // ここを実装する
        public FArea AreaButtonDetail => new FArea(origin: new Vector2(0, 0), size: new Vector2(0.1f, 0.1f));

        private LayoutFrame _frameButtonHisotryDetail;
        public FArea AreaButtonHistoryDetail => _frameButtonHisotryDetail.Area.AsYReversed();

        private LayoutFrame _frameButtonNextGame;
        public FArea AreaButtonNextGame => _frameButtonNextGame.Area.AsYReversed();

        private LayoutFrame _frameButtonHome;
        public FArea AreaButtonHome => _frameButtonHome.Area.AsYReversed();


        private int _fontSizeReason = 0;
        public int FontSizeGameResultReason => _fontSizeReason;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public GameResultLayouter()
        {
            Setup();
        }


        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup()
        {
            FArea areaGameResult = Locator.Instance.layoutManager.AreaMainContentsWithoutStatusBar.AsYReversed();
            _layoutGameResult = new LayoutRoot(area: areaGameResult, spacing: FVec4.ZERO);

            _vStackWhole = new LayoutVStack(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                innerSpacing: 0,
                innerAlign: EDirection.UP,
                offset: new Vector2(0, 0));

            _layoutGameResult.RegisterChildByList(newChildList: new List<PLayout> { _vStackWhole });


            _frameKeyVisual = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.5f)),
                align: EDirection.STAY, // 無視される
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameRate = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 0.1f)),
                align: EDirection.STAY, // 無視される
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.8f, 0.4f)),
                align: EDirection.STAY, // 無視される
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));


            _vStackWhole.RegisterChildByList(newChildList: new List<PLayout> {
                _frameKeyVisual,
                _frameRate,
                _frameButtonArea
                });



            _vStackButtonArea = new LayoutVStack(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 1)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                innerSpacing: 0,
                innerAlign: EDirection.UP,
                offset: new Vector2(0, 0));

            _frameButtonArea.RegisterChildByList(newChildList: new List<PLayout> { _vStackButtonArea });


            _frameButtonArea1 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.333f)),
                align: EDirection.STAY, // 無視される
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea2 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.333f)),
                align: EDirection.STAY, // 無視される
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea3 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.333f)),
                align: EDirection.STAY, // 無視される
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));


            _vStackButtonArea.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonArea1,
                _frameButtonArea2,
                _frameButtonArea3,
                });

            _layoutGameResult.Update();


            var baseWidth = _frameButtonArea1.Area.size.x;
            var baseHeight = Locator.Instance.layoutManager.ButtonBaseHeight;


            _frameButtonShowBoard = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(baseWidth * 0.45f, baseHeight)),
                align: EDirection.UP_LEFT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonHisotryDetail = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(baseWidth * 0.45f, baseHeight)),
                align: EDirection.UP_RIGHT,
                priority: 2,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea1.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonShowBoard,
                _frameButtonHisotryDetail
                });


            

            _frameButtonNextGame = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(baseWidth * 0.8f, baseHeight * 1.25f)),
                align: EDirection.STAY,
                priority: 1,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea2.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonNextGame,
                });


            _frameButtonHome = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(baseWidth * 0.8f, baseHeight)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonArea3.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonHome,
                });


            _layoutGameResult.Update();


            // TODO: 計算する
            _fontSizeReason = 50;
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutGameResult?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("AreaKeyVisual", AreaKeyVisual),
                ("AreaRate", AreaRate),
                //("FrameButtonArea", _frameButtonArea.Area.AsYReversed()),
                //("FrameButtonArea1", _frameButtonArea1.Area.AsYReversed()),
                //("FrameButtonArea2", _frameButtonArea2.Area.AsYReversed()),
                //("FrameButtonArea3", _frameButtonArea3.Area.AsYReversed()),
                ("AreaButtonShowBoard", AreaButtonShowBoard),
                ("AreaButtonDetail", AreaButtonHistoryDetail),
                ("AreaButtonNextGame", AreaButtonNextGame),
                ("AreaButtonHome", AreaButtonHome),
            });
        }
    }
}
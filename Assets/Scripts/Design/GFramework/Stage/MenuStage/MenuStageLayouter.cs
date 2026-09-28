using System.Collections;
using System.Collections.Generic;
using GFramework;
using UnityEngine;

namespace GFramework
{
    public class MenuStageLayouter : PLayouter
    {
        // -----------------------------------------------
        // MARK: - Property

        private LayoutRoot _layoutRootMenuStage;

        private LayoutFrame _frameTitleLogoAreaMax;
        private LayoutFrame _frameButtonAreaMax;
        private LayoutFrame _frameButtonAreaUpper;
        private LayoutFrame _frameButtonAreaSpacer;
        private LayoutFrame _frameButtonAreaLower;

        private LayoutFrame _frameTitleLogo;
        public FArea AreaTitleLogo => _frameTitleLogo.Area.AsYReversed();

        private LayoutFrame _frameButtonGame66;
        public FArea AreaButtonGame66 => _frameButtonGame66.Area.AsYReversed();
        public FArea AreaButtonGame66CompanionImage => _frameButtonGame66.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(-0.065f, -0.18f),
            endPointRatio: new Vector2(0.18f, 1.2f)).AsYReversed();

        private LayoutFrame _frameButtonGame99;
        public FArea AreaButtonGame99 => _frameButtonGame99.Area.AsYReversed();
        public FArea AreaButtonGame99CompanionImage => _frameButtonGame99.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(-0.065f, -0.18f),
            endPointRatio: new Vector2(0.18f, 1.2f)).AsYReversed();

        private LayoutFrame _frameButtonGameWithFriend;
        public FArea AreaButtonGameWithFriend => _frameButtonGameWithFriend.Area.AsYReversed();
        public FArea AreaButtonGameWithFriendCompanionImage => _frameButtonGameWithFriend.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(-0.065f, -0.18f),
            endPointRatio: new Vector2(0.18f, 1.2f)).AsYReversed();


        private LayoutFrame _frameButtonMypage;
        public FArea AreaButtonMypage => _frameButtonMypage.Area.AsYReversed();
        public FArea AreaButtonMypageWithoutRanking => GetAreaButtonMypageWithoutRanking();

        private LayoutFrame _frameButtonBlank;
        public FArea AreaButtonBlank => _frameButtonBlank.Area.AsYReversed();

        private LayoutFrame _frameButtonSetting;
        public FArea AreaButtonSetting => _frameButtonSetting.Area.AsYReversed();

        private LayoutFrame _frameButtonRanking;
        public FArea AreaButtonRanking => _frameButtonRanking.Area.AsYReversed();

        public FArea AreaButtonDebugMenu => _frameTitleLogo.Area.GetPartialAreaByOriginAndEndPoint(
            originRatio: new Vector2(0.65f, 0.05f),
            endPointRatio: new Vector2(0.95f, 0.16f)).AsYReversed();


        // ===============================================
        // MARK: - ● Constructor ─────

        public MenuStageLayouter()
        {
            Setup();
            // ShowLayoutArea();
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void Setup()
        {
            FArea areaMenuStage = Locator.Instance.layoutManager.AreaMainContentsWithoutStatusBar.AsYReversed();
            _layoutRootMenuStage = new LayoutRoot(size: areaMenuStage.size, spacing: new FVec4(0, 0, 0, 0), position: areaMenuStage.origin);

            _frameTitleLogoAreaMax = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 0.475f)),
                align: EDirection.UP,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonAreaMax = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1, 0.525f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _layoutRootMenuStage.RegisterChildByList(newChildList: new List<PLayout> {
                _frameTitleLogoAreaMax,
                _frameButtonAreaMax});

            _layoutRootMenuStage.Update();


            // ------------------

            _frameTitleLogo = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 1f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameTitleLogoAreaMax.RegisterChildByList(newChildList: new List<PLayout> {
                _frameTitleLogo});

            _layoutRootMenuStage.Update();


            // ------------------

            _frameButtonAreaUpper = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.9f, 0.625f)),
                align: EDirection.UP,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonAreaSpacer = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.9f, 0.0735f)),
                align: EDirection.UP,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonAreaLower = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.9f, 0.315f)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            _frameButtonAreaMax.RegisterChildByList(newChildList: new List<PLayout> {
                    _frameButtonAreaUpper,
                    _frameButtonAreaSpacer,
                    _frameButtonAreaLower});

            _layoutRootMenuStage.Update();


            // ------------------

            var buttonSizeForGame66 = new Vector2(_frameButtonAreaUpper.Size.x, Locator.Instance.layoutManager.ButtonBaseHeight * 1.25f);

            _frameButtonGame66 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSizeForGame66),
                align: EDirection.UP,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            var buttonSizeForGame99 = new Vector2(_frameButtonAreaUpper.Size.x, Locator.Instance.layoutManager.ButtonBaseHeight * 1.25f);

            _frameButtonGame99 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSizeForGame99),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            var buttonSizeForGameWithFriend = new Vector2(_frameButtonAreaUpper.Size.x, Locator.Instance.layoutManager.ButtonBaseHeight) * 1.25f;

            _frameButtonGameWithFriend = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSizeForGameWithFriend),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));


            _frameButtonAreaUpper.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonGame66,
                _frameButtonGame99,
                _frameButtonGameWithFriend});

            _layoutRootMenuStage.Update();



            // ------------------

            var buttonSizeForOthers = new Vector2(_frameButtonAreaLower.Size.x * 0.49f, Locator.Instance.layoutManager.ButtonBaseHeight);


            _frameButtonMypage = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSizeForOthers),
                align: EDirection.UP_LEFT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

             _frameButtonBlank = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSizeForOthers),
                align: EDirection.UP_RIGHT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            var buttonSizeForRanking = new Vector2(_frameButtonAreaLower.Size.x * 0.59f, Locator.Instance.layoutManager.ButtonBaseHeight);
            _frameButtonRanking = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSizeForRanking),
                align: EDirection.DOWN_LEFT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));

            var buttonSizeForSetting = new Vector2(_frameButtonAreaLower.Size.x * 0.39f, Locator.Instance.layoutManager.ButtonBaseHeight);
            _frameButtonSetting = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: buttonSizeForSetting),
                align: EDirection.DOWN_RIGHT,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));




            _frameButtonAreaLower.RegisterChildByList(newChildList: new List<PLayout> {
                _frameButtonMypage,
                _frameButtonBlank,
                _frameButtonSetting,
                _frameButtonRanking});

            _layoutRootMenuStage.Update();
        }


        // ===============================================
        // MARK: - ● Destroy ─────

        public FArea GetAreaButtonMypageWithoutRanking()
        {
            var areaButtonMypage = _frameButtonMypage.Area.AsYReversed();
            var areaButtonBlank = _frameButtonBlank.Area.AsYReversed();

            return new FArea(
                p1: areaButtonMypage.origin,
                p2: areaButtonBlank.EndPoint,
                areaConstructorType: FArea.EAreaConstructorType.ORIGIN_ENDPOINT);
        }


        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutRootMenuStage?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("TitleLogoAreaMax", _frameTitleLogoAreaMax.Area.AsYReversed()),
                ("TitleLogoArea", AreaTitleLogo),
                ("ButtonAreaMax", _frameButtonAreaMax.Area.AsYReversed()),
                ("ButtonAreaUpper", _frameButtonAreaUpper.Area.AsYReversed()),
                ("ButtonAreaLower", _frameButtonAreaLower.Area.AsYReversed()),
                ("ButtonGame66", AreaButtonGame66),
                ("ButtonGame99", AreaButtonGame99),
                ("ButtonGameWithFriend", AreaButtonGameWithFriend),
                ("ButtonMypage", AreaButtonMypage),
                ("ButtonSetting", AreaButtonSetting),
                ("ButtonRanking", AreaButtonRanking),
                ("ButtonMypageWithoutRanking", AreaButtonMypageWithoutRanking),
                ("ButtonDebugMenu", AreaButtonDebugMenu)
            });
        }
    }
}
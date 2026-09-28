using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class GameResultRateInfoLayouter : PLayouter
    {
        private LayoutRoot _layoutRateInfoMax;

        private LayoutFrame _rateInfo;

        public FArea AreaRateInfo => _rateInfo.Area.AsYReversed();
        public FArea AreaNextRate => CalcAreaNextRate();
        public FArea AreaArrow => CalcAreaArrow();
        public FArea AreaDeltaRate => CalcAreaDeltaRate();
        public FArea AreaGradeRankIcon => CalcAreaGradeRankIcon();
        public FArea AreaGradeRankCharacter => CalcAreaGradeRankCharacter();


        // ===============================================
        // MARK: - ● Setup Component ─────

        public GameResultRateInfoLayouter(FArea area)
        {
            Setup(area: area);

            //ShowLayoutArea();
        }



        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup(FArea area)
        {
            _layoutRateInfoMax = new LayoutRoot(area: area, spacing: FVec4.ZERO);

            var areaTemp = _layoutRateInfoMax.Area.GetPartialArea(Config.GAME_RESULT_RATE_RESULT_BODY__IMAGE_SIZE.GetXtoYRatio());

            _rateInfo = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: areaTemp.size),
                align: EDirection.STAY,
                priority: 0,
                spacing: new FVec4(0, 0, 0, 0),
                offset: new Vector2(0, 0));


            _layoutRateInfoMax.RegisterChildByList(newChildList: new List<PLayout> {
                _rateInfo,});

            _layoutRateInfoMax.Update();
        }



        // ===============================================
        // MARK: - ● Calc Area ─────

        private FArea CalcAreaNextRate()
        {
            return _rateInfo.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.225f, 0.15f),
                endPointRatio: new Vector2(0.525f, 0.85f)).AsYReversed();
        }


        private FArea CalcAreaArrow()
        {
            return _rateInfo.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.525f, 0.2f),
                endPointRatio: new Vector2(0.6f, 0.8f)).AsYReversed();
        }


        private FArea CalcAreaDeltaRate()
        {
            return _rateInfo.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.6f, 0.2f),
                endPointRatio: new Vector2(0.775f, 0.8f)).AsYReversed();
        }

        private FArea CalcAreaGradeRankIcon()
        {
            return _rateInfo.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.775f, 0.1f),
                endPointRatio: new Vector2(0.88f, 0.9f)).AsYReversed();
        }

        private FArea CalcAreaGradeRankCharacter()
        {
            return _rateInfo.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.88f, 0.28f),
                endPointRatio: new Vector2(0.94f, 0.88f)).AsYReversed();
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutRateInfoMax?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("AreaRateInfoMax", _layoutRateInfoMax.Area.AsYReversed()),
                ("AreaRateInfo", AreaRateInfo),
                ("AreaNextRate", AreaNextRate),
                ("AreaArrow", AreaArrow),
                ("AreaDeltaRate", AreaDeltaRate),
                ("AreaGradeRankIcon", AreaGradeRankIcon),
                ("AreaGradeRankCharacter", AreaGradeRankCharacter),
            });
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace GFramework
{
    public class HistoryItemLayouter : PLayouter
    {
        private LayoutRoot _layoutRoot;

        private LayoutFrame _frameTapArea;

        private LayoutFrame _frameMainContents;


        // ===============================================
        // MARK: - ● Constructor ─────

        public HistoryItemLayouter(in FArea area)
        {
            Setup(area: area);

            //ShowLayoutArea();
        }


        // ===============================================
        // MARK: - ● Layout ─────

        private void Setup(in FArea area)
        {
            _layoutRoot = new LayoutRoot(area: area, spacing: FVec4.ZERO);

            float singleSpacing = Locator.Instance.layoutManager.AreaWindow.size.x * 0.03f;

            _frameTapArea = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 0.95f)),
                align: EDirection.UP,
                priority: 0,
                spacing: new FVec4(singleSpacing),
                offset: new Vector2(0, 0));

            _frameMainContents = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1f, 1f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: new Vector2(0, 0));


            _layoutRoot.RegisterChildByList(
                newChildList: new List<PLayout> { _frameTapArea });

            _frameTapArea.RegisterChildByList(
                newChildList: new List<PLayout> { _frameMainContents });

            // 一度計算
            _layoutRoot.Update();
        }



        // ===============================================
        // MARK: - ● GetArea ─────

        public FArea GetAreaTapArea()
        {
            return _frameTapArea.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0, 0),
                endPointRatio: new Vector2(1f, 1f));
        }


        public FArea GetAreaTextGameNum()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.8f, 0.75f),
                endPointRatio: new Vector2(1f, 1f));
        }


        public FArea GetAreaTextDate()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.25f, 0),
                endPointRatio: new Vector2(0.8f, 0.25f));
        }


        public FArea GetAreaTextDateAdditionalInfo()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.8f, 0),
                endPointRatio: new Vector2(1f, 0.25f));
        }


        public FArea GetAreaResultIcon()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0f, 0f),
                endPointRatio: new Vector2(0.22f, 1f));
        }


        public FArea GetAreaTextOpponentName()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.25f, 0.25f),
                endPointRatio: new Vector2(1f, 0.7f));
        }


        public FArea GetAreaBoardTypeIcon()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.25f, 0.69f),
                endPointRatio: new Vector2(0.425f, 1.02f));
        }

        public FArea GetAreaTextRate()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.450f, 0.7f),
                endPointRatio: new Vector2(0.58f, 1f));
        }

        public FArea GetAreaGradeRankIcon()
        {
            return _frameMainContents.Area.GetPartialAreaByOriginAndEndPoint(
                originRatio: new Vector2(0.58f, 0.72f),
                endPointRatio: new Vector2(0.68f, 0.98f));
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
                ("HistoryItemMax", _layoutRoot.Area.AsYReversed()),
                ("HistoryItemTapArea", GetAreaTapArea().AsYReversed()),
                ("frameMainContents", _frameMainContents.Area.AsYReversed()),
                ("HistoryItemTextGameNum",  GetAreaTextGameNum().AsYReversed()),
                ("HistoryItemTextDate",  GetAreaTextDate().AsYReversed()),
                ("HistoryItemResult", GetAreaResultIcon().AsYReversed()),
                ("HistoryItemTextDateAdditionalInfo", GetAreaTextDateAdditionalInfo().AsYReversed()),
                ("HistoryItemTextOpponentName", GetAreaTextOpponentName().AsYReversed()),
                ("HistoryItemBoardType", GetAreaBoardTypeIcon().AsYReversed()),
                ("HistoryItemTextRate", GetAreaTextRate().AsYReversed()),
                ("HistoryItemTextGradeRankIcon", GetAreaGradeRankIcon().AsYReversed()),
            });
        }
    }
}
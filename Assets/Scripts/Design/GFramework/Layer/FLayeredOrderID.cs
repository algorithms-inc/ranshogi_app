using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    /// <summary>
    /// 表示レイヤー種別
    /// </summary>
    public enum ELayerType
    {
        BACKGROUND,
        MENU_STAGE,
        BOARD,
        PIECE,
        STATUS_BAR,
        STAGE_INPUT,
        OPPONENT_SEARCHER,
        PROMOTION_SELECTOR,
        RESIGN_SELECTOR,
        GAME_STAGE_TEXT_BALLOON,
        GAME_RESULT,
        HISTORY_STAGE,
        HISTORY_DETAIL_STAGE,
        WINDOW_TOP,
        WINDOW_NORMAL,
        WINDOW_CONFIRMATION,
        WINDOW_ERROR,
        SCREEN_EDGE,
        LAYOUT
    }


    /// <summary>
    /// 表示レイヤーと、そのレイヤー内での表示順序
    /// </summary>
    public struct FLayeredOrderID
    {
        private ELayerType _layerType;
        public ELayerType LayerType => _layerType;

        private int _orderID;
        public int OrderID => _orderID;

        public int SortingOrder
        {
            get { return ConvertToSortingOrder(); }
        }

        public FLayeredOrderID(ELayerType layerType, int orderID)
        {
            _layerType = layerType;
            _orderID = orderID;
        }

        public void SetOrderID(int value)
        {
            _orderID = value;
        }

        public FLayeredOrderID Increment()
        {
            _orderID += 1;

            return this;
        }

        public FLayeredOrderID Increment(int delta)
        {
            _orderID += delta;

            return this;
        }

        private int ConvertToSortingOrder()
        {
            Assert.IsTrue(OrderID < Config.ORDERID_GAP_BETWEEN_LAYER);


            int baseOrderID = LayerType switch
            {
                ELayerType.BACKGROUND => 0,
                ELayerType.MENU_STAGE => Config.ORDERID_GAP_BETWEEN_LAYER * 1,
                ELayerType.BOARD => Config.ORDERID_GAP_BETWEEN_LAYER * 2,
                ELayerType.PIECE => Config.ORDERID_GAP_BETWEEN_LAYER * 3,
                ELayerType.STATUS_BAR => Config.ORDERID_GAP_BETWEEN_LAYER * 4,
                ELayerType.STAGE_INPUT => Config.ORDERID_GAP_BETWEEN_LAYER * 5,
                ELayerType.OPPONENT_SEARCHER => Config.ORDERID_GAP_BETWEEN_LAYER * 6,
                ELayerType.PROMOTION_SELECTOR => Config.ORDERID_GAP_BETWEEN_LAYER * 7,
                ELayerType.RESIGN_SELECTOR => Config.ORDERID_GAP_BETWEEN_LAYER * 8,
                ELayerType.GAME_STAGE_TEXT_BALLOON => Config.ORDERID_GAP_BETWEEN_LAYER * 9,
                ELayerType.GAME_RESULT => Config.ORDERID_GAP_BETWEEN_LAYER * 10,
                ELayerType.HISTORY_STAGE => Config.ORDERID_GAP_BETWEEN_LAYER * 11,
                ELayerType.HISTORY_DETAIL_STAGE => Config.ORDERID_GAP_BETWEEN_LAYER * 12,
                ELayerType.WINDOW_TOP => Config.ORDERID_GAP_BETWEEN_LAYER * 13,
                ELayerType.WINDOW_NORMAL => Config.ORDERID_GAP_BETWEEN_LAYER * 14,
                ELayerType.WINDOW_CONFIRMATION => Config.ORDERID_GAP_BETWEEN_LAYER * 15,
                ELayerType.WINDOW_ERROR => Config.ORDERID_GAP_BETWEEN_LAYER * 16,
                ELayerType.SCREEN_EDGE => Config.ORDERID_GAP_BETWEEN_LAYER * 17,
                ELayerType.LAYOUT => Config.ORDERID_GAP_BETWEEN_LAYER * 18,
                _ => throw new NotImplementedException(),
            };

            return baseOrderID + OrderID;
        }
    }
}
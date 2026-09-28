using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


// TODO: レート数字表示エリアを足す

namespace GFramework
{
    public class StatusBarLayouter : PLayouter
    {
        private LayoutRoot _layoutRootStatusBarPlayer2Max;
        private LayoutRoot _layoutRootStatusBarPlayer1Max;

        private LayoutFrame _frameStatusBarPlayer2;
        private LayoutFrame _frameStatusBarPlayer1;

        private LayoutFrame _frameStatusBarPlayer2Main;
        private LayoutFrame _frameStatusBarPlayer1Main;


        // parameter

        private float _widthRatioLifeArea = 0f;
        private float _widthRatioLife = 0f;
        private float _widthRatioMoveOrder = 0f;
        private float _widthRatioTimeLeft = 0f;

        private float _widthRatioSpacerTimeLeftAndMoveOrder = 0f;
        private float _widthRatioSpacerLifeAndName = 0f;

        private float _widthRatioNameP1 = 0f;
        private float _widthRatioNameP2 = 0f;



        public int FontSizeStatusBar => (int)(GetAreaPlayerName(playerType: EPlayerType.PLAYER1).AsYReversed().size.y * 0.8f);



        // ===============================================
        // MARK: - ● Setup Component ─────

        public StatusBarLayouter(int countOfLife)
        {
            CalcParameters(countOfLife: countOfLife);

            Setup();

            //ShowLayoutArea();
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void CalcParameters(int countOfLife)
        {
            _widthRatioLifeArea = 0.225f;
            _widthRatioLife = _widthRatioLifeArea / countOfLife;
            _widthRatioMoveOrder = 0.07f;
            _widthRatioTimeLeft = 0.2f;

            _widthRatioSpacerTimeLeftAndMoveOrder = 0.01f;
            _widthRatioSpacerLifeAndName = 0.02f;

            _widthRatioNameP1 = 1f - _widthRatioMoveOrder - _widthRatioTimeLeft - _widthRatioSpacerTimeLeftAndMoveOrder - _widthRatioLife * countOfLife - _widthRatioSpacerLifeAndName;
            _widthRatioNameP2 = 1f - _widthRatioMoveOrder - _widthRatioTimeLeft - _widthRatioSpacerTimeLeftAndMoveOrder;
        }


        private void Setup()
        {
            _layoutRootStatusBarPlayer2Max = new LayoutRoot(
                area: Locator.Instance.layoutManager.GetAreaStatusBar(playerType: EPlayerType.PLAYER2).AsYReversed(),
                spacing: FVec4.ZERO);

            _layoutRootStatusBarPlayer1Max = new LayoutRoot(
                area: Locator.Instance.layoutManager.GetAreaStatusBar(playerType: EPlayerType.PLAYER1).AsYReversed(),
                spacing: FVec4.ZERO);


            _frameStatusBarPlayer2 = new LayoutFrame(
               sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.95f, 0.9f)),
               align: EDirection.UP,
               priority: 0,
               spacing: FVec4.ZERO,
               offset: new Vector2(0, 0));

            _frameStatusBarPlayer1 = new LayoutFrame(
               sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.95f, 0.9f)),
               align: EDirection.DOWN,
               priority: 0,
               spacing: FVec4.ZERO,
               offset: new Vector2(0, 0));


            _layoutRootStatusBarPlayer2Max.RegisterChildByList(newChildList: new List<PLayout> { _frameStatusBarPlayer2 });
            _layoutRootStatusBarPlayer1Max.RegisterChildByList(newChildList: new List<PLayout> { _frameStatusBarPlayer1 });

            // 一度計算する
            _layoutRootStatusBarPlayer2Max.Update();
            _layoutRootStatusBarPlayer1Max.Update();




            _frameStatusBarPlayer2Main = new LayoutFrame(
               sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.97f, 0.95f)),
               align: EDirection.STAY,
               priority: 0,
               spacing: FVec4.ZERO,
               offset: new Vector2(0, 0));

            _frameStatusBarPlayer1Main = new LayoutFrame(
               sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.97f, 0.95f)),
               align: EDirection.STAY,
               priority: 0,
               spacing: FVec4.ZERO,
               offset: new Vector2(0, 0));


            _frameStatusBarPlayer2.RegisterChildByList(newChildList: new List<PLayout> { _frameStatusBarPlayer2Main });
            _frameStatusBarPlayer1.RegisterChildByList(newChildList: new List<PLayout> { _frameStatusBarPlayer1Main });

            // 一度計算する
            _layoutRootStatusBarPlayer2Max.Update();
            _layoutRootStatusBarPlayer1Max.Update();
        }



        // ===============================================
        // MARK: - ● Get Area ─────

        public FArea GetAreaStatusBar(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => _frameStatusBarPlayer1.Area.AsYReversed(),
                EPlayerType.PLAYER2 => _frameStatusBarPlayer2.Area.AsYReversed(),
                _ => throw new InvalidOperationException(),
            };
        }


        public FArea GetAreaStatusBarMain(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => _frameStatusBarPlayer1Main.Area.AsYReversed(),
                EPlayerType.PLAYER2 => _frameStatusBarPlayer2Main.Area.AsYReversed(),
                _ => throw new InvalidOperationException(),
            };
        }


        public FArea GetAreaLife(int index)
        {
            return GetAreaStatusBarMain(playerType: EPlayerType.PLAYER1).GetPartialAreaByOriginAndSize(
                originRatio: new Vector2(x: _widthRatioLife * index, y: 0f),
                sizeRatio: new Vector2(x: _widthRatioLife, y: 1f)).AsYReversed();
        }


        public FArea GetAreaEmblem()
        {
            var Life0 = GetAreaLife(index: 0);
            var Life2 = GetAreaLife(index: 2);

            return new FArea(p1: Life0.origin, p2: Life2.EndPoint, areaConstructorType: FArea.EAreaConstructorType.ORIGIN_ENDPOINT).AsYReversed();
        }


        public FArea GetAreaMoveOrder(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => GetAreaStatusBarMain(playerType: EPlayerType.PLAYER1).GetPartialAreaByOriginAndSize(
                    originRatio: new Vector2(x: 1f - _widthRatioMoveOrder, y: 0f),
                    sizeRatio: new Vector2(x: _widthRatioMoveOrder, y: 1f)).AsYReversed(),

                EPlayerType.PLAYER2 => GetAreaStatusBarMain(playerType: EPlayerType.PLAYER2).GetPartialAreaByOriginAndSize(
                    originRatio: new Vector2(x: 0f, y: 0f),
                    sizeRatio: new Vector2(x: _widthRatioMoveOrder, y: 1f)).AsYReversed(),

                _ => throw new InvalidOperationException(),
            };
        }


        public FArea GetAreaPlayerName(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => GetAreaStatusBarMain(playerType: EPlayerType.PLAYER1).GetPartialAreaByOriginAndSize(
                    originRatio: new Vector2(x: _widthRatioLifeArea + _widthRatioSpacerLifeAndName, y: 0f),
                    sizeRatio: new Vector2(x: _widthRatioNameP1, y: 1f)).AsYReversed(),

                EPlayerType.PLAYER2 => GetAreaStatusBarMain(playerType: EPlayerType.PLAYER2).GetPartialAreaByOriginAndSize(
                    originRatio: new Vector2(x: 1f - _widthRatioNameP2, y: 0f),
                    sizeRatio: new Vector2(x: _widthRatioNameP2, y: 1f)).AsYReversed(),

                _ => throw new InvalidOperationException(),
            };
        }


        public FArea GetAreaTimeLeft(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => GetAreaStatusBarMain(playerType: EPlayerType.PLAYER1).GetPartialAreaByOriginAndSize(
                    originRatio: new Vector2(x: 1f - _widthRatioMoveOrder - _widthRatioSpacerTimeLeftAndMoveOrder - _widthRatioTimeLeft, y: 0f),
                    sizeRatio: new Vector2(x: _widthRatioTimeLeft, y: 1f)).AsYReversed(),

                EPlayerType.PLAYER2 => GetAreaStatusBarMain(playerType: EPlayerType.PLAYER2).GetPartialAreaByOriginAndSize(
                    originRatio: new Vector2(x: _widthRatioMoveOrder + _widthRatioSpacerTimeLeftAndMoveOrder, y: 0f),
                    sizeRatio: new Vector2(x: _widthRatioTimeLeft, y: 1f)).AsYReversed(),

                _ => throw new InvalidOperationException(),
            };
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            _layoutRootStatusBarPlayer2Max?.Destroy();
            _layoutRootStatusBarPlayer1Max?.Destroy();
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                    //("StatusBarPlayer2Max", _layoutRootStatusBarPlayer2Max.Area.AsYReversed()),
                    //("StatusBarPlayer1Max", _layoutRootStatusBarPlayer1Max.Area.AsYReversed()),
                    //("StatusBarPlayer2", _frameStatusBarPlayer2.Area.AsYReversed()),
                    //("StatusBarPlayer1", _frameStatusBarPlayer1.Area.AsYReversed()),
                    ("StatusBarPlayer2Main", _frameStatusBarPlayer2Main.Area.AsYReversed()),
                    ("StatusBarPlayer1Main", _frameStatusBarPlayer1Main.Area.AsYReversed()),
                    ("Life0", GetAreaLife(index: 0).AsYReversed()),
                    ("Life1", GetAreaLife(index: 1).AsYReversed()),
                    ("Life2", GetAreaLife(index: 2).AsYReversed()),
                    ("Emblem", GetAreaEmblem().AsYReversed()),
                    ("PlayerNamePlayer2", GetAreaPlayerName(playerType: EPlayerType.PLAYER2).AsYReversed()),
                    ("PlayerNamePlayer1", GetAreaPlayerName(playerType: EPlayerType.PLAYER1).AsYReversed()),
                    ("TimeLeftPlayer2", GetAreaTimeLeft(playerType: EPlayerType.PLAYER2).AsYReversed()),
                    ("TimeLeftPlayer1", GetAreaTimeLeft(playerType: EPlayerType.PLAYER1).AsYReversed()),
                    ("MoveOrderPlayer2", GetAreaMoveOrder(playerType: EPlayerType.PLAYER2).AsYReversed()),
                    ("MoveOrderPlayer1", GetAreaMoveOrder(playerType: EPlayerType.PLAYER1).AsYReversed())
                });
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngineInternal;



// TODO: GetArea- 関数 で AsYReversed をしているものとしていないものが混ざっている

namespace GFramework
{
    public sealed class LayoutManager : ILayoutManager
    {
        private bool _isReLayoutable = false;

        // TODO: 便宜上初期値を設定しているが、あやうい
        private EScreenOrientation _presentScreenOrientation = EScreenOrientation.PORTRAIT;

        // スクリーンサイズ
        private Vector2 _presentScreenSize = new Vector2(0, 0);


        // TODO: frameSafeArea をHeader / frameContens / Footer に分ける
        //    -> しかし、footer をHeader より小さくすると frameContens との間に隙間ができる（中央表示なので）よって、やはりVStackで実装し直す必要がある
        // TODO: Contents を frameInfoPlayer1, FrameGameArea, frameInfoPlayer2 に分ける
        // TODO: FrameGameArea を frameWaitinSpacePlayer1, frameBoardAndSpace, frameInfoPlayer2 に分ける
        // TODO: 将来的には、frameHeader, frameFooter に unSafeAreaを含める 



        private GameObject _layoutChecker;



        // メインエリア
        // - デバイスの画面サイズを吸収する x:y = 1:1.6 の表示エリア


        private LayoutVStack _vstack_new;



        // -----------------------------------------------
        // MARK: - 基本フレーム

        /// 画面全体
        private LayoutRoot _frameScreen;

        /// セーフエリア分をスペースとして考慮した画面全体
        private LayoutFrame _frameScreenWithSpacing;

        /// セーフエリア
        private LayoutFrame _frameSafeArea;


        public Vector2 ScaleUICanvas => CalcScaleUICanvas();

        public FArea AreaBackground => _frameScreen.Area.AsYReversed();

        public FArea AreaUICanvas => AreaBackground;

        // -----------------------------------------------
        // MARK: - Window関連フレーム

        private LayoutFrame _frameWindowTop;
        public FArea AreaWindowTop => _frameWindowTop.Area.AsYReversed();


        private LayoutFrame _frameWindow;
        public FArea AreaWindow => _frameWindow.Area.AsYReversed();


        // -----------------------------------------------
        // MARK: - Stage関連フレーム

        /// ステージ全体
        private LayoutFrame _frameMainContents;
        public FArea AreaMainContents => _frameMainContents.Area.AsYReversed();

        /// ステージ全体からプレイヤー1情報欄とその上のスペースを削除したもの
        public FArea AreaMainContentsWithoutStatusBar => CalcAreaMainContentsWithoutStatusBar();


        /// ステージ全体のVStack
        private LayoutVStack _VStackStageMainContents;

        /// ステージのヘッダ
        private LayoutFrame _frameGameStageHeader;
        public FArea AreaGameStageHeader => _frameGameStageHeader.Area.AsYReversed();

        /// ステージのプレイヤー2情報(最大)
        private LayoutFrame _frameStatusBarPlayer2;
        public FArea AreaStatusBarPlayer2 => _frameStatusBarPlayer2.Area.AsYReversed();

        /// ステージのボードセットエリア(最大)
        private LayoutFrame _frameGameStageBoardSet;
        public FArea AreaGameStageBoardSet => _frameGameStageBoardSet.Area.AsYReversed();

        /// ボードのコントロールおよびメッセージ表示(最大)
        private LayoutFrame _frameGameStageBoardControlAndMessage;
        public FArea AreaGameStageBoardControlAndMessage => _frameGameStageBoardControlAndMessage.Area.AsYReversed();

        /// ステージのプレイヤー1情報(最大)
        private LayoutFrame _frameStatusBarPlayer1;
        public FArea AreaStatusBarPlayer1 => _frameStatusBarPlayer1.Area.AsYReversed();


        private LayoutFrame _frameStageSpacer1;
        private LayoutFrame _frameStageSpacer2;
        private LayoutFrame _frameStageSpacer3;
        private LayoutFrame _frameStageSpacer4;



        private EWaitingSpaceType _waitingSpaceType = EWaitingSpaceType.HORIZONTAL;


        // -----------------------------------------------
        // MARK: - Menu Stage 関連フレーム

        /// Menu Stage
        /// - SafeArea から StatusBarPlayer1Maxを除いたエリア
        private LayoutFrame _frameSafeAreaWithoutStatusBar;
        public FArea AreaSafeAreaWithoutStatusBar => _frameSafeAreaWithoutStatusBar.Area.AsYReversed();


        // -----------------------------------------------
        // MARK: - Menu Stage 関連フレーム

        public float ButtonBaseHeight => CalcButtonBaseHeight();



        // ===============================================
        // MARK: - ● Constructor ─────

        public LayoutManager()
        {
            Setup();
        }



        // ===============================================
        // MARK: - ● Destructor ─────

        ~LayoutManager()
        {
            ReleaseLayoutRoot();
        }


        private void ReleaseLayoutRoot()
        {
            if (_frameScreen == null) { return; }

            _frameScreen?.Destroy();
            _frameScreen = null;
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void Setup()
        {
            _presentScreenSize = Utl.GetScreenSize();

            SetupLayoutBase();
            SetupLayoutWindowTop();
            SetupLayoutWindow();
            SetupLayoutStage();
            SetupLayoutSafeAreaWithoutStatusBar(); // SetupLayoutStage() よりも後に行う



            // 念の為再計算
            _frameScreen.Update();

            //-----------------------

            // テストで各種エリアを表示する場合

            //ShowLayoutAreaBase();
            //ShowLayoutAreaStage();
        }



        // ===============================================
        // MARK: - ● Rotatalbe ─────

        public bool IsReLayoutable()
        {
            return _isReLayoutable;
        }


        public void SetIsReLayoutable(bool value)
        {
            _isReLayoutable = value;
        }



        // ===============================================
        // MARK: - ● Check Screen Size and Orientation ─────

        public bool ShouldReLayout(EScreenOrientation newScreenOrientation, Vector2 newScreenSize)
        {
            if (_isReLayoutable == false) { return false; }

            return IsScreenOrientationChanged(newScreenOrientation: newScreenOrientation) || IsScreenSizeChanged(newScreenSize: newScreenSize);
        }


        private bool IsScreenOrientationChanged(EScreenOrientation newScreenOrientation)
        {
            return _presentScreenOrientation == newScreenOrientation;
        }


        private bool IsScreenSizeChanged(Vector2 newScreenSize)
        {
            return _presentScreenSize == newScreenSize;
        }



        // ===============================================
        // MARK: - ● ScreenOrientation ─────

        public EScreenOrientation GetScreenOrientation()
        {
            return _presentScreenOrientation;
        }


        // TODO: UPデートする必要があるかどうかを確認する。
        private void UpdateScreenOrientation()
        {
            if (_isReLayoutable == false) { return; }


            DeviceOrientation presentDeviceOrientation = Utl.GetDeviceOrientation();

            EScreenOrientation newScreenOrientation;

            switch (presentDeviceOrientation)
            {
                case DeviceOrientation.Portrait:
                    newScreenOrientation = EScreenOrientation.PORTRAIT;
                    break;

                case DeviceOrientation.PortraitUpsideDown:
                    // スマートフォンの場合逆さま表示は行わない
                    if (Utl.IsSmartPhone())
                    {
                        newScreenOrientation = EScreenOrientation.PORTRAIT;
                    }
                    else
                    {
                        newScreenOrientation = EScreenOrientation.PORTRAIT_UPSIDEDOWN;
                    }
                    break;

                case DeviceOrientation.LandscapeRight:
                    newScreenOrientation = EScreenOrientation.LANDSCAPE_RIGHT;
                    break;

                case DeviceOrientation.LandscapeLeft:
                    newScreenOrientation = EScreenOrientation.LANDSCAPE_LEFT;
                    break;

                // 不明である場合、平面においている場合
                default:
                    newScreenOrientation = EScreenOrientation.PORTRAIT;
                    break;
            }


            if (newScreenOrientation != _presentScreenOrientation)
            {
                Utl.SetScreenOrientation(value: newScreenOrientation);
                _presentScreenOrientation = newScreenOrientation;
            }

        }







        // ===============================================
        // MARK: - ● Setup Layout Base ─────

        // ひとまず、縦長画面を前提として組む。横長などは後日対応
        private void SetupLayoutBase()
        {
            // スクリーン全体
            FArea screenArea = Utl.GetScreenArea();

            _frameScreen = new LayoutRoot(
                area: screenArea,
                spacing: FVec4.ZERO);


            // セーフエリア
            FArea safeArea = Utl.GetScreenSafeArea();

            // セーフエリアを考慮したスペースを設定
            var space4ForRoot = new FVec4(
                top: safeArea.origin.y - screenArea.origin.y,
                right: screenArea.EndPoint.x - safeArea.EndPoint.x,
                bottom: screenArea.EndPoint.y - safeArea.EndPoint.y,
                left: safeArea.origin.x - screenArea.origin.x);

            _frameScreenWithSpacing = new LayoutFrame(
                sizeSetting: FLayoutSizeSettingByRatio.FULL_SIZE,
                align: EDirection.STAY,
                priority: 0,
                spacing: space4ForRoot,
                offset: Vector2.zero);

            _frameScreen.RegisterChildByList(newChildList: new List<PLayout> { _frameScreenWithSpacing });



            _frameSafeArea = new LayoutFrame(
                sizeSetting: FLayoutSizeSettingByRatio.FULL_SIZE,
                align: EDirection.STAY,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);


            _frameScreenWithSpacing.RegisterChildByList(newChildList: new List<PLayout> { _frameSafeArea });

            _frameScreen.Update();
        }



        // ===============================================
        // MARK: - ● Setup Layout Window ─────

        private void SetupLayoutWindowTop()
        {
            _frameWindowTop = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.95f, 0.3f)),
                align: EDirection.UP,
                priority: 0,
                spacing: Config.WINDOW_BODY__UNRESIZABLE_SPACE4,
                offset: Vector2.zero);

            _frameSafeArea.RegisterChildByList(newChildList: new List<PLayout> { _frameWindowTop });

            _frameScreen.Update();
        }


        private void SetupLayoutWindow()
        {
            _frameWindow = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.9f, 0.8f)),
                align: EDirection.STAY,
                priority: 0,
                spacing: Config.WINDOW_BODY__UNRESIZABLE_SPACE4,
                offset: Vector2.zero);

            _frameSafeArea.RegisterChildByList(newChildList: new List<PLayout> { _frameWindow });

            _frameScreen.Update();
        }



        // ===============================================
        // MARK: - ● Setup Layout Stage ─────

        private void SetupLayoutStage()
        {
            _frameMainContents = new LayoutFrame(
               sizeSetting: new FLayoutSizeSettingByXYRatio(XYRatio: 2.0f),
               align: EDirection.STAY,
               priority: 0,
               spacing: FVec4.ZERO,
               offset: Vector2.zero);

            _frameSafeArea.RegisterChildByList(newChildList: new List<PLayout> { _frameMainContents });


            _VStackStageMainContents = new LayoutVStack(
                sizeSetting: FLayoutSizeSettingByRatio.FULL_SIZE,
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                innerSpacing: 0f,
                innerAlign: EDirection.UP,
                offset: Vector2.zero);

            _frameMainContents.RegisterChildByList(newChildList: new List<PLayout> { _VStackStageMainContents });


            _frameGameStageHeader = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.055f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageSpacer1 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.02f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStatusBarPlayer2 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.05f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageSpacer2 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.005f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameGameStageBoardSet = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.76f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageSpacer3 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.005f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameGameStageBoardControlAndMessage = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.05f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);

            _frameStageSpacer4 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.005f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);



            _frameStatusBarPlayer1 = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(1.0f, 0.05f)),
                align: EDirection.NULL,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);



            _VStackStageMainContents.RegisterChildByList(newChildList: new List<PLayout> {
                _frameGameStageHeader,
                _frameStageSpacer1,
                _frameStatusBarPlayer2,
                _frameStageSpacer2,
                _frameGameStageBoardSet,
                _frameStageSpacer3,
                _frameGameStageBoardControlAndMessage,
                _frameStageSpacer4,
                _frameStatusBarPlayer1 });

            _frameScreen.Update();
        }


        private void SetupLayoutSafeAreaWithoutStatusBar()
        {
            Vector2 safeAreaSize = _frameSafeArea.Area.size;
            float statusBarPlayer1MaxHeight = _frameStatusBarPlayer1.Area.size.y;
            float stageSpacer3Height = _frameStageSpacer3.Area.size.y;

            _frameSafeAreaWithoutStatusBar = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(safeAreaSize.x, safeAreaSize.y - statusBarPlayer1MaxHeight - stageSpacer3Height)),
                align: EDirection.UP,
                priority: 0,
                spacing: FVec4.ZERO,
                offset: Vector2.zero);


            _frameSafeArea.RegisterChildByList(newChildList: new List<PLayout> { _frameSafeAreaWithoutStatusBar });

            _frameScreen.Update();
        }





        
        public void ShowLayoutArea((string, FArea)[] areaList)
        {
            if (_layoutChecker == null)
            {
                _layoutChecker = new GameObject(name: "LayoutChecker");
            }

            var areaID = 0;

            EColorPreset[] colorPresetList = new EColorPreset[] {
                EColorPreset.SYSTEM_BLUE,
                EColorPreset.SYSTEM_CYAN,
                EColorPreset.SYSTEM_GREEN,
                EColorPreset.SYSTEM_MAGENTA,
                EColorPreset.SYSTEM_RED,
                EColorPreset.SYSTEM_YELLOW};

            var countOfcolorPresetList = colorPresetList.Length;

            foreach (var i in areaList)
            {
                var newGameObject = CreateAreaShowingGameObject(
                    name: i.Item1,
                    sortingOrder: Config.BASE_LAYERED_ORDERID_LAYOUT.Increment().SortingOrder,
                    parentTransform: null,
                    area: i.Item2,
                    colorPreset: colorPresetList[areaID % countOfcolorPresetList]);

                newGameObject.transform.parent = _layoutChecker.transform;

                areaID += 1;
            }
        }


        private GameObject CreateAreaShowingGameObject(string name, int sortingOrder, Transform parentTransform, FArea area, EColorPreset colorPreset)
        {
            string texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForDummy.AREA);

            var gameObject = UtlSprite.CreateGameObjectWithSprite(
                name: name,
                texturePathUnderResources: texturePathUnderResources,
                willCacheIfNeeded: true,
                sortingOrder: sortingOrder,
                transformParent: parentTransform);


            Vector2 textureSize = UtlSprite.GetTextureSize(gameObject: gameObject);
            Vector2 scale = area.size / textureSize;

            Vector2 positionByUnit = area.CenterPoint / Config.PIXEL_PER_UNIT;

            gameObject.transform.localScale = scale;
            gameObject.transform.localPosition = positionByUnit;

            gameObject.GetComponent<SpriteRenderer>().material.color = Locator.Instance.colorManager.GetColor3(colorPreset: colorPreset).AsSystemColor();

            return gameObject;
        }



        private void ShowLayoutAreaBase()
        {
            ShowLayoutArea(areaList: new (string, FArea)[] {
                ("Screen", _frameScreen.Area.AsYReversed()),
                ("SafeArea", _frameSafeArea.Area.AsYReversed()),
                ("AreaSafeAreaWithouStatusBar", AreaSafeAreaWithoutStatusBar),
            });
        }


        private void ShowLayoutAreaStage()
        {
            ShowLayoutArea(areaList: new (string, FArea)[] {
                ("AreaMainContents", AreaMainContents),
                ("AreaMainContentsWithouteStatusBar", AreaMainContentsWithoutStatusBar),
                ("AreaGameStageHeader", AreaGameStageHeader),
                ("AreaStatusBarPlayer1", AreaStatusBarPlayer1),
                ("AreaGameStageBoardSet", AreaGameStageBoardSet),
                ("AreaGameStageBoardControlAndMessage", AreaGameStageBoardControlAndMessage),
                ("AreaStatusBarPlayer2", AreaStatusBarPlayer2),
            });
        }


        public void HideLayoutArea()
        {
            if (_layoutChecker == null) { return; }

            GameObject.Destroy(_layoutChecker);
        }


        public EWaitingSpaceType GetWaitingSpaceType()
        {
            return _waitingSpaceType;
        }

        #region GetArea



        public FArea CalcAreaMainContentsWithoutStatusBar()
        {
            var aaa = _frameMainContents.Area.origin;
            var bbbb = _frameGameStageBoardControlAndMessage.Area.EndPoint;
            FArea bbb = new FArea(p1: aaa, p2: bbbb, areaConstructorType: FArea.EAreaConstructorType.ORIGIN_ENDPOINT);

            return bbb.AsYReversed();
        }


        public FArea GetAreaStatusBar(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => AreaStatusBarPlayer1,
                EPlayerType.PLAYER2 => AreaStatusBarPlayer2,
                _ => throw new InvalidOperationException(),
            };
        }



        /// UICanvasのスケールを計算する
        private Vector2 CalcScaleUICanvas()
        {
            var scaleX = 1f / Config.PIXEL_PER_UNIT;
            return new Vector2(scaleX, scaleX);
        }

        #endregion


        public float CalcButtonBaseHeight()
        {
            return (float)(Math.Floor(AreaBackground.size.y * 0.065f));
        }




        public FArea GetAreaOfRipple(ERippleType rippleType)
        {
            var areaBackground = Locator.Instance.layoutManager.AreaBackground.AsYReversed();

            Vector2 positionRatio = new Vector2(x: 0f, y: 0f);
            Vector2 size = new Vector2(x: 0f, y: 0f);


            switch (rippleType)
            {
                case ERippleType.A:
                    positionRatio = new Vector2(x: 0.85f, y: 0.05f);
                    size = new Vector2(x: areaBackground.size.x * 0.3f, y: areaBackground.size.y * 0.3f);
                    break;

                case ERippleType.B:
                    positionRatio = new Vector2(x: 0.6f, y: 0.1f);
                    size = new Vector2(x: areaBackground.size.x * 0.3f, y: areaBackground.size.y * 0.3f);
                    break;

                case ERippleType.C:
                    positionRatio = new Vector2(x: 0.1f, y: 0.9f);
                    size = new Vector2(x: areaBackground.size.x * 0.3f, y: areaBackground.size.y * 0.3f);
                    break;

                case ERippleType.D:
                    positionRatio = new Vector2(x: 0.2f, y: 0.6f);
                    size = new Vector2(x: areaBackground.size.x * 0.3f, y: areaBackground.size.y * 0.3f);
                    break;
            }


            Vector2 centerPoint = areaBackground.size * positionRatio - areaBackground.size / 2f;

            return new FArea(p1: centerPoint.AsYReversed(), p2: size, FArea.EAreaConstructorType.CENTER_SIZE);
        }



    }
}

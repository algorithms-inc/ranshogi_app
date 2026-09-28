using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class TestCaseLayout : ITestCase
    {
        public void ShowBeginLog()
        {
            Debug.Log("Test: " + GetType().Name + ": Begin");
        }

        public void ShowEndLog()
        {
            Debug.Log("Test: " + GetType().Name + ": End");
        }

        public void Execute()
        {
            ShowBeginLog();

            TestFMinMax();
            TestFMinMaxVector();
            TestLayoutRootValue();
            TestLayoutRootRatio();

            ShowEndLog();
        }


        private void TestFMinMax()
        {
            FMinMax a = new FMinMax(min: 50, max: 100, hasMin: true, hasMax: true);

            Assert.IsTrue(a.Clamp(25) == 50);
            Assert.IsTrue(a.Clamp(50) == 50);
            Assert.IsTrue(a.Clamp(75) == 75);
            Assert.IsTrue(a.Clamp(100) == 100);
            Assert.IsTrue(a.Clamp(125) == 100);


            FMinMax b = new FMinMax(min: -1, max: 100, hasMin: false, hasMax: true);

            Assert.IsTrue(b.Clamp(25) == 25);
            Assert.IsTrue(b.Clamp(50) == 50);
            Assert.IsTrue(b.Clamp(75) == 75);
            Assert.IsTrue(b.Clamp(100) == 100);
            Assert.IsTrue(b.Clamp(125) == 100);


            FMinMax c = new FMinMax(min: 50, max: -1, hasMin: true, hasMax: false);

            Assert.IsTrue(c.Clamp(25) == 50);
            Assert.IsTrue(c.Clamp(50) == 50);
            Assert.IsTrue(c.Clamp(75) == 75);
            Assert.IsTrue(c.Clamp(100) == 100);
            Assert.IsTrue(c.Clamp(125) == 125);


            FMinMax d = new FMinMax(min: -1, max: -1, hasMin: false, hasMax: false);

            Assert.IsTrue(d.Clamp(25) == 25);
            Assert.IsTrue(d.Clamp(50) == 50);
            Assert.IsTrue(d.Clamp(75) == 75);
            Assert.IsTrue(d.Clamp(100) == 100);
            Assert.IsTrue(d.Clamp(125) == 125);
        }


        private void TestFMinMaxVector()
        {
            FMinMaxVec a = new FMinMaxVec(
                minMaxX: new FMinMax(min: 50, max: 100, hasMin: true, hasMax: true),
                minMaxY: new FMinMax(min: 75, max: 150, hasMin: true, hasMax: true));


            Assert.IsTrue(a.Clamp(new Vector2(30, 75)) == new Vector2(50, 75));
            Assert.IsTrue(a.Clamp(new Vector2(120, 75)) == new Vector2(100, 75));


            Assert.IsTrue(a.Clamp(new Vector2(60, 45)) == new Vector2(60, 75));
            Assert.IsTrue(a.Clamp(new Vector2(90, 180)) == new Vector2(90, 150));


            Assert.IsTrue(a.Clamp(new Vector2(30, 45)) == new Vector2(50, 75));
            Assert.IsTrue(a.Clamp(new Vector2(120, 180)) == new Vector2(100, 150));


            Assert.IsTrue(a.Clamp(new Vector2(75, 125)) == new Vector2(75, 125));
        }


        private void TestLayoutRootValue()
        {
            LayoutRoot root = new LayoutRoot(
                size: new Vector2(800, 1000),
                spacing: new FVec4(10, 10, 10, 10),
                position: new Vector2(0, 0));


            LayoutFrame frameU = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.UP,
                priority: 8,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0)) ;


            LayoutFrame frameUR = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.UP_RIGHT,
                priority: 6,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            LayoutFrame frameR = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.RIGHT,
                priority: 4,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            LayoutFrame frameDR = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.DOWN_RIGHT,
                priority: 2,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            LayoutFrame frameD = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.DOWN,
                priority: 0,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            LayoutFrame frameDL = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.DOWN_LEFT,
                priority: 1,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            LayoutFrame frameL = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.LEFT,
                priority: 3,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            LayoutFrame frameUL = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.UP_LEFT,
                priority: 5,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            LayoutFrame frameS = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByValue(size: new Vector2(100, 150)),
                align: EDirection.STAY,
                priority: 7,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            List<PLayout> newChildList = new List<PLayout> { frameU, frameUR, frameR, frameDR, frameD, frameDL, frameL, frameUL, frameS };

            root.RegisterChildByList(newChildList: newChildList);
            root.Update();



            // Root の設定が変化していないかの確認

            Assert.IsTrue(root.Size == new Vector2(800, 1000));
            Assert.IsTrue(root.Spacing == new FVec4(10, 10, 10, 10));
            Assert.IsTrue(root.Position == new Vector2(0, 0));



            // 9方向配置の確認

            Assert.IsTrue(frameU.Size == new Vector2(100, 150));
            Assert.IsTrue(frameU.Position == new Vector2(350, 10));


            Assert.IsTrue(frameUR.Size == new Vector2(100, 150));
            Assert.IsTrue(frameUR.Position == new Vector2(690, 10));


            Assert.IsTrue(frameR.Size == new Vector2(100, 150));
            Assert.IsTrue(frameR.Position == new Vector2(690, 425));


            Assert.IsTrue(frameDR.Size == new Vector2(100, 150));
            Assert.IsTrue(frameDR.Position == new Vector2(690, 840));


            Assert.IsTrue(frameD.Size == new Vector2(100, 150));
            Assert.IsTrue(frameD.Position == new Vector2(350, 840));


            Assert.IsTrue(frameDL.Size == new Vector2(100, 150));
            Assert.IsTrue(frameDL.Position == new Vector2(10, 840));


            Assert.IsTrue(frameL.Size == new Vector2(100, 150));
            Assert.IsTrue(frameL.Position == new Vector2(10, 425));


            Assert.IsTrue(frameUL.Size == new Vector2(100, 150));
            Assert.IsTrue(frameUL.Position == new Vector2(10, 10));


            Assert.IsTrue(frameS.Size == new Vector2(100, 150));
            Assert.IsTrue(frameS.Position == new Vector2(350, 425));


            root.Destroy();
        }


        private void TestLayoutRootRatio()
        {
            LayoutRoot root = new LayoutRoot(
                size: new Vector2(800, 1000),
                spacing: new FVec4(10, 10, 10, 10),
                position: new Vector2(0, 0));


            LayoutFrame frameU = new LayoutFrame(
                sizeSetting: new FLayoutSizeSettingByRatio(ratio: new Vector2(0.5f, 0.5f)),
                align: EDirection.UP,
                priority: 8,
                spacing: new FVec4(10, 10, 10, 10),
                offset: new Vector2(0, 0));


            root.RegisterChild(frameU);
            root.Update();



            Assert.IsTrue(frameU.Size == new Vector2(390, 490));
            Assert.IsTrue(frameU.Position == new Vector2(205, 10));


            root.Destroy();
        }
    }

}
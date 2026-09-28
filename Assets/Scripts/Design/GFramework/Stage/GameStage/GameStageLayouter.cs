using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


namespace GFramework
{
    public class GameStageLayouter : PLayouter
    {
        // ===============================================
        // MARK: - ● Property ─────

        public FArea AreaResignButton => CalcAreaResingButton();

        public FArea AreaSettingButton => CalcAreaSettingButton();

        public FArea AreaCompanionImageForSettingButton => CalcAreaCompanionImageForSettingButton();

        public FArea AreaTextBalloonError => CalcAreaTextBalloonError();

        // AI対戦のときのエラーの吹き出し。ヘッダーは「戻る」「待った」「投了」で埋まっているので、盤の上側に出す
        public FArea AreaTextBalloonErrorInAiGame => CalcAreaTextBalloonErrorInAiGame();

        public int FontSizeResignButton => CalcFontSizeResignButton();

        // AI対戦で投了ボタンの左に並べる「戻る」「待った」ボタン。
        // 「待った」は3文字で長いので、「戻る」より幅を広くしている。
        public FArea AreaBackButtonInAiGame => Locator.Instance.layoutManager.AreaGameStageHeader.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.285f, 0.0f), endPointRatio: new Vector2(0.5f, 1f));

        public FArea AreaUndoButtonInAiGame => Locator.Instance.layoutManager.AreaGameStageHeader.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.51f, 0.0f), endPointRatio: new Vector2(0.77f, 1f));


        // ===============================================
        // MARK: - ● Constructor ─────

        public GameStageLayouter()
        {
            Setup();
            //ShowLayoutArea();
        }


        // ===============================================
        // MARK: - ● Setup Layout ─────

        private void Setup()
        {
            // Do Nothing
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public override void Destroy()
        {
            // Do Nothing
        }



        // ===============================================
        // MARK: - ● Calc Area ─────

        private FArea CalcAreaResingButton()
        {
            return Locator.Instance.layoutManager.AreaGameStageHeader.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.78f, 0.0f), endPointRatio: new Vector2(0.98f, 1f));
        }

        private FArea CalcAreaSettingButton()
        {
            return Locator.Instance.layoutManager.AreaGameStageHeader.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.02f, 0.0f), endPointRatio: new Vector2(0.14f, 1f));
        }

        private FArea CalcAreaCompanionImageForSettingButton()
        {
            return CalcAreaSettingButton().GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.25f, 0.25f), endPointRatio: new Vector2(0.75f, 0.75f));
        }

        private FArea CalcAreaTextBalloonError()
        {
            return Locator.Instance.layoutManager.AreaGameStageHeader.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.14f, 0.05f), endPointRatio: new Vector2(0.78f, 0.95f)) ;
        }

        private FArea CalcAreaTextBalloonErrorInAiGame()
        {
            return Locator.Instance.layoutManager.AreaGameStageBoardSet.GetPartialAreaByOriginAndEndPoint(originRatio: new Vector2(0.06f, 0.0f), endPointRatio: new Vector2(0.94f, 0.13f));
        }


        private int CalcFontSizeResignButton()
        {
            return (int)(AreaResignButton.size.y * 0.55f);
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public override void ShowLayoutArea()
        {
            Locator.Instance.layoutManager.ShowLayoutArea(areaList: new (string, FArea)[] {
                ("AreaResignButton", AreaResignButton),
                ("AreaSettingButton", AreaSettingButton),
                ("AreaTextBalloonError", AreaTextBalloonError),
            });
        }
    }
}
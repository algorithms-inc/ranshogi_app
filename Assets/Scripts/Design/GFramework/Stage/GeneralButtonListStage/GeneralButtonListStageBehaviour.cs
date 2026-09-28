using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using GFramework.Core;


namespace GFramework
{
    public class GeneralButtonListStageBehaviour : ImageActionBehaviour, IGeneralButtonListStage
    {
        // ===============================================
        // MARK: - ● Property ─────

        IGeneralButtonListManager _generalButtonListManager = null;

        // -----------------------------------------------
        // MARK: - Game Object

        private Canvas _canvas;

        private bool _isShown = false;
        public bool IsShown => _isShown;

        public override float Alpha => _button0.Alpha;


        private GeneralButtonListStageLayouter _layouter;

        private TextFieldBehaviour _textTitle;

        private List<ButtonBehaviour> _buttonBehaviourList = new List<ButtonBehaviour>();

        private ButtonBehaviour _button0;
        private ButtonBehaviour _button1;
        private ButtonBehaviour _button2;
        private ButtonBehaviour _button3;
        private ButtonBehaviour _button4;
        private ButtonBehaviour _button5;
        private ButtonBehaviour _button6;
        private ButtonBehaviour _button7;
        private ButtonBehaviour _button8;
        private ButtonBehaviour _button9;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(IGeneralButtonListManager generalButtonListManager, string title, List<string> buttonTextList)
        {
            _generalButtonListManager = generalButtonListManager;

            SetupAppManager();

            SetupLayout();

            SetupCanvas();

            SetupTextTitle(title: title);

            SetupButton(buttonTextList: buttonTextList);


            // Stage = Null
            Locator.Instance.audioManager.PlayDefaultStageBGM();
        }


        private void SetupAppManager()
        {
            AppManager.Instance.Setup();
        }


        private void SetupLayout()
        {
            _layouter = new GeneralButtonListStageLayouter();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.MENU_STAGE_INPUT_CANVAS__LAYERED_ORDERID.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupTextTitle(string title)
        {
            var gameObjectText = new GameObject(name: title);
            _textTitle = gameObjectText.AddComponent<TextFieldBehaviour>();

            var text = title;
            FArea area = _layouter.AreaTextTitle;

            var textInitializer = new FTextFieldInitializer(
                text: text,
                fontType: EFontType.BLOCK,
                fontSize: 80,
                alignment: EAlignment.MIDDLE_CENTER,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textTitle.SetupComponent(textInitializer: textInitializer);
        }


        private void SetupButton(List<string> buttonTextList)
        {
            Assert.IsTrue(buttonTextList.Count == 10);

            ButtonBehaviour Support(string name, string buttonText, EButtonImageType buttonImageType, FArea buttonArea, Action action)
            {
                GameObject gameObjectButton = new GameObject(name: name);
                var buttonBehaviour = gameObjectButton.AddComponent<ButtonBehaviour>();

                gameObjectButton.transform.parent = gameObject.transform;

                var buttonInitializer = new FButtonInitializer(
                    targetCanvas: _canvas,
                    buttonImageType: buttonImageType,
                    area: buttonArea,
                    tapJudgeType: ETapJudgeType.CLICK,
                    isPlaySE: true,
                    isEnabled: true,
                    sortingOrder: Config.MENU_STAGE_BUTTON__LAYERED_ORDERID.SortingOrder,
                    action: action);

                buttonBehaviour.SetupComponent(initializer: buttonInitializer);

                var buttonTextInitializer = new FButtonTextInitializer(
                    text: buttonText,
                    fontType: EFontType.SERIF,
                    fontSize: (int)(buttonArea.size.y * 0.4f),
                    alignment: EAlignment.MIDDLE_CENTER);
                buttonBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);


                if (buttonText == "")
                {
                    buttonBehaviour.SetIsEnabled(value: false, willChangeButtonTexture: true);
                }

                return buttonBehaviour;
            }


            _button0 = Support(
                name: "Button0",
                buttonText: buttonTextList[0],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton0,
                action: () => { OnTapButton0(); });
            _buttonBehaviourList.Add(_button0);

            _button1 = Support(
                name: "Button1",
                buttonText: buttonTextList[1],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton1,
                action: () => { OnTapButton1(); });
            _buttonBehaviourList.Add(_button1);

            _button2 = Support(
                name: "Button2",
                buttonText: buttonTextList[2],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton2,
                action: () => { OnTapButton2(); });
            _buttonBehaviourList.Add(_button2);

            _button3 = Support(
                name: "Button3",
                buttonText: buttonTextList[3],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton3,
                action: () => { OnTapButton3(); });
            _buttonBehaviourList.Add(_button3);

            _button4 = Support(
                name: "Button4",
                buttonText: buttonTextList[4],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton4,
                action: () => { OnTapButton4(); });
            _buttonBehaviourList.Add(_button4);

            _button5 = Support(
                name: "Button5",
                buttonText: buttonTextList[5],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton5,
                action: () => { OnTapButton5(); });
            _buttonBehaviourList.Add(_button5);

            _button6 = Support(
                name: "Button6",
                buttonText: buttonTextList[6],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton6,
                action: () => { OnTapButton6(); });
            _buttonBehaviourList.Add(_button6);

            _button7 = Support(
                name: "Button7",
                buttonText: buttonTextList[7],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton7,
                action: () => { OnTapButton7(); });
            _buttonBehaviourList.Add(_button7);

            _button8 = Support(
                name: "Button8",
                buttonText: buttonTextList[8],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton8,
                action: () => { OnTapButton8(); });
            _buttonBehaviourList.Add(_button8);

            _button9 = Support(
                name: "Button9",
                buttonText: buttonTextList[9],
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButton9,
                action: () => { OnTapButton9(); });
            _buttonBehaviourList.Add(_button9);
        }


        // ===============================================
        // MARK: - ● Demo for Layout ─────

        public void OnTapButton0() { _generalButtonListManager.OnTapButton0(); }
        public void OnTapButton1() { _generalButtonListManager.OnTapButton1(); }
        public void OnTapButton2() { _generalButtonListManager.OnTapButton2(); }
        public void OnTapButton3() { _generalButtonListManager.OnTapButton3(); }
        public void OnTapButton4() { _generalButtonListManager.OnTapButton4(); }
        public void OnTapButton5() { _generalButtonListManager.OnTapButton5(); }
        public void OnTapButton6() { _generalButtonListManager.OnTapButton6(); }
        public void OnTapButton7() { _generalButtonListManager.OnTapButton7(); }
        public void OnTapButton8() { _generalButtonListManager.OnTapButton8(); }
        public void OnTapButton9() { _generalButtonListManager.OnTapButton9(); }


        // ===============================================
        // MARK: - ● Change Scene ─────

        // ゲームステージシーンに移動
        private void ChangeScene(string nextStageName)
        {
            SceneManager.LoadScene(nextStageName);
        }



        // ===============================================
        // MARK: - ● Alpha ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            Assert.IsTrue(_isShown == false);

            _textTitle.Show();
            _buttonBehaviourList.ForEach(i => i.Show());

            _isShown = true;
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown == false);

            _textTitle.FadeIn();
            _buttonBehaviourList.ForEach(i => i.FadeIn());

            _isShown = true;
        }


        public override void Hide()
        {
            Assert.IsTrue(_isShown);

            _textTitle.Hide();
            HideButton();

            _isShown = false;
        }

        private void HideButton()
        {
            _buttonBehaviourList.ForEach(i => i.Hide());
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown);

            _textTitle.FadeOut();
            _buttonBehaviourList.ForEach(i => i.FadeOut());

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unity Editor での表示の場合停止タイミングで _layouter が null となるためチェックして破壊
            _layouter?.Destroy();
            _layouter = null;

            _textTitle = null;
            Utl.SafeDestroy(behaviour: _textTitle);

            Utl.SafeDestroy(behaviourList: _buttonBehaviourList);
            _buttonBehaviourList.Clear();

            _canvas = null;
        }



        // as IGeneralButtonListStage

        public void ShowWithAnimation()
        {
            FadeIn();
        }

        public void HideWithAnimationAndChangeScene(string nextSceneName, EStageType nextStageType)
        {
            Assert.IsTrue(_isShown);

            FadeOut();

            // 次のページと曲が異なるならば、曲をフェードする
            Locator.Instance.audioManager.FadeOutBGMIfNeeded(nextStageType: nextStageType, duration: Config.ANIMATION_DURATION_3);

            var action = CAction.CreateActionFire(
                actionBehaviour: this,
                duration: Config.ANIMATION_DURATION_3,
                action: () => { 
                    AppManager.Instance.DestroyStageAndLoadScene(nextSceneName: nextSceneName);
                 });

            RegisterAction(action: action);
        }
    }
}
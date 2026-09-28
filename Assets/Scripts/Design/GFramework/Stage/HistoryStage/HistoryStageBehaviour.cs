using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class HistoryStageBehaviour : ImageActionBehaviour, IHistoryStage
    {
        // ===============================================
        // MARK: - ● Property ─────

        private IHistoryManager _historyManager;


        // -----------------------------------------------
        // MARK: - Game Object

        private Canvas _canvas;

        private bool _isShown = false;
        public bool IsShown => _isShown;

        public override float Alpha => _textPageBehaviour.Alpha;


        private HistoryStageLayouter _layouter;


        private TextFieldBehaviour _textPageBehaviour;
        private int _presentPageNum = 0;
        private int _maxPageNum = 0;

        private List<HistoryItemBehaviour> _historyItemBehaviourList = new List<HistoryItemBehaviour>();

        private List<ButtonBehaviour> _buttonBehaviourList = new List<ButtonBehaviour>();

        private ButtonBehaviour _buttonBack;
        private ButtonBehaviour _buttonNextPage;
        private ButtonBehaviour _buttonPreviousPage;
        private ButtonBehaviour _buttonFirstPage;
        private ButtonBehaviour _buttonLastPage;
        


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(IHistoryManager historyManager)
        {
            _historyManager = historyManager;

            SetupLayout();

            SetupCanvas();

            SetupTextPage();

            SetupButton();

            // Button は非表示にしておく
            HideButton();

            Locator.Instance.audioManager.PlayDefaultStageBGM();
        }


        public void SetupLayout()
        {
            _layouter = new HistoryStageLayouter();

            //_layouter.ShowLayoutArea();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.BASE_LAYERED_ORDERID_HISTORY_STAGE.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupTextPage()
        {
            var gameObjectText = new GameObject(name: "TextPage");
            _textPageBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            var text = "対戦履歴はありません";
            FArea area = _layouter.AreaTextPage;

            var textInitializer = new FTextFieldInitializer(
                text: text,
                fontType: EFontType.BLOCK,
                fontSize: 70,
                alignment: EAlignment.MIDDLE_CENTER,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textPageBehaviour.SetupComponent(textInitializer: textInitializer);
        }


        private void SetupButton()
        {
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
                    sortingOrder: Config.BASE_LAYERED_ORDERID_HISTORY_STAGE.SortingOrder,
                    action: action);

                buttonBehaviour.SetupComponent(initializer: buttonInitializer);


                var buttonTextInitializer = new FButtonTextInitializer(
                    text: buttonText,
                    fontType: EFontType.SERIF,
                    fontSize: (int)(buttonArea.size.y * 0.5f),
                    alignment: EAlignment.MIDDLE_CENTER);
                buttonBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);


                return buttonBehaviour;
            }


            _buttonBack = Support(
                name: "ButtonBack",
                buttonText: "戻る",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonBack,
                action: () => { OnTapButtonBack(); });
            _buttonBehaviourList.Add(_buttonBack);



            _buttonNextPage = Support(
                name: "ButtonNextPage",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonNextPage,
                action: () => { OnTapButtonNextPage(); });
            _buttonBehaviourList.Add(_buttonNextPage);

            _buttonNextPage.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.NEXT),
                area: _layouter.AreaButtonNextPageCompanionImage);

            _buttonPreviousPage = Support(
                name: "ButtonPreviousPage",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonPreviousPage,
                action: () => { OnTapButtonPreviousPage(); });
            _buttonBehaviourList.Add(_buttonPreviousPage);

            _buttonPreviousPage.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.PREVIOUS),
                area: _layouter.AreaButtonPreviousPageCompanionImage);

            _buttonFirstPage = Support(
                name: "ButtonFirstPage",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonFirstPage,
                action: () => { OnTapButtonFirstPage(); });
            _buttonBehaviourList.Add(_buttonFirstPage);

            _buttonFirstPage.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.TO_FIRST),
                area: _layouter.AreaButtonFirstPageCompanionImage);

            _buttonLastPage = Support(
                name: "ButtonLastPage",
                buttonText: "",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonLastPage,
                action: () => { OnTapButtonLastPage(); });
            _buttonBehaviourList.Add(_buttonLastPage);

            _buttonLastPage.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(ETextureTypeForButtonIcon.TO_LAST),
                area: _layouter.AreaButtonLastPageCompanionImage);

            // ページコントロール関係のボタンは当初は無効にしておく
            SetButtonNextPageDisabled();
            SetButtonPreviousPageDisabled();
            SetButtonFirstPageDisabled();
            SetButtonLastPageDisabled();
        }



        // ===============================================
        // MARK: - ● Text Page ─────

        private void UpdateTextPage()
        {
            if (_maxPageNum == 0)
            {
                _textPageBehaviour.SetText(text: "");
            }
            else
            {
                var text = "P." + _presentPageNum.ToString() + " / P." + _maxPageNum.ToString();
                _textPageBehaviour.SetText(text: text);
            }
        }


        // ===============================================
        // MARK: - ● as IHistoryStage ─────

        public void SetPresentPageNum(int value)
        {
            _presentPageNum = value;

            UpdateTextPage();
        }


        public void SetMaxPageNum(int value)
        {
            _maxPageNum = value;

            UpdateTextPage();
        }


        public void SetButtonNextPageEnabled()
        {
            _buttonNextPage.SetIsEnabled(value: true, willChangeButtonTexture: true);
        }

        public void SetButtonNextPageDisabled()
        {
            _buttonNextPage.SetIsEnabled(value: false, willChangeButtonTexture: true);
        }


        public void SetButtonPreviousPageEnabled()
        {
            _buttonPreviousPage.SetIsEnabled(value: true, willChangeButtonTexture: true);
        }

        public void SetButtonPreviousPageDisabled()
        {
            _buttonPreviousPage.SetIsEnabled(value: false, willChangeButtonTexture: true);
        }


        public void SetButtonFirstPageEnabled()
        {
            _buttonFirstPage.SetIsEnabled(value: true, willChangeButtonTexture: true);
        }

        public void SetButtonFirstPageDisabled()
        {
            _buttonFirstPage.SetIsEnabled(value: false, willChangeButtonTexture: true);
        }


        public void SetButtonLastPageEnabled()
        {

            _buttonLastPage.SetIsEnabled(value: true, willChangeButtonTexture: true);
        }

        public void SetButtonLastPageDisabled()
        {
            _buttonLastPage.SetIsEnabled(value: false, willChangeButtonTexture: true);
        }


        public int GetHistoryItemMaxDisplayNum()
        {
            return 5; // ひとまず仮に固定
        }


        public void SetHistoryItemList(in List<FHistoryItemInitializer> historyItemInitializerList)
        {
            Assert.IsTrue(historyItemInitializerList.Count <= GetHistoryItemMaxDisplayNum());

            ClearHistryItemList();


            var index = 0;

            foreach (var i in historyItemInitializerList)
            {
                var gameObject = new GameObject("HistoryItem" + index.ToString());
                var historyItemBehaviour = gameObject.AddComponent<HistoryItemBehaviour>();

                FArea area = _layouter.GetAreaForHistoryItem(index: index);

                historyItemBehaviour.SetupComponent(historyStageBehaviour: this, canvas: _canvas, area: area, initializer: i);
                _historyItemBehaviourList.Add(historyItemBehaviour);

                index += 1;
            }


            if (IsShown)
            {
                _historyItemBehaviourList.ForEach(i => i.Show());
            }
        }

        private void ClearHistryItemList()
        {
            _historyItemBehaviourList.ForEach(i => Utl.SafeDestroy(behaviour: i));
            _historyItemBehaviourList.Clear();
        }



        // ===============================================
        // MARK: - ● History Manager への接続 ─────

        // 表示アニメーションが完了
        private void OnDidShowAnimation()
        {
            _historyManager.OnDidShowAnimation();
        }


        // 戻るボタンを押下
        private void OnTapButtonBack()
        {
            _historyManager.OnTapButtonBack();
        }


        // 棋譜を選択するボタンを押下
        public void OnTapButtonShowItem(int historyDetailID)
        {
            _historyManager.OnTapButtonShowHistoryDetail(historyDetailID: historyDetailID);
        }

        // 次のページへボタンを押下
        private void OnTapButtonNextPage()
        {
            _historyManager.OnTapButtonNextPage();
        }

        // 前のページへボタンを押下
        private void OnTapButtonPreviousPage()
        {
            _historyManager.OnTapButtonPreviousPage();
        }

        // 最初ページへボタンを押下
        private void OnTapButtonFirstPage()
        {
            _historyManager.OnTapButtonFirstPage();
        }

        // 最後のページへボタンを押下
        private void OnTapButtonLastPage()
        {
            _historyManager.OnTapButtonLastPage();
        }


        // ===============================================
        // MARK: - ● Alpha ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void ShowWithAnimation()
        {
            var previousStageType = AppManager.Instance.PreviousStageType;

            FadeIn();

            // TODO: この数字を決めうちにしないこと
            var waitDuration = 0.5f;
            var action = CAction.CreateActionFire(actionBehaviour: this, duration: waitDuration, action: () => { OnDidShowAnimation(); });
            RegisterAction(action: action);
        }


        public override void Show()
        {
            Assert.IsTrue(_isShown == false);

            _textPageBehaviour.Show();
            ShowButton();

            _isShown = true;
        }


        private void ShowButton()
        {
            _buttonBack.Show();

            _buttonNextPage.Show();
            _buttonPreviousPage.Show();
            _buttonFirstPage.Show();
            _buttonLastPage.Show();
        }


        public override void FadeIn(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown == false);

            _textPageBehaviour.FadeIn();
            FadeInHistoryItem();
            ShowButtonWithAnimation();

            _isShown = true;
        }

        private void FadeInHistoryItem()
        {
            _historyItemBehaviourList.ForEach(i => i.FadeIn());
        }

        private void ShowButtonWithAnimation()
        {
            _buttonBack.FadeIn();

            _buttonNextPage.FadeIn();
            _buttonPreviousPage.FadeIn();
            _buttonFirstPage.FadeIn();
            _buttonLastPage.FadeIn();
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


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown);

            _textPageBehaviour.FadeOut();
            FadeOutHistoryItem();
            FadeOutButton();

            _isShown = false;
        }


        public override void Hide()
        {
            Assert.IsTrue(_isShown);

            _textPageBehaviour.Hide();
            HideButton();
            HideHistoryItem();

            _isShown = false;
        }




        private void HideButton()
        {
            _buttonBehaviourList.ForEach(i => i.Hide());
        }


        private void FadeOutButton()
        {
            _buttonBehaviourList.ForEach(i => i.FadeOut());
        }

        private void HideHistoryItem()
        {
            _historyItemBehaviourList.ForEach(i => i.Hide());
        }

        private void FadeOutHistoryItem()
        {
            _historyItemBehaviourList.ForEach(i => i.FadeOut());
        }


        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unity Editor での表示の場合停止タイミングで _layouter が null となるためチェックして破壊
            _layouter?.Destroy();
            _layouter = null;

            _textPageBehaviour = null;

            Utl.SafeDestroy(behaviourList: _buttonBehaviourList);
            _buttonBehaviourList.Clear();

            _historyManager = null;
            _canvas = null;
        }




    }
}
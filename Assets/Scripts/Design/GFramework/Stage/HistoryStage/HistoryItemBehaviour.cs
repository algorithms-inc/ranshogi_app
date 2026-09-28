using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class HistoryItemBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private HistoryStageBehaviour _historyStageBehaviour;

        private Canvas _canvas;

        private FHistoryItemInitializer _setting;



        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => _textGameNumBehaviour.Alpha;

        private HistoryItemLayouter _layouter;

        private List<TextFieldBehaviour> _textFieldBehaviourList = new List<TextFieldBehaviour>();

        private TextFieldBehaviour _textGameNumBehaviour;
        private TextFieldBehaviour _textDateBehaviour;
        private TextFieldBehaviour _textDateAdditionalInfoBehaviour;
        private TextFieldBehaviour _textOpponentNameBehaviour;
        private TextFieldBehaviour _textRateInfoBehaviour;

        private SingleImageActionBehaviour _singleImageActionBehaviourResultIcon;
        private SingleImageActionBehaviour _singleImageActionBehaviourBoardTypeIcon;
        private SingleImageActionBehaviour _singleImageActionBehaviourGradeRankIcon;

        private ButtonBehaviour _buttonShowItemBehaviour;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(HistoryStageBehaviour historyStageBehaviour, Canvas canvas, FArea area, FHistoryItemInitializer initializer)
        {
            _historyStageBehaviour = historyStageBehaviour;
            _canvas = canvas;
            _setting = initializer;

            SetupLayout(area: area);

            SetupText();

            SetupResultIcon();

            SetupBoardTypeIcon();

            SetupGradeRankIcon();

            SetupButton();

            // 初期状態は非表示
            Hide();
        }



        // ===============================================
        // MARK: - ● Layout ─────

        private void SetupLayout(FArea area)
        {
            _layouter = new HistoryItemLayouter(area: area);
        }


        private void SetupText()
        {
            SetupTextGameNum();
            SetupTextDate();
            SetupTextDateAdditionalInfo();
            SetupTextOpponentName();
            SetupTextRateInfo();
        }


        private void SetupTextGameNum()
        {
            var gameObjectText = new GameObject(name: "TextGameNum");
            _textGameNumBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            FArea area = _layouter.GetAreaTextGameNum().AsYReversed();

            var textInitializer = new FTextFieldInitializer(
                text: _setting.GameNum.ToString(),
                fontType: EFontType.BLOCK,
                fontSize: 45,
                alignment: EAlignment.MIDDLE_RIGHT,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textGameNumBehaviour.SetupComponent(textInitializer: textInitializer);

            _textFieldBehaviourList.Add(_textGameNumBehaviour);

            _textGameNumBehaviour.Hide();
        }


        private void SetupTextDate()
        {
            var gameObjectText = new GameObject(name: "TextDate");
            _textDateBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            FArea area = _layouter.GetAreaTextDate().AsYReversed();

            var textInitializer = new FTextFieldInitializer(
                text: _setting.Date,
                fontType: EFontType.BLOCK,
                fontSize: 45,
                alignment: EAlignment.MIDDLE_LEFT,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textDateBehaviour.SetupComponent(textInitializer: textInitializer);

            _textFieldBehaviourList.Add(_textDateBehaviour);

            _textDateBehaviour.Hide();
        }


        private void SetupTextDateAdditionalInfo()
        {
            var gameObjectText = new GameObject(name: "TextDateAdditionalInfo");
            _textDateAdditionalInfoBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            FArea area = _layouter.GetAreaTextDateAdditionalInfo().AsYReversed();

            var textInitializer = new FTextFieldInitializer(
                text: _setting.DateAdditionalInfo,
                fontType: EFontType.BLOCK,
                fontSize: 45,
                alignment: EAlignment.MIDDLE_RIGHT,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textDateAdditionalInfoBehaviour.SetupComponent(textInitializer: textInitializer);

            _textFieldBehaviourList.Add(_textDateAdditionalInfoBehaviour);

            _textDateAdditionalInfoBehaviour.Hide();
        }


        private void SetupTextRateInfo()
        {
            var gameObjectText = new GameObject(name: "TextRateInfo");
            _textRateInfoBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            FArea area = _layouter.GetAreaTextRate().AsYReversed();

            var textInitializer = new FTextFieldInitializer(
                text: _setting.NextRate.ToString(),
                fontType: EFontType.SERIF,
                fontSize: 60,
                alignment: EAlignment.MIDDLE_LEFT,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textRateInfoBehaviour.SetupComponent(textInitializer: textInitializer);

            _textFieldBehaviourList.Add(_textRateInfoBehaviour);

            _textRateInfoBehaviour.Hide();
        }


        private void SetupTextOpponentName()
        {
            var gameObjectText = new GameObject(name: "TextOpponentName");
            _textOpponentNameBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>();

            FArea area = _layouter.GetAreaTextOpponentName().AsYReversed();

            var textInitializer = new FTextFieldInitializer(
                text: _setting.OpponentName,
                fontType: EFontType.SERIF,
                fontSize: 80,
                alignment: EAlignment.MIDDLE_LEFT,
                area: area,
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas); ;

            _textOpponentNameBehaviour.SetupComponent(textInitializer: textInitializer);

            _textFieldBehaviourList.Add(_textOpponentNameBehaviour);

            _textOpponentNameBehaviour.Hide();
        }


        private void SetupResultIcon()
        {
            _singleImageActionBehaviourResultIcon = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "ResultIcon",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(historyItemResult: _setting.HistoryItemResult),
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f)
                );

            var areaByPixel = _layouter.GetAreaResultIcon().AsYReversed() ;
            _singleImageActionBehaviourResultIcon.transform.position = Utl.ConvertImageSizeToUnit(imageSize: areaByPixel.CenterPoint);

            float bodyImageSizeY = Config.HISTORY_DETAIL_STAGE_RESULT_ICON__IMAGE_SIZE.y;
            float bodyDisplaySizeY = areaByPixel.size.y;

            var localScale = new Vector2(bodyDisplaySizeY / bodyImageSizeY, bodyDisplaySizeY / bodyImageSizeY);

            _singleImageActionBehaviourResultIcon.transform.localScale = localScale;
        }


        private void SetupBoardTypeIcon()
        {
            _singleImageActionBehaviourBoardTypeIcon = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "BoardTypeIcon",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(boardType: _setting.BoardType),
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f)
                );

            var areaByPixel = _layouter.GetAreaBoardTypeIcon().AsYReversed() ;
            _singleImageActionBehaviourBoardTypeIcon.transform.position = Utl.ConvertImageSizeToUnit(imageSize: areaByPixel.CenterPoint);

            float bodyImageSizeY = Config.HISTORY_DETAIL_STAGE_BOARD_TYPE_ICON__IMAGE_SIZE.y;
            float bodyDisplaySizeY = areaByPixel.size.y;

            var localScale = new Vector2(bodyDisplaySizeY / bodyImageSizeY, bodyDisplaySizeY / bodyImageSizeY);

            _singleImageActionBehaviourBoardTypeIcon.transform.localScale = localScale;
        }


        private void SetupGradeRankIcon()
        {
            _singleImageActionBehaviourGradeRankIcon = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "RankGradeIcon",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePathOfGradeRankIcon(gradeRankType: _setting.NextGradeRankType, boardType: _setting.BoardType, isDefault: true),
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f),
                area: _layouter.GetAreaGradeRankIcon().AsYReversed(),
                scaleMode: EScaleMode.FIT
                );
        }


        private void SetupButton()
        {
            ButtonBehaviour Support(string name, EButtonImageType buttonImageType, FArea buttonArea, bool isEnabled, Action actionOnFire)
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
                    isEnabled: isEnabled,
                    sortingOrder: Config.MENU_STAGE_BUTTON__LAYERED_ORDERID.SortingOrder,
                    action: actionOnFire);

                buttonBehaviour.SetupComponent(initializer: buttonInitializer);


                return buttonBehaviour;
            }

            FArea area = _layouter.GetAreaTapArea().AsYReversed();

            _buttonShowItemBehaviour = Support(
                name: "ButtonShowItem",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: area,
                isEnabled: _setting.IsEnabled,
                actionOnFire: () => { OnTapButtonShowItem(); });
        }



        // ===============================================
        // MARK: - ● Alpha ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void FadeIn(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown == false);


            _textFieldBehaviourList.ForEach(i => i.FadeIn());
            _singleImageActionBehaviourResultIcon.FadeIn();
            _singleImageActionBehaviourBoardTypeIcon.FadeIn();
            _buttonShowItemBehaviour.FadeIn();
            _singleImageActionBehaviourGradeRankIcon.FadeIn();


            _isShown = true;
        }


        public override void Show()
        {
            Assert.IsTrue(_isShown == false);


            _textFieldBehaviourList.ForEach(i => i.Show());
            _singleImageActionBehaviourResultIcon.Show();
            _singleImageActionBehaviourBoardTypeIcon.Show();
            _buttonShowItemBehaviour.Show();
            _singleImageActionBehaviourGradeRankIcon.Show();


            _isShown = true;
        }


        public override void Hide()
        {
            Assert.IsTrue(_isShown);

            _textOpponentNameBehaviour.Hide();
            _singleImageActionBehaviourResultIcon.Hide();
            _singleImageActionBehaviourBoardTypeIcon.Hide();
            _buttonShowItemBehaviour.Hide();
            _singleImageActionBehaviourGradeRankIcon.Hide();

            _isShown = false;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown);

            _textFieldBehaviourList.ForEach(i => i.FadeOut());
            _singleImageActionBehaviourResultIcon.FadeOut();
            _singleImageActionBehaviourBoardTypeIcon.FadeOut();
            _buttonShowItemBehaviour.FadeOut();
            _singleImageActionBehaviourGradeRankIcon.FadeOut();

            _isShown = false;
        }


        // ===============================================
        // MARK: - ● OnTapButtonShowItem  ─────

        private void OnTapButtonShowItem()
        {
            _historyStageBehaviour.OnTapButtonShowItem(historyDetailID: _setting.HistoryDetailID);
        }


        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unity Editor での表示の場合停止タイミングで _layouter が null となるためチェックして破壊
            _layouter?.Destroy();
            _layouter = null;

            DestroyRelatedGameObject();

            _textGameNumBehaviour = null;
            _textDateBehaviour = null;
            _textDateAdditionalInfoBehaviour = null;
            _textOpponentNameBehaviour = null;
            _textRateInfoBehaviour = null;


            _singleImageActionBehaviourResultIcon = null;
            _singleImageActionBehaviourBoardTypeIcon = null;

            _historyStageBehaviour = null;
            _canvas = null;
        }

        private void DestroyRelatedGameObject()
        {
            Utl.SafeDestroy(behaviour: _textGameNumBehaviour);
            Utl.SafeDestroy(behaviour: _textDateBehaviour);
            Utl.SafeDestroy(behaviour: _textDateAdditionalInfoBehaviour);
            Utl.SafeDestroy(behaviour: _textOpponentNameBehaviour);
            Utl.SafeDestroy(behaviour: _textRateInfoBehaviour);
            Utl.SafeDestroy(behaviour: _singleImageActionBehaviourResultIcon);
            Utl.SafeDestroy(behaviour: _singleImageActionBehaviourBoardTypeIcon);
            
        }
    }
}
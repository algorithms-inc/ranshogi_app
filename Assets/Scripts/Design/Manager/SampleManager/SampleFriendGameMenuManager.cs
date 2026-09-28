using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// こちらの using を挿入する必要あり
using GFramework.Core;


// ===============================================
// MARK: - ● SampleFriendGameMenuManager ─────

// FriendGameMenuManager では、インターフェースとして IBlankPageManager を設定する必要あり
public class SampleFriendGameMenuManager : MonoBehaviour, IBlankPageManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // BlankPageStage への参照
    private IBlankPageStage _blankPageStage = null;


    private EBoardType _boardType = EBoardType.BOARD_66;


    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        Debug.Log(this.GetType().ToString() + ": Awake()");

        SetupAppManager();

        SetupStage();

        SetupStatusBar();

        ShowWindow();
    }


    // AppManager のセットアップ
    // - 開発時にはさまざまなシーンが開始シーンとなります。そのため、どのシーンでも AppManager をセットアップする必要があります。
    // - AppManager のセットアップは何度呼んでも問題ありません（最初に呼ばれたときのみ実行されます）。
    private void SetupAppManager()
    {
        AppManager.Instance.Setup();
    }


    private void SetupStage()
    {
        _blankPageStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);

        _blankPageStage.ShowWithAnimation();
    }


    private void SetupStatusBar()
    {
        AppManager.Instance.SetLife(count: 3);
        AppManager.Instance.SetPlayerName(playerName: "---");
        AppManager.Instance.SetRate(rate: 1500);

        AppManager.Instance.ShowStatusBar();
    }


    // ===============================================
    // MARK: - ● Dummy Method ─────

    private void CreateRoomCodeButtonTapped()
    {
        DisplayRoomID(roomID: "f12345");
    }

    private void JoinRoomButtonTapped()
    {}

    // ===============================================
    // MARK: - ● Show Window Normal ─────

    private void ShowWindow()
    {
       AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "友達と対局する",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFSubHeadingCreateRoom",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.15f),
                endPoint: new Vector2(1f, 1.1f)),
            text: "< 対局部屋を作成する >",
            fontScale: 0.9f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFExplanationForRuleID",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.275f),
                endPoint: new Vector2(1f, 1.0f)),
            text: "ルール",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFRuleId",
            area: new FWindowElementArea(
                origin: new Vector2(0.06f, 0.38f - 0.025f),
                endPoint: new Vector2(1f, 0.95f)),
            text: "",
            fontScale: 1.1f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        UpdateTextFieldOfRule();

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonChangeRule",
               area: new FWindowElementArea(
                   origin: new Vector2(0.66f, 0.38f - 0.0125f - 0.025f),
                   width: 0.34f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight() - 0.025f),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "変更",
               isEnabled: true,
               actionOnTap: () => {
                   ShowRuleSelector();
               });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonMakeRoom",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.55f),
                width: 1f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "部屋IDを新規発行",
            isEnabled: true,
            actionOnTap: () => {
                HideRoomID();
                CreateRoomCodeButtonTapped();
            });

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFExplanationForNewID",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.77f),
                endPoint: new Vector2(1f, 1.0f)),
            text: "部屋ID",
            fontScale: 0.8f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFRoomId",
            area: new FWindowElementArea(
                origin: new Vector2(0.06f, 0.85f),
                endPoint: new Vector2(1f, 0.95f)),
            text: "–––––",
            fontScale: 1.25f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.DISABLED,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonCopy",
               area: new FWindowElementArea(
                   origin: new Vector2(0.66f, 0.8375f),
                   width: 0.34f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight() - 0.025f),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "コピｰ",
               isEnabled: false,
               actionOnTap: () => {
                   // GUIUtility.systemCopyBuffer = currentText;
               });

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.025f),
                endPoint: new Vector2(1f, 1.03f)),
            filePath: "UI/ImageBox/imagebox_line_a.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFSubHeadingEnterRoom",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.09f - 0.015f),
                endPoint: new Vector2(1f, 1.6f)),
            text: "< 対局部屋に入る >",
            fontScale: 0.9f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFRoomId",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.24f - 0.015f),
                width: 0.65f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            actionOnTap: () =>
            {
                // keyboard = TouchScreenKeyboard.Open(currentText, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "部屋ID",
            text: "",
            isPasswordMode: false,
            hasError: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonEnterRoom",
               area: new FWindowElementArea(
                   origin: new Vector2(0.66f, 1.24f - 0.02f),
                   width: 0.34f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.POSITIVE,
               text: "入室",
               isEnabled: false,
               actionOnTap: () => {
                   JoinRoomButtonTapped();
               });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonBack",
               area: new FWindowElementArea(
                   origin: new Vector2(0f, 1.65f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                   width: 1f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "戻る",
               isEnabled: true,
               actionOnTap: () => {
                    _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleMenuStageScene", nextStageType: EStageType.MENU);
               });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});
    }

    private void HideRoomID()
    {
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonCopy", value: false);
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "TFRoomId", text: "–––––");
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeTextColor(name: "TFRoomId", textColorType: EWindowTextColorType.DISABLED);
        AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText(name: "IFRoomId", text: "");
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonEnterRoom", value: false);
    }

    private void DisplayRoomID(string roomID)
    {
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonCopy", value: true);
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "TFRoomId", text: roomID);
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeTextColor(name: "TFRoomId", textColorType: EWindowTextColorType.NORMAL);
        AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText(name: "IFRoomId", text: roomID);
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonEnterRoom", value: true);
    }

    private void UpdateTextFieldOfRule()
    {
        string text;

        if (_boardType == EBoardType.BOARD_66)
        {
            text = "乱将棋";
        }
        else
        {
            text = "将棋";
        }
        
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "TFRuleId", text: text);
    }



    // ===============================================
    // MARK: - ● Rule Selector ─────

    private void ShowRuleSelector()
    {
        int indexOfOn;

        if (_boardType == EBoardType.BOARD_66)
        {
            indexOfOn = 0;
        }
        else
        {
            indexOfOn = 1;
        }


        AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
            name: "TFTitleForRule",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(0.9f, 0.15f)),
            text: "ルール",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);


        AppManager.Instance.UICreator.WindowConfirmation.RadioButtonGroup.Add(
            name: "radioButtonForRule",
            areaList: new List<FWindowElementArea> {
                    new FWindowElementArea(origin: new Vector2(0.05f, 0.2f),  endPoint: new Vector2(0.175f, 0.325f)),
                    new FWindowElementArea(origin: new Vector2(0.05f, 0.4f),  endPoint: new Vector2(0.175f, 0.525f)),
                    },
            companionTextFieldSettingList: new List<FCompanionTextFieldSetting> {
                    new FCompanionTextFieldSetting(
                        windowElementArea: new FWindowElementArea(origin: new Vector2(0.21f, 0.2f), endPoint: new Vector2(1.0f, 0.32f)),
                        text: "乱将棋 (6×6)",
                        fontType: EFontType.SERIF,
                        fontScale: 1.0f,
                        alignment: EAlignment.MIDDLE_LEFT),
                    new FCompanionTextFieldSetting(
                        windowElementArea: new FWindowElementArea(origin: new Vector2(0.21f, 0.4f), endPoint: new Vector2(1.0f, 0.52f)),
                        text: "将棋 (9×9)",
                        fontType: EFontType.SERIF,
                        fontScale: 1.0f,
                        alignment: EAlignment.MIDDLE_LEFT),
            },
            indexOfOn: indexOfOn,
            actionOnTap: () => { Debug.Log($"RadioButton{AppManager.Instance.UICreator.WindowConfirmation.RadioButtonGroup.GetIndexOfOn(name: "radioButtonForRule")} が押されました"); },
            isEnabled: true);


            AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
               name: "buttonConcludeOnRule",
               area: new FWindowElementArea(
                   origin: new Vector2(0.2f, 0.625f),
                   width: 0.5f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "決定",
               isEnabled: true,
               actionOnTap: () => {
                    var indexOfOn = AppManager.Instance.UICreator.WindowConfirmation.RadioButtonGroup.GetIndexOfOn(name: "radioButtonForRule");

                    if (indexOfOn == 0)
                    {
                        _boardType = EBoardType.BOARD_66;
                    }
                    else
                    {
                        _boardType = EBoardType.BOARD_99;
                    }

                    AppManager.Instance.UICreator.WindowConfirmation.Hide();
                    UpdateTextFieldOfRule();
               });

        AppManager.Instance.UICreator.WindowConfirmation.Show(onDidShowAction: null);
    }

    // ===============================================
    // MARK: - ● On Event ─────

    public void OnDidShowAnimation()
    {
        Debug.Log(this.GetType().ToString() + ": OnDidShowAnimation");
    }



    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        Debug.Log(this.GetType().ToString() + ": OnDestroy");

        _blankPageStage = null;
    }
}
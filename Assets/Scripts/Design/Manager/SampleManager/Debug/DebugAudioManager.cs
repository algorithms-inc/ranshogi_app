using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using UnityEngine.Assertions;


// こちらの using を挿入する必要あり
using GFramework.Core;



// ===============================================
// MARK: - ● DebugAudioManager ─────

public class DebugAudioManager : MonoBehaviour, IGeneralButtonListManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // GeneralButtonListStage への参照
    private IGeneralButtonListStage _generalButtonListStage = null;



    // for BGM Setting
    private FCycleCounter _presentIndexOfBGMType = new FCycleCounter(count: 0, min: 0, max: 0);
    private FCycleCounter _presentIndexOfBGMSourceType = new FCycleCounter(count: 0, min: 0, max: 0);
    private int _previousIndexOfBGMSourceType = 0;

    // for JIN Setting
    private FCycleCounter _presentIndexOfJINType = new FCycleCounter(count: 0, min: 0, max: 0);
    private FCycleCounter _presentIndexOfJINSourceType = new FCycleCounter(count: 0, min: 0, max: 0);
    private int _previousIndexOfJINSourceType = 0;

    // for SE Setting
    private FCycleCounter _presentIndexOfSEType = new FCycleCounter(count: 0, min: 0, max: 0);
    private FCycleCounter _presentIndexOfSESourceType = new FCycleCounter(count: 0, min: 0, max: 0);
    private int _previousIndexOfSESourceType = 0;


    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        AppManager.Instance.Setup();

        SetupStage();

        SetupStatusBar();

        SetupMisc();
    }


    private void SetupStage()
    {
        var buttonTextList = new List<string> {
            "音量設定",
            "BGM視聴",
            "JIN視聴",
            "SE視聴",
            "",
            "",
            "",
            "",
            "",
            "Debug Menuに戻る" };


        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "Debug Audio",
            buttonTextList: buttonTextList);

        _generalButtonListStage.ShowWithAnimation();
    }


    private void SetupStatusBar()
    {
        AppManager.Instance.SetLife(count: 3);
        AppManager.Instance.SetPlayerName(playerName: "Taro");
        AppManager.Instance.SetRate(rate: 1234);

        AppManager.Instance.ShowStatusBar();
    }


    private void SetupMisc()
    {
        // for BGM
        _presentIndexOfBGMType = new FCycleCounter(count: 0, min: 0, max: Enum.GetNames(typeof(EBGMType)).Length - 1);
        _presentIndexOfBGMSourceType = new FCycleCounter(count: 0, min: 0, max: Enum.GetNames(typeof(EBGMSourceType)).Length - 1);

        // for JIN
        _presentIndexOfJINType = new FCycleCounter(count: 0, min: 0, max: Enum.GetNames(typeof(EJINType)).Length - 1);
        _presentIndexOfJINSourceType = new FCycleCounter(count: 0, min: 0, max: Enum.GetNames(typeof(EJINSourceType)).Length - 1);

        // for SE
        _presentIndexOfSEType = new FCycleCounter(count: 0, min: 0, max: Enum.GetNames(typeof(ESEType)).Length - 1);
        _presentIndexOfSESourceType = new FCycleCounter(count: 0, min: 0, max: Enum.GetNames(typeof(ESESourceType)).Length - 1);
    }


    // ===============================================
    // MARK: - ● On Event ─────

    // ===============================================
    // MARK: - ● On Event ─────

    public void OnDidShowAnimation()
    {
        Debug.Log(GetType().ToString() + ": OnDidShowAnimation");
    }


    public void OnTapButton0()
    {
        // オーディオに関する現在の設定を取得する

        // 音量(マスター)：値は 0.0f-1.0f
        float volumeMaster = AppManager.Instance.AudioController.GetVolumeMaster();

        bool isMuteMaster = AppManager.Instance.AudioController.IsMuteMaster();
        bool isMuteBGM = AppManager.Instance.AudioController.IsMuteBGM();
        bool isMuteJIN = AppManager.Instance.AudioController.IsMuteJIN();
        bool isMuteSE = AppManager.Instance.AudioController.IsMuteSE();



        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "音量設定",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxMaster",
           area: new FWindowElementArea(
               origin: new Vector2(0f, 0.2f),
               endPoint: new Vector2(0.15f, 0.35f)),
           actionOnTap: () => {
               var isCheckBoxOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxMaster");
               AppManager.Instance.AudioController.SetIsMuteMaster(value: !isCheckBoxOn);
               AppManager.Instance.UICreator.WindowNormal.Slider.SetIsEnabled(name: "sliderVolume", value: isCheckBoxOn);
               AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsEnabled(name: "checkBoxBGM", value: isCheckBoxOn);
               AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsEnabled(name: "checkBoxJIN", value: isCheckBoxOn);
               AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsEnabled(name: "checkBoxSE", value: isCheckBoxOn);
           },
           isOn: !isMuteMaster,
           hasError: false,
           isEnabled: true,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f, 0.2f),
                   endPoint: new Vector2(1.0f, 0.35f)),
               text: "音を鳴らす",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.Slider.Add(
           name: "sliderVolume",
           area: new FWindowElementArea(
               origin: new Vector2(0.0f, 0.4f),
               endPoint: new Vector2(1.0f, 0.55f)),
           maxStep: 10,
           presentStep: (int)(volumeMaster * 10), // 音量の設定値は float (0.0f-1.0f)。スライダーの値は int(0-maxStep)での設定であるため変換が必要
           actionOnChangeStep: () => {
               var newValue = (float)AppManager.Instance.UICreator.WindowNormal.Slider.GetPresentStep(name: "sliderVolume") / 10.0f;
               AppManager.Instance.AudioController.SetVolumeMaster(value: newValue);
           },
           isPlaySE: true,
           isEnabled: !isMuteMaster);

        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxBGM",
           area: new FWindowElementArea(
               origin: new Vector2(0.1f, 0.65f),
               endPoint: new Vector2(0.25f, 0.80f)),
           actionOnTap: () => {
               var isCheckBoxOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxBGM");
               AppManager.Instance.AudioController.SetIsMuteBGM(value: !isCheckBoxOn);
           },
           isOn: !isMuteBGM,
           hasError: false,
           isEnabled: !isMuteMaster,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.3f, 0.65f),
                   endPoint: new Vector2(1.0f, 0.80f)),
               text: "BGM",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxJIN",
           area: new FWindowElementArea(
               origin: new Vector2(0.1f, 0.85f),
               endPoint: new Vector2(0.25f, 1.0f)),
           actionOnTap: () => {
               var isCheckBoxOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxJIN");
               AppManager.Instance.AudioController.SetIsMuteJIN(value: !isCheckBoxOn);
           },
           isOn: !isMuteJIN,
           hasError: false,
           isEnabled: !isMuteMaster,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.3f, 0.85f),
                   endPoint: new Vector2(1.0f, 1.0f)),
               text: "勝敗結果音",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxSE",
           area: new FWindowElementArea(
               origin: new Vector2(0.1f, 1.05f),
               endPoint: new Vector2(0.25f, 1.2f)),
           actionOnTap: () => {
               var isCheckBoxOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxSE");
               AppManager.Instance.AudioController.SetIsMuteSE(value: !isCheckBoxOn);
           },
           isOn: !isMuteSE,
           hasError: false,
           isEnabled: !isMuteMaster,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.3f, 1.05f),
                   endPoint: new Vector2(1.0f, 1.20f)),
               text: "効果音",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.625f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // BGM Setting
    public void OnTapButton1()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
             name: "textFieldA",
             area: new FWindowElementArea(
                 origin: new Vector2(0f, 0.0f),
                 endPoint: new Vector2(1f, 0.15f)),
             text: "BGM設定",
             fontScale: 1.0f,
             fontType: EFontType.SERIF,
             textColorType: EWindowTextColorType.NORMAL,
             alignment: EAlignment.TOP_CENTER,
             actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSubtitleType",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3f),
                endPoint: new Vector2(1f, 0.45f)),
            text: "BGMType:",
            fontScale: 1.0f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldType",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 0.46f),
                endPoint: new Vector2(1f, 0.61f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textEqual",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.61f),
                endPoint: new Vector2(1f, 0.76f)),
            text: "▼",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSubtitleSourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.75f),
                endPoint: new Vector2(1f, 0.9f)),
            text: "BGMSourceType:",
            fontScale: 1.0f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 0.91f),
                endPoint: new Vector2(1f, 1.06f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldFileName",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 1.05f),
                endPoint: new Vector2(1f, 1.2f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTypePrevious",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.30f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＜",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementBGMType(isPlus: false);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTypeNext",
            area: new FWindowElementArea(
                origin: new Vector2(0.86f, 0.30f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＞",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementBGMType(isPlus: true);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTestPlayByType",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.46f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "♪",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                TestPlayBGMType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonSourceTypePrevious",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.75f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＜",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementBGMSourceType(isPlus: false);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonSourceTypeNext",
            area: new FWindowElementArea(
                origin: new Vector2(0.86f, 0.75f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＞",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementBGMSourceType(isPlus: true);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTestPlayBySourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.91f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "♪",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                TestPlayBGMSourceType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonChangeBinding",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.25f),
                endPoint: new Vector2(1f, 1.40f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "Change Binding",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                ChangeBindingOfBGMTypeAndBGMSourceType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });


        SetupBGMSetting();

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // JIN Setting
    public void OnTapButton2()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "JIN設定",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSubtitleType",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3f),
                endPoint: new Vector2(1f, 0.45f)),
            text: "JINType:",
            fontScale: 1.0f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldType",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 0.46f),
                endPoint: new Vector2(1f, 0.61f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textEqual",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.61f),
                endPoint: new Vector2(1f, 0.76f)),
            text: "▼",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSubtitleSourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.75f),
                endPoint: new Vector2(1f, 0.9f)),
            text: "JINSourceType:",
            fontScale: 1.0f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 0.91f),
                endPoint: new Vector2(1f, 1.06f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldFileName",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 1.05f),
                endPoint: new Vector2(1f, 1.2f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTypePrevious",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.30f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＜",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementJINType(isPlus: false);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTypeNext",
            area: new FWindowElementArea(
                origin: new Vector2(0.86f, 0.30f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＞",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementJINType(isPlus: true);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTestPlayByType",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.46f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "♪",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                TestPlayJINType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonSourceTypePrevious",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.75f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＜",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementJINSourceType(isPlus: false);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonSourceTypeNext",
            area: new FWindowElementArea(
                origin: new Vector2(0.86f, 0.75f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＞",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementJINSourceType(isPlus: true);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTestPlayBySourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.91f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "♪",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                TestPlayJINSourceType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonChangeBinding",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.25f),
                endPoint: new Vector2(1f, 1.40f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "Change Binding",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                ChangeBindingOfJINTypeAndJINSourceType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });


        SetupJINSetting();

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // SE Setting
    public void OnTapButton3()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "SE設定",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSubtitleType",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3f),
                endPoint: new Vector2(1f, 0.45f)),
            text: "SEType:",
            fontScale: 1.0f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldType",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 0.46f),
                endPoint: new Vector2(1f, 0.61f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textEqual",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.61f),
                endPoint: new Vector2(1f, 0.76f)),
            text: "▼",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSubtitleSourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.75f),
                endPoint: new Vector2(1f, 0.9f)),
            text: "SESourceType:",
            fontScale: 1.0f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldSourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 0.91f),
                endPoint: new Vector2(1f, 1.06f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldFileName",
            area: new FWindowElementArea(
                origin: new Vector2(0.16f, 1.05f),
                endPoint: new Vector2(1f, 1.2f)),
            text: "",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTypePrevious",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.30f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＜",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementSEType(isPlus: false);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTypeNext",
            area: new FWindowElementArea(
                origin: new Vector2(0.86f, 0.30f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＞",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementSEType(isPlus: true);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTestPlayByType",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.46f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "♪",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                TestPlaySEType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonSourceTypePrevious",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.75f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＜",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementSESourceType(isPlus: false);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonSourceTypeNext",
            area: new FWindowElementArea(
                origin: new Vector2(0.86f, 0.75f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "＞",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                IncrementSESourceType(isPlus: true);
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonTestPlayBySourceType",
            area: new FWindowElementArea(
                origin: new Vector2(0.0f, 0.91f),
                width: 0.13f, height: 0.13f),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "♪",
            isPlaySE: false,
            isEnabled: true,
            actionOnTap: () => {
                TestPlaySESourceType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonChangeBinding",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.25f),
                endPoint: new Vector2(1f, 1.40f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "Change Binding",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                ChangeBindingOfSETypeAndSESourceType();
            });


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });


        SetupSESetting();

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    public void OnTapButton4() { /* Do Nothing */ }
    public void OnTapButton5() { /* Do Nothing */ }
    public void OnTapButton6() { /* Do Nothing */ }
    public void OnTapButton7() { /* Do Nothing */ }
    public void OnTapButton8() { /* Do Nothing */ }


    public void OnTapButton9()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugMenuStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }


    // ===============================================
    // MARK: - ● BGM Setting ─────

    private void SetupBGMSetting()
    {
        UpdateInfoOfBGMTypeAndBGMSourceType(BGMType: (EBGMType)_presentIndexOfBGMType.Value);
    }


    private void UpdateInfoOfBGMTypeAndBGMSourceType(EBGMType BGMType)
    {
        _presentIndexOfBGMType.Value = (int)BGMType;

        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSubtitleType", text: "BGMType: " + (_presentIndexOfBGMType.Value).ToString());
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldType", text: ((EBGMType)_presentIndexOfBGMType.Value).ToString());

        var BGMSourceType = AppManager.Instance.AudioController.GetSourceType(BGMType: (EBGMType)_presentIndexOfBGMType.Value);
        UpdateInfoOfOnlyBGMSourceType(BGMSourceType: BGMSourceType);

        _previousIndexOfBGMSourceType = (int)BGMSourceType;

        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: false);
    }


    private void UpdateInfoOfOnlyBGMSourceType(EBGMSourceType BGMSourceType)
    {
        _presentIndexOfBGMSourceType.Value = (int)BGMSourceType;

        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSubtitleSourceType", text: "BGMSourceType: " + _presentIndexOfBGMSourceType.Value.ToString());
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSourceType", text: ((EBGMSourceType)_presentIndexOfBGMSourceType.Value).ToString());

        var BGMFilePath = AppManager.Instance.AudioController.GetFilePath(BGMSourceType: (EBGMSourceType)_presentIndexOfBGMSourceType.Value);
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldFileName", text: "file: " + BGMFilePath);
    }


    private void IncrementBGMType(bool isPlus)
    {
        if (isPlus)
        {
            _presentIndexOfBGMType.Increment();
        }
        else
        {
            _presentIndexOfBGMType.Decrement();
        }

        UpdateInfoOfBGMTypeAndBGMSourceType(BGMType: (EBGMType)_presentIndexOfBGMType.Value);

        bool isSetButtonEnabled = (_presentIndexOfBGMSourceType.Value != _previousIndexOfBGMSourceType);
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: isSetButtonEnabled);
    }


    private void IncrementBGMSourceType(bool isPlus)
    {
        if (isPlus)
        {
            _presentIndexOfBGMSourceType.Increment();
        }
        else
        {
            _presentIndexOfBGMSourceType.Decrement();
        }

        UpdateInfoOfOnlyBGMSourceType(BGMSourceType: (EBGMSourceType)_presentIndexOfBGMSourceType.Value);

        bool isSetButtonEnabled = (_presentIndexOfBGMSourceType.Value != _previousIndexOfBGMSourceType);
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: isSetButtonEnabled);
    }


    private void TestPlayBGMType()
    {
        AppManager.Instance.AudioController.TestPlay(BGMType: (EBGMType)_presentIndexOfBGMType.Value);
    }


    private void TestPlayBGMSourceType()
    {
        AppManager.Instance.AudioController.TestPlay(BGMSourceType: (EBGMSourceType)_presentIndexOfBGMSourceType.Value);
    }


    private void ChangeBindingOfBGMTypeAndBGMSourceType()
    {
        AppManager.Instance.AudioController.SetSoruceType(BGMType: (EBGMType)_presentIndexOfBGMType.Value, BGMSourceType: (EBGMSourceType)_presentIndexOfBGMSourceType.Value);

        _previousIndexOfBGMSourceType = _presentIndexOfBGMSourceType.Value;
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: false);
    }



    // ===============================================
    // MARK: - ● JIN Setting ─────

    private void SetupJINSetting()
    {
        UpdateInfoOfJINTypeAndJINSourceType(JINType: (EJINType)_presentIndexOfJINType.Value);
    }


    private void UpdateInfoOfJINTypeAndJINSourceType(EJINType JINType)
    {
        _presentIndexOfJINType.Value = (int)JINType;

        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSubtitleType", text: "JINType: " + (_presentIndexOfJINType.Value).ToString());
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldType", text: ((EJINType)_presentIndexOfJINType.Value).ToString());

        var JINSourceType = AppManager.Instance.AudioController.GetSourceType(JINType: (EJINType)_presentIndexOfJINType.Value);
        UpdateInfoOfOnlyJINSourceType(JINSourceType: JINSourceType);

        _previousIndexOfJINSourceType = (int)JINSourceType;

        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: false);
    }


    private void UpdateInfoOfOnlyJINSourceType(EJINSourceType JINSourceType)
    {
        _presentIndexOfJINSourceType.Value = (int)JINSourceType;

        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSubtitleSourceType", text: "JINSourceType: " + _presentIndexOfJINSourceType.Value.ToString());
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSourceType", text: ((EJINSourceType)_presentIndexOfJINSourceType.Value).ToString());

        var JINFilePath = AppManager.Instance.AudioController.GetFilePath(JINSourceType: (EJINSourceType)_presentIndexOfJINSourceType.Value);
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldFileName", text: "file: " + JINFilePath);
    }


    private void IncrementJINType(bool isPlus)
    {
        if (isPlus)
        {
            _presentIndexOfJINType.Increment();
        }
        else
        {
            _presentIndexOfJINType.Decrement();
        }

        UpdateInfoOfJINTypeAndJINSourceType(JINType: (EJINType)_presentIndexOfJINType.Value);

        bool isSetButtonEnabled = (_presentIndexOfJINSourceType.Value != _previousIndexOfJINSourceType);
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: isSetButtonEnabled);
    }


    private void IncrementJINSourceType(bool isPlus)
    {
        if (isPlus)
        {
            _presentIndexOfJINSourceType.Increment();
        }
        else
        {
            _presentIndexOfJINSourceType.Decrement();
        }

        UpdateInfoOfOnlyJINSourceType(JINSourceType: (EJINSourceType)_presentIndexOfJINSourceType.Value);

        bool isSetButtonEnabled = (_presentIndexOfJINSourceType.Value != _previousIndexOfJINSourceType);
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: isSetButtonEnabled);
    }


    private void TestPlayJINType()
    {
        AppManager.Instance.AudioController.TestPlay(JINType: (EJINType)_presentIndexOfJINType.Value);
    }


    private void TestPlayJINSourceType()
    {
        AppManager.Instance.AudioController.TestPlay(JINSourceType: (EJINSourceType)_presentIndexOfJINSourceType.Value);
    }


    private void ChangeBindingOfJINTypeAndJINSourceType()
    {
        AppManager.Instance.AudioController.SetSoruceType(JINType: (EJINType)_presentIndexOfJINType.Value, JINSourceType: (EJINSourceType)_presentIndexOfJINSourceType.Value);

        _previousIndexOfJINSourceType = _presentIndexOfJINSourceType.Value;
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: false);
    }



    // ===============================================
    // MARK: - ● SE Setting ─────

    private void SetupSESetting()
    {
        UpdateInfoOfSETypeAndSESourceType(SEType: (ESEType)_presentIndexOfSEType.Value);
    }


    private void UpdateInfoOfSETypeAndSESourceType(ESEType SEType)
    {
        _presentIndexOfSEType.Value = (int)SEType;

        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSubtitleType", text: "SEType: " + (_presentIndexOfSEType.Value).ToString());
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldType", text: ((ESEType)_presentIndexOfSEType.Value).ToString());

        var SESourceType = AppManager.Instance.AudioController.GetSourceType(SEType: (ESEType)_presentIndexOfSEType.Value);
        UpdateInfoOfOnlySESourceType(SESourceType: SESourceType);

        _previousIndexOfSESourceType = (int)SESourceType;

        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: false);
    }


    private void UpdateInfoOfOnlySESourceType(ESESourceType SESourceType)
    {
        _presentIndexOfSESourceType.Value = (int)SESourceType;

        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSubtitleSourceType", text: "SESourceType: " + _presentIndexOfSESourceType.Value.ToString());
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldSourceType", text: ((ESESourceType)_presentIndexOfSESourceType.Value).ToString());

        var SEFilePath = AppManager.Instance.AudioController.GetFilePath(SESourceType: (ESESourceType)_presentIndexOfSESourceType.Value);
        AppManager.Instance.UICreator.WindowNormal.TextField.ChangeText(name: "textFieldFileName", text: "file: " + SEFilePath);
    }


    private void IncrementSEType(bool isPlus)
    {
        if (isPlus)
        {
            _presentIndexOfSEType.Increment();
        }
        else
        {
            _presentIndexOfSEType.Decrement();
        }

        UpdateInfoOfSETypeAndSESourceType(SEType: (ESEType)_presentIndexOfSEType.Value);

        bool isSetButtonEnabled = (_presentIndexOfSESourceType.Value != _previousIndexOfSESourceType);
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: isSetButtonEnabled);
    }


    private void IncrementSESourceType(bool isPlus)
    {
        if (isPlus)
        {
            _presentIndexOfSESourceType.Increment();
        }
        else
        {
            _presentIndexOfSESourceType.Decrement();
        }

        UpdateInfoOfOnlySESourceType(SESourceType: (ESESourceType)_presentIndexOfSESourceType.Value);

        bool isSetButtonEnabled = (_presentIndexOfSESourceType.Value != _previousIndexOfSESourceType);
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: isSetButtonEnabled);
    }


    private void TestPlaySEType()
    {
        AppManager.Instance.AudioController.TestPlay(SEType: (ESEType)_presentIndexOfSEType.Value);
    }


    private void TestPlaySESourceType()
    {
        AppManager.Instance.AudioController.TestPlay(SESourceType: (ESESourceType)_presentIndexOfSESourceType.Value);
    }


    private void ChangeBindingOfSETypeAndSESourceType()
    {
        AppManager.Instance.AudioController.SetSoruceType(SEType: (ESEType)_presentIndexOfSEType.Value, SESourceType: (ESESourceType)_presentIndexOfSESourceType.Value);

        _previousIndexOfSESourceType = _presentIndexOfSESourceType.Value;
        AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonChangeBinding", value: false);
    }



    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        _generalButtonListStage = null;
    }
}
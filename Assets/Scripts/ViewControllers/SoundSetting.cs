using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Assertions;


// こちらの using を挿入する必要あり
using GFramework.Core;



// ===============================================
// MARK: - ● AudioManager ─────

public class SoundSetting : MonoBehaviour, IBlankPageManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // GeneralButtonListStage への参照
    private IBlankPageStage _blankPageStage  = null;


    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        AppManager.Instance.Setup();
        _blankPageStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);
        _blankPageStage.ShowWithAnimation();

        ShowWindow();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    private void ShowWindow()
    {
        // オーディオに関する現在の設定を取得する

        // 音量(マスター)：値は 0.0f-1.0f

        bool isMuteMaster = PlayerPrefs.GetInt("IsMuteMaster") == 1 ? true : false;
        float volumeMaster = PlayerPrefs.HasKey("VolumeMaster") ? PlayerPrefs.GetInt("VolumeMaster") / 10.0f : 1.0f;
        bool isMuteBGM = PlayerPrefs.GetInt("IsMuteBGM") == 1 ? true : false;
        bool isMuteJIN = PlayerPrefs.GetInt("IsMuteJIN") == 1 ? true : false;
        bool isMuteSE = PlayerPrefs.GetInt("IsMuteSE") == 1 ? true : false;


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
               Debug.Log(isCheckBoxOn);
               AppManager.Instance.AudioController.SetIsMuteMaster(value: !isCheckBoxOn);
               PlayerPrefs.SetInt("IsMuteMaster", isCheckBoxOn ? 0 : 1);
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
               Debug.Log(newValue);
               PlayerPrefs.SetInt("VolumeMaster", (int)(newValue*10));
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
               PlayerPrefs.SetInt("IsMuteBGM", isCheckBoxOn ? 0 : 1);
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
               PlayerPrefs.SetInt("IsMuteJIN", isCheckBoxOn ? 0 : 1);
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
               PlayerPrefs.SetInt("IsMuteSE", isCheckBoxOn ? 0 : 1);
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
            name: "buttonBack",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.625f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "AppSettings", nextStageType: EStageType.BLANK_PAGE);
            });


        AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsOn(name: "checkBoxMaster", !isMuteMaster);
        AppManager.Instance.UICreator.WindowNormal.Slider.SetPresentStep(name: "sliderVolume", (int)(volumeMaster * 10));
        AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsOn(name: "checkBoxBGM", !isMuteBGM);
        AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsOn(name: "checkBoxJIN", !isMuteJIN);
        AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsOn(name: "checkBoxSE", !isMuteSE);

        AppManager.Instance.UICreator.WindowNormal.Slider.SetIsEnabled(name: "sliderVolume", value: !isMuteMaster);
        AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsEnabled(name: "checkBoxBGM", value: !isMuteMaster);
        AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsEnabled(name: "checkBoxJIN", value: !isMuteMaster);
        AppManager.Instance.UICreator.WindowNormal.CheckBox.SetIsEnabled(name: "checkBoxSE", value: !isMuteMaster);


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }
}

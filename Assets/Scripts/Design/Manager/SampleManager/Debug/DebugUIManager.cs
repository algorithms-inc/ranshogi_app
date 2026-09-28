using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// こちらの using を挿入する必要あり
using GFramework.Core;
using GFramework;


// ===============================================
// MARK: - ● DebugUIManager ─────

public class DebugUIManager : MonoBehaviour, IGeneralButtonListManager
{
    // ===============================================
    // MARK: - ● Property ─────

    // GeneralButtonListStage への参照
    private IGeneralButtonListStage _generalButtonListStage = null;



    // ===============================================
    // MARK: - ● Method ─────

    void Start()
    {
        AppManager.Instance.Setup();

        SetupStage();

        SetupStatusBar();
    }


    private void SetupStage()
    {
        var buttonTextList = new List<string> {
            "Layout",
            "TextField",
            "Button",
            "ImageBox",
            "InputField",
            "CheckBox",
            "RadioButtonGroup",
            "Slider",
            "WindowError",
            "Debug Menuに戻る" };


        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "Debug UI",
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



    // ===============================================
    // MARK: - ● On Event ─────

    public void OnDidShowAnimation()
    {
        Debug.Log(GetType().ToString() + ": OnDidShowAnimation");
    }


    // Demo for Layout
    public void OnTapButton0()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.25f)),
                text: "WindowのUIは端末によらず、横：縦＝1：1.6のエリアに表示されます。ここでは参考のためにグリッドを表示しています。UIの表示位置やサイズは、開始点と終点、あるいは、開始点と幅と高さで指定します。例えばWindow中央の座標は、(x,y)=(0.5f,0.8f)となります。",
                fontScale: 0.8f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 1.6f)),
            filePath: "UI/ImageBox/imagebox_dummy_grid.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonShowWindowConfirmation",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.25f),
                width: 1f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "Show Confirmation",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { ShowWindowConfirmation(); });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    private void ShowWindowConfirmation()
    {
        AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
            name: "textFieldMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(0.9f, 0.6f)),
            text: "確認メッセージの表示例です。幅は0.9fまで、高さは0.8fまでです。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
            name: "buttonBack",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.625f),
                endPoint: new Vector2(0.425f, 0.8f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                Debug.Log("戻るボタンが押されました");
                // WindowConfirmationを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: ()=> {
                    Debug.Log("WindowConfirmationの非表示が完了しました");
                });
            });

        AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
            name: "buttonDelete",
            area: new FWindowElementArea(
                origin: new Vector2(0.475f, 0.625f),
                endPoint: new Vector2(0.9f, 0.8f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "削除",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                // タップ時のコールバック
                Debug.Log("削除ボタンが押されました");
                // WindowConfirmationを非表示にする操作。コールバックの設定も可能
                AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: ()=> {
                    Debug.Log("WindowConfirmationの非表示が完了しました");
                });
            });

        AppManager.Instance.UICreator.WindowConfirmation.Show(onDidShowAction: null);
    }


    // Demo for TextField
    public void OnTapButton1()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.5f)),
                text: "TextFieldには、テキストを表示できます。通常のサイズの場合、20行ほど記述することができます。",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldB",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.5f),
                endPoint: new Vector2(1f, 0.75f)),
            text: "大きさ、書体、色、テキストエリアに対する配置などを指定することができます。",
            fontScale: 0.75f,
            fontType: EFontType.BLOCK,
            textColorType: EWindowTextColorType.CAUTION,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textFieldC",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.75f),
                endPoint: new Vector2(1f, 1f)),
            text: "タップを取得することもできます",
            fontScale: 1f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.LINK,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: () => { Debug.Log("textFieldC が押されました"); });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // Demo for Button
    public void OnTapButton2()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.25f)),
                text: "Buttonは一般的なボタン操作を提供します。",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.25f),
                endPoint: new Vector2(1f, 0.4f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "ButtonA:Default",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("ButtonAが押されました"); });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonB",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.45f),
                endPoint: new Vector2(1f, 0.6f)),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "ButtonB:Positive",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("ButtonBが押されました"); });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonC",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.65f),
                endPoint: new Vector2(1f, 0.8f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "ButtonC:Warning",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("ButtonCが押されました"); });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonD",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.85f),
                endPoint: new Vector2(1f, 1.0f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "ButtonD:Disabled",
            isPlaySE: true,
            isEnabled: false,
            actionOnTap: () => { Debug.Log("ButtonDが押されました"); });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }

    // Demo for ImageBox
    public void OnTapButton3()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.25f)),
                text: "ImageBoxには画像を表示することができます。画像は、指定のサイズにフィットするようにストレッチされます。タップ時のコールバックの有無は任意です。",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.8f),
                endPoint: new Vector2(1f, 0.95f)),
            filePath: "UI/ImageBox/imagebox_dummy_a.png",
            actionOnTap: () => { Debug.Log("ImageBoxAが押されました"); });

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBoxB",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.05f),
                endPoint: new Vector2(1f, 1.2f)),
            filePath: "UI/ImageBox/imagebox_dummy_b.png",
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
           name: "buttonToggle",
           area: new FWindowElementArea(
               origin: new Vector2(0f, 1.25f),
               endPoint: new Vector2(1f, 1.4f)),
           buttonDesignType: EUIButtonDesignType.DEFAULT,
           text: "Change ImageBoxA",
           isPlaySE: true,
           isEnabled: true,
           actionOnTap: () =>
           {
               AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBoxA", filePath: "UI/ImageBox/imagebox_dummy_b.png");
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
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // Demo for InputField
    public void OnTapButton4()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.25f)),
                text: "InputFiledは入力フォームを提供します。実際のテキスト入力UIは別途実装する必要があります。",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "inputFieldA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.5f),
                endPoint: new Vector2(1f, 0.65f)),
            actionOnTap: () => { Debug.Log("inputFieldA が押されました"); },
            explanationText: "半角英数字8文字以上",
            text: "Taro Yamada",
            maxDisplayTextLength: 7,
            isPasswordMode: false,
            hasError: false,
            isPlaySE: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "inputFieldB",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.7f),
                endPoint: new Vector2(1f, 0.85f)),
            actionOnTap: () => { Debug.Log("inputFieldB が押されました"); },
            explanationText: "半角英数字8文字以上",
            text: "Taro Yamada",
            isPasswordMode: false,
            hasError: false,
            isPlaySE: false,
            isEnabled: false);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "inputFieldC",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.9f),
                endPoint: new Vector2(1f, 1.05f)),
            actionOnTap: () => { Debug.Log("inputFieldC が押されました"); },
            explanationText: "半角英数字8文字以上",
            text: "Taro Yamada",
            isPasswordMode: true,
            hasError: false,
            isPlaySE: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
           name: "inputFieldD",
           area: new FWindowElementArea(
               origin: new Vector2(0f, 1.1f),
               endPoint: new Vector2(1f, 1.25f)),
           actionOnTap: () => { Debug.Log("inputFieldD が押されました"); },
           explanationText: "半角英数字8文字以上",
           text: "",
           isPasswordMode: true,
           hasError: true,
           isPlaySE: false,
           isEnabled: true);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // Demo for CheckBox
    public void OnTapButton5()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.25f)),
                text: "CheckBoxは一般的なチェックボックスを提供します。付属するテキストの有無、タップ時のコールバックの有無は任意です。",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxA",
           area: new FWindowElementArea(
               origin: new Vector2(0f, 0.6f),
               endPoint: new Vector2(0.15f, 0.75f)),
           actionOnTap: () => { Debug.Log("CheckBoxA が押されました"); },
           isOn: true,
           hasError: false,
           isEnabled: true,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f, 0.6f),
                   endPoint: new Vector2(0.8f, 0.75f)),
               text: "CheckBoxA",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxB",
           area: new FWindowElementArea(
               origin: new Vector2(0f, 0.8f),
               endPoint: new Vector2(0.15f, 0.95f)),
           actionOnTap: () => { Debug.Log("CheckBoxB が押されました"); },
           isOn: false,
           hasError: true,
           isEnabled: true,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f, 0.8f),
                   endPoint: new Vector2(0.8f, 0.95f)),
               text: "CheckBoxB",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxC",
           area: new FWindowElementArea(
               origin: new Vector2(0f, 1.0f),
               endPoint: new Vector2(0.15f, 1.15f)),
           actionOnTap: () => { Debug.Log("CheckBoxC が押されました"); },
           isOn: false,
           hasError: true,
           isEnabled: false,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f, 1.0f),
                   endPoint: new Vector2(0.8f, 1.15f)),
               text: "CheckBoxC",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxD",
           area: new FWindowElementArea(
               origin: new Vector2(0f, 1.2f),
               endPoint: new Vector2(0.15f, 1.35f)),
           actionOnTap: null,
           isOn: true,
           hasError: false,
           isEnabled: true,
           companionTextFieldSetting: null);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // Demo for RadioButtonGroup
    public void OnTapButton6()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.25f)),
                text: "RadioButtonGroupは一般的なラジオボタンを提供します。付属するテキストの有無、タップ時のコールバックの有無は任意です。",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.Add(
           name: "radioButtonGroupA",
           areaList: new List<FWindowElementArea> {
                   new FWindowElementArea(origin: new Vector2(0f, 0.6f),  endPoint: new Vector2(0.15f, 0.75f)),
                   new FWindowElementArea(origin: new Vector2(0f, 0.8f),  endPoint: new Vector2(0.15f, 0.95f)),
                   new FWindowElementArea(origin: new Vector2(0f, 1.0f),  endPoint: new Vector2(0.15f, 1.15f)),
                   },
           companionTextFieldSettingList: new List<FCompanionTextFieldSetting> {
                   new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.6f), endPoint: new Vector2(1.0f, 0.75f)),
                       text: "RadioButton0",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
                  new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.8f), endPoint: new Vector2(1.0f, 0.95f)),
                       text: "RadioButton1",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
                  new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 1.0f), endPoint: new Vector2(1.0f, 1.15f)),
                       text: "RadioButton2",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
           },
           indexOfOn: 0,
           actionOnTap: () => { Debug.Log($"RadioButton{AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.GetIndexOfOn(name: "radioButtonGroupA")} が押されました"); },
           isEnabled: true);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonToggle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.25f),
                endPoint: new Vector2(1f, 1.4f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "Toggle Enability",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () =>
            {
                var presentValue = AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.IsEnabled(name: "radioButtonGroupA");
                AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.SetIsEnabled(name: "radioButtonGroupA", value: !presentValue);
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
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // Demo for Slider
    public void OnTapButton7()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "textFieldA",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0f),
                    endPoint: new Vector2(1f, 0.25f)),
                text: "Sliderはスライドによる値指定機能を提供します。値が変更された時のコールバックの有無は任意です。",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_LEFT,
                actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.Slider.Add(
           name: "sliderA",
           area: new FWindowElementArea(
               origin: new Vector2(0.0f, 0.6f),
               endPoint: new Vector2(1.0f, 0.75f)),
           maxStep: 10,
           presentStep: 0,
           actionOnChangeStep: () => { Debug.Log($"値が {AppManager.Instance.UICreator.WindowNormal.Slider.GetPresentStep(name: "sliderA")} に変更されました"); },
           isPlaySE: true,
           isEnabled: true);


        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonIncrement",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.05f),
                endPoint: new Vector2(1f, 1.2f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "Increment Step",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () =>
            {
                AppManager.Instance.UICreator.WindowNormal.Slider.AddPresentStep(name: "sliderA", deltaStep: 1);
            });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonToggle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.25f),
                endPoint: new Vector2(1f, 1.4f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "Toggle Enability",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () =>
            {
                var presentValue = AppManager.Instance.UICreator.WindowNormal.Slider.IsEnabled(name: "sliderA");
                AppManager.Instance.UICreator.WindowNormal.Slider.SetIsEnabled(name: "sliderA", value: !presentValue);
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
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    // Demo for WindowError
    public void OnTapButton8()
    {
        AppManager.Instance.UICreator.WindowError.TextField.Add(
            name: "textFieldA",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.25f)),
            text: "WindowErrorは、WindowNormalと重ねて表示することができる、エラー表示用のWindowです。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);



        AppManager.Instance.UICreator.WindowError.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.45f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: "CLOSE",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => { Debug.Log("CLOSEボタンが押されました"); AppManager.Instance.UICreator.WindowError.Hide(onDidHideAction: null); });

        AppManager.Instance.UICreator.WindowError.Show(onDidShowAction: null);
    }


    public void OnTapButton9()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "DebugMenuStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }
    


    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        _generalButtonListStage = null;
    }
}
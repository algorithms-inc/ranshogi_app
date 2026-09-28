using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// こちらの using を挿入する必要あり
using GFramework.Core;
using GFramework;
using UnityEditor;


// ===============================================
// MARK: - ● SampleSettingBoardManager ─────

public class BoardSettingViewController : MonoBehaviour, IGeneralButtonListManager
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
            "盤面の表示",
            "駒：おもて",
            "駒：うら(飛角)",
            "駒：うら(銀桂香)",
            "名前の表示",
            "",
            "",
            "",
            "",
            "戻る" };


        _generalButtonListStage = AppManager.Instance.CreateGeneralButtonListStage(
            generalButtonListManager: this,
            title: "盤面設定",
            buttonTextList: buttonTextList);

        _generalButtonListStage.ShowWithAnimation();
    }


    private void SetupStatusBar()
    {
    }



    // ===============================================
    // MARK: - ● On Event ─────

    public void OnDidShowAnimation()
    {
        Debug.Log(GetType().ToString() + ": OnDidShowAnimation");
    }

    #region OnTapButton0
    public void OnTapButton0()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textTitle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "盤面の表示",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxIsShowNumOnBoard",
           area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3125f),
                width: 0.125f, height: 0.125f),
           actionOnTap: () => {
                var isOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxIsShowNumOnBoard");
                PlayerPrefs.SetInt("ShowNumOnBoard", isOn ? 1 : 0);
                AppManager.Instance.DesignController.SetIsShowNumOnBoard(value: isOn);
           },
           isOn: AppManager.Instance.DesignController.IsShowNumOnBoard(),
           hasError: false,
           isEnabled: true,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f + 0.0125f, 0.3f),
                   endPoint: new Vector2(1f, 0.45f)),
               text: "段と筋を表示する",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


       AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxIsChangeBackground",
           area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3125f + 0.2f),
                width: 0.125f, height: 0.125f),
           actionOnTap: () => {
                var isOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxIsChangeBackground");
                PlayerPrefs.SetInt("ChangeBG", isOn ? 1 : 0);
                AppManager.Instance.DesignController.SetIsChangeBackground(value: isOn);
           },
           isOn: AppManager.Instance.DesignController.IsChangeBackground(),
           hasError: false,
           isEnabled: true,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f + 0.0125f, 0.3f + 0.2f),
                   endPoint: new Vector2(1f, 0.45f + 0.2f)),
               text: "相手の番に背景変更",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
           name: "checkBoxIsShowLastMove",
           area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3125f + 0.4f),
                width: 0.125f, height: 0.125f),
           actionOnTap: () => {
                var isOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxIsShowLastMove");
                PlayerPrefs.SetInt("ShowLastMove", isOn ? 1 : 0);
                AppManager.Instance.DesignController.SetIsShowLastMove(value: isOn);
           },
           isOn: AppManager.Instance.DesignController.IsShowLastMove(),
           hasError: false,
           isEnabled: true,
           companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f + 0.0125f, 0.3f + 0.4f),
                   endPoint: new Vector2(1f, 0.45f + 0.4f)),
               text: "相手の移動先を強調",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));



        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.425f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "閉じる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null); 
    }
    #endregion

    #region OnTapButton1
    public void OnTapButton1()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textTitle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "駒：おもて",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox1",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.125f),
                endPoint: new Vector2(0.33f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_0_a0_p1.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox2",
            area: new FWindowElementArea(
                origin: new Vector2(0.33f, 0.125f),
                endPoint: new Vector2(0.66f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_1_a0_p1.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox3",
            area: new FWindowElementArea(
                origin: new Vector2(0.66f, 0.125f),
                endPoint: new Vector2(1.0f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_5_a0_p1.png",
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.Add(
           name: "radioButtonGroupDesignIDOfPiceFront",
           areaList: new List<FWindowElementArea> {
                   new FWindowElementArea(origin: new Vector2(0.0125f, 0.6f), width: 0.125f, height: 0.125f),
                   new FWindowElementArea(origin: new Vector2(0.0125f, 0.8f), width: 0.125f, height: 0.125f)
                   },
           companionTextFieldSettingList: new List<FCompanionTextFieldSetting> {
                   new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.6f - 0.0125f), endPoint: new Vector2(1.0f, 0.75f - 0.0125f)),
                       text: "デザイン1",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
                  new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.8f - 0.0125f), endPoint: new Vector2(1.0f, 0.95f - 0.0125f)),
                       text: "デザイン2",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT)
           },
           indexOfOn: AppManager.Instance.DesignController.GetDesignIDOfPiceFront(),
           actionOnTap: () => {
                var index = AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.GetIndexOfOn(name: "radioButtonGroupDesignIDOfPiceFront");
                AppManager.Instance.DesignController.SetDesignIDOfPieceFront(value: index);
                PlayerPrefs.SetInt("PDPieceFront", (int)index);
                UpdateImageBoxPieceView0();
            },
           isEnabled: true);



        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.425f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "閉じる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });

        UpdateImageBoxPieceView0();
        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }

    private void UpdateImageBoxPieceView0()
    {
        if (AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.GetIndexOfOn(name: "radioButtonGroupDesignIDOfPiceFront") == 0)
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_0_a0_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_1_a0_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox3", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_5_a0_p1.png");
        }
        else
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_0_a1_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_1_a1_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox3", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_5_a1_p1.png");
        }
    }
    #endregion

    #region OnTapButton2
    public void OnTapButton2()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textTitle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "駒：うら(飛角)",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox1",
            area: new FWindowElementArea(
                origin: new Vector2(0.165f, 0.125f),
                endPoint: new Vector2(0.495f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_1_b0_p1.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox2",
            area: new FWindowElementArea(
                origin: new Vector2(0.505f, 0.125f),
                endPoint: new Vector2(0.835f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_2_b0_p1.png",
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.Add(
           name: "radioButtonGroupDesignIDOfPieceBackForGroupRook",
           areaList: new List<FWindowElementArea> {
                   new FWindowElementArea(origin: new Vector2(0.0125f, 0.6f), width: 0.125f, height: 0.125f),
                   new FWindowElementArea(origin: new Vector2(0.0125f, 0.8f), width: 0.125f, height: 0.125f),
                   new FWindowElementArea(origin: new Vector2(0.0125f, 1.0f), width: 0.125f, height: 0.125f)
                   },
           companionTextFieldSettingList: new List<FCompanionTextFieldSetting> {
                   new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.6f - 0.0125f), endPoint: new Vector2(1.0f, 0.75f - 0.0125f)),
                       text: "デザイン1",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
                  new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.8f - 0.0125f), endPoint: new Vector2(1.0f, 0.95f - 0.0125f)),
                       text: "デザイン2",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
                  new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 1.0f - 0.0125f), endPoint: new Vector2(1.0f, 1.15f - 0.0125f)),
                       text: "デザイン3",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT)
           },
           indexOfOn: AppManager.Instance.DesignController.GetDesignIDOfPieceBackForGroupRook(),
           actionOnTap: () => {
                var index = AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.GetIndexOfOn(name: "radioButtonGroupDesignIDOfPieceBackForGroupRook");
                AppManager.Instance.DesignController.SetDesignIDOfPieceBackForGroupRook(value: index);
                PlayerPrefs.SetInt("PDGroupRook", (int)index);
                UpdateImageBoxPieceView1();
            },
           isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.425f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "閉じる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });

        UpdateImageBoxPieceView1();
        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    private void UpdateImageBoxPieceView1()
    {
        var index = AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.GetIndexOfOn(name: "radioButtonGroupDesignIDOfPieceBackForGroupRook");

        if (index == 0)
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_1_b0_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_2_b0_p1.png");
        }
        else if (index == 1)
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_1_b1_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_2_b1_p1.png");
        }
        else
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_1_b2_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_2_b2_p1.png");
        }
    }
    #endregion

    #region OnTapButton3
    public void OnTapButton3()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textTitle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "駒：うら(銀桂香)",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox1",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.125f),
                endPoint: new Vector2(0.33f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_4_b0_p1.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox2",
            area: new FWindowElementArea(
                origin: new Vector2(0.33f, 0.125f),
                endPoint: new Vector2(0.66f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_5_b0_p1.png",
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "imageBox3",
            area: new FWindowElementArea(
                origin: new Vector2(0.66f, 0.125f),
                endPoint: new Vector2(1.0f, 0.5f)),
            filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_6_b0_p1.png",
            actionOnTap: null);


        AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.Add(
           name: "radioButtonGroupDesignIDOfPieceBackForGroupSilver",
           areaList: new List<FWindowElementArea> {
                   new FWindowElementArea(origin: new Vector2(0.0125f, 0.6f), width: 0.125f, height: 0.125f),
                   new FWindowElementArea(origin: new Vector2(0.0125f, 0.8f), width: 0.125f, height: 0.125f),
                   new FWindowElementArea(origin: new Vector2(0.0125f, 1.0f), width: 0.125f, height: 0.125f)
                   },
           companionTextFieldSettingList: new List<FCompanionTextFieldSetting> {
                   new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.6f - 0.0125f), endPoint: new Vector2(1.0f, 0.75f - 0.0125f)),
                       text: "デザイン1",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
                  new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 0.8f - 0.0125f), endPoint: new Vector2(1.0f, 0.95f - 0.0125f)),
                       text: "デザイン2",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT),
                  new FCompanionTextFieldSetting(
                       windowElementArea: new FWindowElementArea(origin: new Vector2(0.2f, 1.0f - 0.0125f), endPoint: new Vector2(1.0f, 1.15f - 0.0125f)),
                       text: "デザイン3",
                       fontType: EFontType.SERIF,
                       fontScale: 1.0f,
                       alignment: EAlignment.MIDDLE_LEFT)
           },
           indexOfOn: AppManager.Instance.DesignController.GetDesignIDOfPieceBackForGroupSilver(),
           actionOnTap: () => {
                var index = AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.GetIndexOfOn(name: "radioButtonGroupDesignIDOfPieceBackForGroupSilver");
                AppManager.Instance.DesignController.SetDesignIDOfPieceBackForGroupSilver(value: index);
                PlayerPrefs.SetInt("PDGroupSilver", (int)index);
                UpdateImageBoxPieceView2();
            },
           isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonClose",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.425f),
                endPoint: new Vector2(1f, 1.6f)),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "閉じる",
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
            });

        UpdateImageBoxPieceView2();
        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }


    private void UpdateImageBoxPieceView2()
    {
        var index = AppManager.Instance.UICreator.WindowNormal.RadioButtonGroup.GetIndexOfOn(name: "radioButtonGroupDesignIDOfPieceBackForGroupSilver");

        if (index == 0)
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_4_b0_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_5_b0_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox3", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_6_b0_p1.png");
        }
        else if (index == 1)
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_4_b1_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_5_b1_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox3", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_6_b1_p1.png");
        }
        else
        {
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox1", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_4_b2_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox2", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_5_b2_p1.png");
            AppManager.Instance.UICreator.WindowNormal.ImageBox.ChangeImage(name: "imageBox3", filePath: "Stage/Common/BoardSet/Pieces/d0/p1/piece_d0_6_b2_p1.png");
        }
    }
    #endregion

        #region OnTapButton4
    public void OnTapButton4()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "textTitle",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.15f)),
            text: "名前の表示",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER,
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
            name: "checkBoxShowNameP2",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3125f),
                width: 0.125f, height: 0.125f),
            actionOnTap: () => { 
                    var isOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxShowNameP2");
                    PlayerPrefs.SetInt("ShowOpponentName", isOn ? 1 : 0);
                    if (isOn) {
                        Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER2);
                    } else {
                        Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER2);
                    }},
            isOn: Locator.Instance.statusBarManager.IsNameShown(playerType: EPlayerType.PLAYER2),
            hasError: false,
            isEnabled: true,
            companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f + 0.0125f, 0.3f),
                   endPoint: new Vector2(1f, 0.45f)),
               text: "相手の名前を表示",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


        AppManager.Instance.UICreator.WindowNormal.CheckBox.Add(
            name: "checkBoxShowNameP1",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.3125f + 0.2f),
                width: 0.125f, height: 0.125f),
            actionOnTap: () => { 
                    var isOn = AppManager.Instance.UICreator.WindowNormal.CheckBox.IsOn(name: "checkBoxShowNameP1");
                    PlayerPrefs.SetInt("ShowPlayerName", isOn ? 1 : 0);
                    if (isOn) {
                        Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER1);
                    } else {
                        Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER1);
                    }},
            isOn: Locator.Instance.statusBarManager.IsNameShown(playerType: EPlayerType.PLAYER1),
            hasError: false,
            isEnabled: true,
            companionTextFieldSetting: new FCompanionTextFieldSetting(
               windowElementArea: new FWindowElementArea(
                   origin: new Vector2(0.2f + 0.0125f, 0.3f + 0.2f),
                   endPoint: new Vector2(1f, 0.45f + 0.2f)),
               text: "自分の名前を表示",
               fontType: EFontType.SERIF,
               fontScale: 1f,
               alignment: EAlignment.MIDDLE_LEFT));


            AppManager.Instance.UICreator.WindowNormal.Button.Add(
                name: "buttonClose",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 1.425f),
                    endPoint: new Vector2(1f, 1.6f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "閉じる",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: null);
                });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
    }
    #endregion
    
    public void OnTapButton5() { /* Do Nothing */ }
    public void OnTapButton6() { /* Do Nothing */ }
    public void OnTapButton7() { /* Do Nothing */ }
    public void OnTapButton8() { /* Do Nothing */ }

    public void OnTapButton9()
    {
        _generalButtonListStage.HideWithAnimationAndChangeScene(nextSceneName: "AppSettings", nextStageType: EStageType.GENERAL_BUTTON_LIST);
    }



    // ===============================================
    // MARK: - ● MonoBehaviour としての破壊処理 ─────

    public void OnDestroy()
    {
        _generalButtonListStage = null;
    }
}
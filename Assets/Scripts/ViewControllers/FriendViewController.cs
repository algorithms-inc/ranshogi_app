using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using TMPro;
using GFramework.Core;


public class FriendViewController : MonoBehaviour, IBlankPageManager
{
    [SerializeField] Canvas canvas;
    [SerializeField] LoadingViewController loadingViewPrefab;
    [SerializeField] TextMeshProUGUI createdText;
    [SerializeField] TextMeshProUGUI roomCodeText;
    [SerializeField] TMP_InputField codeInput;
    [SerializeField] Button copyButton;

    private TouchScreenKeyboard keyboard;
    private string currentText = "";
    private LoadingViewController loadingView;

    IBlankPageStage _inputNicknameStage = null;

    private EBoardType _boardType = EBoardType.BOARD_66;


    void Start()
    {
        AppManager.Instance.Setup();
        _inputNicknameStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);
        _inputNicknameStage.ShowWithAnimation();
        currentText = GameManager.instance.globalVariables.roomCode;

        ShowWindow();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    void Update()
    {
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFRoomId", keyboard.text);
            AppManager.Instance.UICreator.WindowNormal.Button.SetIsEnabled(name: "buttonEnterRoom", value: true);
            currentText = keyboard.text;
        }
    }


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
                   GUIUtility.systemCopyBuffer = currentText;
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
                keyboard = TouchScreenKeyboard.Open(currentText, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "部屋ID",
            text: (GameManager.instance.globalVariables.roomCode != null && GameManager.instance.globalVariables.roomCode != "") ? GameManager.instance.globalVariables.roomCode : "",
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
               isEnabled: (GameManager.instance.globalVariables.roomCode != null && GameManager.instance.globalVariables.roomCode != ""),
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
                    _inputNicknameStage.HideWithAnimationAndChangeScene(nextSceneName: "Menu", nextStageType: EStageType.MENU);
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


    public void CreateRoomCodeButtonTapped() {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("発行中...", false, () => { });
        RoomCodeRequest request = new RoomCodeRequest();
        request._requestJson = JsonUtility.ToJson(new EmptyStruct());

        var context = SynchronizationContext.Current;
        Api.RoomCode(request, (roomCode) => {
            context.Post((state2) => {
                Destroy(loadingView.gameObject);
                //roomCodeText.text = roomCode.room_code;
                //codeInput.text = roomCode.room_code;
                //roomCodeText.gameObject.SetActive(true);
                //createdText.gameObject.SetActive(true);
                //copyButton.gameObject.SetActive(true);
                currentText = roomCode.room_code;
                DisplayRoomID(roomID: roomCode.room_code);
            }, null);
        }, (errorMessage) => {
            Debug.Log(errorMessage);
            Destroy(loadingView.gameObject);
        });
    }

    public void JoinRoomButtonTapped() {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("入室しています...", false, () => { });
        RoomCodeCheckRequest request = new RoomCodeCheckRequest();
        request._requestJson = JsonUtility.ToJson(new RoomCodeCheckRequest.RoomCode{room_code = currentText});

        var context = SynchronizationContext.Current;
        Api.CheckRoomCode(request, (empty) => {
            context.Post((state2) => {
                Destroy(loadingView.gameObject);
                GameManager.instance.globalVariables.roomCode = currentText;

                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: () => {
                    _inputNicknameStage.HideWithAnimationAndChangeScene(nextSceneName: "Game", nextStageType: EStageType.GAME);
                });
            }, null);
        }, (errorMessage) => {
            Debug.Log(errorMessage);
            Destroy(loadingView.gameObject);
        });
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
}

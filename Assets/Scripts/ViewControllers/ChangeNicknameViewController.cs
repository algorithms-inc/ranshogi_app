using UnityEngine;
using TMPro;
using System.Threading;
using GFramework.Core;


public class ChangeNicknameViewController : MonoBehaviour, IBlankPageManager
{
    [SerializeField] Canvas canvas;
    [SerializeField] LoadingViewController loadingViewPrefab;
    private LoadingViewController loadingView;
    [SerializeField] AlertViewController alertPrefab;

    private TouchScreenKeyboard keyboard;
    private string currentText = "";

    IBlankPageStage _inputNicknameStage = null;

    void Start()
    {
        AppManager.Instance.Setup();
        TouchScreenKeyboard.hideInput = true;
        _inputNicknameStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);

        _inputNicknameStage.ShowWithAnimation();
        ShowWindow();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    public void updateNickname() {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("アップデート中 ...", false, () => { });
        UpdateNicknameRequest request = new UpdateNicknameRequest();
        UpdateNicknameRequest.Nickname nickname = new UpdateNicknameRequest.Nickname();
        nickname.nickname = currentText;
        request._requestJson = JsonUtility.ToJson(nickname);
        var context = SynchronizationContext.Current;
        Api.UpdateNickname(request, (history) => {
            context.Post((state2) => {
                Destroy(loadingView.gameObject);
                //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                //alertView.Init("ニックネームを更新しました。");
                var user = Auth.instance.GetUser();
                user.nickname = currentText;
                Auth.instance.SetUser(user);
                AlertViewController.ShowOKConfirmation("ニックネームを変更しました。");
            }, null);
        }, (errorMessage) => {
            context.Post((state2) => {
                Debug.Log(errorMessage);
                Destroy(loadingView.gameObject);
                //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                //alertView.Init(errorMessage);
                AlertViewController.ShowOKConfirmation(errorMessage);
            }, null);
        });
    }


    void Update()
    {
        if (keyboard != null) 
        {
            AppManager.Instance.UICreator.WindowNormal.InputField.ChangeText("IFNickname", keyboard.text);
            currentText = keyboard.text;
            if(keyboard.status == TouchScreenKeyboard.Status.Done) {
                keyboard = null;
            }
        }
    }


    // ===============================================
    // MARK: - ● Show Window Normal ─────

    private void ShowWindow()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "ニックネーム変更",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFNickname",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.4f)),
            text: "対局で表示されるニックネームをご入力ください。",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        string defaultText = Auth.instance.GetUser().nickname;
        //string defaultText = "moriteru2";
        //string debugText = "";
        currentText = defaultText;

        AppManager.Instance.UICreator.WindowNormal.InputField.Add(
            name: "IFNickname",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.75f),
                endPoint: new Vector2(1f, 0.92f)),
            actionOnTap: () =>
            {
                keyboard = TouchScreenKeyboard.Open(currentText, TouchScreenKeyboardType.Default, false, false, false);
            },
            explanationText: "半角英数字4文字以上",
            text: defaultText,
            isPasswordMode: false,
            hasError: false,
            isEnabled: true);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "backButton",
               area: new FWindowElementArea(
                   origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                   width: 0.35f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "戻る",
               isEnabled: true,
               actionOnTap: () => {
                    _inputNicknameStage.HideWithAnimationAndChangeScene(nextSceneName: "UserSettings", nextStageType: EStageType.GENERAL_BUTTON_LIST);
               });

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
               name: "buttonChange",
               area: new FWindowElementArea(
                    origin: new Vector2(0.425f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                    width: 0.575f,
                    height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.POSITIVE,
               text: "変更",
               isEnabled: true,
               actionOnTap: () => {
                    updateNickname();
               });

        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });
    }
}

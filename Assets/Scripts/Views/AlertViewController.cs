using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GFramework.Core;

public class AlertViewController : MonoBehaviour
{
    public enum Type { 
        OK,
        YesNo,
        UnableToRemove
    }

    public delegate void ButtonTapped();

    ButtonTapped okButtonTappedHandler;
    ButtonTapped yesButtonTappedHandler;

    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] Button okButton;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    public void Init(string text, Type type = Type.OK, bool willOKActinManuallySet = false) {
        tmp.text = text;
        if(type == Type.YesNo) {
            okButton.gameObject.SetActive(false);
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            noButton.onClick.AddListener(delegate { OKButtonTapped(); });
        } else if(type == Type.OK) {
            if (!willOKActinManuallySet)
            {
                okButton.onClick.AddListener(delegate { OKButtonTapped(); });
            }
        } else {
            okButton.gameObject.SetActive(false);
        }
    }

    public void SetOKButtonTappedHandler(ButtonTapped okButtonTapped)
    {
        okButtonTappedHandler = okButtonTapped;
        okButton.onClick.AddListener(delegate { okButtonTappedHandler(); });
    }

    public void SetYesButtonTappedHandler(ButtonTapped yesButtonTapped) {
        yesButtonTappedHandler = yesButtonTapped;
        yesButton.onClick.AddListener(delegate { yesButtonTappedHandler(); });
    }

    public void OKButtonTapped() {
        Destroy(gameObject);
    }


    public static void ShowOKConfirmation(string message, Type type = Type.OK, ButtonTapped handler = null)
    {
        AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
            name: "textFieldMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(0.9f, 0.8f)),
            text: message,
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        if(type == Type.UnableToRemove)
        {
            // 何も表示しない
        } else if(type == Type.YesNo) {
            AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
                name: "buttonYES",
                area: new FWindowElementArea(
                    origin: new Vector2(0.0f, 0.625f),
                    endPoint: new Vector2(0.425f, 0.8f)),
                buttonDesignType: EUIButtonDesignType.WARNING,
                text: "はい",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    // タップ時のコールバック
                    Debug.Log("「はい」ボタンが押されました");
                    handler();
                    // WindowConfirmationを非表示にする操作。コールバックの設定も可能
                    AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: ()=> {
                        Debug.Log("WindowConfirmationの非表示が完了しました");
                    });
                });
            AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
                name: "buttonNo",
                area: new FWindowElementArea(
                    origin: new Vector2(0.475f, 0.625f),
                    endPoint: new Vector2(0.9f, 0.8f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "いいえ",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    // タップ時のコールバック
                    Debug.Log("「いいえ」ボタンが押されました");
                    // WindowConfirmationを非表示にする操作。コールバックの設定も可能
                    AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: ()=> {
                        Debug.Log("WindowConfirmationの非表示が完了しました");
                    });
                });

        } else {
            AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
                name: "buttonBack",
                area: new FWindowElementArea(
                    origin: new Vector2(0.24f, 0.625f),
                    endPoint: new Vector2(0.66f, 0.80f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "閉じる",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    // タップ時のコールバック
                    Debug.Log("「閉じる」ボタンが押されました");
                    // WindowConfirmationを非表示にする操作。コールバックの設定も可能
                    AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: ()=> {
                        Debug.Log("WindowConfirmationの非表示が完了しました");
                    });
                });
        }

        AppManager.Instance.UICreator.WindowConfirmation.Show(onDidShowAction: null);
    }
}

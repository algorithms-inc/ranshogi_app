using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;

public class RegistrationReminderViewController : MonoBehaviour, IBlankPageManager
{
    private IBlankPageStage _blankPageStage = null;


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
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFFirst",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(1f, 0.1f)),
            text: "ご注意ください",
            fontScale: 1.15f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.2f),
                endPoint: new Vector2(1f, 0.8f)),
            text: "このまま対局をお楽しみいただけますが、お使いの携帯端末を買い替えるなどしてアプリがリセットされた場合、再びログインすることはできません。\nメニューの「設定」よりメールアドレスとパスワードを登録されることをお勧めいたします。",
            fontScale: 1f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_LEFT);

        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonToMenu",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.6f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.POSITIVE,
            text: "メニューへ",
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowNormal.Hide(onDidHideAction: () => {
                    _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "Menu", nextStageType: EStageType.MENU);
                });
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => {});
    }
}

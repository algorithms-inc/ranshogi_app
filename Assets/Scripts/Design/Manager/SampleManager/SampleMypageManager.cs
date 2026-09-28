using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// こちらの using を挿入する必要あり
using GFramework.Core;


public class SampleMypageViewController : MonoBehaviour, IBlankPageManager
{
    // [SerializeField] TextMeshProUGUI nicknameText;
    // [SerializeField] TextMeshProUGUI loginIDText;
    // [SerializeField] TextMeshProUGUI ratingText;
    // [SerializeField] TextMeshProUGUI winLoseText;

    private string _nicknameText = "Yamada Taro";
    private string _ratingText = "1234";

    private float _winCount = 456;
    private float _loseCount = 123;



    private IBlankPageStage _blankPageStage = null;


    void Start()
    {
        AppManager.Instance.Setup();
        _blankPageStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);
        _blankPageStage.ShowWithAnimation();

        ShowWindow();
        // nicknameText.text = "Nickname: " + Auth.instance.GetUser().nickname;
        // loginIDText.text = "LoginID : " + Auth.instance.GetUser().email;
        // ratingText.text = "Rating: " + Auth.instance.GetUser().rating;
        // winLoseText.text = Auth.instance.GetUser().nWin + "勝" + Auth.instance.GetUser().nLose + "負";
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    private void ShowWindow()
    {
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueNickname",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(1f, 0.1f)),
            //text: Auth.instance.GetUser().nickname,
            text: _nicknameText,
            fontScale: 1.2f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFSubheadingEmail",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.165f),
                endPoint: new Vector2(1f, 0.26f)),
            text: "・登録メールアドレス",
            fontScale: 0.9f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.MIDDLE_LEFT);

        // メールアドレスの登録・認証完了状況を取得する

        // Auth.instance.auth.CurrentUser.ReloadAsync();
        // var isRegistered = Auth.instance.auth.CurrentUser.Email != null && Auth.instance.auth.CurrentUser.Email != "";
        // var isEmailVerified = Auth.instance.auth.CurrentUser.IsEmailVerified;
        // var emaiText = isRegistered ? Auth.instance.auth.CurrentUser.Email : "";

        bool isRegistered = true;
        bool isEmailVerified = false;
        string emailText = "test@abc.de.fg";


        if (isRegistered)
        {
            AppManager.Instance.UICreator.WindowNormal.Button.Add(
                name: "buttonShowEmail",
                area: new FWindowElementArea(
                    origin: new Vector2(0.75f, 0.15f),
                    endPoint: new Vector2(1.0f, 0.28f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "表示",
                isEnabled: true,
                actionOnTap: () => {
                    ShowEmail(email: emailText, isVerified: isEmailVerified);
                });

            if (isEmailVerified == false)
            {
                AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                    name: "TFValueEmailStatus",
                    area: new FWindowElementArea(
                        origin: new Vector2(0f, 0.29f),
                        endPoint: new Vector2(1f, 0.39f)),
                    text: "＊認証が完了していません",
                    fontScale: 0.7f,
                    fontType: EFontType.SERIF,
                    textColorType: EWindowTextColorType.CAUTION,
                    alignment: EAlignment.TOP_RIGHT);
            }
        }
        else
        {
            // case: not registered
            AppManager.Instance.UICreator.WindowNormal.TextField.Add(
                name: "TFValueNoEmail",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.165f),
                    endPoint: new Vector2(1f, 0.26f)),
                text: "なし",
                fontScale: 0.9f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.WARNING,
                alignment: EAlignment.TOP_RIGHT);
        }

        // Rate
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueRate66",
            area: new FWindowElementArea(
                origin: new Vector2(0.19f, 0.59f),
                endPoint: new Vector2(0.52f, 0.71f)),
            //text: Auth.instance.GetUser().rating.ToString(),
            text: _ratingText,
            fontScale: 1.2f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT);

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueRate99",
            area: new FWindowElementArea(
                origin: new Vector2(0.56f, 0.59f),
                endPoint: new Vector2(0.89f, 0.71f)),
            //text: Auth.instance.GetUser().rating.ToString(),
            text: _ratingText,
            fontScale: 1.2f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT);

        // Grade Rank Icon
        string filePathOfGradeRankIcon66 = UtlProgram.ConvertGradeRankTypeToIconFilePath(gradeRankType: EGradeRankType.RANK_1, boardType: EBoardType.BOARD_66);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "IGradeRankIcon66",
            area: new FWindowElementArea(
                origin: new Vector2(0.36f, 0.735f),
                width: 0.11f,
                height: 0.11f),
            filePath: filePathOfGradeRankIcon66,
            actionOnTap: null);

        string filePathOfGradeRankCharacter66 = UtlProgram.GetFilePathOfGradeRankCharacter(gradeRankType: EGradeRankType.RANK_1);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "IGradeRankCharacter66",
            area: new FWindowElementArea(
                origin: new Vector2(0.48f, 0.78f),
                width: 0.05f,
                height: 0.05f),
            filePath: filePathOfGradeRankCharacter66,
            actionOnTap: null);

        string filePathOfGradeRankIcon99 = UtlProgram.ConvertGradeRankTypeToIconFilePath(gradeRankType: EGradeRankType.GRADE_9, boardType: EBoardType.BOARD_99);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "IGradeRankIcon99",
            area: new FWindowElementArea(
                origin: new Vector2(0.73f, 0.735f),
                width: 0.11f,
                height: 0.11f),
            filePath: filePathOfGradeRankIcon99,
            actionOnTap: null);

        string filePathOfGradeRankCharacter99 = UtlProgram.GetFilePathOfGradeRankCharacter(gradeRankType: EGradeRankType.GRADE_9);

        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "IGradeRankCharacter99",
            area: new FWindowElementArea(
                origin: new Vector2(0.845f, 0.78f),
                width: 0.05f,
                height: 0.05f),
            filePath: filePathOfGradeRankCharacter99,
            actionOnTap: null);

        // 勝率計算
        float winCount = _winCount;
        float loseCount = _loseCount;
        float matchCount = winCount + loseCount;
        float winRate = 0f;
        if (matchCount > 0) { winRate = winCount / matchCount * 100; }
        string textWinRate = winRate.ToString("F2");

        // Win Count
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueWin66",
            area: new FWindowElementArea(
                origin: new Vector2(0.1f, 0.86f),
                endPoint: new Vector2(0.46f, 0.96f)),
            text: winCount.ToString(),
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT); 

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueWin99",
            area: new FWindowElementArea(
                origin: new Vector2(0.43f, 0.86f),
                endPoint: new Vector2(0.83f, 0.96f)),
            text:winCount.ToString(),
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT); 

        // Lose Count
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueLose66",
            area: new FWindowElementArea(
                origin: new Vector2(0.1f, 0.985f),
                endPoint: new Vector2(0.46f, 1.085f)),
            text: loseCount.ToString(),
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT); 

        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueLose99",
            area: new FWindowElementArea(
                origin: new Vector2(0.43f, 0.985f),
                endPoint: new Vector2(0.83f, 1.085f)),
            text:loseCount.ToString(),
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT); 


        // Win Rate
        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueWinRate66",
            area: new FWindowElementArea(
                origin: new Vector2(0.1f, 1.11f),
                endPoint: new Vector2(0.46f, 1.21f)),
            text: textWinRate,
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT); 


        AppManager.Instance.UICreator.WindowNormal.TextField.Add(
            name: "TFValueWinRate99",
            area: new FWindowElementArea(
                origin: new Vector2(0.43f, 1.11f),
                endPoint: new Vector2(0.83f, 1.21f)),
            text: textWinRate,
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_RIGHT); 


        // Table Image
        AppManager.Instance.UICreator.WindowNormal.ImageBox.Add(
            name: "ITtable",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.47f),
                endPoint: new Vector2(1f, 0.47f + 838f/1100f)),
            filePath: "Stage/MypageStage/mypage_table.png",
            actionOnTap: null);


        // History Button
        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "buttonHistory",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.275f),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "対局履歴",
            isEnabled: true,
            actionOnTap: () => {
                _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleHistoryStageScene", nextStageType: EStageType.HISTORY);
            });


        // Back Button
        AppManager.Instance.UICreator.WindowNormal.Button.Add(
            name: "backButton",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 1.65f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 1.0f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: "戻る",
            isEnabled: true,
            actionOnTap: () => {
                _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "SampleMenuStageScene", nextStageType: EStageType.MENU);
            });


        AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: () => { });
    }


    private void ShowEmail(string email, bool isVerified)
    {
        AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
            name: "TFTitleForRuleOnEmailWindow",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0f),
                endPoint: new Vector2(0.9f, 0.15f)),
            text: "登録メールアドレス",
            fontScale: 1.0f,
            fontType: EFontType.SERIF,
            textColorType: EWindowTextColorType.NORMAL,
            alignment: EAlignment.TOP_CENTER);

        AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
            name: "TFRegisteredEmailOnEmailWindow",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.15f),
                endPoint: new Vector2(0.9f, 0.3f)),
            text: email,
            fontScale: 0.85f,
            fontType: EFontType.SERIF,
            textColorType: isVerified ? EWindowTextColorType.NORMAL : EWindowTextColorType.CAUTION,
            alignment: EAlignment.MIDDLE_CENTER);

        if (isVerified == false)
        {
            AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
                name: "TFRegisteredNoticeOnEmailWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.35f),
                    endPoint: new Vector2(0.9f, 0.5f)),
                text: "こちらのメールアドレスは、認証が完了していません",
                fontScale: 0.8f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.CAUTION,
                alignment: EAlignment.MIDDLE_LEFT);
        }

        AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
               name: "buttonBackOnEmailWindow",
               area: new FWindowElementArea(
                   origin: new Vector2(0.2f, 0.625f),
                   width: 0.5f,
                   height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
               buttonDesignType: EUIButtonDesignType.DEFAULT,
               text: "閉じる",
               isEnabled: true,
               actionOnTap: () => {
                    AppManager.Instance.UICreator.WindowConfirmation.Hide();
               });

        AppManager.Instance.UICreator.WindowConfirmation.Show(onDidShowAction: () => { });
    }
}

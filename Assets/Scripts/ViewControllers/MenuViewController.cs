using System;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using GFramework.Core;


public class MenuViewController : MonoBehaviour, IMenuManager
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI nGamesLeftText;
    [SerializeField] LoadingViewController loadingViewPrefab;
    [SerializeField] AlertViewController alertPrefab;

    private LoadingViewController loadingView;
    private int waitCount = 0;
    
    private IMenuStage _menuStage = null;


    public void Start()
    {
        DoStart();
    }

    IEnumerator WaitDeviceSizeReadable() {
        yield return new WaitForSeconds(0.1f); // 0.1秒待機
        DoStart();
        yield break;
    }

    public void DoStart() {
        AppManager.Instance.Setup();

        // 対局待機中のユーザがいるときに画面左上に表示する(シーンをまたいで1つだけ作られる)
        WaitingUsersIndicator.EnsureCreated();

        // AI対戦の設定が残っていると、メニューから始めた通常の対局までAI対戦になってしまうので消す
        GameManager.instance.globalVariables.aiGame = null;

        if(Auth.instance.isLoggedIn()) {
            Auth.instance.TokenAsync(() => {}, (errorMessage) => { });
            loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
            loadingView.Init("サーバと通信中...", false, () => { });
            //GetUserProcess();
            GetLaunchInfoProcess();
        } else if(Auth.instance.CurrentUserExist()) {
            var context = SynchronizationContext.Current;

            Auth.instance.TokenAsync(() => {}, (errorMessage) => {
                context.Post((state2) => {
                    Debug.Log("### try reload ");
                    var email = EncryptedPlayerPrefs.GetString("email", "");
                    var password = EncryptedPlayerPrefs.GetString("password", "");
                    if(email != "" && password != "") {
                        Auth.instance.Reauthenticate(email, password, () => {
                            Auth.instance.TokenAsync(() => {}, (errorMessage) => {});
                        }, (errorMessage) => {
                        });
                    }
                    //Auth.instance.ReloadCredentials(() => {}, (errorMessage) => {});
                }, null);
            });
            loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
            loadingView.Init("サーバと通信中...", false, () => { });
            InvokeRepeating(nameof(WaitIdTokenLoaded), 0, 0.1f);
        } else {
            Auth.instance.Signout();
        }


        SetupStage();
        SetUpAudio();
        SetUpBoardSetting();

        if(GameManager.instance.globalVariables.alertMessageOnMenu != null) {
            AlertViewController.ShowOKConfirmation(GameManager.instance.globalVariables.alertMessageOnMenu);
            GameManager.instance.globalVariables.alertMessageOnMenu = null;
        }
        EncryptedPlayerPrefs.SetString("test", "hogefuga");
    }


    private void SetUpAudio() { 
        float volumeMaster = (PlayerPrefs.HasKey("VolumeMaster") ?  PlayerPrefs.GetInt("VolumeMaster") : 10)  / 10.0f;
        bool isMuteMaster = PlayerPrefs.GetInt("IsMuteMaster") == 1 ? true : false;
        bool isMuteBGM = PlayerPrefs.GetInt("IsMuteBGM") == 1 ? true : false;
        bool isMuteJIN = PlayerPrefs.GetInt("IsMuteJIN") == 1 ? true : false;
        bool isMuteSE = PlayerPrefs.GetInt("IsMuteSE") == 1 ? true : false;

        AppManager.Instance.AudioController.SetVolumeMaster(volumeMaster);
        AppManager.Instance.AudioController.SetIsMuteMaster(isMuteMaster);
        AppManager.Instance.AudioController.SetIsMuteBGM(isMuteBGM);
        AppManager.Instance.AudioController.SetIsMuteJIN(isMuteJIN);
        AppManager.Instance.AudioController.SetIsMuteSE(isMuteSE);

    }

    private void SetUpBoardSetting() {
        AppManager.Instance.DesignController.SetIsShowNumOnBoard(value: PlayerPrefs.HasKey("ShowNumOnBoard") ? (PlayerPrefs.GetInt("ShowNumOnBoard") == 1) : true);
        AppManager.Instance.DesignController.SetIsChangeBackground(value: PlayerPrefs.HasKey("ChangeBG") ? (PlayerPrefs.GetInt("ChangeBG") == 1) : true);
        AppManager.Instance.DesignController.SetIsShowLastMove(value: PlayerPrefs.HasKey("ShowLastMove") ? (PlayerPrefs.GetInt("ShowLastMove") == 1) : true);
        if(PlayerPrefs.HasKey("ShowOpponentName") && PlayerPrefs.GetInt("ShowOpponentName") == 0) {
            AppManager.Instance.HideNameOnStatusBarForPlayer2();
        } else {
            AppManager.Instance.ShowNameOnStatusBarForPlayer2();
        }
        if(PlayerPrefs.HasKey("ShowPlayerName") && PlayerPrefs.GetInt("ShowPlayerName") == 0) {
            AppManager.Instance.HideNameOnStatusBarForPlayer1();
        } else {
            AppManager.Instance.ShowNameOnStatusBarForPlayer1();
        }
        //Debug.Log(PlayerPrefs.GetInt("PDGroupRook"));
        //Debug.Log(PlayerPrefs.GetInt("PDPieceFront"));
        //Debug.Log(PlayerPrefs.GetInt("PDGroupSilver"));

        int groupRook = PlayerPrefs.HasKey("PDGroupRook") ? PlayerPrefs.GetInt("PDGroupRook") : 2;
        AppManager.Instance.DesignController.SetDesignIDOfPieceBackForGroupRook(value: groupRook);
        int pieceFront = PlayerPrefs.HasKey("PDPieceFront") ? PlayerPrefs.GetInt("PDPieceFront") : 0;
        AppManager.Instance.DesignController.SetDesignIDOfPieceFront(value: pieceFront);
        int groupSilver = PlayerPrefs.HasKey("PDGroupSilver") ? PlayerPrefs.GetInt("PDGroupSilver") : 0;
        AppManager.Instance.DesignController.SetDesignIDOfPieceBackForGroupSilver(value: groupSilver);
    }


    private void SetupStage()
    {
        _menuStage = AppManager.Instance.CreateMenuStage(menuManager: this);
        _menuStage.ShowWithAnimation();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    private void GoToGame() { 
        GameManager.instance.globalVariables.roomCode = null;

        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "Game", nextStageType: EStageType.GAME);
    }


    public void onUserEarnedReward() {
        Auth.instance.GetUser().incrementAdLife(3);
        AppManager.Instance.SetLife(count: Auth.instance.GetUser().nGamesLeftWithReward());
        GoToGame();
    }


    public void WaitIdTokenLoaded() {
        if(waitCount > 120) { 
            CancelInvoke(nameof(WaitIdTokenLoaded));
            Destroy(loadingView.gameObject);
            AlertViewController.ShowOKConfirmation("サーバとの通信に失敗しました。しばらく時間を置いてからアプリを再起動してください。", AlertViewController.Type.UnableToRemove);
        }
        if (Auth.instance.isLoggedIn())
        {
            CancelInvoke(nameof(WaitIdTokenLoaded));
            GetLaunchInfoProcess();
        }
        waitCount++;
    }


    private void GetLaunchInfoProcess() {
        Debug.Log("In LaunchInfoProcess");
        LaunchInfoRequest request = new LaunchInfoRequest();
        var context = SynchronizationContext.Current;
        Api.GetLaunchInfo(request, (launchInfo) => {
            User user = launchInfo.user;
            Auth.instance.SetUser(user);
            
            if(user.latestGame.state == GameState.OnGame) {
                GameManager.instance.globalVariables.gameForReconnection = user.latestGame;
                GameManager.instance.globalVariables.boardType = user.latestGame.boardType == "shogi" ? EBoardType.BOARD_99 : EBoardType.BOARD_66;
                _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "Game", nextStageType: EStageType.GAME);
                return;
            }
            context.Post((state2) => {
                if(user.nGamesLeftWithReward() > 0) {
                    //nGamesLeftText.text = "本日残り " + user.nGamesLeftWithReward().ToString() + " 回対戦できます。";
                } else {
                    //nGamesLeftText.text = "本日の対局回数は残っていません。広告を見て対局しますか？";
                }
                int lastReadId = PlayerPrefs.GetInt("AnnouncementId");
                //if(launchInfo.announcement != null && launchInfo.announcement.id != lastReadId) {
                if(launchInfo.announcement != null && launchInfo.announcement.id != lastReadId) {
                    switch(launchInfo.announcement.announcement_type) {
                        // 強制アップデート
                        case 3:
                            if(launchInfo.announcement.description.CompareTo(Application.version) > 0) {

                            #if UNITY_ANDROID 
                                AlertViewController.ShowOKConfirmation("最新のバージョン（" + launchInfo.announcement.description + "）をGoogle Play ストアからダウンロードしてください。", AlertViewController.Type.UnableToRemove);
                            #endif

                            #if UNITY_IOS 
                                AlertViewController.ShowOKConfirmation("最新のバージョン（" + launchInfo.announcement.description + "）をApp Storeからダウンロードしてください。", AlertViewController.Type.UnableToRemove);
                            #endif

                            } else {
                                PlayerPrefs.SetInt("AnnouncementId", launchInfo.announcement.id);
                            }
                            break;
                        case 2:
                            AlertViewController.ShowOKConfirmation(launchInfo.announcement.description, AlertViewController.Type.UnableToRemove);
                            break;
                        case 1:
                            AlertViewController.ShowOKConfirmation(launchInfo.announcement.description);
                            PlayerPrefs.SetInt("AnnouncementId", launchInfo.announcement.id);
                            break;
                    }
                }
                AppManager.Instance.SetLife(count: Auth.instance.GetUser().nGamesLeftWithReward());
                AppManager.Instance.SetPlayerName(playerName: Auth.instance.GetUser().nickname);
                AppManager.Instance.SetRate(rate: Auth.instance.GetUser().RatingOf(GameManager.instance.globalVariables.boardType));
                AppManager.Instance.SetIsPremiumUser(Auth.instance.GetUser().isPayedUser);
                AppManager.Instance.ShowStatusBar();
                Destroy(loadingView.gameObject);

                // 「対局待機中のユーザがいます」の通知から来たときは、そのまま対局を始める
                if (GameManager.instance.globalVariables.shouldStartMatchingOnMenu) {
                    GameManager.instance.globalVariables.shouldStartMatchingOnMenu = false;
                    OnTapButtonGame66();
                    return;
                }

                // 勝ち切れ乱将棋の対局から戻ってきたときは、対局した月のカレンダーを開き直す
                var kachikireMonth = GameManager.instance.globalVariables.kachikireMonthToOpen;
                if (kachikireMonth != null) {
                    GameManager.instance.globalVariables.kachikireMonthToOpen = null;
                    LoadKachikirePositions(month: kachikireMonth);
                }
            }, null);
        }, (errorMessage) => {
            Debug.Log(errorMessage);
            Debug.Log("--- failed to get lounch info ---");
            Destroy(loadingView.gameObject);
            if(errorMessage == "ユーザが存在しません。") {
                Auth.instance.Signout();
            }
        });
    }


    public void OnTapButtonGame66()
    {
        GameManager.instance.globalVariables.boardType = EBoardType.BOARD_66;
        CheckNGames();
       //adManager.Show();
    }


    // ===============================================
    // MARK: - ● 勝ち切れ乱将棋 ─────

    // 表示中の月の局面 (日付 → 局面)
    private readonly Dictionary<DateTime, KachikirePosition> _kachikirePositions = new Dictionary<DateTime, KachikirePosition>();

    public void OnTapButtonKachikire()
    {
        // 月を指定しないとサーバ(日本時間)の今月になる
        LoadKachikirePositions(month: null);
    }


    public void OnTapKachikireChangeMonth(int year, int month)
    {
        LoadKachikirePositions(month: year.ToString("D4") + "-" + month.ToString("D2"));
    }


    private void LoadKachikirePositions(string month)
    {
        loadingView = Instantiate(loadingViewPrefab, canvas.transform, false);
        loadingView.Init("サーバと通信中...", false, () => { });

        var request = new KachikirePositionsRequest {
            _requestQueryString = month == null ? "" : "month=" + month,
        };
        Api.GetKachikirePositions(request, (response) => {
            Destroy(loadingView.gameObject);

            var culture = System.Globalization.CultureInfo.InvariantCulture;
            var today = DateTime.ParseExact(response.today, "yyyy-MM-dd", culture);
            var shownMonth = month == null ? new DateTime(today.Year, today.Month, 1) : DateTime.ParseExact(month, "yyyy-MM", culture);

            _kachikirePositions.Clear();
            var days = new List<FKachikireDay>();
            foreach (var position in response.positions) {
                var date = DateTime.ParseExact(position.date, "yyyy-MM-dd", culture);
                _kachikirePositions[date] = position;
                days.Add(new FKachikireDay(date: date, isCleared: position.cleared));
            }

            // startDate を返さない古いサーバでは制限しない
            var startDate = string.IsNullOrEmpty(response.startDate) ? DateTime.MinValue : DateTime.ParseExact(response.startDate, "yyyy-MM-dd", culture);

            _menuStage.ShowWindowKachikireCalendar(year: shownMonth.Year, month: shownMonth.Month, today: today, startDate: startDate, days: days);
        }, (errorMessage) => {
            Destroy(loadingView.gameObject);
            AlertViewController.ShowOKConfirmation(errorMessage);
        });
    }


    public void OnTapKachikireStart(DateTime date)
    {
        if (!_kachikirePositions.TryGetValue(date.Date, out var position)) return;

        var playerColor = position.playerColor == "w" ? Color.White : Color.Black;

        // 対局回数は消費しない。レートも動かない(AI対戦のため)
        GameManager.instance.globalVariables.aiGame = new AiGameSetting {
            rootSfen      = UsiConverter.FromEngineSfen(position.sfen, playerColor),
            playerColor   = playerColor,
            level         = RanshogiEngine.Level.Max,
            PlayerName    = Auth.instance.GetUser().nickname,
            AiName        = "AI",
            kachikireDate = position.date,
            kachikirePv   = position.pv,
        };
        GameManager.instance.globalVariables.boardType = EBoardType.BOARD_66;
        GameManager.instance.globalVariables.gameForReconnection = null;

        GoToGame();
    }

    public void CheckNGames() {
        if(Auth.instance.GetUser().isPayedUser || Auth.instance.GetUser().IsUnlimited()) {
            GoToGame();
            return;
        }
        if(Auth.instance.GetUser().nGamesLeftWithReward() == 0) {
            SubscriptionView view = FindObjectOfType<SubscriptionView>();
            view.SetOnUserEarnedReward(onUserEarnedReward);
            view.ShowAdOrBuySubscriptionConfirmation();
        } else {
            GoToGame();
        }
    }


    public void OnTapButtonGameWithFriend()
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "Friend", nextStageType: EStageType.FRIEND_GAME_MENU);
    }


    // 「対局履歴」ボタン。対局履歴の画面へ直接移る(対局履歴の「戻る」でメニューに戻る)
    public void OnTapButtonHistory()
    {
        GameManager.instance.globalVariables.didComeFromMypageToHistory = false;
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "History", nextStageType: EStageType.HISTORY);
    }


    // 「成績」ボタン。ランキングを廃止した代わりに、これまでのマイページ(成績と対局履歴)を開く
    public void OnTapButtonRecord()
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "MyPage", nextStageType: EStageType.BLANK_PAGE);
    }


    public void OnTapButtonSetting()
    {
        _menuStage.HideWithAnimationAndChangeScene(nextSceneName: "Settings", nextStageType: EStageType.BLANK_PAGE);
    }


    public void OnTapButtonHelp()
    {
        Application.OpenURL("https://ranshogi-production.web.app/help.html");
        //GameManager.instance.globalVariables.webViewURL = "https://ranshogi-production.web.app/help.html";
        //_menuStage.HideWithAnimationAndChangeScene(nextSceneName: "WebView", nextStageType: EStageType.BLANK_PAGE);
    }
}

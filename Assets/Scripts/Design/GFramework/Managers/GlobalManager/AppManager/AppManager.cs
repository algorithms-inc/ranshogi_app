using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


namespace GFramework.Core
{
    public enum EStageType
    {
        GAME,
        MENU,
        FRIEND_GAME_MENU,
        HISTORY,
        HISTORY_DETAIL,
        BLANK_PAGE,
        GENERAL_BUTTON_LIST,
        NULL
    }



    public sealed class AppManager : MonoBehaviour, IAppManager
    {
        // ===============================================
        // MARK: - ● Singleton ─────

        // Singleton Instance
        private static AppManager _appManagerInstance;

        // Get Instance
        public static IAppManager Instance
        {
            get
            {
                if (_appManagerInstance == null)
                {
                    var gameObjectAppManager = new GameObject(name: "AppManager");
                    _appManagerInstance = gameObjectAppManager.AddComponent<AppManager>();

                    Utl.SetDontDestroyOnLoad(targetObject: gameObjectAppManager);
                }

                return _appManagerInstance;
            }
        }



        // ===============================================
        // MARK: - ● Property ─────

        private EStageType _previousStageType = EStageType.NULL;
        public EStageType PreviousStageType => _previousStageType;

        private EStageType _presentStageType = EStageType.NULL;
        public EStageType PresentStageType => _presentStageType;

        private bool _isDebugMode = false;
        public bool IsDebugMode => _isDebugMode;

        public IUICreator UICreator => Locator.Instance.UICreatorManager;
        public IAudioController AudioController => Locator.Instance.audioManager;
        public IDesignController DesignController => Locator.Instance.designManager;

        private bool _isSetupDone = false;

        private GameStageBehaviour _gameStageBehaviour = null;
        private BlankPageStageBehaviour _friendGameMenuStageBehaviour = null;
        private BlankPageStageBehaviour _blankPageStageBehaviour = null;
        private MenuStageBehaviour _menuStageBehaviour = null;
        private HistoryStageBehaviour _historyStageBehaviour = null;
        private HistoryDetailStageBehaviour _historyDetailStageBehaviour = null;
        private GeneralButtonListStageBehaviour _generalButtonListStageBehaviour = null;



        // ===============================================
        // MARK: - ● Constructor ─────

        // Constructor
        private AppManager()
        {
            // Do nothing
        }



        // ===============================================
        // MARK: - ● Awake ─────

        public void Awake()
        {
            // VSync を無視する設定
            // - Unity のルール上、Constructor では設定ができないためここで設定
            QualitySettings.vSyncCount = 0;
            // フレーム数を 30frame/sec に固定
            Application.targetFrameRate = 30;
            // マルチタップを無効にする
            Input.multiTouchEnabled = false;
        }



        // ===============================================
        // MARK: - ● Create Stage ─────

        public IGameStage CreateGameStage(IGameManager gameManager, EBoardType boardType, bool isWithFriendMode, bool isNewGame)
        {
            CheckIsPreviousStageAlive();
            UpdatePresentStageType(newStageType: EStageType.GAME);
            ShowStatusBarOfPlayer1IfNeeded();


            var gameMode = isWithFriendMode ? EGameMode.WITH_FRIEND : EGameMode.NORMAL;
            var gameStartMode = isNewGame ? EGameStartMode.NEW_GAME : EGameStartMode.RESUME;

            var gameObject = new GameObject("GameStage");
            _gameStageBehaviour = gameObject.AddComponent<GameStageBehaviour>();
            _gameStageBehaviour.SetupComponent(gameManager: gameManager, gameMode: gameMode, boardType: boardType, gameStartMode: gameStartMode);

            return _gameStageBehaviour;
        }


        private void UpdatePresentStageType(EStageType newStageType)
        {
            _previousStageType = _presentStageType;
            _presentStageType = newStageType;
        }


        private void CheckIsPreviousStageAlive()
        {
            var result = false;
            
            if (_gameStageBehaviour != null) { result = true; Debug.Log("GameStageBehaviour is alive"); }
            if (_friendGameMenuStageBehaviour != null) { result = true; Debug.Log("FriendGameMenuStageBehaviour is alive"); }
            if (_menuStageBehaviour != null) { result = true; Debug.Log("MenuStageBehaviour is alive"); }
            if (_historyStageBehaviour != null) { result = true; Debug.Log("HistoryStageBehaviour is alive"); }
            if (_historyDetailStageBehaviour != null) { result = true; Debug.Log("HistoryDetailStageBehaviour is alive"); }
            if (_blankPageStageBehaviour != null) { result = true; Debug.Log("BlankPageStageBehaviour is alive"); }
            if (_generalButtonListStageBehaviour != null) { result = true; Debug.Log("GeneralButtonListStageBehaviour is alive"); }

            if (result) { 
                Debug.Log("ひとつ前のステージの Destroy が呼ばれていません");
            }
        }


        public IBlankPageStage CreateBlankPageStage(IBlankPageManager blankPageManager)
        {
            CheckIsPreviousStageAlive();
            UpdatePresentStageType(newStageType: EStageType.BLANK_PAGE);
            ShowStatusBarOfPlayer1IfNeeded();

            var gameObject = new GameObject("BlankPageStage");
            _blankPageStageBehaviour = gameObject.AddComponent<BlankPageStageBehaviour>();
            _blankPageStageBehaviour.SetupComponent(blankPageManager: blankPageManager);

            return _blankPageStageBehaviour;
        }


        public IMenuStage CreateMenuStage(IMenuManager menuManager)
        {
            CheckIsPreviousStageAlive();
            UpdatePresentStageType(newStageType: EStageType.MENU);
            ShowStatusBarOfPlayer1IfNeeded();

            var gameObject = new GameObject("MenuStage");
            _menuStageBehaviour = gameObject.AddComponent<MenuStageBehaviour>();
            _menuStageBehaviour.SetupComponent(menuManager: menuManager);

            return _menuStageBehaviour;
        }


        public IHistoryStage CreateHistoryStage(IHistoryManager historyManager)
        {
            CheckIsPreviousStageAlive();
            UpdatePresentStageType(newStageType: EStageType.HISTORY);
            ShowStatusBarOfPlayer1IfNeeded();

            var gameObject = new GameObject("HistoryStage");
            _historyStageBehaviour = gameObject.AddComponent<HistoryStageBehaviour>();
            _historyStageBehaviour.SetupComponent(historyManager: historyManager);

            return _historyStageBehaviour;
        }


        public IHistoryDetailStage CreateHistoryDetailStage(IHistoryDetailManager historyDetailManager, EBoardType boardType)
        {
            CheckIsPreviousStageAlive();
            UpdatePresentStageType(newStageType: EStageType.HISTORY_DETAIL);
            ShowStatusBarOfPlayer1IfNeeded();

            var gameObjectHistoryStage = new GameObject("HistoryDetailStage");
            _historyDetailStageBehaviour = gameObjectHistoryStage.AddComponent<HistoryDetailStageBehaviour>();
            _historyDetailStageBehaviour.SetupComponent(boardType: boardType, historyDetailManager: historyDetailManager);

            return _historyDetailStageBehaviour;
        }


        public IGeneralButtonListStage CreateGeneralButtonListStage(IGeneralButtonListManager generalButtonListManager, string title, List<string> buttonTextList)
        {
            CheckIsPreviousStageAlive();
            UpdatePresentStageType(newStageType: EStageType.GENERAL_BUTTON_LIST);
            ShowStatusBarOfPlayer1IfNeeded();

            var gameObjectGeneralButtonListStage = new GameObject("GeneralButtonListStage");
            _generalButtonListStageBehaviour = gameObjectGeneralButtonListStage.AddComponent<GeneralButtonListStageBehaviour>();
            _generalButtonListStageBehaviour.SetupComponent(generalButtonListManager: generalButtonListManager, title: title, buttonTextList: buttonTextList);

            return _generalButtonListStageBehaviour;
        }



        // ===============================================
        // MARK: - ● Destroy Stage ─────

        public void DestroyStageAndLoadScene(string nextSceneName)
        {
            switch (_presentStageType)
            {
                case EStageType.GAME:
                    DestroyGameStage();
                    break;

                case EStageType.MENU:
                    DestroyMenuStage();
                    break;

                case EStageType.FRIEND_GAME_MENU:
                    DestroyFriendGameMenuStage();
                    break;

                case EStageType.HISTORY:
                    DestroyHistoryStage();
                    break;

                case EStageType.HISTORY_DETAIL:
                    DestroyHistoryDetailStage();
                    break;

                case EStageType.BLANK_PAGE:
                    DestroyBlankPageStage();
                    break;

                case EStageType.GENERAL_BUTTON_LIST:
                    DestroyGeneralButtonListStage();
                    break;

                default:
                    Assert.IsTrue(false, "未実装のステージタイプです");
                    break;
            }


            SceneManager.LoadScene(nextSceneName);
        }


        private void DestroyGameStage()
        {
            Utl.SafeDestroy(behaviour: _gameStageBehaviour);
            _gameStageBehaviour = null;
        }


        private void DestroyBlankPageStage()
        {
            Utl.SafeDestroy(behaviour: _blankPageStageBehaviour);

            _blankPageStageBehaviour = null;
        }


        private void DestroyMenuStage()
        {
            Utl.SafeDestroy(behaviour: _menuStageBehaviour);

            _menuStageBehaviour = null;
        }


        private void DestroyFriendGameMenuStage()
        {
            Utl.SafeDestroy(behaviour: _friendGameMenuStageBehaviour);

            _friendGameMenuStageBehaviour = null;
        }


        private void DestroyHistoryStage()
        {
            Utl.SafeDestroy(behaviour: _historyStageBehaviour);

            _historyStageBehaviour = null;
        }


        private void DestroyHistoryDetailStage()
        {
            Utl.SafeDestroy(behaviour: _historyDetailStageBehaviour);

            _historyDetailStageBehaviour = null;
        }


        private void DestroyGeneralButtonListStage()
        {
            Utl.SafeDestroy(behaviour: _generalButtonListStageBehaviour);

            _generalButtonListStageBehaviour = null;
        }



        public void Destroy()
        {
            // Do Nothing
        }



        // ===============================================
        // MARK: - ● Setup ─────

        public void Setup()
        {
            // Scene 読み込みごとに行う処理
            Utl.UpdateMainCameraSize();

            if (_isSetupDone == true) { return; }

            // 一度だけ行うべき処理

            if (_isDebugMode == true)
            {
                Debug.Log("Debug Mode で実行しています");
            }

            Locator.Instance.SetAppManager(appManager: this);

            SetupEventSystem();
            SetupColorManager();
            SetupFontManager();
            SetupTextureManager();
            SetupAudioManager();
            SetupDesignManager();
            SetupLayoutManager();
            SetupBackgroundManager();
            SetupUICreatorManager();
            SetupStatusBarManager();
            SetupDebugManager();

            SetupCamera();


            _isSetupDone = true;
        }


        // EventSystem のセットアップ
        // - EventSystemはUnity EditorでCanvasesを作成すると自動で作成されるが、それはDontDestroyOnLoad になっていない。
        // - プログラム開発側で Canvas を作成することがあり、勝手に EventSystem が生成されたことで問題が生じるため、DontDestroyOnLoadを設定しておく
        // - なお、Unity上で、EventSystem が生成された場合と異なり、Script だと BaseInput も生成される。消し方がわからないため、そのままに。
        private void SetupEventSystem()
        {
            var gameObjectEventSystem = GameObject.Find("EventSystem") ?? new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
          
            Utl.SetDontDestroyOnLoad(targetObject: gameObjectEventSystem);
        }


        // カラーマネジャのセットアップ
        private void SetupColorManager()
        {
            Locator.Instance.colorManager = new ColorManager();
        }


        // フォントマネジャのセットアップ
        private void SetupFontManager()
        {
            Locator.Instance.fontManager = new FontManager();
        }


        // テクスチャマネジャのセットアップ
        private void SetupTextureManager()
        {
            Locator.Instance.textureManager = new TextureManager();
        }


        // オーディオマネジャのセットアップ
        private void SetupAudioManager()
        {
            Locator.Instance.audioManager = new AudioManager();
        }


        // デザインマネジャのセットアップ
        private void SetupDesignManager()
        {
            Locator.Instance.designManager = new DesignManager();
        }


        // レイアウトマネジャのセットアップ
        private void SetupLayoutManager()
        {
            Locator.Instance.layoutManager = new LayoutManager();
        }


        // バックグラウンドマネジャのセットアップ
        private void SetupBackgroundManager()
        {
            Locator.Instance.backgroundManager = new BackgroundManager();
        }


        // ウィンドウマネジャのセットアップ
        private void SetupUICreatorManager()
        {
            Locator.Instance.UICreatorManager = new UICreatorManager();

            Locator.Instance.UICreatorManager.Setup();
        }


        // ステータスバーマネジャのセットアップ
        private void SetupStatusBarManager()
        {
            Locator.Instance.statusBarManager = new StatusBarManager();

            Locator.Instance.statusBarManager.Setup();
        }


        // デバッグマネジャのセットアップ
        private void SetupDebugManager()
        {
            if (_isDebugMode == false) { return; }

            Locator.Instance.debugManager = new DebugManager();
        }


        private void SetupCamera()
        {
            Utl.SetComponentPhysics2DRaycasterToMainCamera();
        }



        // ===============================================
        // MARK: - ● Status Bar ─────

        // Unityエディタで、タイトル scene 以外から始めることがあるため、必要に応じてステータスバーを表示する
        private void ShowStatusBarOfPlayer1IfNeeded()
        {
            Locator.Instance.statusBarManager.Show(playerType: EPlayerType.PLAYER1);
        }


        // ステータスバーを表示する
        public void ShowStatusBar()
        {
            Locator.Instance.statusBarManager.Show(playerType: EPlayerType.PLAYER1);
        }


        // ステータスバーを非表示にする
        public void HideStatusBar()
        {
            Locator.Instance.statusBarManager.Hide(playerType: EPlayerType.PLAYER1);
        }


        // 名前を更新
        public void SetPlayerName(string playerName)
        {
            Locator.Instance.statusBarManager.SetPlayerNameForPlayer1(playerName: playerName);
        }


        // プレミアムユーザかどうかを設定
        public void SetIsPremiumUser(bool value)
        {
            Locator.Instance.statusBarManager.SetStatusBarUserType(userType: value ? EStatusBarUserType.PREMIUM : EStatusBarUserType.NORMAL);
        }


        // 残りライフを更新
        public void SetLife(int count)
        {
            Locator.Instance.statusBarManager.SetLife(count: count);
        }


        // レートを更新
        public void SetRate(int rate)
        {
            Locator.Instance.statusBarManager.SetRate(rate: rate);
        }


        public bool IsNameOnStatusBarShownForPlayer1 => 
            Locator.Instance.statusBarManager.IsNameShown(playerType: EPlayerType.PLAYER1);

        public void HideNameOnStatusBarForPlayer1()
        {
            Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER1);
        }
        public void ShowNameOnStatusBarForPlayer1()
        {
            Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER1);
        }

        public bool IsNameOnStatusBarShownForPlayer2 => 
            Locator.Instance.statusBarManager.IsNameShown(playerType: EPlayerType.PLAYER2);

        public void HideNameOnStatusBarForPlayer2()
        {
            Locator.Instance.statusBarManager.HideName(playerType: EPlayerType.PLAYER2);
        }

        public void ShowNameOnStatusBarForPlayer2()
        {
            Locator.Instance.statusBarManager.ShowName(playerType: EPlayerType.PLAYER2);
        }
    }
}
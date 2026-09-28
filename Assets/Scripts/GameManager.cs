using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GFramework.Core;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public struct GlobalVariables {
        public Game gameForHistoryDetail;
        public Game gameForReconnection;
        public EBoardType boardType;

        public bool didComeFromGameScene;

        // 対局履歴をマイページ(成績)から開いたか。対局履歴の「戻る」で、true ならマイページ、false ならメニューへ戻る
        public bool didComeFromMypageToHistory;

        public string roomCode;
        public string webViewURL;

        public string alertMessageOnMenu;

        // AI対戦の設定。検討画面から対局画面へ渡す。
        // null以外なら、対局画面はサーバーに接続せず端末内のエンジンと対戦する。
        public AiGameSetting aiGame;

        // 検討画面からAI対戦した後に検討画面へ戻ったとき、表示していた手数に戻すための値。
        public int historyDetailPly;

        // 同じく、表示していた手順(駒を動かしたり読み筋を再生したりしてできた変化)。本譜のままなら null。
        public System.Collections.Generic.List<Move> historyDetailMoves;

        // 勝ち切れ乱将棋の対局後にメニューへ戻ったとき、カレンダーを開く月(yyyy-MM)。nullなら開かない。
        public string kachikireMonthToOpen;

        // 検討画面を開いたときに、初期局面のAI評価値(読み筋)を出しておくか。
        // 勝ち切れ乱将棋の結果画面の「正解手順」から開いたときに立てる。
        public bool showAiEvaluationOnOpen;

        // メニューを開いたら、そのまま対局(マッチング)を始めるか。
        // 「対局待機中のユーザがいます」の通知の「対局する」を、メニュー以外の画面で押したときに立てる。
        public bool shouldStartMatchingOnMenu;
    }

    public GlobalVariables globalVariables = new GlobalVariables();

    private void Start()
    {
        Debug.Log("GameManager.Awake");
        Unity.Collections.NativeLeakDetection.Mode = Unity.Collections.NativeLeakDetectionMode.EnabledWithStackTrace;
        if (instance == null)
        {
            instance = this;
            MobileAds.Initialize(initStatus => { });
            BetterStreamingAssets.Initialize();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

}

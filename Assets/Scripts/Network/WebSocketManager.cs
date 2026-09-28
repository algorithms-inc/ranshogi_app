using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using WebSocketSharp.Net;
using System.Threading;
using System;
using GFramework.Core;

public class WebSocketManager 
{
    WebSocket ws;

    public delegate void GameStart(string sfen);
    public delegate void OnReceiverMessage(byte[] rawData, string data);
    public delegate void MatchingFinished(Game game);
    public delegate void Void();
    public delegate void OnClose(string errorMessage);

    private OnReceiverMessage onReceiveMessageHandler;
    private MatchingFinished matchingFinishedHandler;
    private Void onConnectHandler;
    private OnClose onCloseHandler;

    public void Init(
        OnReceiverMessage onReceiverMessage, 
        MatchingFinished matchingFinished, 
        Void onConnect,
        OnClose onClose)
    {
        onReceiveMessageHandler = onReceiverMessage;
        matchingFinishedHandler = matchingFinished;
        onConnectHandler = onConnect;
        onCloseHandler = onClose;
    }

    async public void ConnectMatching(EBoardType boardType, string roomCode = null, bool isRetry = false)
    {
        //ws = new WebSocket("ws://35.194.125.106:5001/"); 
        var context = SynchronizationContext.Current;
        Debug.Log(Constant.MatchingHost(boardType));
        ws = new WebSocket(Constant.MatchingHost(boardType));
        // この接続。マッチング成立後に対局サーバへの接続に張り替えるので、古い接続のイベントは無視する
        var matchingWs = ws;
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        if (roomCode != null) {
            ws.SetCookie(new Cookie("roomCode", roomCode));
        }
        ws.SetCookie(new Cookie("idToken", Auth.instance.idToken()));
        ws.SetCookie(new Cookie("AppVersion", Application.version));
        ws.SetCookie(new Cookie("DeviceName", UnityWebRequest.EscapeURL(SystemInfo.deviceName)));

        // 接続時に呼ばれる
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("open");
        };

        // サーバからのデータ受信時に呼ばれる
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log(e.Data);
            try {
                Game game = JsonUtility.FromJson<Game>(e.Data);
                context.Post((state) =>
               {
                    matchingFinishedHandler(game);
                }, null);
            } catch {
            }

        };

        // クローズ時に呼ばれる
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("close");
            Debug.Log(e.ToString());
            Debug.Log($"Close Code: {e.Code}, Reason: {e.Reason}");

            // マッチング成立後に自分で閉じた接続(すでに対局サーバへの接続に張り替え済み)の切断は、
            // 対局の切断として扱わない。扱うと再接続やマッチングのやり直しが走り、対局サーバに二重に接続してしまう
            if (ws != matchingWs) return;

            // 特定のエラーコード（1002 or 1006）かつ リトライ中でない場合
            if ((e.Code == 1002 || e.Code == 1006) && !isRetry) 
            {
                // 同期コンテキストを使って、トークンリフレッシュ処理を起動
                context.Post(_ => {
                    StartTokenRefreshAndReconnect(boardType, roomCode);
                }, null);
            } 
            else 
            {
                context.Post(_ => {
                    onCloseHandler(e.ToString());
                }, null);
            }
        };

        // エラー時に呼ばれる
        ws.OnError += (sender, e) =>
        {
            Debug.Log("on Error");
            Debug.Log(e.Message);
        };

        Debug.Log("try to connect");
        // 接続
        ws.Connect();
    }

    private async void StartTokenRefreshAndReconnect(EBoardType boardType, string roomCode)
    {
        Debug.Log("Attempting to refresh token...");
        
        try {
            bool isRefreshed = await Auth.instance.RefreshToken();
            
            if (isRefreshed) {
                Debug.Log("Token refreshed. Reconnecting...");
                ConnectMatching(boardType, roomCode, true);
            } else {
                Debug.LogError("Refresh failed. Back to Login.");
                onCloseHandler("Session Expired");
            }
        } catch (System.Exception ex) {
            Debug.LogError($"Error during refresh: {ex.Message}");
            onCloseHandler("Network Error");
        }
    }


    public void ConnectGame(Game game)
    {
        ws = new WebSocket("wss://" + game.serverDomain + "/rooms/"+game.boardType+"/"+game.roomId);
        // この接続。再接続で張り替えたあとに古い接続から届く切断は無視する
        var gameWs = ws;
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        ws.SetCookie(new Cookie("idToken", Auth.instance.idToken()));
        var context = SynchronizationContext.Current;

        // 接続時に呼ばれる
        ws.OnOpen += (sender, e) =>
        {
            context.Post((state) => {
                onConnectHandler();
            }, null);
        };

        // サーバからのデータ受信時に呼ばれる
        ws.OnMessage += (sender, e) =>
        {
            context.Post((state) =>
            {
                onReceiveMessageHandler(e.RawData, e.Data);
            }, null);
        };

        // クローズ時に呼ばれる
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("close");
            Debug.Log(e.ToString());
            // 再接続で新しい接続に張り替え済みなら、古い接続の切断で「切断された」扱いにしない
            if (ws != gameWs) return;
            context.Post((state) => {
                onCloseHandler(e.ToString());
            }, null);
        };

        // エラー時に呼ばれる
        ws.OnError += (sender, e) =>
        {
            Debug.Log(e.Message);
        };

        // 接続
        ws.Connect();
    }

    public void SendData(byte[] data)
    {
        ws.Send(data);
    }

    public void SendString(string str)
    {
        ws.Send(str);
    }

    public void Close() {
        ws.Close();
    }

}

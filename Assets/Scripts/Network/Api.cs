using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class Api : MonoBehaviour
{

    async public static void SendRequest<T, S>(S requestObject, UnityAction<T> OnSuccessHandler, UnityAction<string> OnErrorHandler) where S : RequestObject 
    {
        // 1. リクエストの生成
        UnityWebRequest request = CreateRequest(requestObject);

        try {
            await request.SendWebRequest();

            // 2. 認証エラー(401)のチェック
            if (request.responseCode == 401 && requestObject.authorizedRequest()) 
            {
                Debug.Log("Token expired. Refreshing token...");
                
                // トークン更新処理（Authクラスに更新メソッドがある想定）
                bool isRefreshed = await Auth.instance.RefreshToken();

                if (isRefreshed) 
                {
                    request.Dispose();
                    // 3. 新しいトークンで再試行（再帰呼び出し）
                    SendRequest(requestObject, OnSuccessHandler, OnErrorHandler);
                    return;
                } 
                else 
                {
                    OnErrorHandler("セッションが切れました。再度ログインしてください。");
                    request.Dispose();
                    return;
                }
            }

            // 4. 通常のエラーハンドリング
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                HandleErrorResponse(request, OnErrorHandler);
            } 
            else 
            {
                T responseObject = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                OnSuccessHandler(responseObject);
            }
        } 
        catch (Exception e) 
        {
            Debug.LogError(e);
            OnErrorHandler("リクエスト中に例外が発生しました。");
        } 
        finally 
        {
            request.Dispose();
        }
    }

    // リクエスト生成の共通化
    private static UnityWebRequest CreateRequest<S>(S requestObject) where S : RequestObject 
    {
        UnityWebRequest request;
        string baseUrl = Constant.APIHost() + requestObject.path();

        switch(requestObject.httpMethod()) {
            case HTTPMethod.POST: 
                request = UnityWebRequest.PostWwwForm(baseUrl, "POST");
                break;
            case HTTPMethod.PUT:
                request = UnityWebRequest.Put(baseUrl, System.Text.Encoding.UTF8.GetBytes(requestObject.requestJson()));
                break;
            default:
                var url = baseUrl + (requestObject.requestQueryString() != "" ? "?" + requestObject.requestQueryString() : "");
                request = UnityWebRequest.Get(url);
                break;
        }

        if (requestObject.httpMethod() != HTTPMethod.GET) {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(requestObject.requestJson()));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
        }

        if (requestObject.authorizedRequest()) {
            request.SetRequestHeader("Authorization", Auth.instance.idToken());
        }

        request.SetRequestHeader("AppVersion", Application.version);
        request.SetRequestHeader("DeviceName", UnityWebRequest.EscapeURL(SystemInfo.deviceName));
        request.timeout = 8;
        
        return request;
    }

    // エラー解析の共通化
    private static void HandleErrorResponse(UnityWebRequest request, UnityAction<string> OnErrorHandler) 
    {
        try {
            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(request.downloadHandler.text);
            OnErrorHandler(response.message);
        } catch {
            OnErrorHandler(request.error ?? "Unknown Error");
        }
    }

    public static void GetLaunchInfo(LaunchInfoRequest request, UnityAction<LaunchInfo> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void GetUser(UserRequest request, UnityAction<User> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void PostNickname(NicknameRequest request, UnityAction<CustomToken> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void UpdateEmail(UpdateEmailRequest request, UnityAction<EmptyResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void History(HistoryRequest request, UnityAction<HistoryResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void CustomToken(CustomTokenRequest request, UnityAction<CustomToken> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void UpdateNickname(UpdateNicknameRequest request, UnityAction<EmptyResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void DeleteUser(DeleteUserRequest request, UnityAction<EmptyResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void RoomCode(RoomCodeRequest request, UnityAction<RoomCode> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void CheckRoomCode(RoomCodeCheckRequest request, UnityAction<EmptyResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void RegisterAppleSubscription(AppleAppReceiptRequest request, UnityAction<ReceiptResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void RegisterGoogleSubscription(GoogleAppReceiptRequest request, UnityAction<ReceiptResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void GrantUnlimited(GrantUnlimitedRequest request, UnityAction<GrantUnlimitedResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void UpdateShowNameOnRanking(UpdateShowNameOnRankingRequest request, UnityAction<EmptyResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void GetKachikirePositions(KachikirePositionsRequest request, UnityAction<KachikirePositionsResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void RecordKachikireClear(KachikireClearRequest request, UnityAction<EmptyResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }

    public static void GetMatchingUsersCount(MatchingUsersCountRequest request, UnityAction<MatchingUsersCountResponse> OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        SendRequest(request, OnSuccessHandler, OnErrorHandler);
    }
}

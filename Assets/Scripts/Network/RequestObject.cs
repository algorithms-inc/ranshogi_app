using UnityEngine;

public enum HTTPMethod { 
    GET,
    POST,
    PUT
}

public interface RequestObject {
    HTTPMethod httpMethod();
    string path();
    bool authorizedRequest();
    string requestJson();
    string requestQueryString();
}

public struct EmptyStruct { }

public struct LaunchInfoRequest : RequestObject {
    public HTTPMethod httpMethod() { return HTTPMethod.GET; }
    public string path() { return "/launch_info"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return "{}"; } 
    public string requestQueryString() { return ""; } 
}

public struct UserRequest : RequestObject {
    public HTTPMethod httpMethod() { return HTTPMethod.GET; }
    public string path() { return "/users"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return "{}"; } 
    public string requestQueryString() { return ""; } 
}

public struct DeleteUserRequest : RequestObject {
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/delete"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return "{}"; } 
    public string requestQueryString() { return ""; } 
}

public struct NicknameRequest : RequestObject {
    public struct Nickname {
        public string nickname;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users"; }
    public bool authorizedRequest() { return false; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; }
}

// マッチングサーバ(乱将棋)の対局待機人数。認証不要。
public struct MatchingUsersCountRequest : RequestObject {
    public HTTPMethod httpMethod() { return HTTPMethod.GET; }
    public string path() { return "/matching/users/count"; }
    public bool authorizedRequest() { return false; }
    public string requestJson() { return "{}"; }
    public string requestQueryString() { return ""; }
}

public struct UpdateEmailRequest : RequestObject {
    public struct Email {
        public string email;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/email"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; } 
    public string requestQueryString() { return ""; } 
}

public struct SigninRequest : RequestObject {
    public struct Siginin {
        public string email;
        public string password;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/session"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; } 
    public string requestQueryString() { return ""; } 
}


public struct HistoryRequest : RequestObject
{
    public string _requestQueryString;
    public HTTPMethod httpMethod() { return HTTPMethod.GET; }
    public string path() { return "/games"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return "{}"; }
    public string requestQueryString() { return _requestQueryString; } 
}

public struct CustomTokenRequest : RequestObject
{
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.GET; }
    public string path() { return "/users/custom_token"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; } 
}

public struct UpdateNicknameRequest : RequestObject
{
    public struct Nickname
    {
        public string nickname;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/nickname"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; } 
}


public struct RoomCodeRequest : RequestObject
{
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/games/room_codes"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; } 
}

public struct RoomCodeCheckRequest : RequestObject
{
    public struct RoomCode
    {
        public string room_code;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/games/room_codes/check"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; } 
}

public struct AppleAppReceiptRequest : RequestObject
{
    public struct Receipt
    {
        public string receipt;
        public bool do_restore;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/subscription/app_receipt/apple"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; } 
}


public struct GoogleAppReceiptRequest : RequestObject
{
    public struct Receipt
    {
        public string receipt;
        public bool do_restore;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/subscription/app_receipt/google"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; } 
}



public struct GrantUnlimitedRequest : RequestObject
{
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/grant_unlimited"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; } 
}

public struct UpdateShowNameOnRankingRequest : RequestObject {
    public struct ShowNameOnRanking {
        public int showNameOnRanking;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return "/users/show_name_on_ranking"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; }
}

// 勝ち切れ乱将棋: 指定月の局面一覧。_requestQueryString は "month=YYYY-MM" (空なら今月)
public struct KachikirePositionsRequest : RequestObject
{
    public string _requestQueryString;
    public HTTPMethod httpMethod() { return HTTPMethod.GET; }
    public string path() { return Constant.KachikirePathPrefix() + "/positions"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return "{}"; }
    public string requestQueryString() { return _requestQueryString ?? ""; }
}

// 勝ち切れ乱将棋: 勝利の記録
public struct KachikireClearRequest : RequestObject
{
    public struct Clear
    {
        public string date;
    }
    public string _requestJson;
    public HTTPMethod httpMethod() { return HTTPMethod.POST; }
    public string path() { return Constant.KachikirePathPrefix() + "/clears"; }
    public bool authorizedRequest() { return true; }
    public string requestJson() { return _requestJson; }
    public string requestQueryString() { return ""; }
}

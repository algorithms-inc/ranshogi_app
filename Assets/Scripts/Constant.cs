using GFramework.Core;

public class Constant
{
    // 勝ち切れ乱将棋でサーバのモック (/kachikire/mock/...) を使うか。
    // サーバに局面が登録されたら false にする。Production では常に本物を使う。
    const bool KACHIKIRE_USE_MOCK = false;

    public static bool KachikireUseMock() {
        return KACHIKIRE_USE_MOCK && Settings.ENV != Settings.ENVIRONMENT.Production;
    }

    public static string KachikirePathPrefix() {
        return KachikireUseMock() ? "/kachikire/mock" : "/kachikire";
    }

    public static string APIHost() {
        switch(Settings.ENV) {
            case Settings.ENVIRONMENT.Development:
                
                return "https://staging.ranshogi.algorithms.fun";
                break;
            case Settings.ENVIRONMENT.Staging:
                return "https://us-central1-ranshogi-staging.cloudfunctions.net/api";
                break;
            case Settings.ENVIRONMENT.Production:
                return "https://api.ranshogi.algorithms.fun";
                break;
            default: 
                return "http://160.16.140.106:3761";
        }
    }

    public static string MatchingHost(EBoardType boardType) {
        if(boardType == EBoardType.BOARD_66) {
            switch(Settings.ENV) {
                case Settings.ENVIRONMENT.Development:
                    return "wss://staging.ranshogi.algorithms.fun/ws_matching/ranshogi66";
                    break;
                case Settings.ENVIRONMENT.Staging:
                    return "wss://staging.ranshogi.algorithms.fun";
                    break;
                case Settings.ENVIRONMENT.Production:
                    return "wss://ranshogi66.matching.ranshogi.algorithms.fun/ws_matching";
                    break;
                default: 
                    return "http://160.16.140.106:3761";
            }
        } else {
            switch(Settings.ENV) {
                case Settings.ENVIRONMENT.Development:
                    return "wss://staging.ranshogi.algorithms.fun/ws_matching/shogi";
                    break;
                case Settings.ENVIRONMENT.Staging:
                    return "wss://staging.ranshogi.algorithms.fun:5003";
                    break;
                case Settings.ENVIRONMENT.Production:
                    return "wss://shogi.matching.ranshogi.algorithms.fun/ws_matching/shogi";
                    break;
                default: 
                    return "http://160.16.140.106:3761";
            }
        }
    }

}

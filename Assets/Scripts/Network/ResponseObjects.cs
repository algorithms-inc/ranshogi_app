using UnityEngine;
using System;
using System.Collections.Generic;
using GFramework.Core;


[Serializable]
public class CustomToken {
    public string token;
}

[Serializable]
public class IdToken {
    public string token;
}

[Serializable]
public class EmptyResponse {
}

[Serializable]
public class ErrorResponse {
    public string message;
}


// 過去の遺物。将来的に削除する。
[Serializable]
public class Announcement  {
    public int id;
    public int announcement_type;
    public string description;
}

[Serializable]
public class LaunchInfo {
    public Announcement announcement;
    public User user;
}

[Serializable]
public class User {
    public string email;
    public string nickname;
    public string firebaseUid;
    public int ratingRanshogi66;
    public int nWinRanshogi66;
    public int nLoseRanshogi66;
    public int ratingShogi;
    public int nWinShogi;
    public int nLoseShogi;
    public int nGamesLeft;
    public bool isPayedUser;
    public bool showNameOnRanking;
    public long unlimitedUntil;
    public Game latestGame;

    public int nGamesLeftWithReward() {
        if(isPayedUser || IsUnlimited()) {
            return 3;
        } else {
            //return nGamesLeft + (PlayerPrefs.HasKey("NAdLife") ? PlayerPrefs.GetInt("NAdLife") : 0);
            return nGamesLeft;
        }
    }

    public void DecrementAdLife() {
        if(isUserRewarded()) {
            PlayerPrefs.SetInt("NAdLife", PlayerPrefs.GetInt("NAdLife") - 1);
        }
    }

    public void decrementLife() {
        if(isUserRewarded()) {
            DecrementAdLife();
        } else {
            if(nGamesLeft > 0) {
                nGamesLeft -= 1;
            }
        }
    }

    public bool isUserRewarded() {
        return PlayerPrefs.HasKey("NAdLife") && PlayerPrefs.GetInt("NAdLife") > 0;
    }

    public int RatingOf(EBoardType boardType) {
        if(boardType == EBoardType.BOARD_66) {
            return ratingRanshogi66;
        } else {
            return ratingShogi;
        }
    }

    public void incrementAdLife(int n) {
        PlayerPrefs.SetInt("NAdLife", PlayerPrefs.GetInt("NAdLife") + n);
    }
 
    public bool IsUnlimited()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return this.unlimitedUntil > 0 && now <= this.unlimitedUntil;
    }
}

[Serializable]
public class Game {
    public int id;
    public string serverDomain;
    public string roomId;
    public string sfen;
    public int? score;
    public string? pv;
    public GameState state;
    public string blackUid;
    public string whiteUid;
    public string blackNickname;
    public string whiteNickname;
    public int blackRating;
    public int whiteRating;
    public int blackRateTo;
    public int whiteRateTo;
    public int rateDiff;
    public int gameNum;
    public string base64EncodedMoves;
    public string base64EncodedConsumedSeconds;
    public GameFinishReason gameFinishReason;
    public GameResult gameResult;
    public DateTime gameStartedAt;
    public DateTime gameFinishedAt;
    public string boardType;

    private List<Move> _moves = null;
    private List<Move> _pvMoves = null;


    public List<Move> moves() {
        if(_moves != null) { return _moves; }
        if(this.base64EncodedMoves == null) { return new List<Move>(); }
        byte[] moveBytes = Convert.FromBase64String(this.base64EncodedMoves);
        var moves = new List<Move>();

        for (int i = 0; i < moveBytes.Length / 4; i++)
        {
            Move m = new Move();
            m.move = (Move32)(moveBytes[i * 4] + (moveBytes[i * 4 + 1] << 8) + (moveBytes[i * 4 + 2] << 16) + (moveBytes[i * 4 + 3] << 24));
            moves.Add(m);
        }
        _moves = moves;
        return _moves;
    }

    public void SetMoves(List<Move> moves) {
        _moves = moves;
    }

    public List<Move> pvMoves() {
        if(_pvMoves != null) { return _pvMoves; }
        if(this.pv == null) { return new List<Move>(); }

        // sfen形式の指し手には「動かす駒」が入っていない。
        // Position.DoMove()は玉を動かしたかどうかの判定にそれを使っている(kingSquareの更新)ので、
        // 初期局面から実際に並べ直しながら、盤上の駒で補完する。
        // 手番も、Blackから交互と決め打ちにせず盤面から取る。
        Position pos = BoardType() == EBoardType.BOARD_99 ? (Position)new Position99() : new Position66();
        pos.Set(this.sfen);

        var moves = new List<Move>();
        foreach(var sfenMove in this.pv.Split(' ')) {
            if(string.IsNullOrWhiteSpace(sfenMove)) { continue; }
            Move m = UsiConverter.FromUsi(sfenMove, pos);
            moves.Add(m);
            pos.DoMove(m);
        }
        _pvMoves = moves;
        return _pvMoves;
    }

    public EBoardType BoardType() {
        if(boardType == "shogi") {
            return EBoardType.BOARD_99;
        } else {
            return EBoardType.BOARD_66;
        }
    }

    public int RatingOf(Color color) {
        if(color == Color.Black) {
            if(blackRating == 0) {
                return blackRateTo + (gameResult == GameResult.BlackWon ? -rateDiff : rateDiff);
            } else {
                return blackRating;
            }
        } else {
            if(whiteRating == 0) {
                return whiteRateTo + (gameResult == GameResult.WhiteWon ? -rateDiff : rateDiff);
            } else {
                return whiteRating;
            }
        }
    }
    
}

[Serializable]
public class GrantUnlimitedResponse {
    public long unlimitedUntil;
}

[Serializable]
public class HistoryResponse
{
    public int nGames;
    public Game[] games;
}

[Serializable]
public class RoomCode
{
    public string room_code;
}

public enum ReceiptState
{
    ValidNewSubscription = 0,
    ValidRestorable = 1,
    Invalid = 2
}
 
[Serializable]
public class ReceiptResponse {
    public ReceiptState receiptState;
}

// 勝ち切れ乱将棋の局面
[Serializable]
public class KachikirePosition {
    // YYYY-MM-DD
    public string date;
    // USI形式のsfen
    public string sfen;
    // プレイヤーが持つ側 ("b": 先手, "w": 後手)
    public string playerColor;
    // 正解手順 (USI形式の指し手を空白区切り)。生成前の古い局面では null
    public string pv;
    // この局面に勝利済みか
    public bool cleared;
}

[Serializable]
public class KachikirePositionsResponse {
    // サーバ(日本時間)の今日 YYYY-MM-DD
    public string today;
    // 勝ち切れ乱将棋を始めた日 YYYY-MM-DD。これより前の日は選べない
    public string startDate;
    public KachikirePosition[] positions;
}

[Serializable]
public class MatchingUsersCountResponse {
    // 対局待機中の人数(AI・管理者のアカウントを含む)
    public int count;
    // そのうちAIのアカウントの人数。名前が admin_count なのは、配布済みのアプリに合わせたため
    public int admin_count;
    // そのうちAIのアカウントの人数(admin_count と同じ値。古いサーバは返さない → 0)
    public int ai_count;
}

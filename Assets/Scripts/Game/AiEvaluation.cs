using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 検討画面の「AI評価値」。エンジンの読み筋("info depth .. score cp 120 .. pv 3c3d 4d4c ..")を読み取り、
/// 吹き出しに出す文言と、再生用の指し手に変換する。
/// </summary>
public class AiEvaluation
{
    // 読みの条件。深さの上限と1局面あたりの思考時間の上限(ms)のうち、先に届いた方で打ち切る
    public const int SearchDepth = (int)RanshogiEngine.Level.Max;
    public const int SearchMovetimeMs = 5000;

    // 吹き出しに並べる読み筋の手数(再生はすべての手を使う)
    const int MaxPvMovesInText = 8;

    /// <summary>評価値を読み取れたか。</summary>
    public readonly bool HasScore;
    /// <summary>詰みの読みか。</summary>
    public readonly bool IsMate;
    /// <summary>手番側から見た評価値(歩=100)。IsMate のときは詰みまでの手数(正なら手番側が詰ます)。</summary>
    public readonly int Value;
    /// <summary>読み筋(USI形式の指し手)。</summary>
    public readonly List<string> PvUsi = new List<string>();


    public AiEvaluation(string pvInfo)
    {
        if (string.IsNullOrWhiteSpace(pvInfo)) return;

        string[] tokens = pvInfo.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i] == "score" && i + 2 < tokens.Length)
            {
                IsMate = tokens[i + 1] == "mate";
                // 手数の分からない詰み("mate +" / "mate -")は1手として扱う
                string value = tokens[i + 2];
                if (value == "+") { Value = 1; HasScore = true; }
                else if (value == "-") { Value = -1; HasScore = true; }
                else if (int.TryParse(value, out int n)) { Value = n; HasScore = true; }
                i += 2;
            }
            else if (tokens[i] == "pv")
            {
                for (int j = i + 1; j < tokens.Length; j++) PvUsi.Add(tokens[j]);
                break;
            }
        }
    }


    /// <summary>
    /// 吹き出しの文言(1行目に評価値、2行目に読み筋)と、読み筋の指し手を返す。
    /// </summary>
    /// <param name="position">評価した局面。読み筋を並べるので中身が変わる(使い捨ての局面を渡すこと)</param>
    /// <param name="previousTo">この局面の直前の手の移動先(「同」の表記用)。無ければ SQ_Invalid</param>
    /// <param name="headWithoutScore">評価値がないときの1行目。勝ち切れ乱将棋の正解手順のように、読み筋だけがある場合に使う</param>
    public (string text, List<Move> pvMoves) Describe(Position position, Square previousTo, string headWithoutScore = null)
    {
        Color sideToMove = position.sideToMove;

        string head;
        if (!HasScore)
        {
            head = headWithoutScore ?? "評価値 -";
        }
        else if (IsMate)
        {
            Color winner = Value > 0 ? sideToMove : G.Not(sideToMove);
            head = $"{ColorName(winner)}の勝ち（{Math.Abs(Value)}手詰）";
        }
        else
        {
            // 先手から見た値にそろえる
            int blackValue = sideToMove == Color.Black ? Value : -Value;
            string advantage = blackValue > 0 ? "先手有利" : blackValue < 0 ? "後手有利" : "互角";
            head = $"評価値 {(blackValue > 0 ? "+" : "")}{blackValue}（{advantage}）";
        }

        var pvMoves = new List<Move>();
        var names = new List<string>();
        Square lastTo = previousTo;
        foreach (var usi in PvUsi)
        {
            Move m;
            try
            {
                m = UsiConverter.FromUsi(usi, position);
            }
            catch (ArgumentException)
            {
                break;
            }
            if (names.Count < MaxPvMovesInText) names.Add(KifuNotation.ToJapanese(position, m, lastTo));
            pvMoves.Add(m);
            lastTo = m.to();
            position.DoMove(m);
        }

        if (names.Count == 0) return (head, pvMoves);
        return (head + "\n" + string.Join(" ", names) + (pvMoves.Count > names.Count ? " …" : ""), pvMoves);
    }


    public static string ColorName(Color color) => color == Color.Black ? "先手" : "後手";
}


/// <summary>指し手を「▲３三銀成」のような表記にする。</summary>
public static class KifuNotation
{
    static readonly string[] FileChars = { "１", "２", "３", "４", "５", "６", "７", "８", "９" };
    static readonly string[] RankChars = { "一", "二", "三", "四", "五", "六", "七", "八", "九" };

    /// <param name="position">指す前の局面</param>
    /// <param name="previousTo">直前の手の移動先。同じマスなら「同」と書く</param>
    public static string ToJapanese(Position position, Move m, Square previousTo)
    {
        string mark = position.sideToMove == Color.Black ? "▲" : "△";
        string to = m.to() == previousTo
            ? "同"
            : FileChars[position.FileOf(m.to())] + RankChars[position.RankOf(m.to())];

        string suffix = "";
        if (m.isDrop()) suffix = "打";
        else if (m.isPromote()) suffix = "成";
        else if (position.CanPromote(m.movedPiece(), m.from(), m.to(), position.sideToMove)) suffix = "不成";

        return mark + to + PieceName(G.TypeOf(m.movedPiece())) + suffix;
    }

    static string PieceName(PieceType pieceType)
    {
        switch (pieceType)
        {
            case PieceType.Pawn:       return "歩";
            case PieceType.Lance:      return "香";
            case PieceType.Knight:     return "桂";
            case PieceType.Silver:     return "銀";
            case PieceType.Bishop:     return "角";
            case PieceType.Rook:       return "飛";
            case PieceType.Gold:       return "金";
            case PieceType.King:       return "玉";
            case PieceType.Pro_Pawn:   return "と";
            case PieceType.Pro_Lance:  return "成香";
            case PieceType.Pro_Knight: return "成桂";
            case PieceType.Pro_Silver: return "成銀";
            case PieceType.Horse:      return "馬";
            case PieceType.Dragon:     return "龍";
            default:                   return "";
        }
    }
}


/// <summary>
/// 一度読んだ読み筋を端末に保存しておき、同じ局面ではエンジンを動かさずに出す。
/// 局面は開始局面と指し手の列(エンジンに渡す文字列)で見分ける。
/// </summary>
public static class AiEvaluationCache
{
    // 保存しておく局面の数。超えたら古いものから消す
    const int MaxEntries = 500;
    const string FileName = "ai_evaluation_cache.json";

    [Serializable]
    class Entry
    {
        public string key;
        public string pvInfo;
    }

    [Serializable]
    class Store
    {
        public List<Entry> entries = new List<Entry>();
    }

    static Store _store;

    static string FilePath => Path.Combine(Application.persistentDataPath, FileName);


    /// <summary>キャッシュのキー。読みの条件が変わったら別の局面として扱う。</summary>
    public static string Key(string positionArgs)
        => $"d{AiEvaluation.SearchDepth} t{AiEvaluation.SearchMovetimeMs} {positionArgs}";


    /// <summary>保存してある読み筋。無ければ null。</summary>
    public static string Get(string key)
    {
        foreach (var entry in Load().entries)
        {
            if (entry.key == key) return entry.pvInfo;
        }
        return null;
    }


    public static void Put(string key, string pvInfo)
    {
        var store = Load();
        store.entries.RemoveAll(entry => entry.key == key);
        store.entries.Add(new Entry { key = key, pvInfo = pvInfo ?? "" });
        if (store.entries.Count > MaxEntries)
        {
            store.entries.RemoveRange(0, store.entries.Count - MaxEntries);
        }

        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(store));
        }
        catch (Exception e)
        {
            // 保存できなくても表示はできるので、ログだけ残す
            Debug.LogWarning($"[AiEvaluationCache] 保存に失敗しました: {e.Message}");
        }
    }


    static Store Load()
    {
        if (_store != null) return _store;

        try
        {
            if (File.Exists(FilePath)) _store = JsonUtility.FromJson<Store>(File.ReadAllText(FilePath));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[AiEvaluationCache] 読み込みに失敗しました: {e.Message}");
        }

        if (_store == null || _store.entries == null) _store = new Store();
        return _store;
    }
}

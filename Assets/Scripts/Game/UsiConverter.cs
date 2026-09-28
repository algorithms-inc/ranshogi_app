using System;
using System.Collections.Generic;

/// <summary>
/// アプリ内部の表現と、エンジン(やねうら王)が使うUSI形式との変換。
///
/// 注意: このアプリのsfen(サーバーが返す game.sfen)は、
///       "sfen &lt;盤面&gt; &lt;持駒&gt; &lt;自分の手番色&gt; &lt;手番&gt;" という独自の並びで、
///       USIの "&lt;盤面&gt; &lt;手番&gt; &lt;持駒&gt; &lt;手数&gt;" とはフィールドの順番が違う。
///       Position.Set() / ParseSfen() の読み方に合わせてある。
/// </summary>
public static class UsiConverter
{
    const int FileNb = 6;   // 6x6専用(エンジンが6x6ビルドのため)
    const int RankNb = 6;

    /// <summary>アプリ形式のsfen → エンジンに渡すUSI形式のsfen。</summary>
    public static string ToEngineSfen(string appSfen)
    {
        if (string.IsNullOrWhiteSpace(appSfen))
            throw new ArgumentException("sfen is empty");

        // Position.ParseSfen()は cmds[1] を盤面、Position.Set()は
        // cmds[2] を持駒、cmds[4] を手番として読む。
        string[] cmds = appSfen.Split(' ');
        if (cmds.Length < 5)
            throw new ArgumentException($"unexpected sfen format: {appSfen}");

        string board = cmds[1];
        string hand  = cmds[2];
        string stm   = cmds[4];

        return $"{board} {stm} {hand} 1";
    }

    /// <summary>
    /// USI形式のsfen ("&lt;盤面&gt; &lt;手番&gt; &lt;持駒&gt; &lt;手数&gt;"、先頭の "sfen" は有っても無くてもよい) → アプリ形式のsfen。
    /// playerColor はプレイヤーが持つ側。
    /// </summary>
    public static string FromEngineSfen(string usiSfen, Color playerColor)
    {
        if (string.IsNullOrWhiteSpace(usiSfen))
            throw new ArgumentException("sfen is empty");

        string[] cmds = usiSfen.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        int start = cmds[0] == "sfen" ? 1 : 0;
        if (cmds.Length - start < 3)
            throw new ArgumentException($"unexpected usi sfen format: {usiSfen}");

        string board   = cmds[start];
        string stm     = cmds[start + 1];
        string hand    = cmds[start + 2];
        string myColor = playerColor == Color.White ? "w" : "b";

        return $"sfen {board} {hand} {myColor} {stm} 1";
    }

    /// <summary>Move → USI形式の指し手文字列 ("3c3d" / "3c3d+" / "P*3c")。</summary>
    public static string ToUsi(Move m)
    {
        if (m.isDrop())
        {
            char pieceChar = char.ToUpperInvariant(G.PieceToIndex[(int)m.movedPiece()]);
            return $"{pieceChar}*{SquareToUsi(m.to())}";
        }

        return SquareToUsi(m.from()) + SquareToUsi(m.to()) + (m.isPromote() ? "+" : "");
    }

    /// <summary>指し手の列をUSI形式に変換する。</summary>
    public static List<string> ToUsi(IEnumerable<Move> moves)
    {
        var list = new List<string>();
        foreach (var m in moves) list.Add(ToUsi(m));
        return list;
    }

    /// <summary>
    /// USI形式の指し手 → Move。
    /// SfenMoveParserは通常の指し手のとき movedPiece を埋めないので、
    /// 盤面から実際の駒を拾って補完する。
    /// (補完しないと Position.DoMove() が玉の移動を認識できず kingSquare がずれる)
    /// </summary>
    public static Move FromUsi(string usiMove, Position position)
    {
        Move m = SfenMoveParser.Parse(usiMove, position.sideToMove);
        if (m.isDrop()) return m;

        Piece pc = position.Get(m.from());
        return G.MakeMove(m.from(), m.to(), pc, m.isPromote(), false);
    }

    static string SquareToUsi(Square sq)
    {
        int index = (int)sq;
        char file = (char)('1' + index / FileNb);
        char rank = (char)('a' + index % RankNb);
        return $"{file}{rank}";
    }
}

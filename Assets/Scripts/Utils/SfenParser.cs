using System;

public static class SfenMoveParser
{
    private const int BoardSize = 6;
    private const Square NoSquare = Square.SQ_Invalid;

    /// <summary>
    /// SFEN move: "1a2b", "1a2b+", "P*3c"
    /// </summary>
    public static Move Parse(string sfenMove, Color color)
    {
        if (string.IsNullOrWhiteSpace(sfenMove))
            throw new ArgumentException("Move string is empty");

        // 成り判定（末尾 +）
        bool isPromotion = sfenMove[^1] == '+';
        string core = isPromotion ? sfenMove.Substring(0, sfenMove.Length - 1) : sfenMove;

        // 駒打ち
        int star = core.IndexOf('*');
        Square from = NoSquare;
        Square to = NoSquare;
        if (star >= 0)
        {
            // 形式: "P*3c"
            if (core.Length != 4 || star != 1)
                throw new ArgumentException($"Invalid drop move: {sfenMove}");

            if (isPromotion)
                throw new ArgumentException($"Invalid move: drop cannot promote: {sfenMove}");

            char piece = core[0];
            to = (Square)SquareToIndex(core[2], core[3]);
            Piece pc = (Piece)G.PieceToIndex.IndexOf(piece);
            if(G.ColorOf(pc) != color) {
                pc = G.Inverse(pc);
            }

            return G.MakeMove(from, to, pc, false, true);
            
        }

        // 通常手
        // 形式: "1a2b"
        if (core.Length != 4)
            throw new ArgumentException($"Invalid normal move: {sfenMove}");

        from = SquareToIndex(core[0], core[1]);
        to   = SquareToIndex(core[2], core[3]);

        return G.MakeMove(from, to, Piece.No_Piece, isPromotion, false);
    }

    /// <summary>
    /// 盤座標 (例: '1','a') → index
    /// index = (file-1)*6 + (rank-'a')
    /// </summary>
    private static Square SquareToIndex(char fileChar, char rankChar)
    {
        int file = fileChar - '1'; // '1' -> 0
        int rank = rankChar - 'a'; // 'a' -> 0

        if (file < 0 || file >= BoardSize || rank < 0 || rank >= BoardSize)
            throw new ArgumentOutOfRangeException($"Invalid square: {fileChar}{rankChar}");

        return (Square)(file * BoardSize + rank);
    }
}
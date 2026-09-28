using System;
using UnityEngine;
using GFramework.Core;

public enum GameType {
    Ranshogi66,
    Shogi,
}

public enum Color
{
    Black, White
}

public enum Square
{
    SQ_11, SQ_12, SQ_13, SQ_14, SQ_15, SQ_16, SQ_17, SQ_18, SQ_19,
    SQ_21, SQ_22, SQ_23, SQ_24, SQ_25, SQ_26, SQ_27, SQ_28, SQ_29,
    SQ_31, SQ_32, SQ_33, SQ_34, SQ_35, SQ_36, SQ_37, SQ_38, SQ_39,
    SQ_41, SQ_42, SQ_43, SQ_44, SQ_45, SQ_46, SQ_47, SQ_48, SQ_49,
    SQ_51, SQ_52, SQ_53, SQ_54, SQ_55, SQ_56, SQ_57, SQ_58, SQ_59,
    SQ_61, SQ_62, SQ_63, SQ_64, SQ_65, SQ_66, SQ_67, SQ_68, SQ_69,
    SQ_71, SQ_72, SQ_73, SQ_74, SQ_75, SQ_76, SQ_77, SQ_78, SQ_79,
    SQ_81, SQ_82, SQ_83, SQ_84, SQ_85, SQ_86, SQ_87, SQ_88, SQ_89,
    SQ_91, SQ_92, SQ_93, SQ_94, SQ_95, SQ_96, SQ_97, SQ_98, SQ_99,
    SQ_Invalid,
    SQ_NB
}

public enum PieceType
{
    No_Piece,
    Pawn,
    Lance,
    Knight,
    Silver,
    Bishop,
    Rook,
    Gold,
    King,
    Pro_Pawn,
    Pro_Lance,
    Pro_Knight,
    Pro_Silver,
    Horse,
    Dragon,
    AllPieces,
    PieceTypeNb
}

public enum Piece
{
    No_Piece,
    B_Pawn,
    B_Lance,
    B_Knight,
    B_Silver,
    B_Bishop,
    B_Rook,
    B_Gold,
    B_King,
    B_Pro_Pawn,
    B_Pro_Lance,
    B_Pro_Knight,
    B_Pro_Silver,
    B_Horse,
    B_Dragon,
    W_Pawn,
    W_Lance,
    W_Knight,
    W_Silver,
    W_Bishop,
    W_Rook,
    W_Gold,
    W_King,
    W_Pro_Pawn,
    W_Pro_Lance,
    W_Pro_Knight,
    W_Pro_Silver,
    W_Horse,
    W_Dragon
}

public enum Direction
{
    LU = +5, U = -1, RU = -7,
    L = +6, C, R = -6,
    LD = +7, D = +1, RD = -5
}

public enum GameState
{
    OnSearchingOpponent = 1,
    Preparing = 2,
    OnGame = 3,
    GameFinished = 5,
    GameFinishedWaitingServerResponse = 6,
    ConnectingGame = 7,
}

public enum GameFinishReason
{
    Mate = 0,
    PlayerResigned = 1,
    PlayerTimeout = 2,
    PlayerDisconnect = 3,
    Try = 4, // トライ（王が相手の陣地の奥まで来る）
    RepetitionOfMovesDraw = 10, // 千日手
    RepetitionOfMovesCheckFoul = 11, // 王手含みの千日手の反則
    LimitOfMoves = 12, // 200手以上
    ServerError = 100,
}

public enum GameResult
{
    BlackWon = 0,
    WhiteWon = 1,
    Draw = 2,
    InvalidGame = 3
}

public class G
{

    public static string PieceToIndex = " PLNSBRGK      plnsbrgk      ";

    public static Color ColorOf(Piece pc)
    {
        return (int)pc < 15 ? Color.Black : Color.White;
    }

    public static Color Not(Color c) {
        return c == Color.Black ? Color.White : Color.Black;
    }

    public static PieceType TypeOf(Piece pc)
    {
        switch (pc)
        {
            case Piece.No_Piece: return PieceType.No_Piece;
            case Piece.B_Pawn: case Piece.W_Pawn: return PieceType.Pawn;
            case Piece.B_Pro_Pawn: case Piece.W_Pro_Pawn: return PieceType.Pro_Pawn;
            case Piece.B_Lance: case Piece.W_Lance: return PieceType.Lance;
            case Piece.B_Pro_Lance: case Piece.W_Pro_Lance: return PieceType.Pro_Lance;
            case Piece.B_Knight: case Piece.W_Knight: return PieceType.Knight;
            case Piece.B_Pro_Knight: case Piece.W_Pro_Knight: return PieceType.Pro_Knight;
            case Piece.B_Silver: case Piece.W_Silver: return PieceType.Silver;
            case Piece.B_Pro_Silver: case Piece.W_Pro_Silver: return PieceType.Pro_Silver;
            case Piece.B_Gold: case Piece.W_Gold: return PieceType.Gold;
            case Piece.B_Bishop: case Piece.W_Bishop: return PieceType.Bishop;
            case Piece.B_Horse: case Piece.W_Horse: return PieceType.Horse;
            case Piece.B_Rook: case Piece.W_Rook: return PieceType.Rook;
            case Piece.B_Dragon: case Piece.W_Dragon: return PieceType.Dragon;
            case Piece.B_King: case Piece.W_King: return PieceType.King;
        }
        return PieceType.No_Piece;
    }

    public static int RankOf(Square sq) { return (int)sq % 6; }
    public static int FileOf(Square sq) { return (int)sq / 6; }

    public static Square Sq(int file, int rank) {
        return (Square)(file * 6 + rank);
    }

    public static Piece Inverse(Piece pc)
    {
        if (pc == Piece.No_Piece) return Piece.No_Piece;
        if (ColorOf(pc) == Color.Black) return (Piece)((int)pc + 14);
        else return (Piece)((int)pc - 14);
    }

    public static Piece RawOf(Piece pc)
    {
        int pn = (int)pc;
        if ((pn >= 9 && pn <= 14) || pn >= 23 && pn <= 28)
        {
            return (Piece)(pn - 8);
        }
        return pc;
    }

    public static int HandIndexOf(Piece pc)
    {
        if (ColorOf(pc) == Color.Black) return (int)RawOf(pc) - 1;
        else return (int)RawOf(pc) - 15;
    }

    public static int HandIndexOf(PieceType pt)
    {
        return (int)pt - 1;
    }


    public static Piece HandIndexToPiece(int index, Color c)
    {
        if (c == Color.Black) return (Piece)index + 1;
        else return (Piece) index + 15;
    }

    public static int SiblingIndexOf(Piece pc)
    {
        if (ColorOf(pc) == Color.Black) return (int)RawOf(pc) - 1;
        else return (int)RawOf(pc) - 15;
    }

    public static Piece Promoted(Piece pc)
    {
        return (Piece)((int)pc + 8);
    }

    public static Piece Colored(PieceType pt, Color c)
    {
        return c == Color.Black ? (Piece)pt : (Piece)((int)pt + 14);
    }

    public static bool CanPromote(Piece pc)
    {
        return (int)TypeOf(pc) >= 1 && (int)TypeOf(pc) <= 6;
    }

    public static bool CanPromote(Piece pc, Square from_sq, Square to_sq, Color c)
    {
        if (!CanPromote(pc)) return false;
        if (c == Color.Black)
        {
            return RankOf(from_sq) < 2 || RankOf(to_sq) < 2 ;
        }
        else
        {
            return RankOf(from_sq) > 3 || RankOf(to_sq) > 3;
        }
    }

    public static bool MustPromote(Piece pc, Square sq, Color c)
    {
        if (!CanPromote(pc)) return false;
        if (c == Color.Black)
        {
            if(pc == Piece.B_Knight) {
                return RankOf(sq) < 2;
            }
            if(pc == Piece.B_Lance || pc == Piece.B_Pawn) {
                return RankOf(sq) < 1;
            }
        }
        else
        {
            if(pc == Piece.W_Knight) {
                return RankOf(sq) > 3;
            }
            if(pc == Piece.W_Lance || pc == Piece.W_Pawn) {
                return RankOf(sq) > 4;
            }
        }
        return false;
    }

    public static Move MakeMove(Square from, Square to, Piece pc, bool isPromote, bool isDrop) {
        Move m = new Move();
        int move = 0;
        move += (int)to;
        move += ((int)pc << 16);
        if (isDrop)
        {
            move += (int)Move32.MOVE_DROP;
        } else {
            move += ((int)from << 7);
        }
        if (isPromote) move += (int)Move32.MOVE_PROMOTE;
        m.move = (Move32)move;
        return m;
    }

    public static Move MakeResignMove() {
        Move m = new Move();
        m.move = Move32.MOVE_RESIGN;
        return m;
    }


    public static EHistoryItemResult GameResultToEHistoryItemResult(GameResult gameResult, Color playerColor) {
        if(gameResult == GameResult.Draw) {
            return EHistoryItemResult.DRAW;
        }
        if(gameResult == GameResult.BlackWon) {
            return playerColor == Color.Black ? EHistoryItemResult.WIN : EHistoryItemResult.LOSE;
        }
        if(gameResult == GameResult.WhiteWon) {
            return playerColor == Color.Black ? EHistoryItemResult.LOSE : EHistoryItemResult.WIN;
        }
        return EHistoryItemResult.UNKNOWN;
    }

    public static EBoardType GameTypeToEBoardType(GameType gameType) {
        switch(gameType) {
            case GameType.Ranshogi66:
                return EBoardType.BOARD_66;
            default:
                return EBoardType.BOARD_99;
        }
    }

    public static Square ToSquare66(int index) {
        var f = index / 6;
        var r = index % 6;
        return (Square)(f * 9 + r);
    }


    public static EGradeRankType RateToGradeRankType(int rate) {

        if(rate <= 1250) {
            return EGradeRankType.GRADE_9;
        } else if(1250 < rate && rate <= 1300) {
            return EGradeRankType.GRADE_8;
        } else if(1300 < rate && rate <= 1350) {
            return EGradeRankType.GRADE_7;
        } else if(1350 < rate && rate <= 1400) {
            return EGradeRankType.GRADE_6;
        } else if(1400 < rate && rate <= 1450) {
            return EGradeRankType.GRADE_5;
        } else if(1450 < rate && rate <= 1500) {
            return EGradeRankType.GRADE_4;
        } else if(1500 < rate && rate <= 1550) {
            return EGradeRankType.GRADE_3;
        } else if(1550 < rate && rate <= 1600) {
            return EGradeRankType.GRADE_2;
        } else if(1600 < rate && rate <= 1650) {
            return EGradeRankType.GRADE_1;
        } else if(1650 < rate && rate <= 1700) {
            return EGradeRankType.RANK_1;
        } else if(1700 < rate && rate <= 1750) {
            return EGradeRankType.RANK_2;
        } else if(1750 < rate && rate <= 1800) {
            return EGradeRankType.RANK_3;
        } else if(1800 < rate && rate <= 1850) {
            return EGradeRankType.RANK_4;
        } else if(1850 < rate && rate <= 1900) {
            return EGradeRankType.RANK_5;
        } else if(1900 < rate && rate <= 1950) {
            return EGradeRankType.RANK_6;
        } else if(1950 < rate && rate <= 2000) {
            return EGradeRankType.RANK_7;
        } else {
            return EGradeRankType.RANK_8;
        }
    }
}

public enum Move32 : int
{

    MOVE_NONE = 0,             // 無効な移動

    MOVE_NULL = (1 << 7) + 1,  // NULL MOVEを意味する指し手。Square(1)からSquare(1)への移動は存在しないのでここを特殊な記号として使う。
    MOVE_RESIGN = (2 << 7) + 2,  // << で出力したときに"resign"と表示する投了を意味する指し手。
    MOVE_WIN = (3 << 7) + 3,  // 入玉時の宣言勝ちのために使う特殊な指し手

    MOVE_DROP = 1 << 14,       // 駒打ちフラグ
    MOVE_PROMOTE = 1 << 15,       // 駒成りフラグ
};


public struct Move
{
    public bool isDrop() { return ((int)move & (int)Move32.MOVE_DROP) != 0; }
    public bool isPromote() { return ((int)move & (int)Move32.MOVE_PROMOTE) != 0; }
    public bool isResign() { return ((int)move & (int)Move32.MOVE_RESIGN) != 0; }
    public Square from() { return (Square)(((int)move >> 7) & 0x7f); }
    public Square to() { return (Square)((int)move & 0x7f); }
    public Piece movedPieceAfter() { return isPromote() ? G.Promoted(movedPiece()) : movedPiece(); }
    public Piece movedPiece() { return (Piece)(((int)move >> 16) & 0x7f); }
    public Piece capturedPiece() { return (Piece)(((int)move >> 24) & 0x7f); }
    public void setPromote() { 
        move = (Move32)((int)move | (int)Move32.MOVE_PROMOTE); 
    }
    public Color color() { return G.ColorOf(movedPiece()); }



    public void Show() {
        Debug.Log("from: " + from() + ", to: " + to() + ", isDrop: " + isDrop() + ", isPromote: " + isPromote() + ", movedPiece: " + movedPiece() + ", capturedPiece: " + capturedPiece());
    }
    public Move32 move;


    public static bool operator ==(Move m1, Move m2)
    {
        return m1.isDrop() == m2.isDrop() && m1.isPromote() == m2.isPromote() && m1.isResign() == m2.isResign() && m1.from() == m2.from() && m1.to() == m2.to() && m1.color() == m2.color();
    }

    public static bool operator !=(Move m1, Move m2)
    {
        return !(m1.isDrop() == m2.isDrop() && m1.isPromote() == m2.isPromote() && m1.isResign() == m2.isResign() && m1.from() == m2.from() && m1.to() == m2.to() && m1.color() == m2.color());
    }

}


public struct CheckInfo {
    public bool isDoubleCheck;
    public Square checkSq;
}

public enum GameMessageFlag { 
    BlackNickname = 1,
    WhiteNickname = 2,
    Sfen = 3,
    Move = 4,
    ChangeGameState = 5,
    SyncTime = 6,
    MateMove = 7,
}

public static class Extensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }
}

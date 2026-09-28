using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Bitboard 
{

    public ulong p0 = 0;
    public ulong p1 = 0;

    protected Bitboard[] KingEffectBB = new Bitboard[(int)Square.SQ_NB];
    protected Bitboard[, ] KnightEffectBB = new Bitboard[(int)Square.SQ_NB, 2];
    protected Bitboard[, ] GoldEffectBB = new Bitboard[(int)Square.SQ_NB, 2];
    protected Bitboard[, ] SilverEffectBB = new Bitboard[(int)Square.SQ_NB, 2];

    public Bitboard[] KnightMaskBB = new Bitboard[2];
    public Bitboard[] PawnMaskBB = new Bitboard[2];
    public Bitboard[] FileBB;
    public Bitboard[] RankBB;

    protected ulong[, ] RookFileEffect = new ulong[10, 128];
    protected Bitboard[, ] RookRankEffect = new Bitboard[10, 128];

    public Bitboard ZERO_BB;
    public Bitboard[] SquareBB = new Bitboard[(int)Square.SQ_NB];

    public Bitboard() { this.p0 = 0; this.p1 = 0; }
    public Bitboard(ulong p0, ulong p1) { this.p0 = p0; this.p1 = p1; }

    public abstract int FILE_NB();
    public abstract int RANK_NB();
    public abstract int SQ_NB();
    protected abstract int[] Slide();
    protected abstract void InitFileBBAndRankBB();

    public Square Sq(int file, int rank) { return (Square)(file * RANK_NB() + rank); }
    public int RankOf(Square sq) { return (int)sq % RANK_NB(); }
    public int FileOf(Square sq) { return (int)sq / FILE_NB(); }


    public static Bitboard NewBitboard(Type type) {
        if(type == typeof(Bitboard66)) {
            return new Bitboard66();
        } else {
            return new Bitboard99();
        }
    }

    public static Bitboard operator |(Bitboard b1, Bitboard b2)
    {
        Bitboard b = NewBitboard(b1.GetType());
        b.p0 = (b1.p0 | b2.p0);
        b.p1 = (b1.p1 | b2.p1);
        return b;
    }

    public static Bitboard operator &(Bitboard b1, Bitboard b2)
    {
        Bitboard b = NewBitboard(b1.GetType());
        b.p0 = (b1.p0 & b2.p0);
        b.p1 = (b1.p1 & b2.p1);
        return b;
    }

    public static Bitboard operator ^(Bitboard b1, Bitboard b2)
    {
        Bitboard b = NewBitboard(b1.GetType());
        b.p0 = (b1.p0 ^ b2.p0);
        b.p1 = (b1.p1 ^ b2.p1);
        return b;
    }

    public static Bitboard operator <<(Bitboard bb, int shift)
    {
        Bitboard b = NewBitboard(bb.GetType());
        b.p0 = bb.p0 << shift;
        b.p1 = bb.p1 << shift;
        return b;
    }



    public virtual Bitboard AllBB() {
        Bitboard b = Bitboard.NewBitboard(GetType());
        b.p0 |= 0x7fffffffffffffff;
        b.p1 |= 0x3ffff;
        return b;
    }

    public static Bitboard operator |(Bitboard b, Square sq)  {
        if(b.GetType() == typeof(Bitboard66)) {
            return b | (Bitboard)Bitboard66.Instance().SquareBB[(int)sq];
        //} else if(b.GetType() == typeof(Bitboard99)) {
        } else {
            return b | (Bitboard)Bitboard99.Instance().SquareBB[(int)sq];
        }
    }

    public static Bitboard operator &(Bitboard b, Square sq) {
        if(b.GetType() == typeof(Bitboard66)) {
            return (Bitboard66)b & (Bitboard66)Bitboard66.Instance().SquareBB[(int)sq];
        //} else if(b.GetType() == typeof(Bitboard99)) {
        } else {
            return (Bitboard99)b & (Bitboard99)Bitboard99.Instance().SquareBB[(int)sq];
        }
    }

    public static Bitboard operator ^(Bitboard b, Square sq) {
        if(b.GetType() == typeof(Bitboard66)) {
            return (Bitboard66)b ^ (Bitboard66)Bitboard66.Instance().SquareBB[(int)sq];
        //} else if (b.GetType() == typeof(Bitboard99)) {
        } else {
            return (Bitboard99)b ^ (Bitboard99)Bitboard99.Instance().SquareBB[(int)sq];
        }
    }

    public static Bitboard operator ~(Bitboard b) {
        if(b.GetType() == typeof(Bitboard66)) {
            return (Bitboard66)b ^ (Bitboard66)Bitboard66.Instance().AllBB();
        //} else if (b.GetType() == typeof(Bitboard99)) {
        } else {
            return (Bitboard99)b ^ (Bitboard99)Bitboard99.Instance().AllBB();
        }
    }


    public static implicit operator bool(Bitboard b) {
        return b.p0 > 0 || b.p1 > 0;
    }

    public Square Pop() {
        if(p1 > 0) {
            int sq = Bitboard.BitScanForward(p1);
            if(sq == -1) return Square.SQ_Invalid;
            p1 = p1 & (p1-1);
            return (Square)(sq + 63);
        } else {
            int sq = Bitboard.BitScanForward(p0);
            if(sq == -1) return Square.SQ_Invalid;
            p0 = p0 & (p0-1);
            return (Square)sq;
        }
    }

    public List<Square> ToSquares() {
        List<Square> squares = new List<Square>();
        Bitboard bb = (Bitboard)Clone();
        Square sq = Square.SQ_Invalid;
        do {
            sq = bb.Pop();
            if(sq != Square.SQ_Invalid) squares.Add(sq);
        } while(bb);
        return squares;
    }


    public object Clone() {
        return MemberwiseClone();
    }

    public abstract Bitboard rookFileEffect(Square sq, Bitboard occupied);

    public abstract Bitboard rookRankEffect(Square sq, Bitboard occupied);


    public Bitboard kingEffect(Square sq) {
        return KingEffectBB[(int)sq];
    }

    public Bitboard goldEffect(Color c, Square sq) {
        return GoldEffectBB[(int)sq, (int)c];
    }

    public Bitboard silverEffect(Color c, Square sq) {
        return SilverEffectBB[(int)sq, (int)c];
    }

    public Bitboard knightEffect(Color c, Square sq) {
        return KnightEffectBB[(int)sq, (int)c];
    }
    public Bitboard rookStepEffect(Square sq) {
        return rookFileEffect(sq, ZERO_BB) | rookRankEffect(sq, ZERO_BB);
    }

    public Bitboard rookEffect(Square sq, Bitboard occupied) {
        return rookFileEffect(sq, occupied) | rookRankEffect(sq, occupied);
    }

    public Bitboard horseEffect(Square sq, Bitboard occupied) {
        return bishopEffect(sq, occupied) | kingEffect(sq);
    }

    public Bitboard dragonEffect(Square sq, Bitboard occupied) {
        return rookEffect(sq, occupied) | kingEffect(sq);
    }

    public abstract Bitboard NewBitboard();

    public Bitboard lanceStepEffect(Color c, Square sq) {
        Bitboard bb = NewBitboard();
        int f = FileOf(sq);
        int r = RankOf(sq);
        if(c == Color.Black) {
            r--;
            while(r >= 0) { bb |= Sq(f, r); r--; }
        } else {
            r++;
            while(r <= RANK_NB()-1) { bb |= Sq(f, r); r++; }
        }
        return bb;
    }

    public Bitboard lanceEffect(Color c, Square sq, Bitboard occupied) {
        return rookFileEffect(sq, occupied) & lanceStepEffect(c, sq);
    }

    public Bitboard pawnEffect(Color c, Square sq) {
        Bitboard bb = NewBitboard();
        int f = FileOf(sq);
        int r = RankOf(sq);
        if(c == Color.Black) {
            r--;
            if(r >= 0) bb |= Sq(f, r);
        } else {
            r++;
            if(r <= RANK_NB()-1) bb |= Sq(f, r); 
        }
        return bb;
    }

    public Bitboard bishopEffect(Square sq, Bitboard occupied) {
        int[,] directions = {{1, 1}, {-1, 1}, {1, -1}, {-1, -1}};
        Bitboard bb = NewBitboard();

        for(int i=0; i < 4; i++) {
            int d1 = directions[i, 0];
            int d2 = directions[i, 1];

            int f = FileOf(sq);
            int r = RankOf(sq);

            while(true) {
                f += d1;
                r += d2;
                if(f < 0 || r < 0 || f > FILE_NB()-1 || r > RANK_NB()-1) break;
                Square nsq = Sq(f, r);
                bb |= nsq;
                if(occupied & nsq) break;
            }
        }
        return bb;
    }

    public Bitboard bishopStepEffect(Square sq) {
        return bishopEffect(sq, ZERO_BB);
    }


    public Bitboard EffectsFrom(Piece pc, Square sq, Bitboard occ) {
        switch (pc)
        {
            case Piece.B_Pawn: return pawnEffect(Color.Black, sq);
            case Piece.B_Lance: return lanceEffect(Color.Black, sq, occ);
            case Piece.B_Knight: return knightEffect(Color.Black, sq);
            case Piece.B_Silver: return silverEffect(Color.Black, sq);
            case Piece.B_Gold: case Piece.B_Pro_Pawn: case Piece.B_Pro_Lance: case Piece.B_Pro_Knight: case Piece.B_Pro_Silver: return goldEffect(Color.Black, sq);

            case Piece.W_Pawn: return pawnEffect(Color.White, sq);
            case Piece.W_Lance: return lanceEffect(Color.White, sq, occ);
            case Piece.W_Knight: return knightEffect(Color.White, sq);
            case Piece.W_Silver: return silverEffect(Color.White, sq);
            case Piece.W_Gold: case Piece.W_Pro_Pawn: case Piece.W_Pro_Lance: case Piece.W_Pro_Knight: case Piece.W_Pro_Silver: return goldEffect(Color.White, sq);

            //　先後同じ移動特性の駒
            case Piece.B_Bishop: case Piece.W_Bishop: return bishopEffect(sq, occ);
            case Piece.B_Rook: case Piece.W_Rook: return rookEffect(sq, occ);
            case Piece.B_Horse: case Piece.W_Horse: return horseEffect(sq, occ);
            case Piece.B_Dragon: case Piece.W_Dragon: return dragonEffect(sq, occ);
            case Piece.B_King: case Piece.W_King: return kingEffect(sq);
            default: return ZERO_BB;
        }
    }

    public virtual void Init() { 
        ZERO_BB = NewBitboard();
        ZERO_BB = NewBitboard();
        FileBB = new Bitboard[FILE_NB()];
        RankBB = new Bitboard[RANK_NB()];
        InitFileBBAndRankBB();
        // SquareBB
        for(int i = 0; i<(int)SQ_NB(); i++)  {
            SquareBB[i] = NewBitboard();
            if(i < 63) {
                SquareBB[i].p0 = ((ulong)1 << i);
                SquareBB[i].p1 = 0;
            } else {
                SquareBB[i].p0 = 0;
                SquareBB[i].p1 = ((ulong)1 << (i-63));
            }
        }

        KnightMaskBB[(int)Color.Black] = AllBB() ^ RankBB[0] ^ RankBB[1];
        KnightMaskBB[(int)Color.White] = AllBB() ^ RankBB[RANK_NB()-1] ^ RankBB[RANK_NB()-2];
        PawnMaskBB[(int)Color.Black] = AllBB() ^ RankBB[0];
        PawnMaskBB[(int)Color.White] = AllBB() ^ RankBB[RANK_NB()-1];

        for (int rank = 0; rank < RANK_NB(); rank++)
        {
            Square sq = Sq(0, rank);

            int num1s = RANK_NB()-2;
            for (int i = 0; i < (1 << num1s); ++i)
            {
                // iはsqに駒をおいたときに、その筋の2段～5段目の升がemptyかどうかを表現する値なので
                // 1ビットシフトして、1～6段目の升を表現するようにする。
                int ii = i << 1;
                Bitboard bb = NewBitboard();
                for (int r = RankOf(sq) - 1; r >= 0; --r)
                {
                    bb |= Sq(FileOf(sq), r);
                    if ((ii & (1 << r)) > 0)
                        break;
                }
                for (int r = RankOf(sq) + 1; r <= RANK_NB()-1; ++r)
                {
                    bb |= Sq(FileOf(sq), r);
                    if ((ii & (1 << r)) > 0)
                        break;
                }
                RookFileEffect[rank, i] = bb.p0;
            }
        }

        // 飛車の横の利き
        for (int file = 0 ; file <= FILE_NB()-1 ; ++file )
        {
            // sq = SQ_11 , SQ_21 , ... , SQ_NBまで
            Square sq = Sq(file, 0);
            
            int num1s = FILE_NB()-2;
            for (int i = 0; i < (1 << num1s); ++i)
            {
                int ii = i << 1;
                Bitboard bb = NewBitboard();
                for (int f = FileOf(sq) - 1; f >= 0; --f)
                {
                    bb |= Sq(f, RankOf(sq));
                    if ((ii & (1 << f)) > 0)
                        break;
                }
                for (int f = FileOf(sq) + 1; f <= (FILE_NB()-1); ++f)
                {
                    bb |= Sq(f, RankOf(sq));
                    if ((ii & (1 << f)) > 0)
                        break;
                }
                RookRankEffect[file, i] = bb;
                // RookRankEffect[FILE_NB][x] には値を代入していないがC++の規約によりゼロ初期化されている。
            }
        }

        for(int i = 0; i<(int)(SQ_NB()-1); i++)  {
            KingEffectBB[i] = rookEffect((Square)i, AllBB()) | bishopEffect((Square)i, AllBB());
            for(int j=0; j<2; j++) {
                GoldEffectBB[i, j] = rookEffect((Square)i, AllBB());
                KnightEffectBB[i, j] = NewBitboard();
                int f = FileOf((Square)i);
                int r = RankOf((Square)i);
                if((Color)j == Color.Black) {
                    if(r > 0) {
                        if(f > 0) GoldEffectBB[i, j] |= Sq(f-1, r-1);
                        if(f < FILE_NB()-1) GoldEffectBB[i, j] |= Sq(f+1, r-1);
                    }
                    if(r > 1) {
                        if(f > 0) KnightEffectBB[i, j] |= Sq(f-1, r-2);
                        if(f < FILE_NB()-1) KnightEffectBB[i, j] |= Sq(f+1, r-2);
                    }
                } else {
                    if(r < RANK_NB()-1) {
                        if(f > 0) GoldEffectBB[i, j] |= Sq(f-1, r+1);
                        if(f < FILE_NB()-1) GoldEffectBB[i, j] |= Sq(f+1, r+1);
                    }
                    if(r < RANK_NB()-2) {
                        if(f > 0) KnightEffectBB[i, j] |= Sq(f-1, r+2);
                        if(f < FILE_NB()-1) KnightEffectBB[i, j] |= Sq(f+1, r+2);
                    }
                }
                SilverEffectBB[i, j] = pawnEffect((Color)j, (Square)i) | bishopEffect((Square)i, AllBB());
            }
        }
    }

    public void show(Color c = Color.Black) {
        Debug.Log("--- show bitboard ---");
        string line = "";
        if(c == Color.Black) {
            for (int rank = 0; rank <= RANK_NB()-1; ++rank)
            {
                for (int file = FILE_NB()-1; file >= 0; --file) {
                    line += ((this & Sq(file, rank)) ? " *" : " .");
                }
                line += "\n";
            }
        } else {
            for (int rank = RANK_NB()-1; rank >= 0; --rank)
            {
                for (int file = 0; file <= FILE_NB()-1; ++file)
                    line += ((this & Sq(file, rank)) ? " *" : " .");
                line += "\n";
            }
        }
        Debug.Log(line);
    }

    public bool Aligned(Square sq1, Square sq2, Square sq3) {
        int[,,] directions = new int[,,] {{{1, 1}, {-1, -1}}, {{1, 0}, {-1, 0}}, {{0, 1}, {0, -1}}, {{1, -1}, {-1, 1}}};
        for(int i=0; i<4; i++) {
            int f = FileOf(sq1);
            int r = RankOf(sq1);
            bool sq2ok = false;
            bool sq3ok = false;
            for(int j = 0; j<2; j++) {
                f += directions[i, j, 0];
                r += directions[i, j, 1];
                while(f >= 0 && f <= FILE_NB()-1 && r >= 0 && r <= RANK_NB()-1) {
                    if((int)Sq(f, r) == (int)sq2) sq2ok = true;
                    if((int)Sq(f, r) == (int)sq3) sq3ok = true;
                    f += directions[i, j, 0];
                    r += directions[i, j, 1];
                }
            }
            if(sq2ok && sq3ok) return true;
            if((!sq2ok && sq3ok) || (sq2ok && !sq3ok)) return false;
        }
        return false;
    }

    public bool Aligned(Square sq1, Square sq2) {
        int[,,] directions = new int[,,] {{{1, 1}, {-1, -1}}, {{1, 0}, {-1, 0}}, {{0, 1}, {0, -1}}, {{1, -1}, {-1, 1}}};
        for(int i=0; i<4; i++) {
            int f = FileOf(sq1);
            int r = RankOf(sq1);
            bool sq2ok = false;
            for(int j = 0; j<2; j++) {
                f += directions[i, j, 0];
                r += directions[i, j, 1];
                while(f >= 0 && f <= FILE_NB()-1 && r >= 0 && r <= RANK_NB()-1) {
                    if((int)Sq(f, r) == (int)sq2) sq2ok = true;
                    f += directions[i, j, 0];
                    r += directions[i, j, 1];
                }
            }
            if(sq2ok) return true;
        }
        return false;
    }


    private const ulong DeBruijnSequence = 0x37E84A99DAE458F;

    private static readonly int[] MultiplyDeBruijnBitPosition =
    {
        0, 1, 17, 2, 18, 50, 3, 57,
        47, 19, 22, 51, 29, 4, 33, 58,
        15, 48, 20, 27, 25, 23, 52, 41,
        54, 30, 38, 5, 43, 34, 59, 8,
        63, 16, 49, 56, 46, 21, 28, 32,
        14, 26, 24, 40, 53, 37, 42, 7,
        62, 55, 45, 31, 13, 39, 36, 6,
        61, 44, 12, 35, 60, 11, 10, 9,
    };

    private static int BitScanForward(ulong b)
    {
        if(b == 0) return -1;
        return MultiplyDeBruijnBitPosition[((ulong)((long)b & -(long)b) * DeBruijnSequence) >> 58];
    }


    public int[] GetDirection(Square sq1, Square sq2)
    {
        int f1 = FileOf(sq1);
        int r1 = RankOf(sq1);
        int f2 = FileOf(sq2);
        int r2 = RankOf(sq2);
        int f = f2 - f1;
        int r = r2 - r1;
        if(Math.Abs(f) == Math.Abs(r) || Math.Abs(f) == 0 || Math.Abs(r) == 0) {
            if (f != 0) f /= Math.Abs(f);
            if (r != 0) r /= Math.Abs(r);
            return new int[] { f, r };
        } else {
            return new int[] {0, 0};
        }
    }

    public Bitboard BetweenBB(Square sq1, Square sq2) {
        Bitboard bb = NewBitboard();

        int[] di = GetDirection(sq1, sq2);
        // when sq1, sq2 not alined 
        if(di[0] == 0 && di[1] == 0) return bb;

        int f = FileOf(sq1) + di[0];
        int r = RankOf(sq1) + di[1];

        Square s = Sq(f, r);

        while (s != sq2) {
            f += di[0];
            r += di[1];
            bb |= s;
            s = Sq(f, r);
        }

        return bb;
    }

    public Bitboard AlignedBB(Square sq1, Square sq2) {
        Bitboard bb = NewBitboard();
        int[] di = GetDirection(sq2, sq1);
        int[] di2 = GetDirection(sq1, sq2);

        // when sq1, sq2 not alined 
        if(di[0] == 0 && di[1] == 0) return bb;

        int f = FileOf(sq1);
        int r = RankOf(sq1);

        while (f >= 0 && f <= FILE_NB()-1 && r >= 0 && r <= RANK_NB()-1) {
            bb |= Sq(f, r);
            f += di[0];
            r += di[1];
        }

        bb |= sq1;

        f = FileOf(sq1);
        r = RankOf(sq1);

        while (f >= 0 && f <= FILE_NB()-1 && r >= 0 && r <= RANK_NB()-1) {
            bb |= Sq(f, r);
            f += di2[0];
            r += di2[1];
        }

        return bb;
    }

}

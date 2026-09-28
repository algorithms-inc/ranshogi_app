using UnityEngine;
using System;

public class Bitboard66 : Bitboard
{

    public override int FILE_NB() { return 6; }
    public override int RANK_NB() { return 6; }
    public override int SQ_NB() { return 37; }


    public Bitboard66() : base() {  }
    public Bitboard66(ulong p0, ulong p1) : base(p0, p1) {  }

    public override Bitboard NewBitboard() {
        return new Bitboard66();
    }

    protected override int[] Slide() {
        int[] slide = {
            1, 1, 1, 1, 1, 1,
            7, 7, 7, 7, 7, 7,
            13, 13, 13, 13, 13, 13,
            19, 19, 19, 19, 19, 19,
            25, 25, 25, 25, 25, 25,
            31, 31, 31, 31, 31, 31,
        };
        return slide;
    }

    static Bitboard66 bb = null;

    public override Bitboard AllBB() {
        Bitboard b = Bitboard.NewBitboard(GetType());
        b.p0 |= 0xfffffffff;
        b.p1 = 0;
        return b;
    }

    public static Bitboard66 Instance() {
        if(bb == null) {
            bb = new Bitboard66();
            bb.Init();
        }
        return bb;
    }

    protected override void InitFileBBAndRankBB() {
        FileBB[0] = new Bitboard66((ulong)0x3f, 0);
        FileBB[1] = new Bitboard66((ulong)0xfc0, 0);
        FileBB[2] = new Bitboard66((ulong)0x3f000, 0);
        FileBB[3] = new Bitboard66((ulong)0xfc0000, 0);
        FileBB[4] = new Bitboard66((ulong)0x3f000000, 0);
        FileBB[5] = new Bitboard66((ulong)0xfc0000000, 0);

        for(int i=0; i<RANK_NB(); i++) {
            RankBB[i] = new Bitboard66((ulong)(1090785345) << i, 0);
        }
    }


    public override Bitboard rookFileEffect(Square sq, Bitboard occupied) {
        int index = (int)((occupied.p0 >> Slide()[(int)sq]) & 0x0f);
        int f = G.FileOf(sq);
        Bitboard b = new Bitboard66();
        b.p0 = RookFileEffect[(int)G.RankOf(sq), index] << (int)Sq(f, 0);
        return b;
    }

    public override Bitboard rookRankEffect(Square sq, Bitboard occupied) {
        int index = 0;
        int r = G.RankOf(sq);
        for(int i = 0; i < FILE_NB()-2; i++) {
            if(((Bitboard66)occupied & G.Sq(i+1, r)).p0 > 0) {
                index += (int)Math.Pow(2, i);
            }
        }
        Bitboard b = new Bitboard66();
        return RookRankEffect[(int)G.FileOf(sq), index] << r;
    }

}
using System;
using UnityEngine;

public class Bitboard99 : Bitboard
{
    public override int FILE_NB() { return 9; }
    public override int RANK_NB() { return 9; }
    public override int SQ_NB() { return 82; }


    public override Bitboard NewBitboard() {
        return new Bitboard99();
    }

    public Bitboard99() : base() {  }
    public Bitboard99(ulong p0, ulong p1) : base(p0, p1) {  }

    protected override int[] Slide() {
        int[] slide = {
            1, 1, 1, 1, 1, 1, 1, 1, 1,
            10, 10, 10, 10, 10, 10, 10, 10, 10,
            19, 19, 19, 19, 19, 19, 19, 19, 19,
            28, 28, 28, 28, 28, 28, 28, 28, 28,
            37, 37, 37, 37, 37, 37, 37, 37, 37,
            46, 46, 46, 46, 46, 46, 46, 46, 46,
            55, 55, 55, 55, 55, 55, 55, 55, 55,
            1, 1, 1, 1, 1, 1, 1, 1, 1,
            10, 10, 10, 10, 10, 10, 10, 10, 10,
            0
        };
        return slide;
    }


    static Bitboard99 bb = null;

    public static Bitboard99 Instance() {
        if(bb == null) {
            bb = new Bitboard99();
            bb.Init();
        }
        return bb;
    }

    protected override void InitFileBBAndRankBB()
    {
        FileBB[0] = new Bitboard99((ulong)(0x1ff) << (9 * 0), 0);
        FileBB[1] = new Bitboard99((ulong)(0x1ff) << (9 * 1), 0);
        FileBB[2] = new Bitboard99((ulong)(0x1ff) << (9 * 2), 0);
        FileBB[3] = new Bitboard99((ulong)(0x1ff) << (9 * 3), 0);
        FileBB[4] = new Bitboard99((ulong)(0x1ff) << (9 * 4), 0);
        FileBB[5] = new Bitboard99((ulong)(0x1ff) << (9 * 5), 0);
        FileBB[6] = new Bitboard99((ulong)(0x1ff) << (9 * 6), 0);
        FileBB[7] = new Bitboard99(0, (ulong)0x1ff << (9 * 0));
        FileBB[8] = new Bitboard99(0, (ulong)0x1ff << (9 * 1));

        for(int i=0; i<RANK_NB(); i++)
            RankBB[i] = new Bitboard99((ulong)(0x40201008040201) << i, (ulong)0x201 << i);
    }


    public override Bitboard rookFileEffect(Square sq, Bitboard occupied) {
        int index = (int)(((sq <= Square.SQ_79 ? occupied.p0 : occupied.p1) >> Slide()[(int)sq]) & 0x7f);
        int f = FileOf(sq);
        Bitboard b = new Bitboard99();
        b.p0 = RookFileEffect[(int)RankOf(sq), index] << (int)Sq(f, 0);
        return (f <= 6) ?
            new Bitboard99(RookFileEffect[RankOf(sq), index] << (int)Sq(f, 0), 0) :
            new Bitboard99(0, RookFileEffect[RankOf(sq), index] << (int)(Sq((f - 7), 0)));
        return b;
    }

    public override Bitboard rookRankEffect(Square sq, Bitboard occupied) {
        int index = 0;
        int r = RankOf(sq);
        for(int i = 0; i < 7; i++) {
            var bb = ((Bitboard99)occupied & Sq(i+1, r));
            if(bb) {
                index += (int)Math.Pow(2, i);
            }
        }
        return RookRankEffect[(int)FileOf(sq), index] << r;
    }

}
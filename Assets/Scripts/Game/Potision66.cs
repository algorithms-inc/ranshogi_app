using System.Collections.Generic;
using GFramework.Core;

public class Position66 : Position
{

    public override Bitboard NewBitboard() { return new Bitboard66(); }
    public override Bitboard BitboardInstance() { return Bitboard66.Instance(); }

    public override int FILE_NB() { return 6; }
    public override int RANK_NB() { return 6; }
    public override int N_RANK_PROMOTE() { return 3; }
    public override void Init() {
        byTypeBB = new Bitboard66[(int)PieceType.PieceTypeNb];
        byColorBB = new Bitboard66[2];

        checkSquares = new Bitboard66[(int)PieceType.PieceTypeNb];
        checkersBB = new Bitboard66();

        // for evasion
        attackersBB = new Bitboard66[2];

        // for mate check
        attackersBBWithoutKing = new Bitboard66[2];

        blockersForKing = new Bitboard66[2];

        for(int i=0; i<(int)PieceType.PieceTypeNb; i++) {
            byTypeBB[i] = new Bitboard66();
            checkSquares[i] = new Bitboard66();
        }
        for(int i=0; i<2; i++) {
            byColorBB[i] = new Bitboard66();
        }
        base.Init();

    }

    // 型安全でないが仕方ない
    //public override List<FPieceAndSquare> NewPieceList() {
        //return (new List<FPieceAndSquare66>()).Cast<FPieceAndSquare>.ToList();
    //}


}
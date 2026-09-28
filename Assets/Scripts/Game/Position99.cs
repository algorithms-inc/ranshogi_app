using System.Collections.Generic;
using GFramework.Core;

public class Position99 : Position
{
    public override Bitboard NewBitboard() { return new Bitboard99(); }
    public override Bitboard BitboardInstance() { return Bitboard99.Instance(); }

    public override int FILE_NB() { return 9; }
    public override int RANK_NB() { return 9; }
    public override int N_RANK_PROMOTE() { return 3; }
    public override void Init() {
        byTypeBB = new Bitboard99[(int)PieceType.PieceTypeNb];
        byColorBB = new Bitboard99[2];

        checkSquares = new Bitboard99[(int)PieceType.PieceTypeNb];
        checkersBB = new Bitboard99();

        // for evasion
        attackersBB = new Bitboard99[2];

        // for mate check
        attackersBBWithoutKing = new Bitboard99[2];

        blockersForKing = new Bitboard99[2];

        for(int i=0; i<(int)PieceType.PieceTypeNb; i++) {
            byTypeBB[i] = new Bitboard99();
            checkSquares[i] = new Bitboard99();
        }
        for(int i=0; i<2; i++) {
            byColorBB[i] = new Bitboard99();
        }
        base.Init();

    }
    // 型安全でないが仕方ない
    //public override List<FPieceAndSquare> NewPieceList() {
        //return (new List<FPieceAndSquare66>()).Cast<FPieceAndSquare>.ToList();
    //}

}
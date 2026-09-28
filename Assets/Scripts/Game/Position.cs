using System;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;

public abstract class Position : MonoBehaviour
{
    public Piece[] board;
    public bool[] emphasizedCells;
    public int[] whiteHand = new int[7];
    public int[] blackHand = new int[7];

    public Square[] kingSquare = new Square[2];

    public Color sideToMove = Color.Black;
    public Color myColor = Color.Black;
    public Color opponentColor = Color.Black;


    public Bitboard[] byTypeBB;
    public Bitboard[] byColorBB;

    public Bitboard[] checkSquares;
    public Bitboard checkersBB;

    public List<Move> moves = new List<Move>();
    // for evasion
    public Bitboard[] attackersBB;

    // for mate check
    protected Bitboard[] attackersBBWithoutKing;

    public Bitboard[] blockersForKing;

    public abstract Bitboard NewBitboard();
    public abstract Bitboard BitboardInstance();

    public int SQ_NB() { return FILE_NB() * RANK_NB() + 1; }
    public abstract int FILE_NB();
    public abstract int RANK_NB();
    public abstract int N_RANK_PROMOTE();
    public Square Sq(int file, int rank) { return (Square)(file * RANK_NB() + rank); }
    public int RankOf(Square sq) { return (int)sq % RANK_NB(); }
    public int FileOf(Square sq) { return (int)sq / FILE_NB(); }
    public Square SquareToDesignSquare(int index) { 
        int f = index / FILE_NB();
        int r = index % RANK_NB();
        return (Square)(f * 9 + r);
    }

    public Position()
    {
        Init();
    }

    public virtual void Init()
    {
        board = new Piece[SQ_NB()-1];
        emphasizedCells = new bool[SQ_NB()-1];
        for (int i = 0; i < SQ_NB() - 1; i++) {
            board[i] = Piece.No_Piece;
            emphasizedCells[i] = false;
        }
        for (int i = 0; i < 7; i++)
        {
            blackHand[i] = 0;
            whiteHand[i] = 0;
        }
    }


    public void Set(string sfen)
    {
        Debug.Log("sfen: " + sfen);
        Piece[] pieces = ParseSfen(sfen);
        for (int i = 0; i < SQ_NB()-1; i++)
        {
            if (pieces[i] != Piece.No_Piece)
            {
                Set(pieces[i], (Square)i);
            }
        }
        string[] cmds = sfen.Split(' ');
        myColor = cmds[3] == "w" ? Color.White : Color.Black;
        opponentColor = cmds[3] == "w" ? Color.Black : Color.White;
        sideToMove = cmds[4] == "w" ? Color.White : Color.Black;


        Debug.Log(sfen);
        if (cmds[2] != "-")
        {
            int count = 1;
            foreach (char c in cmds[2])
            {
                if (Char.IsDigit(c))
                {
                    count = c - '0';
                }
                else
                {
                    Piece pc = (Piece)G.PieceToIndex.IndexOf(c);
                    if(G.ColorOf(pc) == Color.Black) {
                        blackHand[G.HandIndexOf(pc)] = count;
                    } else { 
                        whiteHand[G.HandIndexOf(pc)] = count;
                    }
                    count = 1;
                }
            }
        }
        kingSquare[(int)Color.Black] = Pieces(Color.Black, PieceType.King).Pop();
        kingSquare[(int)Color.White] = Pieces(Color.White, PieceType.King).Pop();
        SetCheckInfo();
        SetAttackersBB(Color.Black);
        SetAttackersBB(Color.White);
    }

    public void Set(Piece pc, Square sq)
    {
        board[(int)sq] = pc;
        byTypeBB[(int)PieceType.AllPieces] |= sq;
        byTypeBB[(int)G.TypeOf(pc)] |= sq;
        byColorBB[(int)G.ColorOf(pc)] |= sq;
    }

    public Piece Get(Square sq)
    {
        return board[(int)sq];
    }

    public Piece[] ParseSfen(string sfen)
    {
        string[] cmds = sfen.Split(' ');
        string[] ranks = cmds[1].Split('/');
        Piece[] pieces = new Piece[SQ_NB()-1];

        int i = SQ_NB()-1-FILE_NB();
        foreach (string rank in ranks)
        {
            int k = i;
            bool isPromoted = false;
            foreach (char c in rank)
            {
                if (c == '+')
                {
                    isPromoted = true;
                    continue;
                }
                if (Char.IsDigit(c))
                {
                    int n = c - '0';
                    for (int j = 0; j < n; j++)
                    {
                        pieces[k - FILE_NB() * j] = Piece.No_Piece;
                    }
                    k -= FILE_NB() * n;
                }
                else
                {
                    Piece p = (Piece)G.PieceToIndex.IndexOf(c);
                    if (isPromoted) p = G.Promoted(p);
                    pieces[k] = p;
                    k -= FILE_NB();
                }
                isPromoted = false;
            }
            i++;
        }
        return pieces;
    }


    public void show() {
        Debug.Log("--- show ---");
        for (int i = 0; i < SQ_NB()-1; i++) Debug.Log(board[i]);
    }


    public Bitboard MovableBB(Piece pc, Square sq)
    {
        if (pc == Piece.No_Piece) return NewBitboard();

        //Debug.Log("###: ");
        //Pieces(sideToMove).show();
        //attackersBB[(int)sideToMove].show();

        if(InCheck()) {
            CheckInfo checkInfo = GetCheckInfo();
            Square ksq = kingSquare[(int)sideToMove];

            Bitboard bb = NewBitboard();
            if(G.TypeOf(pc) == PieceType.King) {
                bb = BitboardInstance().kingEffect(ksq) & ~(Pieces(sideToMove) | attackersBB[(int)sideToMove]);
                return bb;
            } else {
                if(checkInfo.isDoubleCheck) return NewBitboard();
            }

            Bitboard target1 = BitboardInstance().BetweenBB(checkInfo.checkSq, ksq);
            Bitboard target2 = target1 | checkInfo.checkSq;

            bb = (BitboardInstance().EffectsFrom(pc, sq, Pieces()) & target2);
            if(blockersForKing[(int)sideToMove] & sq) {
                bb &= BitboardInstance().AlignedBB(sq, kingSquare[(int)sideToMove]);
            }
            return bb;
        } else {
            Color c = G.ColorOf(pc);
            Bitboard effect = NewBitboard();
            switch(G.TypeOf(pc)) {
                case PieceType.Pawn: 
                    effect = BitboardInstance().pawnEffect(c, sq); break;
                case PieceType.Knight: 
                    effect = BitboardInstance().knightEffect(c, sq); break;
                case PieceType.Silver: 
                    effect = BitboardInstance().silverEffect(c, sq); break;
                case PieceType.Gold: 
                case PieceType.Pro_Pawn: 
                case PieceType.Pro_Lance: 
                case PieceType.Pro_Knight: 
                case PieceType.Pro_Silver: 
                    effect = BitboardInstance().goldEffect(c, sq); break;
                case PieceType.King: 
                    effect = BitboardInstance().kingEffect(sq) & ~(Pieces(sideToMove) | attackersBB[(int)sideToMove]); break;
                case PieceType.Lance: 
                    effect = BitboardInstance().lanceEffect(c, sq, Pieces()); break;
                case PieceType.Bishop: 
                    effect = BitboardInstance().bishopEffect(sq, Pieces()); break;
                case PieceType.Rook: 
                    effect = BitboardInstance().rookEffect(sq, Pieces()); break;
                case PieceType.Horse: 
                    effect = BitboardInstance().horseEffect(sq, Pieces()); break;
                case PieceType.Dragon: 
                    effect = BitboardInstance().dragonEffect(sq, Pieces()); break;
                default:
                    break;
            }
            effect &= ~Pieces(c); 

            if(blockersForKing[(int)sideToMove] & sq) {
                effect &= BitboardInstance().AlignedBB(sq, kingSquare[(int)sideToMove]);
            }
            return effect;
        }
    }

    public Bitboard DropableBB(Color c) {
        Bitboard bb = NewBitboard();
        int[] hand = c == Color.Black ? blackHand : whiteHand;
        if(hand[G.HandIndexOf(PieceType.Pawn)] > 0) {
            bb |= PawnDroppableBB(c);
        }
        if (hand[G.HandIndexOf(PieceType.Lance)] > 0)
        {
            bb |= LanceDroppableBB(c);
        }
        if (hand[G.HandIndexOf(PieceType.Knight)] > 0)
        {
            bb |= KnightDroppableBB(c);
        }
        if (hand[G.HandIndexOf(PieceType.Gold)] > 0 || hand[G.HandIndexOf(PieceType.Silver)] > 0 || hand[G.HandIndexOf(PieceType.Rook)] > 0 || hand[G.HandIndexOf(PieceType.Bishop)] > 0)
        {
            bb |= ~Pieces();
        }
        return bb;
    }

    public Bitboard DropableBBOf(Piece pc)
    {
        Bitboard bb = ~Pieces() & BitboardInstance().AllBB();
        if(InCheck()) {
            CheckInfo checkInfo = GetCheckInfo();
            Square ksq = kingSquare[(int)sideToMove];
            if(checkInfo.isDoubleCheck) return NewBitboard();
            bb &= BitboardInstance().BetweenBB(checkInfo.checkSq, ksq);
        }

        if (G.TypeOf(pc) == PieceType.Pawn)
        {
            Bitboard doublePawnMaskBB = BitboardInstance().AllBB();
            for (int i = 0; i < SQ_NB()-1; i++)
            {
                if (board[i] == (sideToMove == Color.Black ? Piece.B_Pawn : Piece.W_Pawn)) {
                    int f = i/RANK_NB();
                    doublePawnMaskBB ^= BitboardInstance().FileBB[f];
                }
            }
            //doublePawnMaskBB.show();
            //BitboardInstance().PawnMaskBB[(int)sideToMove].show();
            bb = bb & doublePawnMaskBB & BitboardInstance().PawnMaskBB[(int)sideToMove];

            Color them = G.Not(sideToMove);
            Square thksq = KingSquare(G.Not(sideToMove));
            // 打ち歩詰めのチェック
            Bitboard pe = BitboardInstance().pawnEffect(them, thksq);

            if(bb & pe) {
                Square to = pe.Pop();
                Debug.Log("####");
                Debug.Log(to);
                Debug.Log(thksq);
                Debug.Log(LegalDrop(to));
                if(!LegalDrop(to)) {
                    bb ^= to;
                }
            }

            return bb;
        }
        if (G.TypeOf(pc) == PieceType.Knight)  
            bb &= BitboardInstance().KnightMaskBB[(int)sideToMove];
        if (G.TypeOf(pc) == PieceType.Lance) {
            bb &= BitboardInstance().PawnMaskBB[(int)sideToMove];
        }
        return bb;
    }

    private Square KingSquare(Color c) {
        return kingSquare[(int)c];
    }

    public Bitboard Pieces() {
        return byTypeBB[(int)PieceType.AllPieces];
    }

    public Bitboard Pieces(Color c) {
        return byColorBB[(int)c];
    }

    public Bitboard Pieces(PieceType pt) {
        return byTypeBB[(int)pt];
    }

    public Bitboard Pieces(Color c, PieceType pt) {
        return Pieces(c) & Pieces(pt);
    }


    private Bitboard SliderBlockers(Color c, Square sq) {
        Bitboard snipers =
            ((Pieces(PieceType.Rook) & BitboardInstance().rookStepEffect(sq))
            | (Pieces(PieceType.Bishop) & BitboardInstance().bishopStepEffect(sq))
            | (Pieces(PieceType.Dragon) & BitboardInstance().rookStepEffect(sq))
            | (Pieces(PieceType.Horse) & BitboardInstance().bishopStepEffect(sq))
            | (Pieces(PieceType.Lance) & BitboardInstance().lanceStepEffect(G.Not(c), sq))
            ) & Pieces(c);

        Bitboard blockers = NewBitboard();

        while(snipers) {
            Square s = snipers.Pop();
            if(s == Square.SQ_Invalid) break;
            int [] di = blockers.GetDirection(s, sq);
            s = Sq(FileOf(s) + di[0], RankOf(s) + di[1]);
            int count = 0;
            Square sqCandidate = Square.SQ_Invalid;
            while(sq != s) {
                if(Pieces() & s) { 
                    sqCandidate = s;
                    count++;
                }
                s = Sq(FileOf(s) + di[0], RankOf(s) + di[1]);
            }
            if(count == 1) { blockers |= sqCandidate; }
        }
        return blockers;
    }

    // c no oppnent no kiki
    private void SetAttackersBB(Color c) {
        Bitboard bb = (Bitboard)Pieces(G.Not(c)).Clone();
        Square ksq = KingSquare(sideToMove);
        Bitboard pieces = Pieces();
        Bitboard piecesWithoutKing = pieces ^ ksq;
        attackersBB[(int)c] = NewBitboard();
        attackersBBWithoutKing[(int)c] = NewBitboard();
        while(bb) {
            Square sq = bb.Pop();
            Piece pc = board[(int)sq];
            attackersBB[(int)c] |= BitboardInstance().EffectsFrom(pc, sq, piecesWithoutKing);
            if(G.TypeOf(pc) != PieceType.King) {
                attackersBBWithoutKing[(int)c] |= BitboardInstance().EffectsFrom(pc, sq, pieces);
            }
        }
    }

    private void SetCheckInfo() {
        blockersForKing[(int)Color.Black] = SliderBlockers(Color.White, KingSquare(Color.Black));
        blockersForKing[(int)Color.White] = SliderBlockers(Color.Black, KingSquare(Color.White));

        Square ksq = KingSquare(G.Not(sideToMove));

        // 駒種Xによって敵玉に王手となる升のbitboard

        // 歩であれば、自玉に敵の歩を置いたときの利きにある場所に自分の歩があればそれは敵玉に対して王手になるので、
        // そういう意味で(ksq,them)となっている。

        Bitboard occ = Pieces();
        Color them = G.Not(sideToMove);

        // この指し手が二歩でないかは、この時点でテストしない。指し手生成で除外する。なるべくこの手のチェックは遅延させる。
        checkSquares[(int)PieceType.Pawn] = BitboardInstance().pawnEffect(them, ksq);
        checkSquares[(int)PieceType.Knight] = BitboardInstance().knightEffect(them, ksq);
        checkSquares[(int)PieceType.Silver] = BitboardInstance().silverEffect(them, ksq);
        checkSquares[(int)PieceType.Bishop] = BitboardInstance().bishopEffect(ksq, occ);
        checkSquares[(int)PieceType.Rook] = BitboardInstance().rookEffect(ksq, occ);
        checkSquares[(int)PieceType.Gold] = BitboardInstance().goldEffect(them, ksq);

        // 香で王手になる升は利きを求め直さずに飛車で王手になる升を香のstep effectでマスクしたものを使う。
        checkSquares[(int)PieceType.Lance] = checkSquares[(int)PieceType.Rook] & BitboardInstance().lanceStepEffect(them, ksq);

        // 王を移動させて直接王手になることはない。それは自殺手である。
        checkSquares[(int)PieceType.King] = NewBitboard();

        // 成り駒。この初期化は馬鹿らしいようだが、gives_check()は指し手ごとに呼び出されるので、その処理を軽くしたいので
        // ここでの初期化は許容できる。(このコードはdo_move()に対して1回呼び出されるだけなので)
        checkSquares[(int)PieceType.Pro_Pawn] = checkSquares[(int)PieceType.Gold];
        checkSquares[(int)PieceType.Pro_Lance] = checkSquares[(int)PieceType.Gold];
        checkSquares[(int)PieceType.Pro_Knight] = checkSquares[(int)PieceType.Gold];
        checkSquares[(int)PieceType.Pro_Silver] = checkSquares[(int)PieceType.Gold];
        checkSquares[(int)PieceType.Horse] = checkSquares[(int)PieceType.Bishop] | BitboardInstance().kingEffect(ksq);
        checkSquares[(int)PieceType.Dragon] = checkSquares[(int)PieceType.Rook] | BitboardInstance().kingEffect(ksq);
    }

    private void XorPiece(Square sq, Piece pc) {
        byTypeBB[(int)PieceType.AllPieces] ^= BitboardInstance().SquareBB[(int)sq];
        byTypeBB[(int)G.TypeOf(pc)] ^= BitboardInstance().SquareBB[(int)sq];
        byColorBB[(int)G.ColorOf(pc)] ^= BitboardInstance().SquareBB[(int)sq];
    }

    private void RemovePiece(Square sq) {
        Piece pc = board[(int)sq];
        board[(int)sq] = Piece.No_Piece;
        XorPiece(sq, pc);
    }

    private void PutPiece(Square sq, Piece pc) {
        board[(int)sq] = pc;
        XorPiece(sq, pc);
    }

    private bool Discovered(Square from, Square to, Square ourKing, Bitboard pinned) {
        return (bool)pinned
            && (bool)(pinned & from)
            && !pinned.Aligned(from, to, ourKing);
    }

    public void DoMove(Move m)
    {
        bool givesCheck = GivesCheck(m);
        checkersBB = NewBitboard();
        if (m.isDrop())
        {
            if (sideToMove == Color.Black)
            {
                blackHand[G.HandIndexOf(m.movedPiece())]--;
            }
            else
            {
                whiteHand[G.HandIndexOf(m.movedPiece())]--;
            }
            PutPiece(m.to(), m.movedPiece());
            if(givesCheck) {
                checkersBB |= m.to();
            }
        }
        else
        {
            Piece pcFrom = board[(int)m.from()];
            Piece pcTo = board[(int)m.to()];
            if (pcTo != Piece.No_Piece)
            {
                if (sideToMove == Color.Black)
                {
                    blackHand[G.HandIndexOf(G.Inverse(pcTo))]++;
                }
                else
                {
                    whiteHand[G.HandIndexOf(G.Inverse(pcTo))]++;
                }
                RemovePiece(m.to());
            }

            RemovePiece(m.from());
            PutPiece(m.to(), m.isPromote() ? G.Promoted(pcFrom) : pcFrom);
            if(G.TypeOf(m.movedPiece()) == PieceType.King) {
                kingSquare[(int)sideToMove] = m.to();
            }
            if(givesCheck) {
                checkersBB = checkSquares[(int)G.TypeOf(m.movedPieceAfter())] & m.to();
                if(Discovered(m.from(), m.to(), kingSquare[(int)G.Not(sideToMove)], blockersForKing[(int)G.Not(sideToMove)] & m.from())) {
                    int [] di = checkersBB.GetDirection(kingSquare[(int)G.Not(sideToMove)], m.from());
                    int f = FileOf(m.from()) + di[0];
                    int r = RankOf(m.from()) + di[1];
                    Bitboard b = Pieces() & Sq(f, r);
                    while(!(bool)b) {
                        f += di[0];
                        r += di[1];
                        b = Pieces() & Sq(f, r);
                    }
                    checkersBB |= b;
                }
            }
        }
        moves.Add(m);

        sideToMove = (sideToMove == Color.Black ? Color.White : Color.Black);
        SetCheckInfo();
        SetAttackersBB(sideToMove);
        SetAttackersBB(G.Not(sideToMove));
    }

    public bool InCheck() {
        return checkersBB;
    }

    public bool GivesCheck(Move m) {
        Piece p = m.movedPieceAfter();
        Square to = m.to();
        Square from = m.from();
        if(checkSquares[(int)G.TypeOf(p)] & to) 
            return true;
        return !m.isDrop()
            && (((blockersForKing[(int)G.Not(sideToMove)] /*& pieces(sideToMove)*/) & from)
            && !NewBitboard().Aligned(from, to, KingSquare(G.Not(sideToMove))));
    }

    public bool IsEnableToCaptureChecker() {
        CheckInfo checkInfo = GetCheckInfo();
        Bitboard pieces = (Bitboard)Pieces(sideToMove).Clone();
        Square sq = pieces.Pop();
        while(sq != Square.SQ_Invalid) {
            if(MovableBB(board[(int)sq], sq) & checkInfo.checkSq) return true;
            sq = pieces.Pop();
        }
        return false;
    }

    public bool IsMated() {

        if(InCheck()) {
            Square ksq = kingSquare[(int)sideToMove];
            // evasion
            Bitboard bb = BitboardInstance().kingEffect(ksq) & ~(Pieces(sideToMove) | attackersBB[(int)sideToMove]);

            if(bb) return false;
            // enkaku oute
            CheckInfo checkInfo = GetCheckInfo();
            if(checkInfo.isDoubleCheck) return true;
            // oute no koma wo toru
            //attackersBBWithoutKing[(int)G.Not(sideToMove)].show();
            //attackersBB[(int)G.Not(sideToMove)].show();

            var isEnableToCaptureChecker = IsEnableToCaptureChecker();
            // aigoma
            var isEnableToIdouAi = (bool)(BitboardInstance().BetweenBB(ksq, checkInfo.checkSq) & attackersBBWithoutKing[(int)G.Not(sideToMove)]);
            var isEnableToDrop = (bool)(BitboardInstance().BetweenBB(ksq, checkInfo.checkSq) & DropableBB(sideToMove));

            //BitboardInstance().BetweenBB(ksq, checkInfo.checkSq).show();
            //attackersBBWithoutKing[(int)G.Not(sideToMove)].show();

            //Debug.Log("isEnableToCaptureChecker: " + isEnableToCaptureChecker);
            //Debug.Log("isEnableToIdouAi: " + isEnableToIdouAi);
            //Debug.Log("isEnableToDrop: " + isEnableToDrop);
            return !(isEnableToCaptureChecker || isEnableToIdouAi || isEnableToDrop);
        }
        return false;
    }


    public CheckInfo GetCheckInfo() {
        CheckInfo ci = new CheckInfo();
        int checkersCnt = 0;
        Bitboard checkers = (Bitboard)checkersBB.Clone();
        Square checkSq = Square.SQ_Invalid;
        do
        {
            checkSq = checkers.Pop();
            checkersCnt++;
        } while (checkers);
        if(checkersCnt > 1) ci.isDoubleCheck = true;
        ci.checkSq = checkSq;
        return ci;
    }

    public void ResetEmphasizedCells() {
        for (int i = 0; i < SQ_NB()-1; i++) {
            emphasizedCells[i] = false;
        }
    }

    public bool isNoHand(Color c) {
        for (int i = 0; i < 7; i++)
        {
            if (c == Color.Black)
            {
                if (blackHand[i] > 0) return false;
            }
            else
            {
                if (blackHand[i] > 0) return false;
            }
        }
        return true;
    }

    public Bitboard PawnDroppableBB(Color c) {

        Bitboard bb = ~Pieces();

        Bitboard doublePawnMaskBB = BitboardInstance().AllBB();
        for (int i = 0; i < SQ_NB()-1; i++)
        {
            if (board[i] == (c == Color.Black ? Piece.B_Pawn : Piece.W_Pawn))
            {
                int f = i / RANK_NB();
                doublePawnMaskBB ^= BitboardInstance().FileBB[f];
            }
        }
        return (bb & doublePawnMaskBB & BitboardInstance().PawnMaskBB[(int)c]);
    }

    public Bitboard KnightDroppableBB(Color c) {
        Bitboard bb = ~Pieces();
        return (bb & BitboardInstance().KnightMaskBB[(int)c]);
    }

    public Bitboard LanceDroppableBB(Color c) {
        Bitboard bb = ~Pieces();
        return (bb & BitboardInstance().PawnMaskBB[(int)c]);
    }

    // 打ち歩詰めのチェック
    private bool LegalDrop(Square to) {
        Color them = G.Not(sideToMove);

        // 歩を支える自駒がなければ歩を取れるので true
        if(!(attackersBB[(int)them] & to)) return true;

        Bitboard b = AttackersToPawn(them, to);
        Bitboard pinned = blockersForKing[(int)them];

        if(b & (~pinned | BitboardInstance().FileBB[FileOf(to)])) {
            return true;
        }

        Bitboard escape_bb = BitboardInstance().kingEffect(kingSquare[(int)them]) & ~Pieces(them);
        escape_bb ^= to;

        Bitboard bb = (Bitboard)Pieces(G.Not(them)).Clone();
        //Square ksq = KingSquare(sideToMove);

        // 歩を打つことで効きが遮られて移動できる箇所が増える可能性があるので敵の効きを再計算する。
        Bitboard pieces = Pieces();
        pieces |= to;
        var attackersBBnow = NewBitboard();
        while(bb) {
            Square sq = bb.Pop();
            Piece pc = board[(int)sq];
            attackersBBnow |= BitboardInstance().EffectsFrom(pc, sq, pieces);
        }

        //escape_bb.show();
        //Debug.Log("-- attackersBBs -- ");
        //(attackersBB[(int)sideToMove]).show();
        //(attackersBB[(int)them]).show();
        //(~attackersBB[(int)them]).show();
        //Debug.Log("escape_bb: ");
        //(escape_bb).show();
        //(~attackersBBnow).show();
        //(attackersBBnow).show();
        if(escape_bb & ~attackersBBnow) return true;

        return false;
    }

    private Bitboard AttackersToPawn(Color c, Square pawn_sq) {
        Color them = G.Not(c);
        Bitboard occ = Pieces();
        Bitboard bb_hd = BitboardInstance().kingEffect(pawn_sq) & (Pieces(PieceType.Horse) | Pieces(PieceType.Dragon));

        return (
            Pieces(c) &
            (
                (BitboardInstance().knightEffect(them, pawn_sq) & Pieces(PieceType.Knight)) |
                (BitboardInstance().silverEffect(them, pawn_sq) & (Pieces(PieceType.Silver) | bb_hd)) |
                (BitboardInstance().goldEffect(them, pawn_sq) & (Pieces(PieceType.Gold) | Pieces(PieceType.Pro_Pawn) | Pieces(PieceType.Pro_Lance) | Pieces(PieceType.Pro_Knight) | Pieces(PieceType.Pro_Silver) | bb_hd)) |
                (BitboardInstance().bishopEffect(pawn_sq, occ) & (Pieces(PieceType.Bishop) | Pieces(PieceType.Horse))) |
                (BitboardInstance().rookEffect(pawn_sq, occ) & (Pieces(PieceType.Rook) | Pieces(PieceType.Dragon)))
            )
        );
    }

    public (List<FPieceAndSquare>, List<FPieceAndSquare>) PieceLists() {

        var player1PieceList = new List<FPieceAndSquare>();
        var player2PieceList = new List<FPieceAndSquare>();

        for (int i = 0; i < SQ_NB()-1; i++)
        {
            if (board[i] != Piece.No_Piece)
            {
                var pieceAndSquare = new FPieceAndSquare(
                        G.TypeOf(board[i]),
                        SquareToDesignSquare(i));

                if(G.ColorOf(board[i]) == myColor) {
                    player1PieceList.Add(pieceAndSquare);
                } else {
                    player2PieceList.Add(pieceAndSquare);
                }
            }
        }
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 7; i++)
            {
                Color c = (j == 0 ? Color.Black : Color.White);
                int count = (c == Color.Black ? blackHand[i] : whiteHand[i]);
                for (int k = 0; k < count; k++)
                {
                    Piece pc = G.HandIndexToPiece(i, c);

                    var pieceAndSquare = new FPieceAndSquare(
                            G.TypeOf(pc),
                            (Square)Square.SQ_Invalid);

                    if(c == myColor) {
                        player1PieceList.Add(pieceAndSquare);
                    } else {
                        player2PieceList.Add(pieceAndSquare);
                    }
                }
            }
        }
        return (player1PieceList, player2PieceList);
    }

    public bool CanPromote(Piece pc)
    {
        return (int)G.TypeOf(pc) >= 1 && (int)G.TypeOf(pc) <= 6;
    }

    public bool CanPromote(Piece pc, Square from_sq, Square to_sq, Color c)
    {
        if (!CanPromote(pc)) return false;
        if (c == Color.Black)
        {
            return RankOf(from_sq) < N_RANK_PROMOTE() || RankOf(to_sq) < N_RANK_PROMOTE() ;
        }
        else
        {
            return RankOf(from_sq) > RANK_NB()-(N_RANK_PROMOTE()+1) || RankOf(to_sq) > RANK_NB()-(N_RANK_PROMOTE()+1);
        }
    }

    public bool MustPromote(Piece pc, Square sq, Color c)
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
                return RankOf(sq) > RANK_NB()-3;
            }
            if(pc == Piece.W_Lance || pc == Piece.W_Pawn) {
                return RankOf(sq) > RANK_NB()-2;
            }
        }
        return false;
    }

}

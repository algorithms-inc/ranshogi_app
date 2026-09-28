using UnityEngine;
using UnityEngine.EventSystems;

public class PieceController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    PieceView view;

    public Piece piece;
    public PieceType pieceType;
    public Color c;
    public Color myColor;
    public bool inHand = false;
    private Square _sq;
    GameController gameController;

    private void Awake()
    {
        view = GetComponent<PieceView>();
    }

    void Start()
    {
        gameController = (GameController)FindObjectOfType(typeof(GameController));
    }

    public void Init(Piece pc, Square sq, Color mc)
    {
        piece = pc;
        pieceType = G.TypeOf(pc);
        c = G.ColorOf(pc);
        myColor = mc;
        view.UpdatePieceAppearance(pc, mc);
        _sq = sq;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetInHand()
    {
        pieceType = G.TypeOf(piece);
        c = G.ColorOf(piece);
        view.UpdatePieceAppearance(piece, myColor);
        _sq = Square.SQ_Invalid;
        inHand = true;
    }

    public void Captured()
    {
        piece = G.RawOf(G.Inverse(piece));
    }

    public void DoMove(Move m)
    {
        _sq = m.to();
        if (m.isDrop())
        {
            inHand = false;
        }
        if (m.isPromote())
        {
            piece = G.Promoted(piece);
            c = G.ColorOf(piece);
            view.UpdatePieceAppearance(piece, myColor);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (inHand)
        {
            //gameController.onTap(piece);
        }
        else
        {
            gameController.OnTap(_sq);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.AddressableAssets;


public class PieceView : MonoBehaviour
{

    private int x11 = 215;
    private int y11 = 208;
    private int width = 103;

    private RectTransform pieceTransform;
    private Image pieceImage;

    private void Awake()
    {
        pieceTransform = GetComponent<RectTransform>();
        pieceImage = GetComponent<Image>();
    }

    public void UpdatePieceAppearance(Piece piece, Color myColor)
    {
        if (piece == Piece.No_Piece) return;
        string spriteName;
        if (piece == Piece.W_King)
        {
            spriteName = "White_King";
        }
        else if (piece == Piece.B_King)
        {
            spriteName = "Black_King";
        }
        else
        {
            spriteName = G.TypeOf(piece).ToString();
        }


        //Addressables
        //    .LoadAssetAsync<Sprite>("Assets/Sprites/Game/" + spriteName + ".png")
        //    .Completed += op => {
        //        pieceImage.sprite = op.Result;
        //    };

        if (G.ColorOf(piece) == myColor)
        {
            pieceTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            pieceTransform.localRotation = Quaternion.Euler(0, 0, 180);
        }


    }

    public void Promote(Piece pc)
    {
    }

}

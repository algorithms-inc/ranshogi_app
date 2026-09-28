using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CellController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    Square sq;
    Image image;
    public bool emphasized = false;
    GameController gameController;

    [SerializeField] TextMeshProUGUI checkText;

    private void Start()
    {
        image = GetComponent<Image>();
        gameController = (GameController)FindObjectOfType(typeof(GameController));

        checkText.transform.localPosition = new Vector3(0, -65, 0);
        checkText.rectTransform.sizeDelta = new Vector2(Screen.width/6, 50);
    }

    public void Init(Square squ) {
        sq = squ;
    }

    public void emphasize()
    {
        emphasized = true;
        image.color = new Color32(255, 0, 0, 96);
    }

    public void reset()
    {
        emphasized = false;
        image.color = new Color32(255, 255, 255, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameController.OnTap(sq);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void EmphasizeForAShortTime() {
        image.color = new Color32(180, 0, 0, 96);
        Invoke("reset", 1);
    }

    public void ShowCheck() {
        checkText.transform.SetSiblingIndex(2);
        checkText.gameObject.SetActive(true);
        Invoke("HideCheckText", 1);
    }

    public void HideCheckText() {
        checkText.gameObject.SetActive(false);
    }

}

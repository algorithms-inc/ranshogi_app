using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingViewController : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button button;

    public delegate void OnButtonClicked();

    OnButtonClicked onButtonClickedHandler;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(string message, bool withButton, OnButtonClicked onButtonClicked) {
        text.text = message;
        onButtonClickedHandler = onButtonClicked;
        button.gameObject.SetActive(withButton);
        button.onClick.AddListener(delegate { onButtonClickedHandler(); });
    }


}

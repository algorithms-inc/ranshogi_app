using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HistoryCell : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI blackNicknameText;
    [SerializeField] TextMeshProUGUI whiteNicknameText;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] Button detailButton;

    Game game;

    public delegate void ButtonTapped();

    // Start is called before the first frame update
    void Start()
    {
        detailButton.onClick.AddListener(delegate {
            GameManager.instance.globalVariables.gameForHistoryDetail = this.game;
            SceneManager.LoadScene("HistoryDetail");
        });

    }

    public void Populate(Game game) {
        blackNicknameText.text = game.blackNickname;
        whiteNicknameText.text = game.whiteNickname;
        switch(game.gameResult) {
            case GameResult.BlackWon:
                resultText.text = "先手勝ち";
                break;
            case GameResult.WhiteWon:
                resultText.text = "後手勝ち";
                break;
            case GameResult.Draw:
                resultText.text = "引き分け";
                break;
        }
        this.game = game;
    }
}

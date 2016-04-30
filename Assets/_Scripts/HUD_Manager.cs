using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD_Manager : MonoBehaviour
{
    public static HUD_Manager hud;
    public Canvas menuHUD;
    public Canvas gameHUD;
    public Text moveText;
    public Text scoreText;

    //it is called when the play button is clicked
    public void Play()
    {
        hud = this;
        menuHUD.transform.gameObject.SetActive(false);
        gameHUD.transform.gameObject.SetActive(true);
        Board.board.StartGame();
    }

    //used to refresh the HUD
    public void UpdateHUD()
    {
        moveText.text = Board.playerMove.ToString();
        scoreText.text = Board.playerScore.ToString();
    }
}

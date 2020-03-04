using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    static GameManager gameManager;
    int Score = 0;
    public bool gameOver = false;

    public Canvas UICanvas;
    public Text gameOverText;
    public Text scoreText;
    Text DebugText;
    PlayerControl player;
    // Start is called before the first frame update

    void Awake() {
        gameManager = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        scoreText.text = "Score: 0";
        DebugText = GameObject.Find("DebugText").GetComponent<Text>();
    }

    public void SetGameOver() {
        Debug.Log("Game Over");
        gameOver = true;
        gameOverText.enabled = true;
    }

    public void GetPoints(int points) {
        Score += points;
        scoreText.text = "Score: " + Score.ToString();
    }

    public void SetDebugText(string debugText) {
        DebugText.text = debugText;
    }

    public static GameManager GetInstance() {
        return gameManager;
    }
}

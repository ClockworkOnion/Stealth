using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    static GameManager gameManager;
    int Score = 0;
    public bool gameOver = false;

    public Canvas UICanvas;
    public Canvas gameLostCanvas, gameWonCanvas;
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
        gameLostCanvas = GameObject.Find("LostCanvas").GetComponent<Canvas>();
        gameLostCanvas.enabled = false;
        gameWonCanvas = GameObject.Find("WonCanvas").GetComponent<Canvas>();
        gameWonCanvas.enabled = false;
        scoreText.text = "Score: 0";
        DebugText = GameObject.Find("DebugText").GetComponent<Text>();



    }

    public void SetGameLost() {
        Debug.Log("Game Over");
        gameOver = true;
        Stopwatch.GetInstance().SetPause(true);
        gameLostCanvas.enabled = true;
    }

    public void SetGameWon() {
        gameOver = true;
        Stopwatch.GetInstance().SetPause(true);
        gameWonCanvas.enabled = true;
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

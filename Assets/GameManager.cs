using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    int Score = 0;

    public Canvas UICanvas;
    public Text gameOverText;
    public Text scoreText;
    PlayerControl player;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        scoreText.text = "Score: 0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameOver() {
        Debug.Log("Game Over");
        player.gameOver = true;
        gameOverText.enabled = true;
    }

    public void GetPoints(int points) {
        Score += points;
        scoreText.text = "Score: " + Score.ToString();
    }
}

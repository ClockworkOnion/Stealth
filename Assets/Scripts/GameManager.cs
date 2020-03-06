using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    static GameManager gameManager;
    public bool gameOver = false;
    public bool gamePaused = false;

    public Canvas UICanvas;
    Canvas gameLostCanvas, gameWonCanvas, gamePausedCanvas;
    Text moneyText;
    Text DebugText;
    PlayerControl player;
    // Start is called before the first frame update

    void Awake() {
        gameManager = this;

        if (!SceneManager.GetSceneByBuildIndex(0).isLoaded) {
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        
        gameLostCanvas = GameObject.Find("LostCanvas").GetComponent<Canvas>();
        gameLostCanvas.enabled = false;
        gameWonCanvas = GameObject.Find("WonCanvas").GetComponent<Canvas>();
        gameWonCanvas.enabled = false;
        gamePausedCanvas = GameObject.Find("PauseMenuCanvas").GetComponent<Canvas>();
        gamePausedCanvas.enabled = false;

        
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        moneyText.text = "Money: " + GlobalManager.GetInstance().GetMoney();

        DebugText = GameObject.Find("DebugText").GetComponent<Text>();
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver) {
            TogglePause();
        }

    }

    public void SetGameLost() {
        if (!gameOver) {
            Debug.Log("Game Over");
            gameOver = true;
            gameLostCanvas.enabled = true;
        }
    }

    public void SetGameWon() {
        if (!gameOver) {
            gameOver = true;
            gameWonCanvas.enabled = true;
            GlobalManager.GetInstance().LevelCompleted(SceneManager.GetActiveScene().buildIndex-2);
        }
    }

    public void SetDebugText(string debugText) {
        DebugText.text = debugText;
    }

    public static GameManager GetInstance() {
        return gameManager;
    }

    public void NextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1, LoadSceneMode.Single);
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene(1);
    }

    public void SetMoneyText(int amount) {
        moneyText.text = "Money: " + amount;
    }

    public void TogglePause() {
        gamePaused = !gamePaused;
        gamePausedCanvas.enabled = gamePaused;
    }
}

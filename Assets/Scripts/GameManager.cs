using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[DefaultExecutionOrder(10)]
public class GameManager : MonoBehaviour
{
    static GameManager gameManager;
    public bool gameOver = false;
    public bool gamePaused = false;
    public float levelTimer;

    // Player Variablen
    public bool playerIsCloaked;

    public Canvas UICanvas;
    Canvas gameLostCanvas, gameWonCanvas, gamePausedCanvas, shopCanvas;
    Text moneyText;
    Text DebugText;
    PlayerControl player;
    GuardAI[] guardsList;
    public bool playerIsHunted;

    [Header("Item Amount Texts")]
    public TextMeshProUGUI cloakingDeviceAmount;
    public TextMeshProUGUI smokeBombAmount;
    public TextMeshProUGUI glueAmount;
    public TextMeshProUGUI stoneAmount;

    [Header("Shop Buttons")]
    public Button BuyCloakingDevice;
    public Button BuySmokeBomb;
    public Button BuyGlue;
    public Button BuyStone;

    bool timeScalePause;


    void Awake() {
        gameManager = this;

        if (!SceneManager.GetSceneByBuildIndex(0).isLoaded) {
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
    }

    void Start()
    {
        // Referenzen
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        gameLostCanvas = GameObject.Find("LostCanvas").GetComponent<Canvas>();
        gameLostCanvas.enabled = false;
        gameWonCanvas = GameObject.Find("WonCanvas").GetComponent<Canvas>();
        gameWonCanvas.enabled = false;
        gamePausedCanvas = GameObject.Find("PauseMenuCanvas").GetComponent<Canvas>();
        gamePausedCanvas.enabled = false;
        shopCanvas = GameObject.Find("ShopCanvas").GetComponent<Canvas>();
        shopCanvas.enabled = false;

        guardsList = GameObject.FindObjectsOfType<GuardAI>();
        
        // Items / Shop / Geld
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        moneyText.text = "Credits: " + GlobalManager.GetInstance().GetMoney();
        RefreshItemCount();

        // Debug
        DebugText = GameObject.Find("DebugText").GetComponent<Text>();

        // Timer setzen
        Stopwatch.GetInstance().SetTimer(levelTimer);

        // Shop am Levelanfang öffnen
        ToggleShopMenu();
        // Invoke("ToggleShopMenu",0.01f);
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && !shopCanvas.enabled) {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S) && !gameOver && !gamePausedCanvas.enabled && GlobalManager.GetInstance().cheatsActivated) {
            ToggleShopMenu(); // TODO : Am Ende rausnehmen, dass der Spieler den Shop öffnen kann
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            GlobalManager.GetInstance().cheatsActivated = !GlobalManager.GetInstance().cheatsActivated;
            Debug.Log("Cheats " + (GlobalManager.GetInstance().cheatsActivated ? "activated" : "deactivated"));
        }

        if (Input.GetKeyDown(KeyCode.G) && GlobalManager.GetInstance().cheatsActivated) {
            GlobalManager.GetInstance().AddMoney(100);
            GameManager.GetInstance().SetMoneyText(GlobalManager.GetInstance().GetMoney());
        }

        if (Input.GetKeyDown(KeyCode.L) && GlobalManager.GetInstance().cheatsActivated) {
            GlobalManager.GetInstance().UnlockProgression();
            Debug.Log("All levels unlocked");
        }


        if (gamePaused) {Time.timeScale = 0;} else { Time.timeScale = 1;}


        //////// Spieler gesehen? ///////////////
        playerIsHunted = false; // zurücksetzen...
        foreach(GuardAI guard in guardsList) {
            if (guard.guardState == GuardAI.State.pursuing || guard.guardState == GuardAI.State.searching)
                playerIsHunted = true;
        }
    }

    public void SetGameLost() {
        if (!gameOver) {
            Debug.Log("Game Over");
            GlobalManager.GetInstance().LevelLost();
            gameLostCanvas.GetComponentInChildren<Animator>().SetTrigger("ShowText");
            gameOver = true;
            gameLostCanvas.enabled = true;
        }
    }

    public void SetGameWon() {
        if (!gameOver) {
            if (SceneManager.GetActiveScene().buildIndex >= 4) {
                GameObject.Find("NextLevelButton").GetComponent<Button>().interactable = false;
                SceneManager.LoadScene("FinishingScene");
            }
            gameOver = true;
            gameWonCanvas.enabled = true;
            gameWonCanvas.GetComponentInChildren<Animator>().SetTrigger("displayText");
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
        moneyText.text = "Credits: " + amount;
    }

    public void TogglePause() {
        gamePaused = !gamePaused;
    }

    public void TogglePauseMenu() {
        gamePaused = !gamePaused;
        gamePausedCanvas.enabled = gamePaused;
    }


    /////// Shop Methoden ///////////////////////

    public void BuyItem(ItemObject buyItem) {
        GlobalManager.GetInstance().AddItem(buyItem.itemEnum);
        GlobalManager.GetInstance().SubtractMoney(buyItem.price);
        CheckCreditsForItems();
        RefreshItemCount();
    }

    public void RefreshItemCount() {
       Dictionary<PlayerItems, int> inventory = GlobalManager.GetInstance().GetInventoryAsMap();
       
       int tempInt;

       inventory.TryGetValue(PlayerItems.cloakingDevice, out tempInt);
       cloakingDeviceAmount.text = tempInt.ToString();

       inventory.TryGetValue(PlayerItems.smokeBomb, out tempInt);
       smokeBombAmount.text = tempInt.ToString();

       inventory.TryGetValue(PlayerItems.glue, out tempInt);
       glueAmount.text = tempInt.ToString();

       inventory.TryGetValue(PlayerItems.stone, out tempInt);
       stoneAmount.text = tempInt.ToString();
   }

   public void ToggleShopMenu() {

        gamePaused = !gamePaused;
        shopCanvas.enabled = gamePaused;

        if (shopCanvas.enabled) {
            CheckCreditsForItems();
            BuyStone.Select();
        }
            
   }

    public void CheckCreditsForItems() {
       int credits = GlobalManager.GetInstance().GetMoney();
       BuyCloakingDevice.interactable = (credits >= 150) ? true : false;
       BuySmokeBomb.interactable = (credits >= 80) ? true : false;
       BuyGlue.interactable = (credits >= 50) ? true : false;
       BuyStone.interactable = (credits >= 10) ? true : false;
    }

    public GuardAI[] GetGuardAIs() {
        return guardsList;
    }

    public void SetPlayerCloak(bool cloaked) {
        playerIsCloaked = cloaked;
    }

    public void ToggleTimescalePause() {
        if (Time.timeScale >= 0.9f) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

}


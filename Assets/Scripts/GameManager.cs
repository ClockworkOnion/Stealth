using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    static GameManager gameManager;
    public bool gameOver = false;
    public bool gamePaused = false;

    // Player Variablen
    public bool playerIsCloaked;

    public Canvas UICanvas;
    Canvas gameLostCanvas, gameWonCanvas, gamePausedCanvas, shopCanvas;
    Text moneyText;
    Text DebugText;
    PlayerControl player;

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
        
        // Items / Shop / Geld
        moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        moneyText.text = "Money: " + GlobalManager.GetInstance().GetMoney();
        RefreshItemCount();

        // Debug
        DebugText = GameObject.Find("DebugText").GetComponent<Text>();
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && !shopCanvas.enabled) {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.S) && !gameOver && !gamePausedCanvas.enabled) {
            ToggleShopMenu();
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
        }
   }

    public void CheckCreditsForItems() {
       int credits = GlobalManager.GetInstance().GetMoney();
       BuyCloakingDevice.interactable = (credits >= 150) ? true : false;
       BuySmokeBomb.interactable = (credits >= 80) ? true : false;
       BuyGlue.interactable = (credits >= 50) ? true : false;
       BuyStone.interactable = (credits >= 10) ? true : false;
    }

}


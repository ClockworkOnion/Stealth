using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{

    public static readonly int NUMBER_OF_LEVELS = 3;
    static GlobalManager instance;
    bool[] completedLevels;

    Dictionary<PlayerItems, int> itemMap;
    const int START_MONEY = 500;
    int money;

    void Awake() {
        if (instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        ResetProgression();

    }

    void Start()
    {
        DontDestroyOnLoad(this);

       if (SceneManager.GetActiveScene().buildIndex == 0) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
       }


    }

    public static GlobalManager GetInstance() {
        return instance;
    }

    public void LevelCompleted(int stageIndex) {
        completedLevels[stageIndex] = true;
    }

    public void ResetProgression() {
        completedLevels = new bool[NUMBER_OF_LEVELS];
        for (int i = 0; i < completedLevels.Length; i++)
        {
            completedLevels[i] = false;
        }
        money = START_MONEY;

        itemMap = new Dictionary<PlayerItems, int>();
        itemMap.Add(PlayerItems.cloakingDevice, 0);
        itemMap.Add(PlayerItems.smokeBomb, 0);
        itemMap.Add(PlayerItems.glue, 0);
        itemMap.Add(PlayerItems.stone, 0);
    }

    public bool GetLevelCompleted(int stageIndex) {
        return completedLevels[stageIndex];
    }

    public Dictionary<PlayerItems, int> GetInventoryAsMap() {
        return itemMap;
    }

    public int GetMoney() {
        return money;
    }

    public void AddMoney(int amount) {
        money += amount;
        GameManager.GetInstance().SetMoneyText(money);
        
    }

    public void SubtractMoney(int amount) {
        money -= amount;
        GameManager.GetInstance().SetMoneyText(money);
    }

   
    /////// Shop Methoden    ////////////////////////////////////////

    public void AddItem(PlayerItems items) {
        itemMap[items] += 1;
    }

    public void SubtractItem(PlayerItems items) {
        if (itemMap[items] > 0) {
            itemMap[items] -= 1;
        } else {
            Debug.Log("Fehler beim Item abziehen");
        }
    }

}



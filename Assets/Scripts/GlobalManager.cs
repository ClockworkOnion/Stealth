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

    Dictionary<PlayerItems, int> savedItemMap;
    Dictionary<PlayerItems, int> itemMapInCurrentLevel;

    const int START_MONEY = 100;
    int savedMoney;
    int MoneyInCurrentRound;

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
        savedMoney = MoneyInCurrentRound;
        savedItemMap = copyOfCurrentItemMap();

        completedLevels[stageIndex] = true;
    }

    public void LevelLost()
    {
        MoneyInCurrentRound = savedMoney;
        itemMapInCurrentLevel = copyOfSavedItemMap();
    }

    public void ResetProgression() {
        completedLevels = new bool[NUMBER_OF_LEVELS];
        for (int i = 0; i < completedLevels.Length; i++)
        {
            completedLevels[i] = false;
        }
        savedMoney = START_MONEY;
        MoneyInCurrentRound = START_MONEY;

        savedItemMap = new Dictionary<PlayerItems, int>();
        savedItemMap.Add(PlayerItems.cloakingDevice, 0);
        savedItemMap.Add(PlayerItems.smokeBomb, 0);
        savedItemMap.Add(PlayerItems.glue, 0);
        savedItemMap.Add(PlayerItems.stone, 0);

        itemMapInCurrentLevel = new Dictionary<PlayerItems, int>();
        itemMapInCurrentLevel.Add(PlayerItems.cloakingDevice, 0);
        itemMapInCurrentLevel.Add(PlayerItems.smokeBomb, 0);
        itemMapInCurrentLevel.Add(PlayerItems.glue, 0);
        itemMapInCurrentLevel.Add(PlayerItems.stone, 0);
    }

    public bool GetLevelCompleted(int stageIndex) {
        return completedLevels[stageIndex];
    }

    public Dictionary<PlayerItems, int> GetInventoryAsMap() {
        return itemMapInCurrentLevel;
    }

    public int GetMoney() {
        return MoneyInCurrentRound;
    }

    public void AddMoney(int amount) {
        MoneyInCurrentRound += amount;
        GameManager.GetInstance().SetMoneyText(MoneyInCurrentRound);
        
    }

    public void SubtractMoney(int amount) {
        MoneyInCurrentRound -= amount;
        GameManager.GetInstance().SetMoneyText(MoneyInCurrentRound);
    }


    /////// Shop Methoden    ////////////////////////////////////////

    public void AddItem(PlayerItems items) {
        itemMapInCurrentLevel[items] += 1;
    }

    public void SubtractItem(PlayerItems items) {
        if (itemMapInCurrentLevel[items] > 0) {
            itemMapInCurrentLevel[items] -= 1;
        } else {
            Debug.Log("Fehler beim Item abziehen");
        }
    }


    //////////// Hilfsmethoden /////////////////////
    private Dictionary<PlayerItems, int> copyOfCurrentItemMap()
    {
        Dictionary<PlayerItems, int> copy = new Dictionary<PlayerItems, int>();
        copy.Add(PlayerItems.cloakingDevice, itemMapInCurrentLevel[PlayerItems.cloakingDevice]);
        copy.Add(PlayerItems.smokeBomb, itemMapInCurrentLevel[PlayerItems.smokeBomb]);
        copy.Add(PlayerItems.glue, itemMapInCurrentLevel[PlayerItems.glue]);
        copy.Add(PlayerItems.stone, itemMapInCurrentLevel[PlayerItems.stone]);

        return copy;
    }

    private Dictionary<PlayerItems, int> copyOfSavedItemMap()
    {
        Dictionary<PlayerItems, int> copy = new Dictionary<PlayerItems, int>();
        copy.Add(PlayerItems.cloakingDevice, savedItemMap[PlayerItems.cloakingDevice]);
        copy.Add(PlayerItems.smokeBomb, savedItemMap[PlayerItems.smokeBomb]);
        copy.Add(PlayerItems.glue, savedItemMap[PlayerItems.glue]);
        copy.Add(PlayerItems.stone, savedItemMap[PlayerItems.stone]);

        return copy;
    }

}



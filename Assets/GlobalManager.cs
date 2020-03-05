using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{

    static readonly int NUMBER_OF_LEVELS = 3;
    static GlobalManager instance;
    bool[] completedLevels = new bool[NUMBER_OF_LEVELS];


    void Awake() {
        if (instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
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
    }
}


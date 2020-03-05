using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{

    static GlobalManager instance;


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
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public static GlobalManager GetInstance() {
        return instance;
    }
}


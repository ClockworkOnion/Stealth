using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    public static GlobalManager GetInstance() {
        return instance;
    }
}


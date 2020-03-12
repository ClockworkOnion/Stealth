﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGoal : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider) {
        if (GameManager.GetInstance().playerIsHunted)
            return;
        if (collider.gameObject.tag=="Player") {
            GameManager.GetInstance().SetGameWon();
        }
    }
}

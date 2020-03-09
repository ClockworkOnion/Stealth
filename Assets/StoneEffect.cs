﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneEffect : MonoBehaviour
{
    bool effectUsed;  // wird true, wenn der Stein irgendwo gegen geprallt ist

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag=="Player"){
            GlobalManager.GetInstance().AddItem(PlayerItems.stone);
            Destroy(this.gameObject);
        } else if (!effectUsed) {
            GuardAI[] guardsList = GameManager.GetInstance().GetGuardAIs();
            foreach (GuardAI guard in guardsList)
            {
                // Ablenkung
                if (Vector3.Distance(guard.transform.position, transform.position) < 5) {
                    guard.SetDestination(this.transform);
                    guard.SetNextState(GuardAI.State.searching, 4f);
                }
            }
            effectUsed = true;
        }


    }
}
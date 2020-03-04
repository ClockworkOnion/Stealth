using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondPickup : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.GetInstance();
    }

       void OnTriggerEnter (Collider collider) {
        if (collider.gameObject.tag=="Player") {
            gameManager.GetPoints(10);
            GameObject.Destroy(this.gameObject);
        }
    }
}

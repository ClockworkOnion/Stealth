using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondPickup : MonoBehaviour
{
    public int value = 100;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {   

    }

    void OnTriggerEnter (Collider collider) {
        if (collider.gameObject.tag=="Player") {
            GlobalManager.GetInstance().AddMoney(value);
            GameObject.Destroy(this.gameObject);
            GameManager.GetInstance().SetMoneyText(GlobalManager.GetInstance().GetMoney());
    }
}
}

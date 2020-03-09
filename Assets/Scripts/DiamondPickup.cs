using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondPickup : MonoBehaviour
{
    public int value = 100;
    Animator animator;
    Animator creditsDisplayAnimator;
    // Start is called before the first frame update
    void Start()
    {   
        animator = GetComponentInChildren<Animator>();
        creditsDisplayAnimator = GameObject.Find("MoneyText").GetComponent<Animator>();
    }

    void OnTriggerEnter (Collider collider) {
        if (collider.gameObject.tag=="Player") {
            GlobalManager.GetInstance().AddMoney(value);
            GameObject.Destroy(this.gameObject, 1);
            animator.SetBool("pickedUp", true);
            creditsDisplayAnimator.SetTrigger("CreditsUp");
            GameManager.GetInstance().SetMoneyText(GlobalManager.GetInstance().GetMoney());
    }
}
}

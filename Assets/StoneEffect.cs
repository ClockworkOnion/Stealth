using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneEffect : MonoBehaviour
{
    bool effectUsed;  // wird true, wenn der Stein irgendwo gegen geprallt ist
    bool pickedUp;
    Rigidbody rb;
    public AudioClip stoneSound;
    AudioSource soundPlay;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        soundPlay = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag=="Player" && !pickedUp){
            GlobalManager.GetInstance().AddItem(PlayerItems.stone);
            GameManager.GetInstance().RefreshItemCount();
            pickedUp = true;
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
            soundPlay.PlayOneShot(stoneSound);
        }
    }

      public void GiveForce(Vector3 direction, float power) {
        rb.AddForce(direction * power);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueEffect : MonoBehaviour
{
    public float gluedDuration = 0.5f;
    public AudioClip glueStart;
    AudioSource soundSource;

    void Start() {
        soundSource = GetComponent<AudioSource>();
        soundSource.PlayOneShot(glueStart);
    }
    
    void OnTriggerStay(Collider collider) {
        if (collider.tag=="Enemy") {
            collider.gameObject.GetComponent<GuardAI>().SetGlueTimer(gluedDuration);
        }
        if (collider.tag=="Player") {
            collider.gameObject.GetComponent<PlayerControl>().SetGlueTimer(gluedDuration);
        }
    }
}

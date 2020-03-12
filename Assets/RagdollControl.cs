using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollControl : MonoBehaviour
{
    public float force = 10;
    Rigidbody[] rigidbodies;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i<rigidbodies.Length; i++) {
            rigidbodies[i].AddForce(-transform.forward*force);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollControl : MonoBehaviour
{
    public float force = 2f;
    Rigidbody[] rigidbodies;

    // Start is called before the first frame update
    public void ApplyForce(Vector3 puncher, Vector3 player)
    {
        Vector3 punchForce = (puncher - player)*force;
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i<rigidbodies.Length; i++) {
            rigidbodies[i].AddForce(-punchForce);
        }
    }
}

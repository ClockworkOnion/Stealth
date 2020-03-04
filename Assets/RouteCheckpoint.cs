using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteCheckpoint : MonoBehaviour
{
    public float waitDuration = 0f;
    public Transform nextWaypoint;
    void OnTriggerEnter(Collider collider) {
        if (collider.tag=="Enemy" && collider.GetComponent<GuardAI>().guardState == GuardAI.State.patrolling) {
            GuardAI guard = collider.GetComponent<GuardAI>();
            guard.SetDestination(nextWaypoint);
        }
    }
}

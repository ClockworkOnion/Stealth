using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteCheckpoint : MonoBehaviour
{
    public float waitDuration = 0f;
    float SetNextWaypointTimer = 0f;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Enemy" && collider.GetComponent<GuardAI>().guardState == GuardAI.State.patrolling)
        {
            GuardAI guard = collider.GetComponent<GuardAI>();
            if (guard.waypoints.Contains(this))
            {
                guard.CheckpointReached(waitDuration);
                // guard.NextWaypoint();
            }
        }
    }

    void OnTriggerStay(Collider collider) {
        if (collider.tag == "Enemy") {
            SetNextWaypointTimer += Time.deltaTime;
        }
        if (collider.tag == "Enemy" && SetNextWaypointTimer > 8) {
            collider.GetComponent<GuardAI>().NextWaypoint();
            SetNextWaypointTimer = 0;
        }
    }

    void OnTriggerExit(Collider collider) {
        SetNextWaypointTimer = 0f;
    }
}

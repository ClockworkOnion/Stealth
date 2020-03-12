using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckpointMenu : MonoBehaviour
{
    public GameObject otherWaypoint;
    public float waitingTime = 0;
    public GameObject guard;

    private void OnTriggerEnter(Collider other)
    {
        Invoke("setNewWaypointAfterTime", waitingTime);
    }

    private void setNewWaypointAfterTime()
    {
        guard.GetComponent<NavMeshAgent>().destination = otherWaypoint.transform.position;
    }
}

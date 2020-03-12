using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardInMenuScript : MonoBehaviour
{
    public GameObject firstWaypoint;

    private void Awake()
    {
        GetComponent<NavMeshAgent>().destination = firstWaypoint.transform.position;
        GetComponentInChildren<Light>().color = Color.cyan;
        GetComponentInChildren<Animator>().SetBool("walking", true);
    }
}

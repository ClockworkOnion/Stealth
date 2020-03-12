using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardInMenuScript : MonoBehaviour
{
    //Public variablen
    public float sightRange = 4;
    public float FOVfactor = 1;
    public float searchDuration = 5f; // Wie lange der Waechter nach der Verfolgung noch sucht
    public float pursueDuration = 2f; // Wie lange die Position des Spielers bekannt bleibt
    public float awarenessPatrolling = 0.02f;
    public float awarenessPursuing = 0.1f;
    public float standardWalkspeed = 2f;
    public float runningSpeed = 4f;
    public float gluedWalkspeed = 0.5f;
    public float gluedTimer = 0;
    public Transform nextRouteCheckpoint;
    public List<RouteCheckpoint> waypoints;
    int currentWaypointInList = -1;
    // public UnityEvent playerSeen; // Aktuell unbenutzt

    // Komponenten
    NavMeshAgent navMeshAgent;
    PlayerControl player;
    Light searchLight;
    Animator animator;





    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        searchLight = GetComponentInChildren<Light>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Komponenten initialisieren
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();


        // Patrouille starten
        NextWaypoint();

        animator.SetBool("walking", true);
        searchLight.color = Color.cyan;
    }

    public void SetDestination(Transform newDestination)
    {
        nextRouteCheckpoint = newDestination;
        navMeshAgent.destination = newDestination.position;
    }

    public void NextWaypoint()
    {
        if (waypoints.Count != 0)
        {
            currentWaypointInList = (currentWaypointInList + 1) % waypoints.Count;
            SetDestination(waypoints[currentWaypointInList].transform);
        }
    }
}

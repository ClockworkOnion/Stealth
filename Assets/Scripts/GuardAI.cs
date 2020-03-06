using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardAI : MonoBehaviour
{
    //Public variablen
    public float sightRange = 4;
    public float FOVfactor = 1;
    public float searchDuration = 5f; // Wie lange der Waechter nach der Verfolgung noch sucht
    public float pursueDuration = 2f; // Wie lange die Position des Spielers bekannt bleibt
    public float awarenessPatrolling = 0.02f;
    public float awarenessPursuing = 0.1f;
    public Transform nextRouteCheckpoint;
    public State guardState = State.patrolling;
    public List<RouteCheckpoint> waypoints;
    int currentWaypointInList = -1;
    // public UnityEvent playerSeen; // Aktuell unbenutzt

    // Komponenten
    NavMeshAgent navMeshAgent;
    PlayerControl player;
    Light searchLight;
    Animator animator;

    // 
    Vector3 FOVleft, FOVright, scanLine;
    Vector3 lastKnownPlayerPosition;
    float FOVinterpolation = 0f;
    float StateTimer = 0;
    float DistanceToPlayer;
    float awareness;
    float animationSpeedMemory;
    bool isPaused;

    public enum State
    {
        patrolling,
        pursuing,
        returning,
        searching,
        waiting
    }

    // Start is called before the first frame update
    void Start()
    {
        // Komponenten initialisieren
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        searchLight = GetComponentInChildren<Light>();
        animator = GetComponentInChildren<Animator>();

        // Variablen initialisieren
        awareness = awarenessPatrolling;
        animationSpeedMemory = animator.speed;

        // Patrouille starten
        NextWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        // Checken ob gameOver oder pausiert, wenn ja hier abbrechen
        if (IsGameOver() || IsGamePaused()) {
            return;
        }

        ///////////////////////////////////////////////////////////////

        // Switch-Block Gegner KI
        switch (guardState)
        {
            case State.patrolling:
                animator.SetBool("walking", true);
                awareness = awarenessPatrolling;
                searchLight.color = Color.cyan;
                break;

            case State.searching:
                if (StateTimer > 0)
                {
                    StateTimer -= Time.deltaTime;
                }
                else
                {
                    SetNextState(State.patrolling);
                    navMeshAgent.destination = nextRouteCheckpoint.position;
                }
                break;

            case State.pursuing:
                awareness = awarenessPursuing;
                searchLight.color = Color.red;

                // Spielerposition verfolgen und vergessen
                if (StateTimer > 0)
                {
                    StateTimer -= Time.deltaTime;
                    lastKnownPlayerPosition = player.GetMovementPrediction();
                    DistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                    navMeshAgent.destination = Vector3.Lerp(player.transform.position, player.GetMovementPrediction(), DistanceToPlayer);
                }
                else
                {
                    SetNextState(State.searching, searchDuration);
                }

                break;
            
            case State.waiting:
                animator.SetBool("walking", false);
                if (StateTimer > 0) {
                    StateTimer -= Time.deltaTime;
                } else {
                    SetNextState(State.patrolling);
                    NextWaypoint();
                }
                break;
            

            default:
                break;
        }

        // Sichtfeld berechnen
        FOVinterpolation = (FOVinterpolation += awareness) > 1f ? 0 : FOVinterpolation += awareness;
        FOVleft = transform.forward - transform.right * FOVfactor;
        FOVright = transform.forward + transform.right * FOVfactor;
        scanLine = Vector3.Slerp(FOVleft, FOVright, FOVinterpolation) * sightRange;

        RaycastHit hit;
        bool seen = Physics.Raycast(transform.position, scanLine, out hit, sightRange);

        if (seen && hit.collider.gameObject.tag == "Player" && !GameManager.GetInstance().gamePaused)
        {
            Debug.Log("Player seen");
            lastKnownPlayerPosition = player.transform.position;
            SetNextState(State.pursuing, pursueDuration);
            navMeshAgent.destination = lastKnownPlayerPosition;
            // playerSeen.Invoke(); // Event, falls man noch andere Sachen einbauen will
        }

        // Debug Rays
        Debug.DrawRay(transform.position, transform.forward * sightRange);
        Debug.DrawRay(transform.position, FOVleft, Color.cyan);
        Debug.DrawRay(transform.position, FOVright, Color.cyan);
        Debug.DrawRay(transform.position, scanLine, Color.red);
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            animator.SetBool("walking", false);
            GameManager.GetInstance().SetGameLost();
        }
    }

    public void SetDestination(Transform newDestination)
    {
        nextRouteCheckpoint = newDestination;
        navMeshAgent.destination = newDestination.position;
    }

    void SetNextState(State nextState)
    {
        SetNextState(nextState, 0f);
    }

    void SetNextState(State nextState, float timer)
    {
        guardState = nextState;
        StateTimer = timer;
    }

    public void NextWaypoint()
    {
        if (waypoints.Count != 0)
        {
            currentWaypointInList = (currentWaypointInList + 1) % waypoints.Count;
            SetDestination(waypoints[currentWaypointInList].transform);
        }
    }

    public void CheckpointReached(float waitDuration) {
        SetNextState(State.waiting, waitDuration);
    }

    public void TogglePause() {
        if (!isPaused) {
            animationSpeedMemory = animator.speed;
            animator.speed = 0;
            
            isPaused = true;
        } else {
            animator.speed = animationSpeedMemory;
            isPaused = false;
        }
    }

    private bool IsGameOver() { 
        if (GameManager.GetInstance().gameOver) {
            navMeshAgent.isStopped = true;
            animator.speed = 0;
            return true;
        } 
        return false;
    }

    private bool IsGamePaused() {
        if (GameManager.GetInstance().gamePaused) {
            if (animationSpeedMemory == 0) { // Geschwindigkeit nur einmal zwischenspeichern
                animationSpeedMemory = animator.speed;
                animator.speed = 0;
                navMeshAgent.isStopped = true;
            } 
            return true;
        } else {
            if (animationSpeedMemory != 0) { // Geschwindigkeit nur einmal setzen und löschen
                animator.speed = animationSpeedMemory;
                animationSpeedMemory = 0;
                navMeshAgent.isStopped = false;
            }
        }
        return false;
    }

}

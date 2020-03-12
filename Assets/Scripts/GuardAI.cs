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
    public float standardWalkspeed = 2f;
    public float runningSpeed = 4f;
    public float gluedWalkspeed = 0.5f;
    public float gluedTimer = 0;
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
    Vector3 FOVleft, FOVright, scanLine1, scanLine2;
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
        waiting,
        confused,
        punching
    }

    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        searchLight = GetComponentInChildren<Light>();
        animator = GetComponentInChildren<Animator>();

        // Variablen initialisieren
        awareness = awarenessPatrolling;
        animationSpeedMemory = animator.speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Komponenten initialisieren
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();


        // Patrouille starten
        NextWaypoint();
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        // Animationsstopp wenn keine Bewegung durch NavmeshAgent durchgeführt wird
        if (Vector3.Distance(navMeshAgent.destination, transform.position) < navMeshAgent.stoppingDistance + 0.1f) {
            animator.SetBool("walking", false);
        } else {
            animator.SetBool("walking", true);
        }

        // Checken ob gameOver oder pausiert, wenn ja hier abbrechen
        // Die Methoden stoppen automatisch die Animation des Wächters
        if (IsGameOver() || IsGamePaused()) {
            return;
        }

        ///////////////////////////////////////////////////////////////

        // Switch-Block Gegner KI
        switch (guardState)
        {
            case State.patrolling:
                animator.SetBool("walking", true);
                animator.SetBool("running", false);
                awareness = awarenessPatrolling;
                searchLight.color = Color.cyan;
                break;

            case State.searching:
                searchLight.color = Color.yellow;
                navMeshAgent.stoppingDistance = 1;
                if (StateTimer > 0)
                {
                    StateTimer -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Searching done. Patrolling...");
                    SetNextState(State.patrolling);
                    navMeshAgent.stoppingDistance = 0;
                    NextWaypoint();
                    navMeshAgent.destination = nextRouteCheckpoint.position;
                }
                break;

            case State.pursuing:
                awareness = awarenessPursuing;
                searchLight.color = Color.red;
                navMeshAgent.stoppingDistance = 0;

                if (GameManager.GetInstance().playerIsCloaked) { // Damit Spieler nicht unsichtbar weitere 2 Sekunden verfolgt wird
                    SetNextState(State.searching, searchDuration); // bzw. damit sofort verfolgung abgebrochen wird
                    break;
                }

                // Spielerposition verfolgen und vergessen
                if (StateTimer > 0)
                {
                    animator.SetBool("walking", false);
                    animator.SetBool("running", true);
                    navMeshAgent.speed = runningSpeed;
                    StateTimer -= Time.deltaTime;
                    lastKnownPlayerPosition = player.GetMovementPrediction();
                    DistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                    navMeshAgent.destination = Vector3.Lerp(player.transform.position, player.GetMovementPrediction(), DistanceToPlayer);
                }
                else
                {
                    navMeshAgent.speed = standardWalkspeed;
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

            case State.confused:
                if (StateTimer > 0) {
                    StateTimer -= Time.deltaTime;
                    navMeshAgent.destination = transform.position;
                    animator.SetBool("walking", false);
                    return;
                } else {
                    Debug.Log("Confusion done. Searching...");
                    SetNextState(State.searching, 3);
                }
                break;
            

            default:
                break;
        }

        ///// Walkspeed effekte /////

        // Glue effekt
        if (gluedTimer > 0) {
            gluedTimer -= Time.deltaTime;
            navMeshAgent.speed = gluedWalkspeed;
        } else {
            navMeshAgent.speed = animator.GetBool("running") ? runningSpeed : standardWalkspeed;
        }


        if (!GameManager.GetInstance().playerIsCloaked) {
            ///// Sichtfeld berechnen //////
            FOVinterpolation = (FOVinterpolation += awareness) > 1f ? 0 : FOVinterpolation += awareness;
            FOVleft = transform.forward - transform.right * FOVfactor;
            FOVright = transform.forward + transform.right * FOVfactor;
            scanLine1 = Vector3.Slerp(FOVleft, FOVright, FOVinterpolation) * sightRange;
            scanLine2 = Vector3.Slerp(FOVleft, FOVright, (FOVinterpolation+0.5f)%1) * sightRange;

            RaycastHit hit;
            bool seen = Physics.Raycast(transform.position, scanLine1, out hit, sightRange);
            if (!seen || hit.collider.tag != "Player") {
                seen = Physics.Raycast(transform.position, scanLine2, out hit, sightRange);
            }

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
            Debug.DrawRay(transform.position, scanLine1, Color.red);
            Debug.DrawRay(transform.position, scanLine2, Color.red);
        }
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player collision");
            animator.SetBool("walking", false);
            animator.SetTrigger("punch");
            SetNextState(State.waiting,3);

            // Invoke("PunchPlayer")
            // GameManager.GetInstance().SetGameLost();
            // Noch Spielstatus auf Verloren stellen
        }
    }

    public void PunchPlayer() {
            player.GetPunched(this.transform);
    }

    public void SetDestination(Transform newDestination)
    {
        nextRouteCheckpoint = newDestination;
        navMeshAgent.destination = newDestination.position;
    }

    public void SetNextState(State nextState)
    {
        SetNextState(nextState, 0f);
    }

    public void SetNextState(State nextState, float timer)
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

    private bool IsGameOver() { // Stoppt den Wächter wenn Spielstatus GameOver
        if (GameManager.GetInstance().gameOver) {
            navMeshAgent.isStopped = true;
            animator.speed = 0;
            return true;
        } 
        return false;
    }

    private bool IsGamePaused() { // Stoppt den Wächter bei Spielstatus Pause
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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void SetGlueTimer(float duration) {
        gluedTimer = duration;
    }

  

}

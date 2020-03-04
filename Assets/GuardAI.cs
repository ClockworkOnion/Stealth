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
    public float awareness = 0.02f;
    public float FOVfactor = 1;
    public Transform goal;
    public State guardState = State.patrolling;

    // Komponenten
    NavMeshAgent agent;
    PlayerControl player;
    Light searchLight;
    LayerMask playerLayer = 1 << 9;

    // 
    Vector3 FOVleft, FOVright, scanLine;
    Vector3 lastKnownPlayerPosition;
    float FOVinterpolation = 0f;
    float forgetPlayerTimer = 0;
    float DistanceToPlayer;

    GameManager gameManager;
    public UnityEvent playerSeen;

    public enum State {
        patrolling,
        pursuing,
        returning,
        searching
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        searchLight = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (forgetPlayerTimer > 0) {
            forgetPlayerTimer -= Time.deltaTime;
            lastKnownPlayerPosition = player.GetMovementPrediction();
            DistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            agent.destination = Vector3.Lerp(player.transform.position, player.GetMovementPrediction(), DistanceToPlayer);
        } else {
            guardState = State.patrolling;
            agent.destination = goal.position;
        }
            gameManager.SetDebugText("Timer: " + forgetPlayerTimer.ToString());

        switch (guardState)
        {
            case State.patrolling:
                awareness = 0.02f;
                searchLight.color = Color.cyan;
                break;
            case State.pursuing:
                awareness = 0.1f;
                searchLight.color = Color.red;
                break;
            default:
                break;
        }



        FOVinterpolation = (FOVinterpolation += awareness) > 1f ? 0 : FOVinterpolation+= awareness;
        FOVleft = transform.forward - transform.right * FOVfactor;
        FOVright = transform.forward + transform.right * FOVfactor;
        scanLine = Vector3.Slerp(FOVleft, FOVright, FOVinterpolation) * sightRange;

        Debug.DrawRay(transform.position, transform.forward * sightRange);
        Debug.DrawRay(transform.position, FOVleft, Color.cyan);
        Debug.DrawRay(transform.position, FOVright, Color.cyan);
        Debug.DrawRay(transform.position, scanLine, Color.red);
        
        RaycastHit hit;
        bool seen = Physics.Raycast(transform.position, scanLine, out hit, sightRange);

        if (seen && hit.collider.gameObject.tag=="Player") {
            Debug.Log("Player seen");
            // agent.destination = hit.transform.position;
            lastKnownPlayerPosition = player.transform.position;
            forgetPlayerTimer = 2;
            agent.destination = lastKnownPlayerPosition;
            guardState = State.pursuing;
            // playerSeen.Invoke(); // Event, falls man noch andere Sachen einbauen will
        }

    }
    
    void fixedUpdate() {
    }

    public void SetDestination(Transform destination) {
        goal = destination;
        agent.destination = destination.position;
    }

    public void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag=="Player") { 
            gameManager.SetGameOver();
        }
    }

}

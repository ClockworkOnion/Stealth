using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardAI : MonoBehaviour
{
    public float sightRange = 4;
    public float awareness = 0.02f;
    public Transform goal;
    NavMeshAgent agent;
    LayerMask playerLayer = 1 << 9;
    Vector3 FOVleft, FOVright, scanLine;
    float FOVinterpolation = 0f;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        FOVinterpolation = (FOVinterpolation += awareness) > 1f ? 0 : FOVinterpolation+= awareness;
        FOVleft = transform.forward - transform.right;
        FOVright = transform.forward + transform.right;
        scanLine = Vector3.Slerp(FOVleft, FOVright, FOVinterpolation) * sightRange;

        Debug.DrawRay(transform.position, transform.forward * sightRange);
        Debug.DrawRay(transform.position, FOVleft, Color.cyan);
        Debug.DrawRay(transform.position, FOVright, Color.cyan);
        Debug.DrawRay(transform.position, scanLine, Color.red);
        
        RaycastHit hit;
        bool seen = Physics.Raycast(transform.position, scanLine, out hit, sightRange);

        if (seen && hit.collider.gameObject.tag=="Player") {
            Debug.Log("Player seen");
            agent.destination = hit.transform.position;
        }
    }
    
    void fixedUpdate() {
    }

    public void SetDestination(Transform destination) {
        agent.destination = destination.position;
    }

    public void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag=="Player") { 
            gameManager.SetGameOver();
        }
    }
}

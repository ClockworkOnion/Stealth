using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    float xAxis, yAxis;
    bool runButton;
    [Range(1f, 20f)]
    public float walkSpeed = 2;
    [Range(1f, 20f)]
    public float runSpeed = 10;
    float playerSpeed = 2;
    [Range(0f,1f)]
    public float smoothing = 0.1f;
    Rigidbody rigidBody;
    Vector3 lastPosition;
    Vector3 prediction;
    Vector3 wantedVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.GetInstance().gamePaused) {
            playerSpeed = (runButton)? runSpeed : walkSpeed; // Rennknopf setzt Geschwindigkeit auf 10 (vorlaeufig)
            runButton = Input.GetKey("x");
            wantedVelocity.x = Input.GetAxis("Horizontal") * playerSpeed; 
            wantedVelocity.z = Input.GetAxis("Vertical") * playerSpeed;
        }


        prediction = (transform.position - lastPosition) * 20;
        Debug.DrawRay(transform.position, prediction, Color.cyan);
    }

    void LateUpdate() {
        lastPosition = transform.position;
    }

    void FixedUpdate() {
        if (!GameManager.GetInstance().gamePaused || !GameManager.GetInstance().gameOver) {
            wantedVelocity.y = rigidBody.velocity.y;

            Vector3 wantedMovement = Vector3.Lerp(rigidBody.velocity, wantedVelocity, smoothing);
            if (wantedMovement.magnitude > playerSpeed) { // Normalisieren, damit diagonale Laufgeschwindigkeit nicht hoeher ist
                wantedMovement = wantedMovement.normalized * playerSpeed;
            }
            rigidBody.velocity = wantedMovement;
        }
        GameManager.GetInstance().SetDebugText(GetComponent<Rigidbody>().velocity.magnitude.ToString());
    }

    public Vector3 GetMovementPrediction() {
        return transform.position + prediction;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerSpeed);
    }
}

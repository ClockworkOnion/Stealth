using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    float xAxis, yAxis;
    bool runButton;
    public float playerSpeed = 2;
    public bool gameOver = false;
    Rigidbody rigidBody;
    Vector3 lastPosition;
    Vector3 prediction;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            xAxis = -Input.GetAxis("Horizontal"); // Umgedrehte Achse??
            yAxis = -Input.GetAxis("Vertical");
            runButton = Input.GetKey("x");
        }

        playerSpeed = (runButton)? 10 : 2; // Rennknopf setzt Geschwindigkeit auf 10 (vorlaeufig)

        prediction = (transform.position - lastPosition) * 20;
        Debug.DrawRay(transform.position, prediction, Color.cyan);
    }

    void LateUpdate() {
        lastPosition = transform.position;
    }

    void FixedUpdate() {
        if (!gameOver) {
            Vector3 movement = new Vector3(xAxis, 0, yAxis);
            // rigidBody.AddForce(new Vector3(xAxis * playerSpeed * Time.deltaTime, 0, yAxis * playerSpeed * Time.deltaTime));
            transform.position = transform.position + movement * playerSpeed * Time.deltaTime;
        }
    }

    public Vector3 GetMovementPrediction() {
        return transform.position + prediction;
    }
}

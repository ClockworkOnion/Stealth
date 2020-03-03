using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    float xAxis, yAxis;
    public float playerSpeed = 5;
    public bool gameOver = false;
    Rigidbody rigidBody;
    Text DebugText;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        DebugText = GameObject.Find("DebugText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            xAxis = -Input.GetAxis("Horizontal"); // Umgedrehte Achse??
            yAxis = -Input.GetAxis("Vertical");
        }


        DebugText.text = "x Achse: " + xAxis + "\ny Achse: " + yAxis;

    }

    void FixedUpdate() {
        if (!gameOver) {
            Vector3 movement = new Vector3(xAxis, 0, yAxis);
            // rigidBody.AddForce(new Vector3(xAxis * playerSpeed * Time.deltaTime, 0, yAxis * playerSpeed * Time.deltaTime));
            transform.position = transform.position + movement * playerSpeed * Time.deltaTime;
        }

    }
}

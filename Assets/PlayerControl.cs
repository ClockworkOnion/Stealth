using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float playerSpeed = 5;
    public bool gameOver = false;
    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            float xAxis = -Input.GetAxis("Horizontal"); // Umgedrehte Achse??
            float yAxis = -Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(xAxis, 0, yAxis);
            // rigidBody.AddForce(new Vector3(xAxis * playerSpeed * Time.deltaTime, yAxis * playerSpeed * Time.deltaTime, 0));
            transform.position = transform.position + movement * playerSpeed * Time.deltaTime;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Rigidbody rigidBody;
    Vector2 movementDir;
    Animator playerAnimator;
    // Start is called before the first frame update

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerAnimator = GetComponentInChildren<Animator>();
    }

    void Update() {

    }

    void FixedUpdate()
    {
        // Spieler in die Richtung rotieren, in die er laeuft
        Vector2 direction = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
        if (direction.magnitude > 0.1f) { // Nur drehen, falls Geschwindigkeit > 0.1, sonst dreht sich Spieler permanent
            movementDir = direction;
            float angle = Mathf.Atan2(-movementDir.y, movementDir.x) * Mathf.Rad2Deg + 90;

            Quaternion q = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, 0.2f);

            // Zusätzlich Animator steuern
            playerAnimator.SetBool("isMoving", true);
        } else {
            playerAnimator.SetBool("isMoving", false);
        }

    }
}

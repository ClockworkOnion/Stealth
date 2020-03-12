using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    bool followPlayer = true;
    Transform cameraTarget;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        cameraTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        offset = transform.position - cameraTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        if (followPlayer) {
            transform.position = Vector3.Lerp(transform.position, cameraTarget.position + offset, 0.2f);
        }
    }

    public void SetFollow(bool following) {
        followPlayer = following;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    bool followPlayer = true;
    Transform cameraTarget;
    Vector3 offset;
    Scene currentScene;
    AudioSource audioSource;
    AudioListener audioListener;
    public AudioClip Scene1BackgroundMusic;
    public AudioClip Scene2BackgroundMusic;
    public AudioClip Scene3BackgroundMusic;
    public AudioClip MenuBackGroundMusic;
    public AudioClip CreditsBackgroundMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioListener = GetComponent<AudioListener>();
        currentScene = SceneManager.GetActiveScene();
        switch (currentScene.name)
        {
            case "Scene1":
                audioSource.PlayOneShot(Scene1BackgroundMusic);
                break;
            case "Scene2":
                audioSource.PlayOneShot(Scene2BackgroundMusic);
                break;
            case "Scene3":
                audioSource.PlayOneShot(Scene3BackgroundMusic);
                break;
            case "Menu":
                audioSource.PlayOneShot(MenuBackGroundMusic);
                break;
            case "":
                // audioSource.PlayOneShot(MenuBackGroundMusic);
                break;
            default:
                break;
        }
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

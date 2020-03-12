using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stopwatch : MonoBehaviour
{
    static Stopwatch instance;
    int minutes = 0;
    int seconds = 0;
    int hundredthsOfASecond = 0;
    
    float tmpTimer;
    public float timer;

    bool started = false;
    TextMeshProUGUI stopwatchText;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        stopwatchText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetInstance().gamePaused || GameManager.GetInstance().gameOver) {
            return;
        } else {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                tmpTimer = timer;

                minutes = (int) tmpTimer / 60;
                tmpTimer %= 60f;

                seconds = (int) tmpTimer;

                hundredthsOfASecond = ((int)(tmpTimer * 100f)) % 100;

                stopwatchText.SetText(minutes + ":" + seconds.ToString("00") + ":" + hundredthsOfASecond.ToString("00"));
            } else {
                GameManager.GetInstance().SetGameLost();
            }
        }
    }

    public void Reset()
    {
        timer = 0;
        hundredthsOfASecond = 0;
        seconds = 0;
        minutes = 0;
    }

    public void SetTimer(float time) {
        timer = time;
    }

    public static Stopwatch GetInstance()
    {
        return instance;
    }


}

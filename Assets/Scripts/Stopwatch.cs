﻿using System.Collections;
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
    

    string hundrethsString;
    string secondsString;
    float timer = 90;
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
            timer -= Time.deltaTime;
            hundrethsString = (timer*100).ToString("00").Substring(0,2);

            minutes = (int)timer / 60;
            seconds = (int) timer % 60;
            stopwatchText.SetText(minutes + ":" + seconds.ToString("00") + ":" + hundrethsString);
        }
    }

    public void Reset()
    {
        timer = 0;
        hundredthsOfASecond = 0;
        seconds = 0;
        minutes = 0;
    }

    public static Stopwatch GetInstance()
    {
        return instance;
    }


}

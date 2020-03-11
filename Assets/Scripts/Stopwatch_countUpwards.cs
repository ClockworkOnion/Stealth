using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stopwatch_countUpwards : MonoBehaviour
{
    static Stopwatch_countUpwards instance;
    int minutes = 0;
    int seconds = 0;
    int hundredthsOfASecond = 0;
    float timer = 0;
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
            stopwatchText.SetText(minutes + ":" + seconds.ToString("00") + ":" + hundredthsOfASecond.ToString("00"));
            timer += Time.deltaTime;
            hundredthsOfASecond = (int)(timer * 100);
            while (hundredthsOfASecond >= 100)
            {
                hundredthsOfASecond -= 100;
                timer -= 1;
                seconds++;
            }
            while (seconds >= 60)
            {
                minutes++;
                seconds -= 60;
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

    public static Stopwatch_countUpwards GetInstance()
    {
        return instance;
    }


}

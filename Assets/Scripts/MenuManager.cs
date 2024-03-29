﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    int activeMenuItem;
    TextMeshProUGUI[] menuItems;
    TextMeshProUGUI graphicsButtonText;
    Button newGameButton; 
    Button stage1Button;
    Button stage2Button;
    Button stage3Button;
    Button selectStageButton;
    Button settingsButton;
    Button soundButton;
    Canvas selectCanvas; 
    Canvas settingsCanvas; 

    void Awake() {
        if (!SceneManager.GetSceneByBuildIndex(0).isLoaded) {
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Button Referenzen
        newGameButton = GameObject.Find("NewGameButton").GetComponent<Button>();
        stage1Button = GameObject.Find("Stage1Button").GetComponent<Button>();
        stage2Button = GameObject.Find("Stage2Button").GetComponent<Button>();
        stage3Button = GameObject.Find("Stage3Button").GetComponent<Button>();
        selectStageButton = GameObject.Find("SelectStageButton").GetComponent<Button>();
        settingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
        soundButton = GameObject.Find("SoundButton").GetComponent<Button>();
        graphicsButtonText = GameObject.Find("GraphicsButton").GetComponentInChildren<TextMeshProUGUI>();

        // Komponenten-Referenzen
        menuItems = GetComponentsInChildren<TextMeshProUGUI>();
        selectCanvas = GameObject.Find("SelectCanvas").GetComponent<Canvas>();
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();

        // Nicht verfuegbare Stages ausgrauen
        stage1Button.interactable = true;
        stage2Button.interactable = GlobalManager.GetInstance().GetLevelCompleted(0);
        stage3Button.interactable = GlobalManager.GetInstance().GetLevelCompleted(1);

        // Initialwerte 
        newGameButton.Select();
        settingsCanvas.enabled = false;
        selectCanvas.enabled = false;
        soundButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText(AudioListener.pause ? "Sound: Off" : "Sound: On");
    }

    // Update is called once per frame
    void Update()
    {
        if (selectCanvas.enabled && (Input.GetButtonDown("Cancel") || Input.GetAxis("Horizontal") < -0.5f))  {
            SelectStage();
            selectStageButton.Select();

        }        

        if (settingsCanvas.enabled && (Input.GetButtonDown("Cancel") || Input.GetAxis("Horizontal") < -0.5f))  {
            Settings();
            settingsButton.Select();
        }        

    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void NewGame() {
        GlobalManager.GetInstance().ResetProgression();
        SceneManager.LoadScene(2);
    }

    public void SelectStage() {
        settingsCanvas.enabled = false;
        selectCanvas.enabled = !selectCanvas.enabled;
        stage1Button.Select();

    } 

    public void StartStage(int stage) {
        SceneManager.LoadScene(stage+1);

    }

    public void Settings() {
        selectCanvas.enabled = false;
        settingsCanvas.enabled = !settingsCanvas.enabled;
        soundButton.Select();
    }

    public void ToggleGraphics() {
        if (graphicsButtonText.text == "Graphics: Ultra") {
            graphicsButtonText.text = "Graphics: Max";
        } else if (graphicsButtonText.text == "Graphics: Max") {
            graphicsButtonText.text = "Graphics: Ultra";
        }
    }

    public void ToggleSound() {
        Debug.Log("Sound toggled");
        AudioListener.pause = !AudioListener.pause;
        soundButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText(AudioListener.pause ? "Sound: Off" : "Sound: On");
        // soundButton.colors = AudioListener.pause ? ColorBlock. : Color.white;
        }
}

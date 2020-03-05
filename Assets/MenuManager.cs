using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    int activeMenuItem;
    TextMeshProUGUI[] menuItems;
    Button newGameButton; 
    Button stage1Button;
    Button selectStageButton;
    Button soundButton;
    Canvas selectCanvas; 
    Canvas settingsCanvas; 

    // Start is called before the first frame update
    void Start()
    {
        newGameButton = GameObject.Find("NewGameButton").GetComponent<Button>();
        stage1Button = GameObject.Find("Stage1Button").GetComponent<Button>();
        selectStageButton = GameObject.Find("SelectStageButton").GetComponent<Button>();
        soundButton = GameObject.Find("SoundButton").GetComponent<Button>();

        menuItems = GetComponentsInChildren<TextMeshProUGUI>();
        newGameButton.Select();
        selectCanvas = GameObject.Find("SelectCanvas").GetComponent<Canvas>();
        selectCanvas.enabled = false;
        settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
        settingsCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Input.GetKeyDown("w");
        Input.GetKeyDown("s");
        if (activeMenuItem < 0) {
            activeMenuItem = menuItems.Length-1;
        }
        activeMenuItem = activeMenuItem % menuItems.Length;

        if (selectCanvas.enabled && Input.GetButtonDown("Cancel")) {
            SelectStage();
            selectStageButton.Select();

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
}

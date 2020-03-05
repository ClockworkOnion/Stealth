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
    public Button testButton; 

    // Start is called before the first frame update
    void Start()
    {
        menuItems = GetComponentsInChildren<TextMeshProUGUI>();
        testButton.Select();
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
        
    }
}

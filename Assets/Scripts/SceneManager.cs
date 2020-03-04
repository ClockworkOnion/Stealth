using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    static SceneManager instance;
    List<GuardAI> guardsList;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        // Erstellt eine Liste mit allen Waechtern aus dem aktuellen Level
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < tempList.Length; i++)
        {
            guardsList.Add(tempList[i].GetComponent<GuardAI>());
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static SceneManager GetInstance() {
        return instance;
    }
}

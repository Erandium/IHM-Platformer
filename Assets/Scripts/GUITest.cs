using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUITest : MonoBehaviour
{
    public string Sgravity =  "10";
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void OnGUI()
    {


        // Make a background box
        GUI.Box(new Rect(10, 10, 100, 90), "Loader Menu");

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(20, 40, 80, 20), "Level 1"))
        {
            SceneManager.LoadScene(1);
        }

        // Make the second button.
        if (GUI.Button(new Rect(20, 70, 80, 20), "Level 2"))
        {
            SceneManager.LoadScene(2);
        }

        GUI.Box(new Rect(10, 110, 110, 300), "Change Variables");

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed

        Sgravity = GUI.TextField(new Rect(20,140,40,40),"");

        if (GUI.Button(new Rect(55, 140, 50, 20), "Gravity"))
        {
            SceneManager.LoadScene(1);
        }

        // Make the second button.
        if (GUI.Button(new Rect(20, 170, 80, 20), "Level 2"))
        {
            SceneManager.LoadScene(2);
        }
    }
}

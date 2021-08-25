using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text : MonoBehaviour
{
    public int fontSize = 20; //change the font size

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
 
    void OnGUI()
    {
        guiStyle.fontSize = fontSize;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(820, 13, 100, 20), "Phase Space", guiStyle);
    }
}

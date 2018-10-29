using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class About : MonoBehaviour {

    bool aboutVisible = false;

    public void DoAbout()
    {
        aboutVisible = !aboutVisible;
    }

    private void OnGUI()
    {
        if (aboutVisible)
        {
            // the About Window has an ID of 1.
            GUIStyle bigFontStyle = new GUIStyle(GUI.skin.window);
            bigFontStyle.fontSize = 28;
            GUILayout.Window(1, new Rect((Screen.width - 600) * 0.5f, (Screen.height - 380) * 0.5f, 600, 320), makeAboutWindow, new GUIContent("A Brown RISD Game Developers Creation\nArt:\nList of Names\nDesign:\nList of Names\nProgramming:\nList of Names\nSound:\nList of Names"), bigFontStyle);
        }
    }

    // Make the contents of the window
    void makeAboutWindow(int windowID)
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Close")))
        {
            aboutVisible = !aboutVisible;
        }
    }
}

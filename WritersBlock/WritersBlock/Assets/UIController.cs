using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Animator _CamAnimator;
    public GameObject startB;
    public GameObject aboutB;
    public GameObject quitB;
    public GameObject flipB;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame(){
        Object.Destroy(startB);
        Object.Destroy(aboutB);
        Object.Destroy(quitB);
        Object.Destroy(flipB);
        var bkCtrl = GameObject.FindObjectOfType<MegaBookBuilder>();
        bkCtrl.SetPageTexture(LoadPNG("C:\\Users\\David\\Documents\\WritersBlock\\WritersBlock\\Images\\prototype_background_L.jpg"), 0, false);
        bkCtrl.SetPageTexture(LoadPNG("C:\\Users\\David\\Documents\\WritersBlock\\WritersBlock\\Images\\prototype_background_R.jpg"), 1, true);
        _CamAnimator.SetTrigger ("GameStartZoom");
        bkCtrl.NextPage();
        bkCtrl.NextPage();
	}

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}

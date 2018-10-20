using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Animator _CamAnimator;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame(){
		_CamAnimator.SetTrigger ("GameStartZoom");
		var bkCtrl = GameObject.FindObjectOfType<MegaBookBuilder>();
        Texture2D tex = new Texture2D(1024, 1024);
        //bkCtrl.FetchTexture("https://images.pexels.com/photos/259915/pexels-photo-259915.jpeg", tex);
        bkCtrl.SetPageTexture(LoadPNG("C:\\Users\\David\\Documents\\WritersBlock\\WritersBlock\\Images\\prototype_background.jpg"), 0, false);
        bkCtrl.NextPage ();
		//bkCtrl.NextPage ();
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

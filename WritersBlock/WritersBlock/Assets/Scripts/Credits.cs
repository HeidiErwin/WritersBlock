using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Submit")) {
			SceneManager.LoadScene("Start_Screen");
		}
		if (Input.GetButtonDown("Cancel")) {
			SceneManager.LoadScene("Start_Screen");
		}
	}
}

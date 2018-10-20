using System.Collections;
using System.Collections.Generic;
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
		bkCtrl.NextPage ();
		bkCtrl.NextPage ();
	}
}

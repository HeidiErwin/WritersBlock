using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * GameController just keeps track of the player for now; 
 * Will potentially add more to this class as events occur.
 */
public class GameController : MonoBehaviour {

    public PlayerController player;

	void Start () {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterController : MonoBehaviour {

    public GameObject player;
    private SpriteRenderer right;
    private SpriteRenderer left;

	// Use this for initialization
	void Start () {
        right = GetComponentsInChildren<SpriteRenderer>()[0];
        left = GetComponentsInChildren<SpriteRenderer>()[1];
    }

    // Update is called once per frame
    void Update () {
		if (player.GetComponent<PlayerController>().GetPage() == 1)
        {
            left.enabled = false;
            right.enabled = true;
        } else
        {
            left.enabled = true;
            right.enabled = false;
        }
	}
}

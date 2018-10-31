using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SentenceCoordinator : MonoBehaviour {

    protected SentenceController[] sentences;
    protected Animator animator;

	// Use this for initialization
	void Start () {
        sentences = GetComponentsInChildren<SentenceController>();
        animator = GetComponent<Animator>();
	}

}

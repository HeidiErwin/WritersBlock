using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SentenceCoordinatorLevelThree : MonoBehaviour {


    /**
     * 
     * Sentence 0: "I'm so TIRED. I wish i were grown up so I could retire."
     * Sentence 1: "The wizard’s book is in the CUPBOARD"
     * Sentence 2: [wizard received cup]
     * Sentence 3: "I'm going to make this LEMONade disappear"
     * Sentence 4: "I'm OFF WORK"
     * Sentence 5+: [no effects]
     * 
     **/

    public GameObject lemonade;
    public GameObject lad;

    public Sprite deadLad;

    private SentenceController[] sentences;

    private void Start() {
        sentences = GetComponentsInChildren<SentenceController>();
    }

    private void Update() {

        // Sentence One: "old"        
        if (this.sentences[0].GetWords()[0].Equals("old")) {
            KillLad();
        }
        // Sentence Two: "air"
        //if (this.sentences[1].GetWords()[0].Equals("air") || this.sentences[1].GetWords()[1].Equals("air")) { // AIR can take the place of either CUP or BOARD
        //     MakeBookFloat();
        //}
        // Sentence Three: "cup"
        //if (this.sentences[2].GetWords()[0].Equals("cup")) {
        //    GiveWizardCup();

        //}
        //// Sentence Three: "block"
        //if (this.sentences[3].GetWords()[0].Equals("block")) {
        //    DestroyBlockade();

        //}
        //// Sentence Four: "on" and "board"
        //if (this.sentences[4].GetWords()[0].Equals("on") &&
        //    this.sentences[4].GetWords()[1].Equals("board")) {
        //    BeatLevel();
        //}
    }

    void DestroyBlockade() {
        // TODO: break down blockade so characters can leave
    }

    void GiveWizardCup() {
        // TODO: show frame of lad aging & dying    
    }

    void KillLad() {
        // TODO: show frame of lad aging & dying
        lad.GetComponent<SpriteRenderer>().sprite = deadLad;
        Debug.Log("lad dead");
    }

    void MakeBookFloat() {
        // TODO: make book sprite appear in air
    }

    void BeatLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}

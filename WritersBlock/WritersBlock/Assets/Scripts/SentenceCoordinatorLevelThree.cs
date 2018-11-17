using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SentenceCoordinatorLevelThree : MonoBehaviour {

    /**
     * Level Summary: 
     * 1. Player kills lad by making lad "so OLD" instead of "so TIRED". 
     * 2. Move book to floating in the air by "the wizard's book is in the AIR"
     * 3. Give wizard CUP as container 
     * 4. Make "BLOCKade" disappear
     * 5. Convince wizard to help you by making him say "I'm ON BOARD"
     * 
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
    public GameObject ladSpeech;
    public GameObject floatingRecipeBook;
    public GameObject disappearSentenceObject; // "I'm going to make this LEMONade disappear"
    public GameObject sentenceAboutWorkObject; // "I'm OFF WORK"
    public GameObject offWordObject;
    public GameObject workWordObject;
    public GameObject cupWordObject;
    public GameObject lemonWordObject;
    public GameObject playerSpeech;
    public GameObject originalWizard;
    public GameObject movedWizard;
    public GameObject blankToHandWizardCup;
    public GameObject wizardNeedsCup;
    public GameObject oldText;
    public GameObject endingText;


    private SentenceController disappearSentence;
    private SentenceController sentenceAboutWork;
    private SentenceController handWizardCupSentence;

    private WordController offWord;
    private WordController workWord;
    private WordController lemonWord;


    public Sprite deadLad;
    private bool lemonadeConjured = false;
    private bool blockadeDestroyed = false;
    private bool bookInAir = false;
    private bool ladDead = false;
    private bool ended = false;

    private SentenceController[] sentences;

    private void Start() {
        sentences = GetComponentsInChildren<SentenceController>();
        disappearSentence = disappearSentenceObject.GetComponent<SentenceController>();
        sentenceAboutWork = sentenceAboutWorkObject.GetComponent<SentenceController>();
        handWizardCupSentence = blankToHandWizardCup.GetComponent<SentenceController>();
        offWord = offWordObject.GetComponent<WordController>();
        workWord = workWordObject.GetComponent<WordController>();
    }

    private void Update() {
        if (this.ended)
        {
            if (Input.GetButtonDown("Submit"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        // Sentence One: "old"        
        if (this.sentences[0].GetWords()[0].Equals("old")) {
            KillLad();
          //  sentences[0].Lock();
        }

        // Sentence Two: "air" // AIR can take the place of either CUP or BOARD
        if ((this.sentences[1].GetWords()[0].Equals("air") && this.sentences[1].GetWords()[1].Equals(""))
            || (this.sentences[1].GetWords()[1].Equals("air") && this.sentences[1].GetWords()[0].Equals(""))) { 
             MakeBookFloat();
           // sentences[1].Lock();
        }

        // Sentence Three: "cup"
        if (bookInAir && ladDead && handWizardCupSentence.GetWords()[0].Equals("cup")) {
            GiveWizardCup();
           // handWizardCupSentence.Lock();
        }

        // Sentence Three: "block"
        if (!blockadeDestroyed && lemonadeConjured && disappearSentence.GetWords()[0].Equals("block")) {
            DestroyBlockade();
            PlayerAsksForWizardsHelp();
        }
        // Sentence Four: "on" and "board"
        if (blockadeDestroyed && sentenceAboutWork.GetWords()[0].Equals("on") &&
            sentenceAboutWork.GetWords()[1].Equals("board")) {
            WizardConvinced();
          //  sentenceAboutWork.Lock();
        }
    }

    void DestroyBlockade() {
        blockadeDestroyed = true;
        GameObject.Find("Blockade").SetActive(false);
    }

    void GiveWizardCup() {
        lemonade.SetActive(true);
        lemonadeConjured = true;
        cupWordObject.SetActive(false);
        wizardNeedsCup.SetActive(false);
        disappearSentenceObject.SetActive(true);
        lemonWordObject.SetActive(true);
    }

    void KillLad() {
        // TODO: show frame of lad aging & dying
        lad.GetComponent<SpriteRenderer>().sprite = deadLad;
        ladDead = true;
        ladSpeech.SetActive(false);
    }

    void PlayerAsksForWizardsHelp() {
        playerSpeech.SetActive(true);
        sentenceAboutWorkObject.SetActive(true);
        offWordObject.SetActive(true);
        workWordObject.SetActive(true);
        // wizard responds "I'm OFF WORK"
    }

    void MakeBookFloat() {
        bookInAir = true;
        floatingRecipeBook.SetActive(true);
        blankToHandWizardCup.SetActive(true);
        movedWizard.SetActive(true);
        originalWizard.SetActive(false);
        wizardNeedsCup.SetActive(true);
    }

    void WizardConvinced() { //TODO
        BeatLevel();
    }

    void BeatLevel()
    {
        oldText.SetActive(false);
        endingText.SetActive(true);
        this.ended = true;
    }
}

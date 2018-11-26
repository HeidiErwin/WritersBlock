﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SentenceCoordinatorLevelTwo : MonoBehaviour
{

    /**
     * Sentence 0: The block is ON the chest.
     * Sentence 1: No! I'm holding my STICK forever
     * Sentence 2: [received key]
     * Sentence 3: The boy takes a BREATH of fresh AIR
     * Sentence 4+: [no effects]
     * 
     **/

    public GameObject block;
    public GameObject dentist;
    public GameObject chest;
    public GameObject stick;
    public GameObject off;
    public Sprite pointingDentist;
    public Sprite deadDentist;
    public Sprite openChest;
    public Sprite openChestWithStick;
    public GameObject inventory;
    public GameObject offKeyWord;
    public GameObject offWord;
    public GameObject keyWord;
    public GameObject chestThought;
    public GameObject stickSentenceObject; // "I'm holding my ___ forever!"
    public GameObject stickStaticTextObject;
    public GameObject oldText; // disappears when level is beat
    public GameObject endingText; // displayed when level is beat
    public GameObject stickInAir1;
    public GameObject stickInAir2;
    public GameObject droppedStick;
    public GameObject chestText;
    public GameObject chestTextStatic;
    public GameObject promptToKillDentist;
    public GameObject ladWantsChewy;
    public GameObject alasText; // alas... the stick was not your pen
    public GameObject filter;

    private SentenceController stickSentence;

    private bool blockRemoved = false;
    private bool stickRemoved = false;
    private bool offkeySeparated = false;
    private bool stickEnabled = false;
    private bool killedDentist = false;
    private bool helpedBoy = false;
    private bool ended = false;

    private SentenceController[] sentences;

    private void Start()
    {
        sentences = GetComponentsInChildren<SentenceController>();
        offWord.SetActive(false);
        keyWord.SetActive(false);
        stickSentenceObject.SetActive(false);
        stickSentence = stickSentenceObject.GetComponent<SentenceController>();
        stickSentence.Start();
    }

    private void Update()
    {
        if (this.ended)
        {
            if (Input.GetButtonDown("Submit"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        if (inventory.GetComponent<WordBank>().HaveWord("offkey") && !offkeySeparated)
        {
            this.SeparateOffkey();
        }
        // Sentence One: "off"        
        if (this.sentences[0].GetWords()[0].Equals("off"))
        {
            block.SetActive(false);
            blockRemoved = true;
            sentences[0].Lock();
        }
        // Sentence Two: "breath"
        if (this.sentences[1].GetWords()[0].Equals("breath"))
        {
            dentist.GetComponent<SpriteRenderer>().sprite = deadDentist;
            alasText.SetActive(true);
            promptToKillDentist.SetActive(false);
            killedDentist = true;
            droppedStick.SetActive(true);
            Debug.Log("dentist dead");
            BeatLevel();
            sentences[1].Lock();
        }
        // Sentence Three: "key"
        if (this.sentences[2].GetWords()[0].Equals("key") && blockRemoved && !stickRemoved)
        {
            StartCoroutine(AnimateStickLeavingChest());
            stickRemoved = true;

            chestText.SetActive(false);
            chestTextStatic.SetActive(false);
            off.SetActive(false);
            chestThought.SetActive(false);
            if (!stickEnabled)
            {
                stickSentenceObject.SetActive(true);
                stickStaticTextObject.SetActive(true);
                //foreach (SpriteRenderer sprite in sentences[1].GetComponentsInChildren<SpriteRenderer>())
                //{
                //    sprite.enabled = true;
                //}
                //sentences[1].GetComponentInChildren<BoxCollider2D>().enabled = true;
                stick.SetActive(true);
                stickEnabled = true;
            }
            this.sentences[2].Lock();
        }
        // Sentence Four: "stick" and "gum"
        if (this.sentences[3].GetWords()[0].Equals("stick") &&
            this.sentences[3].GetWords()[1].Equals("gum"))
        {
            ladWantsChewy.SetActive(false);
            helpedBoy = true;
            BeatLevel();
            sentences[3].Lock();
        }
    }

    private IEnumerator AnimateStickLeavingChest() {
        chest.GetComponent<SpriteRenderer>().sprite = openChestWithStick;
        chest.transform.position = new Vector2(chest.transform.position.x, -3.0f);
        yield return new WaitForSeconds(1);
        chest.GetComponent<SpriteRenderer>().sprite = openChest;
        stickInAir1.SetActive(true);
        yield return new WaitForSeconds(1);
        stickInAir1.SetActive(false);
        stickInAir2.SetActive(true);
        yield return new WaitForSeconds(1);
        stickInAir2.SetActive(false);
        dentist.GetComponent<SpriteRenderer>().sprite = pointingDentist;
        yield return new WaitForSeconds(1);
        promptToKillDentist.SetActive(true);
        yield break;
    }

    void SeparateOffkey() {    
        offKeyWord.GetComponent<WordController>().InSentence = false;
        offKeyWord.SetActive(false);
        offWord.SetActive(true);
        keyWord.SetActive(true);
    }

    void BeatLevel()
    {
        if (killedDentist && helpedBoy) {
            oldText.SetActive(false);
            filter.SetActive(false);
            endingText.SetActive(true);
            this.ended = true;
        }
    }

}

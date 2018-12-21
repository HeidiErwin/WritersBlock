﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBank : MonoBehaviour {

    [SerializeField] private int maxCount;

    private List<WordController> wordsInBank;
    private int currentPage;

    private void Start()
    {
        wordsInBank = new List<WordController>();
    }

    private void Update()
    {
        WordController toRem = null;
        foreach (WordController word in wordsInBank)
        {
            if (!word.InSentence)
            {
                toRem = word;
            }
        }
        if (toRem)
        {
            wordsInBank.Remove(toRem);
            this.RealignWords();
        }
    }

    private void RealignWords()
    {
        float y = this.transform.position.y + (this.transform.GetComponent<SpriteRenderer>().bounds.size.y / 2.0f);
        float offset = 0;
        foreach (WordController word in wordsInBank)
        {
            word.transform.position = new Vector3(this.transform.position.x, y + offset - (word.GetHeight() / 2.0f), word.transform.position.z);
            offset -= word.GetHeight();
        }
    }

    private bool HaveWord(WordController addWord)
    {
        foreach (WordController word in wordsInBank)
        {
            if (word.GetText().Equals(addWord.GetText()))
            {
                return true;
            }
        }
        return false;
    }

    public bool HaveWord(string wordText)
    {
        foreach (WordController word in wordsInBank)
        {
            if (word.GetText().Equals(wordText))
            {
                return true;
            }
        }
        return false;
    }

    private void KickFirstWord()
    {
        int flip = 1;
        if (currentPage == 2)
        {
            flip = -1;
        }
        WordController word = wordsInBank[0];
        wordsInBank.Remove(word);
        word.transform.position =
            new Vector3(this.transform.position.x +
            (this.transform.GetComponent<SpriteRenderer>().bounds.size.x
            * 1.0f) * flip, word.transform.position.y, word.transform.position.z);
        word.transform.SetParent(null);

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0))
        {
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            if (!this.HaveWord(colliderWord))
            {
                wordsInBank.Add(colliderWord);
                colliderWord.transform.SetParent(this.transform);
                colliderWord.InSentence = true;
                if (wordsInBank.Count > maxCount)
                {
                    this.KickFirstWord();
                }
                this.RealignWords();
            } 
        }
    }

    public void SetPage(int newPage)
    {
        currentPage = newPage;
    }

}

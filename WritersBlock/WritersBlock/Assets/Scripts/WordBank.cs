using System;
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
            word.transform.position = new Vector2(this.transform.position.x, y + offset - (word.GetHeight() / 2.0f));
            offset -= word.GetHeight();
        }
    }

    public void SetPage(int pageNum) {
        currentPage = pageNum;
        this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y * pageNum);
        this.RealignWords();
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

    private void KickWord(WordController word)
    {
        word.transform.position =
            new Vector2(this.transform.position.x +
            (this.transform.GetComponent<SpriteRenderer>().bounds.size.x
            * 0.75f), word.transform.position.y);

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0))
        {
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            if (!this.HaveWord(colliderWord))
            {
                if (wordsInBank.Count < maxCount)
                {
                    wordsInBank.Add(colliderWord);
                    colliderWord.InSentence = true;
                    this.RealignWords();
                } else
                {
                    this.KickWord(colliderWord);
                }
            } 
        }
    }

}

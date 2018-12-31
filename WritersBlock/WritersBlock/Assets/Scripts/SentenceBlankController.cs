using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceBlankController : MonoBehaviour {

    public float size;
    private float start_size;

    private WordController word;
    private SentenceController sentence;
    private bool locked = false;
    private bool firstCheck = true;

    private GameController gameCtrl;

    private void Start()
    {
        sentence = this.transform.parent.GetComponent<SentenceController>();
        this.start_size = this.size;
        if (GetComponent<SpriteRenderer>()) {
            this.size = GetComponent<SpriteRenderer>().bounds.size.x; // TODO decide if fixed size of blanks or based off sprite
        }
       gameCtrl = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Update()
    {
        if(word && !word.InSentence)
        {
            if (GetComponent<SpriteRenderer>()) {
                GetComponent<SpriteRenderer>().enabled = true;
            }
            word = null;
            if (GetComponent<SpriteRenderer>()) {
                this.size = GetComponent<SpriteRenderer>().bounds.size.x; // this.start_size;
            }
            sentence.RealignWords();
        } else if ((word == null) || !word.InSentence) {
            if (GetComponent<SpriteRenderer>()) {
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0) && !this.locked && 
            (collision.GetComponent<WordDraggable>().OnSamePageAsPlayer || firstCheck)) {
            if (word != null) {
                word.transform.position = new Vector3(this.transform.position.x,
                    this.transform.position.y + word.transform.GetComponent<SpriteRenderer>().bounds.size.y, word.transform.position.z);
                word = null;
            }
            
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            if (colliderWord.numBlanksOver == 1) {
                word = colliderWord;
                word.InSentence = true;
                if (GetComponent<SpriteRenderer>()) {
                    GetComponent<SpriteRenderer>().enabled = false;
                }
            }
           
            word.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, word.transform.position.z);
            this.size = word.transform.GetComponent<SpriteRenderer>().bounds.size.x;
            sentence.RealignWords();
        }
        firstCheck = false;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Word")) {
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            colliderWord.numBlanksOver++;
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Word")) {
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            colliderWord.numBlanksOver--;
        }
    }

    public string GetText()
    {
        if (word != null) {
            return word.GetText();
        } else {
            return "";
        }
    }

    public float GetSize()
    {
        return this.size;
    }

    public void LockWord()
    {
        word.GetComponent<BoxCollider2D>().enabled = false;
        this.locked = true;
    }

}

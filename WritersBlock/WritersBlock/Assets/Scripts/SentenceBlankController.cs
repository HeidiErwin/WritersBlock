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

    private void Start()
    {
        sentence = this.transform.parent.GetComponent<SentenceController>();
        this.start_size = this.size;
        if (GetComponent<SpriteRenderer>()) {
            this.size = GetComponent<SpriteRenderer>().bounds.size.x; // TODO decide if fixed size of blanks or based off sprite
        }
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
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0) && !this.locked && (collision.GetComponent<WordDraggable>().OnSamePageAsPlayer || firstCheck))
        {
            if (word != null)
            {
                word.transform.position = new Vector3(this.transform.position.x, 
                    this.transform.position.y + word.transform.GetComponent<SpriteRenderer>().bounds.size.y, word.transform.position.z);
            }
            if (GetComponent<SpriteRenderer>()) {
                GetComponent<SpriteRenderer>().enabled = false;
            }
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            word = colliderWord;
            word.InSentence = true;
            word.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, word.transform.position.z);
            this.size = word.transform.GetComponent<SpriteRenderer>().bounds.size.x;
            sentence.RealignWords();
        }
        firstCheck = false;
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

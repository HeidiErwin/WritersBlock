using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceBlankController : MonoBehaviour {

    public float size;
    private float start_size;

    private WordController word;
    private SentenceController sentence;

    private void Start()
    {
        sentence = this.transform.parent.GetComponent<SentenceController>();
        this.start_size = this.size;
        this.size = GetComponent<SpriteRenderer>().bounds.size.x; // TODO decide if fixed size of blanks or based off sprite
    }

    private void Update()
    {
        if(word && !word.InSentence)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            word = null;
            this.size = GetComponent<SpriteRenderer>().bounds.size.x; // this.start_size;
            sentence.RealignWords();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0))
        {
            if (word != null)
            {
                word.transform.position = new Vector2(this.transform.position.x, 
                    this.transform.position.y + word.transform.GetComponent<SpriteRenderer>().bounds.size.y);
            }
            GetComponent<SpriteRenderer>().enabled = false;
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            word = colliderWord;
            word.InSentence = true;
            word.transform.position = this.transform.position;
            this.size = word.transform.GetComponent<SpriteRenderer>().bounds.size.x;
            sentence.RealignWords();
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
}

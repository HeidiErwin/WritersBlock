using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceBlankController : MonoBehaviour {

    public WordController word;

    private void Update()
    {
        if(word && !word.GetInSentence())
        {
            word = null;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0))
        {
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            if (colliderWord.CheckText("key"))
            {
                word = colliderWord;
                word.SetInSentence(true);
                word.transform.position = this.transform.position;
            }
        }
    }

}

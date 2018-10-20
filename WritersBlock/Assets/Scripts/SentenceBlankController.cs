using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceBlankController : MonoBehaviour {

    public WordController word;

    private void Update()
    {
        if(word && !word.InSentence)
        {
            word = null;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("colliding");
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0))
        {
            WordController colliderWord = collision.transform.GetComponent<WordController>();
            word = colliderWord;
            word.InSentence = true;
            word.transform.position = this.transform.position;
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

}

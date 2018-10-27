using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordController : MonoBehaviour {

    public string text;

    public bool InSentence { get; set; }

    public string GetText()
    {
        return this.text;
    }

    public float GetHeight()
    {
        return this.transform.GetComponent<SpriteRenderer>().bounds.size.y;
    }

}

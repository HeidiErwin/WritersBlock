using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordController : MonoBehaviour {

    public string text;
    public bool swappable;

    private bool inSentence;

    private WordDraggable dragScript;

    private void Start()
    {
        inSentence = false;
        dragScript = GetComponent<WordDraggable>();
    }

    public bool CheckText(string otherText)
    {
        return this.text.Equals(otherText);
    }

    public void SetInSentence(bool isIn)
    {
        this.inSentence = isIn;
    }

    public bool GetInSentence()
    {
        return inSentence;
    }

}

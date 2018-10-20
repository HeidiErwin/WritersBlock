using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceController : MonoBehaviour {

    public int numBlanks;
    private SentenceBlankController[] blanks;

    private void Start()
    {
        blanks = GetComponentsInChildren<SentenceBlankController>();
    }

    public string[] GetWords()
    {
        string[] wordText = new string[blanks.Length];
        for (int i = 0; i < blanks.Length; i++)
        {
            wordText[i] = blanks[i].GetText();
        }
        return wordText;
    }

}

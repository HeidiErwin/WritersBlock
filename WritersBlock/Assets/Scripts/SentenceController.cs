using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceController : MonoBehaviour {

    public int numBlanks;
    private SentenceBlankController[] blanks;

    private void Start()
    {
        blanks = new SentenceBlankController[numBlanks];
    }



}

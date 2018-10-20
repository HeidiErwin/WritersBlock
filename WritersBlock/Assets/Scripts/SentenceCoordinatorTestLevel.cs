using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceCoordinatorTestLevel : SentenceCoordinator {

    public GameObject toDelete;

    void Update()
    {
        string[] sentenceWords = this.sentences[0].GetWords();
        if (sentenceWords[0].Equals("key"))
        {
            Object.Destroy(toDelete);
        }
    }

}

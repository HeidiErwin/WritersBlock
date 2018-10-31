using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceCoordinatorLevelOne : SentenceCoordinator {

    /**
     * 
     * Sentence 1: The Queen ate the pie
     * Sentence 2: The Berries were poisoned.
     * Sentence 3: [ empty sentence to give crown to dentist (?) ] 
     * 
     **/

    public GameObject crown; // Enabled upon poisoning

    public WordController pie; // Can be poisoned

    private void Start()
    {
        crown.SetActive(false);
    }

    private void Update()
    {
        // Get current value of all sentence blanks
        string[] queenBlanks = this.sentences[0].GetWords();
        string[] bushBlanks = this.sentences[1].GetWords();
        string[] dentistBlanks = this.sentences[2].GetWords();
        // Interpret values of blanks, triggering animations/effects
        this.ParseQueenSentence(queenBlanks);
        this.ParseBushSentence(bushBlanks);
        this.ParseDentistSentence(dentistBlanks);
    }

    void ParseQueenSentence(string[] blanks)
    {
        switch (blanks[0]) // switch on all words that trigger some effect
        {
            case "berry":
                this.KillQueen();
                break;
            case "poisoned_pie":
                this.KillQueen();
                break;
        }
    }

    void ParseBushSentence(string[] blanks)
    {
        switch (blanks[0])
        {
            case "pie":
                this.PoisonBerry();
                break;
        }
    }

    void ParseDentistSentence(string[] blanks)
    {
        switch (blanks[0])
        {
            case "crown":
                this.Beat_Level();
                break;

        }
    }

    void PoisonBerry()
    {
        pie.SetText("poisoned_pie");
        animator.SetTrigger("poison_pie");
    }

    void KillQueen()
    {
        animator.SetTrigger("queen_death");
        // TODO: wait for animation to finish
        crown.SetActive(true);
    }

    void Beat_Level()
    {
        // TODO
    }

}

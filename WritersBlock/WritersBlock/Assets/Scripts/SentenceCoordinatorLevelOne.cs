using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SentenceCoordinatorLevelOne : MonoBehaviour {

    /**
     * 
     * Sentence 1: The Queen ate the pie
     * Sentence 2: The Berries were poisoned.
     * Sentence 3: [ empty sentence to give crown to dentist (?) ] 
     * 
     **/

    public GameObject crown; // Enabled upon poisoning
    public GameObject queen;
    public WordController pie; // Can be poisoned

    public Sprite poisonedPie;
    public Sprite deadQueen;

    private SentenceController[] sentences;

    private void Start()
    {
        crown.SetActive(false);
        sentences = GetComponentsInChildren<SentenceController>();
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
        pie.GetComponent<SpriteRenderer>().sprite = poisonedPie;
        pie.SetText("poisoned_pie");
    }

    void KillQueen()
    {
        queen.GetComponent<SpriteRenderer>().sprite = deadQueen;
        crown.SetActive(true);
    }

    void Beat_Level()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}

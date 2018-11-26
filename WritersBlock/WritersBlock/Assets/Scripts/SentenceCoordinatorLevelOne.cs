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
    public GameObject oldText;
    public GameObject endingText;

    public Sprite poisonedPie;
    public Sprite deadQueen;
    public Sprite normalQueen;
    public Sprite emptyHandedQueen;
    public GameObject filter;


    private bool ended;
    private bool queenAlive = true;

    private SentenceController[] sentences;

    private void Start()
    {
        crown.SetActive(false);
        sentences = GetComponentsInChildren<SentenceController>();
        this.ended = false;
    }

    private void Update()
    {
        if (this.ended)
        {
            if (Input.GetButtonDown("Submit"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
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
                sentences[0].Lock();
                break;
            case "poisoned_pie":
                this.KillQueen();
                sentences[0].Lock();
                break;
            case "pie":
                if (queenAlive) {
                    queen.GetComponent<SpriteRenderer>().sprite = normalQueen; // queen holding pie
                }
                break;
            default:
                if (queenAlive) {
                    queen.GetComponent<SpriteRenderer>().sprite = emptyHandedQueen;
                }
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
                sentences[2].Lock();
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
        queenAlive = false;
        queen.GetComponent<SpriteRenderer>().sprite = deadQueen;
        sentences[0].Lock();
        crown.SetActive(true);
    }

    void Beat_Level()
    {
        oldText.SetActive(false);
        filter.SetActive(false);
        endingText.SetActive(true);
        this.ended = true;
    }

}

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
    public GameObject dentist;
    public GameObject hintToKillQueen;
    public GameObject hintAboutCrown;
    public bool berryPoisonedSoundPlayed = false;
    public bool crownGivenSoundPlayed = false;

    public Sprite poisonedPie;
    public Sprite happyDentist;
    public Sprite deadQueen;
    public Sprite normalQueen;
    public Sprite emptyHandedQueen;
    public GameObject filter;
    private PlayerController player;

    private bool ended;
    private bool queenAlive = true;
    private bool queenDeathSoundPlayed = false;

    private SentenceController[] sentences;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.currentHint = hintToKillQueen;
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
        if (!berryPoisonedSoundPlayed) {
            player.PlaySolvedSound();
            berryPoisonedSoundPlayed = true;
        }
    }


    void KillQueen()
    {
        queenAlive = false;
        player.currentHint = hintAboutCrown;
        queen.GetComponent<SpriteRenderer>().sprite = deadQueen;
        if (!queenDeathSoundPlayed) {
            player.PlayDeathSound();
            queenDeathSoundPlayed = true;
        }
        sentences[0].Lock();
        crown.SetActive(true);
    }

    void Beat_Level()
    {
        dentist.GetComponent<SpriteRenderer>().sprite = happyDentist;
        if (!crownGivenSoundPlayed) {
            player.PlaySolvedSound();
            crownGivenSoundPlayed = true;
        }
        crown.SetActive(false);
        oldText.SetActive(false);
        filter.SetActive(false);
        endingText.SetActive(true);
        this.ended = true;
    }

}

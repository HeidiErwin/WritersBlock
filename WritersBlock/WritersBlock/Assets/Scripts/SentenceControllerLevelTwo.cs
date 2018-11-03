using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SentenceCoordinatorLevelTwo : MonoBehaviour
{

    /**
     * 
     * Sentence 0: The block is ON the chest.
     * Sentence 1: No! I'm holding my STICK forever
     * Sentence 2: [received key]
     * Sentence 3: The boy takes a BREATH of fresh AIR
     * Sentence 4+: [no effects]
     * 
     **/

    public GameObject block;
    public GameObject dentist;
    public GameObject chest;
    public Sprite pointingDentist;
    public Sprite deadDentist;
    public Sprite openChest;

    private bool blockRemoved = false;

    private SentenceController[] sentences;

    private void Start()
    {
        sentences = GetComponentsInChildren<SentenceController>();
    }

    private void Update()
    {
        // Sentence One: "off"        
        if (this.sentences[0].GetWords()[0].Equals("off"))
        {
            block.SetActive(false);
            blockRemoved = true;
        }
        // Sentence Two: "breath"
        if (this.sentences[1].GetWords()[0].Equals("breath"))
        {
            dentist.GetComponent<SpriteRenderer>().sprite = deadDentist;
            BeatLevel();
        }
        // Sentence Three: "key"
        if (this.sentences[2].GetWords()[0].Equals("key") && blockRemoved)
        {
            chest.GetComponent<SpriteRenderer>().sprite = openChest;

        }
        // Sentence Four: "stick" and "gum"
        if (this.sentences[3].GetWords()[0].Equals("stick") &&
            this.sentences[3].GetWords()[1].Equals("gum"))
        {
            BeatLevel();
        }
    }

    void BeatLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}

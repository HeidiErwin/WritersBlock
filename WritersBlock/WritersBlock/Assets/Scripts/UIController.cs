using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Animator _CamAnimator;
    public GameObject startB;
    public GameObject aboutB;
    public GameObject quitB;   
    public GameObject writersBlockTitle;
    public GameObject gamePremiseText;
    private bool onFirstPage = true;

    private AudioSource source;
    public AudioClip menuSound;
    public AudioClip levelTransitionSound;
    private MegaBookBuilder bkCtrl;

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.clip = menuSound;
        source.Play(0);
    }
   
    public void QuitGame()
    {
        Application.Quit();
    }
	
	public void About()
    {
        SceneManager.LoadScene("Credits");
    }

    public void StartGame(){
        startB.SetActive(false);
        aboutB.SetActive(false);
        quitB.SetActive(false);
        writersBlockTitle.SetActive(false);
        //Object.Destroy(startB);
        //Object.Destroy(aboutB);
        //Object.Destroy(quitB);
        bkCtrl = GameObject.FindObjectOfType<MegaBookBuilder>();
        _CamAnimator.SetTrigger ("GameStartZoom");
        bkCtrl.NextPage();
        
        StartCoroutine(WaitToDisplayPremiseText());
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            bkCtrl.NextPage();
            if (onFirstPage) {
                gamePremiseText.SetActive(false);
                onFirstPage = false;
            }
            StartCoroutine(Wait());
        }
        if (Input.GetButtonDown("Cancel"))
        {
            bkCtrl.PrevPage();
            startB.SetActive(true);
            aboutB.SetActive(true);
            quitB.SetActive(true);
        }
    }

    private IEnumerator Wait()
    {
		yield return new WaitForSeconds(1);
		source.Stop();
		source.PlayOneShot(levelTransitionSound, 0.5f);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Level1");
    }

    private IEnumerator WaitToDisplayPremiseText() {
        yield return new WaitForSeconds(.8f);
        gamePremiseText.SetActive(true);
        yield break;
    }

}

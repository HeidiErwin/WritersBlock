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
    private AudioSource source;
    public AudioClip menuSound;
    public AudioClip levelTransitionSound;

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.clip = menuSound;
        source.Play(0);
    }
	
	// Update is called once per frame
	void Update () {
		
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
        Object.Destroy(startB);
        Object.Destroy(aboutB);
        Object.Destroy(quitB);
        var bkCtrl = GameObject.FindObjectOfType<MegaBookBuilder>();
        _CamAnimator.SetTrigger ("GameStartZoom");
        bkCtrl.NextPage();
        bkCtrl.NextPage();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
		yield return new WaitForSeconds(1);
		source.Stop();
		source.PlayOneShot(levelTransitionSound, 0.5f);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Intro");
    }
}

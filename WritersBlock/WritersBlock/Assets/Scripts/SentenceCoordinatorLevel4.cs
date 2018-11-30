using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SentenceCoordinatorLevel4 : MonoBehaviour {

    public GameObject didYouReally;
    public GameObject wasItWorth;
    public GameObject lookAt;

    private PlayerController player;
    int frame = 0;

    // Use this for initialization
    void Awake () {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.isLevel4 = true;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Submit")) {
            frame++;
            UpdateScreen();
        }
    }

    private void UpdateScreen() {
       switch (frame) {
            case 1:
                didYouReally.SetActive(false);
                wasItWorth.SetActive(true);
                break;
            case 2:
                wasItWorth.SetActive(false);
                lookAt.SetActive(true);
                break;
            default:
                SceneManager.LoadScene(0);
                break;
        }
    }
}

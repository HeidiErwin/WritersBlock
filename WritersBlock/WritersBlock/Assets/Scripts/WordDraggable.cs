using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordDraggable : MonoBehaviour {

    private WordController controller;
    private GameController gameController;
    public bool OnSamePageAsPlayer { get; set; }
    private Vector3 positionBeforeMove;
    private AudioSource source;
    public AudioClip wordClickSound;
    int pageBeforeDrag;

    private void Start()
    {
        controller = GetComponent<WordController>();
        source = GetComponent<AudioSource>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Update() {
        OnSamePageAsPlayer = (gameController.player.GetPage() == controller.GetPage());

        if (transform.position.x <= 0) {
            controller.SetPage(1);
        } else {
            controller.SetPage(2);
        }

    }

    private void OnMouseDown()
    {
        source.PlayOneShot(wordClickSound, 0.5f);
        positionBeforeMove = new Vector3(this.transform.position.x, this.transform.position.y);
        pageBeforeDrag = controller.GetPage();
        if (OnSamePageAsPlayer) {
            gameObject.layer = 10;
            controller.InSentence = false;
            this.transform.SetParent(null);
        }
    }

    private void OnMouseUp()
    {
        // if tried to drag word to different page than player is on, snap back to position before dragging started
        if (!OnSamePageAsPlayer) { 
            this.transform.position = positionBeforeMove;
            controller.SetPage(pageBeforeDrag);
        }
        gameObject.layer = 9;
    }

    private void OnMouseDrag()
    {
        if (pageBeforeDrag == gameController.player.GetPage()) {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            this.transform.position = pos;
        }
    }

}

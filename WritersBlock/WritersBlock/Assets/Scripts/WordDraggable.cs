using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordDraggable : MonoBehaviour {

    private WordController controller;
    private GameController gameController;
    private bool onSamePageAsPlayer;
    private Vector3 positionBeforeMove;
    int pageBeforeDrag;

    private void Start()
    {
        controller = GetComponent<WordController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Update() {
        onSamePageAsPlayer = (gameController.player.GetPage() == controller.GetPage());

        if (transform.position.x <= 0) {
            controller.SetPage(1);
        } else {
            controller.SetPage(2);
        }

    }

    private void OnMouseDown()
    {
        positionBeforeMove = new Vector3(this.transform.position.x, this.transform.position.y);
        pageBeforeDrag = controller.GetPage();
        if (onSamePageAsPlayer) {
            Debug.Log("down");
            gameObject.layer = 10;
            controller.InSentence = false;
            this.transform.parent = null;
        }
    }

    private void OnMouseUp()
    {
        // if tried to drag word to different page than player is on, snap back to position before dragging started
        if (!onSamePageAsPlayer) { 
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

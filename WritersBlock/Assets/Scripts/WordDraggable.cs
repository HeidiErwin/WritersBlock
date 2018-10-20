using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordDraggable : MonoBehaviour {

    private WordController controller;

    private void Start()
    {
        controller = GetComponent<WordController>();
    }

    private void OnMouseDown()
    {
        controller.InSentence = false;
    }

    private void OnMouseDrag()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        this.transform.position = pos;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordController : MonoBehaviour {

    public string text;
    [SerializeField] private int page = 1; //either 1 or 2, for left / right page respectively

    public bool InSentence { get; set; }

    public string GetText()
    {
        return this.text;
    }

    public void SetText(string newText)
    {
        this.text = newText;
    }

    public float GetHeight()
    {
        return this.transform.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    public void SetPage(int pg) {
        page = pg;
    }

    public int GetPage() {
        return page;
    }

}

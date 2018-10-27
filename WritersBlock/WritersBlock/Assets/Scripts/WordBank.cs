using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBank : MonoBehaviour {

    private List<WordController> wordsInBank;
    private int currentPage;

    public void SetPage(int pageNum) {
        currentPage = pageNum;
        this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y* pageNum);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour {

    [SerializeField] private GameObject wordBank;
    private WordBank wb;
    public int pageNum;

	// Use this for initialization
	void Start () {
        wb = wordBank.GetComponent<WordBank>();
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.name.Equals("Player")) {
            wb.SetPage(pageNum);
        }
    }
}

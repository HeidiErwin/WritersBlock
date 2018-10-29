using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour {

    [SerializeField] private GameObject wordBank;
   // private WordBank wb;
    public int pageNum;
    PlayerController player;

    // Use this for initialization
    void Start () {
       // wb = wordBank.GetComponent<WordBank>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        player.page = pageNum;
        
        
        // if (collision.name.Equals("Player")) {
       //     wb.SetPage(pageNum);
        //}
    }
}

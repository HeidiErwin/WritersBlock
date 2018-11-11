using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {

    private List<WordController> contents = new List<WordController>();

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Word") && !Input.GetMouseButton(0))
        {
            collision.transform.SetParent(this.transform);
            // stops inventory from blocking word
            collision.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, -2);
        }
    }

}

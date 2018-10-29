using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPrototype : MonoBehaviour {

public void DoStart()
    {
        SceneManager.LoadScene("Level1");
    }
}

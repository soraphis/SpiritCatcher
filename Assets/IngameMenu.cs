using UnityEngine;
using System.Collections;

public class IngameMenu : MonoBehaviour {

    // Use this for initialization
    void Start() {
        Game.Instance.IngameMenu = gameObject;
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

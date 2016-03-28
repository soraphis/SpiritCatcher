using UnityEngine;
using System.Collections;
using System.Reflection;

public class IngameMenu : MonoBehaviour {

    // Use this for initialization
    void Start() {
        Game.Instance.IngameMenu = gameObject;
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SaveGameButton() {
        Game.Instance.SaveGame();
        Game.Instance.ToggleIngameMenu();
    }

    public void QuitButton() {
        // ask for rly exiting
    }

    public void QuitGame() {
        Application.Quit();
    }
}

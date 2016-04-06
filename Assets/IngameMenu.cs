using System;
using System.Linq;
using Assets.Soraphis.Spirits.Scripts;
using Gamelogic;
using UnityEngine;
using UnityEngine.UI;


public class IngameMenu : MonoBehaviour {

    [Serializable]
    public struct IngameMenuUIElements {
        public Button Button_Team;
        public Button Button_Items;
        public Button Button_Save;
        public Button Button_Exit;

        public RectTransform TeamPanel;
        public RectTransform ItemPanel;

        public RectTransform SidePane;

    }

    [Layout] public IngameMenuUIElements UIElements;
    public bool activeItemSelection = false;

    // Use this for initialization
    void Start() {
        Game.Instance.IngameMenu = this.gameObject;
        gameObject.SetActive(false);
        CloseIngameMenu();
	}
	
	// Update is called once per frame
	void Update () {
        UIElements.Button_Team.gameObject.SetActive(Player.Instance.team.Count > 0);
    }

    public void CloseIngameMenu() {
        CloseTeamView();
        Game.Instance.CurrentGameState = Game.GameState.World;
        UIElements.SidePane.gameObject.SetActive(true);
        UIElements.ItemPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void CloseTeamView() {
        activeItemSelection = false;
        UIElements.TeamPanel.gameObject.SetActive(false);
    }

    public void OpenTeamView() {
        UIElements.TeamPanel.gameObject.SetActive(true);
        var children = UIElements.TeamPanel.GetChildren().Where(c => c.name == "SpiritPanel").ToArray();

        for(int i = 0; i < children.Length; i++) {
            if(i < Player.Instance.team.Count) {
                var spirit = Player.Instance.team[i];
                children[i].gameObject.SetActive(true);

                children[i].GetComponentInChildren<Text>().text = spirit.Name;

            } else {
                children[i].gameObject.SetActive(false);
            }

           
        }
    }

    public void SaveGameButton() {
        Game.Instance.SaveGame();
        Game.Instance.ToggleIngameMenu();
    }

    public void QuitButton() {
        Application.Quit(); // TODO: ask for rly exiting
    }

    public void QuitGame() {
        Application.Quit();
    }
}

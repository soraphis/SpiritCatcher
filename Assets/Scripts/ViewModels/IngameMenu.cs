using UnityEngine;
using System.Collections;
using System.Linq;
using MarkLight.Views.UI;
using UnityEngine.EventSystems;

public class IngameMenu : UIView {

	public bool hasTeam;
    public bool SaveDisabled;
    public bool QuitDisabled;

    public ViewSwitcher ContentViewSwitcher;

    public MarkLight.Views.UI.Button TeamViewButton;

    public void Start() {
        Game.Instance.IngameMenu = this.gameObject;
        gameObject.SetActive(false);
    }

    void OnEnable() {
        SetValue(() => hasTeam, Player.Instance.team.Any());
        ContentViewSwitcher.SwitchTo(0);
    }

    public void OpenItemView() {
        Debug.Log("hey");
        ContentViewSwitcher.SwitchTo(1);
    }

    public void OpenTeamView() {    ContentViewSwitcher.SwitchTo(2);    }

    public void SaveGame() {
        Debug.Log("test123");
        Game.Instance.SaveGame();
        CloseIngameMenu();
    }

    public void CloseIngameMenu() {
        Game.Instance.CurrentGameState = Game.GameState.World;
        gameObject.SetActive(false);
    }
    public void QuitGame() {
        Application.Quit();
    }
}

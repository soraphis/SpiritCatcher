using UnityEngine;
using System.IO;
using Assets.Soraphis.SaveGame;

public class MainMenu : MonoBehaviour {
    void Start() {
    }

    public void NewGameButton() {
        // write player default values:

        // load scene:
        Game.Instance.LoadLevel(3);

    }

    public void LoadGameButton() {
        try {
            using(TextReader tr = new StreamReader(Game.SaveGamePath)) {
                DataNode dn = DataNode.Read(tr);

                Game.Instance.savedData = dn;
                Game.Instance.gameObject.GetComponent<SavePacker>().Load(dn);

               /* foreach(var root in Game.SceneRoots()) {
                    var pack = root.GetComponent<SavePacker>();
                    pack?.Load(dn);
                }*/
            }
        }
        catch(FileNotFoundException) {
            Debug.Log("No Savegames found");
        }
    }

    public void QuitButton() {
        Application.Quit();
    }

}

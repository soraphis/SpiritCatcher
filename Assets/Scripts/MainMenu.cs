using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Assets.Soraphis.SaveGame;
using UnityEditor;

public class MainMenu : MonoBehaviour {

    private const string SaveGamePath = "savegame.save";


    void Start() {
    }

    public void NewGameButton() {
        // write player default values:

        // load scene:
        Game.Instance.LoadLevel("LevelScenes/spukwald");

    }

    public void LoadGameButton() {
        try {
            using(TextReader tr = new StreamReader(SaveGamePath)) {
                DataNode dn = DataNode.Read(tr);

                Game.Instance.Load(dn);

                foreach(var root in Game.SceneRoots()) {
                    var pack = root.GetComponent<SavePacker>();
                    pack?.Load(dn);
                }
            }
        }
        catch(FileNotFoundException) {
            Debug.Log("No Savegames found");
        }
    }

    public void QuitButton() {

    }

}

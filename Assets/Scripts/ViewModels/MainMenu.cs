using UnityEngine;
using System.Collections;
using System.IO;
using Assets.Soraphis.SaveGame;
using MarkLight.Views.UI;

public class MainMenu : UIView {
    public void StartGame() {
            Game.Instance.LoadLevel(3);
        }

        public void LoadGame() {
            try {
                using (TextReader tr = new StreamReader(Game.SaveGamePath)) {
                    DataNode dn = DataNode.Read(tr);
                    Game.Instance.savedData = dn;
                    Game.Instance.gameObject.GetComponent<SavePacker>().Load(dn);
                }
            } catch (FileNotFoundException) {
                Debug.Log("No Savegames found");
            }
        }

        public void QuitGame() {
            Application.Quit();
        }
}

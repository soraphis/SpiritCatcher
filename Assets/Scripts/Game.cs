using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using Assets.Soraphis.SaveGame;
using Assets.Soraphis.Spirits.Scripts;
using Gamelogic;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

[Serializable]
public struct Quest1Variables {
    public bool SendToSarim;
    public bool SendToRato;
    public bool FestivalAktive;
    public bool FestivalEnded;
    public bool BanditsAppeared;
    public bool BanditsDefeated;
    public int FestivalListened;
    public bool SendToExtractor;
}

[Serializable]
public struct DayNightColorCycle {
    public AnimationCurve red;
    public AnimationCurve gree;
    public AnimationCurve blue;
    public AnimationCurve sat;
}

public class Game : Singleton<Game>, Saveable {
    public const string SaveGamePath = "savegame.save";

    public enum GameState {
        World, Battle, Menu
    }

    public GameState CurrentGameState = GameState.Menu;
    public SpiritLibrary SpiritLibrary;
    public AttackLibrary AttackLibrary;
    public int currentScene;

    public GameDate GameTime = new GameDate(month:3, days:2, hours:12, minutes:30);
    public GameObject IngameMenu = null;
    [NonSerialized] public DataNode savedData = null;

    [Layout] public Quest1Variables QuestPart1Variables;
    [Layout] public DayNightColorCycle DayNightColorCycle;
    

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void Start() {
//        Application.LoadLevelAdditive(2);
        StartCoroutine(chronos());
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(CurrentGameState != GameState.Battle && IngameMenu != null) {
                ToggleIngameMenu();
            }
        }

        var ccc = Camera.main.GetComponent<ColorCorrectionCurves>();
        if(ccc != null) { 
            if(CurrentGameState != GameState.Battle && Player.Instance.isOutdoor) {
                var h = (GameTime.Hour*60 + GameTime.Minute)/(24f*60f);
                ccc.blueChannel = AnimationCurve.Linear(0, 0, 1, DayNightColorCycle.blue.Evaluate(h));
                ccc.redChannel = AnimationCurve.Linear(0, 0, 1, DayNightColorCycle.red.Evaluate(h));
                ccc.greenChannel = AnimationCurve.Linear(0, 0, 1, DayNightColorCycle.gree.Evaluate(h));
                ccc.saturation = DayNightColorCycle.sat.Evaluate(h);
            } else {
                ccc.blueChannel = AnimationCurve.Linear(0, 0, 1, 1);
                ccc.redChannel = AnimationCurve.Linear(0, 0, 1, 1);
                ccc.greenChannel = AnimationCurve.Linear(0, 0, 1, 1);
                ccc.saturation = 1;
            }
            ccc.UpdateParameters();
        }

    }

    public void ToggleIngameMenu() {
        if(IngameMenu.activeSelf) IngameMenu.GetComponent<IngameMenu>().CloseIngameMenu();
        else IngameMenu.SetActive(true);

        CurrentGameState = IngameMenu.activeSelf ? GameState.Menu : GameState.World;
    }

    private IEnumerator chronos() {
        while (true) {
            if(CurrentGameState == GameState.World) {
                yield return new WaitForSeconds(1f);
                GameTime += 1;
            } else
                yield return null;
        }
    }

    private void OnLevelWasLoaded(int level) {
        Debug.Log($"on level was loaded ({level})");
        if (level <= 2) return;
        if(savedData != null) {
            currentScene = level;
        }
    }

    public void LoadLevel(string level) {
        LoadLevel(SceneManager.GetSceneByName(level).buildIndex);
    }

    public void LoadLevel(int level) {
        // "unload" old level
        SceneManager.UnloadScene(currentScene);
        if (currentScene == 0) {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        CurrentGameState = GameState.World;
        SceneManager.LoadScene(level, LoadSceneMode.Additive);
        currentScene = level;
    }

    public void Load(DataNode parent) {
        DataNode node = parent.GetChild("Game");
        if(node == null) return;
        GameTime = new GameDate(node.GetChild("Time").Get<ulong>());

        int loadscene = node.GetChild("Scene").Get<int>();
        if(loadscene <= 2) throw new ApplicationException("save file seems corrupt");

        var n1 = node.GetChild("Quest1Status");
        QuestPart1Variables.FestivalAktive = n1.GetChild("FestivalAktive").Get<bool>();
        QuestPart1Variables.FestivalEnded = n1.GetChild("FestivalEnded").Get<bool>();
        QuestPart1Variables.SendToSarim = n1.GetChild("SendToSarim").Get<bool>();
        QuestPart1Variables.SendToRato = n1.GetChild("SendToRato").Get<bool>();
        QuestPart1Variables.BanditsAppeared = n1.GetChild("BanditsAppeared").Get<bool>();
        QuestPart1Variables.BanditsDefeated = n1.GetChild("BanditsDefeated").Get<bool>();
        QuestPart1Variables.FestivalListened = n1.GetChild("FestivalListened").Get<int>();
        QuestPart1Variables.SendToExtractor = n1.GetChild("SendToExtractor").Get<bool>();

        //        savedData = parent;
        LoadLevel(loadscene);
        StartCoroutine(AfterLoad(loadscene));
    }

    private IEnumerator AfterLoad(int loadscene) {
        while (SceneManager.GetAllScenes().All(s => s.buildIndex != loadscene)) {
            yield return null;
        }
        yield return null;

        foreach (var go in GameObject.FindObjectsOfType<SavePacker>()) {
            print(go.name);
            if (go.transform.parent != null) continue;
            if (go.transform == this.transform) continue;
            go.GetComponent<SavePacker>().Load(savedData);
        }
    }

    public DataNode Save() {
        DataNode node = new DataNode();
        node.Name = "Game";

        node.AddChild("Time", GameTime.Time);
        node.AddChild("Scene", currentScene);

        var n1 = node.AddChild("Quest1Status");
        n1.AddChild("FestivalAktive", QuestPart1Variables.FestivalAktive);
        n1.AddChild("FestivalEnded", QuestPart1Variables.FestivalEnded);
        n1.AddChild("SendToSarim", QuestPart1Variables.SendToSarim);
        n1.AddChild("SendToRato", QuestPart1Variables.SendToRato);
        n1.AddChild("BanditsAppeared", QuestPart1Variables.BanditsAppeared);
        n1.AddChild("BanditsDefeated", QuestPart1Variables.BanditsDefeated);
        n1.AddChild("FestivalListened", QuestPart1Variables.FestivalListened);
        n1.AddChild("SendToExtractor", QuestPart1Variables.SendToExtractor);

        return node;
    }

    public void CreateDefault() {
        throw new NotImplementedException();
    }


    public void SaveGame(bool writeSaveFile = true) {
        savedData = savedData ?? new DataNode();

        foreach(var go in GameObject.FindObjectsOfType<SavePacker>()) {
            if(go.transform.parent != null) continue;
            DataNode node = go.GetComponent<SavePacker>().Save();
            savedData.GetChild(node.Name, true).Merge(node, true);
        }

        if(!writeSaveFile) return; // just update "savedData"

        using (StreamWriter sw = new StreamWriter(SaveGamePath)) {
            savedData.Write(sw);
        }
    }

}
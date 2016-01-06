using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Soraphis.SaveGame;
using Assets.Soraphis.Spirits.Scripts;
using Gamelogic;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>, Saveable {
    public const string SaveGamePath = "savegame.save";

    public enum GameState {
        World, Battle, Menu
    }

    public GameState CurrentGameState = GameState.Menu;
    public SpiritLibrary SpiritLibrary;
    public int currentScene;

    public GameDate GameTime = new GameDate(month:3);
    public GameObject IngameMenu = null;
    [NonSerialized] public DataNode savedData = null;

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
                IngameMenu.SetActive(!IngameMenu.activeSelf);
                CurrentGameState = IngameMenu.activeSelf ? GameState.Menu : GameState.World;
            }
        }
    }

    private IEnumerator chronos() {
        while (true) {
            if(CurrentGameState == GameState.World) {
                yield return new WaitForSeconds(1);
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

        var n1 = node.AddChild("Time", GameTime.Time);
        var n2 = node.AddChild("Scene", currentScene);

        return node;
    }

    public void CreateDefault() {
        throw new NotImplementedException();
    }


    public static IEnumerable<GameObject> SceneRoots() {
        var prop = new HierarchyProperty(HierarchyType.GameObjects);
        var expanded = new int[0];
        while (prop.Next(expanded)) {
            yield return prop.pptrValue as GameObject;
        }
    }

    public void SaveGame() {
        savedData = savedData ?? new DataNode();

        foreach(var go in GameObject.FindObjectsOfType<SavePacker>()) {
            if(go.transform.parent != null) continue;
            DataNode node = go.GetComponent<SavePacker>().Save();
            savedData.GetChild(node.Name, true).Merge(node, true);
        }

        using (StreamWriter sw = new StreamWriter(SaveGamePath)) {
            savedData.Write(sw);
        }
    }
}
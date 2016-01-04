using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Soraphis.SaveGame;
using Assets.Soraphis.Spirits.Scripts;
using Gamelogic;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>, Saveable {

    public enum GameState {
        World, Battle, Menu
    }

    public GameState CurrentGameState = GameState.Menu;
    public SpiritLibrary SpiritLibrary;
    public Scene currentScene;

    public GameDate GameTime = new GameDate(month:3);

    public GameObject IngameMenu = null;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        currentScene = SceneManager.GetActiveScene();
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

    public void LoadLevel(string level) {
        // "unload" old level
        SceneManager.UnloadScene(currentScene.buildIndex);
        if (currentScene.buildIndex == 0) {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        CurrentGameState = GameState.World;
        SceneManager.LoadScene(level, LoadSceneMode.Additive);
    }

    public void Load(DataNode parent) {
        DataNode node = parent.GetChild("Game");
        if(node == null) return;
        GameTime = node.GetChild("Time").Get<GameDate>();
        currentScene = SceneManager.GetSceneAt(node.GetChild("Scene").Get<int>());

        LoadLevel(currentScene.name);
    }

    public DataNode Save() {
        DataNode node = new DataNode();
        node.Name = "Game";

        node.AddChild("Time", GameTime);
        node.AddChild("Scene", currentScene.buildIndex);

        return node;
    }

    public void CreateDefault() {
        throw new System.NotImplementedException();
    }


    public static IEnumerable<GameObject> SceneRoots() {
        var prop = new HierarchyProperty(HierarchyType.GameObjects);
        var expanded = new int[0];
        while (prop.Next(expanded)) {
            yield return prop.pptrValue as GameObject;
        }
    }
}
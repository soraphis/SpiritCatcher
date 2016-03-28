using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Soraphis.SaveGame;
using Assets.Soraphis.Spirits.Scripts;
using Gamelogic;

public class Player : Singleton<Player>, Saveable {

    public enum ActionState {
        Default, Talking, Battle, Cutszene // dont know .... 
    }

    public void Start() {
        if(team == null || team.Count == 0) {
            team = new List<Spirit> {
                Spirit.GenerateSpirit(Game.Instance.SpiritLibrary.Spirits[0], 5)
            };
        }
    }

    public ActionState CurrentActionState = ActionState.Default;
    public List<Spirit> team;


    public void Load(DataNode parent) {
        DataNode node = parent.GetChild("Player");
        if (node == null) return;
        team = node.GetChild("team").Get<List<Spirit>>();
    }

    public DataNode Save() {
        DataNode node = new DataNode();
        node.Name = "Player";

        node.AddChild("team", team);

        return node;
    }

    public void CreateDefault() {
        throw new System.NotImplementedException();
    }
}

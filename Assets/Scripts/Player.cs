using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Soraphis.Spirits.Scripts;
using Gamelogic;

public class Player : Singleton<Player> {

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



}

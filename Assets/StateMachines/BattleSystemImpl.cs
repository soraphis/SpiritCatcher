using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public delegate void OnWeatherEffectProc(BattleObject obj, Action onEndProc);

public struct Buff {
    public string Name;
    public string Attribute;
    public float Value;
    public float Duration; // measured in "speed" -> 100 ~ 1 round // -1 forever (this fight)
    public int Stacks;
}

public class PlayerAction {
    public enum ActionType {
        None,
        Attack,
        Item,
        SwitchSpirit,
        Run
    }

    public readonly ActionType action;
    public readonly int? actionValue;

    public PlayerAction(int? actionValue, ActionType action) {
        this.actionValue = actionValue;
        this.action = action;
    }
}

public class WeatherEffect {
//    public static WeatherEffect PilzSporenSturm = new WeatherEffect("PilzSporenSturm", bo => {
//        var p = new Promise();
//        Spirit paraspor_cladis = null;
//        if(bo.GetSpirit(0).Type.Name == "Paraspor Cladis") {
//            paraspor_cladis = bo.GetSpirit(0);
//        }
//        if(bo.GetSpirit(1).Type.Name == "Paraspor Cladis") {
//            paraspor_cladis = bo.GetSpirit(1);
//        }
//        if(paraspor_cladis != null) {
//            // show text ... maybe //-> then p.resolve
//            paraspor_cladis.CurrentHP += 10f;
//            
//        }
//        p.Resolve();
//        return p;
//    }) {CanBeReplaced = false, Duration = -1, Speed = 15};

    // ------
    public readonly string Name;
    public float Speed;
    public float Stamina = 0;
    public readonly OnWeatherEffectProc Proc;
    public float Duration = -1; // measured in "speed" -> 100 ~ 1 round // -1 forever (this fight)
    public bool CanBeReplaced = false;

    public WeatherEffect(string name, OnWeatherEffectProc proc) {
        Proc = proc;
        Name = name;
    }
}

public abstract class BattleActionHandler {
    protected BattleObject BattleObject;
    protected int player_id = -1;

    public BattleActionHandler(BattleObject bo) {
        this.BattleObject = bo;
    }

    public abstract IEnumerator OnTurn(Action action); // simple turn
    public abstract IEnumerator OnSpiritDown(Action action); // choose other spirit
}
//
public class AIBattleActionHandler : BattleActionHandler {
    public override IEnumerator OnTurn(Action action) {
        BattleObject.action[player_id] = new PlayerAction(
            UnityEngine.Random.Range(0, BattleObject.GetSpirit(player_id).Attacks.Count)
            , PlayerAction.ActionType.Attack);
        yield return new WaitForEndOfFrame();
        action.Invoke();
    }

    public override IEnumerator OnSpiritDown(Action action) {
        BattleObject.action[player_id] = new PlayerAction(
            Array.IndexOf(BattleObject.GetTeam(player_id).Spirits.ToArray(),
                          BattleObject.GetTeam(player_id).Spirits.First(s => s.CurrentHP > 0))
            , PlayerAction.ActionType.SwitchSpirit);
        yield return new WaitForEndOfFrame();
        action.Invoke();
    }

    public AIBattleActionHandler(BattleObject bo, int player_id = 1) : base(bo) {
        this.player_id = player_id;
    }
}

public class PlayerBattleActionHandler : BattleActionHandler {

    public PlayerBattleActionHandler(BattleObject bo, int player_id = 0) : base(bo) {
        this.player_id = player_id;
    }

    public override IEnumerator OnTurn(Action action) {
        BattleController.Instance.MessageBox.gameObject.SetActive(false);
        BattleController.Instance.BasicActionsPanel.SetActive(true);
        BattleController.Instance.action = null;
        yield return null;
        yield return new WaitUntil(() => BattleController.Instance.action != null && BattleController.Instance.action.action == PlayerAction.ActionType.Attack && BattleController.Instance.action.actionValue > -1);
        BattleObject.action[player_id] = BattleController.Instance.action;

        yield return new WaitForEndOfFrame();
        BattleController.Instance.BasicActionsPanel.SetActive(false);
        BattleController.Instance.AttackOptionsPanel.SetActive(false);
        action.Invoke();
    } 

    public override IEnumerator OnSpiritDown(Action action) {

        yield return null;
        // copy from AI:
        BattleObject.action[player_id] = new PlayerAction(
          Array.IndexOf(BattleObject.GetTeam(player_id).Spirits.ToArray(),
                        BattleObject.GetTeam(player_id).Spirits.First(s => s.CurrentHP > 0))
          , PlayerAction.ActionType.SwitchSpirit);

        yield return new WaitForEndOfFrame();
        action.Invoke();
    } 
}
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RSG;
using UnityEngine;

public delegate Promise OnWeatherEffectProc(BattleObject obj);

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
//
//public class UserBattleActionHandler : BattleActionHandler {
////    private readonly BattleSystemImpl BUI;
//
//    public override IEnumerator OnTurn() {
////        yield return new WaitUntil(() => BattleObject.action != null);
//    }
//
//    public override IEnumerator OnSpiritDown() {
////        yield return new WaitUntil(() => BattleObject.action != null);
//    }
//
//    public UserBattleActionHandler(BattleObject bo, BattleSystemImpl BUI, int player_id = 0) : base(bo) {
//        this.BUI = BUI;
//        this.player_id = player_id;
//    }
//}


//using System;
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Assets.Soraphis.Spirits.Scripts;
//using DG.Tweening;
//using Gamelogic;
//using JetBrains.Annotations;
//using RSG;
//using Soraphis.BattleSystem;
//using UnityEngine.UI;
//using BattleObject = Soraphis.BattleSystem.BattleObject;
//using Debug = UnityEngine.Debug;
//using Random = UnityEngine.Random;
//
//public class BattleSystemImpl : Singleton<BattleSystemImpl> {
//
//    public Image[] Spirits = new Image[2];
//
//    public RectTransform[] StaminaImages = new RectTransform[2];
//    public Image[] HPBar = new Image[2];
//    public Text[] HPText = new Text[2];
//
//    public RectTransform StaminaBar;
//    public GameObject ActionPanel;
//
//    private Text ActionPanel_Message;
//
//    private Soraphis.BattleSystem.BattleObject battleObject;
//
//    [SerializeField] private SpiritTeam t1;
//    [SerializeField] private SpiritTeam t2;
//
//    public void Start() {
//        ActionPanel_Message = ActionPanel.transform.FindChild("MessageBox").GetComponent<Text>();
//        Spirits[0].enabled = false;
//        Spirits[1].enabled = false;
//    }
//
//    public void ButtonChooseAttack(int attack) { 
//        battleObject.action = new PlayerAction(attack, PlayerAction.ActionType.Attack);
//    }
//
//    public void ButtonTeam() {
//        // show team menu
//    }
//
//    public void ButtonChooseSpirit(int spirit_index) {
//    }
//
//    public void Update() {
//        if(battleObject == null && Input.GetKeyDown(KeyCode.T)) {
//            battleObject = new Soraphis.BattleSystem.BattleObject(t1, t2);
//            battleObject.Start();
//        }
//
//        battleObject?.Update();
//
//        // update hp bars and so on
//
//    }
//
//    public IEnumerator WriteMessage(string message, Promise p = null) {
//        ActionPanel.SetActive(true);
//
//        ActionPanel.transform.GetChildren().ForEach(c => c.gameObject.SetActive(false));
//
//        ActionPanel_Message.gameObject.SetActive(true);
//        ActionPanel_Message.text = message;
//
//        yield return new WaitForEndOfFrame();
//
//        p?.Resolve();
//    }
//
//
//    public void StartBattle(Soraphis.BattleSystem.BattleObject obj) {
//        battleObject = obj;
//    }
//
//}
//
//
//namespace Soraphis.BattleSystem {
//    
//
//    public class PlayerAction {
//        public enum ActionType { None, Attack, Item, SwitchSpirit, Run }
//        public readonly ActionType action;
//        public readonly int? actionValue;
//
//        public PlayerAction(int? actionValue, ActionType action) {
//            this.actionValue = actionValue;
//            this.action = action;
//        }
//    }
//

//
////    public class BattleObject {
////        private readonly bool trainerFight;
////        public readonly NPC trainer;
////
////        public readonly SpiritTeam enemyTeam;
////        public readonly SpiritTeam playerTeam;
////
////        private Spirit playerSpirit;
////        private Spirit enemySpirit;
////
////        public BattleActionHandler[] ActionHandlers;
////
////        private readonly BattleSystemImpl BUI;
////
////        private WeatherEffect weatherEffect = null;
////        private Dictionary<Spirit, List<Buff>> Buffs = new Dictionary<Spirit, List<Buff>>();
////
////        private int? activePlayer = null;
////        public PlayerAction action;
////
////        public BattleObject(SpiritTeam playerTeam, SpiritTeam enemyTeam, NPC trainer = null) {
////            this.playerTeam = playerTeam;
////            this.enemyTeam = enemyTeam;
////
////            if(trainer != null) {
////                trainerFight = false;
////                this.trainer = trainer;
////            }
////
////            ActionHandlers = new BattleActionHandler[] {
////                new UserBattleActionHandler(this, BUI),
////                new AIBattleActionHandler(this, 1)
////            };
////        }
////
////        public Spirit GetSpirit(int i) => i == 0 ? playerSpirit : enemySpirit;
////        private void SetSpirit(int i, Spirit s) {
////            if (i == 0) playerSpirit = s;
////            else enemySpirit = s;
////        }
////
////        public SpiritTeam GetTeam(int i) => i == 0 ? playerTeam : enemyTeam;
////
////        private bool started = false;
////
////        private BattleState _battleState;
////        private BattleState battleState {
////            get { return _battleState; }
////            set {
////                if(_battleState == value) return;
////                var stateName = Enum.GetName(typeof(BattleState), battleState);
////                var method = this.GetType().GetMethod("On" + stateName + "Exit", BindingFlags.Instance | BindingFlags.NonPublic);
////                method?.Invoke(this, new object[] { });
////
////                _battleState = value;
////                var x = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
////                
////                stateName = Enum.GetName(typeof(BattleState), battleState);
////
////                var b = x.Any(m => m.Name == "On" + stateName + "Enter");
////                var t = x.First(m => m.Name == "On" + stateName + "Enter");
////
////                method = this.GetType().GetMethod("On" + stateName + "Enter", BindingFlags.Instance | BindingFlags.NonPublic);
////                method?.Invoke(this, new object[] {});
////            }
////        }
////
////        private enum BattleState {
////            BattleStart, SpawnWildSpirits, BattleStartMessage, SpawnSpirits,
////            GenerateEnergy,
////            EvaluateBattle,
////            PlayerAction
////        }
////
////        public void Start() {
////            // is valid battle?
////            started = true;
////            battleState = BattleState.BattleStart;
////        }
////
////        private float GetBattleSpeed(Spirit s) {
////            if (Buffs.ContainsKey(s))
////                return (s.SPEED + Buffs[s].Where(b => b.Attribute == "speed").Select(b => b.Value).Sum()) * SpiritType.PP_SPEED;
////            return s.SPEED * SpiritType.PP_SPEED;
////        }
////
////        public void Update() {
////            if(!started) return;
////            var stateName = Enum.GetName(typeof(BattleState), battleState);
////
////            var x = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
////
////            var method = this.GetType().GetMethod(stateName + "Update", BindingFlags.Instance | BindingFlags.NonPublic);
////            method?.Invoke(this, new object[] {});
////        }
////
////        private IEnumerator SpawnSpirit(int team, int index) {
////            // wait for spawn spirits
////
////            if (GetSpirit(team) == GetTeam(team).Spirits[index]) {
////                if (GetSpirit(team).currentHP > 0) {
////                    // spirit is already in fight
////                } else {
////                    
////                    Debug.LogError("Unhandled situation ... (to lazy for now)");
////                }
////            }
////
////            SetSpirit(team, GetTeam(team).Spirits[index]);
////            var tween = BUI.Spirits[team].DOFade(1, 0.3f).From()
////                            .OnStart(() => {
////                                BUI.Spirits[team].enabled = true;
////                                BUI.Spirits[team].sprite = GetSpirit(team).Type.Image;
////                            });
////            yield return tween.WaitForCompletion();
////
////            /*
////              private IPromise SpawnSpirit(int team, int index) {
////                        if(GetSpirit(team) == GetTeam(team).Spirits[index]) {
////                            if(GetSpirit(team).currentHP > 0)
////                                return Promise.Resolved();
////                            else {
////                                Debug.LogError("Unhandled situation ... (to lazy for now)");
////                            }
////                        }
////
////                        var p = new Promise();
////                        SetSpirit(team, GetTeam(team).Spirits[index]);
////                        BUI.Spirits[team].DOFade(1, 0.3f).From()
////                            .OnStart(() => BUI.Spirits[team].sprite = GetSpirit(team).Type.Image)
////                            .OnComplete(() => p.Resolve());
////                        return p;
////                    }
////
////            */
////
////        }
////
////        [UsedImplicitly]
////        private void BattleStartUpdate() {
////            if(trainerFight) battleState = BattleState.BattleStartMessage;
////            else battleState = BattleState.SpawnWildSpirits;
////            Debug.Log("battle started");
////        }
////
////        [UsedImplicitly]
////        private IEnumerator OnSpawnWildSpiritsEnter() {
////            Debug.Log("spawn wild spirits");
////            yield return SpawnSpirit(1, 0);
////            battleState = BattleState.BattleStartMessage;
////        }
////
////        [UsedImplicitly]
////        private void OnBattleStartMessageEnter() {
////            Debug.Log("hey");
////        }
////
////        [UsedImplicitly]
////        private IEnumerator BattleStartMessageUpdate() {
////            started = false;
////            yield return BUI.StartCoroutine(BUI.WriteMessage("message"));
////            battleState = BattleState.SpawnSpirits;
////            started = true;
////        }
////
////        [UsedImplicitly]
////        private IEnumerator OnSpawnSpiritsEnter() {
////            started = false;
////            yield return SpawnSpirit(0, 0);
////            if(trainerFight) yield return SpawnSpirit(1, 0);
////            battleState = BattleState.GenerateEnergy;
////            started = true;
////        }
////
////        private void WeatherEffectEnd() {
////            // maybe print a message ?! 
////            weatherEffect = null;
////        }
////
////        [UsedImplicitly]
////        private void GenerateEnergyUpdate() {
////            var maxSpeed = Mathf.Max(GetBattleSpeed(GetSpirit(0)), GetBattleSpeed(GetSpirit(1)));
////            if(weatherEffect != null) maxSpeed = Mathf.Max(maxSpeed, weatherEffect.Speed);
////            var time_multiplicator = 100/maxSpeed;
////
////            for(int i = 0; i <= 1; ++i) {
////                GetSpirit(i).CurrentStamina += GetBattleSpeed(GetSpirit(i))*Time.deltaTime*time_multiplicator;
////
////                var v = BUI.StaminaImages[i].localPosition;
////                v.x = GetSpirit(i).CurrentStamina/100*BUI.StaminaBar.rect.width - BUI.StaminaBar.rect.width/2;
////                BUI.StaminaImages[i].localPosition = v;
////            }
////            if(weatherEffect != null) {
////                weatherEffect.Stamina += weatherEffect.Speed*Time.deltaTime*time_multiplicator;
////                if(weatherEffect.Duration > 0) {
////                    weatherEffect.Duration -= weatherEffect.Speed*Time.deltaTime*time_multiplicator;
////                    if(weatherEffect.Duration <= 0) WeatherEffectEnd();
////                }
////            }
////
////            if(GetSpirit(0).CurrentStamina >= 100) {
////                // player turn
////                activePlayer = 0;
////                battleState = BattleState.PlayerAction;
////            } else if(GetSpirit(1).CurrentStamina >= 100) {
////                // enemy turn
////                activePlayer = 1;
////                battleState = BattleState.PlayerAction;
////            } else if(weatherEffect != null && weatherEffect.Stamina >= 100) {
////                // weather turn
////                weatherEffect.Proc(this);
////                battleState = BattleState.EvaluateBattle;
////            }
////        }
////
////        [UsedImplicitly]
////        private IEnumerator OnPlayerActionEnter() {
////            if(activePlayer == null) throw new ArgumentException();
////            bool TurnIllegal = false;
////            do {
////                TurnIllegal = false;
////                yield return ActionHandlers[(int) activePlayer].OnTurn();
////                 if(action.action == PlayerAction.ActionType.Run && trainerFight) {
////                     TurnIllegal = true;
////                 }
////                yield return null;
////            } while(TurnIllegal);
////            activePlayer = null;
////            // ****
////            // do action, e.g. attack / switch spirit
////            if(action.action == PlayerAction.ActionType.Attack) {
////                Debug.Log(GetSpirit((int)activePlayer).Attacks[activePlayer.Value].Name);
////            }else if(action.action == PlayerAction.ActionType.SwitchSpirit) {
////                
////            }else if(action.action == PlayerAction.ActionType.Run) {
////                
////            }
////
////            // ****
////            // TODO: catch spirit
////            battleState = BattleState.EvaluateBattle;
////        }
////
////        private IEnumerator OnEvaluateBattleEnter() {
////            if(GetSpirit(0).currentHP > 0 && GetSpirit(1).currentHP > 0) {
////                // no spirit defeated
////                battleState = BattleState.GenerateEnergy;
////                yield break;
////            }
////            for(int i = 1; i >= 0; --i) {
////                if(GetSpirit(i).currentHP <= 0) {
////                    do {
////                        yield return ActionHandlers[i].OnSpiritDown();
////                    } while(
////                        action.action != PlayerAction.ActionType.SwitchSpirit &&
////                        action.actionValue != null &&
////                        GetTeam(i).Spirits[(int) action.actionValue].currentHP > 0
////                    );
////                    yield return SpawnSpirit(i, (int)action.actionValue);
////                }
////            }
////            yield return null;
////
////        }
////
////    }
//
//    /*
//
//    public class BattleObject {
//        private readonly SpiritTeam enemyTeam;
//        private NPC enemy;
//
//        private readonly bool trainerFight;
//        private readonly SpiritTeam playerTeam;
//
//        private readonly BattleSystemImpl BUI;
//        private WeatherEffect weatherEffect = null;
//
//        private Spirit playerSpirit;
//        private Spirit enemySpirit;
//        private Dictionary<Spirit, List<Buff>> Buffs = new Dictionary<Spirit, List<Buff>>();
//        private bool genEnergy = false;
//
//        private readonly Promise<object>[] staminaFull = new Promise<object>[2];
//
//        private Spirit GetSpirit(int i) => i == 0 ? playerSpirit : enemySpirit;
//        private void SetSpirit(int i, Spirit s) {
//            if(i == 0) playerSpirit = s;
//            else enemySpirit = s;
//        }
//
//        private SpiritTeam GetTeam(int i) => i == 0 ? playerTeam : enemyTeam;
//        public BattleObject(SpiritTeam enemyTeam, bool trainerFight = false) {
//            this.enemyTeam = enemyTeam;
//            this.trainerFight = trainerFight;
//            this.playerTeam = new SpiritTeam();
//            playerTeam.Spirits = new Spirit[Mathf.Min(6, Player.Instance.team.Count)];
//            for (int i = 0; i < playerTeam.Spirits.Length; ++i) {
//                playerTeam.Spirits[i] = (Player.Instance.team[i]);
//            }
//            
//        }
//
//
//        public IEnumerator Battle() {
//            var b = false;
//            SpawnWildSpirits()
//                .Then(_ => BattleStartMessage())
//                .Then(_ => SpawnSpirits())
//                .Then(() => b = true)
//                .Done();
//
//            yield return new WaitUntil(() => b);
//            // battle loop!
//            b = false;
//
//            BattleLoop(ref b);
//
//            genEnergy = true;
//            while (!b) {
//                if(genEnergy) {
//                    GenerateEnergy();
//                }
//
//                yield return null;
//            }
//
//
////            while (evalBattle) {
////                yield return null;
////
////                GenerateEnergy().Done();
////
////
////                evalBattle = false;
////            }
//
//            // end of battle loop
//            yield return new WaitUntil(() => b);
//        }
//
//        private IPromise ExcecuteAction(object o) {
//            var p = new Promise();
//            genEnergy = false;
//            if(o is Spirit) {
//                if(o == GetSpirit(0)) {
//                    // action from player
//                    BUI.UserAction = new Promise<PlayerAction>();
//                    BUI.UserAction.Then(action => {
//
//                        switch() {
//                        }
//
//
//                    }).Then(action => p.Resolve()).Done();
//                }else if(o == GetSpirit(1)) {
//                    // action from AI
//                } else {
//                    throw new ArgumentException("invalid object");
//                }
//            }else if(o is WeatherEffect && o == weatherEffect) {
//                (o as WeatherEffect).Proc(this);
//                p.Resolve();
//            } else {
//                throw new ArgumentException("invalid object");
//            }
//
//            return p;
//        }
//
//        private void BattleLoop(ref bool b) {
//            Promise.Resolved()
//                .ThenRace(() => new IPromise<object>[] {
//                    staminaFull[0],
//                    staminaFull[1],
//                    weatherEffect?.staminaFull
//                })
//                .Then((o) => ExcecuteAction(o))
//                .Done();
//        }
//
//        private float GetBattleSpeed(Spirit s) {
//            if(Buffs.ContainsKey(s))
//                return (s.SPEED + Buffs[s].Where(b => b.Attribute == "speed").Select(b => b.Value).Sum()) * SpiritType.PP_SPEED;
//            return s.SPEED*SpiritType.PP_SPEED;
//        }
//
//        private void GenerateEnergy() {
//            var maxSpeed = Mathf.Max(GetBattleSpeed(GetSpirit(0)), GetBattleSpeed(GetSpirit(1)));
//            if(weatherEffect != null) maxSpeed = Mathf.Max(maxSpeed, weatherEffect.Speed);
//            var time_multiplicator = 100 / maxSpeed;
//
//            for (int i = 0; i <= 1; ++i) {
//                GetSpirit(i).CurrentStamina += GetBattleSpeed(GetSpirit(i)) * Time.deltaTime * time_multiplicator;
//                staminaFull[i] = staminaFull[i] ?? new Promise<object>();
//
//                if(GetSpirit(i).CurrentStamina >= 100) staminaFull[i].Resolve(GetSpirit(i));
//
//                var v = BUI.StaminaImages[i].localPosition;
//                v.x = GetSpirit(i).CurrentStamina / 100 * BUI.StaminaBar.rect.width - BUI.StaminaBar.rect.width / 2;
//                BUI.StaminaImages[i].localPosition = v;
//            }
//            if (weatherEffect != null) { 
//                weatherEffect.Stamina += weatherEffect.Speed * Time.deltaTime * time_multiplicator;
//                if (weatherEffect.Stamina >= 100) weatherEffect.staminaFull.Resolve(weatherEffect);
//            }
//        }
//
//        private IPromise SpawnSpirit(int team, int index) {
//            if(GetSpirit(team) == GetTeam(team).Spirits[index]) {
//                if(GetSpirit(team).currentHP > 0)
//                    return Promise.Resolved();
//                else {
//                    Debug.LogError("Unhandled situation ... (to lazy for now)");
//                }
//            }
//
//            var p = new Promise();
//            SetSpirit(team, GetTeam(team).Spirits[index]);
//            BUI.Spirits[team].DOFade(1, 0.3f).From()
//                .OnStart(() => BUI.Spirits[team].sprite = GetSpirit(team).Type.Image)
//                .OnComplete(() => p.Resolve());
//            return p;
//        }
//
//        public Promise<bool> SpawnWildSpirits() {
//            Promise<bool> b = new Promise<bool>();
//            if(trainerFight) b.Resolve(false);
//            else {
//                SpawnSpirit(1, 0).Then(() => b.Resolve(true));
////                enemySpirit = enemyTeam.Spirits[0];
////                BUI.Spirits[1].DOFade(1, 0.3f).From()
////                    .OnStart(() => BUI.Spirits[1].sprite = enemySpirit.Type.Image)
////                    .OnComplete(() => b.Resolve(true));
//            }
//            return b;
//        }
//
//        public Promise<bool> BattleStartMessage() {
//            Promise<bool> b = new Promise<bool>();
//            BUI.StartCoroutine(BUI.WriteMessage("message", b));
//            return b;
//        }
//
//        IPromise SpawnSpirits() {
//            var b = new Promise();
//
//            SpawnSpirit(1, 0)
//                .Then(() => SpawnSpirit(0, 0))
//                .Then(() => b.Resolve())
//                .Done();
//
//
//            return b;
//        }
//
//    }
//    */
//}
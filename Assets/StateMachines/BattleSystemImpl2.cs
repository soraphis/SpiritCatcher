// #define BUI BattleController.Instance

using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Soraphis.Spirits.Scripts;
using DG.Tweening;
using props = BattleSystemProperties;
using BUI = BattleController;


public class BattleObject {
    public readonly bool trainerFight;
    public readonly GameObject trainer;

    public readonly SpiritTeam enemyTeam;
    public readonly SpiritTeam playerTeam;

    public Spirit playerSpirit;
    public Spirit enemySpirit;

    public BattleActionHandler[] ActionHandlers;

    public WeatherEffect weatherEffect = null;
    public Dictionary<Spirit, List<Buff>> Buffs = new Dictionary<Spirit, List<Buff>>();

    public int? activePlayer = null;
    public PlayerAction[] action = new PlayerAction[2];

    public int winner = -1;

    public BattleObject(SpiritTeam enemyTeam, SpiritTeam playerTeam = null, GameObject trainer = null) {
        this.enemyTeam = enemyTeam;
        if(playerTeam == null) {
            playerTeam = new SpiritTeam();
            playerTeam.Spirits = new List<Spirit>();
            for(int i = 0; i < Mathf.Min(6, Player.Instance.team.Count); ++i) {
                playerTeam.Spirits[i] = (Player.Instance.team[i]);
            }
        } else {
            this.playerTeam = playerTeam;
        }

        this.trainer = trainer;
        trainerFight = trainer != null;
    }

    public Spirit GetSpirit(int i) => i == 0 ? playerSpirit : enemySpirit;

    public void SetSpirit(int i, Spirit s) {
        if(i == 0) playerSpirit = s;
        else enemySpirit = s;
    }

    public SpiritTeam GetTeam(int i) => i == 0 ? playerTeam : enemyTeam;

    public float GetBattleSpeed(Spirit s) {
        float bonus_speed = 0;
        if(Buffs.ContainsKey(s))
            bonus_speed = s.SPEED + Buffs[s].Where(b => b.Attribute == "speed").Select(b => b.Value).Sum();

        return Mathf.Log(1 + s.SPEED + bonus_speed)*(SpiritType.PP_SPEED);
    }
}

public class BattleSystemImpl2 : MonoBehaviour, IBattleSystemBattleStartHandler, IBattleSystemBattleStartMessageHandler,
    IBattleSystemCatchSpiritHandler, IBattleSystemEndHandler, IBattleSystemEnemyWinHandler,
    IBattleSystemEvaluateBattleHandler, IBattleSystemGenerateEnergyHandler, IBattleSystemPlayerActionHandler,
    IBattleSystemPlayerWinHandler, IBattleSystemDoSelectedActionHandler,
    IBattleSystemRunHandler, IBattleSystemSpawnNextHandler, IBattleSystemSpawnSpiritsHandler,
    IBattleSystemSpawnWildSpiritsHandler, IBattleSystemSpiritCatchedHandler,
    IBattleSystemSpiritWastedHandler, IBattleSystemWeatherEffectsHandler {
    private StateMachine stateMachine;
    public BattleObject BattleObject;

    private void Start() {
        stateMachine = GetComponent<StateMachine>();
    }

    private void Update() {
        if(BattleObject == null) return;
    }

    /* this line is so important, that i have to wrap it with comments */

    private void LateUpdate() {
        stateMachine.SetProperty(props.StateFinished, false);
    }

    private IEnumerator FinishState() {
        yield return new WaitForEndOfFrame();
        stateMachine.SetProperty(props.StateFinished, true);
    }

    /* --------------------------------------------------------------- */


    private void SpawnSpirit(int team) {
        //Promise p = new Promise();

        var image = team == 0 ? BattleObject.GetSpirit(team).Type.ImageBack : BattleObject.GetSpirit(team).Type.Image;
        BattleObject.GetSpirit(team).CurrentStamina = 0f;

        var hp = team == 0 ? BattleObject.GetSpirit(0).CurrentHP : BattleObject.GetSpirit(1).CurrentHP;
        if(team == 0)
            stateMachine.SetProperty(props.PlayerHP, hp);
        else if(team == 1)
            stateMachine.SetProperty(props.EnemyHP, hp);


        //BattleObject.GetSpirit(team)
        BUI.Instance.Spirits[team].DOFade(0, 0.7f).From()
            .OnStart(() => {
                BUI.Instance.Spirits[team].sprite = image;
                BUI.Instance.Spirits[team].enabled = true;
                BUI.Instance.SpiritUI[team].SpiritStaminaImage.gameObject.SetActive(true);
            })
            .OnComplete(
                () => {
                    stateMachine.SetProperty(props.SpiritsToSpawn,
                        stateMachine.GetProperty<int>(props.SpiritsToSpawn) - 1);
                });
    }


    public void OnEnterBattleStart() {
        if(BattleObject == null) return;
        stateMachine.SetProperty(props.TrainerFight, BattleObject.trainerFight);


        BattleObject.SetSpirit(0, BattleObject.GetTeam(0).Spirits[0]);
        BattleObject.SetSpirit(1, BattleObject.GetTeam(1).Spirits[0]);

        stateMachine.SetProperty(props.PlayerSpirits, BattleObject.GetTeam(0).Spirits.Count);
        stateMachine.SetProperty(props.EnemySpirits, BattleObject.GetTeam(1).Spirits.Count);
        stateMachine.SetProperty(props.StateFinished, true);
    }

    public void OnEnterBattleStartMessage() {
        var message = BattleObject.trainerFight
            ? "Insert trainer spuch hier"
            : $"Wildes {BattleObject.enemyTeam.Spirits[0].Name} greift an";

        StartCoroutine(BUI.Instance.WriteMessage(message, () => stateMachine.SetProperty(props.StateFinished, true)));
    }

    public void OnEnterCatchSpirit() {
        throw new System.NotImplementedException();
    }

    public void OnGenerateEnergy() {
        float[] speed = new[] {
            BattleObject.GetBattleSpeed(BattleObject.GetSpirit(0)),
            BattleObject.GetBattleSpeed(BattleObject.GetSpirit(1))
        };

        var maxSpeed = Math.Max(speed[0], speed[1]);
        if(BattleObject.weatherEffect != null) maxSpeed = Mathf.Max(maxSpeed, BattleObject.weatherEffect.Speed);
        var time_multiplicator = 100/maxSpeed;

        BattleObject.activePlayer = null;
        stateMachine.SetProperty(props.SpiritReady, -1);
        for(int i = 0; i <= 1; ++i) {
            BattleObject.GetSpirit(i).CurrentStamina += speed[i]*Time.deltaTime*time_multiplicator;
            if(BattleObject.GetSpirit(i).CurrentStamina >= 100) {
                stateMachine.SetProperty(props.SpiritReady, i);
            }
        }

        for(int i = 0; i <= 1; ++i) {
            if(i == stateMachine.GetProperty<int>(props.SpiritReady)) {
                BattleObject.activePlayer = i;
                break;
            }

//            var v = BUI.StaminaImages[i].localPosition;
//            v.x = BattleObject.GetSpirit(i).CurrentStamina/100*BUI.StaminaBar.rect.width - BUI.StaminaBar.rect.width/2;
//            BUI.StaminaImages[i].localPosition = v;
        }
        if(BattleObject.weatherEffect != null) {
            BattleObject.weatherEffect.Stamina += BattleObject.weatherEffect.Speed*Time.deltaTime*time_multiplicator;
            stateMachine.SetProperty(props.WeatherEffectReady, BattleObject.weatherEffect.Stamina >= 100);

            if(BattleObject.weatherEffect.Duration > 0) {
                BattleObject.weatherEffect.Duration -= BattleObject.weatherEffect.Speed*Time.deltaTime*
                                                       time_multiplicator;
            } else {
                BattleObject.weatherEffect = null;
            }
        }

        stateMachine.SetProperty(props.StateFinished, true);
    }

    public void OnEnterWeatherEffects() {
        BattleObject.weatherEffect.Proc(BattleObject, () => stateMachine.SetProperty(props.StateFinished, true));
    }

    public void OnEnterPlayerAction() {
        if(BattleObject.activePlayer == null) throw new ArgumentException("this should not happen");
        StartCoroutine(BattleObject.ActionHandlers[(int) BattleObject.activePlayer].OnTurn(
            () => stateMachine.SetProperty(props.StateFinished, true)));
    }

    public void OnEnterDoSelectedAction() {
        //// -----
        var p = (int) BattleObject.activePlayer;
        var b = BattleObject.action[p];
        if(b.action == PlayerAction.ActionType.Attack) {
            var attack = BattleObject.GetSpirit(p).Attacks[(int) b.actionValue];
            var attacktype = attack.AttackName;
            var method = typeof(AttackAnimation).GetMethod(attacktype.ToString("F"),
                BindingFlags.Static | BindingFlags.Public);

            float acc;
            var precision = attack.CalculateAccuracy(BattleObject.GetSpirit(p), out acc);

            Action a = () => {
                BattleObject.GetSpirit(p).CurrentStamina -= attack.StaminaCost;

                bool crit;
                float effective;
                var dmg = attack.CalculateDamage(BattleObject.GetSpirit(p), BattleObject.GetSpirit(1 - p), acc, out crit,
                    out effective);

                BattleObject.GetSpirit(1 - p).CurrentHP -= Mathf.Floor(dmg);
                string msg_part1 = "";
                string msg_part2 = "";
                if(crit) msg_part1 = "wurde kritisch getroffen und ";
                if(effective >= 1.5) msg_part2 = "Der Angriff war sehr effektiv";
                else if(effective > 1) msg_part2 = "Der Angriff war effektiv";
                else if(effective < 1) msg_part2 = "Der Angriff war nicht sehr effektiv";
                else if(effective < 0.5) msg_part2 = "Der Angriff zeigt kaum Wirkung";

                string message2 = "";
                if(effective <= 0) message2 = "Der Angriff hatte keine Wirkung";
                else
                    message2 =
                        $"{BattleObject.GetSpirit(1 - p).Name} {msg_part1} hat {Mathf.Floor(dmg)} Schaden erlitten. {msg_part2}";

                StartCoroutine(BUI.Instance.WriteMessage(message2,
                    () => stateMachine.SetProperty(props.StateFinished, true)));
            };
            // man nehme an method sei immer != null :D
            var message = $"{BattleObject.GetSpirit(p).Name} greift mit {attack.Name} an.";

            StartCoroutine(BUI.Instance.WriteMessage(message,
                () => {
                    if(precision) {
                        StartCoroutine((IEnumerator) method.Invoke(null, new object[] {BUI.Instance, p, a}));
                    } else {
                        BattleObject.GetSpirit(p).CurrentStamina -= attack.StaminaCost*2/3;
                        StartCoroutine(BUI.Instance.WriteMessage("Die Attacke ging daneben",
                            () => stateMachine.SetProperty(props.StateFinished, true)));
                    }
                }));
        }
        //// ----
    }

    public void OnEnterEvaluateBattle() {
        if(BattleObject.activePlayer == null) throw new ArgumentException("this should not happen");
        if(BattleObject.action[(int) BattleObject.activePlayer] == null)
            throw new ArgumentException("this should not happen");
        stateMachine.SetProperty(props.PlayerAction, (int) BattleObject.action[(int) BattleObject.activePlayer].action);

        stateMachine.SetProperty(props.CatchSpirit, false); // TODO: is true if item selected is xyz
        stateMachine.SetProperty(props.MinSpiritHP,
            Mathf.Min(BattleObject.GetSpirit(0).CurrentHP, BattleObject.GetSpirit(1).CurrentHP));

        stateMachine.SetProperty(props.PlayerHP, BattleObject.playerSpirit.CurrentHP);
        stateMachine.SetProperty(props.EnemyHP, BattleObject.enemySpirit.CurrentHP);
        BattleObject.activePlayer = null;
        StartCoroutine(FinishState());
    }

    public void OnEnterSpiritWasted() {
        stateMachine.SetProperty(props.PlayerSpirits,
            BattleObject.GetTeam(0).Spirits.Count(spirit => spirit.CurrentHP > 0));
        stateMachine.SetProperty(props.EnemySpirits, BattleObject.GetTeam(1).Spirits.Count(s => s.CurrentHP > 0));
        StartCoroutine(FinishState());
    }

    public void OnEnterSpawnNext() {
        int[] k = {0};
        for(int i = 1; i >= 0; --i) {
            if(BattleObject.GetSpirit(i).CurrentHP <= 0) {
                k[0]++;
                StartCoroutine(BattleObject.ActionHandlers[i].OnSpiritDown(() => {
                    k[0]--;
                    if(k[0] == 0) {
                        stateMachine.SetProperty(props.StateFinished, true);
                    }
                }));
            }
        }
//        ActionHandlers[i].OnSpiritDown();
    }

    public void OnEnterRun() {
        throw new System.NotImplementedException();
    }


    public void OnEnterSpawnSpirits() {
        if(BattleObject.trainerFight) {
            stateMachine.SetProperty(props.SpiritsToSpawn, 1);
            SpawnSpirit(1);
        }
        SpawnSpirit(0);
        stateMachine.SetProperty(props.SpiritsToSpawn, stateMachine.GetProperty<int>(props.SpiritsToSpawn) + 1);

        // Promise.Resolved().ThenAll(() => ps.ToArray()).Then(() => stateMachine.SetProperty(props.StateFinished, true));
    }

    public void OnEnterSpawnWildSpirits() {
        stateMachine.SetProperty(props.SpiritsToSpawn, 1);
        SpawnSpirit(1);
    }

    public void OnEnterSpiritCatched() {
        throw new System.NotImplementedException();
    }

    #region EndOfBattle

    public void OnEnterEnemyWin() {
        BattleObject.winner = 0;
        StartCoroutine(FinishState());
    }

    public void OnEnterPlayerWin() {
        BattleObject.winner = 1;
        StartCoroutine(FinishState());
    }

    public void OnEnterEnd() {
        StartCoroutine(FinishState());
    }

    #endregion
}

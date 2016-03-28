using System.Collections;
using Assets.Soraphis.Spirits.Scripts;
using DG.Tweening;
using UnityEngine;
using Gamelogic;
using RSG;
using UnityEngine.UI;

public class BattleController : Singleton<BattleController> {

    public Image[] Spirits = new Image[2];

    public RectTransform[] StaminaImages = new RectTransform[2];
    public Image[] HPBar = new Image[2];
    public Text[] HPText = new Text[2];

    public RectTransform StaminaBar;


    public GameObject ActionPanel;
    private Text ActionPanel_Message;
    public RectTransform MessageBox;

    private BattleObject battleObject;
    
    [SerializeField] private SpiritTeam t1;
    [SerializeField] private SpiritTeam t2;

    [HideInInspector] public PlayerAction action = new PlayerAction(0, PlayerAction.ActionType.None);
    [HideInInspector] public object actionValue = null;
    private StateMachine stateMachine;

    public void PlayerSelectAttack(int i) {
        action = new PlayerAction(i, PlayerAction.ActionType.Attack);
    }

    /////// JUST FOR TESTING!!!
    public void Start() {
        ActionPanel_Message = ActionPanel.transform.FindChild("MessageBox").GetComponentInChildren<Text>();
        stateMachine = GetComponent<StateMachine>();
        Spirits[0].enabled = false;
        Spirits[1].enabled = false;
        StaminaImages[0].gameObject.SetActive(false);
        StaminaImages[1].gameObject.SetActive(false);
        //Player.Instance.team = new Team();s
        //Player.Instance.team.TeamList.Add(SpiritType.GenerateSpirit("Furbold", 5));
    }

    public void Update() {
        /*if(Input.GetKey(KeyCode.X)) {
            Team t = new Team();
            t.TeamList.Add(SpiritType.GenerateSpirit("Furbold", 3));
            BattleObject b = new BattleObject(true, t);
            StartBattle(b);
        }*/
        if(battleObject == null && Input.GetKeyDown(KeyCode.T)) {
            StartCoroutine(StartBattle(new BattleObject((SpiritTeam) t2.Clone(), (SpiritTeam) t1.Clone())));
        }

        if(battleObject != null) {
            for(int i = 0; i < 2; ++i) {
                var sp = battleObject.GetSpirit(i);
                var hpp = Mathf.Ceil(sp.CurrentHP)/(sp.HP*SpiritType.PP_HP);
                HPBar[i].fillAmount = hpp;
            }
            
        }
        if (battleObject != null && stateMachine.currentState.name == "GenerateEnergy" 
            || stateMachine.currentState.name == "SpawnWildSpirits"
            || stateMachine.currentState.name == "EvaluateBattle"
            || stateMachine.currentState.name == "DoSelectedAction"
            || stateMachine.currentState.name == "SpawnSpirits") {
            for(int i = 0; i < 2; ++i) { 
                var v = StaminaImages[i].localPosition;
                v.x = battleObject.GetSpirit(i).CurrentStamina/100*StaminaBar.rect.width - StaminaBar.rect.width/2;
                var dif = Mathf.Abs(v.x - StaminaImages[i].localPosition.x);
                if(dif < 10)
                    StaminaImages[i].localPosition = v;
                else
                    StaminaImages[i].DOLocalMoveX(v.x, dif/1000, false);
            }
        }

    }

    private IEnumerator StartBattle(BattleObject bo) {
        yield return new WaitForEndOfFrame();
        battleObject = bo;
        GetComponent<BattleSystemImpl2>().BattleObject = bo;
        GetComponent<StateMachine>().GoToState("BattleStart");
        battleObject.ActionHandlers = new[] {new AIBattleActionHandler(bo, 0), new AIBattleActionHandler(bo, 1)};
    }


    private IEnumerator PrintMessage(string message, RectTransform box, Text text) {

        box.gameObject.SetActive(true);
        box.GetChildren().ForEach(c => c.gameObject.SetActive(false));
        MessageBox.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        text.text = message;

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
        MessageBox.gameObject.SetActive(false);
    }
    public IEnumerator WriteMessage(string message, System.Action callback = null) {
        yield return PrintMessage(message, ActionPanel.GetComponent<RectTransform>(), ActionPanel_Message);
        yield return new WaitForEndOfFrame();
        callback?.Invoke();
    }

        //    public IEnumerator BattleRoutine(BattleObject battle) {
        //        Player.Instance.CurrentActionState = Player.ActionState.Battle;
        //        Game.Instance.CurrentGameState = Game.GameState.Battle;
        //
        //        Spirit[] fs = {null, null}; // fighting spirits team 0(you)/1(enemy)
        //        var st_bar = StaminaBar.GetComponent<RectTransform>();
        //        bool[] hpChanged = {false, false};
        //        do {
        //            for(int i = 1; i >= 0; --i) {
        //                if(fs[i] == null) {
        //                    if (! battle.GetTeam(i).Spirits.Any(s => s.currentHP > 0f)) {
        //                        battle.winner = 1 - i; // nothing to summon?
        //                        break;
        //                    }
        //                    fs[i] = battle.GetTeam(i).Spirits.First(s => s.currentHP > 0f);
        //                    // change picture
        //                    fs[i].CurrentStamina = 0;
        //                    while(false) {
        //                        // wait for summon animation
        //                        yield return null;
        //                        
        //                    }
        //                }
        //            }
        //            if(battle.winner == -1) {
        //                var time_multiplicator = 100/Mathf.Max(fs[0].BTL_SPEED, fs[1].BTL_SPEED);
        //                foreach(var s in fs)
        //                    s.CurrentStamina += s.BTL_SPEED*Time.deltaTime*time_multiplicator; // iteration step
        //                for(int i = 0; i < 2; ++i) {
        //                    var v = StaminaImages[i].localPosition;
        //                    v.x = fs[i].CurrentStamina/100*st_bar.rect.width - st_bar.rect.width/2;
        //                    StaminaImages[i].localPosition = v;
        //
        //                    if(! hpChanged[i]) continue;
        //                    var t = HPBar[i].DOFillAmount(Mathf.Ceil(100*fs[i].currentHP/fs[i].BTL_HP)/100, 0.3f);
        //                    yield return t.WaitForCompletion();
        //                    HPText[i].text = $"{Mathf.Max(0, fs[i].currentHP)} / {fs[i].BTL_HP}";
        //                    hpChanged[i] = false;
        //                }
        //
        //                foreach(var s in fs.Where(sp => sp.CurrentStamina >= 100 && sp.currentHP > 0)) {
        //                    // one spirit has full stamina: let the ai make his move
        //                    if(s == fs[1]) {
        //                        AITurn(fs[1], fs[0]);
        //                        hpChanged[0] = true;
        //                        continue;
        //                    }
        //                    // player turn:
        //                    action = PlayerAction.None;
        //                    actionValue = null;
        //                    ActionPanel.SetActive(true);
        //                    ActionPanel.transform.Find("Attack Options").gameObject.SetActive(false);
        //                    ActionPanel.transform.Find("Basic Options").gameObject.SetActive(true);
        //                    // make UI visible - wait for button click
        //                    while(action == PlayerAction.None && actionValue == null) {
        //                        if(action != PlayerAction.None) Debug.Log(action);
        //                        if(actionValue != null) Debug.Log(actionValue);
        //                        yield return null;
        //                    }
        //                    ActionPanel.SetActive(false);
        //                    // do selected action
        //                    PlayerTurn(fs[0], fs[1]);
        //                    hpChanged[1] = true;
        //                }
        //                for(int i = 0; i < 2; ++i) {
        //                    if(fs[i].currentHP <= 0) {
        //                        // die animation
        //
        //
        //                        Spirits[i].DOFade(0f, 1.2f);
        //                        fs[i] = null;
        //                    }
        //                }
        //            }
        //            yield return null;
        //        } while(battle.winner == -1);
        //
        //        Player.Instance.CurrentActionState = Player.ActionState.Default;
        //        Game.Instance.CurrentGameState = Game.GameState.World;
        //    }

        //    private void PlayerTurn(Spirit you, Spirit enemy) {
        //        you.CurrentStamina -= 50;
        //        enemy.currentHP -= 5;
        //        Spirits[0].rectTransform.DOPunchAnchorPos((Vector2.right + Vector2.up)*4, 0.3f);
        //    }
        //
        //    private void AITurn(Spirit you, Spirit enemy) {
        //        you.CurrentStamina -= 25;
        //        enemy.currentHP -= 5;
        //    }

    }

//public class BattleObject {
//    private readonly SpiritTeam enemyTeam;
//    public bool TrainerFight; // -> enemy is catchable
//
//    private readonly SpiritTeam playerTeam;
//    public int winner = -1;
//
//    public BattleObject(bool trainer, SpiritTeam enemyTeam) {
//        this.enemyTeam = enemyTeam;
//        playerTeam = new SpiritTeam();
//        playerTeam.Spirits = new Spirit[Mathf.Min(6, Player.Instance.team.Count)];
//        for (int i = 0; i < playerTeam.Spirits.Length; ++i) {
//            playerTeam.Spirits[i] = (Player.Instance.team[i]);
//        }
//    }
//
//    public SpiritTeam GetTeam(int x) {
//        if(x == 1) return enemyTeam;
//        return playerTeam;
//
//    }
//
//
//}

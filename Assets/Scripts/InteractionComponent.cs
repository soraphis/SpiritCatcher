using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Assets;
using Assets.Soraphis.Spirits.Scripts;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InteractionComponent : MonoBehaviour {
    [SerializeField] private SpiritTeam team;
    private BattleObject bo;
    private BattleController bc;

    public Dialog Dialog;

    [SerializeField] private UnityEngine.Events.UnityEvent OnBattlePlayerWin = new UnityEvent();


    // ---
   /* [SerializeField] private bool recovering;
    [SerializeField] private float recovertime;*/

    public void Start() {
        team = team?.Clone() as SpiritTeam;
        // bo = new BattleObject(team);
    }

    public IEnumerator NewInteract() {
        yield return null;
       /* if(recovering)
            if(recovertime > 0) {
            } else {
                recovering = false;
            }*/
    }

    public IEnumerator Interact() {
        Dialog.SetProperty("HasTeam", Player.Instance.team.Count(s => s.CurrentHP > 0) > 0);
        Dialog.SetProperty("NPCFight", team.Spirits.Count(s => s.CurrentHP > 0) > 0);
        if(Dialog.GetProperty("NPCFight", false)) {
            Dialog.SetProperty("FightEnd", false);
        }

        Dialog.currentStatement = Dialog.Container.StartingPoint;
        yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));
        var n = Dialog.currentStatement.name;
        Debug.Log(n);
        if(n == "FightStart" && team != null) {

            var x = ScriptableObject.CreateInstance<SpiritTeam>();
            x.Spirits = Player.Instance.team;

            bo = new BattleObject(team, x, this.gameObject);
            yield return StartCoroutine(LoadBattle());
            Dialog.SetProperty("PlayerWinsFight", bo.winner == 0);
            Dialog.SetProperty("FightEnd", true);

            Debug.Log(bo.winner);
            if(bo.winner == 0)
                OnBattlePlayerWin.Invoke();
            if(bo.winner == 1) {
                team.Spirits.ForEach(s => s.CurrentHP = s.HP * SpiritType.PP_HP);
            }


            yield return StartCoroutine(DialogWindow.Instance.ShowDialog(Dialog));
            // Dialog.Container.StartingPoint = Dialog.currentStatement;

        }else if(n == "NPCWins") {
            StartCoroutine(Player.Instance.DefeatedInFight());
            Player.Instance.money = 0;
        }

        yield return null;
    }

    private IEnumerator LoadBattle() {
        Destroy(GameObject.Find("Canvas: Battle"));
        AsyncOperation async = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        yield return async;
        bc = GameObject.Find("Canvas: Battle").GetComponent<BattleController>();
        yield return StartCoroutine(bc.StartBattle(bo));
        SceneManager.UnloadScene(2);
        Destroy(GameObject.Find("Canvas: Battle"));
        yield return new WaitForEndOfFrame();
    }

    /*    
        public IEnumerator Interact() {
            yield return StartCoroutine(DialogWindow.Instance.ShowDialog("hey ho wie gehts"));
            if (team.Spirits.Any(s => s.CurrentHP > 0)) {
                yield return StartCoroutine(LoadBattle());
    //            if (bo.winner == 0)
    //                yield return StartCoroutine(DialogWindow.Instance.ShowDialog("yo krasser kampf"));
    //            else
    //                yield return StartCoroutine(DialogWindow.Instance.ShowDialog("Hab ich dich platt gemacht!"));
            } else
                yield return StartCoroutine(DialogWindow.Instance.ShowDialog("Meine Spirits müssen erst wieder regenerieren bevor wir nocheinmal kämpfen"));
            yield return null;
        }

        */
}

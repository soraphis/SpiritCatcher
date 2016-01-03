using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Soraphis.Spirits.Scripts;

public class InteractionComponent : MonoBehaviour {

    [SerializeField] private SpiritTeam team;
    private BattleObject bo;
    private BattleController bc;


    // ---
    [SerializeField] private bool recovering;
    [SerializeField] private float recovertime;

    public void Start() {
        bo = new BattleObject(true, team);
    }

    public IEnumerator NewInteract() {
        yield return null;
        if(recovering)
            if(recovertime > 0) {
                
            } else {
                recovering = false;
            }
    }

    public IEnumerator Interact() {
        yield return StartCoroutine(Dialog.Instance.ShowDialog("hey ho wie gehts"));
        if (team.Spirits.Any(s => s.currentHP > 0)) {
            yield return StartCoroutine(LoadBattle());
            if (bo.winner == 0)
                yield return StartCoroutine(Dialog.Instance.ShowDialog("yo krasser kampf"));
            else
                yield return StartCoroutine(Dialog.Instance.ShowDialog("Hab ich dich platt gemacht!"));
        } else
            yield return StartCoroutine(Dialog.Instance.ShowDialog("Meine Spirits müssen erst wieder regenerieren bevor wir nocheinmal kämpfen"));
        yield return null;
    }

    private IEnumerator LoadBattle() {
        Destroy(GameObject.Find("Canvas: Battle"));
        AsyncOperation async = Application.LoadLevelAdditiveAsync(1);
        yield return async;
        bc = GameObject.Find("Canvas: Battle").GetComponent<BattleController>();
        yield return StartCoroutine(bc.BattleRoutine(bo));
        Destroy(GameObject.Find("Canvas: Battle"));
    }
    
}

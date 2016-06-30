using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Soraphis.Spirits.Scripts;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Fog : MonoBehaviour {
    [System.Serializable]
    public struct RandomEncounterData {
        public string SpiritType;
        public int Level;
    }

    private new SpriteRenderer renderer;
    public RandomEncounterData[] Data;
    

    // Use this for initialization
    void Start () {
        renderer = GetComponent<SpriteRenderer>();
        Invoke("Fade", 0);
    }

    void Fade() {
        var seq = DOTween.Sequence();
        seq.Append(renderer.DOFade(0.5f, Random.value + 0.3f));
        seq.Append(renderer.DOFade(1f, Random.value + 0.3f));
        seq.OnComplete(() => Invoke("Fade", Random.value));
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (Data.Length < 1) return;
        StartCoroutine(StartFight());
    }

    IEnumerator StartFight() {
        var dt = Data[Random.Range(0, Data.Length)];

        var spirit = Spirit.GenerateSpirit(dt.SpiritType, dt.Level);
        var y = ScriptableObject.CreateInstance<SpiritTeam>();
        y.Spirits = new List<Spirit>() { spirit };

        var x = ScriptableObject.CreateInstance<SpiritTeam>();
        x.Spirits = Player.Instance.team;

        var bo = new BattleObject(y, x);
        yield return StartCoroutine(LoadBattle(bo));

        if (bo.winner == 1) {
            StartCoroutine(Player.Instance.DefeatedInFight());
        }

    }

    private IEnumerator LoadBattle(BattleObject bo) {
        Destroy(GameObject.Find("Canvas: Battle"));
        AsyncOperation async = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        yield return async;
        var bc = GameObject.Find("Canvas: Battle").GetComponent<BattleController>();
        yield return StartCoroutine(bc.StartBattle(bo));
        SceneManager.UnloadScene(2);
        Destroy(GameObject.Find("Canvas: Battle"));
        yield return new WaitForEndOfFrame();
    }

}

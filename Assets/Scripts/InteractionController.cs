using UnityEngine;
using System.Collections;
using Assets.Scripts.Movement;

public class InteractionController : MonoBehaviour {

    private MovementComponent mc = null;
    private Collider2D collider = null;
    public const float InteractionRange = 5;

	// Use this for initialization
	void Start () {
	    mc = GetComponent<MovementComponent>();
	    collider = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!Input.GetKeyDown(KeyCode.X)) return;
        if (Player.Instance.CurrentActionState == Player.ActionState.Cutszene) return;
        if (Game.Instance.CurrentGameState != Game.GameState.World) return;

        if (Player.Instance.CurrentActionState == Player.ActionState.Talking) return;

	    int oldlayer = gameObject.layer;
        gameObject.layer = GameLayer.IGNORERAYCAST;
        RaycastHit2D hit = Physics2D.Raycast(collider.bounds.center, mc.FacingDirection, InteractionRange);
        hit.collider?.SendMessage("Interact", null, SendMessageOptions.DontRequireReceiver);
        gameObject.layer = oldlayer;
	}

#if UNITY_EDITOR
    void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 100, 100));
        var t = Game.Instance.GameTime;
        GUILayout.Label($"{t.Day}.{t.Month}.{t.Year} | {t.Hour}:{t.Minute}");
        if (Input.GetKey(KeyCode.RightArrow)) GUILayout.Label("→");
        if (Input.GetKey(KeyCode.UpArrow)) GUILayout.Label("↑");
        if (Input.GetKey(KeyCode.LeftArrow)) GUILayout.Label("←");
        if (Input.GetKey(KeyCode.DownArrow)) GUILayout.Label("↓");
        if (Input.GetKey(KeyCode.X)) GUILayout.Label("X");
        GUILayout.EndArea();
    }
#endif
}

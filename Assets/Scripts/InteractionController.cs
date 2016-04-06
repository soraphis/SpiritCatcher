using UnityEngine;
using Assets.Scripts.Movement;

public class InteractionController : MonoBehaviour {

    private MovementComponent mc = null;
    private new Collider2D collider = null;
    public const float InteractionRange = 64;

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


	    var InteractionRange_mod = InteractionRange;
        var origin = collider.bounds.center;
	    if(mc.FacingDirection == Vector2.up) {
	        InteractionRange_mod *= 1.3f;
	        // origin.y += collider.bounds.extents.y;
	    }

        Debug.DrawRay(origin, mc.FacingDirection * InteractionRange_mod, Color.red, 0.2f);
        RaycastHit2D hit = Physics2D.Raycast(collider.bounds.center, mc.FacingDirection, InteractionRange);
        if(hit.collider != null) { 
            var trig = hit.collider.isTrigger;
            if(!trig)
                hit.collider.SendMessage("Interact", null, SendMessageOptions.DontRequireReceiver);
        }
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

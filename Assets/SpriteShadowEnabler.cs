using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SpriteShadowEnabler : MonoBehaviour {

    public ShadowCastingMode shadowCast = ShadowCastingMode.Off;
    public bool shadowReceive = false;

	// Use this for initialization
	void Awake () {
	    var rend = this.GetComponent<SpriteRenderer>();
	    rend.shadowCastingMode = shadowCast;
	    rend.receiveShadows = shadowReceive;
	}

}

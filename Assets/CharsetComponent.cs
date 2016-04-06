using UnityEngine;

public class CharsetComponent : MonoBehaviour {

    public Texture Texture;

	// Use this for initialization
	void Start () {
	    var renderer = GetComponent<Renderer>();
	    renderer.material.mainTexture = Texture;
	}

}

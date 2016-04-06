using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectSpecificEvents : MonoBehaviour {

    [SerializeField] private UnityEngine.Events.UnityEvent StartActions = new UnityEvent();
    // private UnityEngine.Events.UnityEvent UpdateActions;

    // Use this for initialization
    void Start() {
        StartActions.Invoke();
    }

    // Update is called once per frame
    void Update() {

    }
}

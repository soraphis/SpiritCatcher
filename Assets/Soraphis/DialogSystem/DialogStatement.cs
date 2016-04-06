using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogStatement : ScriptableObject {
    [SerializeField] public string speaker;
    [SerializeField] public string text;

    [HideInInspector] public Rect NodeRect;
    [HideInInspector] public int Id;

    [SerializeField] public List<DialogAnswer> Answers;

    public void OnEnable() {
        speaker = speaker ?? "";
        text = text ?? "";
        Answers = Answers ?? new List<DialogAnswer>();

        this.hideFlags = HideFlags.HideInHierarchy;
        foreach(var a in Answers) {
            a.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    public virtual void OnGUI() {
        
    }
}

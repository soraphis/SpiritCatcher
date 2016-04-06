using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogAnswer : ScriptableObject {
    [HideInInspector] public DialogStatement From;
    public DialogStatement To;

    [HideInInspector] public int Id;
    [HideInInspector] public Rect BoundingRect;

    public List<DialogAnswerCondition> conditions; 

    public string title;
    public DialogContainer parent;

    

    public void OnEnable() {
        title = title ?? "";
        conditions = conditions ?? new List<DialogAnswerCondition>();
        //hideFlags = HideFlags.HideAndDontSave;
    }

    public bool DoesPassConditions() {
        var cx = conditions.TrueForAll(c => {
            var prop = parent.Properties.Find(p => p.Name == c.propertyName);
            if(prop == null) {
                Debug.Log(c.propertyName + " could not be found in properties");
                return true;
            }
            return c.DoesPassCondition(prop);
        });

        return cx;
    }

}
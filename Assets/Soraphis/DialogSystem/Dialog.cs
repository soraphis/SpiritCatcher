using System;
using System.Linq;
using UnityEngine;


public class Dialog : MonoBehaviour {
    public int ActiveContainer = 0;
    public DialogContainer[] Containers;

    [HideInInspector] public DialogStatement currentStatement;

    public DialogContainer Container => Containers[ActiveContainer];

    private void OnEnable() {
        for (int i = 0; i < Containers.Length; ++i) {
            Containers[i] = (DialogContainer)Containers[i].Clone();
        }
    }

    private void Start() {
        // currentStatement = Containers[ActiveContainer].StartingPoint;
    }

    public void SetProperty<T>(string name, T value) {
        var prop = Container.Properties.Find(p => p.Name == name);
        if(prop != null) {
            if(value is int) prop.intValue = (int) (object) value;
            else if(value is float) prop.floatValue = (float) (object) value;
            else if(value is bool) prop.boolValue = (bool) (object) value;
        }
    }

    public T GetProperty<T>(string name, T @default = default(T)) {
        var prop = Container.Properties.Find(p => p.Name == name);
        if(prop != null) {
            if(@default is int) return (T) (object) prop.intValue;
            else if(@default is float) return (T) (object) prop.floatValue;
            else if(@default is bool) return (T) (object) prop.boolValue;
        }
        return @default;
    }

    public DialogAnswer[] Answers {
        get { return Container.Answers.FindAll(a => a.From == currentStatement).ToArray(); }
    }

    public DialogAnswer Answer {
        set {
            if(!Container.Answers.Contains(value)) throw new ArgumentException();
            currentStatement = value.To;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class DialogContainer : ScriptableObject, ICloneable {

    public string DialogName;

    public DialogStatement StartingPoint;
    [SerializeField] public List<DialogStatement> Statements;
    //    [SerializeField] public List<DialogAnswer> Answers;

    [HideInInspector] public List<DialogProperty> Properties;

    public List<DialogAnswer> Answers {
        get {
            return Statements.SelectMany(s => s.Answers).ToList();
        }
    }

    public void OnEnable() {
        DialogName = DialogName ?? "";
        Statements = Statements ?? new List<DialogStatement>();
        Properties = Properties ?? new List<DialogProperty>();

        //Answers = Answers ?? new List<DialogAnswer>();
    }

    public object Clone() {
        DialogContainer container = ScriptableObject.CreateInstance(typeof(DialogContainer)) as DialogContainer;
        container.Properties = new List<DialogProperty>();
        container.Statements = new List<DialogStatement>();

        foreach(var property in Properties) {
            var t = new DialogProperty();
            t.Name = property.Name;
            t.type = property.type;
            t.boolValue = property.boolValue;
            t.floatValue = property.floatValue;
            t.intValue = property.intValue;
            container.Properties.Add(t);
        }

        foreach(var statement in Statements) {
            var x = ScriptableObject.CreateInstance(typeof(DialogStatement)) as DialogStatement;
            container.Statements.Add(x);

            x.Id = statement.Id;
            x.speaker = statement.speaker;
            x.text = statement.text;
            x.name = statement.name;
        }

        container.StartingPoint = container.Statements.Find(s => s.Id == StartingPoint.Id);

        // after all statements are there, do transitions
        foreach (var statement in Statements) {
            var x = container.Statements.Find(s => s.Id == statement.Id);
            x.Answers = new List<DialogAnswer>();
            foreach (var answer in statement.Answers) {
                var y = ScriptableObject.CreateInstance(typeof(DialogAnswer)) as DialogAnswer; 
                y.From = x;
                y.To = container.Statements.Find(s => s.Id == answer.To.Id);
                y.title = answer.title;
                y.parent = container;
                y.conditions = new List<DialogAnswerCondition>();
                foreach(var condition in answer.conditions) {
                    var c = new DialogAnswerCondition();
                    c.BoolcompareType = condition.BoolcompareType;
                    c.NumcompareType = condition.NumcompareType;
                    c.boolValue = condition.boolValue;
                    c.floatValue = condition.floatValue;
                    c.intValue = condition.intValue;
                    c.type = condition.type;
                    c.propertyName = condition.propertyName;
                    y.conditions.Add(c);
                }
                x.Answers.Add(y);
            }
        }
        return container;
    }
}

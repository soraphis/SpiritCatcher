using System;
using UnityEngine;

[Serializable]
public class DialogAnswerCondition {

    public bool boolValue;
    public int intValue;
    public float floatValue;

    public DialogProperty.Type type;
    public BoolCondition BoolcompareType;
    public NumericCondition NumcompareType;

    public string propertyName;

    public bool DoesPassCondition(DialogProperty property) {
        switch(property.type) {
            case DialogProperty.Type.BOOL: return DoesPassCondition(property.boolValue);
            case DialogProperty.Type.FLOAT: return DoesPassCondition(property.floatValue);
            case DialogProperty.Type.INT: return DoesPassCondition(property.intValue);
            default: throw new ArgumentException();
        }
    }

    private bool DoesPassCondition(int property) {
        var value = intValue;
        switch (NumcompareType) {
            case NumericCondition.GreaterThan: return property > value;
            case NumericCondition.GreaterThanOrEquals: return property >= value;
            case NumericCondition.LesserThan: return property < value;
            case NumericCondition.LesserThanOrEquals: return property <= value;
            case NumericCondition.Equal: return property == value;
            case NumericCondition.NotEqual: return property != value;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private bool DoesPassCondition(float property) {
        var value = floatValue;
        switch (NumcompareType) {
            case NumericCondition.GreaterThan: return property > value;
            case NumericCondition.GreaterThanOrEquals: return property >= value;
            case NumericCondition.LesserThan: return property < value;
            case NumericCondition.LesserThanOrEquals: return property <= value;
            case NumericCondition.Equal: return Mathf.Approximately(property, value);
            case NumericCondition.NotEqual: return !Mathf.Approximately(property, value);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private bool DoesPassCondition(bool property) {
        var value = boolValue;
        switch (BoolcompareType) {
            case BoolCondition.Equal: return value == property;
            case BoolCondition.NotEqual: return value != property;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}


[Serializable]
public class DialogProperty {

    public string Name;
    public Type type;

    public bool boolValue;
    public int intValue;
    public float floatValue;

    public enum Type {
        INT, FLOAT, BOOL
    }

}


public enum BoolCondition {
    Equal,
    NotEqual
}

public enum NumericCondition {
    GreaterThan,
    GreaterThanOrEquals,
    LesserThan,
    LesserThanOrEquals,
    Equal,
    NotEqual
}
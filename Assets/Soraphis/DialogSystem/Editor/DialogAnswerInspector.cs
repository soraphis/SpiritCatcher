using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DialogAnswer))]
public class DialogAnswerInspector : UnityEditor.Editor {

    private static AnswerConditionRenderer renderer;

    public void OnEnable() {
        DialogAnswer answer = (DialogAnswer)target;
    }

    public override void OnInspectorGUI() {
        DialogAnswer answer = (DialogAnswer)target;
        Draw(answer);
    }

    public static void Draw(DialogAnswer answer) {
        // EditorGUILayout.LabelField("DialogWindow Answer", answer.title);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        answer.title = EditorGUILayout.TextField("Answer", answer.title);

        if (answer.parent == null) return;

        if(renderer == null || renderer.answer != answer) renderer = new AnswerConditionRenderer(answer);
        renderer.DrawList();

        List<string> l = new List<string> {"-"};
        l.AddRange(answer.parent.Statements.Where(x => x != answer.From).Select(x => x.name));

        int t = EditorGUILayout.Popup("To:", Mathf.Max(0, l.IndexOf(answer.To?.name)), l.ToArray());
        if (t > 0) answer.To = answer.parent.Statements.Where(x => x != answer.From).ToArray()[t - 1];
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

}

public class AnswerConditionRenderer {
    public ReorderableList list;
    public DialogContainer container;
    public DialogAnswer answer;

    public AnswerConditionRenderer(DialogAnswer answer) {
        this.answer = answer;
        container = answer.parent;

        list = new ReorderableList(answer.conditions, typeof(DialogAnswerCondition), false, true, true, true);
        list.drawElementCallback += OnDrawElement;
        list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Conditions");
    }

    private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused) {
        DialogAnswerCondition condition = answer.conditions[index];

        float spacing = rect.width / 2;
        int propIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, spacing, EditorGUIUtility.singleLineHeight),
            GetIndexFrom(AvailablePropertyNames(), condition.propertyName), AvailablePropertyNames());

        condition.propertyName = GetSelectionByIndex(AvailablePropertyNames(), propIndex);
        DialogProperty prop = null;
        try {
            prop = container.Properties.Find(p => p.Name == condition.propertyName);
        }catch(ArgumentNullException) {}

        var r = new Rect(rect.x + (spacing*1.5f), rect.y, spacing/2, EditorGUIUtility.singleLineHeight);
        if (prop != null) {
            switch(prop.type) {
                case DialogProperty.Type.BOOL:
                condition.BoolcompareType = (BoolCondition)EditorGUI.EnumPopup(new Rect(rect.x + spacing, rect.y, spacing / 2, EditorGUIUtility.singleLineHeight), condition.BoolcompareType);
                condition.boolValue = EditorGUI.Toggle(new Rect(rect.xMax - 15, rect.y, 15, EditorGUIUtility.singleLineHeight), condition.boolValue);
                    break;
                case DialogProperty.Type.FLOAT:
                condition.NumcompareType = (NumericCondition)EditorGUI.EnumPopup(new Rect(rect.x + spacing, rect.y, spacing / 2, EditorGUIUtility.singleLineHeight), condition.NumcompareType);
                condition.floatValue = EditorGUI.FloatField(r, condition.floatValue);
                    break;
                case DialogProperty.Type.INT:
                condition.NumcompareType = (NumericCondition)EditorGUI.EnumPopup(new Rect(rect.x + spacing, rect.y, spacing / 2, EditorGUIUtility.singleLineHeight), condition.NumcompareType);
                condition.intValue = EditorGUI.IntField(r, condition.intValue);
                 break;
            }
        }
    }

    public void DrawList() {
        this.list.DoLayoutList();
    }

    private int GetIndexFrom(string[] array, string selected) {
        for (int i = 0; i < array.Length; i++) {
            if (array[i] == selected) {
                return i;
            }
        }

        return 0;
    }

    private string GetSelectionByIndex(string[] array, int selected) {
        if (array.Length > selected) {
            return array[selected];
        }

        return string.Empty;
    }

    private string[] AvailablePropertyNames() {
        var results = from property in container.Properties
                      select property.Name;

        return results.ToArray();
    }
}
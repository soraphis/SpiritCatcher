using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DialogContainer))]
public class DialogInspector : UnityEditor.Editor {
    public ReorderableList list;
    public DialogContainer container; 

    public void OnEnable() {
        container = (DialogContainer)target;
        list = new ReorderableList(container.Properties, typeof(DialogProperty), false, true, true, true);
        list.drawElementCallback += OnDrawElement;
        list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Properties");
    }

    private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
        DialogProperty property = container.Properties[index];
        float x = rect.x;

        property.type = (DialogProperty.Type)EditorGUI.EnumPopup(new Rect(x, rect.y, 60, EditorGUIUtility.singleLineHeight), property.type); x += 65;
        property.Name = EditorGUI.TextField(new Rect(x, rect.y, 100, EditorGUIUtility.singleLineHeight), property.Name); x += 105;

        float propertyWidth = rect.xMax - x;

             if (property.type == DialogProperty.Type.BOOL)  property.boolValue  = EditorGUI.Toggle(new Rect(x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), property.boolValue);
        else if (property.type == DialogProperty.Type.INT)   property.intValue   = EditorGUI.IntField(new Rect(x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), property.intValue);
        else if (property.type == DialogProperty.Type.FLOAT) property.floatValue = EditorGUI.FloatField(new Rect(x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), property.floatValue);
        else {
            EditorGUILayout.TextField("cant parse value type");
        }
    }

    public override void OnInspectorGUI() {
        container = (DialogContainer) target;
        container.DialogName = EditorGUILayout.TextField("DialogName:", container.DialogName);

        list.DoLayoutList();

       container.Statements.ForEach((s) => s?.OnGUI());
        
    }
}

[CustomEditor(typeof(DialogStatement))]
public class DialogStatementInspector : UnityEditor.Editor {

    public override void OnInspectorGUI() {
        DialogStatement statement = (DialogStatement)target;
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        statement.name = EditorGUILayout.TextField("DialogWindow Statement", statement.name);
        if(EditorGUI.EndChangeCheck()) { Undo.RecordObject(target, "Changed statement Name"); }

        EditorGUILayout.LabelField(statement.Id.ToString(), GUILayout.MaxWidth(30));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField(name);

        EditorGUI.BeginChangeCheck();
        statement.speaker = EditorGUILayout.TextField("Speaker Name", statement.speaker);
        if (EditorGUI.EndChangeCheck()) { Undo.RecordObject(target, "Changed speaker Name"); }

        EditorStyles.textArea.wordWrap = true;
        EditorStyles.textField.wordWrap = true;

        EditorGUI.BeginChangeCheck();
        statement.text = EditorGUILayout.TextArea(statement.text, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * 4));
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed spoken text");
        }

        statement.Answers.ForEach(DialogAnswerInspector.Draw);
        EditorGUILayout.Space();

    }
}
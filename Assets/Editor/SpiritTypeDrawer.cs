/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;
using Editor = UnityEditor.Editor;

[CustomPropertyDrawer(typeof(SpiritTypeAttribute))]
public class SpiritTypeDrawer : PropertyDrawer {

    public Dictionary<string, SpiritType> spirits;
    public Dictionary<string, ElementType> elements;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType != SerializedPropertyType.Generic) {
            EditorGUI.LabelField(position, label.text, "D'oh - wrong type: " + property.propertyType);
            return;
        }

        var name = property.FindPropertyRelative("name");
        var csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Pokemon - Pokemon.txt");
        SpiritTypes.LoadFromCSV(csv, out spirits, out elements);

        Rect p2 = new Rect(position);
        p2.position = new Vector2(position.position.x + position.size.x - 100, p2.position.y);
        p2.size = new Vector2(100, p2.size.y);
        position.size = new Vector2(position.size.x-100, position.size.y);

        //name.stringValue = EditorGUI.TextField(position, name.stringValue);
        //*
        var alltypes = spirits.Values.Select(v => v.name).ToList();
        var i = EditorGUI.Popup(position, alltypes.IndexOf(name.stringValue), alltypes.ToArray());
        i = Mathf.Max(i, 0);
        name.stringValue = alltypes.ToArray()[i];

        GUI.Button(p2, "Generate");
        
        //#1#
    }

}

[CustomEditor(typeof(Team))]
public class TeamEditor : UnityEditor.Editor{

    public Dictionary<string, SpiritType> spirits;
    public Dictionary<string, ElementType> elements;

    private int selection = 0;
    private int selectedLevel = 0;

    public override void OnInspectorGUI() {
        Team team = (Team) target;
        team.TeamList = team.TeamList ?? new Team.MyTeamList();

        List<string> alltypes;
        try {
            alltypes = SpiritTypes.Instance.spirits.Values.Select(v => v.name).ToList();
        }
        catch(Exception e) {
            var csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Pokemon - Pokemon.txt");
            SpiritTypes.LoadFromCSV(csv, out spirits, out elements);
            alltypes = spirits.Values.Select(v => v.name).ToList();
        }

        // var property = serializedObject.FindProperty("TeamList");

        EditorGUI.BeginChangeCheck();

        for(int i = 0; i < team.TeamList.Count; ++i) {
            var x = team.TeamList[i];
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-")) {
                team.TeamList.RemoveAt(i);
                EditorUtility.SetDirty(target);
                Repaint();
            }
            TypeList(ref x.type.name, alltypes);
            EditorGUILayout.LabelField("Level: " + x.level);
            EditorGUILayout.EndHorizontal();
            x.name = EditorGUILayout.TextField("(Nick)Name: ", x.name);

            EditorGUI.indentLevel++;
            x.currentHP = EditorGUILayout.FloatField("Current HP", x.currentHP);
            if(x.Attributes.Count < 1)
                Debug.Break();
            foreach (var attribute in x.Attributes) {
                EditorGUILayout.FloatField(attribute.Key, attribute.Value);
                
            }
            EditorGUI.indentLevel--;
        }


        //EditorGUILayout.PropertyField(iterator, true);

            EditorGUILayout.BeginHorizontal();
        
        selection = EditorGUILayout.Popup(selection, alltypes.ToArray());
        selectedLevel = Mathf.Clamp(EditorGUILayout.IntField(selectedLevel), 1, 100);
        if(GUILayout.Button("Generate")) {
            if(team.TeamList == null) team.TeamList = new Team.MyTeamList();
            team.TeamList.Add(SpiritType.GenerateSpirit(spirits[alltypes[selection]], selectedLevel));

            team.TeamList.RemoveAll(s => s == null | s.type == null | s.type.name == "" | s.level == 0);
            EditorUtility.SetDirty(target);
            Repaint();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndChangeCheck();
    }

    private void TypeList(ref int selected, List<string> alltypes ) {
        selected = EditorGUILayout.Popup(selected, alltypes.ToArray());
    }

    private void TypeList(ref string selected, List<string> alltypes) {
        var i = EditorGUILayout.Popup(alltypes.IndexOf(selected), alltypes.ToArray());
        selected = alltypes[Mathf.Clamp(i, 0, alltypes.Count-1)];
    }
}
*/

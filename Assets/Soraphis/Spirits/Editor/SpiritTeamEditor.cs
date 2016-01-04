using System;
using System.IO;
using System.Linq;
using Assets.Soraphis.Spirits.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Editor {

    [CustomEditor(typeof(SpiritTeam))]
    public class SpiritTeamEditor : UnityEditor.Editor {

        private static SpiritLibrary library = null;
        private string path = "Assets/Resources/New Spirit Library.asset";
        private int selection;
        private int selectedLevel;
        private static string[] list;
        private static bool[] foldouts;

        public override void OnInspectorGUI() {
            path = GUILayout.TextField(path);
            Exception ex = new FileNotFoundException();
            try {
                library = library ?? AssetDatabase.LoadAssetAtPath<SpiritLibrary>(path);
            }
            catch(Exception e) {ex = e;}
            list = list ?? library.Spirits.Select(s => s.Name).ToArray();
            SpiritTeam team = (SpiritTeam)target;
            var t = team.Spirits.ToList();

            if (foldouts == null || foldouts.Length != team.Spirits.Length) {
                if(foldouts == null) foldouts = new bool[team.Spirits.Length];
                else Array.Resize(ref foldouts, team.Spirits.Length);
            }

            EditorGUILayout.BeginHorizontal();
            selection = EditorGUILayout.Popup(selection, list);
            selectedLevel = Mathf.Clamp(EditorGUILayout.IntField(selectedLevel), 1, 100);
            if(GUILayout.Button("Generate")) {
                if(library == null) {
                    throw ex;
                }
                t.Add(Spirit.GenerateSpirit(library.Spirits[selection], selectedLevel));
                team.Spirits = t.ToArray();
                return;
            }
            EditorGUILayout.EndHorizontal();

            for(int i = 0; i < team.Spirits.Length; ++i) {
                var x = team.Spirits[i];
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], x.Name);
                if(! foldouts[i]) continue;
                if (GUILayout.Button("Delete")) {
                    t.RemoveAt(i);
                    team.Spirits = t.ToArray();
                }
                x.Name = EditorGUILayout.TextField("(Nick)Name: ", x.Name);
                x.currentHP = EditorGUILayout.FloatField("Current HP", x.currentHP);
                foreach (var attribute in x.Attribute) {
                    EditorGUILayout.FloatField(attribute.Key, attribute.Value);
                }
            }


            //base.OnInspectorGUI();
        }
    }
}
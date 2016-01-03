using System.Collections.Generic;
using System.Linq;
using Assets.Soraphis.Spirits.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Editor {
    [CustomEditor(typeof(SpiritLibrary))]
    public class SpiritLibraryEditor : UnityEditor.Editor {

        private string file = "Assets/SpiritLib.txt";
        // private string Attackfile = "Assets/Attacks.txt";
        private static bool[] foldouts;


        public override void OnInspectorGUI() {
            SpiritLibrary tgt = (SpiritLibrary)target;

            GUILayout.BeginHorizontal();
            file = GUILayout.TextField(file);

            if(GUILayout.Button("Load")) {
                tgt.LoadFromCSV(AssetDatabase.LoadAssetAtPath <TextAsset>(file));
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Spirits");
            if (tgt.Spirits == null || tgt.Spirits.Length == 0) return;
            GUILayout.Label("Size: " + tgt.Spirits.Length);
            GUILayout.EndHorizontal();

            if(foldouts == null || foldouts.Length != tgt.Spirits.Length) {
                foldouts = new bool[tgt.Spirits.Length];
            }
            EditorGUI.BeginChangeCheck();
            for (int index = 0; index < tgt.Spirits.Length; index++) {
                var spirit = tgt.Spirits[index];
                var foldout = foldouts[index];
                foldouts[index] = EditorGUILayout.Foldout(foldout, spirit.Name);
                if(foldout) {
                    int f2 = Screen.width/2;
                    int f3 = Screen.width/3;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Attribute", GUILayout.MinWidth(f2), GUILayout.MaxWidth(f2));
                    GUILayout.Label("Value", GUILayout.MinWidth(f3));
                    GUILayout.EndHorizontal();



                    foreach (var entry in spirit.Attributes.ToDictionary(entry => entry.Key, entry => entry.Value)) {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label((string) entry.Key, GUILayout.MinWidth(f2), GUILayout.MaxWidth(f2));
                        float x = EditorGUILayout.FloatField((float) entry.Value, GUILayout.MinWidth(f3));
                        if(! Mathf.Approximately(x, (float) tgt.Spirits[index].Attributes[entry.Key])) {
                            tgt.Spirits[index].Attributes[entry.Key] = x;
                            EditorUtility.SetDirty(target);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUI.EndChangeCheck();

        }

    

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Soraphis.Spirits.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Editor {
    [CustomEditor(typeof(SpiritLibrary))]
    public class SpiritLibraryEditor : UnityEditor.Editor {

        // private string file = "Assets/SpiritLib.txt";
        private TextAsset txtAsset;
        // private string Attackfile = "Assets/Attacks.txt";
        private static bool[] foldouts;

        public override void OnInspectorGUI() {
            SpiritLibrary tgt = (SpiritLibrary)target;

            GUILayout.BeginHorizontal();

            txtAsset = EditorGUILayout.ObjectField("Source File", txtAsset, typeof(TextAsset), false) as TextAsset;
            // file = GUILayout.TextField(file);

            if(GUILayout.Button("Load")) {
                // AssetDatabase.LoadAssetAtPath <TextAsset>(file)
                tgt.LoadFromCSV(txtAsset);
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

            SerializedObject lib = new UnityEditor.SerializedObject(tgt);
            var spirits = lib.FindProperty("Spirits");

            //            EditorGUILayout.PropertyField(spirits, true);
            //            lib.ApplyModifiedProperties();
            //            EditorGUI.EndChangeCheck();
            //            return;

            for (int index = 0; index < spirits.arraySize; ++index) {
                var spiritObject = spirits.GetArrayElementAtIndex(index).objectReferenceValue as SpiritType;
                var spirit = new SerializedObject(spiritObject);

                spirit.Update();

                
                var foldout = foldouts[index];
                foldouts[index] = EditorGUILayout.Foldout(foldout, spirit.FindProperty("Name").stringValue);

                var prop = spirit.GetIterator().Copy();

                if (foldout) {
                    int f2 = Screen.width/2;
                    int f3 = Screen.width/3;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Attribute", GUILayout.MinWidth(f2), GUILayout.MaxWidth(f2));
                    GUILayout.Label("Value", GUILayout.MinWidth(f3));
                    GUILayout.EndHorizontal();

                    prop.Next(true);
                    var depth = prop.depth;
                    do {
                        if (prop.displayName == "Attacks") continue;
                        RenderAttribute(prop, f2);
                    } while(prop.NextVisible(false) && prop.depth >= depth);

                    var prop_attacks = spirit.FindProperty("Attacks");
                    EditorGUILayout.PropertyField(prop_attacks);

                }

                spirit.ApplyModifiedProperties();
            }

            lib.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
    
        }

        private void RenderAttribute(SerializedProperty value, int f2) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(value, GUILayout.MinWidth(f2));
            GUILayout.EndHorizontal();
        }

    }
}

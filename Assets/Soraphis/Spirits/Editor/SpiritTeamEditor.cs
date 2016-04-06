using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Soraphis.Lib;
using Assets.Soraphis.Spirits.Scripts;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Soraphis.Spirits.Editor {
    [CustomEditor(typeof(SpiritTeam))]
    public class SpiritTeamEditor : UnityEditor.Editor {

        private SpiritLibrary library;
        private AttackLibrary atkLibrary;
        private string[] libraryList;
        private int selectedListItem;
        private int selectedLevel;

        private ReorderableList spiritList;
        private ReorderableList attackList;
        private SerializedProperty selectedSpirit;
        private SerializedProperty selectedAttack;

        private void OnEnable() {

            spiritList = new ReorderableList(serializedObject, serializedObject.FindProperty("Spirits"), true, true,
                true, true);
            spiritList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Spirits");
            spiritList.drawElementCallback = (rect, index, active, focused) => {
                var el = spiritList.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.ObjectField(rect.SplitRectH(2, 0), el.FindPropertyRelative("Type"), typeof(SpiritType), GUIContent.none);
                EditorGUI.PropertyField(rect.SplitRectH(4, 3), el.FindPropertyRelative("Level"), GUIContent.none);
            };
            spiritList.onSelectCallback = SelectionChanged;
            spiritList.onAddCallback = list => {
                if(list.count < 6)
                    ReorderableList.defaultBehaviours.DoAddButton(spiritList);
            };
            spiritList.onRemoveCallback = list => {
                selectedSpirit = null;
                selectedAttack = null;
                ReorderableList.defaultBehaviours.DoRemoveButton(spiritList);
            };
        }

        private void SelectionChanged(ReorderableList list) {
            if(list.index > list.count) {
                attackList = null;
                selectedSpirit = null;
                selectedAttack = null;
                return;
            }
            selectedSpirit = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if(selectedSpirit == null) {
                attackList = null;
                selectedAttack = null;
                return;
            }

            var attacks = selectedSpirit.FindPropertyRelative("Attacks");
            attackList = new ReorderableList(attacks.serializedObject, attacks, true, true, true, true);
            attackList.drawHeaderCallback =
                rect => EditorGUI.LabelField(rect, $"{selectedSpirit.displayName}'s attacks");
            attackList.drawElementCallback = (rect, index, active, focused) => {
                var el = attackList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, el.FindPropertyRelative("AttackName"));
            };
            attackList.onSelectCallback = reorderableList => {
                selectedAttack = attackList.serializedProperty.GetArrayElementAtIndex(reorderableList.index);
            };
        }

        public override void OnInspectorGUI() {
            library = EditorGUILayout.ObjectField("Library", library, typeof(SpiritLibrary), false) as SpiritLibrary;
            atkLibrary = EditorGUILayout.ObjectField("AttackLibrary", atkLibrary, typeof(AttackLibrary), false) as AttackLibrary;

            serializedObject.Update();
            if(library != null && atkLibrary != null) RenderGeneratorStuff();
            spiritList.DoLayoutList();

            if(selectedSpirit != null) {
                RenderSpirit(selectedSpirit);

                attackList?.DoLayoutList();
                if(selectedAttack != null) {
                    RenderAttack(selectedAttack);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderGeneratorStuff() {
            SpiritTeam t = target as SpiritTeam;
            var sprop = serializedObject.FindProperty("Spirits");

            libraryList = library.Spirits.Select(s => s.Name).ToArray(); // list of available spirits

            EditorGUILayout.BeginHorizontal();
            selectedListItem = EditorGUILayout.Popup(selectedListItem, libraryList);
            selectedLevel = Mathf.Clamp(EditorGUILayout.IntField(selectedLevel), 1, 100);
            GUI.enabled = t.Spirits.Count < 6;
            if(GUILayout.Button("Generate")) {
                var generated = Spirit.GenerateSpirit(library.Spirits[selectedListItem], selectedLevel, atkLibrary);

                int newIndex = sprop.arraySize;
                sprop.InsertArrayElementAtIndex(newIndex);
                var newspirit = sprop.GetArrayElementAtIndex(newIndex);

                // newspirit.FindPropertyRelative("Name").stringValue = "test";
                fillPropertyFromObject(newspirit, generated);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void fillPropertyFromObject(SerializedProperty property, object o) {
            if(!o.GetType().IsSerializable) throw new ArgumentException("object is not Serializable");

            var it = property.Copy();
            it.NextVisible(true);
            var d = it.depth;
            do {
                var field = o.GetType().GetField(it.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(field == null) {
                    Debug.Log($"field: {it.name}({it.displayName}) not found");
                    continue;
                }
                switch(it.propertyType) {
                    case SerializedPropertyType.Generic:
                        if(it.isArray) {
                            var list = field.GetValue(o) as IList;
                            if(list == null) break;
                            var prop = it.FindPropertyRelative("Array");
                            prop.FindPropertyRelative("size").intValue = list.Count;
                            int index = 0;
                            foreach(var l in list) {
                                fillPropertyFromObject(prop.GetArrayElementAtIndex(index++), l);
                            }
                        }
                        break;
                    case SerializedPropertyType.Enum:
                        if(!(field.GetValue(o) is Enum)) it.enumValueIndex = 0;
                        it.enumValueIndex = (int) field.GetValue(o);
                    break;
                    case SerializedPropertyType.Integer:
                        it.intValue = (int) field.GetValue(o);
                        break;
                    case SerializedPropertyType.Boolean:
                        it.boolValue = (bool) field.GetValue(o);
                        break;
                    case SerializedPropertyType.Float:
                        it.floatValue = (float) field.GetValue(o);
                        break;
                    case SerializedPropertyType.String:
                        it.stringValue = (string) field.GetValue(o);
                        break;
                    case SerializedPropertyType.ObjectReference:
                        it.objectReferenceValue = (Object) field.GetValue(o);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            } while(it.NextVisible(false) && it.depth >= d);
        }

        private void RenderSpirit(SerializedProperty spirit) {
            EditorGUILayout.Space();
//            EditorGUILayout.PropertyField(spirit, true);

            var it = spirit.Copy();
            it.NextVisible(true);
            var d = it.depth;
            do {
                if(it.isArray && it.displayName == "Attacks") continue;
                EditorGUI.indentLevel = it.depth;
                EditorGUILayout.PropertyField(it, false);
            } while(it.NextVisible(false) && it.depth >= d);


            EditorGUILayout.Space();
        }

        private void RenderAttack(SerializedProperty spirit) {
            EditorGUILayout.Space();
            //            EditorGUILayout.PropertyField(spirit, true);

            var it = spirit.Copy();
            it.NextVisible(true);
            var d = it.depth;
            do {
                EditorGUI.indentLevel = it.depth;
                EditorGUILayout.PropertyField(it, false);
            } while (it.NextVisible(false) && it.depth >= d);


            EditorGUILayout.Space();
        }
    }
}
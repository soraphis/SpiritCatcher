using System;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {

#if UNITY_EDITOR
    using UnityEditor;
    using Assets.Soraphis.Lib;

    namespace Editor {
        [CustomPropertyDrawer(typeof(AttackLevelTuple), true)]
        public class AttackLevelTupleDrawer : PropertyDrawer {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                var prop_attack = property.FindPropertyRelative("Attack");
                var prop_level = property.FindPropertyRelative("Level");

                EditorGUI.PropertyField(position.SplitRectH(6, 0, 5), prop_attack, GUIContent.none);
                EditorGUI.PropertyField(position.SplitRectH(6, 5), prop_level, GUIContent.none);

            }
        }

        

        [CustomPropertyDrawer(typeof(LayoutAttribute), false)]
        public class LayoutDrawer : PropertyDrawer {
            private const float kHeadingSpace = 22.0f;

            static Styles m_Styles;

            private class Styles {
                public readonly GUIStyle header = "ShurikenModuleTitle";

                internal Styles() {
                    header.font = (new GUIStyle("Label")).font;
                    header.border = new RectOffset(15, 7, 4, 4);
                    header.fixedHeight = kHeadingSpace;
                    header.contentOffset = new Vector2(20f, -2f);
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
                if (!property.isExpanded)
                    return kHeadingSpace;

                var count = property.CountInProperty();
                return EditorGUIUtility.singleLineHeight * count + 15;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                if (m_Styles == null)
                    m_Styles = new Styles();

                position.height = EditorGUIUtility.singleLineHeight;
                property.isExpanded = Header(position, property.displayName, property.isExpanded);
                position.y += kHeadingSpace;

                if (!property.isExpanded)
                    return;

                var x = property.depth;

                foreach (SerializedProperty child in property) {
                    if(child.depth - x != 2 && child.isArray) continue;
                    EditorGUI.PropertyField(position, child, true);
                    position.y += EditorGUIUtility.singleLineHeight;
                }
            }

            private bool Header(Rect position, String title, bool display) {
                Rect rect = position;
                position.height = EditorGUIUtility.singleLineHeight;
                GUI.Box(rect, title, m_Styles.header);

                Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
                if (Event.current.type == EventType.Repaint)
                    EditorStyles.foldout.Draw(toggleRect, false, false, display, false);

                Event e = Event.current;
                if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                    display = !display;
                    e.Use();
                }
                return display;
            }
        }

    }
#endif

    [AttributeUsage(AttributeTargets.Field)]
    public class LayoutAttribute : PropertyAttribute { }

    [Serializable]
    public class AttackLevelTuple {
        public string Attack;
        public int Level;
    }

    [Serializable]
    public struct AttackLevelTupleList {
        public AttackLevelTuple[] List;
    }

    [Serializable]
    public class SpiritType : ScriptableObject {

        public const int PP_HP = 5; // per point
        public const int PP_DMG = 2; 
        public const float PP_DEF = 0.8f; 
        public const float PP_SPEED = 0.2f;
        public const float PP_CRIT = 0.07f;
        public const float PP_ACC = 0.3f;

        [SerializeField] public string Name;
        [SerializeField] public Sprite Image;
        [SerializeField] public Sprite ImageBack;
        [SerializeField] public Sprite ImageIcon;

        [SerializeField] public string Typ;

        [SerializeField] public int HP;
        [SerializeField] public int DMG;
        [SerializeField] public int DEF;
        [SerializeField] public int SPEED;
        [SerializeField] public int CRIT;
        [SerializeField] public int ACC;

        [SerializeField] public int PL_HP;
        [SerializeField] public int PL_DMG;
        [SerializeField] public int PL_DEF;
        [SerializeField] public int PL_SPEED;

        [SerializeField] public float Catchiness;
        [SerializeField] public bool isStarter;

        [SerializeField] public bool isLegendary;
        [SerializeField] [Layout] public AttackLevelTupleList Attacks;
        
    }
}

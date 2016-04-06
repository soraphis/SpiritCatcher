using UnityEngine;

#if UNITY_EDITOR
namespace Editor {
    using UnityEditor;

    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    public class PositionHandleEditor : UnityEditor.Editor {
        
        void OnSceneGUI() {
            var t = target as MonoBehaviour;
            if(t == null) return;

            foreach(var fieldInfo in t.GetType().GetFields()) {
                try {
                    var attribs = fieldInfo.GetCustomAttributes(typeof(PositionHandleAttribute), false);
                    if(attribs.Length > 0 && fieldInfo.FieldType == typeof(Vector3)) {
                        Vector3 v = (Vector3) fieldInfo.GetValue(t);
                        v = Handles.PositionHandle((Vector3)v, Quaternion.identity);
                        Handles.Label(v, fieldInfo.Name);
                        fieldInfo.SetValue(t, v);
                    }
                }catch(System.Exception) {
                    // ignored
                }
            }

        }
    }

}
#endif
public class PositionHandleAttribute : PropertyAttribute {}

public class TownNPCFestivalPosition : MonoBehaviour {

    private Vector3 OriginalPosition;
    // private Vector2 OriginalRotation;
    [PositionHandle] public Vector3 FestivalPosition;
    // vpublic Vector2 FestivalRotation = Vector2.down;


    // Use this for initialization
    void Start() {
        OriginalPosition = transform.position;
        // OriginalRotation = Vector2.down;
    }

    // Update is called once per frame
    private void Update() {
        if(Game.Instance.QuestPart1Variables.FestivalAktive) {
            transform.position = FestivalPosition;
        } else if(Game.Instance.QuestPart1Variables.FestivalEnded) {
            transform.position = OriginalPosition;

            Destroy(this); // not this.gameObject
        }
    }
}

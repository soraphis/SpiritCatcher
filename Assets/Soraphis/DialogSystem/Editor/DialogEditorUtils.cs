using UnityEngine;
using UnityEditor;

public class DialogEditorUtils : ScriptableObject {
    [MenuItem("Window/DialogWindow Editor")]
    static void Init() { DialogEditorWindow.GetWindow(typeof(DialogEditorWindow)); }

    private static GUISkin Skin = null;

    public static void RasterBackgroundTexture(out Texture2D tex) {
        tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        for (int x = 0; x < 16; ++x) {
            for (int y = 0; y < 16; ++y) {
                if (x == 15 || y == 15)
                    tex.SetPixel(x, y, new Color(0.4f, 0.4f, 0.4f));
                else
                    tex.SetPixel(x, y, new Color(0.2f, 0.2f, 0.2f));

                tex.Apply();
            }
        }
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.hideFlags = HideFlags.HideAndDontSave;
    }

    public static GUISkin GetGUISkin() {
        Skin = Skin ?? AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/DialogSystem/New GUISkin.guiskin");
        return Skin;
    }
}
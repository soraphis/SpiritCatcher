using UnityEngine;
using System.Collections;
using UnityEditor;

public class DialogEditorWindow : EditorWindow {
    private Texture2D tex; // = DialogEditorUtils.RasterBackgroundTexture();

    public DialogContainer ActiveContainer;
    private DialogGraphGUI graph = new DialogGraphGUI();
    private Rect content = new Rect(0, 0, Screen.width, Screen.height);

    private void OnEnable() {
        OnSelectionChange();
    }

    private void OnSelectionChange() {
        AssetDatabase.SaveAssets();
        var x = Selection.GetFiltered(typeof(DialogContainer), SelectionMode.Editable);
        if(x.Length == 0) {
            return;
        }
        ActiveContainer = (DialogContainer) x[0];
        graph.OnSelectionChange();
        Repaint();
    }

    private void Update() {
        if(graph.controllMode != DialogGraphGUI.ControllMode.SELECTION)
            Repaint();
    }

    private void OnGUI() {
        if(tex == null) { DialogEditorUtils.RasterBackgroundTexture(out tex); }
        content.width = Screen.width;
        content.height = Screen.height;
        GUI.DrawTextureWithTexCoords(content, tex, new Rect(0, 0, content.width/tex.width, content.height/tex.height));

        if(ActiveContainer == null /*|| graph == null*/) return;

        graph.Nodes = () => ActiveContainer.Statements;
        graph.Edges = () => ActiveContainer.Answers;
        graph.Styles = new GUIStyle[] {GUI.skin.FindStyle("flow node 0")};
        

        GUILayout.BeginArea(content);
        graph.BeginGraphGUI(this);
        graph.OnGraphGUI();
        graph.EndGraphGUI();
        GUILayout.EndArea();

        
    }
}

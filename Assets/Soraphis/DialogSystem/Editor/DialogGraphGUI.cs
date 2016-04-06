using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class DialogGraphGUI {
    private DialogEditorWindow host;
    private Rect GraphExtends = new Rect(-2000, -2000, 2000, 2000);
    private Vector2 ScrollPosition;

    public delegate List<DialogStatement> NodesFromHost();

    public delegate List<DialogAnswer> EdgesFromHost();

    public NodesFromHost Nodes;
    public EdgesFromHost Edges;

    private DialogStatement selectedNode = null;
    private DialogAnswer selectedEdge = null;
    private Vector2 lastMousePos = Vector2.zero;

    private int _idGen = 0;
    private int idGenerator {
        get {
            while( (Nodes() != null && Nodes().Any(n => n.Id >= _idGen)) 
                || (Edges() != null && Edges().Any(e => e.Id >= _idGen))) {
                _idGen++;
            }

            return _idGen++;
        }
    }

    public enum ControllMode {
        SELECTION,
        DRAWEDGE,
        SCROLL
    }

    public ControllMode controllMode { get; private set; } = ControllMode.SELECTION;
    public GUIStyle[] Styles;

    private DialogStatement startNode;

    public readonly Dictionary<string, GenericMenu.MenuFunction2> menuDict =
        new Dictionary<string, GenericMenu.MenuFunction2>();

    public readonly Dictionary<string, GenericMenu.MenuFunction2> nodeDict =
        new Dictionary<string, GenericMenu.MenuFunction2>();

    public readonly Dictionary<string, GenericMenu.MenuFunction2> edgeDict =
        new Dictionary<string, GenericMenu.MenuFunction2>();

    public DialogGraphGUI() {
        menuDict.Add("New Statement", delegate {
            DialogStatement statement = ScriptableObject.CreateInstance<DialogStatement>();
            statement.Id = idGenerator;
            statement.NodeRect = new Rect(lastMousePos, new Vector2(160, 50));
            statement.name = "New Statement " + statement.Id;
            Nodes().Add(statement);

            AssetDatabase.AddObjectToAsset(statement, host.ActiveContainer);
            statement.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(host.ActiveContainer));
        });

        nodeDict.Add("Add Transition", delegate(object data) {
            startNode = Nodes().First(n => n.Id == (int) data);
            controllMode = ControllMode.DRAWEDGE;
        });
        nodeDict.Add("Mark as Starting Node", delegate (object data) {
            host.ActiveContainer.StartingPoint = Nodes().First(n => n.Id == (int) data);
        });
        nodeDict.Add("Delete Node", delegate(object data) {
            Edges().RemoveAll(e => e.From.Id == (int) data || e.To.Id == (int) data);
            Nodes().RemoveAll(n => n.Id == (int) data);
        });

        edgeDict.Add("Delete Edge", delegate(object data) {
            var ed = Edges().First(e => e.Id == (int) data);
            ed.From.Answers.Remove(ed);
        });
    }

    public void BeginGraphGUI(DialogEditorWindow host) {
        //UnityEditor.Graphs.GraphGUI
        this.host = host;
        ScrollPosition = GUI.BeginScrollView(new Rect(0, 0, host.position.width, host.position.height),
            ScrollPosition, GraphExtends, GUIStyle.none, GUIStyle.none);
        //GUI.skin = DialogEditorUtils.GetGUISkin();
    }

    private void OnNodeEvent(DialogStatement node) {
        var rect = node.NodeRect;
        if (rect.Contains(Event.current.mousePosition)) { 
        if(Event.current.type == EventType.ContextClick) {
            EditorContextMenu(nodeDict, node.Id);
            Event.current.Use();
        }
        if (Event.current.type == EventType.MouseDown) {
            if(controllMode == ControllMode.DRAWEDGE && startNode != node) {
                DialogAnswer answer = ScriptableObject.CreateInstance<DialogAnswer>();
                answer.From = startNode;
                answer.To = node;
                answer.Id = idGenerator;
                startNode.Answers.Add(answer);
                AssetDatabase.AddObjectToAsset(answer, host.ActiveContainer);
                answer.hideFlags = HideFlags.HideInHierarchy;
                answer.parent = host.ActiveContainer;
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(host.ActiveContainer));
                controllMode = ControllMode.SELECTION;
            } else {
                selectedNode = node;
                Selection.activeObject = node;
            }
            Event.current.Use();
        }
        }
        if (selectedNode != node) return;
        if (Event.current.type == EventType.MouseDrag) {
            node.NodeRect.position += Event.current.delta;
            EditorUtility.SetDirty(node);
            Event.current.Use();
        }
    }

    private void OnNodeDraw(DialogStatement node) {
        int i = node == host.ActiveContainer.StartingPoint ? 5 : 0;
        string s = node == selectedNode ? " on" : "";
        
        GUIStyle style = $"flow node {i}{s}";

        var labelstyle = GUI.skin.label;
        labelstyle.alignment = TextAnchor.MiddleCenter;
        EditorGUIUtility.AddCursorRect(node.NodeRect, MouseCursor.MoveArrow);
        GUI.BeginGroup(node.NodeRect, style);
        GUI.Label(new Rect(0, 0, node.NodeRect.width, node.NodeRect.height), $"{node.name}", GUI.skin.label);
        GUI.EndGroup();
    }

    public void OnGraphGUI() {
        // Draw Nodes:
        //host.BeginWindows();
        foreach(var dialogStatement in Nodes()) {
            OnNodeEvent(dialogStatement);
            OnNodeDraw(dialogStatement);
        }
            
        //host.EndWindows();

        // Draw Edges:
        Edges().ForEach(DrawEdge);

        // Draw Dragable (new) Edge:
        if(controllMode == ControllMode.DRAWEDGE) {
            Vector2 p1;
            Vector2[] points;
            GetRectBorderCenters(startNode.NodeRect, out points);
            GetClosestPoint(out p1, points, Event.current.mousePosition);
            p1 = LineBoxIntersection(startNode.NodeRect, Event.current.mousePosition);
            Handles.color = Color.white;
            DrawEdgeLine(p1, Event.current.mousePosition);
        }

        HandleMouseActions();
        lastMousePos = Event.current.mousePosition;
    }

    public void EndGraphGUI() {
        GUI.EndScrollView();
        GUI.skin = null;
    }

    private void WindowFunction(int id) {
        var node = Nodes().First(n => n.Id == id);

      /*  EditorGUILayout.BeginVertical();
        EditorGUILayout.PrefixLabel("Speaker Name");
        node.speaker = EditorGUILayout.TextField(node.speaker);
        EditorGUILayout.PrefixLabel("Spoken Text");
        node.text = EditorGUILayout.TextArea(node.text);
        EditorGUILayout.EndVertical();*/

        if(Event.current.type != EventType.MouseDown) {
            GUI.DragWindow();
            return;
        }
        if(controllMode == ControllMode.DRAWEDGE && Event.current.button == 0) {
            // left clicket a node with "new edge drawing":
            if(startNode.Id != id) {
               
            }
        } else if(Event.current.button == 1) {
            EditorContextMenu(nodeDict, id);
            Event.current.Use();
        } else {
            Selection.activeObject = node;
            GUI.DragWindow();
        }
    }

    private void HandleMouseActions() {
        Vector2 v = Event.current.mousePosition;
        
        if (Event.current.button == 0 && Event.current.type == EventType.mouseDrag
            && selectedNode == null && selectedEdge == null) {
            if (Vector2.Distance(lastMousePos, v) < 50) {
                ScrollPosition -= Event.current.delta;
                Event.current.Use();
            }
            lastMousePos = v;
        }

        if (Event.current.type != EventType.MouseDown) {
            return;
        }
        
        
        selectedNode = null;
        selectedEdge = null;
        foreach(var edge in Edges()) {
            if(Vector2.Distance(v, ((edge.BoundingRect.size + edge.BoundingRect.position)*0.5f)) >
               (edge.BoundingRect.position - edge.BoundingRect.size).magnitude*0.5f
               ||
               !(PointdistanceToLine(Event.current.mousePosition, edge.BoundingRect.position, edge.BoundingRect.size) <
                 6))
                continue;
            selectedEdge = edge;
            Selection.activeObject = edge;
            Event.current.Use();

            if(Event.current.button == 1) {
                // rightclick:
                EditorContextMenu(edgeDict, edge.Id);
            }

            return;
        }
        if(Event.current.button > 1) return;
        Selection.activeObject = null;
        if(Event.current.button == 1) {
            // rightclick:
            if(controllMode == ControllMode.DRAWEDGE) {
                controllMode = ControllMode.SELECTION;
            } else {
                EditorContextMenu(menuDict);
            }
        }
        Event.current.Use();
    }

    private void EditorContextMenu(Dictionary<string, GenericMenu.MenuFunction2> dict, object param = null) {
        var menu = new GenericMenu();
        foreach(var m in dict) {
            menu.AddItem(new GUIContent(m.Key), false, m.Value, param);
        }
        menu.ShowAsContext();
    }

    private void DrawEdge(DialogAnswer edge) {
        Vector2 p1, p2;
        GetEndPoints(edge, out p1, out p2);
        Handles.color = edge.Equals(selectedEdge) ? Color.cyan : Color.white;
        DrawEdgeLine(p1, p2);
        edge.BoundingRect.position = p1;
        edge.BoundingRect.size = p2;
    }

    private void DrawEdgeLine(Vector2 @from, Vector2 to) {
        Handles.DrawAAPolyLine(3f, @from, to); // line
        // triangle:
        Vector2 dir = (to - @from).normalized;
        float distance = Vector2.Distance(@from, to);

        Vector2 v = @from + dir*distance*0.6f;
        Vector2 o = new Vector2(dir.y, -dir.x);
        Handles.DrawAAConvexPolygon(v + o*5f, v - o*5f, v + dir*12f);
    }

    private void GetEndPoints(DialogAnswer edge, out Vector2 start, out Vector2 end) {
        Rect r1 = edge.From.NodeRect, r2 = edge.To.NodeRect;

        List<DialogAnswer> CommonEdges =
            new List<DialogAnswer>(Edges().Where(e => (e.To == edge.To || e.To == edge.From)));
        CommonEdges.Sort((e1, e2) => e1.Id - e2.Id);

        // make it neat looking:
        int t = 0;
        if(CommonEdges.Count > 1) {
            for(int i = 0; i < CommonEdges.Count; ++i) {
                if(CommonEdges[i] == edge) {
                    t = i;
                    break;
                }
            }
            if(t%2 == 1) t = -t - 1;
        }

        start = LineBoxIntersection(r1, r2.center, r1.center + Vector2.right*t*8);
        end = LineBoxIntersection(r2, r1.center, r2.center + Vector2.right*t*8);
        

    }


    // Some static methods to calculate nice edges:

    /// <returns>The (positive) distance from A to the line from p1 to p2</returns>
    private static float PointdistanceToLine(Vector2 A, Vector2 p1, Vector2 p2) {
        Vector2 dir = p2 - p1;
        Vector2 perp = new Vector2(dir.y, -dir.x).normalized;

        float f = Vector2.Dot(p1, perp) - Vector2.Dot(A, perp);
        return Mathf.Abs(f);
    }

    #region STATIC_EDGE_HELPER_METHODS

    private static Vector2 LineBoxIntersection(Rect r, Vector2 end) {
        return LineBoxIntersection(r, end, r.center);
    }

    private static Vector2 LineBoxIntersection(Rect r, Vector2 end, Vector2 start) {
        Vector2 dir = (end - start).normalized;
        Vector2[] rectPoints;
        GetRectBorderCenters(r, out rectPoints);
        Vector2 @out;



        Vector2[] vset;
        if(Mathf.Abs(dir.y) > Mathf.Epsilon) {
            // test horizontal:
            float dy1 = rectPoints[0].y - start.y;
            float dy2 = rectPoints[1].y - start.y;
            float sx = Mathf.Abs(dir.x)*dy1/dir.y;

            vset = new Vector2[2];

            if(r.xMin <= start.x + sx && start.x + sx <= r.xMax) vset[0] = start + dir*dy1/dir.y;

            sx = Mathf.Abs(dir.x) * dy2/dir.y;
            if (r.xMin <= start.x + sx && start.x + sx <= r.xMax) vset[1] = start + dir * dy2 / dir.y;
            GetClosestPoint(out @out, vset, end);
            @out.x = Mathf.Clamp(@out.x, r.xMin, r.xMax);
            @out.y = Mathf.Clamp(@out.y, r.yMin, r.yMax);
            if (vset[0] != Vector2.zero || vset[1] != Vector2.zero) return @out;
        }
        // test vertical:
        float dx1 = rectPoints[2].x - start.x;
        float dx2 = rectPoints[3].x - start.x;
        vset = new Vector2[2];
        float sy = Mathf.Abs(dir.y)*dx1/dir.x;
        if (r.yMin <= start.y - sy && start.y - sy <= r.yMax) vset[0] = start + dir*dx1/dir.x;

        sy = Mathf.Abs(dir.y) * dx2/dir.x;
        if(r.yMin <= start.y - sy && start.y - sy <= r.yMax) vset[1] = start + dir*dx2/dir.x;
        GetClosestPoint(out @out, vset, end);
        @out.x = Mathf.Clamp(@out.x, r.xMin, r.xMax);
        @out.y = Mathf.Clamp(@out.y, r.yMin, r.yMax);
        if (vset[0] != Vector2.zero || vset[1] != Vector2.zero) return @out;
        return start;
    }

    private static void GetClosestPoint(out Vector2 point, Vector2[] from, Vector2 to) {
        point = from.Aggregate((v, w) => Vector2.Distance(v, to) < Vector2.Distance(w, to) ? v : w);
    }

    private static void GetRectBorderCenters(Rect r, out Vector2[] points) {
        points = new[] {
            r.center + Vector2.up*r.height/2,
            r.center - Vector2.up*r.height/2,
            r.center + Vector2.right*r.width/2,
            r.center - Vector2.right*r.width/2,
        };
    }

    #endregion

    public void OnSelectionChange() {
        _idGen = 0;
    }
}

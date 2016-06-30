/**
Copyright (c) <2015>, <Devon Klompmaker>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace uState
{
    public class StateMachineEditorWindow : EditorWindow
    {
        private static State manipulatingState;
        private static State selectedState;
        private static State createTransitionState;

        public static Transition selectedTransition;

        private static bool runtimeMode = false;
        private static StateMachine runtimeController;
        private ModelPropertyListRenderer propertyListRenderer;
        private TransitionConditionListRenderer transitionConditionListRenderer;
        private TransitionListRenderer transitionListRenderer;
        private Vector2 scrollOffset;

        [MenuItem("Window/State Editor")]
        public static void Initialize()
        {
            StateMachineEditorWindow window = EditorWindow.GetWindow<StateMachineEditorWindow>("uState");

            window.OnEnable();
        }

        [MenuItem("Assets/Create/State Model")]
        public static void CreateStateModel()
        {
            CodeGeneratorUtility.GenerateModel("New");
        }

        public void OnEnable()
        {
            GUIContent titleContent = this.GetTitle();
            titleContent.image = Resources.Load<Texture>("Images/compile_icon");
            titleContent.text = "State Editor";
        }

        public GUIContent GetTitle()
        {
#if UNITY_4_6
                PropertyInfo cachedTitleContent = GetType().GetProperty("cachedTitleContent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                return (GUIContent)cachedTitleContent.GetValue(this, null);
#endif

#if UNITY_5
            return titleContent;
#endif
        }

        public void DrawInfoBox()
        {
            GUI.Box(new Rect(0, EditorGUIUtility.singleLineHeight, 300, position.height), string.Empty, GUI.skin.FindStyle("flow overlay box"));

            GUILayout.BeginArea(new Rect(10, 10 + EditorGUIUtility.singleLineHeight, 280, position.height - 20));

            scrollOffset = GUILayout.BeginScrollView(scrollOffset);

            if (selectedState != null)
            {
                this.DrawSelectedStateInfo();
            }
            else
            {
                this.DrawModelInfo();
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        public void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.ExpandWidth(true));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Compile", Resources.Load<Texture>("Images/compile_icon"), "Generates the Interfaces needed to implement this state machine"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                CodeGeneratorUtility.GenerateInterfaceFromModel(selectedModel);
                CodeGeneratorUtility.GeneratePropertiesFromModel(selectedModel);
            }

            GUILayout.EndHorizontal();
        }

        public void DrawModelInfo()
        {
            if (runtimeMode && Application.isPlaying)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Viewing active instance of: " + selectedModel.name, EditorStyles.label);
                GUILayout.EndVertical();

                return;
            }
            else
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Editing Model: " + selectedModel.name, EditorStyles.label);
                GUILayout.EndVertical();
            }

            if (propertyListRenderer == null || propertyListRenderer.model != selectedModel)
            {
                propertyListRenderer = new ModelPropertyListRenderer(selectedModel);
            }

            propertyListRenderer.DrawList();
        }

        public void DrawSelectedStateInfo()
        {
            if(runtimeMode && Application.isPlaying)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Viewing active instance of: " + selectedModel.name, EditorStyles.label);
                GUILayout.EndVertical();

                return;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            selectedState.name = EditorGUILayout.TextField("State Name", selectedState.name);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Interface Generation", EditorStyles.helpBox);
            selectedState.generateEnter = EditorGUILayout.Toggle("Generate Enter", selectedState.generateEnter);
            selectedState.generateUpdate = EditorGUILayout.Toggle("Generate Update", selectedState.generateUpdate);
            selectedState.generateExit = EditorGUILayout.Toggle("Generate Exit", selectedState.generateExit);
            GUILayout.EndVertical();

            if (transitionListRenderer == null || selectedState != transitionListRenderer.state)
            {
                transitionListRenderer = new TransitionListRenderer(selectedModel, selectedState);
            }

            transitionListRenderer.DrawList();

            if (selectedTransition != null)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                selectedTransition.isMuted = EditorGUILayout.Toggle("Is Muted", selectedTransition.isMuted);

                GUILayout.EndVertical();

                if (transitionConditionListRenderer == null || selectedTransition != transitionConditionListRenderer.transition)
                {
                    transitionConditionListRenderer = new TransitionConditionListRenderer(selectedModel, selectedState, selectedTransition);
                }

                transitionConditionListRenderer.DrawList();


            }
        }

        public Model selectedModel
        {
            get
            {
                if (Selection.activeObject != null)
                {
                    if (typeof(Model).IsAssignableFrom(Selection.activeObject.GetType()))
                    {
                        runtimeMode = false;

                        return (Model)Selection.activeObject;
                    }
                }

                if (Selection.activeGameObject != null)
                {
                    if (Selection.activeGameObject.GetComponent<StateMachine>())
                    {
                        if (Selection.activeGameObject.GetComponent<StateMachine>().model != null)
                        {
                            runtimeMode = true;
                            runtimeController = Selection.activeGameObject.GetComponent<StateMachine>();

                            return Selection.activeGameObject.GetComponent<StateMachine>().model;
                        }
                    }
                }

                return null;
            }
        }

        private void OnGUI()
        {
            DrawBackground();

            if (selectedModel != null)
            {
                Texture editorGridBackground = Resources.Load<Texture>("Images/editor_grid_background");

                for (int x = (int)selectedModel.editorOffset.x; x <= this.position.width; x += editorGridBackground.width)
                {
                    for (int y = (int)selectedModel.editorOffset.y; y < this.position.height; y += editorGridBackground.height)
                    {
                        GUI.DrawTexture(new Rect(x, y, editorGridBackground.width, editorGridBackground.height), editorGridBackground);
                    }
                }

                Rect backgroundRect = new Rect(300, EditorGUIUtility.singleLineHeight, position.width - 300, position.height);

                GUI.BeginGroup(backgroundRect);

                if (createTransitionState != null)
                {
                    DrawStateMouseTransition(createTransitionState);
                }

                foreach (State state in selectedModel.states)
                {
                    DrawStateTransitions(state);
                }

                foreach (State state in selectedModel.states)
                {
                    OnStateEvents(state);
                    OnDrawState(state);
                }

                Rect checkrect = new Rect(0, 0, backgroundRect.width, backgroundRect.height);

                if (checkrect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.ContextClick)
                    {
                        DrawWindowContextMenu();

                        Event.current.Use();
                    }

                    if (Event.current.type == EventType.MouseDown)
                    {
                        selectedState = null;
                        selectedTransition = null;

                        Event.current.Use();
                    }

                    if (manipulatingState == null)
                    {
                        if (Event.current.type == EventType.MouseDrag)
                        {
                            selectedModel.editorOffset += Event.current.delta;
                            selectedModel.editorOffset.x = Mathf.Min(selectedModel.editorOffset.x, 0);
                            selectedModel.editorOffset.y = Mathf.Min(selectedModel.editorOffset.y, 0);

                            Event.current.Use();

                            EditorUtility.SetDirty(selectedModel);
                        }
                    }
                }

                GUI.EndGroup();

                this.DrawInfoBox();
                this.DrawToolbar();
            }

            Repaint();
        }

        private void OnDrawState(State state)
        {
            Rect stateRect = GetStateRect(state);
            GUIStyle stateStyle = OnGetDrawStyle(state);
            GUIStyle labelStyle = GUI.skin.label;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.normal.textColor = Color.white;

            EditorGUIUtility.AddCursorRect(stateRect, MouseCursor.MoveArrow);

            GUI.BeginGroup(stateRect, stateStyle);
            GUI.Label(new Rect(0, 0, stateRect.width, stateRect.height), GetStateContent(state), labelStyle);
            GUI.EndGroup();
        }

        private void OnDrawStateContextMenu(State state)
        {
            GenericMenu menu = new GenericMenu();

            if (!state.isDefault)
            {
                menu.AddItem(new GUIContent("Set As Default"), false, new GenericMenu.MenuFunction(delegate
                {
                    selectedModel.SetDefaultState(state.name);

                    EditorUtility.SetDirty(selectedModel);
                }));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Set As Default"));
            }

            menu.AddItem(new GUIContent("Create Transition"), false, new GenericMenu.MenuFunction(delegate
            {
                createTransitionState = state;

                EditorUtility.SetDirty(selectedModel);
            }));

            if (!state.isAnyState)
            {
                menu.AddSeparator(string.Empty);

                menu.AddItem(new GUIContent("Delete State"), false, new GenericMenu.MenuFunction(delegate
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to delete this state?", "Delete", "Cancel"))
                    {
                        selectedModel.states.Remove(state);

                        EditorUtility.SetDirty(selectedModel);
                    }
                }));
            }

            menu.ShowAsContext();
        }

        private GUIContent GetStateContent(State state)
        {
            return new GUIContent(state.name);
        }

        private GUIStyle OnGetDrawStyle(State state)
        {
            GUIStyle stateStyle = null;

            if (runtimeMode && Application.isPlaying)
            {
                if (state == runtimeController.currentState)
                {
                    stateStyle = GUI.skin.FindStyle("flow node 3");

                    if (state == selectedState)
                    {
                        stateStyle = GUI.skin.FindStyle("flow node 3 on");
                    }
                }
                else if (state.isAnyState)
                {
                    stateStyle = GUI.skin.FindStyle("flow node 2");

                    if (state == selectedState)
                    {
                        stateStyle = GUI.skin.FindStyle("flow node 2 on");
                    }
                }
                else
                {
                    stateStyle = GUI.skin.FindStyle("flow node 1");

                    if (state == selectedState)
                    {
                        stateStyle = GUI.skin.FindStyle("flow node 1 on");
                    }
                }
            }
            else
            {
                if (state.isDefault)
                {
                    stateStyle = GUI.skin.FindStyle("flow node 5");

                    if (state == selectedState)
                    {
                        stateStyle = GUI.skin.FindStyle("flow node 5 on");
                    }
                }
                else if (state.isAnyState)
                {
                    stateStyle = GUI.skin.FindStyle("flow node 2");

                    if (state == selectedState)
                    {
                        stateStyle = GUI.skin.FindStyle("flow node 2 on");
                    }
                }
                else
                {
                    stateStyle = GUI.skin.FindStyle("flow node 1");

                    if (state == selectedState)
                    {
                        stateStyle = GUI.skin.FindStyle("flow node 1 on");
                    }
                }
            }

            return stateStyle;
        }

        private void DrawWindowContextMenu()
        {
            GenericMenu context = new GenericMenu();

            context.AddItem(new GUIContent("Add New State"), false, new GenericMenu.MenuFunction2(delegate(object data)
            {
                Event currentEvent = (Event)data;

                State nState = new State();
                nState.name = "New State";
                nState.editorPosition = new Vector2((currentEvent.mousePosition.x - 300) - selectedModel.editorOffset.x, currentEvent.mousePosition.y - selectedModel.editorOffset.y);

                selectedModel.states.Add(nState);

                EditorUtility.SetDirty(selectedModel);

            }), Event.current);

            context.ShowAsContext();
        }

        private Rect GetStateRect(State state)
        {
            Rect stateRect = new Rect(state.editorPosition.x + selectedModel.editorOffset.x, state.editorPosition.y + selectedModel.editorOffset.y, 150, 30);

            return stateRect;
        }

        private void DrawStateMouseTransition(State state)
        {
            Rect stateRect = GetStateRect(state);
            Vector3 mousePoint = Event.current.mousePosition;

            Vector3 startPoint = ClosestPointOnRectangle(mousePoint, stateRect);
            Vector3 pointDir = mousePoint - startPoint;
            Vector3 t1 = startPoint + new Vector3(pointDir.x / 4, 0);
            Vector3 t2 = mousePoint;

            Handles.BeginGUI();

            Handles.DrawBezier(startPoint, mousePoint, t1, t2, Color.white, null, 3);

            Handles.EndGUI();
        }

        private void DrawStateTransitions(State state)
        {
            Rect stateRect = GetStateRect(state);

            List<Transition> removeTransitions = new List<Transition>();

            foreach (Transition transition in state.transitions)
            {
                State target = selectedModel.GetStateByGUID(transition.targetStateGUID);

                if (target != null)
                {
                    Color lineColor = Color.white;

                    if (transition == selectedTransition)
                    {
                        lineColor = Color.yellow;
                    }

                    if (transition.isMuted)
                    {
                        lineColor = Color.gray;
                        lineColor = new Color(lineColor.r, lineColor.g, lineColor.b, .15f);

                        if (transition == selectedTransition)
                        {
                            lineColor = Color.grey;
                        }
                    }

                    Rect targetRect = GetStateRect(target);

                    Vector3 startPoint = ClosestPointOnRectangle(targetRect.center, stateRect);
                    Vector3 endPoint = ClosestPointOnRectangle(startPoint, targetRect);
                    Vector3 pointDir = endPoint - startPoint;

                    Vector3 t1 = startPoint + new Vector3(pointDir.x / 2, 0);
                    Vector3 t2 = endPoint;
                    Vector3 endDir = (Vector3)targetRect.center - endPoint;

                    Texture arrowTexture = null;
                    float widthSubtract = 0;
                    float heightSubtract = 0;

                    if (endDir.x > 0 && endDir.y == 0)
                    {
                        arrowTexture = Resources.Load<Texture>("Images/arrow_right");
                        heightSubtract = arrowTexture.height / 2;
                        endPoint.x -= arrowTexture.width;
                        t2 -= new Vector3(pointDir.x / 2, 0);
                    }

                    if (endDir.x < 0 && endDir.y == 0)
                    {
                        arrowTexture = Resources.Load<Texture>("Images/arrow_left");
                        widthSubtract = arrowTexture.width;
                        heightSubtract = arrowTexture.height / 2;
                        t2 -= new Vector3(pointDir.x / 2, 0);
                        endPoint.x += arrowTexture.width;
                    }

                    if (endDir.x == 0 && endDir.y > 0)
                    {
                        arrowTexture = Resources.Load<Texture>("Images/arrow_down");
                        widthSubtract = arrowTexture.width / 2;
                        heightSubtract = arrowTexture.height;
                    }

                    if (endDir.x == 0 && endDir.y < 0)
                    {
                        arrowTexture = Resources.Load<Texture>("Images/arrow_up");
                        widthSubtract = arrowTexture.width / 2;
                        heightSubtract = 0;
                    }

                    GUI.color = lineColor;

                    Handles.BeginGUI();

                    Handles.DrawBezier(startPoint, endPoint, t1, t2, lineColor, null, 3);

                    Handles.EndGUI();

                    if (arrowTexture != null)
                    {
                        GUI.DrawTexture(new Rect(endPoint.x - widthSubtract, endPoint.y - heightSubtract, arrowTexture.width, arrowTexture.height), arrowTexture);
                    }

                    GUI.color = Color.white;
                }
            }

            foreach (Transition transition in removeTransitions)
            {
                state.transitions.Remove(transition);
            }
        }



        private void OnStateEvents(State state)
        {
            Rect stateRect = GetStateRect(state);

            if (stateRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    if (createTransitionState != null && createTransitionState != state)
                    {
                        Transition transition = createTransitionState.GetTransitionTo(state);

                        if (transition == null)
                        {
                            transition = new Transition();
                            transition.targetStateGUID = state.stateGUID;

                            createTransitionState.transitions.Add(transition);

                            EditorUtility.SetDirty(selectedModel);
                        }

                        createTransitionState = null;
                    }
                    else if (manipulatingState == null)
                    {
                        manipulatingState = state;
                        selectedState = state;
                        selectedTransition = null;

                        GUIUtility.keyboardControl = -1;
                    }

                    Event.current.Use();
                }

                // EDIT TIME ONLY EVENTS
                if (!runtimeMode || (runtimeMode && !Application.isPlaying))
                {
                    if (Event.current.type == EventType.ContextClick)
                    {
                        OnDrawStateContextMenu(state);

                        Event.current.Use();
                    }
                }
            }

            if (manipulatingState == state && Event.current.type == EventType.MouseDrag)
            {
                state.editorPosition += Event.current.delta;

                EditorUtility.SetDirty(selectedModel);
            }

            if (manipulatingState == state && Event.current.type == EventType.MouseUp)
            {
                manipulatingState = null;
            }
        }

        private void DrawBackground()
        {
            GUI.Box(new Rect(0, 0, this.position.width, this.position.height), string.Empty, GUI.skin.FindStyle("flow background"));
        }

        public static Vector2 ClosestPointOnRectangle(Vector2 point, Rect rect)
        {
            Vector2 pointA = new Vector2(rect.xMin, rect.center.y);
            Vector2 pointB = new Vector2(rect.xMax, rect.center.y);
            Vector2 pointC = new Vector2(rect.center.x, rect.yMax);
            Vector2 pointD = new Vector2(rect.center.x, rect.yMin);

            Vector2 closest = pointA;

            if (Vector2.Distance(point, closest) > Vector2.Distance(pointB, point)) { closest = pointB; }
            if (Vector2.Distance(point, closest) > Vector2.Distance(pointC, point)) { closest = pointC; }
            if (Vector2.Distance(point, closest) > Vector2.Distance(pointD, point)) { closest = pointD; }

            return closest;
        }

        public static Vector2 PointOnBezier(Vector2 s, Vector2 e, Vector2 st, Vector2 et, float t)
        {
            return (((-s + 3 * (st - et) + e) * t + (3 * (s + et) - 6 * st)) * t + 3 * (st - s)) * t + s;
        }
    }
}
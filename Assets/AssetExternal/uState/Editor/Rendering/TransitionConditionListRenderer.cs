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
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections;

namespace uState
{
    public class TransitionConditionListRenderer
    {
        public ReorderableList reorderableList;
        public Model model;
        public State state;
        public Transition transition;

        public TransitionConditionListRenderer(Model model, State state, Transition transition)
        {
            this.model = model;
            this.state = state;
            this.transition = transition;

            this.reorderableList = new ReorderableList(transition.conditions, typeof(TransitionCondition));
            this.reorderableList.drawElementCallback += new ReorderableList.ElementCallbackDelegate(this.OnDrawElement);
            this.reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Transition Conditions");
            };
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            TransitionCondition condition = transition.conditions[index];

            float spacing = rect.width / 2;

            int propertyIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, spacing, EditorGUIUtility.singleLineHeight), GetIndexFrom(AvailablePropertyNames(), condition.propertyName), AvailablePropertyNames());

            condition.propertyName = GetSelectionByIndex(AvailablePropertyNames(), propertyIndex);

            Property property = model.GetProperty(condition.propertyName);

            if (property != null)
            {
                switch (property.propertyType)
                {
                    case IPropertyType.BOOL:
                        condition.boolValue = EditorGUI.Toggle(new Rect(rect.xMax - 15, rect.y, 15, EditorGUIUtility.singleLineHeight), condition.boolValue);
                        break;
                    case IPropertyType.FLOAT:
                        condition.numericCompareType = (ITransitionNumericCompareType)EditorGUI.EnumPopup(new Rect(rect.x + spacing, rect.y, spacing / 2, EditorGUIUtility.singleLineHeight), condition.numericCompareType);
                        condition.floatValue = EditorGUI.FloatField(new Rect(rect.x + (spacing * 1.5f), rect.y, spacing / 2 , EditorGUIUtility.singleLineHeight), condition.floatValue);
                        break;
                    case IPropertyType.INT:
                        condition.numericCompareType = (ITransitionNumericCompareType)EditorGUI.EnumPopup(new Rect(rect.x + spacing, rect.y, spacing / 2, EditorGUIUtility.singleLineHeight), condition.numericCompareType);
                        condition.intValue = EditorGUI.IntField(new Rect(rect.x + (spacing * 1.5f), rect.y, spacing / 2, EditorGUIUtility.singleLineHeight), condition.intValue);
                        break;
                    case IPropertyType.GAMEOBJECT:
                        condition.gameObjectCompareType = (ITransitionGameObjectCompareType)EditorGUI.EnumPopup(new Rect(rect.x + spacing, rect.y, spacing / 2, EditorGUIUtility.singleLineHeight), condition.gameObjectCompareType);
                        condition.boolValue = EditorGUI.Toggle(new Rect(rect.xMax - 15, rect.y, 15, EditorGUIUtility.singleLineHeight), condition.boolValue);
                        break;
                }
            }
        }

        public void DrawList()
        {
            this.reorderableList.DoLayoutList();
        }

        public int GetIndexFrom(string[] array, string selected)
        {
            for(int i = 0; i < array.Length; i++)
            {
                if(array[i] == selected)
                {
                    return i;
                }
            }

            return 0;
        }

        public string GetSelectionByIndex(string[] array, int selected)
        {
            if(array.Length > selected)
            {
                return array[selected];
            }

            return string.Empty;
        }

        public string[] AvailablePropertyNames()
        {
            var results = from property in model.properties
                          select property.name;

            return results.ToArray();
        }
    }
}

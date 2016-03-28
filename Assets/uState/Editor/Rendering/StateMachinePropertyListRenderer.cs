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
using System.Collections;

namespace uState
{
    public class StateMachinePropertyListRenderer
    {
        public ReorderableList reorderableList;
        public StateMachine stateMachine;

        public StateMachinePropertyListRenderer(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            this.reorderableList = new ReorderableList(stateMachine.properties, typeof(Property));
            this.reorderableList.drawElementCallback += new ReorderableList.ElementCallbackDelegate(this.OnDrawElement);
            this.reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Instance Properties");
            };
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            Property property = stateMachine.properties[index];

            float x = rect.x;

            property.name = EditorGUI.TextField(new Rect(x, rect.y, 120, EditorGUIUtility.singleLineHeight), property.name); x += 125;

            float propertyWidth = rect.xMax - x;

            switch(property.propertyType) 
            {
                case IPropertyType.BOOL:
                    property.boolValue = EditorGUI.Toggle(new Rect(rect.xMax - 15, rect.y, 15, EditorGUIUtility.singleLineHeight), property.boolValue);
                    break;
                case IPropertyType.FLOAT:
                    property.floatValue = EditorGUI.FloatField(new Rect(x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), property.floatValue);
                    break;
                case IPropertyType.INT:
                    property.intValue = EditorGUI.IntField(new Rect(x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), property.intValue);
                    break;
                case IPropertyType.GAMEOBJECT:
                    property.gameObjectValue = (GameObject)EditorGUI.ObjectField(new Rect(x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), property.gameObjectValue, typeof(GameObject), true);
                    break;
                case IPropertyType.COMPONENT:
                    property.componentValue = (Component)EditorGUI.ObjectField(new Rect(x, rect.y, propertyWidth, EditorGUIUtility.singleLineHeight), property.componentValue, typeof(Component), true);
                    break;
            }
        }

        public void DrawList()
        {
            this.reorderableList.DoLayoutList();
        }
    }
}

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
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using uState;

[AddComponentMenu("Miscellaneous/State Machine")]
public class StateMachine : MonoBehaviour
{
    public Model model;

    private string m_currentState;
    private Dictionary<string, List<Component>> m_cachedComponents = new Dictionary<string, List<Component>>();
    private List<Property> m_properties = new List<Property>();

    public List<Property> properties
    {
        get
        {
            return m_properties;
        }
    }

    public State currentState
    {
        get
        {
            return model.GetStateByName(m_currentState);
        }
    }

    public Property FindPropertyByName(string name)
    {
        var result = from property in m_properties
                     where property.name == name
                     select property;

        return result.FirstOrDefault();
    }

    private void Start()
    {
        UpdateCachedComponents();

        foreach (Property property in model.properties)
        {
            Property pInstance = new Property();
            pInstance.name = property.name;
            pInstance.propertyType = property.propertyType;
            pInstance.intValue = property.intValue;
            pInstance.floatValue = property.floatValue;
            pInstance.boolValue = property.boolValue;
            pInstance.gameObjectValue = property.gameObjectValue;
            pInstance.componentValue = property.componentValue;

            m_properties.Add(pInstance);
        }

        foreach (State state in model.states)
        {
            if (state.isDefault)
            {
                GoToState(state.name);

                break;
            }
        }
    }

    public void GoToState(State state)
    {
        GoToState(state.name);
    }

    public void GoToState(string stateName)
    {
        if (!string.IsNullOrEmpty(m_currentState) && m_currentState != stateName)
        {
            if (currentState != null)
            {
                InvokeOnExit(currentState);
            }
        }

        if (m_currentState != stateName)
        {
            m_currentState = stateName;

            InvokeOnEnter(currentState);
        }
    }

    private void Update()
    {
        this.InvokeOn(currentState);
        this.EvaluateStateTransitions(currentState);

        State anyState = model.GetStateByName("Any State");

        if (anyState != null)
        {
            this.InvokeOn(anyState);
            this.EvaluateStateTransitions(anyState);
        }
    }

    private void EvaluateStateTransitions(State state)
    {
        foreach (Transition transition in state.transitions)
        {
            if (transition.isMuted) { continue; }

            bool invokeTransition = true;

            foreach (TransitionCondition condition in transition.conditions)
            {
                Property property = FindPropertyByName(condition.propertyName);

                if (property != null)
                {
                    if (condition.DoesPassCondition(property) == false)
                    {
                        invokeTransition = false;
                    }
                }
            }

            if (invokeTransition)
            {
                GoToState(model.GetStateByGUID(transition.targetStateGUID));

                break;
            }
        }
    }

    private void UpdateCachedComponents()
    {
        m_cachedComponents.Clear();

        foreach (State state in model.states)
        {
            Type interfaceType = Type.GetType(state.interfaceName);

            if (interfaceType != null)
            {
                foreach (Component component in GetComponents<Component>())
                {
                    if (interfaceType.IsAssignableFrom(component.GetType()))
                    {
                        if (!m_cachedComponents.ContainsKey(state.interfaceName))
                        {
                            m_cachedComponents.Add(state.interfaceName, new List<Component>());
                        }

                        m_cachedComponents[state.interfaceName].Add(component);
                    }
                }
            }
        }
    }

    private void InvokeOnEnter(State state)
    {
        if (m_cachedComponents.ContainsKey(state.interfaceName))
        {
            foreach (Component component in m_cachedComponents[state.interfaceName])
            {
                MethodInfo methodInfo = component.GetType().GetMethod("OnEnter" + state.safeStateName);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(component, null);
                }
            }
        }
    }

    private void InvokeOn(State state)
    {
        if (m_cachedComponents.ContainsKey(state.interfaceName))
        {
            foreach (Component component in m_cachedComponents[state.interfaceName])
            {
                MethodInfo methodInfo = component.GetType().GetMethod("On" + state.safeStateName);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(component, null);
                }
            }
        }
    }

    private void InvokeOnExit(State state)
    {
        if (m_cachedComponents.ContainsKey(state.interfaceName))
        {
            foreach (Component component in m_cachedComponents[state.interfaceName])
            {
                MethodInfo methodInfo = component.GetType().GetMethod("OnExit" + state.safeStateName);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(component, null);                
                }
            }
        }
    }

    public void SetProperty<T>(string propertyName, T value)
    {
        Property property = FindPropertyByName(propertyName);
        
        if(property == null)
        {
            Debug.LogErrorFormat("{0} - {1}", "StateMachine Property Error", "Could not find property of name => " + propertyName);

            return;
        }

        switch (typeof(T).Name)
        {
            case "Int32":
                property.SetInt((int)(object)value);
                break;
            case "Single":
                property.SetFloat((float)(object)value);
                break;
            case "Boolean":
                property.SetBool((bool)(object)value);
                break;
            case "GameObject":
                property.SetGameObject((GameObject)(object)value);
                break;
        }

        if (typeof(Component).IsAssignableFrom(typeof(T)))
        {
            property.SetComponent((Component)(object)value);
        }
    }

    public T GetProperty<T>(string propertyName)
    {
        Property property = FindPropertyByName(propertyName);

        if (property == null)
        {
            Debug.LogErrorFormat("{0} - {1}", "StateMachine Property Error", "Could not find property of name => " + propertyName);

            return default(T);
        }

        switch(typeof(T).Name)
        {
            case "Int32":
                return (T)(object)property.GetInt();
            case "Single":
                return (T)(object)property.GetFloat();
            case "Boolean":
                return (T)(object)property.GetBool();
            case "GameObject":
                return (T)(object)property.GetGameObject();
        }

        if(typeof(Component).IsAssignableFrom(typeof(T)))
        {
            return (T)(object)property.GetComponent();
        }

        return default(T); 
    }

    public void AdjustProperty(string propertyName, int amount)
    {
        SetProperty<int>(propertyName, GetProperty<int>(propertyName) + amount);
    }

    public void AdjustProperty(string propertyName, float amount)
    {
        SetProperty<float>(propertyName, GetProperty<float>(propertyName) + amount);
    }
}

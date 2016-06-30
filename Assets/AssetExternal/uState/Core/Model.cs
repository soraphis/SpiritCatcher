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
using System.Linq;
using System.Collections.Generic;

namespace uState
{
    public class Model : ScriptableObject
    {
        [HideInInspector]
        public List<State> states = new List<State>();
        [HideInInspector]
        public List<Property> properties = new List<Property>();
        [HideInInspector]
        public Vector2 editorOffset;
        [HideInInspector]
        public string propertiesAssetGUID;

        public State GetStateByName(string name)
        {
            var result = from state in states
                         where state.name == name
                         select state;

            return result.FirstOrDefault();
        }

        public State GetStateByGUID(string guid)
        {
            var result = from state in states
                         where state.stateGUID == guid
                         select state;

            return result.FirstOrDefault();
        }

        public Property GetProperty(string name)
        {
            var result = from property in properties
                         where property.name == name
                         select property;

            return result.FirstOrDefault();
        }

        public void SetDefaultState(string name)
        {
            foreach(State state in states)
            {
                if(state.name == name)
                {
                    state.isDefault = true;
                }
                else
                {
                    state.isDefault = false;
                }
            }
        }

        private void OnEnable()
        {
            State anyState = GetStateByName("Any State");

            if(anyState == null)
            {
                anyState = new State();
                anyState.name = "Any State";
                anyState.isAnyState = true;
                anyState.isDefault = true;

                states.Add(anyState);
            }
        }
    }
}
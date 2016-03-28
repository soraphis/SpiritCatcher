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
using System.Collections;

namespace uState
{
    [System.Serializable]
    public class Property
    {
        public string name;
        public IPropertyType propertyType;

        public bool boolValue;
        public int intValue;
        public float floatValue;
        public Component componentValue;
        public GameObject gameObjectValue;

        public int GetInt()
        {
            return this.intValue;
        }

        public float GetFloat()
        {
            return this.floatValue;
        }

        public bool GetBool()
        {
            return this.boolValue;
        }

        public Component GetComponent()
        {
            return componentValue;
        }

        public GameObject GetGameObject()
        {
            return gameObjectValue;
        }

        public void SetInt(int value)
        {
            this.intValue = value;
        }

        public void SetFloat(float value)
        {
            this.floatValue = value;
        }

        public void SetBool(bool value)
        {
            this.boolValue = value;
        }

        public void SetComponent(Component value)
        {
            this.componentValue = value;
        }

        public void SetGameObject(GameObject value)
        {
            this.gameObjectValue = value;
        }
    }

    public enum IPropertyType
    {
        BOOL,
        INT,
        FLOAT,
        GAMEOBJECT,
        COMPONENT
    }
}

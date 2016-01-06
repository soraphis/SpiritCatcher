using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.Soraphis.Lib;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {
    [Serializable]
    public class AttributeDict : SerializableDictionary<string, float> {

        public AttributeDict() : base() {
            
        }

        public AttributeDict(SerializationInfo info, StreamingContext context) : base(info, context) {
            
        }
    }

    [Serializable]
    public class SpiritType {

        [SerializeField] public string Name;
        [SerializeField] public AttributeDict Attributes;
    }
}

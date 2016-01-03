using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Soraphis.Lib;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {
    [Serializable] public class AttributeDict : SerializableDictionary<string, float> { }

    [Serializable]
    public class SpiritType {

        [SerializeField] public string Name;
        [SerializeField] public AttributeDict Attributes;
    }
}

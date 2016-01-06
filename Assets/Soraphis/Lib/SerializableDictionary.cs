/*
 christophfranke123 @ answers.unity3d.com:
 http://answers.unity3d.com/answers/809221/view.html
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Assets.Soraphis.Lib {
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public SerializableDictionary() {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context) {
        }


        // save the dictionary to lists
        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this) {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize() {
            this.Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (int i = 0; i < keys.Count; i++)
                this.Add(keys[i], values[i]);
        }
    }
}
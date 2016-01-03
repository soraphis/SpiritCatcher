using System;
using System.Linq;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {

    [Serializable]
    public class Spirit {
        public string Name;
        public int Level;
        public readonly SpiritType Type;

        [HideInInspector] public float currentHP;
        [HideInInspector] public float expierience; // -> to next level
        private float currentStamina;

        [SerializeField] public AttributeDict Attribute;

        public float CurrentStamina {
            get { return currentStamina; }
            set { currentStamina = Mathf.Clamp(value, 0, 100);  }
        }

        protected Spirit(SpiritType type) {
            Type = type;
        }

        public static Spirit GenerateSpirit(string s, int level) {
            return GenerateSpirit(Game.Instance.SpiritLibrary.Spirits.First(x => x.Name == s), level);
        }

        public static Spirit GenerateSpirit(SpiritType t, int level) {
            var spirit = new Spirit(t) {
                Level = level,
                Name = t.Name,
                Attribute = new AttributeDict()
            };

            foreach(var attr in t.Attributes) {
                bool b = attr.Key.Contains("Level");
                var Key = b ? attr.Key.Substring(5) : attr.Key;
                float Val = b ? attr.Value * level : attr.Value;

                if (spirit.Attribute.ContainsKey(Key))
                    spirit.Attribute[Key] = spirit.Attribute[Key] + Val;
                else
                    spirit.Attribute.Add(Key, Val);
            }

            spirit.currentHP = spirit.Attribute["HP"];
            return spirit;
        }

    }

}


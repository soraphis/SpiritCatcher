using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {
    [Serializable]
    public class Attack {
        public AttackType AttackType;
        public float Precision;
        public float StaminaCost;
        public float BaseDMG;
        public Type Type;
    }

    [Serializable]
    public class Spirit : ICloneable {
        public string Name;
        public int Level;
        public SpiritType Type;

        [SerializeField] private float currentHP;
        [HideInInspector] public float expierience; // -> to next level
        private float currentStamina;

        // [SerializeField] public AttackList Attribute;

            // "SKILL POINTS"
        [SerializeField] public int HP;
        [SerializeField] public int DMG;
        [SerializeField] public int DEF;
        [SerializeField] public int SPEED;
        [SerializeField] public int CRIT;
        [SerializeField] public int INIT;

        [Obsolete][HideInInspector] public float BTL_HP => HP * SpiritType.PP_HP;
        [Obsolete][HideInInspector] public float BTL_SPEED => SPEED * SpiritType.PP_SPEED;

        public List<Attack> Attacks = new List<Attack>(4);


        public float CurrentHP {
            get { return currentHP; }
            set { currentHP = Mathf.Clamp(value, 0, HP* SpiritType.PP_HP); }
        }

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

                HP = t.HP + t.PL_HP * level,
                DEF = t.DEF + t.PL_DEF * level,
                SPEED = t.SPEED + t.PL_SPEED * level,
                DMG = t.DMG + t.PL_DMG * level,
                CRIT = t.CRIT,
                INIT = t.INIT,
            };

            /*foreach(var attr in t.Attributes) {
                bool b = attr.Key.Contains("Level");
                var Key = b ? attr.Key.Substring(5) : attr.Key;
                float Val = b ? attr.Value * level : attr.Value;

                if (spirit.Attribute.ContainsKey(Key))
                    spirit.Attribute[Key] = spirit.Attribute[Key] + Val;
                else
                    spirit.Attribute.Add(Key, Val);
            }*/

            spirit.currentHP = spirit.HP;
            return spirit;
        }

        public object Clone() {
            Spirit copy = new Spirit(this.Type);
            copy.Name = Name;
            copy.Level = Level;
            copy.currentHP = currentHP;
            copy.expierience = expierience;
            copy.CurrentStamina = currentStamina;
            copy.HP = HP;
            copy.DEF = DEF;
            copy.DMG = DMG;
            copy.SPEED = SPEED;
            copy.CRIT = CRIT;
            copy.INIT = INIT;

            copy.Attacks = Attacks.ToList();

            return copy;
        }
    }

}


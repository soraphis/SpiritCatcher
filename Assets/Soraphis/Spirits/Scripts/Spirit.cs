using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Soraphis.Spirits.Scripts {


    [Serializable]
    public class Attack {
        public AttackName AttackName;
        public string Name;
        public float Accuracy;
        public float StaminaCost;
        public float BaseDMG;
        public AttackType Type;

        /// <returns>true if the attack will hit</returns>
        public bool CalculateAccuracy(Spirit s, out float acc) {
            var p = Random.Range(0, Accuracy);
            acc = (p + (5 - s.ACC));
            return acc < Accuracy;
        }

        public int CalculateDamage(Spirit attacker, Spirit defender, float acc, out bool crit, out float effective) {
            var base_dmg = BaseDMG * (1 + (attacker.DMG * SpiritType.PP_DMG) / 100f);
            crit = acc < Accuracy/2 && Random.value < attacker.CRIT*SpiritType.PP_CRIT;
            if (crit) base_dmg *= 2; // crit

            effective = 1f;
            base_dmg *= effective;
            
            // var dmg = base_dmg - (defender.DEF/2f*SpiritType.PP_DEF); // flat dmg absorption
            var dmg = base_dmg*(SpiritType.PP_DEF*defender.DEF)/(2* (1 + Mathf.Clamp(defender.Level - attacker.Level, 0, 100)) + (SpiritType.PP_DEF*defender.DEF));
 

            return Mathf.FloorToInt(Mathf.Max(0, dmg));
        }
    }

    [Serializable]
    public class Spirit : ICloneable {

        public string Name;
        public int Level;
        public SpiritType Type;

        [SerializeField] private float currentHP;
        [HideInInspector] public int expierience; // -> to next level
        private float currentStamina;

        // [SerializeField] public AttackList Attribute;

            // "SKILL POINTS"
        [SerializeField] public int HP;
        [SerializeField] public int DMG;
        [SerializeField] public int DEF;
        [SerializeField] public int SPEED;
        [SerializeField] public int CRIT;
        [SerializeField] public int ACC;

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

        public Spirit(SpiritType type) {
            Type = type;
        }

        public void getExperience(int amount) {
            expierience += amount;
            var cap = (int) ((1.5f + this.Level)*20);
            if(expierience > cap) {
                expierience %= cap;
                this.Level++;

                
                var newAttacks = this.Type.Attacks.List.Where(a => a.Level == this.Level).ToArray();
                if(newAttacks.Length > 0) {
                    var atklib = Game.Instance.AttackLibrary;
                    foreach(var attack in newAttacks) {
                        var atk = atklib.Attacks.First(a => a.Name == attack.Attack);
                        if(atk != null) Attacks.Add(atk);
                    }
                }

            }
        }

        public static Spirit GenerateSpirit(string s, int level) {
            return GenerateSpirit(Game.Instance.SpiritLibrary.Spirits.First(x => x.Name == s), level);
        }

        public static Spirit GenerateSpirit(SpiritType t, int level, AttackLibrary atklib = null) {
            var spirit = new Spirit(t) {
                Level = level,
                Name = t.Name,

                HP = t.HP + t.PL_HP * level,
                DEF = t.DEF + t.PL_DEF * level,
                SPEED = t.SPEED + t.PL_SPEED * level,
                DMG = t.DMG + t.PL_DMG * level,
                CRIT = t.CRIT,
                ACC = t.ACC,
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

            atklib = atklib ?? Game.Instance.AttackLibrary;
            spirit.currentHP = spirit.HP * SpiritType.PP_HP;

            for(int x = 0, i = t.Attacks.List.Length-1; x < 4 && i >= 0; --i) {
                if(t.Attacks.List[i].Level > level) continue;
                ++x;
                var atk = atklib.Attacks.First(a => a.Name == t.Attacks.List[i].Attack);
                spirit.Attacks.Add(atk);
            }

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
            copy.ACC = ACC;

            copy.Attacks = Attacks.ToList();

            return copy;
        }
    }

}


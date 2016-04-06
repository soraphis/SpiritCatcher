using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {

    [Serializable]
    [CreateAssetMenu(menuName = "Spirit/Team")]
    public class SpiritTeam : ScriptableObject, ICloneable {
        public List<Spirit> Spirits;

        public void OnEnable() {
            Spirits = Spirits ?? new List<Spirit>();
        }

        public object Clone() {
            SpiritTeam copy = ScriptableObject.CreateInstance(typeof(SpiritTeam)) as SpiritTeam;
            copy.Spirits = new List<Spirit>(Spirits.Count);
            for(int index = 0; index < Spirits.Count; index++) {
                var spirit = Spirits[index];
                copy.Spirits.Add(spirit.Clone() as Spirit);
            }
            return copy;
        }
    }
}

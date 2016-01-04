using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {

    [Serializable]
    [CreateAssetMenu(menuName = "Spirit/Team")]
    public class SpiritTeam : ScriptableObject, ICloneable {
        public Spirit[] Spirits;

        public object Clone() {
            SpiritTeam copy = new SpiritTeam();
            copy.Spirits = new Spirit[Spirits.Length];
            for(int index = 0; index < Spirits.Length; index++) {
                var spirit = Spirits[index];
                copy.Spirits[index] = spirit.Clone() as Spirit;
            }
            return copy;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {

    [Serializable]
    [CreateAssetMenu(menuName = "Spirit/Team")]
    public class SpiritTeam : ScriptableObject {
        public Spirit[] Spirits;
    }
}

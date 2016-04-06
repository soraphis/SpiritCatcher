using System;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {
    [Serializable]
    [CreateAssetMenu(menuName = "Spirit/AttackLibrary")]
    public class AttackLibrary : ScriptableObject {
        public Attack[] Attacks;

        public void OnEnable() {
            
        }

    }
}
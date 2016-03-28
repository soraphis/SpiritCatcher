using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.Soraphis.Lib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Soraphis.Spirits.Scripts {

    [Serializable]
    public class SpiritType : ScriptableObject {

        public const int PP_HP = 5; // per point
        public const int PP_DMG = 2; 
        public const float PP_DEF = 0.8f; 
        public const float PP_SPEED = 0.2f;
        public const float PP_CRIT = 0.7f;

        [SerializeField] public string Name;
        [SerializeField] public Sprite Image;
        [SerializeField] public Sprite ImageBack;

        [SerializeField] public string Typ;

        // "skill points"
        [SerializeField] public int HP;
        [SerializeField] public int DMG;
        [SerializeField] public int DEF;
        [SerializeField] public int SPEED;
        [SerializeField] public int CRIT;
        [SerializeField] public int INIT;

        [SerializeField] public int PL_HP;
        [SerializeField] public int PL_DMG;
        [SerializeField] public int PL_DEF;
        [SerializeField] public int PL_SPEED;

        [SerializeField] public float Catchiness;
        [SerializeField] public bool isStarter;
        [SerializeField] public bool isLegendary;
        

        [SerializeField] public Attack[] Attacks;
    }
}

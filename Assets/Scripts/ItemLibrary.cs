using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Assets.Scripts.ViewModels;
using Assets.Soraphis.Spirits.Scripts;
using UnityEngine;

public static class ItemLibrary {
    static ItemLibrary() {  Init(); }

    private static List<ItemType> items = null;

    private static void Init() {
        items = new List<ItemType>();
        items.Add(new ItemType("Heilkraut", 10, true, true, onUse: (onEnd) => {
            OverlayView.Instance.ShowOverlay(OverlayView.Overlay.TeamView, (spiritnum) => {

                if(! Player.Instance.team.Any()) {
                    Debug.Log("Das kannst du noch nicht verwenden");
                    onEnd.Invoke();
                    return;
                }

                if(Player.Instance.team[spiritnum].CurrentHP < Player.Instance.team[spiritnum].HP*SpiritType.PP_HP &&
                   Player.Instance.team[spiritnum].CurrentHP > 0) {

                    Player.Instance.team[spiritnum].CurrentHP = Mathf.Min(
                        Player.Instance.team[spiritnum].CurrentHP + 10,
                        Player.Instance.team[spiritnum].HP*SpiritType.PP_HP);
                    Player.Instance.Items.AddItem("Heilkraut", -1);
                } else {
                    Debug.Log("Das h�tte keinen effekt");
                }
                onEnd.Invoke();
            });
        }));
        items.Add(new ItemType("Extractor", -1, false, false));
    }

    public static ItemType Lookup(string name) {
        if(items == null) Init();
        return items.First(i => i.Name == name);
    }

    public class ItemType {
        public string Name { get; private set; }
        public int price { get; private set; }

        public bool UsableInFight { get; private set; }
        public bool UsableInWorld { get; private set; }
        public bool CanBeSold { get; private set; }
        public bool Stackable { get; private set; }

        public Action<Action> OnUse { get; private set; }

        internal ItemType(string name, 
                        int price = 0, 
                        bool usableInFight = false, 
                        bool usableInWorld = false, 
                        bool stackable = true, 
                        bool canBeSold = true, 
                        Action<Action> onUse = null) {
            Name = name;
            this.price = price;
            UsableInFight = usableInFight;
            UsableInWorld = usableInWorld;
            Stackable = stackable;
            CanBeSold = canBeSold;
            OnUse = onUse;
        }

    }



}

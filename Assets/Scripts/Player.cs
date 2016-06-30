using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Soraphis.SaveGame;
using Assets.Soraphis.Spirits.Scripts;
using Gamelogic;
using UnityEngine.UI;

public class Player : Singleton<Player>, Saveable {

    public class InventoryElement {
        public string Name;
        public int Amount;
        public InventoryElement(string name, int amount) {
            Name = name;
            Amount = amount;
        }
    }

    public class Inventory {
        private List<InventoryElement> elements = new List<InventoryElement>();

        public event Action OnChange;

        public bool Contains(string name) {
            return elements.Any(e => e.Name == name);
        }

        public bool Contains(string name, int amount) {
            return elements.Any(e => e.Name == name && e.Amount == amount);
        }

        public InventoryElement this[string name] {
            get { return elements.Find(e => e.Name == name); }
            set {
                elements[elements.FindIndex(e => e.Name == name)] = value;
                if (OnChange != null) {
                    OnChange.Invoke();
                }
            }
        }

        public void AddItem(string name, int amount = 1) {
            if(!this.Contains(name)) elements.Add(new InventoryElement(name, amount));
            else {
                if(this[name].Amount + amount >= 0) this[name].Amount += amount;
                else throw new ArgumentException("removing more items then there are");
            }
            if(this[name].Amount == 0) elements.Remove(this[name]);
            if(OnChange != null) {
                OnChange.Invoke();
            }
        }

        public List<InventoryElement> GetElements() {
            return elements.ToList();
        }
    }


    public enum ActionState {
        Default, Talking, Battle, Cutszene // dont know .... 
    }

    public void Start() {
        if(team == null || team.Count == 0) {
            team = new List<Spirit> {
                // Spirit.GenerateSpirit(Game.Instance.SpiritLibrary.Spirits[0], 5)
            };
        }
    }

    public ActionState CurrentActionState = ActionState.Default;
    public List<Spirit> team;

    public bool isOutdoor = false;
    public Inventory Items = new Inventory();
    public int money = 10;

    public IEnumerator DefeatedInFight() {
        yield return StartCoroutine(FadeScreen.Instance.FadeOut(0, 1f));
        Game.Instance.GameTime += new GameDate(hours:3);
        team[0].CurrentHP = team[0].HP * SpiritType.PP_HP / 2f;

        yield return StartCoroutine(FadeScreen.Instance.FadeIn(0, 1f));
    }

    public void Load(DataNode parent) {
        DataNode node = parent.GetChild("Player");
        if (node == null) return;
        var t_node = node.GetChild("team");

        team = new List<Spirit>();

        foreach(var c in t_node.Children) {
            var type = Game.Instance.SpiritLibrary.Spirits.First(t => t.Name == c.GetChild("Type").Get<string>());
            var spirit = new Spirit(type);

            spirit.Level = c.GetChild("Level").Get<int>();
            spirit.HP = c.GetChild("HP").Get<int>();
            spirit.CurrentHP = c.GetChild("CurrentHP").Get<float>();
            spirit.expierience = c.GetChild("EP").Get<int>();

            spirit.DMG = c.GetChild("DMG").Get<int>();
            spirit.DEF = c.GetChild("DEF").Get<int>();
            spirit.SPEED = c.GetChild("SPEED").Get<int>();
            spirit.ACC = c.GetChild("ACC").Get<int>();
            spirit.CRIT = c.GetChild("CRIT").Get<int>();
            spirit.Name = c.GetChild("Name").Get<string>();

            var a_node = c.GetChild("Attack");

            foreach(var a in a_node.Children) {
                var atk = new Attack();
                atk.Name = a.GetChild("Name").Get<string>();
                atk.Type = (AttackType) a.GetChild("Type").Get<int>();
                atk.Accuracy = a.GetChild("Accuracy").Get<float>();
                atk.AttackName = (AttackName) a.GetChild("AttackName").Get<int>();
                atk.BaseDMG = a.GetChild("BaseDMG").Get<int>();
                atk.StaminaCost = a.GetChild("StaminaCost").Get<int>();
                spirit.Attacks.Add(atk);
            }

            team.Add(spirit);
        }

        var i_node = node.GetChild("items");
        // var items = Items.GetElements();
        foreach(var item in i_node.Children) {
            Items.AddItem(item.Name, (int) item.Value);
        }

    }

    public DataNode Save() {
        DataNode node = new DataNode();
        node.Name = "Player";

        var t_node = node.AddChild("team");
        for(int i = 0; i < team.ToArray().Length; i++) {
            var spirit = team.ToArray()[i];
            var c = t_node.AddChild(spirit.Name + "_" + i);

            c.AddChild("Type", spirit.Type.Name);

            c.AddChild("Level", spirit.Level);
            c.AddChild("EP", spirit.expierience);
            c.AddChild("HP", spirit.HP);
            c.AddChild("DMG", spirit.DMG);
            c.AddChild("DEF", spirit.DEF);
            c.AddChild("SPEED", spirit.SPEED);
            c.AddChild("ACC", spirit.ACC);
            c.AddChild("CRIT", spirit.CRIT);
            c.AddChild("Name", spirit.Name);
            c.AddChild("CurrentHP", spirit.CurrentHP);

            var a_node = c.AddChild("Attack");

            foreach (var attack in spirit.Attacks) {
                var a = a_node.AddChild("" + spirit.Attacks.IndexOf(attack));
                a.AddChild("Name", attack.Name);
                a.AddChild("Type", (int)attack.Type);
                a.AddChild("Accuracy", attack.Accuracy);
                a.AddChild("AttackName", (int)attack.AttackName);
                a.AddChild("BaseDMG", attack.BaseDMG);
                a.AddChild("StaminaCost", attack.StaminaCost);
            }
        }

        var i_node = node.AddChild("items");
        var items = Items.GetElements();
        for (int i = 0; i < items.Count; ++i) {
            i_node.AddChild(items[i].Name, items[i].Amount);
        }

        return node;
    }

    public void CreateDefault() {
        throw new System.NotImplementedException();
    }
}

/*
using System;
using UnityEngine;
using System.Collections.Generic;
using Gamelogic;

public class SpiritTypeAttribute : PropertyAttribute {

}

[Serializable] public class AttributeDict : Dictionary<string, float> {}

[Serializable]
public class Spirit {

    [SpiritType] public SpiritType type;
    public string name = ""; // nickname
    public int level;

    [HideInInspector] public float currentHP = 0f;
    private float currentStamina = 0f;

    public float CurrentStamina {
        get { return currentStamina;}
        set { currentStamina = Mathf.Clamp(value, 0, 100); }
    }

    [HideInInspector] public float XP = 0f;

    [SerializeField] public AttributeDict Attributes;

}

[System.Serializable]
public class SpiritType {
    public string name = "";
    public ElementType type;
    public AttributeDict Attributes = new AttributeDict();

    public static Spirit GenerateSpirit(string s, int level) {
        return GenerateSpirit(SpiritTypes.Instance.spirits[s], level);
    }

    public static Spirit GenerateSpirit(SpiritType t, int level) {
        var spirit = new Spirit();
        spirit.Attributes = new AttributeDict();
        spirit.level = level;
        spirit.name = t.name;
        spirit.type = t;
        foreach(var attr in t.Attributes) {
            bool b = attr.Key.Contains("Level");
            var Key = b ? attr.Key.Substring(5) : attr.Key;
            float Val = b ? attr.Value * level : attr.Value;

            if (spirit.Attributes.ContainsKey(attr.Key))
                spirit.Attributes[Key] = spirit.Attributes[Key] + Val;
            else
                spirit.Attributes.Add(attr.Key, attr.Value);
            
        }
        spirit.currentHP = spirit.Attributes["HP"]; // make it full life
        return spirit;
    }
}

public struct ElementType {
    public string name { get; }

    public ElementType(string n) {
        name = n;
    }
}

public class SpiritTypes : Singleton<SpiritTypes> {

    public TextAsset csvFile;
    public Dictionary<string, SpiritType> spirits;
    public Dictionary<string, ElementType> elements;

    // Use this for initialization
    public void Start() {
        LoadFromCSV(csvFile, out spirits, out elements);

        // DEBUG: PRINT ALL SPIRITS
        foreach(var kv in spirits) {
            var s = kv.Value;
            //Debug.Log($"{s.name}, {s.type.name}, { string.Join(", ", s.Attributes.Select(k => $"[{k.Key}:{k.Value}]").ToArray()) }");
        }

    }

    public static void LoadFromCSV(TextAsset file, out Dictionary<string, SpiritType> spirits, out Dictionary<string, ElementType> elements) {
        spirits = new Dictionary<string, SpiritType>();
        elements = new Dictionary<string, ElementType>();

        string[,] grid = CSVReader.SplitCsvGrid(file.text);

        for (int y = 1; y < grid.GetUpperBound(1); y++) {
            if (grid[1, y] == "") continue;
            SpiritType t = new SpiritType();
            if (!elements.ContainsKey(grid[2, y]))
                elements.Add(grid[2, y], new ElementType(grid[2, y]));
            t.type = elements[grid[2, y]];
            t.name = grid[1, y];

            for (int x = 3; x < grid.GetUpperBound(0); x++) {
                if (grid[x, 0] != "")
                    t.Attributes.Add(grid[x, 0], float.Parse(grid[x, y]));
            }
            spirits.Add(t.name, t);
        }
    }
}
*/

using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {
    [Serializable]
    [CreateAssetMenu(menuName = "Spirit/Library")]
    public class SpiritLibrary : ScriptableObject {
        public SpiritType[] Spirits;

#if UNITY_EDITOR
        public void LoadFromCSV(TextAsset file) {
            List<SpiritType> csvLib = new List<SpiritType>();

            string[,] grid = CSVReader.SplitCsvGrid(file.text);

            HashSet<string> fieldnotfound = new HashSet<string>();

            for(int y = 1; y < grid.GetUpperBound(1); y++) {
                if(grid[1, y] == "") continue;
                //SpiritType t = new SpiritType();
                SpiritType t = ScriptableObject.CreateInstance<SpiritType>();


                Type type = typeof(SpiritType);


                //if (!elements.ContainsKey(grid[2, y]))
                //elements.Add(grid[2, y], new ElementType(grid[2, y]));
                //t.type = elements[grid[2, y]];
                t.Name = grid[1, y];

                for(int x = 3; x < grid.GetUpperBound(0); x++) {
                    if(grid[x, 0] != "") {
                        FieldInfo field = type.GetField(grid[x, 0]);
                        if(field == null) {
                            fieldnotfound.Add(grid[x, 0]);
                            continue;
                        }
                        if(field.FieldType == typeof(int)) {
                            field.SetValue(t, int.Parse(grid[x, y]));
                        } else if(field.FieldType == typeof(float)) {
                            field.SetValue(t, float.Parse(grid[x, y]));
                        } else if(field.FieldType == typeof(bool)) {
                            field.SetValue(t, bool.Parse(grid[x, y]));
                        } else {
                            field.SetValue(t, grid[x, y]);
                        } // string
                    }
                    //t.Attributes.Add(, float.Parse());
                }
                csvLib.Add(t);
            }
            Spirits = csvLib.ToArray();

            foreach(var t in Spirits) {
                t.name = t.Name; // dont confuse SpiritType 'Name' with Scriptableobject 'Name' ... 

                AssetDatabase.AddObjectToAsset(t, this);
                t.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
            }

            foreach(var v in fieldnotfound) {
                Debug.Log("field " + v + " not found");
            }
        }
#endif
    }
}

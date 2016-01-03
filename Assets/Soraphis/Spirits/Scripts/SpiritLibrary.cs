using System;
using System.Collections.Generic;
using Assets.Soraphis.Lib;
using UnityEngine;

namespace Assets.Soraphis.Spirits.Scripts {
    [Serializable][CreateAssetMenu(menuName = "Spirit/Library")]
    public class SpiritLibrary : ScriptableObject {

        public SpiritType[] Spirits;

        public void LoadFromCSV(TextAsset file) {
            List<SpiritType> csvLib = new List<SpiritType>();

            string[,] grid = CSVReader.SplitCsvGrid(file.text);

            for (int y = 1; y < grid.GetUpperBound(1); y++) {
                if (grid[1, y] == "") continue;
                SpiritType t = new SpiritType();
                t.Attributes = new AttributeDict();

                //if (!elements.ContainsKey(grid[2, y]))
                //elements.Add(grid[2, y], new ElementType(grid[2, y]));
                //t.type = elements[grid[2, y]];
                t.Name = grid[1, y];

                for (int x = 3; x < grid.GetUpperBound(0); x++) {
                    if (grid[x, 0] != "")
                        t.Attributes.Add(grid[x, 0], float.Parse(grid[x, y]));
                }
                csvLib.Add(t);
            }
            Spirits = csvLib.ToArray();
        }
    }
}

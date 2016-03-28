using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;

namespace Tiled2Unity
{
    interface ICustomTiledImporter
    {
        // A game object within the prefab has some custom properites assigned through Tiled that are not consumed by Tiled2Unity
        // This callback gives customized importers a chance to react to such properites.
        void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties);

        // Called just before the prefab is saved to the asset database
        // A last chance opporunity to modify it through script
        void CustomizePrefab(GameObject prefab);
    }
}

// Examples
[Tiled2Unity.CustomTiledImporter]
class CustomImporterAddComponent : Tiled2Unity.ICustomTiledImporter
{
    public static Type[] getTypeByName(string className) {
        List<Type> returnVal = new List<Type>();

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
            Type[] assemblyTypes = a.GetTypes();
            for (int j = 0; j < assemblyTypes.Length; j++) {
                if (assemblyTypes[j].Name == className) {
                    returnVal.Add(assemblyTypes[j]);
                }
            }
        }

        return returnVal.ToArray();
    }

    public void HandleCustomProperties(UnityEngine.GameObject gameObject,
        IDictionary<string, string> props)
    {
        // Simply add a component to our GameObject
        if (props.ContainsKey("AddComp")) {

            Type[] t = getTypeByName(props["AddComp"]);
            if(t.Length == 1) gameObject.AddComponent(t[0]);
            else Debug.LogWarning("Component \"" + props["AddComp"] + "\" could not be identified. Found " + t.Length + " expected 1.");
        }
    }


    public void CustomizePrefab(GameObject prefab)
    {
        // Do nothing
    }
}

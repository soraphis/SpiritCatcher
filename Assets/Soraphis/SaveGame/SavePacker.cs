using System;
using Gamelogic;
using UnityEngine;

namespace Assets.Soraphis.SaveGame {
    [DisallowMultipleComponent]
    public sealed class SavePacker : MonoBehaviour, Saveable {

        public string SKeyName = "Change Me!";

        public void Load(DataNode parent) {
            DataNode pack = parent.GetChild(SKeyName);
            foreach(var component in this.GetComponents<MonoBehaviour>()) {
                if(!(component is Saveable) || component == this) continue;
                if(pack == null) {
                    try {   (component as Saveable).CreateDefault();
                    }catch(NotImplementedException) {}
                } else {
                    (component as Saveable).Load(pack);
                }
            }
            foreach (var child in transform.GetChildren()) {
                child.GetComponent<SavePacker>()?.Load(pack);
            }
        }

        public DataNode Save() {
            DataNode pack = new DataNode();
            pack.Name = SKeyName;
            foreach (var component in this.GetComponents<MonoBehaviour>()) {
                if (component is Saveable && component != this)
                    pack.AddChild((component as Saveable).Save());
            }

            foreach (var child in transform.GetChildren()) {
                var childpacker = child.GetComponent<SavePacker>();
                if (childpacker != null)
                    pack.AddChild(childpacker.Save());
            }

            return pack;
        }

        public void CreateDefault() {}
    }
}

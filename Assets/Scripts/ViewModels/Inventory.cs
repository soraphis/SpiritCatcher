using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ViewModels {
    public class Inventory : UIView{
        public ObservableList<Player.InventoryElement> Items = new ObservableList<Player.InventoryElement>();
        public MarkLight.Views.UI.List ItemList;

        public bool hasBackButton = false;
        public int test = 7 ;

        public override void Initialize() {
            base.Initialize();

            Items = new ObservableList<Player.InventoryElement>();
        }

        void OnEnable() {
            Items.Clear();
            Items.AddRange(Player.Instance.Items.GetElements());
            Player.Instance.Items.OnChange += UpdateListOnChange;
        }

        void OnDisable() {
            Player.Instance.Items.OnChange -= UpdateListOnChange;
        }

        void UpdateListOnChange() {
            Items.Clear();
            Items.AddRange(Player.Instance.Items.GetElements());
        }

        public void ItemClicked(BaseEventData e) {
            if (!(e is PointerEventData)) return;
            var a = e as PointerEventData;


            var index = a.pointerEnter.transform.parent.GetSiblingIndex() - 1; // minus 1 because template object
            var item = Items[index];

            var itemtype = ItemLibrary.Lookup(item.Name);

            if(OverlayView.Instance.isActive) {
                OverlayView.Instance.Value = index;
                return;
            }
            

            if ((Game.Instance.CurrentGameState & Game.GameState.Battle) != 0)
                if(itemtype.UsableInFight) {
                    UnityEngine.Debug.Log("use item");
                    itemtype.OnUse.Invoke(() => {});
                }
            if ((Game.Instance.CurrentGameState & Game.GameState.World) != 0)
                if(itemtype.UsableInWorld) {
                    UnityEngine.Debug.Log("use item");
                    itemtype.OnUse.Invoke(() => { });
                }
        }

    }
}

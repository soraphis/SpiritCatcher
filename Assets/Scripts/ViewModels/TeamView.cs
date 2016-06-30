using System.Linq;
using Assets.Soraphis.Spirits.Scripts;
using MarkLight;
using MarkLight.Views.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.ViewModels {
    class TeamView : UIView {

        public bool hasBackButton = false;
        public MarkLight.Views.UI.List TeamList;
        public ObservableList<Spirit> Spirits = new ObservableList<Spirit>();

        void OnEnable() {
            Spirits.Clear();
            Spirits.AddRange(Player.Instance.team.ToList());
        }

        public void ItemClicked(BaseEventData e) {
            if (!(e is PointerEventData)) return;
            var a = e as PointerEventData;

            if (OverlayView.Instance.isActive) {
                var index = a.pointerEnter.transform.parent.GetSiblingIndex() - 1; // minus 1 because template object
                OverlayView.Instance.Value = index;
                return;
            }
        }

        public float getPercentHP(float current, float points) {
            return current/(SpiritType.PP_HP*points);
        }

        public void CloseOverlay() {
            OverlayView.Instance.CancelOverlay();
        }
    }
}

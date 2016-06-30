using System;
using System.Collections;
using MarkLight.Views.UI;
using UnityEngine;

namespace Assets.Scripts.ViewModels {
    class OverlayView : UIView {

        public enum Overlay{ ItemView = 1, TeamView = 2 }

        public static OverlayView Instance = null;

        private int? value = null;
        public bool isActive { private set; get; }

        public ViewSwitcher ContentViewSwitcher;

        public void Start() {
            if (Instance != null) 
                if(Instance.gameObject != null) throw new Exception();
            Instance = this;
        }

        public void ShowOverlay(Overlay which, Action<int> onResolve) {
            if(isActive) {
                throw new ArgumentException("there's already an overlay active");
            }
            this.transform.SetAsLastSibling();
            value = null;
            isActive = true;
            ContentViewSwitcher.SwitchTo((int)which);
            StartCoroutine(WaitForResolve(onResolve));
        }

        private IEnumerator WaitForResolve(Action<int> onResolve) {
            yield return new WaitUntil(() => value != null);
            ContentViewSwitcher.SwitchTo(0);
            isActive = false;
            var val = (int) value;
            onResolve.Invoke(val);
        }

        public void CancelOverlay() {
            if (! isActive) return;
            isActive = false;
            ContentViewSwitcher.SwitchTo(0);
            StopAllCoroutines();
        }

        public int? Value {
            set { if(isActive) this.value = value; }
            get { return value;  }
        }

    }
}

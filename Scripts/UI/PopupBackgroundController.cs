using System;
using Event;
using UnityEngine;

namespace UI {
    public class PopupBackgroundController : MonoBehaviour {

        public static event EventHandler<OutsideClickEventArgs> OutsideClickEvent;

        public void OnClickOutside() => OutsideClickEventEmit();
        
        private void OutsideClickEventEmit() {
            if (OutsideClickEvent == null) return;
            foreach (var invocation in OutsideClickEvent.GetInvocationList())
                invocation.DynamicInvoke(this, new OutsideClickEventArgs());
        }
    }
}
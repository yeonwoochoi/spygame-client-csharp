using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util
{
    public class SafeArea: MonoBehaviour
    {
        public static event EventHandler<EventArgs> SafeAreaFittedEvent; 
        private RectTransform panel;
        private Rect lastSafeArea = new Rect(0, 0, 0,0);

        private void Awake() {
            panel = GetComponent<RectTransform>();
        }

        private void Update() {
            Refresh();
        }

        private void Refresh() {
            var safeArea = Screen.safeArea;
            if (safeArea != lastSafeArea) ApplySafeArea(safeArea);
        }
        
        private void ApplySafeArea(Rect area) {
            lastSafeArea = area;

            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
            
            EmitSafeAreaFitted();
        }
        
        private void EmitSafeAreaFitted() {
            if (SafeAreaFittedEvent == null) return;
            foreach (var invocation in SafeAreaFittedEvent.GetInvocationList())
                invocation.DynamicInvoke(this, EventArgs.Empty);
        }
    }
}
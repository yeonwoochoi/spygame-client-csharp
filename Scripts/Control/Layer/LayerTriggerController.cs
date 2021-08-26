using System;
using Control.Movement;
using UnityEngine;

namespace Control.Layer
{
    public enum LayerType
    {
        Layer1, Layer2, Layer3
    }

    public static class LayerTypeUtils
    {
        private static readonly string layer1Name = "Layer1";
        private static readonly string layer2Name = "Layer2";
        private static readonly string layer3Name = "Layer3";
        
        public static string LayerTypeToString(this LayerType type)
        {
            switch (type)
            {
                case LayerType.Layer1:
                    return layer1Name;
                case LayerType.Layer2:
                    return layer2Name;
                case LayerType.Layer3:
                    return layer3Name;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }    
    }
    
    public class LayerTriggerController: MonoBehaviour
    {
        public LayerType layer;
        public LayerType sortingLayer;

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<PlayerMoveController>() == null) return;
            other.gameObject.layer = LayerMask.NameToLayer(layer.LayerTypeToString());

            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer.LayerTypeToString();
            var spriteRenderers = other.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
            {
                sr.sortingLayerName = sortingLayer.LayerTypeToString();
            }
        }
    }
}
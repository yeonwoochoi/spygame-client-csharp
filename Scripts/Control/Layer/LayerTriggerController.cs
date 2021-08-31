using System;
using Control.Movement;
using UnityEngine;

namespace Control.Layer
{
    #region Enum

    public enum LayerType
    {
        Layer1, Layer2, Layer3
    }

    #endregion

    public static class LayerTypeUtils
    {
        #region Static Variables

        private const string Layer1Name = "Layer1";
        private const string Layer2Name = "Layer2";
        private const string Layer3Name = "Layer3";

        #endregion

        #region Static Method

        public static string LayerTypeToString(this LayerType type)
        {
            return type switch
            {
                LayerType.Layer1 => Layer1Name,
                LayerType.Layer2 => Layer2Name,
                LayerType.Layer3 => Layer3Name,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        #endregion
    }
    
    public class LayerTriggerController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private LayerType layer;
        [SerializeField] private LayerType sortingLayer;

        #endregion

        #region Event Method

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<PlayerMoveController>() == null) return;
            other.gameObject.layer = LayerMask.NameToLayer(layer.LayerTypeToString());

            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer.LayerTypeToString();
            var spriteRenderers = other.gameObject.GetComponentsInChildren<SpriteRenderer>(); // TODO(Player Spy에 붙여줘 child object 모아서 넘겨주는거)
            foreach (var sr in spriteRenderers)
            {
                sr.sortingLayerName = sortingLayer.LayerTypeToString();
            }
        }

        #endregion
    }
}
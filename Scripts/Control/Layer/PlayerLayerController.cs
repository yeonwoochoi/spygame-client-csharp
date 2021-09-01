using UnityEngine;

namespace Control.Layer
{
    public class PlayerLayerController: MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        public void ChangeLayer(LayerType layer, LayerType sortingLayer)
        {
            gameObject.layer = LayerMask.NameToLayer(layer.LayerTypeToString());
            foreach (var sr in spriteRenderers)
            {
                sr.sortingLayerName = sortingLayer.LayerTypeToString();
            }
        }
    }
}
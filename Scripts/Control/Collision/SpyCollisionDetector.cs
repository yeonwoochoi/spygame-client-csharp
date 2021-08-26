using Control.Movement;
using UnityEngine;

namespace Control.Collision
{
    public class SpyCollisionDetector: MonoBehaviour
    {
        [SerializeField] private SpyMoveController spyMoveController;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!spyMoveController.IsSet) return;
            if (other.gameObject.transform.IsChildOf(transform.parent.transform)) return;
            if (other.gameObject.tag == "Detector" || other.gameObject.layer == LayerMask.NameToLayer("UI")) return;
            spyMoveController.StopWandering();
        }
    }
}
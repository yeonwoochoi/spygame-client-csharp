using UnityEngine;

namespace Control.Collision
{
    public class CharacterCollisionBlocker: MonoBehaviour
    {
        [SerializeField] private BoxCollider2D characterCollider2D;
        [SerializeField] private BoxCollider2D characterBlockerCollider2D;

        private void Start()
        {
            Physics2D.IgnoreCollision(characterCollider2D, characterBlockerCollider2D, true);
        }
    }
}
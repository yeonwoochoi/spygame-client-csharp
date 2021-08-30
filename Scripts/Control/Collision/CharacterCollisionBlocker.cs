using UnityEngine;

namespace Control.Collision
{
    public class CharacterCollisionBlocker: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private BoxCollider2D characterCollider2D;
        [SerializeField] private BoxCollider2D characterBlockerCollider2D;

        #endregion

        #region Event Method

        private void Start()
        {
            Physics2D.IgnoreCollision(characterCollider2D, characterBlockerCollider2D, true);
        }

        #endregion
    }
}
using System.Collections;
using StageScripts;
using UnityEngine;

namespace Camera
{
    public class CameraFollowController : MonoBehaviour
    {
        #region Private Variables
        private Transform target;
        private float lerpSpeed = 2.5f;
        private Vector2 offset;
        private Vector2 targetPos;
        #endregion

        #region Public Method
        public void SetOffset(Transform targetTransform)
        {
            target = targetTransform;
            offset = targetTransform.position - target.position;
            StartCoroutine(FollowPlayer());
        }

        #endregion

        #region Private Method

        private IEnumerator FollowPlayer()
        {
            if (target == null) yield break;
            while (true)
            {
                yield return null;
                targetPos = (Vector2) target.position + offset;
                var distance = ((Vector2) transform.position - targetPos).sqrMagnitude;
                if (distance < 0.0001f) continue;
                var pos = Vector2.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
                transform.position = new Vector3(pos.x, pos.y, -10);
            }
        }

        #endregion
    }
}
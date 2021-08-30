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
        private Vector3 offset;
        private Vector3 targetPos;
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
                targetPos = target.position + offset;
                if (transform.position == targetPos) continue;
                transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
                if (transform.position.z > -1)
                {
                    transform.position += new Vector3(0, 0, -10);
                }
            }
        }

        #endregion
    }
}
using StageScripts;
using UnityEngine;

namespace Camera
{
    public class CameraFollowController : MonoBehaviour
    {
        private Transform target;
        
        private float lerpSpeed = 2.5f;
        private Vector3 offset;
        private Vector3 targetPos;

        public void SetOffset(Transform targetTransform)
        {
            target = targetTransform;
            offset = targetTransform.position - target.position;
        }

        private void Update()
        {
            if (target == null) return;
            targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
            if (transform.position.z > -1)
            {
                transform.position += new Vector3(0, 0, -10);
            }
        }
    }
}
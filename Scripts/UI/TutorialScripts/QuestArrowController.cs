using System;
using UnityEngine;
using Util;

namespace UI.TutorialScripts
{
    public class QuestArrowController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Transform targetTransform;
        private UnityEngine.Camera camera;
        private RectTransform pointerRectTransform;
        private Vector3 targetPosition;
        private CanvasGroup cGroup;
        private bool isSet = false;

        #endregion

        #region Setter

        public void Init(UnityEngine.Camera camera)
        {
            this.camera = camera;
            targetPosition = targetTransform.position;
            pointerRectTransform = GetComponent<RectTransform>();
            cGroup = GetComponent<CanvasGroup>();
            cGroup.Visible(false);
            isSet = true;
        }

        #endregion

        private void Update()
        {
            if (!isSet) return;
            var toPosition = (Vector2) targetPosition;
            var fromPosition = (Vector2) camera.transform.position;

            var dir = (toPosition - fromPosition).normalized;
            var angle = GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        private float GetAngleFromVectorFloat(Vector2 dir)
        {
            var eulerAngle = Mathf.Atan2(dir.y, dir.x);
            Debug.Log(eulerAngle + 90);
            return eulerAngle + 90;
        }
    }
}
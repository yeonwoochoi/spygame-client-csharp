using System;
using UnityEngine;
using Util;

namespace UI.TutorialScripts
{
    public class PointerUIController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Animator animator;
        private RectTransform pointerRect;
        private RectTransform targetRect;
        private CanvasGroup cGroup;

        private bool isSet = false;
        private bool isPointing = false;

        #endregion

        #region Const & Static Variables

        private static readonly int AnimatorParamIDIsPointing = Animator.StringToHash(AnimatorParamIsPointing);
        private const string AnimatorParamIsPointing = "IsPointing";
        
        private static readonly float offset = 150f;

        #endregion

        #region Setter

        private void IsPointing(bool flag)
        {
            isPointing = flag;
            cGroup.Visible(flag);
            animator.SetBool(AnimatorParamIDIsPointing, true);
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            pointerRect = GetComponent<RectTransform>();
            cGroup = GetComponent<CanvasGroup>();
            
            IsPointing(false);
            isSet = true;
        }

        public void StartPointing(RectTransform target)
        {
            if (!isSet) return;
            if (isPointing) return;

            targetRect = target;
            SetPointerPosition();
            IsPointing(true);
        }

        public void EndPointing()
        {
            if (!isSet) return;
            IsPointing(false);
        }

        #endregion

        #region Private Methods

        private void SetPointerPosition()
        {
            pointerRect.transform.SetParent(targetRect.transform);
            pointerRect.localPosition = new Vector3(0, offset, 0);
        }
        
        #endregion
    }
}
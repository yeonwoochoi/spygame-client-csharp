using System;
using System.Collections;
using Event;
using UnityEngine;
using UnityEngine.UI;

namespace Base
{
    public abstract class BaseLoadingController: MonoBehaviour
    {
        #region Protected Variables

        [SerializeField] protected Image loadingBar;
        [SerializeField] protected CanvasGroup cGroup;

        protected string nextScene = "";

        #endregion

        #region Event

        public static event EventHandler<AlertOccurredEventArgs> AlertOccurredEvent; 

        #endregion

        #region Event Method

        protected virtual void Start()
        {
            loadingBar.fillAmount = 0;
            HandleLoading();
        }

        #endregion

        #region Protected Methods

        protected abstract void HandleLoading();

        protected IEnumerator Fade(bool isFadeIn)
        {
            var timer = 0f;

            while (timer <= 1f)
            {
                yield return null;
                timer += Time.unscaledDeltaTime * 2f;
                cGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
            }
        }
        
        protected void EmitAlertOccurredEvent(AlertOccurredEventArgs e) {
            if (AlertOccurredEvent == null) return;
            foreach (var invocation in AlertOccurredEvent.GetInvocationList())
                invocation?.DynamicInvoke(this, e);
        }

        #endregion
    }
}
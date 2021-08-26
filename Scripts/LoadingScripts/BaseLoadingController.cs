using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace LoadingScripts
{
    public abstract class BaseLoadingController: MonoBehaviour
    {
        [SerializeField] protected Image loadingBar;
        [SerializeField] protected CanvasGroup cGroup;
        
        protected string nextScene = "";

        protected void Start()
        {
            loadingBar.fillAmount = 0;
            HandleLoading();
        }
        
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
    }
}
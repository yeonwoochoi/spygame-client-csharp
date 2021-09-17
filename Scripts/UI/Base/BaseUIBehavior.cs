using System;
using System.Collections;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Base
{
    public abstract class BaseUIBehavior: MonoBehaviour
    {
        #region Protected Variables

        [SerializeField] protected CanvasGroup cGroup;
        [SerializeField] protected CanvasGroup bgCanvasGroup;
        protected bool isOpen;

        #endregion

        #region Event Methods

        protected virtual void Start()
        {
            isOpen = false;
        }
        
        protected virtual void OnDisable() {}

        #endregion


        #region Protected Methods

        protected void ActivateUI(bool flag = true)
        {
            cGroup.Visible(flag);
            bgCanvasGroup.Visible(flag);
        }

        protected IEnumerator DoFade(CanvasGroup target, bool isFadeIn = true)
        {
            if (isFadeIn)
            {
                var secondsToFade = 1.5f;
                var startValue = 0;
                var rate = 1.0f / secondsToFade;
 
                for (var x = 0.0f; x <= 1.0f; x += Time.deltaTime * rate) {
                    target.alpha = Mathf.Lerp(startValue, 1, x);
                    yield return null;
                }
                target.Visible();
            }
            else
            {
                var secondsToFade = 1.5f;
                var startValue = 1;
                var rate = 1.0f / secondsToFade;
 
                for (var x = 1.0f; x >= 0.0f; x -= Time.deltaTime * rate) {
                    target.alpha = Mathf.Lerp(startValue, 1, x);
                    yield return null;
                }
                target.Visible(false);
            }
            
        }
        
        // TODO : 없애기 상의후
        protected IEnumerator TypingComment(Text target, string comment)
        {
            yield return new WaitForSeconds(1);
            target.text = "";
            foreach (var letter in comment.ToCharArray())
            {
                target.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
        }
        
        #endregion
    }
}
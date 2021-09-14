using System.Collections;
using System.Collections.Generic;
using Control.Pointer;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.TutorialScripts
{
    public class TutorialNpcTalkingBehavior: BaseUIBehavior
    {
        #region Private Variables

        [SerializeField] private CanvasGroup npcCanvasGroup;
        [SerializeField] private Text commentText;
        
        private bool isTalking = false;

        #endregion

        #region Getter

        public bool IsTalking()
        {
            return isTalking;
        }

        #endregion

        #region Public Methods

        public void StartTalking(List<string> comments)
        {
            if (isTalking) return;
            isTalking = true;
            StartCoroutine(StartNpcTalking(comments));
        }

        #endregion

        #region Private Method

        private void EndTalking()
        {
            if (!isTalking) return;
            commentText.text = "";
            ActivateUI(false);
            isTalking = false;
        }

        private IEnumerator StartNpcTalking(List<string> comments)
        {
            ActivateUI();
            yield return StartCoroutine(DoFade(npcCanvasGroup));

            var loopCount = comments.Count;

            while (loopCount > 0)
            {
                var isClicking = false;
                yield return TypingComment(commentText, comments[comments.Count - loopCount]);
                while (!isClicking)
                {
                    if (Input.GetMouseButtonDown(0)) isClicking = true;
                    yield return null;
                }
                loopCount--;
                yield return null;
            }
            
            EndTalking();
        }

        #endregion
    }
}
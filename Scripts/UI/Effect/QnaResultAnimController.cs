using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UI.Popup.Qna;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Effect
{
    public class QnaResultAnimController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Image correctImage;
        [SerializeField] private List<Image> wrongImages;
        private bool isRun = false;
        
        #endregion

        #region Public Methods

        public void PlayQnaResultAnim(bool isCorrect)
        {
            if (isRun) return;
            isRun = true;
            StartCoroutine(isCorrect ? PlayCorrectAnim() : PlayWrongAnim());
        }

        public void Reset()
        {
            correctImage.fillAmount = 0;
            foreach (var wrongImage in wrongImages)
            {
                wrongImage.fillAmount = 0;
            }
        }

        #endregion

        #region Private Method

        private IEnumerator PlayCorrectAnim()
        {
            yield return StartCoroutine(PlayQnaResultAnim(correctImage));
            yield return new WaitForSeconds(1f);
            isRun = false;
        }

        private IEnumerator PlayWrongAnim()
        {
            yield return StartCoroutine(PlayQnaResultAnim(wrongImages[0]));
            yield return StartCoroutine(PlayQnaResultAnim(wrongImages[1]));
            yield return new WaitForSeconds(1f);
            isRun = false;
        }

        private IEnumerator PlayQnaResultAnim(Image target)
        {
            var goal = 1f;
            var speed = 5f;
            for (var f = 0f; f <= goal; f += Time.deltaTime * speed)
            {
                target.fillAmount = f;
                yield return null;
            }
            
            target.fillAmount = goal;
            yield return null;
        }

        #endregion
    }
}
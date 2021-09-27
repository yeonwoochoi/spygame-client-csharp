using System;
using System.Collections;
using System.Collections.Generic;
using Event;
using UI.Popup.Qna;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Effect
{
    public class QnaResultAnimController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Image correctImage;
        [SerializeField] private List<Image> wrongImages;
        [SerializeField] private bool isSpyQna = true;
        
        private bool isRun;

        private Action<bool> gradingCallback;

        #endregion

        #region Event Methods

        private void Start()
        {
            isRun = false;
            if (isSpyQna)
            {
                SpyQnaPopupBehavior.PlayQnaGradingAnimEvent += PlayQnaGrading;   
            }
            else
            {
                ItemQnaPopupBehavior.PlayQnaGradingAnimEvent += PlayQnaGrading;   
            }
        }

        private void OnDisable()
        {
            if (isSpyQna)
            {
                SpyQnaPopupBehavior.PlayQnaGradingAnimEvent -= PlayQnaGrading;   
            }
            else
            {
                ItemQnaPopupBehavior.PlayQnaGradingAnimEvent -= PlayQnaGrading;   
            }
        }

        #endregion

        private void PlayQnaGrading(object _, PlayQnaGradingAnimEventArgs e)
        {
            if (isRun) return;
            isRun = true;
            gradingCallback = e.callback;
            StartCoroutine(e.isCorrect ? PlayCorrectAnim(e.isClickCorrectBtn) : PlayWrongAnim(e.isClickCorrectBtn));        
        }
        
        private void Reset()
        {
            gradingCallback = null;
            correctImage.fillAmount = 0;
            foreach (var wrongImage in wrongImages)
            {
                wrongImage.fillAmount = 0;
            }
            isRun = false;
        }
        
        #region Private Method

        private IEnumerator PlayCorrectAnim(bool isClickCorrectBtn)
        {
            yield return StartCoroutine(PlayQnaResultAnim(correctImage));
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Correct!");
            gradingCallback?.Invoke(isClickCorrectBtn);
            Reset();
        }

        private IEnumerator PlayWrongAnim(bool isClickCorrectBtn)
        {
            yield return StartCoroutine(PlayQnaResultAnim(wrongImages[0]));
            yield return StartCoroutine(PlayQnaResultAnim(wrongImages[1]));
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Wrong!");
            gradingCallback?.Invoke(isClickCorrectBtn);
            Reset();
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
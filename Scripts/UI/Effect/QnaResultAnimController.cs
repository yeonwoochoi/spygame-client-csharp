using System.Collections;
using System.Collections.Generic;
using Event;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Effect
{
    public class QnaResultAnimController: MonoBehaviour
    {
        #region Delegate
        public delegate void GradingCallback(bool isClickCorrectBtn);

        #endregion
        
        #region Private Variables

        [SerializeField] private Image correctImage;
        [SerializeField] private List<Image> wrongImages;
        
        private IEnumerator correctAnimEnumerator = null;
        private IEnumerator wrongAnimEnumerator1 = null;
        private IEnumerator wrongAnimEnumerator2 = null;
        
        private readonly float popupCloseDelay = 0.5f;
        private readonly float speed = 4f;

        private bool isRun;
        private GradingCallback gradingCallback;
        
        #endregion

        #region Event Methods

        private void Start()
        {
            Reset();
        }

        #endregion

        #region Public Method

        public void Play(bool isCorrect, bool isClickCorrectBtn, GradingCallback callback)
        {
            if (isRun) return;
            isRun = true;
            gradingCallback = callback;
            StartCoroutine(isCorrect ? PlayCorrectAnim(isClickCorrectBtn) : PlayWrongAnim(isClickCorrectBtn));        
        }

        #endregion


        #region Private Method

        private void Reset()
        {
            if (correctAnimEnumerator != null) StopCoroutine(correctAnimEnumerator);
            if (wrongAnimEnumerator1 != null) StopCoroutine(wrongAnimEnumerator1);
            if (wrongAnimEnumerator2 != null) StopCoroutine(wrongAnimEnumerator2);
            correctAnimEnumerator = null;
            wrongAnimEnumerator1 = null;
            wrongAnimEnumerator2 = null;
            
            gradingCallback = null;
            correctImage.fillAmount = 0;
            foreach (var wrongImage in wrongImages)
            {
                wrongImage.fillAmount = 0;
            }
            isRun = false;
        }


        private IEnumerator PlayCorrectAnim(bool isClickCorrectBtn)
        {
            wrongAnimEnumerator2 = PlayQnaResultAnim(correctImage);
            yield return StartCoroutine(wrongAnimEnumerator2);
            yield return new WaitForSeconds(popupCloseDelay);
            gradingCallback(isClickCorrectBtn);
            Reset();
        }

        private IEnumerator PlayWrongAnim(bool isClickCorrectBtn)
        {
            correctAnimEnumerator = PlayQnaResultAnim(wrongImages[0]);
            wrongAnimEnumerator1 = PlayQnaResultAnim(wrongImages[1]);
            yield return StartCoroutine(correctAnimEnumerator);
            yield return StartCoroutine(wrongAnimEnumerator1);
 
            yield return new WaitForSeconds(popupCloseDelay);
            gradingCallback(isClickCorrectBtn);
            Reset();
        }

        private IEnumerator PlayQnaResultAnim(Image target)
        {
            var start = 0f;
            var goal = 1f;
            
            while (target.fillAmount < goal)
            {
                start += Time.deltaTime * speed;
                target.fillAmount = start;
                yield return null;
            }
            
            target.fillAmount = goal;
            yield return null;
        }

        #endregion
    }
}
using System;
using System.Collections;
using Base;
using Event;
using MainScripts;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LoadingScripts
{
    public class NormalLoadingController: BaseLoadingController
    {
        #region Private Variable

        [SerializeField] private Text progressText;

        #endregion

        #region Protected Method

        protected override void HandleLoading()
        {
            if (LoadingManager.Instance.CurrentType == MainSceneType.Main)
            {
                nextScene = SceneNameManager.SceneChapter;    
            }

            if (LoadingManager.Instance.CurrentType == MainSceneType.Select)
            {
                switch (LoadingManager.Instance.NextType)
                {
                    case MainSceneType.Main:
                        nextScene = SceneNameManager.SceneMain;
                        break;
                    case MainSceneType.Play:
                        nextScene = SceneNameManager.SceneStage;
                        QnaManager.Instance.GetQnaData(LoadingManager.Instance.ChapterType, LoadingManager.Instance.StageType);
                        break;
                }
            }

            if (LoadingManager.Instance.CurrentType == MainSceneType.Play)
            {
                nextScene = SceneNameManager.SceneChapter;
            }

            StartCoroutine(LoadScene(nextScene));
        }

        #endregion

        #region Private Method

        // TODO()
        private IEnumerator LoadScene(string nextScene)
        {
            yield return StartCoroutine(Fade(true));
            loadingBar.fillAmount = 0f;
            var manager = SceneManager.LoadSceneAsync(nextScene);
            manager.allowSceneActivation = false;

            var timer = 0f;
            while (!manager.isDone)
            {
                yield return null;
                
                timer += Time.unscaledDeltaTime;

                if (manager.progress < 0.9f)
                {
                    loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, manager.progress, timer);
                    progressText.text = $"{(int)(loadingBar.fillAmount * 100)}%";

                    if (loadingBar.fillAmount >= manager.progress) timer = 0f;
                }
                else
                {
                    loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timer);
                    progressText.text = $"{(int)(loadingBar.fillAmount * 100)}%";

                    if (loadingBar.fillAmount < 1.0f) continue;

                    yield return StartCoroutine(GetQnaData());

                    manager.allowSceneActivation = true;
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(Fade(false));
                    yield break;
                }
            }
        }

        private IEnumerator GetQnaData()
        {
            var done = false;
            // waiting time is not over 2 minute
            var limitCount = 1200;
                    
            while (!done)
            {
                if (limitCount < 0)
                {
                    // TODO (error) Server data is not loaded.
                    break;
                }
                if (LoadingManager.Instance.nextType == MainSceneType.Play)
                {
                    if (QnaManager.Instance.isLoaded)
                    {
                        LoadingManager.Instance.qna = QnaManager.Instance.response.content;
                        QnaManager.Instance.isLoaded = false;
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }

                limitCount--;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        #endregion
    }
}
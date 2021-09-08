using System;
using System.Collections;
using Base;
using Event;
using Http;
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
        private bool isLoaded = true;

        #endregion

        #region Protected Method

        protected override void HandleLoading()
        {
            LoadingManager.Instance.loadingType = LoadingType.Normal;
            if (LoadingManager.Instance.currentType == MainSceneType.Main)
            {
                nextScene = SceneNameManager.SceneChapter;    
            }

            if (LoadingManager.Instance.currentType == MainSceneType.Select)
            {
                switch (LoadingManager.Instance.nextType)
                {
                    case MainSceneType.Main:
                        nextScene = SceneNameManager.SceneMain;
                        break;
                    case MainSceneType.Play:
                        nextScene = SceneNameManager.SceneStage;
                        isLoaded = false;
                        StartCoroutine(GetQnaData());
                        break;
                }
            }

            if (LoadingManager.Instance.currentType == MainSceneType.Play)
            {
                nextScene = SceneNameManager.SceneChapter;
            }

            StartCoroutine(LoadScene(nextScene));
        }

        #endregion

        #region Private Method

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

                    while (!isLoaded) yield return null;

                    manager.allowSceneActivation = true;
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(Fade(false));
                    yield break;
                }
            }
        }

        private IEnumerator GetQnaData()
        {
            var www = HttpFactory.Build(RequestUrlType.Qna);
            yield return www.SendWebRequest();
            
            NetworkManager.HandleResponse(www, out var response, out var errorResponse);

            if (response == null && errorResponse == null)
            {
                NetworkManager.HandleServerError();
                yield break;
            }

            if (response != null)
            {
                isLoaded = true;
                yield break;
            }

            var code = errorResponse.GetErrorCode();
            NetworkManager.HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("No qna data")
                .Content(code.message)
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
        }
        
        #endregion
    }
}
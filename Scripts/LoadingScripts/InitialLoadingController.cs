using System.Collections;
using Base;
using Event;
using Http;
using Manager;
using Manager.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace LoadingScripts
{
    // TODO (Chapter Info) : chapter info 한번에 받아와야해서 이거 만들어야함..
    public class InitialLoadingController: BaseLoadingController
    {
        #region Private Variable

        [SerializeField] private Text progressText;
        private bool isLoaded = false;
        private bool isTutorial = false;

        #endregion

        #region Event Method

        protected override void Start()
        {
            cGroup.Visible();
            base.Start();
        }

        #endregion

        #region Protected Method

        protected override void HandleLoading()
        {
            LoadingManager.Instance.currentType = MainSceneType.Init;
            LoadingManager.Instance.loadingType = LoadingType.Init;
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
            nextScene = isTutorial ? SceneNameManager.SceneTutorial : SceneNameManager.SceneMain;
            isLoaded = false;
            StartCoroutine(GetStageInfo());
            StartCoroutine(LoadScene(nextScene));
        }

        #endregion

        #region Private Method

        private IEnumerator LoadScene(string nextScene)
        {
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
                    yield return new WaitForSeconds(0.1f);
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

        private IEnumerator GetStageInfo()
        {
            var www = HttpFactory.Build(RequestUrlType.ChapterInfo);
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
                .Title("No Chapter Info")
                .Content(code.message)
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
        }
        
        #endregion
    }
}
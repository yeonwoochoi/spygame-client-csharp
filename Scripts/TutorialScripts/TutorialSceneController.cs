using System.Collections;
using Base;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TutorialScripts
{
    public class TutorialSceneController: BaseSceneController
    {
        [SerializeField] private TutorialStageSpawner tutorialStageSpawner;

        protected override void Start()
        {
            base.Start();
            tutorialStageSpawner.Init();
            StartCoroutine(StartTutorialFlow());
        }

        private IEnumerator StartTutorialFlow()
        {
            yield return null;
            
            // Set tutorial clear in playerPref
            var tutorialManager = GlobalDataManager.Instance.Get<TutorialManager>(GlobalDataKey.TUTORIAL);
            tutorialManager.isClear = true;
            GlobalDataManager.Instance.Set(GlobalDataKey.TUTORIAL, tutorialManager);
            LoadMainScene();
        }

        private void LoadMainScene()
        {
            LoadingManager.Instance.currentType = MainSceneType.Tutorial;
            LoadingManager.Instance.nextType = MainSceneType.Main;
            LoadingManager.Instance.loadingType = LoadingType.Normal;

            StartCoroutine(StartLoadingAnimator(() =>
                {
                    nextScene = SceneNameManager.SceneMain;
                },
                () =>
                {
                    SceneManager.LoadScene(nextScene);
                }));
        }
    }
}
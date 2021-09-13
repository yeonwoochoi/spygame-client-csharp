using System;
using System.Collections;
using Base;
using Domain;
using Event;
using Manager;
using Manager.Data;
using UI.TutorialScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TutorialScripts
{
    public class TutorialSceneController: BaseSceneController
    {
        #region Private Variables

        [SerializeField] private TutorialStageSpawner tutorialStageSpawner;
        [SerializeField] private Transform housePortalTransform;

        private PointerController pointerController;
        private Transform playerTransform;
        private Transform initSpyTransform;
        private Transform initBoxTransform;

        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            tutorialStageSpawner.Init();
            Init();

            TutorialDonePopupController.ExitTutorialEvent += ExitTutorial;
        }

        private void OnDisable()
        {
            TutorialDonePopupController.ExitTutorialEvent -= ExitTutorial;
        }

        #endregion

        #region Private Method

        private void Init()
        {
            pointerController = tutorialStageSpawner.pointerController;
            playerTransform = tutorialStageSpawner.initPlayerTransform;
            initSpyTransform = tutorialStageSpawner.initSpyTransform[0];
            initBoxTransform = tutorialStageSpawner.initBoxTransform[0];
            StartCoroutine(StartTutorialFlow());
        }

        private IEnumerator StartTutorialFlow()
        {
            yield return null;
            // TODO (TUTORIAL) : 세부적인 튜토리얼 flow 는 여기서 짜자..
            
            //questPointerController.StartPointing(playerTransform, initSpyTransform);
        }

        private void ExitTutorial(object _, ExitTutorialEventArgs e)
        {
            // Set tutorial clear in playerPref
            var tutorialManager = TutorialManager.Create();
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


        #endregion
    }
}
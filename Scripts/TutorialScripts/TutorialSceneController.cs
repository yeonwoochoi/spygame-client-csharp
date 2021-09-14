using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Control.Pointer;
using Control.Portal;
using Domain;
using Event;
using Manager;
using Manager.Data;
using UI.Qna;
using UI.TutorialScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace TutorialScripts
{
    public class TutorialSceneController: BaseSceneController
    {
        #region Private Variables
        
        [SerializeField] private TutorialStageSpawner tutorialStageSpawner;
        [SerializeField] private PointerUIController pointerUIController;
        [SerializeField] private Transform housePortalTransform;
        [SerializeField] private TutorialNpcTalkingBehavior npcTalkingBehavior;

        private PointerController pointerController;
        private Transform playerTransform;
        private Transform initSpyTransform;
        private Transform initBoxTransform;

        #endregion

        #region Event

        public static event EventHandler<StartTutorialGameEventArgs> StartTutorialGameEvent; 

        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            tutorialStageSpawner.Init();
            Init();

            TutorialDonePopupController.ExitTutorialEvent += ExitTutorial;
            SpyQnaPopupBehavior.CaptureSpyEvent += EndSpyPointing;
            ItemQnaPopupBehavior.ItemGetEvent += EndBoxPointing;
            PortalController.PortalMoveEvent += EndPortalPointing;
        }

        private void OnDisable()
        {
            TutorialDonePopupController.ExitTutorialEvent -= ExitTutorial;
            SpyQnaPopupBehavior.CaptureSpyEvent -= EndSpyPointing;
            ItemQnaPopupBehavior.ItemGetEvent -= EndBoxPointing;
            PortalController.PortalMoveEvent -= EndPortalPointing;
        }

        #endregion

        #region Private Method

        private void Init()
        {
            playerTransform = tutorialStageSpawner.GetPlayerTransform();
            initSpyTransform = tutorialStageSpawner.GetInitSpyPosition();
            initBoxTransform = tutorialStageSpawner.GetInitBoxPosition();

            pointerController = tutorialStageSpawner.GetPointerController();
            pointerUIController.Init();
            
            StartCoroutine(StartTutorialFlow());
        }

        private IEnumerator StartTutorialFlow()
        {
            yield return null;
            // TODO (TUTORIAL) : 세부적인 튜토리얼 flow 는 여기서 짜자..

            var firstComments = new List<string>
            {
                "군 내부에 스파이가 잠입했다는 정보가 들어왔습니다.",
                "심문을 해서 스파이를 색출해내세요.",
                "질문에 대해 오답을 말하는 병사가 스파이입니다.",
                "앞에 보이는 병사에게 다가가 심문을 해보세요."
            };
            yield return StartCoroutine(StartNpcTalking(firstComments));
            
            pointerController.StartPointing(playerTransform, initSpyTransform);
            while (pointerController.GetIsPointing()) yield return null;

            yield return new WaitForSeconds(1f);
            
            var secondComments = new List<string>
            {
                "잘하셨어요. 다음으로 아이템을 얻는 방법을 알려드릴게요.",
                "상자 근처로 다가가 문제를 맞추면 아이템을 얻을 수 있습니다.",
                "앞에 보이는 상자 근처로 가서 문제를 풀어보세요."
            };
            yield return StartCoroutine(StartNpcTalking(secondComments));
            
            pointerController.StartPointing(playerTransform, initBoxTransform);
            while (pointerController.GetIsPointing()) yield return null;
            
            yield return new WaitForSeconds(1f);

            var thirdComments = new List<string>
            {
                "잘하셨어요. 얻으신 아이템은 우측 버튼을 눌러 사용하실 수 있습니다.",
                "막사 내부에도 스파이가 잠입했나봐요.",
                "화살표를 따라가 막사 내부로 이동합시다."
            };
            yield return StartCoroutine(StartNpcTalking(thirdComments));
            
            pointerController.StartPointing(playerTransform, housePortalTransform);
            while (pointerController.GetIsPointing()) yield return null;

            yield return new WaitForSeconds(1f);

            var fourthComments = new List<string>
            {
                $"제한시간 {TutorialStageSpawner.time}초 내에 스파이 1명을 색출해보세요."
            };
            yield return StartCoroutine(StartNpcTalking(fourthComments));
            
            EmitStartTutorialGameEvent(new StartTutorialGameEventArgs());
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

        private IEnumerator StartNpcTalking(List<string> comments)
        {
            npcTalkingBehavior.StartTalking(comments);
            while (npcTalkingBehavior.IsTalking()) yield return null;
        }

        private void ExitTutorial(object _, ExitTutorialEventArgs e)
        {
            // Set tutorial clear in playerPref
            var tutorialManager = TutorialManager.Create();
            tutorialManager.isClear = true;
            GlobalDataManager.Instance.Set(GlobalDataKey.TUTORIAL, tutorialManager);
            LoadMainScene();
        }

        private void EndSpyPointing(object _, CaptureSpyEventArgs e)
        {
            if (!pointerController.GetIsPointing()) return;
            pointerController.EndPointing();
        }

        private void EndBoxPointing(object _, ItemGetEventArgs e)
        {
            if (!pointerController.GetIsPointing()) return;
            pointerController.EndPointing();
        }

        private void EndPortalPointing(object _, PortalMoveEventArgs e)
        {
            if (!pointerController.GetIsPointing()) return;
            pointerController.EndPointing();
        }

        private void EmitStartTutorialGameEvent(StartTutorialGameEventArgs e)
        {
            if (StartTutorialGameEvent == null) return;
            foreach (var invocation in StartTutorialGameEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }

        #endregion
    }
}
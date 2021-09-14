using System;
using System.Collections;
using Domain.StageObj;
using Event;
using Manager;
using Manager.Data;
using StageScripts;
using TutorialScripts;
using UI.StageScripts.Hud;
using UI.StageScripts.Popup;
using UI.TutorialScripts;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Timer
{
    public class TimerHudController: MonoBehaviour
    {
        #region Public Variable

        private int time;

        #endregion

        #region Private Variables
        
        [SerializeField] private Text timerText;

        private CanvasGroup cGroup;
        
        private bool isSet = false;
        private bool isTutorial = false;
        private bool isClear = false;
        private bool isPaused = false;

        #endregion

        #region Getter

        public int GetTime()
        {
            return time;
        }

        #endregion
        
        #region Event
        public static event EventHandler<ExitStageEventArgs> TimeOverEvent;
        public static event EventHandler<ExitTutorialEventArgs> TutorialTimeOverEvent;

        #endregion

        #region Event Methods

        private void Start()
        {
            cGroup = GetComponent<CanvasGroup>();
            
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
            cGroup.Visible(!isTutorial);
            if (!isTutorial) Init();
            
            ItemInventoryHudController.ItemUseEvent += EatTimeUpItem;
            
            // Stage Scene Event
            StageStateController.StageDoneEvent += StopTimerByStageDone;
            StagePausePopupController.PauseGameEvent += PauseGame;
            
            // Tutorial Scene Event
            TutorialSceneController.StartTutorialGameEvent += StartTutorialTimer;
            TutorialStateController.TutorialDoneEvent += StopTimerByTutorialDone;
            TutorialDonePopupController.PauseTutorialEvent += PauseGame;
        }

        private void OnDisable()
        {
            ItemInventoryHudController.ItemUseEvent -= EatTimeUpItem;
            
            // Stage Scene Event
            StageStateController.StageDoneEvent -= StopTimerByStageDone;
            StagePausePopupController.PauseGameEvent -= PauseGame;
            
            // Tutorial Scene Event
            TutorialSceneController.StartTutorialGameEvent -= StartTutorialTimer;
            TutorialStateController.TutorialDoneEvent -= StopTimerByTutorialDone;
            TutorialDonePopupController.PauseTutorialEvent -= PauseGame;
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            if (isSet) return;
            time = isTutorial
                ? TutorialStageSpawner.time 
                : ChapterManager.Instance.GetStageInfo(LoadingManager.Instance.chapterType, LoadingManager.Instance.stageType).limitTime;
            StartCoroutine(StartTimer());
            isSet = true;
        }

        private IEnumerator StartTimer()
        {
            while (time >= 0 && !isClear)
            {
                while (isPaused) yield return null;
                timerText.text = $"Timer : {time}";
                time--;
                yield return new WaitForSeconds(1);
            }

            if (!isClear)
            {
                if (isTutorial)
                {
                    EmitTutorialTimeOverEvent(new ExitTutorialEventArgs
                    {
                        tutorialExitType = TutorialExitType.TimeOver
                    });
                }
                else
                {
                    EmitGameOverEvent(new ExitStageEventArgs
                    {
                        exitType = StageExitType.GameOver
                    });       
                }
            }
            
            yield return null;
        }

        private void StartTutorialTimer(object _, StartTutorialGameEventArgs e)
        {
            cGroup.Visible();
            Init();
        }

        private void EmitGameOverEvent(ExitStageEventArgs e)
        {
            if (TimeOverEvent == null) return;
            foreach (var invocation in TimeOverEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private void EmitTutorialTimeOverEvent(ExitTutorialEventArgs e)
        {
            if (TutorialTimeOverEvent == null) return;
            foreach (var invocation in TutorialTimeOverEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private void EatTimeUpItem(object _, ItemUseEventArgs e)
        {
            if (e.item.type != ItemType.Time) return;
            time += e.item.effect;
        }

        private void StopTimerByStageDone(object _, ExitStageEventArgs e)
        {
            if (e.exitType == StageExitType.GiveUp) return;
            isClear = true;
        }

        private void StopTimerByTutorialDone(object _, ExitTutorialEventArgs e)
        {
            isClear = true;
        }
        
        private void PauseGame(object _, PauseGameEventArgs e)
        {
            isPaused = e.isPaused;
        }

        #endregion
    }
}
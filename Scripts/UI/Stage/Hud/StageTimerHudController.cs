using System;
using System.Collections;
using Domain.StageObj;
using Event;
using Manager;
using StageScripts;
using UI.Stage.Popup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Stage.Hud
{
    public class StageTimerHudController: MonoBehaviour
    {
        #region Public Variable

        public int time;

        #endregion

        #region Private Variables

        [SerializeField] private Text timerText;
        private bool isSet = false;
        private bool isClear = false;
        private bool isPaused = false;

        #endregion
        
        #region Event
        public static event EventHandler<ExitStageEventArgs> TimeOverEvent;

        #endregion

        #region Event Methods

        private void Start()
        {
            ItemInventoryHudController.ItemUseEvent += EatTimeUpItem;
            StageStateController.UpdateStageStateEvent += SetTimer;
            StageStateController.StageDoneEvent += StopTimerByStageDone;
            StagePausePopupController.PauseGameEvent += PauseGame;
        }

        private void OnDisable()
        {
            ItemInventoryHudController.ItemUseEvent -= EatTimeUpItem;
            StageStateController.UpdateStageStateEvent -= SetTimer;
            StageStateController.StageDoneEvent -= StopTimerByStageDone;
            StagePausePopupController.PauseGameEvent -= PauseGame;
        }

        #endregion

        #region Private Methods

        private void SetTimer(object _, UpdateStageStateEventArgs e)
        {
            if (isSet) return;
            time = ChapterManager.Instance.GetStageInfo(LoadingManager.Instance.chapterType, LoadingManager.Instance.stageType).limitTime;
            Init(true);
        }
        
        private void Init(bool flag)
        {
            isSet = flag;
            if (isSet) StartCoroutine(StartTimer());
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
                EmitGameOverEvent(new ExitStageEventArgs
                {
                    exitType = StageExitType.GameOver
                });   
            }
            yield return null;
        }

        private void EmitGameOverEvent(ExitStageEventArgs e)
        {
            if (TimeOverEvent == null) return;
            foreach (var invocation in TimeOverEvent.GetInvocationList())
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
        
        private void PauseGame(object _, PauseGameEventArgs e)
        {
            isPaused = e.isPaused;
        }

        #endregion
    }
}
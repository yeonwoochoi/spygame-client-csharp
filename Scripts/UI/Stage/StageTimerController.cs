using System;
using System.Collections;
using Domain;
using Domain.StageObj;
using Event;
using StageScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Stage
{
    // TODO()
    public class StageTimerController: MonoBehaviour
    {
        #region Public Variable

        public int time;

        #endregion

        #region Private Variables

        [SerializeField] private Text timerText;
        private bool isSet = false;
        private bool isClear = false;

        #endregion

        private bool IsSet
        {
            get => isSet;
            set
            {
                isSet = value;
                if (isSet) StartCoroutine(StartTimer());
            }
        }

        #region Event
        public static event EventHandler<ExitStageEventArgs> TimeOverEvent;

        #endregion

        #region Event Methods

        private void Start()
        {
            ItemInventoryController.ItemUseEvent += EatTimeUpItem;
            StageStateController.UpdateStageStateEvent += SetTimer;
            StageStateController.StageDoneEvent += StopTimerByStageDone;
        }

        private void OnDisable()
        {
            ItemInventoryController.ItemUseEvent -= EatTimeUpItem;
            StageStateController.UpdateStageStateEvent -= SetTimer;
            StageStateController.StageDoneEvent -= StopTimerByStageDone;
        }

        #endregion

        #region Private Methods

        private void SetTimer(object _, UpdateStageStateEventArgs e)
        {
            if (isSet) return;
            time = e.currentStage.limitTime;
            IsSet = true;
        }

        private IEnumerator StartTimer()
        {
            while (time >= 0 && !isClear)
            { 
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

        #endregion
    }
}
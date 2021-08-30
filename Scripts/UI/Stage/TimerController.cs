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
    public class TimerController: MonoBehaviour
    {
        [SerializeField] private Text timerText;

        public int time;

        private bool isSet = false;
        private bool IsSet
        {
            get => isSet;
            set
            {
                isSet = value;
                if (isSet) StartCoroutine(StartTimer());
            }
        }
        
        private bool isClear = false;

        public static event EventHandler<ExitStageEventArgs> TimeOverEvent; 

        private void Start()
        {
            ItemTabController.ItemUseEvent += EatTimeUpItem;
            StageStateController.UpdateStageStateEvent += SetTimer;
            StageStateController.StageDoneEvent += StopTimerByStageDone;
        }

        private void OnDisable()
        {
            ItemTabController.ItemUseEvent -= EatTimeUpItem;
            StageStateController.UpdateStageStateEvent -= SetTimer;
            StageStateController.StageDoneEvent -= StopTimerByStageDone;
        }

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
                EmitGameOverEvent(new ExitStageEventArgs(StageExitType.GameOver));   
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
    }
}
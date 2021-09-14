using System;
using Domain;
using Event;
using Manager;
using UI.Qna;
using UnityEngine;

namespace TutorialScripts
{
    public class TutorialStateController: MonoBehaviour
    {
        private bool isSampleSpyCapture = true;
        private int goalSpyCount;

        // Indoor 들어가서 Spy 다 잡았을 때 Emit 하면 됨
        public static event EventHandler<ExitTutorialEventArgs> TutorialDoneEvent;

        private void Start()
        {
            goalSpyCount = TutorialStageSpawner.goalSpyCount - 1;
            
            SpyQnaPopupBehavior.CaptureSpyEvent += UpdateCurrentSpyCount;
        }

        private void OnDisable()
        {
            SpyQnaPopupBehavior.CaptureSpyEvent -= UpdateCurrentSpyCount;
        }

        private void UpdateCurrentSpyCount(object _, CaptureSpyEventArgs e)
        {
            if (isSampleSpyCapture)
            {
                isSampleSpyCapture = false;
                return;
            }
            var case1 = e.type == CaptureSpyType.Capture && !e.spy.isSpy;
            var case2 = e.type == CaptureSpyType.Release && e.spy.isSpy;
            
            AudioManager.instance.Play(!case1 && !case2 ? SoundType.Correct : SoundType.Wrong);

            if (case1)
            {
                // 딱 목표 스파이만큼 스파이 만들거니까 하나라도 틀리면 바로 실패
                EmitTutorialDoneEvent(new ExitTutorialEventArgs
                {
                    isSuccess = false
                });
                return;
            }

            if (e.type == CaptureSpyType.Capture && e.spy.isSpy)
            {
                goalSpyCount--;
                if (goalSpyCount <= 0)
                {
                    EmitTutorialDoneEvent(new ExitTutorialEventArgs
                    {
                        isSuccess = true
                    });
                }
            }
        }

        private void EmitTutorialDoneEvent(ExitTutorialEventArgs e)
        {
            if (TutorialDoneEvent == null) return;
            foreach (var invocation in TutorialDoneEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }
    }
}
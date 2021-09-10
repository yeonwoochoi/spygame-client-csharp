using System;
using Event;
using UI.Qna;
using UnityEngine;

namespace TutorialScripts
{
    public class TutorialStateController: MonoBehaviour
    {
        private int currentSpyCount;

        // Indoor 들어가서 Spy 다 잡았을 때 Emit 하면 됨
        public static event EventHandler<ExitTutorialEventArgs> TutorialDoneEvent;

        private void Start()
        {
            currentSpyCount = TutorialStageSpawner.spyCount;
            SpyQnaPopupBehavior.CaptureSpyEvent += UpdateCurrentSpyCount;
        }

        private void OnDisable()
        {
            SpyQnaPopupBehavior.CaptureSpyEvent -= UpdateCurrentSpyCount;
        }

        private void UpdateCurrentSpyCount(object _, CaptureSpyEventArgs e)
        {
            currentSpyCount--;
            Debug.Log(currentSpyCount);
            if (currentSpyCount <= 0)
            {
                EmitTutorialDoneEvent(new ExitTutorialEventArgs
                {
                    isSuccess = true
                });
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
using System;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using StageScripts;
using UI.Popup.Qna;
using UnityEngine;
using UnityEngine.UI;

namespace TutorialScripts
{
    public class TutorialStateController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Text hpText;
        [SerializeField] private Text normalSpyCountText;
        [SerializeField] private Text bossSpyCountText;

        private int currentNormalSpyCount;
        private int currentBossSpyCount;
        
        private int captureNormalSpyCount;
        private int captureBossSpyCount;

        private int currentHp;
        
        private int goalNormalSpyCount;
        private int goalBossSpyCount;

        private bool isSampleSpyCapture;

        #endregion

        #region Event

        // Indoor 들어가서 Spy 다 잡았을 때 Emit 하면 됨
        public static event EventHandler<UpdateStageStateEventArgs> UpdateTutorialStateEvent;
        public static event EventHandler<ExitTutorialEventArgs> TutorialDoneEvent;

        #endregion

        #region Setter

        private void SetTutorialState()
        {
            isSampleSpyCapture = true;

            currentHp = StageStateController.PlayerHp;
            hpText.text = $"{currentHp}";

            currentNormalSpyCount = TutorialStageSpawner.normalSpyCount;
            currentBossSpyCount = TutorialStageSpawner.bossSpyCount;
            
            captureNormalSpyCount = 0;
            captureBossSpyCount = 0;
            
            goalNormalSpyCount = TutorialStageSpawner.goalNormalSpyCount;
            goalBossSpyCount = TutorialStageSpawner.goalBossSpyCount;
            
            normalSpyCountText.text = $"{captureNormalSpyCount} / {goalNormalSpyCount}";
            bossSpyCountText.text = $"{captureBossSpyCount} / {goalBossSpyCount}";
        }
        
        private void SetCurrentSpyCount(bool isNormalSpy, bool isSuccessCapture)
        {
            if (isNormalSpy)
            {
                if (isSuccessCapture) captureNormalSpyCount++;
                currentNormalSpyCount--;
            }
            else
            {
                if (isSuccessCapture) captureBossSpyCount++;
                currentBossSpyCount--;
            }
            
            hpText.text = $"{currentHp}";
            normalSpyCountText.text = $"{captureNormalSpyCount} / {goalNormalSpyCount}";
            bossSpyCountText.text = $"{captureBossSpyCount} / {goalBossSpyCount}";

            if (currentHp <= 0)
            {
                EmitTutorialDoneEvent(new ExitTutorialEventArgs
                {
                    tutorialExitType = TutorialExitType.Failure
                });
                return;
            }

            if (captureNormalSpyCount >= goalNormalSpyCount && captureBossSpyCount >= goalBossSpyCount)
            {
                EmitTutorialDoneEvent(new ExitTutorialEventArgs
                {
                    tutorialExitType = TutorialExitType.Success
                });
                return;
            } 
            
            if (currentNormalSpyCount < goalNormalSpyCount - captureNormalSpyCount || currentBossSpyCount < goalBossSpyCount - captureBossSpyCount)
            {
                EmitTutorialDoneEvent(new ExitTutorialEventArgs
                {
                    tutorialExitType = TutorialExitType.Failure
                });
            }
        }

        #endregion

        #region Event Methods

        private void Start()
        {
            SetTutorialState();
            SpyQnaPopupBehavior.CaptureSpyEvent += UpdateCurrentSpyCount;
        }

        private void OnDisable()
        {
            SpyQnaPopupBehavior.CaptureSpyEvent -= UpdateCurrentSpyCount;
        }

        #endregion

        #region Private Methods

        private void UpdateCurrentSpyCount(object _, CaptureSpyEventArgs e)
        {
            if (isSampleSpyCapture)
            {
                isSampleSpyCapture = false;
                return;
            }

            if (!e.IsCorrect())
            {
                currentHp--;
            }
            
            AudioManager.instance.Play(e.IsCorrect() ? SoundType.Correct : SoundType.Wrong);
            SetCurrentSpyCount(e.spy.type == SpyType.Normal, e.IsCorrect());
            EmitUpdateTutorialStateEvent(new UpdateStageStateEventArgs(currentHp, captureNormalSpyCount, captureBossSpyCount, currentNormalSpyCount, currentBossSpyCount));
        }

        private void EmitTutorialDoneEvent(ExitTutorialEventArgs e)
        {
            if (TutorialDoneEvent == null) return;
            foreach (var invocation in TutorialDoneEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }

        private void EmitUpdateTutorialStateEvent(UpdateStageStateEventArgs e)
        {
            if (UpdateTutorialStateEvent == null) return;
            foreach (var invocation in UpdateTutorialStateEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }

        #endregion
    }
}
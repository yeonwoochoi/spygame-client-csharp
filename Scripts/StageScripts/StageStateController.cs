using System;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using UI.Qna;
using UI.Stage;
using UnityEngine;


namespace StageScripts
{
    public class StageStateController: MonoBehaviour
    {
        #region Public Variables

        [HideInInspector] public Stage currentStage;
        [HideInInspector] public bool isSet = false;

        #endregion

        #region Private Variables

        private int currentHp;
        private int currentNormalSpyCount;
        private int currentBossSpyCount;
        private int captureNormalSpyCount;
        private int captureBossSpyCount;
        
        #endregion

        #region Static Variable

        public static readonly int playerHp = 3;

        #endregion

        #region Events

        public static event EventHandler<UpdateStageStateEventArgs> UpdateStageStateEvent;
        public static event EventHandler<ExitStageEventArgs> StageDoneEvent;

        #endregion

        // TODO (GETTER SETTER)
        public int CurrentHp
        {
            get => currentHp;
            set
            {
                currentHp = value;
                UpdateState();
                if (currentHp <= 0)
                {
                    EmitStageDoneEvent(new ExitStageEventArgs
                    {
                        exitType = StageExitType.GameOver
                    });
                }
            }
        }

        public int CurrentNormalSpyCount
        {
            get => currentNormalSpyCount;
            set
            {
                currentNormalSpyCount = value;
                UpdateState();
                if (currentStage.goalNormalSpyCount > captureNormalSpyCount)
                {
                    if (currentStage.goalNormalSpyCount - captureNormalSpyCount > currentNormalSpyCount)
                    {
                        EmitStageDoneEvent(new ExitStageEventArgs
                        {
                            exitType = StageExitType.GameOver
                        });
                    }
                }
            }
        }

        public int CurrentBossSpyCount
        {
            get => currentBossSpyCount;
            set
            {
                currentBossSpyCount = value;
                UpdateState();
                if (currentStage.goalBossSpyCount > captureBossSpyCount)
                {
                    if (currentStage.goalBossSpyCount - captureBossSpyCount > currentBossSpyCount)
                    {
                        EmitStageDoneEvent(new ExitStageEventArgs
                        {
                            exitType = StageExitType.GameOver
                        });   
                    }
                }
            }
        }

        public int CaptureNormalSpyCount
        {
            get => captureNormalSpyCount;
            set
            {
                captureNormalSpyCount = value;
                UpdateState();
                if (currentStage.goalNormalSpyCount <= captureNormalSpyCount && currentStage.goalBossSpyCount <= captureBossSpyCount)
                {
                    EmitStageDoneEvent(new ExitStageEventArgs
                    {
                        exitType = StageExitType.StageClear
                    });
                }
            }
        }

        public int CaptureBossSpyCount
        {
            get => captureBossSpyCount;
            set
            {
                captureBossSpyCount = value;
                UpdateState();
                if (currentStage.goalNormalSpyCount <= captureNormalSpyCount && currentStage.goalBossSpyCount <= captureBossSpyCount)
                {
                    EmitStageDoneEvent(new ExitStageEventArgs
                    {
                        exitType = StageExitType.StageClear
                    });
                }
            }
        }

        #region Event Methods

        private void Start()
        {
            ItemInventoryController.ItemUseEvent += GetHp;
            SpyQnaPopupBehavior.CaptureSpyEvent += LoseHp;
            SpyQnaPopupBehavior.CaptureSpyEvent += ProcessSpyInterviewResult;
        }

        private void OnDisable()
        {
            ItemInventoryController.ItemUseEvent -= GetHp;
            SpyQnaPopupBehavior.CaptureSpyEvent -= LoseHp;
            SpyQnaPopupBehavior.CaptureSpyEvent -= ProcessSpyInterviewResult;
        }

        #endregion

        #region Public Method

        public void SetStageState()
        {
            CurrentHp = playerHp;
            currentNormalSpyCount = currentStage.normalSpyCount;
            currentBossSpyCount = currentStage.bossSpyCount;
            captureNormalSpyCount = 0;
            captureBossSpyCount = 0;
            UpdateState();
            isSet = true;
        }

        #endregion

        #region Private Methods

        private void ProcessSpyInterviewResult(object _, CaptureSpyEventArgs e)
        {
            if (e.type == CaptureSpyType.Capture && e.spy.isSpy)
            {
                if (e.spy.type == SpyType.Normal) CaptureNormalSpyCount++;
                else CaptureBossSpyCount++;
            }

            if (e.spy.type == SpyType.Normal) CurrentNormalSpyCount--;
            else CurrentBossSpyCount--;
        }

        private void UpdateState()
        {
            EmitUpdateStageStateEvent(
                new UpdateStageStateEventArgs(currentStage, currentHp, captureNormalSpyCount, captureBossSpyCount, currentNormalSpyCount, currentBossSpyCount));
        }

        private void GetHp(object _, ItemUseEventArgs e)
        {
            if (e.item.type == ItemType.Hp && currentHp < playerHp) CurrentHp++;
        }
        
        private void LoseHp(object _, CaptureSpyEventArgs e)
        {
            var case1 = e.type == CaptureSpyType.Capture && !e.spy.isSpy;
            var case2 = e.type == CaptureSpyType.Release && e.spy.isSpy;

            if (!case1 && !case2)
            {
                AudioManager.instance.Play(SoundType.Correct);
                return;
            }
            if (CurrentHp <= 0) return;
            CurrentHp--;
            AudioManager.instance.Play(SoundType.Wrong);
        }
        
        private void EmitUpdateStageStateEvent(UpdateStageStateEventArgs e)
        {
            if (UpdateStageStateEvent == null) return;
            foreach (var invocation in UpdateStageStateEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void EmitStageDoneEvent(ExitStageEventArgs e)
        {
            if (StageDoneEvent == null) return;
            foreach (var invocation in StageDoneEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        #endregion
    }
}
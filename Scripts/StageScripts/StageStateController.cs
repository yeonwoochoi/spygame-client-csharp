using System;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using UI.Qna;
using UI.StageScripts;
using UI.StageScripts.Hud;
using UnityEngine;


namespace StageScripts
{
    public class StageStateController: MonoBehaviour
    {
        #region Private Variables

        private StageInfo currentStageInfo;
        private int currentHp;
        private int currentNormalSpyCount;
        private int currentBossSpyCount;
        private int captureNormalSpyCount;
        private int captureBossSpyCount;

        private bool isSet = false;

        #endregion

        #region Static Variable

        public const int PlayerHp = 3;

        #endregion

        #region Setter

        private void SetCurrentHp(int hp)
        {
            currentHp = hp;
            UpdateState();
            if (currentHp <= 0)
            {
                EmitStageDoneEvent(new ExitStageEventArgs
                {
                    exitType = StageExitType.GameOver
                });
            }
        }

        private void SetCurrentNormalSpyCount(int count)
        {
            currentNormalSpyCount = count;
            UpdateState();
            if (currentStageInfo.goalNormalSpyCount > captureNormalSpyCount)
            {
                if (currentStageInfo.goalNormalSpyCount - captureNormalSpyCount > currentNormalSpyCount)
                {
                    EmitStageDoneEvent(new ExitStageEventArgs
                    {
                        exitType = StageExitType.GameOver
                    });
                }
            }
        }

        private void SetCurrentBossSpyCount(int count)
        {
            currentBossSpyCount = count;
            UpdateState();
            if (currentStageInfo.goalBossSpyCount > captureBossSpyCount)
            {
                if (currentStageInfo.goalBossSpyCount - captureBossSpyCount > currentBossSpyCount)
                {
                    EmitStageDoneEvent(new ExitStageEventArgs
                    {
                        exitType = StageExitType.GameOver
                    });   
                }
            }
        }

        private void SetCaptureNormalSpyCount(int count)
        {
            captureNormalSpyCount = count;
            UpdateState();
            if (currentStageInfo.goalNormalSpyCount <= captureNormalSpyCount && currentStageInfo.goalBossSpyCount <= captureBossSpyCount)
            {
                EmitStageDoneEvent(new ExitStageEventArgs
                {
                    exitType = StageExitType.StageClear
                });
            }
        }

        private void SetCaptureBossSpyCount(int count)
        {
            captureBossSpyCount = count;
            UpdateState();
            if (currentStageInfo.goalNormalSpyCount <= captureNormalSpyCount && currentStageInfo.goalBossSpyCount <= captureBossSpyCount)
            {
                EmitStageDoneEvent(new ExitStageEventArgs
                {
                    exitType = StageExitType.StageClear
                });
            }
        }

        #endregion

        #region Events

        public static event EventHandler<UpdateStageStateEventArgs> UpdateStageStateEvent;
        public static event EventHandler<ExitStageEventArgs> StageDoneEvent;

        #endregion

        #region Event Methods

        private void Start()
        {
            ItemInventoryHudController.ItemUseEvent += GetHp;
            SpyQnaPopupBehavior.CaptureSpyEvent += LoseHp;
            SpyQnaPopupBehavior.CaptureSpyEvent += ProcessSpyInterviewResult;
            
            currentStageInfo = ChapterManager.Instance.GetStageInfo(LoadingManager.Instance.chapterType, LoadingManager.Instance.stageType);
            SetStageState();
        }

        private void OnDisable()
        {
            ItemInventoryHudController.ItemUseEvent -= GetHp;
            SpyQnaPopupBehavior.CaptureSpyEvent -= LoseHp;
            SpyQnaPopupBehavior.CaptureSpyEvent -= ProcessSpyInterviewResult;
        }

        #endregion

        #region Public Method

        private void SetStageState()
        {
            if (isSet) return;
            SetCurrentHp(PlayerHp);
            currentNormalSpyCount = currentStageInfo.normalSpyCount;
            currentBossSpyCount = currentStageInfo.bossSpyCount;
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
                if (e.spy.type == SpyType.Normal) SetCaptureNormalSpyCount(captureNormalSpyCount + 1);
                else SetCaptureBossSpyCount(captureBossSpyCount + 1);
            }

            if (e.spy.type == SpyType.Normal) SetCurrentNormalSpyCount(currentNormalSpyCount - 1);
            else SetCurrentBossSpyCount(currentBossSpyCount - 1);
        }

        private void UpdateState()
        {
            EmitUpdateStageStateEvent(
                new UpdateStageStateEventArgs(currentHp, captureNormalSpyCount, captureBossSpyCount, currentNormalSpyCount, currentBossSpyCount));
        }

        private void GetHp(object _, ItemUseEventArgs e)
        {
            if (e.item.type == ItemType.Hp && currentHp < PlayerHp) SetCurrentHp(currentHp + 1);
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
            if (currentHp <= 0) return;
            SetCurrentHp(currentHp - 1);
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
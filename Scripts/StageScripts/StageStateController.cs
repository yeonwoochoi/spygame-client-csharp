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
        [HideInInspector] public Stage currentStage;
        [HideInInspector] public bool isSet = false;

        public static readonly int playerHp = 3;

        private int currentHp;
        public int CurrentHp
        {
            get => currentHp;
            set
            {
                currentHp = value;
                UpdateState();
                if (currentHp <= 0)
                {
                    EmitGameOverEvent(new GameOverEventArgs());
                }
            }
        }
        
        private int currentNormalSpyCount;

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
                        EmitGameOverEvent(new GameOverEventArgs());   
                    }
                }
            }
        }

        private int currentBossSpyCount;
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
                        EmitGameOverEvent(new GameOverEventArgs());   
                    }
                }
            }
        }
        
        private int captureNormalSpyCount;
        public int CaptureNormalSpyCount
        {
            get => captureNormalSpyCount;
            set
            {
                captureNormalSpyCount = value;
                UpdateState();
                if (currentStage.goalNormalSpyCount <= captureNormalSpyCount && currentStage.goalBossSpyCount <= captureBossSpyCount)
                {
                    EmitStageClearEvent(new StageClearEventArgs());
                }
            }
        }
        
        private int captureBossSpyCount;
        public int CaptureBossSpyCount
        {
            get => captureBossSpyCount;
            set
            {
                captureBossSpyCount = value;
                UpdateState();
                if (currentStage.goalNormalSpyCount <= captureNormalSpyCount && currentStage.goalBossSpyCount <= captureBossSpyCount)
                {
                    EmitStageClearEvent(new StageClearEventArgs());
                }
            }
        }

        public static event EventHandler<UpdateStageStateEventArgs> UpdateStageStateEvent;
        public static event EventHandler<GameOverEventArgs> GameOverEvent;
        public static event EventHandler<StageClearEventArgs> StageClearEvent;

        private void Start()
        {
            ItemTabController.ItemUseEvent += GetHp;
            SpyQnaPopupBehavior.CaptureSpyEvent += LoseHp;
            SpyQnaPopupBehavior.CaptureSpyEvent += ProcessSpyInterviewResult;
        }

        private void OnDestroy()
        {
            ItemTabController.ItemUseEvent -= GetHp;
            SpyQnaPopupBehavior.CaptureSpyEvent -= LoseHp;
            SpyQnaPopupBehavior.CaptureSpyEvent -= ProcessSpyInterviewResult;
        }

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
        
        private void EmitStageClearEvent(StageClearEventArgs e)
        {
            if (StageClearEvent == null) return;
            foreach (var invocation in StageClearEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void EmitGameOverEvent(GameOverEventArgs e)
        {
            if (GameOverEvent == null) return;
            foreach (var invocation in GameOverEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
    }
}
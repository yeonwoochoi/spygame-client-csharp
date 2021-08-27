using System;
using Domain;
using Event;
using Manager;
using Manager.Data;
using StageScripts;
using UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Stage
{
    public class StageDonePopupController: BasePopupBehavior
    {
        [SerializeField] private Text stageText;
        [SerializeField] private StarHandler starHandler;
        [SerializeField] private GameObject retryButton;
        [SerializeField] private GameObject exitButton;
        [SerializeField] private TimerController timerController;

        private Domain.Stage currentStage;
        private int currentHp;
        private readonly string stageClearComment = "Stage Clear";
        private readonly string gameOverComment = "Game Over";
        private bool isDone;
        
        public static event EventHandler<ExitStageEventArgs> ExitStageEvent;

        protected override void Start()
        {
            base.Start();
            isDone = false;
            retryButton.GetComponent<Button>().onClick.AddListener(RestartGame);
            exitButton.GetComponent<Button>().onClick.AddListener(ExitGame);
            StageStateController.UpdateStageStateEvent += UpdateStageState;
            
            // AdmobManager.CloseStageAdEvent += OpenStageClear;
            // AdmobManager.CloseGameOverAdEvent += OpenGameOver;
            
            TimerController.TimeOverEvent += OpenGameOver;
            StageStateController.GameOverEvent += OpenGameOver;
            StageStateController.StageClearEvent += OpenStageClear;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StageStateController.UpdateStageStateEvent -= UpdateStageState;
            
            //AdmobManager.CloseStageAdEvent -= OpenStageClear;
            //AdmobManager.CloseGameOverAdEvent -= OpenGameOver;
            
            TimerController.TimeOverEvent -= OpenGameOver;
            StageStateController.GameOverEvent -= OpenGameOver;
            StageStateController.StageClearEvent -= OpenStageClear;
        }

        private void UpdateStageState(object _, UpdateStageStateEventArgs e)
        {
            currentStage = e.currentStage;
            currentHp = e.hp;
        }

        private void OpenGameOver(object _, GameOverEventArgs e)
        {
            if (isDone) return;
            isDone = true;
            titleText.text = $"{gameOverComment}";
            stageText.text = $"{currentStage.stageType}";
            OnOpenPopup();
            starHandler.ShowStars(0);
            AudioManager.instance.Stop(SoundType.Background);
            AudioManager.instance.Play(SoundType.GameOver);
        }
        
        private void OpenStageClear(object _, StageClearEventArgs e)
        {
            if (isDone) return;
            isDone = true;
            titleText.text = $"{stageClearComment}";
            stageText.text = $"{currentStage.stageType}";
            OnOpenPopup();
            var score = CalculateStarScore();
            starHandler.ShowStars(score);
            if (currentStage.score < score)
            {
                currentStage.score = score;
                var chapterManager = GlobalDataManager.Instance.Get<ChapterManager>(GlobalDataKey.CHAPTER);
                var chapterIndex = (int) currentStage.chapterType;
                var stageIndex = (int) currentStage.stageType;
                chapterManager.chapterInfos[chapterIndex - 1].stageInfos[stageIndex].score = score;
                GlobalDataManager.Instance.Set(GlobalDataKey.CHAPTER, chapterManager);
            }

            AudioManager.instance.Stop(SoundType.Background);
            AudioManager.instance.Play(SoundType.StageClear);
        }
        
        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ExitGame()
        {
            EmitExitStageEvent(new ExitStageEventArgs(StageExitType.Clear));
            OnClosePopup();
        }

        private int CalculateStarScore()
        {
            var time = timerController.time;
            var totalTime = currentStage.limitTime;
            
            var totalScore = 0;

            if (currentHp >= 3)
            {
                totalScore += 2;
            }
            else if (currentHp == 2)
            {
                totalScore += 1;
            }

            if (time >= totalTime * 0.5)
            {
                totalScore += 2;
            } 
            else if (time >= totalTime * 0.25 && time < totalTime * 0.5)
            {
                totalScore += 1;
            }

            if (totalScore >= 4) return 3;
            else if (totalScore >= 2) return 2;
            else return 1;
        }
        
        private void EmitExitStageEvent(ExitStageEventArgs e)
        {
            if (ExitStageEvent == null) return;
            foreach (var invocation in ExitStageEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
    }
}
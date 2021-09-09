using System;
using Domain;
using Event;
using Manager;
using Manager.Data;
using StageScripts;
using UI.Base;
using UI.StageScripts.Hud;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.StageScripts.Popup
{
    public class StageDonePopupController: BasePopupBehavior
    {
        #region Private Variables

        [SerializeField] private Text stageText;
        [SerializeField] private StarHandler starHandler;
        [SerializeField] private GameObject retryButton;
        [SerializeField] private GameObject exitButton;
        [SerializeField] private StageTimerHudController stageTimerHudController;

        private ChapterType chapterType;
        private StageType stageType;
        
        private StageInfo currentStageInfo;
        private StageScoreManager stageScoreManager;
        
        private int currentHp;
        private readonly string stageClearComment = "Stage Clear";
        private readonly string gameOverComment = "Game Over";
        private bool isDone;

        #endregion

        #region Event

        public static event EventHandler<ExitStageEventArgs> ExitStageSceneEvent;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            isDone = false;
            retryButton.GetComponent<Button>().onClick.AddListener(RestartGame);
            exitButton.GetComponent<Button>().onClick.AddListener(ExitGame);
            
            chapterType = LoadingManager.Instance.chapterType;
            stageType = LoadingManager.Instance.stageType;
            
            currentStageInfo = ChapterManager.Instance.GetStageInfo(chapterType, stageType);
            stageScoreManager = GlobalDataManager.Instance.Get<StageScoreManager>(GlobalDataKey.STAGE_SCORE);
            
            StageStateController.UpdateStageStateEvent += UpdateStageState;
            StageTimerHudController.TimeOverEvent += OpenGameOver;
            StageStateController.StageDoneEvent += OpenGameOver;
            StageStateController.StageDoneEvent += OpenStageDone;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            StageStateController.UpdateStageStateEvent -= UpdateStageState;
            StageTimerHudController.TimeOverEvent -= OpenGameOver;
            StageStateController.StageDoneEvent -= OpenGameOver;
            StageStateController.StageDoneEvent -= OpenStageDone;
        }

        #endregion

        #region Private Methods

        private void UpdateStageState(object _, UpdateStageStateEventArgs e)
        {
            currentHp = e.hp;
        }

        private void OpenGameOver(object _, ExitStageEventArgs e)
        {
            if (e.exitType != StageExitType.GameOver) return;
            if (isDone) return;
            isDone = true;
            titleText.text = $"{gameOverComment}";
            stageText.text = $"{currentStageInfo.stageType}";
            OnOpenPopup();
            starHandler.ShowStars(0);
            AudioManager.instance.Stop(SoundType.Background);
            AudioManager.instance.Play(SoundType.GameOver);
        }
        
        private void OpenStageDone(object _, ExitStageEventArgs e)
        {
            if (e.exitType != StageExitType.StageClear) return;
            if (isDone) return;
            
            var score = CalculateStarScore();
            SetStageScore(score);
            
            titleText.text = $"{stageClearComment}";
            stageText.text = $"{currentStageInfo.stageType}";
            starHandler.ShowStars(score);
            OnOpenPopup();

            AudioManager.instance.Stop(SoundType.Background);
            AudioManager.instance.Play(SoundType.StageClear);
            
            isDone = true;
        }

        private void SetStageScore(int score)
        {
            if (stageScoreManager.GetStageScore(chapterType, stageType) >= score) return;
            stageScoreManager.SetStageScore(chapterType, stageType, score);
            GlobalDataManager.Instance.Set(GlobalDataKey.STAGE_SCORE, stageScoreManager);
        }
        
        private void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ExitGame()
        {
            EmitExitStageEvent(new ExitStageEventArgs
            {
                exitType = StageExitType.StageClear
            });
            OnClosePopup();
        }

        private int CalculateStarScore()
        {
            var time = stageTimerHudController.time;
            var totalTime = currentStageInfo.limitTime;
            
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
            if (ExitStageSceneEvent == null) return;
            foreach (var invocation in ExitStageSceneEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        #endregion
    }
}
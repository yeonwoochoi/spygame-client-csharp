using System;
using Domain;
using Event;
using Manager;
using StageScripts;
using UI.Qna;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Stage
{
    public class StageHudController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Text normalSpyCountText;
        [SerializeField] private Text bossSpyCountText;
        [SerializeField] private Text hpText;
        [SerializeField] private Button pauseButton;

        private PseudoStageInfo currentStage;
        private int hp;
        private int captureNormalSpyCount;
        private int captureBossSpyCount;

        #endregion

        #region Event

        public static event EventHandler<OpenStagePauseEventArgs> OpenStagePauseEvent;

        #endregion

        #region Event Methods

        private void Start()
        {
            pauseButton.onClick.AddListener(EmitOpenStagePauseEvent);
            currentStage = PseudoChapter.Instance.GetStageInfo(LoadingManager.Instance.chapterType,
                LoadingManager.Instance.stageType);
            StageStateController.UpdateStageStateEvent += UpdateStageState;
        }

        private void OnDisable()
        {
            StageStateController.UpdateStageStateEvent -= UpdateStageState;
        }

        #endregion

        #region Private Methods

        private void UpdateStageState(object _, UpdateStageStateEventArgs e)
        {
            hp = e.hp;
            captureNormalSpyCount = e.captureNormalSpyCount;
            captureBossSpyCount = e.captureBossSpyCount;
            normalSpyCountText.text = $"{captureNormalSpyCount} / {currentStage.goalNormalSpyCount}";
            bossSpyCountText.text = $"{captureBossSpyCount} / {currentStage.goalBossSpyCount}";
            hpText.text = $"{hp}";
        }
        
        
        private void EmitOpenStagePauseEvent()
        {
            if (OpenStagePauseEvent == null) return;
            foreach (var invocation in OpenStagePauseEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, new OpenStagePauseEventArgs());
            }
        }

        #endregion
    }
}
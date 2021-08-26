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
        [SerializeField] private Text normalSpyCountText;
        [SerializeField] private Text bossSpyCountText;
        [SerializeField] private Text hpText;
        [SerializeField] private Button pauseButton;

        private Domain.Stage currentStage;
        private int hp;
        private int captureNormalSpyCount;
        private int captureBossSpyCount;

        public static event EventHandler<OpenStagePauseEventArgs> OpenStagePauseEvent;

        private void Start()
        {
            pauseButton.onClick.AddListener(EmitOpenStagePauseEvent);
            StageStateController.UpdateStageStateEvent += UpdateStageState;
        }

        private void OnDestroy()
        {
            StageStateController.UpdateStageStateEvent -= UpdateStageState;
        }

        private void UpdateStageState(object _, UpdateStageStateEventArgs e)
        {
            hp = e.hp;
            currentStage = e.currentStage;
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
    }
}
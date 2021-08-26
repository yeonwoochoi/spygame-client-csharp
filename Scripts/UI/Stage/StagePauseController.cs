using System;
using Event;
using Manager;
using StageScripts;
using UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Stage
{
    public class StagePauseController: BasePopupBehavior
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button effectButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button exitButton;

        public static event EventHandler<ExitStageEventArgs> ExitStageEvent;

        protected override void Start()
        {
            base.Start();
            cancelButton.onClick.AddListener(RestartGame);
            soundButton.onClick.AddListener(MuteSound);
            effectButton.onClick.AddListener(MuteEffect);
            retryButton.onClick.AddListener(RetryGame);
            exitButton.onClick.AddListener(ExitGame);
            StageHudController.OpenStagePauseEvent += OpenPausePopup;
            
            soundButton.GetComponent<ImageChangeButtonController>().Set(!AudioManager.instance.IsSoundMute);
            effectButton.GetComponent<ImageChangeButtonController>().Set(!AudioManager.instance.IsEffectMute);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StageHudController.OpenStagePauseEvent -= OpenPausePopup;
        }

        private void MuteSound()
        {
            AudioManager.instance.IsSoundMute = !AudioManager.instance.IsSoundMute;
        }

        private void MuteEffect()
        {
            AudioManager.instance.IsEffectMute = !AudioManager.instance.IsEffectMute;
        }

        private void RestartGame()
        {
            OnClosePopup();
        }

        private void RetryGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ExitGame()
        {
            EmitExitStageEvent(new ExitStageEventArgs(StageExitType.Pause));
        }

        private void OpenPausePopup(object _, OpenStagePauseEventArgs e)
        {
            OnOpenPopup();
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
using System;
using Domain;
using Event;
using Manager;
using UI.Base;
using UI.Hud;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Popup.StageScripts
{
    public class StagePausePopupController: BasePopupBehavior
    {
        #region Private Variables

        [SerializeField] private Button cancelButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button effectButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button exitButton;

        #endregion

        #region Event
        public static event EventHandler<ExitStageEventArgs> ExitStageEvent;

        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            cancelButton.onClick.AddListener(RestartGame);
            soundButton.onClick.AddListener(MuteSound);
            effectButton.onClick.AddListener(MuteEffect);
            retryButton.onClick.AddListener(RetryGame);
            exitButton.onClick.AddListener(ExitGame);
            StageStateHudController.OpenStagePauseEvent += OpenPausePopup;
            
            soundButton.GetComponent<ImageChangeButtonController>().Init(!AudioManager.instance.GetIsSoundMute());
            effectButton.GetComponent<ImageChangeButtonController>().Init(!AudioManager.instance.GetIsEffectMute());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StageStateHudController.OpenStagePauseEvent -= OpenPausePopup;
        }

        #endregion

        #region Private Methods

        private void MuteSound()
        {
            AudioManager.instance.SetIsSoundMute(!AudioManager.instance.GetIsSoundMute());
        }

        private void MuteEffect()
        {
            AudioManager.instance.SetIsEffectMute(!AudioManager.instance.GetIsEffectMute());
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
            AudioManager.instance.Stop(SoundType.Background);
            EmitExitStageEvent(new ExitStageEventArgs
            {
                exitType = StageExitType.GiveUp
            });
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

        #endregion
    }
}
using Base;
using Control.Movement;
using Control.SpeechBalloon;
using Domain;
using Manager;
using UnityEngine;

namespace Control.Collision
{
    public class SpyDetectorBehavior: BaseDetectorBehavior
    {
        #region Private Variables

        private AudioManager audioManager;
        private SpyMoveController spyMoveController;
        private GameObject speechBalloon;
        private SpySpeechBalloonController speechBalloonController;

        #endregion

        #region Event Methods

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            if (eControlType == EControlType.Mouse) other.gameObject.GetComponent<PlayerMoveController>().StopMove();
            spyMoveController.SpyStateType = SpyStateType.Examined;
            speechBalloon.SetActive(true);
            speechBalloonController.StartDetection();
            audioManager.Play(SoundType.Meet);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            speechBalloonController.clicked = false;
            speechBalloon.SetActive(false);
            spyMoveController.SpyStateType = SpyStateType.Free;
            spyMoveController.StartWandering();
        }

        #endregion
        
        #region Protected Method

        protected override void InitDetector()
        {
            base.InitDetector();
            audioManager = AudioManager.instance;
            spyMoveController = GetParentController<SpyMoveController>();
            speechBalloon = spyMoveController.speechBalloon;
            speechBalloonController = speechBalloon.GetComponent<SpySpeechBalloonController>();
            isSet = true;
        }

        #endregion

        #region Private Method

        private bool IsValidTrigger(string tag)
        {
            if (!isSet || spyMoveController == null || !spyMoveController.IsSet || tag == "Detector") return false;
            if (spyMoveController.SpyStateType == SpyStateType.Capture || spyMoveController.SpyStateType == SpyStateType.Release) return false;
            return tag == "Player";
        }

        #endregion
    }
}
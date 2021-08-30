using System;
using Base;
using Control.Item;
using Control.Movement;
using Control.SpeechBalloon;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Control.Collision
{
    public class BoxDetectorBehavior: BaseDetectorBehavior
    {
        #region Private Variables

        private ItemBoxController itemBoxController;
        private GameObject speechBalloon;
        private BoxSpeechBalloonController speechBalloonController;
        private AudioManager audioManager;

        #endregion
        
        #region Event Methods

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            audioManager.Play(SoundType.Meet);
            if (eControlType == EControlType.KeyBoard) return;
            other.gameObject.GetComponent<PlayerMoveController>().StopMove();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            speechBalloon.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            speechBalloon.SetActive(false);
            if (eControlType == EControlType.KeyBoard) return;
            speechBalloonController.clicked = false;
        }

        #endregion

        #region Protected Method

        protected override void InitDetector()
        {
            base.InitDetector();
            audioManager = AudioManager.instance;
            itemBoxController = GetParentController<ItemBoxController>();
            speechBalloon = itemBoxController.speechBalloon;
            speechBalloonController = speechBalloon.GetComponent<BoxSpeechBalloonController>();
            isSet = true;
        }

        #endregion

        #region Private Method

        private bool IsValidTrigger(string tag)
        {
            if (!isSet || itemBoxController == null || !itemBoxController.IsSet || itemBoxController.IsOpen) return false;
            return tag == "Player";
        }

        #endregion
    }
}
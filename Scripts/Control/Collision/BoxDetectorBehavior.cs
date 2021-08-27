using System;
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
        private ItemBoxController itemBoxController;

        private void Start()
        {
            itemBoxController = InitDetector<ItemBoxController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            AudioManager.instance.Play(SoundType.Meet);
            if (eControlType == EControlType.KeyBoard) return;
            other.gameObject.GetComponent<PlayerMoveController>().StopMove();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            itemBoxController.speechBalloon.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            itemBoxController.speechBalloon.SetActive(false);
            if (eControlType == EControlType.KeyBoard) return;
            itemBoxController.speechBalloon.GetComponent<BoxSpeechBalloonController>().clicked = false;
        }

        // TODO
        private bool IsValidTrigger(string tag)
        {
            if (!isSet) return false;
            if (itemBoxController == null) return false;
            if (!itemBoxController.IsSet) return false;
            if (itemBoxController.IsOpen) return false;
            return tag == "Player";
        }
    }
}
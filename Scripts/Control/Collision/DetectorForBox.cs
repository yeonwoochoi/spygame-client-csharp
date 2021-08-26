﻿using System;
using Control.Item;
using Control.Movement;
using Control.SpeechBalloon;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Control.Collision
{
    public class DetectorForBox: MonoBehaviour
    {
        [SerializeField] private ItemBoxController itemBoxController;

        private EControlManager eControlManager;

        private void Start()
        {
            eControlManager = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            AudioManager.instance.Play(SoundType.Meet);
            if (eControlManager.eControlType == EControlType.KeyBoard) return;
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
            if (eControlManager.eControlType == EControlType.KeyBoard) return;
            itemBoxController.speechBalloon.GetComponent<BoxSpeechBalloonController>().clicked = false;
        }

        private bool IsValidTrigger(string tag)
        {
            if (!itemBoxController.IsSet) return false;
            if (itemBoxController.Item == null) return false;
            if (itemBoxController.IsOpen) return false;
            return tag == "Player";
        }
    }
}
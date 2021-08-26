using Control.Movement;
using Control.SpeechBalloon;
using Domain;
using Manager;
using UnityEngine;

namespace Control.Collision
{
    public class SpyDetectorBehavior: BaseDetectorBehavior
    {
        private SpyMoveController spyMoveController;

        private void Start()
        {
            spyMoveController = InitDetector<SpyMoveController>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            if (eControlType == EControlType.Mouse) other.gameObject.GetComponent<PlayerMoveController>().StopMove();
            spyMoveController.SpyStateType = SpyStateType.Examined;
            spyMoveController.speechBalloon.SetActive(true);
            AudioManager.instance.Play(SoundType.Meet);
        }

        protected void OnTriggerStay2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            spyMoveController.speechBalloon.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsValidTrigger(other.gameObject.tag)) return;
            spyMoveController.speechBalloon.GetComponent<SpySpeechBalloonController>().clicked = false;
            spyMoveController.speechBalloon.SetActive(false);
            spyMoveController.SpyStateType = SpyStateType.Free;
            spyMoveController.StartWandering();
        }

        private bool IsValidTrigger(string tag)
        {
            if (!isSet) return false;
            if (spyMoveController == null) return false;
            if (!spyMoveController.IsSet) return false;
            if (tag == "Detector") return false;
            if (spyMoveController.SpyStateType == SpyStateType.Capture || spyMoveController.SpyStateType == SpyStateType.Release) return false;
            return tag == "Player";
        }
    }
}
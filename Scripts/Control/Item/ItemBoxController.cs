using System;
using Control.SpeechBalloon;
using Domain;
using Event;
using Manager;
using UI.Qna;
using UnityEngine;

namespace Control.Item
{
    public class ItemBoxController: MonoBehaviour
    {
        private static string ANIMATION_VARIABLE_BOX_OPEN = "IsOpen";

        [SerializeField] public GameObject speechBalloon;

        [HideInInspector] public BoxSpeechBalloonController boxSpeechBalloonController;
        private bool isSet = false;

        public bool IsSet
        {
            get => isSet;
            set
            {
                isSet = value;
                if (isSet)
                {
                    isOpen = false;
                    animator = GetComponent<Animator>();
                    animator.SetBool(ANIMATION_VARIABLE_BOX_OPEN, false);
                }
            }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;
                animator.SetBool(ANIMATION_VARIABLE_BOX_OPEN, isOpen);
            }
        }

        private Animator animator;
        
        private Domain.StageObj.Item item;
        public Domain.StageObj.Item Item
        {
            get => item;
            set
            {
                item = value;
                boxSpeechBalloonController = speechBalloon.GetComponent<BoxSpeechBalloonController>();
                boxSpeechBalloonController.item = value;
                IsSet = true;
            }
        }

        private void Start()
        {
            ItemQnaPopupBehavior.ItemGetEvent += InactivateSpeechBalloon;
        }

        private void OnDestroy()
        {
            ItemQnaPopupBehavior.ItemGetEvent -= InactivateSpeechBalloon;
        }

        private void InactivateSpeechBalloon(object _, ItemGetEventArgs e)
        {
            if (!IsSet) return;
            if (e.item.index != item.index) return;
            IsOpen = true;
            speechBalloon.SetActive(false);
        }
    }
}
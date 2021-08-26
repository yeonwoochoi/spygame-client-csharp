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

        private Animator animator;
        
        public bool IsSet { get; private set; } = false;
        
        private bool isOpen;
        public bool IsOpen
        {
            get => isOpen;
            private set
            {
                isOpen = value;
                animator.SetBool(ANIMATION_VARIABLE_BOX_OPEN, isOpen);
            }
        }

        public Domain.StageObj.Item Item { get; private set; }

        private void Start()
        {
            ItemQnaPopupBehavior.ItemGetEvent += InactivateSpeechBalloon;
        }

        private void OnDestroy()
        {
            ItemQnaPopupBehavior.ItemGetEvent -= InactivateSpeechBalloon;
        }

        public void Init(Domain.StageObj.Item item)
        {
            isOpen = false;
            animator = GetComponent<Animator>();
            animator.SetBool(ANIMATION_VARIABLE_BOX_OPEN, false);
            Item = item;
            boxSpeechBalloonController = speechBalloon.GetComponent<BoxSpeechBalloonController>();
            boxSpeechBalloonController.item = item;
            IsSet = true;
        }

        private void InactivateSpeechBalloon(object _, ItemGetEventArgs e)
        {
            if (!IsSet) return;
            if (e.item.index != Item.index) return;
            IsOpen = true;
            speechBalloon.SetActive(false);
        }
    }
}
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
        #region Public Variables

        [SerializeField] public GameObject speechBalloon;

        [HideInInspector] public BoxSpeechBalloonController boxSpeechBalloonController;

        #endregion

        #region Private Variables

        private Animator animator;

        #endregion

        #region Static Variable

        private static string ANIMATION_VARIABLE_BOX_OPEN = "IsOpen";

        #endregion

        // TODO (Property => Getter Setter)
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

        #region Event Methods

        private void Start()
        {
            ItemQnaPopupBehavior.ItemGetEvent += InactivateSpeechBalloon;
        }

        private void OnDisable()
        {
            ItemQnaPopupBehavior.ItemGetEvent -= InactivateSpeechBalloon;
        }

        #endregion

        #region Public Method

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

        #endregion

        #region Private Method

        private void InactivateSpeechBalloon(object _, ItemGetEventArgs e)
        {
            if (!IsSet) return;
            if (e.item.index != Item.index) return;
            IsOpen = true;
            speechBalloon.SetActive(false);
        }

        #endregion
    }
}
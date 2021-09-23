using System;
using Control.SpeechBalloon;
using Domain;
using Event;
using Manager;
using UI.Popup.Qna;
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

        private Domain.StageObj.Item item;
        private Animator animator;
        private bool isSet = false;
        private bool isOpen = false;

        #endregion

        #region Const Variable

        private const string AnimationVariableBoxOpen = "IsOpen";
        private static readonly int Open = Animator.StringToHash(AnimationVariableBoxOpen);

        #endregion

        #region Getter

        public bool GetIsSet()
        {
            return isSet;
        }

        public bool GetIsOpen()
        {
            return isOpen;
        }

        public Domain.StageObj.Item GetItem()
        {
            return item;
        }

        #endregion

        #region Setter

        private void SetIsOpen(bool flag)
        {
            isOpen = flag;
            animator.SetBool(Open, isOpen);
        }

        #endregion

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
            animator = GetComponent<Animator>();
            animator.SetBool(Open, false);
            this.item = item;
            boxSpeechBalloonController = speechBalloon.GetComponent<BoxSpeechBalloonController>();
            boxSpeechBalloonController.item = item;
            isSet = true;
        }

        #endregion

        #region Private Method

        private void InactivateSpeechBalloon(object _, ItemGetEventArgs e)
        {
            if (!isSet) return;
            if (e.item.index != item.index) return;
            SetIsOpen(true);
            speechBalloon.SetActive(false);
        }

        #endregion
    }
}
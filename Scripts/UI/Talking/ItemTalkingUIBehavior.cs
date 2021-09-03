using System;
using System.Collections;
using Control.Movement;
using Control.SpeechBalloon;
using Domain;
using Domain.StageObj;
using Event;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Talking
{
    public class ItemTalkingUIBehavior: BaseTalkingUIBehavior
    {
        #region Private Variables

        [SerializeField] private CanvasGroup npcCanvasGroup;
        [SerializeField] private Text npcCommentText;

        private Item item;

        #endregion

        #region Readonly Variable

        private const string NpcComment = "아이템 얻기를 시도하시겠습니까?";

        #endregion

        #region Events

        public static event EventHandler<OpenItemQnaEventArgs> OpenItemQnaPopupEvent;
        public static event EventHandler<SkipItemQnaEventArgs> SkipItemQnaEvent;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            npcCanvasGroup.Visible(false);
            BoxSpeechBalloonController.OpenItemQnaEvent += MeetItem;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BoxSpeechBalloonController.OpenItemQnaEvent -= MeetItem;
        }

        #endregion

        #region Protected Methods

        protected override void OnClickOkBtn()
        {
            EmitOpenItemQnaPopupEvent(new OpenItemQnaEventArgs { item = item });
            base.OnClickOkBtn();
        }

        protected override void ResetAll()
        {
            base.ResetAll();
            npcCanvasGroup.Visible(false);
            npcCommentText.text = "";
            okButton.SetActive(false);
            cancelButton.SetActive(false);
        }
        
        protected override void ReactivateSpeechBalloon()
        {
            base.ReactivateSpeechBalloon();
            EmitSkipItemQnaEvent(new SkipItemQnaEventArgs { item = item });
        }

        #endregion

        #region Private Methods

        private void MeetItem(object _, OpenItemQnaEventArgs e)
        {
            item = e.item;
            ActivateUI();
            StartCoroutine(BeforeQuiz());
        }

        private IEnumerator BeforeQuiz()
        {
            yield return DOFade(npcCanvasGroup);
            yield return TypingComment(npcCommentText, NpcComment);
            
            okButton.SetActive(true);
            cancelButton.SetActive(true);
        }

        private void EmitOpenItemQnaPopupEvent(OpenItemQnaEventArgs e)
        {
            if (OpenItemQnaPopupEvent == null) return;
            foreach (var invocation in OpenItemQnaPopupEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private void EmitSkipItemQnaEvent(SkipItemQnaEventArgs e)
        {
            if (SkipItemQnaEvent == null) return;
            foreach (var invocation in SkipItemQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        #endregion
    }
}
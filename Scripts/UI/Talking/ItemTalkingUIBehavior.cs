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
        [SerializeField] private CanvasGroup npcCanvasGroup;
        [SerializeField] private Text npcCommentText;

        public static event EventHandler<OpenItemQnaEventArgs> OpenItemQnaPopupEvent;
        public static event EventHandler<SkipItemQnaEventArgs> SkipItemQnaEvent; 

        
        private readonly string npcComment = "아이템 얻기를 시도하시겠습니까?";

        private Item item;
        
        protected override void Start()
        {
            base.Start();
            npcCanvasGroup.Visible(false);
            BoxSpeechBalloonController.OpenItemQnaEvent += MeetItem;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            BoxSpeechBalloonController.OpenItemQnaEvent -= MeetItem;
        }

        private void MeetItem(object _, OpenItemQnaEventArgs e)
        {
            item = e.item;
            ActivateUI();
            StartCoroutine(BeforeQuiz());
        }
        
        private IEnumerator BeforeQuiz()
        {
            yield return FadeIn(npcCanvasGroup);
            yield return TypingComment(npcCommentText, npcComment);
            
            okButton.SetActive(true);
            cancelButton.SetActive(true);
        }

        protected override void OnClickOkBtn()
        {
            EmitOpenItemQnaPopupEvent(new OpenItemQnaEventArgs(item));
            base.OnClickOkBtn();
        }
        
        private void EmitOpenItemQnaPopupEvent(OpenItemQnaEventArgs e)
        {
            if (OpenItemQnaPopupEvent == null) return;
            foreach (var invocation in OpenItemQnaPopupEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
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
            EmitSkipItemQnaEvent(new SkipItemQnaEventArgs(item));
        }

        private void EmitSkipItemQnaEvent(SkipItemQnaEventArgs e)
        {
            if (SkipItemQnaEvent == null) return;
            foreach (var invocation in SkipItemQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
    }
}
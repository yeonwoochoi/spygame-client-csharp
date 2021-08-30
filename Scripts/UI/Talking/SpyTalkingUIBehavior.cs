using System;
using System.Collections;
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
    public class SpyTalkingUIBehavior: BaseTalkingUIBehavior
    {
        [SerializeField] private Text playerCommentText;
        [SerializeField] private Text spyCommentText;
        [SerializeField] private CanvasGroup playerCanvasGroup;
        [SerializeField] private CanvasGroup spyCanvasGroup;
        
        private Spy spy;
        
        private readonly string spyComment = "무슨 일이시죠?";
        private readonly string playerComment = "이 병사를 심문하시겠습니까?";
        
        public static event EventHandler<OpenSpyQnaEventArgs> OpenSpyQnaPopupEvent;
        public static event EventHandler<SkipSpyQnaEventArgs> SkipSpyQnaEvent;

        protected override void Start()
        {
            base.Start();
            spyCanvasGroup.Visible(false);
            playerCanvasGroup.Visible(false);
            SpySpeechBalloonController.OpenSpyQnaEvent += MeetSpy;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            SpySpeechBalloonController.OpenSpyQnaEvent -= MeetSpy;
        }
        
        private void MeetSpy(object _, OpenSpyQnaEventArgs e)
        {
            spy = e.spy;
            ActivateUI();
            StartCoroutine(BeforeExamineEvent());
        }

        private IEnumerator BeforeExamineEvent()
        {
            yield return FadeIn(spyCanvasGroup);
            yield return TypingComment(spyCommentText, spyComment);
            
            yield return FadeIn(playerCanvasGroup);
            yield return TypingComment(playerCommentText, playerComment);
            
            okButton.SetActive(true);
            cancelButton.SetActive(true);
        }
        
        protected override void OnClickOkBtn()
        {
            EmitOpenQnaPopupEvent(new OpenSpyQnaEventArgs(spy));
            base.OnClickOkBtn();
        }
        
        private void EmitOpenQnaPopupEvent(OpenSpyQnaEventArgs e)
        {
            if (OpenSpyQnaPopupEvent == null) return;
            foreach (var invocation in OpenSpyQnaPopupEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void EmitSkipItemQnaEvent(SkipSpyQnaEventArgs e)
        {
            if (SkipSpyQnaEvent == null) return;
            foreach (var invocation in SkipSpyQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        protected override void ResetAll()
        {
            base.ResetAll();
            playerCommentText.text = "";
            spyCommentText.text = "";
            okButton.SetActive(false);
            cancelButton.SetActive(false);
            spyCanvasGroup.Visible(false);
            playerCanvasGroup.Visible(false);
        }

        protected override void ReactivateSpeechBalloon()
        {
            base.ReactivateSpeechBalloon();
            EmitSkipItemQnaEvent(new SkipSpyQnaEventArgs(spy));
        }

    }
}
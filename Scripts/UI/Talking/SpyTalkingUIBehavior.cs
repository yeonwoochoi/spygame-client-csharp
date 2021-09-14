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
        #region Private Variables

        [SerializeField] private Text playerCommentText;
        [SerializeField] private Text spyCommentText;
        [SerializeField] private CanvasGroup playerCanvasGroup;
        [SerializeField] private CanvasGroup spyCanvasGroup;
        
        private Spy spy;

        #endregion

        #region Readonly Variables

        private const string SpyComment = "무슨 일이시죠?";
        private const string PlayerComment = "이 병사를 심문하시겠습니까?";

        #endregion

        #region Events

        public static event EventHandler<OpenSpyQnaEventArgs> OpenSpyQnaPopupEvent;
        public static event EventHandler<SkipSpyQnaEventArgs> SkipSpyQnaEvent;

        #endregion

        #region Event Methods

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

        #endregion

        #region Protected Methods

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

        protected override void OnClickOkBtn()
        {
            EmitOpenQnaPopupEvent(new OpenSpyQnaEventArgs { spy = spy });
            base.OnClickOkBtn();
        }

        protected override void ReactivateSpeechBalloon()
        {
            base.ReactivateSpeechBalloon();
            EmitSkipItemQnaEvent(new SkipSpyQnaEventArgs { spy = spy });
        }

        #endregion

        #region Private Methods

        private void MeetSpy(object _, OpenSpyQnaEventArgs e)
        {
            spy = e.spy;
            ActivateUI();
            StartCoroutine(BeforeExamineEvent());
        }

        private IEnumerator BeforeExamineEvent()
        {
            yield return DoFade(spyCanvasGroup);
            yield return TypingComment(spyCommentText, SpyComment);
            
            yield return DoFade(playerCanvasGroup);
            yield return TypingComment(playerCommentText, PlayerComment);
            
            okButton.SetActive(true);
            cancelButton.SetActive(true);
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

        #endregion
    }
}
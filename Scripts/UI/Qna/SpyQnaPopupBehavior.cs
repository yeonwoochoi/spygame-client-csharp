using System;
using System.Collections;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using StageScripts;
using UI.Base;
using UI.Talking;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Qna
{
    public enum CaptureSpyType
    {
        Capture, Release
    }
    
    public class SpyQnaPopupBehavior: BasePopupBehavior
    {
        public static string ANIMATION_VARIABLE_BOMB = "BombTrigger";
        public static string ANIMATION_VARIABLE_EXPLOSION = "ExplosionTrigger";

        [SerializeField] private Text playerQuestionText;
        [SerializeField] private Text spyAnswerText;
        [SerializeField] private Text spyOrNotText;
        [SerializeField] private GameObject captureBtn;
        [SerializeField] private GameObject releaseBtn;
        [SerializeField] private GameObject bombTimer;
        [SerializeField] private GameObject explosion;
        [SerializeField] private Text timerText;
        
        public static event EventHandler<CaptureSpyEventArgs> CaptureSpyEvent;
        public static event EventHandler<SkipSpyQnaEventArgs> SkipSpyQnaEvent;

        private Spy spy;
        
        private readonly string popupTitle = "심문 보고서";
        private readonly string spyOrNotQuestionComment = "이 병사를 포획하시겠습니까?";
        private readonly int timer = 3;
        
        private Animator bombTimerAnimator;
        private Animator explosionAnimator;
        private CanvasGroup explsionCanvasGroup;

        private bool isSolved;
        private bool IsSolved
        {
            get => isSolved;
            set
            {
                isSolved = value;
                if (isSolved) AudioManager.instance.Stop(SoundType.Timer);
            }
        }

        protected override void Start()
        {
            base.Start();
            SpyTalkingUIBehavior.OpenSpyQnaPopupEvent += OpenSpyQnaPopup;

            bombTimerAnimator = bombTimer.GetComponent<Animator>();
            explosionAnimator = explosion.GetComponent<Animator>();
            explsionCanvasGroup = explosion.GetComponent<CanvasGroup>();
            explsionCanvasGroup.Visible(false);

            captureBtn.GetComponent<Button>().onClick.AddListener(OnClickCaptureBtn);
            releaseBtn.GetComponent<Button>().onClick.AddListener(OnClickReleaseBtn);
            captureBtn.SetActive(false);
            releaseBtn.SetActive(false);

            titleText.text = popupTitle;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SpyTalkingUIBehavior.OpenSpyQnaPopupEvent -= OpenSpyQnaPopup;
        }

        private void OpenSpyQnaPopup(object _, OpenSpyQnaEventArgs e)
        {
            spy = e.spy;
            IsSolved = false;
            ResetAll();
            OnOpenPopup();
            StartCoroutine(TypingReportContent());
        }

        private IEnumerator TypingReportContent()
        {
            yield return new WaitForSeconds(0.5f);
            yield return TypingComment(playerQuestionText, $"Player : {spy.question}");
            yield return TypingComment(spyAnswerText, $"Soldier : {spy.answer}");
            yield return TypingComment(spyOrNotText, spyOrNotQuestionComment);
            
            captureBtn.SetActive(true);
            releaseBtn.SetActive(true);
            
            yield return StartTimer();
        }
        
        private void OnClickCaptureBtn()
        {
            OnClosePopup();
            IsSolved = true;
            EmitCaptureSpyEventArgs(new CaptureSpyEventArgs(spy, CaptureSpyType.Capture));
        }
        private void OnClickReleaseBtn()
        {
            OnClosePopup();
            IsSolved = true;
            EmitCaptureSpyEventArgs(new CaptureSpyEventArgs(spy, CaptureSpyType.Release));
        }

        private void EmitCaptureSpyEventArgs(CaptureSpyEventArgs e)
        {
            if (CaptureSpyEvent == null) return;
            foreach (var invocation in CaptureSpyEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private IEnumerator StartTimer()
        {
            var remainingTime = timer;
            bombTimerAnimator.SetBool(ANIMATION_VARIABLE_BOMB, true);
            while (remainingTime > 0)
            {
                if (IsSolved)
                {
                    OnClosePopup();
                    bombTimerAnimator.SetBool(ANIMATION_VARIABLE_BOMB, false);
                    explosionAnimator.SetBool(ANIMATION_VARIABLE_EXPLOSION, false);
                    AudioManager.instance.Stop(SoundType.Timer);
                    yield break;
                }
                timerText.text = $"{remainingTime}";
                AudioManager.instance.Play(SoundType.Timer);
                yield return new WaitForSeconds(1f);
                remainingTime--;
            }
            explsionCanvasGroup.Visible();
            AudioManager.instance.Stop(SoundType.Timer);
            explosionAnimator.SetBool(ANIMATION_VARIABLE_EXPLOSION, true);
            AudioManager.instance.Play(SoundType.Explosion);
            yield return new WaitForSeconds(0.8f);
            explsionCanvasGroup.Visible(false);
            OnClosePopup();
            bombTimerAnimator.SetBool(ANIMATION_VARIABLE_BOMB, false);
            explosionAnimator.SetBool(ANIMATION_VARIABLE_EXPLOSION, false);
        }
        
        protected override void ResetAll()
        {
            base.ResetAll();
            timerText.text = "";
            playerQuestionText.text = "";
            spyAnswerText.text = "";
            spyOrNotText.text = "";
            captureBtn.SetActive(false);
            releaseBtn.SetActive(false);
        }

        protected override void ReactivateSpeechBalloon()
        {
            base.ReactivateSpeechBalloon();
            EmitReactivateSpySpeechBalloonEvent(new SkipSpyQnaEventArgs(spy));
        }
        
        private void EmitReactivateSpySpeechBalloonEvent(SkipSpyQnaEventArgs e)
        {
            if (SkipSpyQnaEvent == null) return;
            foreach (var invocation in SkipSpyQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
    }
}
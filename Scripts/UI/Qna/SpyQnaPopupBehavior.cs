using System;
using System.Collections;
using Control.Pointer;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using Manager.Data;
using StageScripts;
using TutorialScripts;
using UI.Base;
using UI.StageScripts.Popup;
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
        #region Private Variables

        [SerializeField] private PointerUIController pointerUIController;
        [SerializeField] private Text playerQuestionText;
        [SerializeField] private Text spyAnswerText;
        [SerializeField] private GameObject captureBtn;
        [SerializeField] private GameObject releaseBtn;
        [SerializeField] private GameObject bombTimer;
        [SerializeField] private GameObject explosion;
        [SerializeField] private Text timerText;
        
        private Spy spy;
        private Animator bombTimerAnimator;
        private Animator explosionAnimator;
        private CanvasGroup explosionCanvasGroup;
        private bool isSolved;
        private bool isTutorial = false;

        #endregion

        #region Static Variables

        private static readonly int BombTrigger = Animator.StringToHash(AnimationBomb);
        private static readonly int ExplosionTrigger = Animator.StringToHash(AnimationExplosion);
        public const string AnimationBomb = "BombTrigger";
        public const string AnimationExplosion = "ExplosionTrigger";

        #endregion
        
        #region Readonly Variables

        private const string PopupTitle = "심문 보고서";
        private const int QuizTimer = 3;

        #endregion

        #region Events

        public static event EventHandler<CaptureSpyEventArgs> CaptureSpyEvent;
        public static event EventHandler<SkipSpyQnaEventArgs> SkipSpyQnaEvent;
        
        #endregion

        #region Setter

        private void SetIsSolved(bool flag)
        {
            isSolved = flag;
            if (isSolved) AudioManager.instance.Stop(SoundType.Timer);
        }

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            SpyTalkingUIBehavior.OpenSpyQnaPopupEvent += OpenSpyQnaPopup;

            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
            
            bombTimerAnimator = bombTimer.GetComponent<Animator>();
            explosionAnimator = explosion.GetComponent<Animator>();
            explosionCanvasGroup = explosion.GetComponent<CanvasGroup>();
            explosionCanvasGroup.Visible(false);

            captureBtn.GetComponent<Button>().onClick.AddListener(OnClickCaptureBtn);
            releaseBtn.GetComponent<Button>().onClick.AddListener(OnClickReleaseBtn);
            captureBtn.SetActive(false);
            releaseBtn.SetActive(false);

            titleText.text = PopupTitle;
            
            if (isTutorial)
            {
                pointerUIController.Init();    
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SpyTalkingUIBehavior.OpenSpyQnaPopupEvent -= OpenSpyQnaPopup;
        }

        #endregion

        #region Protected Methods

        protected override void ResetAll()
        {
            base.ResetAll();
            timerText.text = "";
            playerQuestionText.text = "";
            spyAnswerText.text = "";
            captureBtn.SetActive(false);
            releaseBtn.SetActive(false);
        }

        protected override void ReactivateSpeechBalloon()
        {
            base.ReactivateSpeechBalloon();
            EmitReactivateSpySpeechBalloonEvent(new SkipSpyQnaEventArgs { spy = spy });
        }

        #endregion

        #region Private Methods

        private void OpenSpyQnaPopup(object _, OpenSpyQnaEventArgs e)
        {
            spy = e.spy;
            SetIsSolved(false);
            ResetAll();
            OnOpenPopup();
            StartCoroutine(TypingReportContent(e.spy.isSpy));
        }

        private IEnumerator TypingReportContent(bool isSpy)
        {
            yield return new WaitForSeconds(0.5f);
            yield return TypingComment(playerQuestionText, $"Q : {spy.GetQuestion()}");
            yield return TypingComment(spyAnswerText, $"A : {spy.GetAnswer()}");
            
            captureBtn.SetActive(true);
            releaseBtn.SetActive(true);

            if (isTutorial)
            {
                pointerUIController.StartPointing(isSpy ? captureBtn.GetComponent<RectTransform>() : releaseBtn.GetComponent<RectTransform>());   
            }

            yield return StartTimer();
        }

        private void OnClickCaptureBtn()
        {
            OnClosePopup();
            SetIsSolved(true);
            EmitCaptureSpyEventArgs(new CaptureSpyEventArgs(spy, CaptureSpyType.Capture));

            if (isTutorial) pointerUIController.EndPointing();
        }

        private void OnClickReleaseBtn()
        {
            OnClosePopup();
            SetIsSolved(true);
            EmitCaptureSpyEventArgs(new CaptureSpyEventArgs(spy, CaptureSpyType.Release));
            
            if (isTutorial) pointerUIController.EndPointing();
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
            var remainingTime = QuizTimer;
            bombTimerAnimator.SetBool(BombTrigger, true);
            while (remainingTime > 0)
            {
                if (isSolved)
                {
                    OnClosePopup();
                    bombTimerAnimator.SetBool(BombTrigger, false);
                    explosionAnimator.SetBool(ExplosionTrigger, false);
                    AudioManager.instance.Stop(SoundType.Timer);
                    yield break;
                }
                timerText.text = $"{remainingTime}";
                AudioManager.instance.Play(SoundType.Timer);
                yield return new WaitForSeconds(1f);
                remainingTime--;
            }
            explosionCanvasGroup.Visible();
            AudioManager.instance.Stop(SoundType.Timer);
            explosionAnimator.SetBool(ExplosionTrigger, true);
            AudioManager.instance.Play(SoundType.Explosion);
            yield return new WaitForSeconds(0.8f);
            explosionCanvasGroup.Visible(false);
            OnClosePopup();
            bombTimerAnimator.SetBool(BombTrigger, false);
            explosionAnimator.SetBool(ExplosionTrigger, false);
        }

        private void EmitReactivateSpySpeechBalloonEvent(SkipSpyQnaEventArgs e)
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
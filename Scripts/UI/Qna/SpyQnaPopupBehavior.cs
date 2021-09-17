using System;
using System.Collections;
using Control.Pointer;
using Control.SpeechBalloon;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using Manager.Data;
using UI.Base;
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
        [SerializeField] private GameObject wrongButton;
        [SerializeField] private GameObject correctButton;
        [SerializeField] private GameObject bombTimer;
        [SerializeField] private GameObject explosion;
        [SerializeField] private Text timerText;
        
        private Spy spy;
        private Animator bombTimerAnimator;
        private Animator explosionAnimator;
        private CanvasGroup explosionCanvasGroup;
        private bool isSolved;

        #endregion

        #region Static Variables

        private static readonly int BombTrigger = Animator.StringToHash(AnimationBomb);
        private static readonly int ExplosionTrigger = Animator.StringToHash(AnimationExplosion);
        public const string AnimationBomb = "BombTrigger";
        public const string AnimationExplosion = "ExplosionTrigger";

        #endregion
        
        #region Readonly Variables

        private const string PopupTitle = "심문 보고서";
        private const int QuizTimer = 10;

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
            SpySpeechBalloonController.OpenSpyQnaPopupEvent += OpenSpyQnaPopup;
            
            bombTimerAnimator = bombTimer.GetComponent<Animator>();
            explosionAnimator = explosion.GetComponent<Animator>();
            explosionCanvasGroup = explosion.GetComponent<CanvasGroup>();
            explosionCanvasGroup.Visible(false);

            wrongButton.GetComponent<Button>().onClick.AddListener(OnClickWrongBtn);
            correctButton.GetComponent<Button>().onClick.AddListener(OnClickCorrectBtn);
            wrongButton.SetActive(false);
            correctButton.SetActive(false);

            titleText.text = PopupTitle;
            
            if (isTutorial)
            {
                pointerUIController.Init();    
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SpySpeechBalloonController.OpenSpyQnaPopupEvent -= OpenSpyQnaPopup;
        }

        #endregion

        #region Protected Methods

        protected override void ResetAll()
        {
            base.ResetAll();
            timerText.text = "";
            playerQuestionText.text = "";
            spyAnswerText.text = "";
            wrongButton.SetActive(false);
            correctButton.SetActive(false);
        }

        protected override void ReactivateSpeechBalloon()
        {
            base.ReactivateSpeechBalloon();
            EmitReactivateSpySpeechBalloonEvent(new SkipSpyQnaEventArgs { spy = spy });
        }

        #endregion

        #region Private Methods

        private void OpenSpyQnaPopup(object _, OpenSpyQnaPopupEventArgs e)
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
            playerQuestionText.text = $"Q : {spy.GetQuestion()}";
            spyAnswerText.text = $"A : {spy.GetAnswer()}";
            
            wrongButton.SetActive(true);
            correctButton.SetActive(true);

            if (isTutorial)
            {
                pointerUIController.StartPointing(isSpy ? wrongButton.GetComponent<RectTransform>() : correctButton.GetComponent<RectTransform>());   
            }

            yield return StartTimer();
        }

        private void OnClickWrongBtn()
        {
            OnClosePopup();
            SetIsSolved(true);
            EmitCaptureSpyEventArgs(new CaptureSpyEventArgs(spy, CaptureSpyType.Capture));

            if (isTutorial) pointerUIController.EndPointing();
        }

        private void OnClickCorrectBtn()
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
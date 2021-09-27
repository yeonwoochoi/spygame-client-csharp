using System;
using System.Collections;
using Control.Pointer;
using Control.SpeechBalloon;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using StageScripts;
using UI.Base;
using UI.Effect;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Popup.Qna
{
    public enum ItemGetType
    {
        Get, Miss
    }
    
    public class ItemQnaPopupBehavior: BasePopupBehavior
    {
        #region Private Variables

        [SerializeField] private PointerUIController pointerUIController;
        [SerializeField] private Text questionText;
        [SerializeField] private Text answerText;
        [SerializeField] private GameObject yesButton;
        [SerializeField] private GameObject noButton;
        [SerializeField] private GameObject bombTimer;
        [SerializeField] private GameObject explosion; 
        [SerializeField] private Text timerText;

        private Item item;

        private readonly string popupTitle = "Quiz";
        private readonly int timer = 10;

        private Animator bombTimerAnimator;
        private Animator explosionAnimator;
        private CanvasGroup explosionCanvasGroup;
        private Coroutine reportCoroutine;

        private bool isSolved;
        private bool isShowingResult;

        #endregion

        #region Events

        public static event EventHandler<ItemGetEventArgs> ItemGetEvent;
        public static event EventHandler<SkipItemQnaEventArgs> SkipItemQnaEvent;
        public static event EventHandler<PlayQnaGradingAnimEventArgs> PlayQnaGradingAnimEvent;

        #endregion

        #region Setter

        private void IsSolved(bool flag)
        {
            isSolved = flag;
            if (isSolved) AudioManager.instance.Stop(SoundType.Timer);
        }

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            
            titleText.text = $"{popupTitle}";
            BoxSpeechBalloonController.OpenItemQnaPopupEvent += OpenItemQnaPopup;
            StageStateController.StageDoneEvent += FinishStage;

            bombTimerAnimator = bombTimer.GetComponent<Animator>();
            explosionAnimator = explosion.GetComponent<Animator>();
            explosionCanvasGroup = explosion.GetComponent<CanvasGroup>();
            explosionCanvasGroup.Visible(false);
            
            yesButton.GetComponent<Button>().onClick.AddListener(OnClickYesBtn);
            noButton.GetComponent<Button>().onClick.AddListener(OnClickNoBtn);

            isShowingResult = false;

            if (isTutorial)
            {
                pointerUIController.Init();    
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BoxSpeechBalloonController.OpenItemQnaPopupEvent -= OpenItemQnaPopup;
            StageStateController.StageDoneEvent -= FinishStage;
        }

        #endregion

        #region Protected Methods

        protected override void ResetAll()
        {
            base.ResetAll();
            timerText.text = "";
            questionText.text = "";
            answerText.text = "";
            yesButton.SetActive(false);
            noButton.SetActive(false);
        }
        
        protected override void ReactivateSpeechBalloon()
        {
            base.ReactivateSpeechBalloon();
            EmitReactivateItemSpeechBalloonEvent(new SkipItemQnaEventArgs { item = item });
        }

        #endregion

        #region Private Methods

        private void OpenItemQnaPopup(object _, OpenItemQnaEventArgs e)
        {
            item = e.item;
            IsSolved(false);
            ResetAll();
            OnOpenPopup();
            if (reportCoroutine != null) StopCoroutine(reportCoroutine);
            reportCoroutine = StartCoroutine(TypingReportContent(e.item.isCorrect));
        }

        private IEnumerator TypingReportContent(bool isCorrect)
        {
            yield return new WaitForSeconds(0.5f);
            questionText.text = $"Q : {item.GetQuestion()}";
            answerText.text = $"A : {item.GetAnswer()}";

            yesButton.SetActive(true);
            noButton.SetActive(true);
            
            if (isTutorial)
            {
                pointerUIController.StartPointing(isCorrect ? yesButton.GetComponent<RectTransform>() : noButton.GetComponent<RectTransform>());   
            }

            yield return StartTimer();
        }
        
        private void OnClickYesBtn()
        {
            if (isShowingResult) return;
            isShowingResult = true;
            ShowQnaResult(item.isCorrect, true);
        }

        private void OnClickNoBtn()
        {
            if (isShowingResult) return;
            isShowingResult = true;
            ShowQnaResult(!item.isCorrect, false);
        }
        
        private void ShowQnaResult(bool isCorrect, bool isClickCorrectBtn)
        {
            IsSolved(true);

            EmitPlayQnaGradingAnimEvent(new PlayQnaGradingAnimEventArgs
            {
                isCorrect = isCorrect,
                isClickCorrectBtn = isClickCorrectBtn,
                callback = ShowQnaResultCallback
            });
        }
        
        private void ShowQnaResultCallback(bool isClickCorrectBtn)
        {
            OnClosePopup();
            
            var case1 = isClickCorrectBtn && item.isCorrect;
            var case2 = !isClickCorrectBtn && !item.isCorrect;

            var isCorrect = case1 || case2;
            
            EmitItemGetEvent(isCorrect
                ? new ItemGetEventArgs(item, ItemGetType.Get)
                : new ItemGetEventArgs(item, ItemGetType.Miss));        
            
            if (isTutorial) pointerUIController.EndPointing();
            isShowingResult = false;
        }

        private IEnumerator StartTimer()
        {
            var remainingTime = timer;
            bombTimerAnimator.SetBool(SpyQnaPopupBehavior.AnimationBomb, true);
            yield return new WaitForSeconds(0.4f);
            while (remainingTime > 0)
            {
                if (isSolved)
                {
                    OnClosePopup();
                    bombTimerAnimator.SetBool(SpyQnaPopupBehavior.AnimationBomb, false);
                    explosionAnimator.SetBool(SpyQnaPopupBehavior.AnimationExplosion, false);
                    AudioManager.instance.Stop(SoundType.Timer);
                    yield break;
                }
                timerText.text = $"{remainingTime}";
                AudioManager.instance.Play(SoundType.Timer);
                yield return new WaitForSeconds(1f);
                remainingTime--;
            }
            AudioManager.instance.Stop(SoundType.Timer);
            yield return StartCoroutine(StartBomb());
        }

        private IEnumerator StartBomb()
        {
            explosionCanvasGroup.Visible();
            explosionAnimator.SetBool(SpyQnaPopupBehavior.AnimationExplosion, true);
            if (!isSolved) AudioManager.instance.Play(SoundType.Explosion);
            yield return new WaitForSeconds(0.8f);
            explosionCanvasGroup.Visible(false);
            OnClosePopup();
            bombTimerAnimator.SetBool(SpyQnaPopupBehavior.AnimationBomb, false);
            explosionAnimator.SetBool(SpyQnaPopupBehavior.AnimationExplosion, false);
        }

        private void FinishStage(object _, ExitStageEventArgs e)
        {
            if (reportCoroutine != null)
            {
                StopCoroutine(reportCoroutine);
            }

            if (isOpen)
            {
                OnClosePopup();
            }
        }

        private void EmitItemGetEvent(ItemGetEventArgs e)
        {
            if (ItemGetEvent == null) return;
            foreach (var invocation in ItemGetEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private void EmitReactivateItemSpeechBalloonEvent(SkipItemQnaEventArgs e)
        {
            if (SkipItemQnaEvent == null) return;
            foreach (var invocation in SkipItemQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void EmitPlayQnaGradingAnimEvent(PlayQnaGradingAnimEventArgs e)
        {
            if (PlayQnaGradingAnimEvent == null) return;
            foreach (var invocation in PlayQnaGradingAnimEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }
        #endregion
    }
}
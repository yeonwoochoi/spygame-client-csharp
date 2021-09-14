using System;
using System.Collections;
using Control.Pointer;
using Domain;
using Event;
using Manager;
using UI.Base;
using UI.Talking;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Domain.StageObj;
using Manager.Data;
using TutorialScripts;
using UI.StageScripts.Popup;
using Util;

namespace UI.Qna
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
        [SerializeField] private Text correctOrNotText;
        [SerializeField] private GameObject yesButton;
        [SerializeField] private GameObject noButton;
        [SerializeField] private GameObject bombTimer;
        [SerializeField] private GameObject explosion; 
        [SerializeField] private Text timerText;

        private Item item;

        private readonly string correctOrNotComment = "Is it Correct?";
        private readonly string popupTitle = "Quiz";
        private readonly int timer = 3;

        private Animator bombTimerAnimator;
        private Animator explosionAnimator;
        private CanvasGroup explosionCanvasGroup;

        private bool isSolved;
        private bool isTutorial = false;

        #endregion

        #region Events

        public static event EventHandler<ItemGetEventArgs> ItemGetEvent;
        public static event EventHandler<SkipItemQnaEventArgs> SkipItemQnaEvent;

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
            
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);

            titleText.text = $"{popupTitle}";
            ItemTalkingUIBehavior.OpenItemQnaPopupEvent += OpenItemQnaPopup;

            bombTimerAnimator = bombTimer.GetComponent<Animator>();
            explosionAnimator = explosion.GetComponent<Animator>();
            explosionCanvasGroup = explosion.GetComponent<CanvasGroup>();
            explosionCanvasGroup.Visible(false);
            
            yesButton.GetComponent<Button>().onClick.AddListener(OnClickYesBtn);
            noButton.GetComponent<Button>().onClick.AddListener(OnClickNoBtn);
            
            if (isTutorial)
            {
                pointerUIController.Init();    
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ItemTalkingUIBehavior.OpenItemQnaPopupEvent -= OpenItemQnaPopup;
        }

        #endregion

        #region Protected Methods

        protected override void ResetAll()
        {
            base.ResetAll();
            timerText.text = "";
            questionText.text = "";
            answerText.text = "";
            correctOrNotText.text = "";
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
            SetIsSolved(false);
            ResetAll();
            OnOpenPopup();
            StartCoroutine(TypingReportContent(e.item.isCorrect));
        }

        private IEnumerator TypingReportContent(bool isCorrect)
        {
            yield return new WaitForSeconds(0.5f);
            yield return TypingComment(questionText, $"Q : {item.GetQuestion()}");
            yield return TypingComment(answerText, $"A : {item.GetAnswer()}");
            yield return TypingComment(correctOrNotText, correctOrNotComment);
            
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
            OnClosePopup();
            SetIsSolved(true);
            EmitItemGetEvent(item.isCorrect
                ? new ItemGetEventArgs(item, ItemGetType.Get)
                : new ItemGetEventArgs(item, ItemGetType.Miss));

            if (isTutorial) pointerUIController.EndPointing();
        }

        private void OnClickNoBtn()
        {
            OnClosePopup();
            SetIsSolved(true);
            EmitItemGetEvent(item.isCorrect
                ? new ItemGetEventArgs(item, ItemGetType.Miss)
                : new ItemGetEventArgs(item, ItemGetType.Get));
            
            if (isTutorial) pointerUIController.EndPointing();
        }

        private IEnumerator StartTimer()
        {
            var remainingTime = timer;
            bombTimerAnimator.SetBool(SpyQnaPopupBehavior.AnimationBomb, true);
            while (remainingTime > 0)
            {
                while (isPaused) yield return null;
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
            explosionCanvasGroup.Visible();
            explosionAnimator.SetBool(SpyQnaPopupBehavior.AnimationExplosion, true);
            AudioManager.instance.Play(SoundType.Explosion);
            yield return new WaitForSeconds(0.8f);
            explosionCanvasGroup.Visible(false);
            OnClosePopup();
            bombTimerAnimator.SetBool(SpyQnaPopupBehavior.AnimationBomb, false);
            explosionAnimator.SetBool(SpyQnaPopupBehavior.AnimationExplosion, false);
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
        #endregion
    }
}
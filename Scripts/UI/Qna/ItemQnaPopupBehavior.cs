﻿using System;
using System.Collections;
using Domain;
using Event;
using Manager;
using UI.Base;
using UI.Talking;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Domain.StageObj;
using Util;

namespace UI.Qna
{
    public enum ItemGetType
    {
        Get, Miss
    }
    
    public class ItemQnaPopupBehavior: BasePopupBehavior
    {
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
        
        public static event EventHandler<ItemGetEventArgs> ItemGetEvent;
        public static event EventHandler<SkipItemQnaEventArgs> SkipItemQnaEvent;

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
            titleText.text = $"{popupTitle}";
            ItemTalkingUIBehavior.OpenItemQnaPopupEvent += OpenItemQnaPopup;
            
            bombTimerAnimator = bombTimer.GetComponent<Animator>();
            explosionAnimator = explosion.GetComponent<Animator>();
            explosionCanvasGroup = explosion.GetComponent<CanvasGroup>();
            explosionCanvasGroup.Visible(false);
            
            yesButton.GetComponent<Button>().onClick.AddListener(OnClickYesBtn);
            noButton.GetComponent<Button>().onClick.AddListener(OnClickNoBtn);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ItemTalkingUIBehavior.OpenItemQnaPopupEvent -= OpenItemQnaPopup;
        }

        private void OpenItemQnaPopup(object _, OpenItemQnaEventArgs e)
        {
            item = e.item;
            IsSolved = false;
            ResetAll();
            OnOpenPopup();
            StartCoroutine(TypingReportContent());
        }
        
        private IEnumerator TypingReportContent()
        {
            yield return new WaitForSeconds(0.5f);
            yield return TypingComment(questionText, $"Q : {item.question}");
            yield return TypingComment(answerText, $"A : {item.answer}");
            yield return TypingComment(correctOrNotText, correctOrNotComment);
            
            yesButton.SetActive(true);
            noButton.SetActive(true);
            
            yield return StartTimer();
        }
        
        private void OnClickYesBtn()
        {
            OnClosePopup();
            IsSolved = true;
            EmitItemGetEvent(item.isCorrect
                ? new ItemGetEventArgs(item, ItemGetType.Get)
                : new ItemGetEventArgs(item, ItemGetType.Miss));
        }
        
        private void OnClickNoBtn()
        {
            OnClosePopup();
            IsSolved = true;
            EmitItemGetEvent(item.isCorrect
                ? new ItemGetEventArgs(item, ItemGetType.Miss)
                : new ItemGetEventArgs(item, ItemGetType.Get));
        }
        
        private IEnumerator StartTimer()
        {
            var remainingTime = timer;
            bombTimerAnimator.SetBool(SpyQnaPopupBehavior.ANIMATION_VARIABLE_BOMB, true);
            while (remainingTime > 0)
            {
                if (IsSolved)
                {
                    OnClosePopup();
                    bombTimerAnimator.SetBool(SpyQnaPopupBehavior.ANIMATION_VARIABLE_BOMB, false);
                    explosionAnimator.SetBool(SpyQnaPopupBehavior.ANIMATION_VARIABLE_EXPLOSION, false);
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
            explosionAnimator.SetBool(SpyQnaPopupBehavior.ANIMATION_VARIABLE_EXPLOSION, true);
            AudioManager.instance.Play(SoundType.Explosion);
            yield return new WaitForSeconds(0.8f);
            explosionCanvasGroup.Visible(false);
            OnClosePopup();
            bombTimerAnimator.SetBool(SpyQnaPopupBehavior.ANIMATION_VARIABLE_BOMB, false);
            explosionAnimator.SetBool(SpyQnaPopupBehavior.ANIMATION_VARIABLE_EXPLOSION, false);
        }

        private void EmitItemGetEvent(ItemGetEventArgs e)
        {
            if (ItemGetEvent == null) return;
            foreach (var invocation in ItemGetEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
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
            EmitReactivateItemSpeechBalloonEvent(new SkipItemQnaEventArgs(item));
        }
        
        private void EmitReactivateItemSpeechBalloonEvent(SkipItemQnaEventArgs e)
        {
            if (SkipItemQnaEvent == null) return;
            foreach (var invocation in SkipItemQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
    }
}
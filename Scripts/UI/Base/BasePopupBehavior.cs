using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Util;
using DG.Tweening;

namespace UI.Base
{
    public enum PopupTweenType
    {
        BottomToTop, TopToBottom, RightToLeft, LeftToRight, ScaleUp
    }

    public class BasePopupBehavior: BaseUIBehavior
    {
        [SerializeField] protected Text titleText;
        [SerializeField] protected PopupTweenType tweenType;
        private RectTransform popupUITransform;
        
        protected Vector3 initPosition;
        protected float moveSpeed = 10f;
        private int initPositionValue = 2000;

        protected override void Start()
        {
            base.Start();
            popupUITransform = GetComponent<RectTransform>();

            
            switch (tweenType)
            {
                case PopupTweenType.BottomToTop:
                    initPosition = new Vector3(0, -initPositionValue, 0);
                    break;
                case PopupTweenType.TopToBottom:
                    initPosition = new Vector3(0, initPositionValue, 0);
                    break;
                case PopupTweenType.RightToLeft:
                    initPosition = new Vector3(initPositionValue, 0, 0);
                    break;
                case PopupTweenType.LeftToRight:
                    initPosition = new Vector3(-initPositionValue, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            popupUITransform.localPosition = initPosition;

            
            
            /*
            initPosition = new Vector3(960, 540, 0);
            
            switch (tweenType)
            {
                case PopupTweenType.ScaleUp:
                    break;
                case PopupTweenType.BottomToTop:
                    initPosition += new Vector3(0, -initPositionValue, 0);
                    break;
                case PopupTweenType.TopToBottom:
                    initPosition += new Vector3(0, initPositionValue, 0);
                    break;
                case PopupTweenType.LeftToRight:    
                    initPosition += new Vector3(-initPositionValue, 0, 0);
                    break;
                case PopupTweenType.RightToLeft:
                    initPosition += new Vector3(initPositionValue, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            popupUITransform.position = initPosition;
            */
            
            isOpen = false;
        }

        protected void OnOpenPopup()
        {
            if (isOpen) return;
            isOpen = true;
            StartCoroutine(OpenPopup());
        }

        private IEnumerator OpenPopup()
        {
            ActivateUI();
            while (isOpen)
            {
                if (tweenType == PopupTweenType.TopToBottom || tweenType == PopupTweenType.BottomToTop)
                {
                    popupUITransform.localPosition = new Vector3(0, Mathf.Lerp(popupUITransform.localPosition.y, 0, Time.deltaTime * moveSpeed), 0);
                    // DoTweenByType(false);
                }
                else
                {
                    popupUITransform.localPosition = new Vector3(Mathf.Lerp(popupUITransform.localPosition.x, 0, Time.deltaTime * moveSpeed), 0, 0);
                    // DoTweenByType(false);
                }
                
                yield return null;

                if (Vector3.Distance(popupUITransform.position, Vector3.zero) <= 0.001f)
                {
                    yield break;
                }
            }
        } 
        
        protected void OnClosePopup()
        {
            if (!isOpen) return;
            isOpen = false;
            StartCoroutine(ClosePopup());
            ReactivateSpeechBalloon();
        }

        private IEnumerator ClosePopup()
        {
            ActivateUI(false);
            while (!isOpen)
            {
                if (tweenType == PopupTweenType.TopToBottom || tweenType == PopupTweenType.BottomToTop)
                {
                    popupUITransform.localPosition = new Vector3(0, Mathf.Lerp(popupUITransform.localPosition.y, initPosition.y, Time.deltaTime * moveSpeed), 0);
                    //DoTweenByType(true);
                }
                else
                {
                    popupUITransform.localPosition = new Vector3(Mathf.Lerp(popupUITransform.localPosition.x, initPosition.x, Time.deltaTime * moveSpeed), 0, 0);
                    //DoTweenByType(true);
                }
                
                yield return null;
                if (!(Vector3.Distance(popupUITransform.position, initPosition) <= 0.001f)) continue;
                ResetAll();
                yield break;
            }
        }
        
        private void DoTweenByType(bool isReverse) {
            var openEase = Ease.Linear;
            var closeEase = Ease.Linear;
            switch (tweenType) {
                case PopupTweenType.ScaleUp:
                    if (!isReverse)
                        DOTween.Sequence().Append(popupUITransform.DOScale(new Vector3(1.1f, 1.1f), 0.2f).SetEase(Ease.InQuart))
                            .Append(popupUITransform.DOScale(Vector3.one, 0.2f));
                    break;
                case PopupTweenType.TopToBottom:
                    if (isReverse)
                        popupUITransform.DOAnchorPosY(initPositionValue, 0.25f).SetEase(closeEase);
                    else
                        popupUITransform.DOAnchorPosY(0, 0.5f).SetEase(openEase);
                    break;
                case PopupTweenType.BottomToTop:
                    if (isReverse)
                        popupUITransform.DOAnchorPosY(-initPositionValue, 0.25f).SetEase(closeEase);
                    else
                        popupUITransform.DOAnchorPosY(0, 0.5f).SetEase(openEase);
                    break;
                case PopupTweenType.LeftToRight:
                    if (isReverse)
                        popupUITransform.DOAnchorPosX(-initPositionValue, 0.25f).SetEase(closeEase);
                    else
                        popupUITransform.DOAnchorPosX(0, 0.25f).SetEase(openEase);
                    break;
                case PopupTweenType.RightToLeft:
                    if (isReverse)
                        popupUITransform.DOAnchorPosX(initPositionValue, 0.25f).SetEase(closeEase);
                    else
                        popupUITransform.DOAnchorPosX(0, 0.25f).SetEase(openEase);
                    break;
            }
        }
        
        protected virtual void ResetAll() {}
        
        protected virtual void ReactivateSpeechBalloon() {}
    }
}
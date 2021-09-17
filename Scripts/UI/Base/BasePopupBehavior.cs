using System;
using System.Collections;
using Control.Pointer;
using UnityEngine;
using UnityEngine.UI;
using Util;
using DG.Tweening;
using Manager;
using Manager.Data;

namespace UI.Base
{
    #region Enum

    public enum PopupMoveType
    {
        BottomToTop, TopToBottom, RightToLeft, LeftToRight
    }

    #endregion

    public abstract class BasePopupBehavior: BaseUIBehavior
    {
        #region Protected Variables

        [SerializeField] protected Text titleText;
        [SerializeField] protected PopupMoveType moveType;
        protected bool isTutorial;

        #endregion

        #region Private Variables

        private RectTransform popupUITransform;
        private Vector3 initPosition;
        private float moveSpeed = 10f;
        private int initPositionValue = 2000;

        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
            
            popupUITransform = GetComponent<RectTransform>();

            switch (moveType)
            {
                case PopupMoveType.BottomToTop:
                    initPosition = new Vector3(0, -initPositionValue, 0);
                    break;
                case PopupMoveType.TopToBottom:
                    initPosition = new Vector3(0, initPositionValue, 0);
                    break;
                case PopupMoveType.RightToLeft:
                    initPosition = new Vector3(initPositionValue, 0, 0);
                    break;
                case PopupMoveType.LeftToRight:
                    initPosition = new Vector3(-initPositionValue, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            popupUITransform.localPosition = initPosition;

            isOpen = false;
        }

        #endregion

        #region Protected Method

        protected void OnOpenPopup()
        {
            if (isOpen) return;
            isOpen = true;
            PauseGame();
            StartCoroutine(OpenPopup());
        }

        protected void OnClosePopup()
        {
            if (!isOpen) return;
            isOpen = false;
            ContinueGame();
            StartCoroutine(ClosePopup());
            ReactivateSpeechBalloon();
        }

        protected virtual void ResetAll() {}
        
        protected virtual void ReactivateSpeechBalloon() {}

        #endregion

        #region Private Methods
        
        private void PauseGame()
        {
            if (isTutorial)
            {
                GlobalTutorialManager.Instance.PauseGame();
            }
            else
            {
                GlobalStageManager.Instance.PauseGame();
            } 
        }

        private void ContinueGame()
        {
            if (isTutorial)
            {
                GlobalTutorialManager.Instance.ContinueGame();
            }
            else
            {
                GlobalStageManager.Instance.ContinueGame();
            }
        }

        private IEnumerator OpenPopup()
        {
            ActivateUI();
            while (isOpen)
            {
                if (moveType == PopupMoveType.TopToBottom || moveType == PopupMoveType.BottomToTop)
                {
                    popupUITransform.localPosition = new Vector3(0, Mathf.Lerp(popupUITransform.localPosition.y, 0, Time.deltaTime * moveSpeed), 0);
                }
                else
                {
                    popupUITransform.localPosition = new Vector3(Mathf.Lerp(popupUITransform.localPosition.x, 0, Time.deltaTime * moveSpeed), 0, 0);
                }
                
                yield return null;

                if (Vector3.Distance(popupUITransform.position, Vector3.zero) <= 0.001f)
                {
                    yield break;
                }
            }
        }

        private IEnumerator ClosePopup()
        {
            ActivateUI(false);
            while (!isOpen)
            {
                if (moveType == PopupMoveType.TopToBottom || moveType == PopupMoveType.BottomToTop)
                {
                    popupUITransform.localPosition = new Vector3(0, Mathf.Lerp(popupUITransform.localPosition.y, initPosition.y, Time.deltaTime * moveSpeed), 0);
                }
                else
                {
                    popupUITransform.localPosition = new Vector3(Mathf.Lerp(popupUITransform.localPosition.x, initPosition.x, Time.deltaTime * moveSpeed), 0, 0);
                }
                
                yield return null;
                if (!(Vector3.Distance(popupUITransform.position, initPosition) <= 0.001f)) continue;
                ResetAll();
                yield break;
            }
        }

        #endregion
    }
}
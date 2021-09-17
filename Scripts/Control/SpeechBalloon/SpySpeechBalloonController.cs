using System;
using Base;
using Domain.StageObj;
using Event;
using UI.Qna;
using UnityEngine;

namespace Control.SpeechBalloon
{
    public class SpySpeechBalloonController: BaseSpeechBalloonController
    {
        #region Public Variable

        public Spy spy;

        #endregion

        #region Event

        public static event EventHandler<OpenSpyQnaPopupEventArgs> OpenSpyQnaPopupEvent;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            SpyQnaPopupBehavior.SkipSpyQnaEvent += SkipSpyQna;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SpyQnaPopupBehavior.SkipSpyQnaEvent -= SkipSpyQna;
        }

        #endregion

        #region Public Method

        public void OpenSpyQnaPopup(OpenSpyQnaPopupEventArgs e)
        {
            EmitOpenSpyQnaPopupEvent(e);   
        }

        #endregion

        #region Protected Method

        // TODO(?)
        protected override void CheckValidHit(GameObject collider)
        {
            base.CheckValidHit(collider);
            if (collider.TryGetComponent(out SpySpeechBalloonController controller))
            {
                if (controller.spy == spy)
                {
                    clicked = true;
                    OpenSpyQnaPopup(new OpenSpyQnaPopupEventArgs { spy = spy });
                }
            }
        }

        #endregion

        #region Private Method

        private void EmitOpenSpyQnaPopupEvent(OpenSpyQnaPopupEventArgs e)
        {
            if (OpenSpyQnaPopupEvent == null) return;
            foreach (var invocation in OpenSpyQnaPopupEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private void SkipSpyQna(object _, SkipSpyQnaEventArgs e)
        {
            if (spy != e.spy) return;
            clicked = false;
        }

        #endregion
    }
}
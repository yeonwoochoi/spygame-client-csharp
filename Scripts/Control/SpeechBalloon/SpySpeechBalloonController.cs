using System;
using Control.Base;
using Domain;
using Domain.StageObj;
using Event;
using UI.Qna;
using UI.Talking;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Control.SpeechBalloon
{
    public class SpySpeechBalloonController: BaseSpeechBalloonController
    {
        #region Public Variable

        public Spy spy;

        #endregion

        #region Event

        public static event EventHandler<OpenSpyQnaEventArgs> OpenSpyQnaEvent;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            SpyQnaPopupBehavior.SkipSpyQnaEvent += SkipSpyQna;
            SpyTalkingUIBehavior.SkipSpyQnaEvent += SkipSpyQna;
        }
        
        private void OnDisable()
        {
            SpyQnaPopupBehavior.SkipSpyQnaEvent -= SkipSpyQna;
            SpyTalkingUIBehavior.SkipSpyQnaEvent -= SkipSpyQna;
        }

        #endregion

        #region Public Method

        public void EmitOpenSpyQnaEvent(OpenSpyQnaEventArgs e)
        {
            if (OpenSpyQnaEvent == null) return;
            foreach (var invocation in OpenSpyQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        #endregion

        #region Protected Method

        // TODO()
        protected override void CheckValidHit(GameObject collider)
        {
            base.CheckValidHit(collider);
            if (collider.TryGetComponent(out SpySpeechBalloonController controller))
            {
                if (controller.spy == spy)
                {
                    clicked = true;
                    EmitOpenSpyQnaEvent(new OpenSpyQnaEventArgs { spy = spy });
                }
            }
        }

        #endregion

        #region Private Method

        private void SkipSpyQna(object _, SkipSpyQnaEventArgs e)
        {
            if (spy != e.spy) return;
            clicked = false;
        }

        #endregion
    }
}
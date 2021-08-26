using System;
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
        public static event EventHandler<OpenSpyQnaEventArgs> OpenSpyQnaEvent;
        
        public Spy spy;

        protected override void Start()
        {
            base.Start();
            SpyQnaPopupBehavior.SkipSpyQnaEvent += SkipSpyQna;
            SpyTalkingUIBehavior.SkipSpyQnaEvent += SkipSpyQna;
        }
        
        private void OnDestroy()
        {
            SpyQnaPopupBehavior.SkipSpyQnaEvent -= SkipSpyQna;
            SpyTalkingUIBehavior.SkipSpyQnaEvent -= SkipSpyQna;
        }

        public void EmitOpenSpyQnaEvent(OpenSpyQnaEventArgs e)
        {
            if (OpenSpyQnaEvent == null) return;
            foreach (var invocation in OpenSpyQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private void SkipSpyQna(object _, SkipSpyQnaEventArgs e)
        {
            if (spy != e.spy) return;
            clicked = false;
        }
        
        protected override void CheckValidHit(GameObject collider)
        {
            base.CheckValidHit(collider);
            if (collider.TryGetComponent(out SpySpeechBalloonController controller))
            {
                if (controller.spy == spy)
                {
                    clicked = true;

                    EmitOpenSpyQnaEvent(new OpenSpyQnaEventArgs(spy));
                }
            }
        }
    }
}
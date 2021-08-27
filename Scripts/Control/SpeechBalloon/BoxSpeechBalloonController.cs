using System;
using Control.Movement;
using Domain;
using Event;
using UI.Qna;
using UI.Talking;
using UnityEngine;
using UnityEngine.UIElements;

namespace Control.SpeechBalloon
{
    public class BoxSpeechBalloonController : BaseSpeechBalloonController
    {
        public static event EventHandler<OpenItemQnaEventArgs> OpenItemQnaEvent;

        public Domain.StageObj.Item item;

        protected override void Start()
        {
            base.Start();
            ItemQnaPopupBehavior.SkipItemQnaEvent += SkipQna;
            ItemTalkingUIBehavior.SkipItemQnaEvent += SkipQna;
        }

        // TODO()
        protected void OnDestroy()
        {
            ItemQnaPopupBehavior.SkipItemQnaEvent -= SkipQna;
            ItemTalkingUIBehavior.SkipItemQnaEvent -= SkipQna;
        }
        
        
        public void EmitOpenItemQnaEvent(OpenItemQnaEventArgs e)
        {
            if (OpenItemQnaEvent == null) return;
            foreach (var invocation in OpenItemQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void SkipQna(object _, SkipItemQnaEventArgs e)
        {
            if (item != e.item) return;
            clicked = false;
        }
        
        protected override void CheckValidHit(GameObject collider)
        {
            base.CheckValidHit(collider);
            if (collider.TryGetComponent(out BoxSpeechBalloonController controller))
            {
                if (controller.item == item)
                {
                    clicked = true;
                    EmitOpenItemQnaEvent(new OpenItemQnaEventArgs(item));
                }
            }
        }
    }
}
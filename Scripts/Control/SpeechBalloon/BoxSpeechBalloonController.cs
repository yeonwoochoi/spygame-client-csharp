using System;
using Base;
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
        #region Public Variable

        public Domain.StageObj.Item item;

        #endregion

        #region Event
        public static event EventHandler<OpenItemQnaEventArgs> OpenItemQnaEvent;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            ItemQnaPopupBehavior.SkipItemQnaEvent += SkipQna;
            ItemTalkingUIBehavior.SkipItemQnaEvent += SkipQna;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            ItemQnaPopupBehavior.SkipItemQnaEvent -= SkipQna;
            ItemTalkingUIBehavior.SkipItemQnaEvent -= SkipQna;
        }

        #endregion

        #region Public Method

        public void EmitOpenItemQnaEvent(OpenItemQnaEventArgs e)
        {
            if (OpenItemQnaEvent == null) return;
            foreach (var invocation in OpenItemQnaEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        #endregion

        #region Protected Method

        protected override void CheckValidHit(GameObject collider)
        {
            base.CheckValidHit(collider);
            if (collider.TryGetComponent(out BoxSpeechBalloonController controller))
            {
                if (controller.item == item)
                {
                    clicked = true;
                    EmitOpenItemQnaEvent(new OpenItemQnaEventArgs { item = item });
                }
            }
        }

        #endregion

        #region Private Method

        private void SkipQna(object _, SkipItemQnaEventArgs e)
        {
            if (item != e.item) return;
            clicked = false;
        }

        #endregion
    }
}
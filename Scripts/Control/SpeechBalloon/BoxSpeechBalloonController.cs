using System;
using Base;
using Event;
using UI.Popup.Qna;
using UnityEngine;

namespace Control.SpeechBalloon
{
    public class BoxSpeechBalloonController : BaseSpeechBalloonController
    {
        #region Public Variable

        public Domain.StageObj.Item item;

        #endregion

        #region Event
        public static event EventHandler<OpenItemQnaEventArgs> OpenItemQnaPopupEvent;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            ItemQnaPopupBehavior.SkipItemQnaEvent += SkipQna;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            ItemQnaPopupBehavior.SkipItemQnaEvent -= SkipQna;
        }

        #endregion

        #region Public Method

        public void OpenItemQnaPopup(OpenItemQnaEventArgs e)
        {
            EmitOpenItemQnaPopupEvent(e);
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
                    OpenItemQnaPopup(new OpenItemQnaEventArgs { item = item });
                }
            }
        }

        #endregion

        #region Private Method
        
        private void EmitOpenItemQnaPopupEvent(OpenItemQnaEventArgs e)
        {
            if (OpenItemQnaPopupEvent == null) return;
            foreach (var invocation in OpenItemQnaPopupEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void SkipQna(object _, SkipItemQnaEventArgs e)
        {
            if (item != e.item) return;
            clicked = false;
        }

        #endregion
    }
}
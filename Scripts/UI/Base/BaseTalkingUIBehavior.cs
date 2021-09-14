using System;
using System.Collections;
using Event;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Base
{
    public abstract class BaseTalkingUIBehavior: BaseUIBehavior
    {
        #region Protected Variables

        [SerializeField] protected GameObject okButton;
        [SerializeField] protected GameObject cancelButton;

        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            okButton.GetComponent<Button>().onClick.AddListener(OnClickOkBtn);
            cancelButton.GetComponent<Button>().onClick.AddListener(OnClickCancelBtn);
            okButton.SetActive(false);
            cancelButton.SetActive(false);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnClickOkBtn()
        {
            ResetAll();
            ActivateUI(false);
        }
        
        protected virtual void OnClickCancelBtn()
        {
            ResetAll();
            ReactivateSpeechBalloon();
            ActivateUI(false);
        }
        
        protected virtual void ResetAll() {}
        protected virtual void ReactivateSpeechBalloon() {}

        #endregion
    }
}
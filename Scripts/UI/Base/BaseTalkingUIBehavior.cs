using System;
using System.Collections;
using Event;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Base
{
    public class BaseTalkingUIBehavior: BaseUIBehavior
    {
        [SerializeField] protected GameObject okButton;
        [SerializeField] protected GameObject cancelButton;

        protected override void Start()
        {
            base.Start();
            okButton.GetComponent<Button>().onClick.AddListener(OnClickOkBtn);
            cancelButton.GetComponent<Button>().onClick.AddListener(OnClickCancelBtn);
            okButton.SetActive(false);
            cancelButton.SetActive(false);
        }

        protected virtual void OnClickOkBtn()
        {
            ResetAll();
            cGroup.Visible(false);
        }
        
        protected virtual void OnClickCancelBtn()
        {
            ResetAll();
            ReactivateSpeechBalloon();
            ActivateUI(false);
        }
        
        protected virtual void ResetAll() {}
        protected virtual void ReactivateSpeechBalloon() {}
    }
}
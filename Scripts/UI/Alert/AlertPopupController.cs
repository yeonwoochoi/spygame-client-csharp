using System.Collections;
using Base;
using Event;
using Http;
using MainScripts;
using UI.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Alert
{
    public class AlertPopupController: BasePopupBehavior
    {
        [SerializeField] private Text contentText;
        [SerializeField] private AlertButtonController okButtonController;
        [SerializeField] private AlertButtonController cancelButtonController;

        private float waitSeconds;
        
        protected override void Start() {
            base.Start();
            NetworkManager.AlertOccurredEvent += AlertOccurredEventHandler;
            BaseLoadingController.AlertOccurredEvent += AlertOccurredEventHandler;
            BaseSceneController.AlertOccurredEvent += AlertOccurredEventHandler;
            waitSeconds = 0.45f;
        }

        protected override void OnDisable() {
            base.OnDisable();
            NetworkManager.AlertOccurredEvent -= AlertOccurredEventHandler;
            BaseLoadingController.AlertOccurredEvent -= AlertOccurredEventHandler;
            BaseSceneController.AlertOccurredEvent -= AlertOccurredEventHandler;
        }

        private void AlertOccurredEventHandler(object _, AlertOccurredEventArgs e)
        {
            titleText.text = e.title;
            contentText.text = e.content;
            okButtonController.GetButtonText().text = e.okBtnText ?? okButtonController.GetButtonText().text;
            okButtonController.GetButton().onClick.AddListener(() =>
            {
                StartCoroutine(OnHandleClose(e.okHandler));
            });

            if (e.type == AlertType.Notice)
            {
                cancelButtonController.Active(false);
            }
            else
            {
                cancelButtonController.GetButtonText().text = e.cancelBtnText ?? cancelButtonController.GetButtonText().text;
                cancelButtonController.GetButton().onClick.AddListener(() => {
                    StartCoroutine(OnHandleClose(e.cancelHandler));
                });
            }
            
            OnOpenPopup();
        }

        private IEnumerator OnHandleClose(UnityAction handler)
        {
            OnClosePopup();
            yield return new WaitForSeconds(waitSeconds);
            handler?.Invoke();
        }
    }
}
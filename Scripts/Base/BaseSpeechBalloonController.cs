using System;
using System.Collections;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Base
{
    public abstract class BaseSpeechBalloonController: MonoBehaviour
    {
        #region Public Variable

        public bool clicked;

        #endregion

        #region Private Variable

        private EControlType eControlType;
        private Coroutine onClickSpeechBalloon;
        private bool isTutorial;

        #endregion

        #region Event Methods

        protected virtual void Start() { }

        protected virtual void OnDisable()
        {
            StopDetection();
        }

        #endregion

        #region Public Method

        public void StartDetection()
        {
            InitSpeechBalloon();
            if (eControlType != EControlType.Mouse) return;
            if (onClickSpeechBalloon != null)
            {
                StopCoroutine(onClickSpeechBalloon);
            }
            onClickSpeechBalloon = StartCoroutine(DetectClickSpeechBalloon());
        }

        #endregion

        #region Protected Method
        protected virtual void CheckValidHit(GameObject collider) {}

        #endregion

        #region Private Method

        private void InitSpeechBalloon()
        {
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
        }
        
        private void StopDetection()
        {
            if (onClickSpeechBalloon != null)
            {
                StopCoroutine(onClickSpeechBalloon);
            }
        }

        private IEnumerator DetectClickSpeechBalloon()
        {
            while (true)
            {
                if (!Input.GetMouseButtonDown(0) || clicked)
                {
                    yield return null;
                    continue;
                }
                var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                var hit2D = Physics2D.GetRayIntersection(ray);
                if (hit2D.collider != null)
                {
                    if (hit2D.collider.tag.Contains("Speech Balloon"))
                    {
                        CheckValidHit(hit2D.collider.gameObject);
                    }
                    else
                    {
                        clicked = false;
                    }
                }

                yield return null;
            }
        }

        #endregion
    }
}
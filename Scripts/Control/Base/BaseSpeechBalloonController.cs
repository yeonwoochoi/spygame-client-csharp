using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Control.Base
{
    public abstract class BaseSpeechBalloonController: MonoBehaviour
    {
        #region Public Variable

        [HideInInspector] public bool clicked;

        #endregion

        #region Private Variable

        private EControlType eControlType;

        #endregion

        #region Event Methods

        protected virtual void Start()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            gameObject.SetActive(false);
        }
        
        // TODO()
        private void Update()
        {
            if (eControlType == EControlType.KeyBoard) return;
            OnClickSpeechBalloon();
        }

        #endregion

        #region Private Method

        private void OnClickSpeechBalloon()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (eControlType != EControlType.Mouse) return;
            if (clicked) return;
            // TODO()
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
        }

        #endregion

        #region Protected Methods

        protected virtual void EmitEvent(GameObject target) {}
        
        protected virtual void CheckValidHit(GameObject collider) {}

        #endregion
    }
}
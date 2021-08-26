using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Control.SpeechBalloon
{
    public abstract class BaseSpeechBalloonController: MonoBehaviour
    {
        [HideInInspector] public bool clicked;
        
        private EControlManager eControlManager;

        protected virtual void Start()
        {
            eControlManager = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL);
            gameObject.SetActive(false);
        }
        
        private void Update()
        {
            if (eControlManager.eControlType == EControlType.KeyBoard) return;
            OnClickSpeechBalloon();
        }
        
        private void OnClickSpeechBalloon()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (eControlManager.eControlType != EControlType.Mouse) return;
            if (clicked) return;
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
        
        protected virtual void EmitEvent(GameObject target) {}
        
        protected virtual void CheckValidHit(GameObject collider) {}
    }
}
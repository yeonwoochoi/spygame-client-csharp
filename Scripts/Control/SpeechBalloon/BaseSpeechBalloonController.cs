using Domain;
using Manager;
using Manager.Data;
using UnityEngine;

namespace Control.SpeechBalloon
{
    public abstract class BaseSpeechBalloonController: MonoBehaviour
    {
        [HideInInspector] public bool clicked;
        private EControlType eControlType;
        
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
        
        protected virtual void EmitEvent(GameObject target) {}
        
        protected virtual void CheckValidHit(GameObject collider) {}
    }
}
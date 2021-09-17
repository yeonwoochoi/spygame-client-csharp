using System;
using System.Collections;
using Domain;
using Event;
using Manager;
using Manager.Data;
using StageScripts;
using UI.StageScripts;
using UI.StageScripts.Popup;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Control.Movement
{
    public class JoystickMoveController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        #region Private Variables

        [SerializeField] private RectTransform joystickRect;
        [SerializeField] private RectTransform joystickBgRect;
        [SerializeField] private Button actionButton;

        private float joystickBgRadius;
        private PlayerMoveController playerMoveController;
        private EControlType eControlType;
        private bool isSet = false;
        private bool isTutorial;

        #endregion

        #region Getter

        private bool IsPaused()
        {
            return isTutorial ? GlobalTutorialManager.Instance.IsPaused() : GlobalStageManager.Instance.IsPaused();
        }

        #endregion
        
        #region Event Methods

        private void Awake()
        {
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isSet) return;
            if (IsPaused()) return;
            MovePlayerByJoystick(eventData.position);
        }

        // button 클릭시 실행되는 event 함수
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isSet) return;
            if (IsPaused()) return;
            MovePlayerByJoystick(eventData.position);
        }

        // button click 떼는 순간 실행되는 event 함수
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isSet) return;
            if (IsPaused()) return;
            playerMoveController.MovePlayer(false, Vector2.zero);
            joystickRect.localPosition = Vector2.zero;
        }

        #endregion

        #region Public Method

        public void SetJoystick(PlayerMoveController controller, EControlType e)
        {
            eControlType = e;
            if (eControlType == EControlType.Mouse) return;

            playerMoveController = controller;
            joystickBgRadius = joystickBgRect.rect.width * 0.5f;
            actionButton.onClick.AddListener(controller.OnClickActionBtn);
            
            isSet = true;
        }

        #endregion

        #region Private Method

        private void MovePlayerByJoystick(Vector2 touchPos)
        {
            var joystickBgPos = joystickBgRect.position;
            var joystickPos = joystickRect.position;

            var touchOffset = new Vector2(touchPos.x - joystickBgPos.x, touchPos.y - joystickBgPos.y);

            // vector 값을 joystick bg radius 이하로 제한
            touchOffset = Vector2.ClampMagnitude(touchOffset, joystickBgRadius);
            joystickRect.localPosition = touchOffset;

            // Joystick 을 얼마나 더 Drag했는지에 따라 속도를 변경해주고 싶으면 이 값을 touchOffsetNormal에 곱해주면 된다.
            var distanceRatio = (joystickBgPos - joystickPos).sqrMagnitude / (joystickBgRadius * joystickBgRadius);

            var touchOffsetNormal = touchOffset.normalized;

            if (touchOffsetNormal.sqrMagnitude > 0)
            {
                
                playerMoveController.MovePlayer(true, touchOffsetNormal);
            }
        }

        #endregion
    }
}
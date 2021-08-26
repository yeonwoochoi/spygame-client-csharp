using Domain;
using Manager.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Control.Movement
{
    public class JoystickMoveController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private RectTransform joystickRect;
        [SerializeField] private RectTransform joystickBgRect;
        [SerializeField] private Button actionButton;
        
        private float joystickBgRadius;
        
        private GameObject playerObj;
        private PlayerMoveController playerMoveController;
        private EControlType eControlType;
        private float speed;
        private bool isSet = false;

        private bool isTouched;
        private bool IsTouched
        {
            get => isTouched;
            set
            {
                isTouched = value;
                playerMoveController.CurrentState = isTouched ? MoveStateType.Move : MoveStateType.Idle;
                if (!isTouched) playerMoveController.rb2D.velocity = Vector2.zero;
            }
        }
        
        public void SetJoystick(GameObject player, EControlType e)
        {
            isSet = false;
            eControlType = e;
            if (eControlType == EControlType.Mouse) return;
            playerObj = player;
            playerMoveController = player.GetComponent<PlayerMoveController>();
            speed = playerMoveController.speed;
            joystickBgRadius = joystickBgRect.rect.width * 0.5f;
            isTouched = false;
            actionButton.onClick.AddListener(playerMoveController.OnClickActionBtn);
            isSet = true;
        }
        

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
                playerMoveController.rb2D.velocity = touchOffsetNormal * speed;
            }
            
            playerMoveController.animator.SetFloat(BaseMoveController.ANIMATION_VARIABLE_PLAYER_HORIZONTAL, touchOffsetNormal.x * 50);
            playerMoveController.animator.SetFloat(BaseMoveController.ANIMATION_VARIABLE_PLAYER_VERTICAL, touchOffsetNormal.y * 50);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isSet) return;
            MovePlayerByJoystick(eventData.position);
            IsTouched = true;
        }

        // button 클릭시 실행되는 event 함수
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isSet) return;
            MovePlayerByJoystick(eventData.position);
            IsTouched = true;
        }

        // button click 떼는 순간 실행되는 event 함수
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isSet) return;
            joystickRect.localPosition = Vector2.zero;
            IsTouched = false;
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Control.Pointer
{
    public class PointerController : MonoBehaviour
    {
        #region Private Variables

        private Transform pointerTransform;
        private SpriteRenderer pointerSpriteRenderer;
        private Animator animator;
        private UnityEngine.Camera camera;
        private Transform playerTransform;
        private Transform targetTransform;
        
        private bool isSet = false;
        private bool isPointing = false;
        private Coroutine pointerMoveCoroutine;

        private float borderSizeX;
        private float borderSizeY;
        
        #endregion

        #region Getter

        public bool GetIsPointing()
        {
            return isPointing;
        }

        private bool IsOffScreen()
        {
            // Target position이 화면 밖에 있는지 여부 확인
            var targetPositionScreenPoint = camera.WorldToScreenPoint(targetTransform.position);
            var isOffScreen = targetPositionScreenPoint.x <= borderSizeX
                              || targetPositionScreenPoint.x >= Screen.width
                              || targetPositionScreenPoint.y <= borderSizeY
                              || targetPositionScreenPoint.y >= Screen.height;
            return isOffScreen;
        }
        
        private float GetAngleFromVectorFloat(Vector2 dir)
        {
            var eulerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return eulerAngle;
        }

        #endregion

        #region Setter

        private void SetIsPointing(bool flag)
        {
            isPointing = flag;
            pointerSpriteRenderer.color = new Color(1, 1, 1, flag ? 1 : 0);
        }

        private void SetPointerPosition(Vector3 screenPoint)
        {
            var worldPosition = camera.ScreenToWorldPoint(screenPoint);
            pointerTransform.localPosition = worldPosition;
        }

        #endregion

        #region Public Methods

        public void Init(UnityEngine.Camera camera)
        {
            this.camera = camera;
            
            pointerTransform = GetComponent<Transform>();
            animator = GetComponent<Animator>();
            pointerSpriteRenderer = GetComponent<SpriteRenderer>();

            borderSizeX = Screen.width * 0.05f;
            borderSizeY = Screen.height * 0.05f;
            
            SetIsPointing(false);
            isSet = true;
        }

        public void StartPointing(Transform from, Transform to)
        {
            if (!isSet) return;
            if (isPointing) return;
            
            playerTransform = from;
            targetTransform = to;
            SetIsPointing(true);
            StartCoroutine(StartPointingTarget());
        }
        
        public void EndPointing()
        {
            if (!isSet) return;
            if (pointerMoveCoroutine != null)
            {
                StopCoroutine(pointerMoveCoroutine);
                pointerMoveCoroutine = null;
            }
            playerTransform = null;
            targetTransform = null;
            SetIsPointing(false);
        }

        #endregion

        #region Private Methods
        
        private IEnumerator StartPointingTarget()
        {
            if (!isSet) yield break;

            while (isPointing)
            {
                var targetPositionScreenPoint = camera.WorldToScreenPoint(targetTransform.position);
                
                if (IsOffScreen())
                {
                    pointerMoveCoroutine = null;
                    // Target 방향으로 회전
                    RotatePointerTowardTarget();
                
                    // 화면 밖에 Target이 있을 때 화살표(pointer) 위치 설정
                    var cappedTargetScreen = targetPositionScreenPoint;
                    if (cappedTargetScreen.x <= 0) cappedTargetScreen.x = borderSizeX;
                    if (cappedTargetScreen.x >= Screen.width) cappedTargetScreen.x = Screen.width - borderSizeX;
                    if (cappedTargetScreen.y <= 0) cappedTargetScreen.y = borderSizeY;
                    if (cappedTargetScreen.y >= Screen.height) cappedTargetScreen.y = Screen.height - borderSizeY;
                    
                    SetPointerPosition(cappedTargetScreen);
                }
                else
                {
                    pointerMoveCoroutine ??= StartCoroutine(PointerUpDownMove());
                }

                yield return null;
            }
        }

        private void RotatePointerTowardTarget()
        {
            var fromPosition = (Vector2) playerTransform.position;
            var toPosition = (Vector2) targetTransform.position;
            
            // 각도 계산해서 화살표 회전시켜줌 (Target 위치로 방향 가리켜야하니까)
            var dir = (toPosition - fromPosition).normalized;
            var angle = GetAngleFromVectorFloat(dir);
            pointerTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        private IEnumerator PointerUpDownMove()
        {
            // 화살표 아래 방향으로 향하게
            pointerTransform.localEulerAngles = Vector3.forward * -90;

            // 위 아래로 움직일 위치 정하기
            var isMoveUp = true;
            var downPos = (Vector2) targetTransform.position + Vector2.up * targetTransform.localScale * 0.7f;
            var upPos = downPos + Vector2.up;

            pointerTransform.position = downPos;
            
            while (!IsOffScreen())
            {
                yield return null;
                
                if (isMoveUp)
                {
                    var after = Vector2.Lerp(pointerTransform.position, upPos, Time.deltaTime * 1.5f);
                    pointerTransform.position = after;
                    var distance = (after - upPos).sqrMagnitude;
                    if (distance < 0.1f)
                    {
                        isMoveUp = false;
                    }
                }
                else
                {
                    var after = Vector2.Lerp(pointerTransform.position, downPos, Time.deltaTime * 1.5f);
                    pointerTransform.position = after;
                    var distance = (after - downPos).sqrMagnitude;
                    if (distance < 0.1f)
                    {
                        isMoveUp = true;
                    }
                }
                
            }
        }
        
        #endregion
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.TutorialScripts
{
    public class QuestPointerController: MonoBehaviour
    {
        #region Private Variables

        private UnityEngine.Camera camera;
        private RectTransform pointerRectTransform;
        private Transform playerTransform;
        private Transform targetTransform;

        private CanvasGroup cGroup;
        private Image pointerImg;

        private bool isSet = false;
        private bool isPointing = false;

        #endregion

        #region Const Variable

        private const float borderSize = 50f;

        #endregion

        #region Getter

        private bool IsOffScreen()
        {
            // Target position이 화면 밖에 있는지 여부 확인
            var targetPositionScreenPoint = camera.WorldToScreenPoint(targetTransform.position);
            var isOffScreen = targetPositionScreenPoint.x <= borderSize
                              || targetPositionScreenPoint.x >= Screen.width
                              || targetPositionScreenPoint.y <= borderSize
                              || targetPositionScreenPoint.y >= Screen.height;
            return isOffScreen;
        }

        #endregion

        #region Setter

        private void IsPointing(bool flag)
        {
            isPointing = flag;
            cGroup.Visible(flag);
        }

        private void SetPointerPosition(Vector3 screenPoint)
        {
            var pointerWorldPosition = camera.ScreenToWorldPoint(screenPoint);
            pointerRectTransform.position = pointerWorldPosition;
            pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x,
                pointerRectTransform.localPosition.y, 0f);
        }

        #endregion

        #region Public Methods

        public void Init(UnityEngine.Camera camera)
        {
            this.camera = camera;
            
            pointerRectTransform = GetComponent<RectTransform>();
            pointerImg = GetComponent<Image>();
            
            cGroup = GetComponent<CanvasGroup>();
            IsPointing(false);
            isSet = true;
        }

        public void StartPointing(Transform from, Transform to)
        {
            if (!isSet) return;
            if (isPointing) return;
            
            playerTransform = from;
            targetTransform = to;
            IsPointing(true);
            StartCoroutine(StartPointingTarget());
        }

        public void EndPointing()
        {
            if (!isSet) return;
            IsPointing(false);
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
                    // Target 방향으로 회전
                    RotatePointerTowardTarget();
                
                    // 화면 밖에 Target이 있을 때 화살표(pointer) 위치 설정
                    var cappedTargetScreen = targetPositionScreenPoint;
                    if (cappedTargetScreen.x <= 0) cappedTargetScreen.x = 0f;
                    if (cappedTargetScreen.x >= Screen.width - borderSize) cappedTargetScreen.x = Screen.width - borderSize;
                    if (cappedTargetScreen.y <= 0) cappedTargetScreen.y = 0f;
                    if (cappedTargetScreen.y >= Screen.height - borderSize) cappedTargetScreen.y = Screen.height - borderSize;
                    SetPointerPosition(cappedTargetScreen);
                }
                else
                {
                    // 회전 제거
                    pointerRectTransform.localEulerAngles = Vector3.zero;
                
                    // 화면 안에 Target이 있을 때 화살표(Pointer) 위치 설정
                    SetPointerPosition(targetPositionScreenPoint);
                    StartCoroutine(PointerUpDownMove());
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
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        private float GetAngleFromVectorFloat(Vector2 dir)
        {
            var eulerAngle = Mathf.Atan2(dir.y, dir.x);
            return eulerAngle;
        }

        private IEnumerator PointerUpDownMove()
        {
            var downPos = pointerRectTransform.localPosition;
            var upPos = downPos + Vector3.up;

            var isMoveUp = true;
            
            while (!IsOffScreen())
            {
                yield return null;
                if (isMoveUp)
                {
                    pointerRectTransform.localPosition = Vector2.Lerp(pointerRectTransform.localPosition, upPos, Time.deltaTime);
                    var after = pointerRectTransform.localPosition;
                    var distance = (after - upPos).sqrMagnitude;
                    if (distance < 0.001f)
                    {
                        isMoveUp = false;
                    }
                }
                else
                {
                    pointerRectTransform.localPosition = Vector2.Lerp(pointerRectTransform.localPosition, downPos, Time.deltaTime);
                    var after = pointerRectTransform.localPosition;
                    var distance = (after - downPos).sqrMagnitude;
                    if (distance < 0.001f)
                    {
                        isMoveUp = true;
                    }
                }
            }
        }
        
        #endregion
    }
}
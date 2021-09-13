﻿using System.Collections;
using UnityEngine;
using Util;

namespace UI.TutorialScripts
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

        #endregion

        #region Const & Static Variables

        private static readonly int AnimatorParamIDIsPointing = Animator.StringToHash(AnimatorParamIsPointing);

        private const string AnimatorParamIsPointing = "IsPointing";
        private const float borderSize = 200f;

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
        
        private float GetAngleFromVectorFloat(Vector2 dir)
        {
            var eulerAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return eulerAngle;
        }

        #endregion

        #region Setter

        private void IsPointing(bool flag)
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
                    pointerMoveCoroutine = null;
                    // Target 방향으로 회전
                    RotatePointerTowardTarget();
                
                    // 화면 밖에 Target이 있을 때 화살표(pointer) 위치 설정
                    var cappedTargetScreen = targetPositionScreenPoint;
                    if (cappedTargetScreen.x <= 0) cappedTargetScreen.x = borderSize;
                    if (cappedTargetScreen.x >= Screen.width - borderSize) cappedTargetScreen.x = Screen.width - borderSize;
                    if (cappedTargetScreen.y <= 0) cappedTargetScreen.y = borderSize;
                    if (cappedTargetScreen.y >= Screen.height - borderSize) cappedTargetScreen.y = Screen.height - borderSize;
                    
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
            pointerTransform.localEulerAngles = Vector3.forward * -90;

            var isMoveUp = true;
            var downPos = (Vector2) targetTransform.position;
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
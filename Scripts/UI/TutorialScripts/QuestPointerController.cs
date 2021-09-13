﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.TutorialScripts
{
    public class QuestPointerController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private RectTransform pointerRectTransform;
        private Animator animator;
        private UnityEngine.Camera camera;
        private Transform playerTransform;
        private Transform targetTransform;

        private CanvasGroup cGroup;

        private bool isSet = false;
        private bool isPointing = false;
        private Coroutine pointerMoveCoroutine;

        #endregion

        #region Const & Static Variables

        private static readonly int PointingAnimator = Animator.StringToHash(AnimatorParamIsPointing);

        private const string AnimatorParamIsPointing = "IsPointing";
        private const float borderSize = 150f;

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
            cGroup.Visible(flag);
        }

        private void SetPointerPosition(Vector3 screenPoint)
        {
            var pointerViewportPosition = camera.ScreenToViewportPoint(screenPoint);
            var pointerScreenPosition = new Vector2(
                (pointerViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                (pointerViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
                );
            pointerRectTransform.anchoredPosition = pointerScreenPosition;
        }

        #endregion

        #region Public Methods

        public void Init(UnityEngine.Camera camera)
        {
            this.camera = camera;
            
            // pointerRectTransform = GetComponent<RectTransform>();
            animator = GetComponent<Animator>();
            
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
                    // 회전 제거
                    pointerRectTransform.localEulerAngles = Vector3.forward * -90;
                
                    // 화면 안에 Target이 있을 때 화살표(Pointer) 위치 설정
                    SetPointerPosition(targetPositionScreenPoint);
                    
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
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        private IEnumerator PointerUpDownMove()
        {
            var downPos = (Vector2) pointerRectTransform.localPosition;
            var upPos = downPos + Vector2.up * 3;

            var isMoveUp = true;
            while (!IsOffScreen())
            {
                yield return null;
                
                var targetPositionScreenPoint = camera.WorldToScreenPoint(targetTransform.position);
                SetPointerPosition(targetPositionScreenPoint);
                
                
                if (isMoveUp)
                {
                    pointerRectTransform.localPosition = Vector2.Lerp(pointerRectTransform.localPosition, upPos, Time.deltaTime);
                    var after = (Vector2) pointerRectTransform.localPosition;
                    var distance = (after - upPos).sqrMagnitude;
                    Debug.Log($"up before : {upPos} / after : {after}");
                    if (distance < 0.01f)
                    {
                        isMoveUp = false;
                    }
                }
                else
                {
                    pointerRectTransform.localPosition = Vector2.Lerp(pointerRectTransform.localPosition, downPos, Time.deltaTime);
                    var after = (Vector2) pointerRectTransform.localPosition;
                    var distance = (after - downPos).sqrMagnitude;
                    Debug.Log($"down before : {downPos} / after : {after}");
                    if (distance < 0.01f)
                    {
                        isMoveUp = true;
                    }
                }
                
            }
            animator.SetBool(PointingAnimator, false);
        }
        
        #endregion
    }
}
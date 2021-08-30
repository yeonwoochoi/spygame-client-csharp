using System;
using System.Collections;
using System.Collections.Generic;
using Control.Base;
using Control.Collision;
using Domain;
using Manager.Data;
using UnityEngine;

namespace Control.Movement
{
    public class PlayerMoveController: BaseMoveController
    {
        #region Public Variable

        public Action onClickActionBtn;

        #endregion

        #region Public Methods

        public void Init()
        {
            if (isSet) return;
            SetCurrentState(MoveStateType.Idle);
            objectType = MoveObjectType.Player;
            
            if (eControlType == EControlType.KeyBoard)
            {
                speed = 3f;
                isSet = true;
                return;
            }
            
            speed = 4f;
            StartCoroutine(CheckIdle());
            isSet = true;
        }

        public void MovePlayer(List<Vector3> positions)
        {
            if (GetCurrentState() != MoveStateType.Idle) return;
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(Move(positions));
        }

        public void MovePlayer(bool isMove, Vector2 dir)
        {
            if (!isMove)
            {
                SetCurrentState(MoveStateType.Idle);
                rb2D.velocity = Vector3.zero;
                return;
            }

            SetCurrentState(MoveStateType.Move);
            rb2D.velocity = dir * speed;
            animator.SetFloat(ANIMATION_VARIABLE_PLAYER_HORIZONTAL, dir.x * 50);
            animator.SetFloat(ANIMATION_VARIABLE_PLAYER_VERTICAL, dir.y * 50);
        }

        public void StopMove()
        {
            if (moveCoroutine == null) return;
            StopCoroutine(moveCoroutine);
            SetCurrentState(MoveStateType.Idle);
        }

        public void OnClickActionBtn()
        {
            onClickActionBtn?.Invoke();
        }

        #endregion

        #region Private Method

        private IEnumerator CheckIdle()
        {
            while (true)
            {
                yield return null;
                var beforePos = transform.position;
                yield return new WaitForSeconds(0.5f);
                var afterPos = transform.position;
                if (GetCurrentState() == MoveStateType.Idle) continue;
                if ((afterPos - beforePos).sqrMagnitude < 0.01f)
                {
                    SetCurrentState(MoveStateType.Idle);
                    if (moveCoroutine != null)
                    {
                        StopCoroutine(moveCoroutine);
                    }
                }
            }
        }

        #endregion
    }
}
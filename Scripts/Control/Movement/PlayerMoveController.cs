using System;
using System.Collections;
using System.Collections.Generic;
using Control.Collision;
using Domain;
using Manager.Data;
using UnityEngine;

namespace Control.Movement
{
    public class PlayerMoveController: BaseMoveController
    {
        public Action onClickActionBtn;

        public void Init()
        {
            if (isSet) return;
            CurrentState = MoveStateType.Idle;
            objectType = MoveObjectType.Player;
            speed = 3f;
            if (eControlType == EControlType.KeyBoard) return;
            speed = 4f;
            StartCoroutine(CheckIdle());
            isSet = true;
        }

        public void MovePlayer(List<Vector3> positions)
        {
            if (CurrentState != MoveStateType.Idle) return;
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
                CurrentState = MoveStateType.Idle;
                rb2D.velocity = Vector3.zero;
                return;
            }

            CurrentState = MoveStateType.Move;
            rb2D.velocity = dir * speed;
            animator.SetFloat(ANIMATION_VARIABLE_PLAYER_HORIZONTAL, dir.x * 50);
            animator.SetFloat(ANIMATION_VARIABLE_PLAYER_VERTICAL, dir.y * 50);
        }

        public void StopMove()
        {
            if (moveCoroutine == null) return;
            StopCoroutine(moveCoroutine);
            CurrentState = MoveStateType.Idle;
        }
        
        private IEnumerator CheckIdle()
        {
            while (true)
            {
                yield return null;
                var beforePos = transform.position;
                yield return new WaitForSeconds(0.5f);
                var afterPos = transform.position;
                if (CurrentState == MoveStateType.Idle) continue;
                if ((afterPos - beforePos).sqrMagnitude < 0.01f)
                {
                    CurrentState = MoveStateType.Idle;
                    if (moveCoroutine != null)
                    {
                        StopCoroutine(moveCoroutine);
                    }
                }
            }
        }
        
        public void OnClickActionBtn()
        {
            onClickActionBtn?.Invoke();
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using Control.Collision;
using Domain;
using Manager.Data;
using UnityEngine;

namespace Control.Movement
{
    public class PlayerMoveController: BaseMoveController
    {
        [SerializeField] private DetectorForPlayer detectorForPlayer;

        public void SetPlayer()
        {
            if (isSet) return;
            CurrentState = MoveStateType.Idle;
            objectType = MoveObjectType.Player;
            speed = 3f;
            detectorForPlayer.Set(eControlType);
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
            detectorForPlayer.OnClickActionBtn();
        }
        

        /*
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (moveCoroutine == null) return;
            if (other.gameObject.tag == "Detector" || other.gameObject.layer == LayerMask.NameToLayer("UI")) return;
            StopCoroutine(moveCoroutine);
            CurrentState = MoveStateType.Idle;
        }
        */
    }
}
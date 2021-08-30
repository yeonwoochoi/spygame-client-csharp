﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base;
using Control.SpeechBalloon;
using Domain;
using Domain.StageObj;
using Event;
using UI.Qna;
using UI.Talking;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Control.Movement
{
    #region Enum

    public enum SpyStateType
    {
        Free, Examined, Capture, Release
    }

    #endregion

    public class SpyMoveController: BaseMoveController
    {
        #region Public Variables

        [SerializeField] public GameObject speechBalloon;
        public SpySpeechBalloonController speechBalloonController;

        #endregion

        #region Private Variables

        private readonly int wanderRange = 3;
        private readonly float wanderDelay = 3f;
        private Coroutine wanderCoroutine;
        private SpriteRenderer spriteRenderer;

        private Spy spy;

        #endregion

        public Spy Spy => spy;
        
        private SpyStateType spyStateType;

        public SpyStateType SpyStateType
        {
            get => spyStateType;
            set
            {
                spyStateType = value;
                if (spyStateType != SpyStateType.Free)
                {
                    StopWandering();
                }
            }
        }

        public bool IsSet => isSet;

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            speechBalloonController = speechBalloon.GetComponent<SpySpeechBalloonController>();
            
            SpyQnaPopupBehavior.CaptureSpyEvent += InactivateSpy;
            SpyTalkingUIBehavior.OpenSpyQnaPopupEvent += InactivateMoving;
        }

        protected void OnDisable()
        {
            SpyQnaPopupBehavior.CaptureSpyEvent -= InactivateSpy;
            SpyTalkingUIBehavior.OpenSpyQnaPopupEvent -= InactivateMoving;
        }

        private void Update()
        {
            if (speechBalloonController.clicked)
            {
                StopWandering();
            }
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (!IsSet) return;
            if (SpyStateType != SpyStateType.Free) return;
            if (other.collider.transform.IsChildOf(transform)) return;
            StopWandering();
        }


        protected void OnCollisionExit2D(Collision2D other)
        {
            if (!IsSet) return;
            if (SpyStateType != SpyStateType.Free) return;
            if (other.collider.transform.IsChildOf(transform)) return;
            StartWandering();
        }

        #endregion

        #region Public Methods

        public void Init(Spy spy)
        {
            this.spy = spy;
            speechBalloon.GetComponent<SpySpeechBalloonController>().spy = spy;
            speed = 2f;
            objectType = MoveObjectType.Spy;
            SpyStateType = SpyStateType.Free;
            wanderCoroutine = StartCoroutine(Wander());
            StartCoroutine(CheckIdle());
            isSet = true;
        }

        public void StartWandering()
        {
            if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            wanderCoroutine = StartCoroutine(Wander());
            SetCurrentState(MoveStateType.Move);
        }

        #endregion

        #region Private Methods

        private IEnumerator Wander()
        {
            while (true)
            {
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                yield return null;
                yield return moveCoroutine = StartCoroutine(Move(GetRandomDestination()));
                yield return new WaitForSeconds(wanderDelay);
            }
        }

        private IEnumerator CheckIdle()
        {
            while (true)
            {
                yield return null;
                var beforePos = transform.position;
                yield return new WaitForSeconds(1.5f);
                var afterPos = transform.position;
                if ((afterPos - beforePos).sqrMagnitude < 1)
                {
                    if (SpyStateType == SpyStateType.Free)
                    {
                        StopWandering();
                        StartWandering();   
                    }
                }
            }
        }

        private List<Vector3> GetRandomDestination()
        {
            var dirX = Vector2.right;
            var dirY = Vector2.up;
            
            var hitX = Physics2D.RaycastAll(transform.position, dirX, nodeSize.x * 2);
            var hitY = Physics2D.RaycastAll(transform.position, dirY, nodeSize.y * 2);

            var isHitX = false;
            var isHitY = false;
            
            foreach (var hit2D in hitX)
            {
                if (hit2D.transform.IsChildOf(transform)) continue;
                isHitX = true;
            }

            foreach (var hit2D in hitY)
            {
                if (hit2D.transform.IsChildOf(transform)) continue;
                isHitY = true;
            }

            var offsetX = isHitX ? Random.Range(-wanderRange, 1) : Random.Range(0, wanderRange + 1);
            var offsetY = isHitY ? Random.Range(-wanderRange, 1) : Random.Range(0, wanderRange + 1);
            
            var result = GetNodeList(new Vector3(offsetX, offsetY, 0));
       
            return result;
        }

        private List<Vector3> GetNodeList(Vector3 offset)
        {
            var sizeX = (int) offset.x;
            var sizeY = (int) offset.y;

            var result = new List<Vector3>();
            var temp = transform.position;

            if (sizeX >= 0)
            {
                for (var i = 0; i < sizeX; i++)
                {
                    temp.x += 1;
                    result.Add(temp);
                }
            }
            else
            {
                for (var i = 0; i < -sizeX; i++)
                {
                    temp.x -= 1;
                    result.Add(temp);
                }
            }

            if (sizeY >= 0)
            {
                for (var i = 0; i < sizeY; i++)
                {
                    temp.y += 1;
                    result.Add(temp);
                }
            }
            else
            {
                for (var i = 0; i < -sizeY; i++)
                {
                    temp.y -= 1;
                    result.Add(temp);
                }
            }

            return result;
        }

        private void StopWandering()
        {
            if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            SetCurrentState(MoveStateType.Idle);
        }

        private IEnumerator FadeOut()
        {
            for (float i = 1; i >= 0; i -= Time.deltaTime * 1.5f)
            {
                spriteRenderer.color = new Color(1, 1, 1, i);
                yield return null;
            }
            Destroy(gameObject);
        }

        private void InactivateSpy(object _, CaptureSpyEventArgs e)
        {
            if (e.spy.index != spy.index) return;
            SpyStateType = e.type switch
            {
                CaptureSpyType.Capture => SpyStateType.Capture,
                CaptureSpyType.Release => SpyStateType.Release,
                _ => SpyStateType
            };
            speechBalloon.SetActive(false);
            StartCoroutine(FadeOut());
        }

        private void InactivateMoving(object _, OpenSpyQnaEventArgs e)
        {
            if (e.spy.index != spy.index) return;
            SpyStateType = SpyStateType.Examined;
        }

        #endregion
    }
}
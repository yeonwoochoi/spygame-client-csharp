using System;
using System.Collections;
using System.Collections.Generic;
using Domain;
using Manager;
using Manager.Data;
using StageScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Control.Movement
{
    public enum MoveObjectType
    {
        Player, Spy
    }

    public enum MoveStateType
    {
        Idle, Move
    }
    
    public abstract class BaseMoveController: MonoBehaviour
    {
        public static string ANIMATION_VARIABLE_PLAYER_HORIZONTAL = "Horizontal";
        public static string ANIMATION_VARIABLE_PLAYER_VERTICAL = "Vertical";
        public static string ANIMATION_VARIABLE_PLAYER_SPEED = "Speed";
        
        [HideInInspector] public MoveObjectType objectType;

        protected float speed;
        protected EControlType eControlType;
        protected Rigidbody2D rb2D;
        protected Animator animator;
        
        protected Coroutine moveCoroutine;
        protected Vector2 nodeSize;
        protected bool isSet = false;

        private MoveStateType currentState;
        public MoveStateType CurrentState
        {
            get => currentState;
            set
            {
                currentState = value;
                animator.SetFloat(ANIMATION_VARIABLE_PLAYER_SPEED, currentState == MoveStateType.Idle ? 0 : 1);
            }
        }

        private Tilemap tilemap;
        public Tilemap Tilemap
        {
            get => tilemap;
            set
            {
                tilemap = value;
                nodeSize = tilemap.transform.localScale;
            }
        }

        private void Awake()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            rb2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        protected virtual void Start() { }

        protected IEnumerator Move(List<Vector3> positions)
        {
            if (positions.Count == 0) yield break;

            CurrentState = MoveStateType.Move;

            for (var i = 0; i < positions.Count; i++)
            {
                var pos = positions[i];
                var offset = pos - transform.position;
                var remainingDistance = offset.sqrMagnitude;

                while (remainingDistance >= 0.001f)
                {
                    if (rb2D != null)
                    {
                        var newPosition = Vector2.MoveTowards(rb2D.position, pos, speed * Time.deltaTime);
                        rb2D.MovePosition(newPosition);
                        offset = pos - transform.position;
                        animator.SetFloat(ANIMATION_VARIABLE_PLAYER_HORIZONTAL, offset.x * 50);
                        animator.SetFloat(ANIMATION_VARIABLE_PLAYER_VERTICAL, offset.y * 50);
                        remainingDistance = offset.sqrMagnitude;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            CurrentState = MoveStateType.Idle;
        }

        public Vector3 GetNodePosition(Vector3 position)
        {
            var posX = (int) (position.x / nodeSize.x) * nodeSize.x + nodeSize.x / 2;
            var posY = (int) (position.y / nodeSize.y) * nodeSize.y + nodeSize.y / 2;
            return new Vector3(posX, posY, 0);
        }
    }
}
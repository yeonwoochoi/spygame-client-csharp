using System;
using System.Collections;
using System.Collections.Generic;
using Domain;
using Event;
using Manager;
using Manager.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Base
{
    #region Enums

    public enum MoveObjectType
    {
        Player, Spy
    }

    public enum MoveStateType
    {
        Idle, Move
    }

    public enum WeaponType
    {
        Arrow, Spear, Sword
    }

    #endregion

    public abstract class BaseMoveController: MonoBehaviour
    {
        #region Public Variable
        [HideInInspector] public MoveObjectType objectType;

        #endregion

        #region Protected Variables

        protected float speed;
        protected EControlType eControlType;
        protected Rigidbody2D rb2D;
        protected Animator animator;
        protected Coroutine moveCoroutine;
        protected Vector2 nodeSize;
        protected bool isSet = false;
        
        // R
        protected float x, y;
        protected bool isWalking = false;
        protected bool isDeath = false;
        protected Vector3 moveDir;

        #endregion

        #region Private Variables
        private Tilemap tilemap;
        private MoveStateType currentState;
        private bool isTutorial = false;

        #endregion

        #region Static Variables

        // R
        private static readonly int AnimatorIdX = Animator.StringToHash(AnimatorParamX);
        private static readonly int AnimatorIdY = Animator.StringToHash(AnimatorParamY);
        private static readonly int AnimatorIdIsMoving = Animator.StringToHash(AnimatorParamIsMoving);

        // R
        private const string AnimatorParamX = "X";
        private const string AnimatorParamY = "Y";
        private const string AnimatorParamIsMoving = "IsMoving";

        #endregion
        
        // R
        #region Setter

        protected void SetDirection(float xDir, float yDir)
        {
            x = xDir;
            y = yDir;
            animator.SetFloat(AnimatorIdX, x);
            animator.SetFloat(AnimatorIdY, y);
        }

        #endregion
        
        #region Event Methods
        private void Awake()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            rb2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
        }

        protected virtual void Start() { }
        protected virtual void OnDisable() { }

        #endregion

        #region Public Methods
        public MoveStateType GetCurrentState()
        {
            return currentState;
        }

        public void SetTilemap(Tilemap map)
        {
            tilemap = map;
            nodeSize = tilemap.transform.localScale;
        }

        public Vector3 GetNodePosition(Vector3 position)
        {
            var posX = (int) (position.x / nodeSize.x) * nodeSize.x + nodeSize.x / 2;
            var posY = (int) (position.y / nodeSize.y) * nodeSize.y + nodeSize.y / 2;
            return new Vector3(posX, posY, 0);
        }
        
        protected void StartMoving()
        {
            rb2D.velocity = moveDir * speed * Time.fixedDeltaTime;    
        }

        protected void StopMoving()
        {
            rb2D.velocity = Vector3.zero;
        }
        #endregion

        #region Protected Methods
        protected void SetCurrentState(MoveStateType moveStateType)
        {
            currentState = moveStateType;
            isWalking = currentState == MoveStateType.Move;
            animator.SetBool(AnimatorIdIsMoving, isWalking);
            if (!isWalking) StopMoving();
        }

        protected IEnumerator Move(List<Vector3> positions)
        {
            if (positions.Count == 0) yield break;
            if (isWalking) yield break;

            SetCurrentState(MoveStateType.Move);
            
            foreach (var pos in positions)
            {
                var offset = pos - transform.position;
                var remainingDistance = offset.sqrMagnitude;

                while (remainingDistance >= 0.001f)
                {
                    yield return StartCoroutine(StopMoveByPausing());
                    if (rb2D != null)
                    {
                        var newPosition = Vector2.MoveTowards(rb2D.position, pos, speed * Time.fixedDeltaTime);
                        rb2D.MovePosition(newPosition);
                        offset = pos - transform.position;
                        SetDirection(offset.x * 50, offset.y * 50);
                        remainingDistance = offset.sqrMagnitude;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            
            SetCurrentState(MoveStateType.Idle);
        }

        protected IEnumerator StopMoveByPausing()
        {
            if (!isTutorial)
            {
                while (GlobalStageManager.Instance.IsPaused())
                {
                    yield return null;
                }       
            }
            else
            {
                while (GlobalTutorialManager.Instance.IsPaused())
                {
                    yield return null;
                }
            }
        }
        #endregion
    }
}
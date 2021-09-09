using System.Collections;
using System.Collections.Generic;
using Domain;
using Event;
using Manager;
using Manager.Data;
using UI.StageScripts;
using UI.StageScripts.Popup;
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
        protected bool isPaused = false;
        
        #endregion

        #region Private Variables
        private Tilemap tilemap;
        private MoveStateType currentState;

        #endregion

        #region Static Variables
        
        private static readonly int Horizontal = Animator.StringToHash(AnimationPlayerHorizontal);
        private static readonly int Vertical = Animator.StringToHash(AnimationPlayerVertical);
        private static readonly int Speed = Animator.StringToHash(AnimationPlayerSpeed);
        
        protected const string AnimationPlayerHorizontal = "Horizontal";
        protected const string AnimationPlayerVertical = "Vertical";
        private const string AnimationPlayerSpeed = "Speed";

        #endregion
        
        #region Event Methods
        private void Awake()
        {
            eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            rb2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }
        protected virtual void Start()
        {
            StagePausePopupController.PauseGameEvent += PauseGame;
        }

        protected virtual void OnDisable()
        {
            StagePausePopupController.PauseGameEvent -= PauseGame;
        }
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
        #endregion

        #region Protected Methods
        protected void SetCurrentState(MoveStateType moveStateType)
        {
            currentState = moveStateType;
            animator.SetFloat(Speed, currentState == MoveStateType.Idle ? 0 : 1);
        }

        protected IEnumerator Move(List<Vector3> positions)
        {
            if (positions.Count == 0) yield break;

            SetCurrentState(MoveStateType.Move);

            foreach (var pos in positions)
            {
                var offset = pos - transform.position;
                var remainingDistance = offset.sqrMagnitude;

                while (remainingDistance >= 0.001f)
                {
                    while (isPaused) yield return null;
                    if (rb2D != null)
                    {
                        var newPosition = Vector2.MoveTowards(rb2D.position, pos, speed * Time.deltaTime);
                        rb2D.MovePosition(newPosition);
                        offset = pos - transform.position;
                        animator.SetFloat(Horizontal, offset.x * 50);
                        animator.SetFloat(Vertical, offset.y * 50);
                        remainingDistance = offset.sqrMagnitude;
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            SetCurrentState(MoveStateType.Idle);
        }
        #endregion

        #region Private Method

        private void PauseGame(object _, PauseGameEventArgs e)
        {
            isPaused = e.isPaused;
        }

        #endregion
    }
}
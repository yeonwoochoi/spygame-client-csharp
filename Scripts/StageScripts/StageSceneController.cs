using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base;
using Camera;
using Control.Item;
using Control.Movement;
using Domain;
using Domain.StageObj;
using Event;
using MainScripts;
using Manager;
using Manager.Data;
using UI.Stage;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Util;
using Random = UnityEngine.Random;


namespace StageScripts
{
    public enum StageMissionType
    {
        Capture, SneakIn, Kill, Rescue
    }
    
    public class StageSceneController: BaseSceneController
    {
        #region Private Variables

        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Transform initPlayerTransform;
        [SerializeField] private LineRenderer line;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject normalSpy;
        [SerializeField] private GameObject bossSpy;
        [SerializeField] private GameObject item;

        private List<Qna> qna;
        
        private JoystickMoveController joystickMoveController;
        private Transform playerObjParent;
        private Transform spyObjParent;
        private Transform boxObjParent;

        private StageInfo currentStageInfo;
        
        private List<Vector3> normalSpyPositions;
        private List<Vector3> bossSpyPositions;
        private List<Vector3> itemPositions;
        private Vector3 nodeSize;
        
        private EControlType eControlType;
        private ChapterType currentChapterType;
        private StageType currentStageType;
        
        #endregion

        #region Event

        public static event EventHandler<OpenStageMissionPopupEventArgs> OpenStageMissionPopupEvent;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            StagePauseController.ExitStageEvent += ExitStageScene;
            StageDonePopupController.ExitStageSceneEvent += ExitStageScene;
            StartCoroutine(ShowMissionPopup());
        }

        private void OnDisable()
        {
            StagePauseController.ExitStageEvent -= ExitStageScene;
            StageDonePopupController.ExitStageSceneEvent -= ExitStageScene;
        }

        #endregion

        #region Public Methods

        public void SetStageObjParent(Transform playerParent, Transform spyParent, Transform boxParent)
        {
            playerObjParent = playerParent;
            spyObjParent = spyParent;
            boxObjParent = boxParent;
        }
        
        public void SetCurrentStage(JoystickMoveController joystick, EControlType e, UnityEngine.Camera camera)
        {
            qna = QnaManager.Instance.qna;
            Debug.Log(qna.Count);
            currentChapterType = LoadingManager.Instance.chapterType;
            currentStageType = LoadingManager.Instance.stageType;
            currentStageInfo = ChapterManager.Instance.GetStageInfo(currentChapterType, currentStageType);
            
            eControlType = e;
            
            nodeSize = tilemap.transform.localScale / 2;
            nodeSize.z = 0;

            // Instantiate player and setting controller
            var playerObj = Instantiate(player, initPlayerTransform.position + nodeSize, Quaternion.identity);
            playerObj.transform.SetParent(playerObjParent);
            var playerMoveController = playerObj.GetComponent<PlayerMoveController>();
            playerMoveController.Init();
            playerMoveController.SetTilemap(tilemap);

            // Set joystick
            joystickMoveController = joystick;
            joystickMoveController.SetJoystick(playerMoveController, eControlType);
            
            // Set line generator
            if (eControlType == EControlType.Mouse)
            {
                var lineGenerator = gameObject.AddComponent<LineGenerator>();
                lineGenerator.Init(tilemap, line, playerObj, eControlType, camera);
            }

            // Set Camera offset
            camera.GetComponent<CameraFollowController>().SetOffset(playerObj.transform);

            // Init stage game object positions
            normalSpyPositions = new List<Vector3>();
            bossSpyPositions = new List<Vector3>();
            itemPositions = new List<Vector3>();
            
            // Set spies and items
            SetSpy();
            SetItem();
        }

        #endregion

        #region Private Methods

        private void SetSpy()
        {
            SetSpyPositions();
            var currentSetNormalSpyCount = 0;
            var currentSetBossSpyCount = 0;
            for (var i = 0; i < normalSpyPositions.Count; i++)
            {
                var pos = normalSpyPositions[i];
                var spyObj = Instantiate(normalSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(spyObjParent);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.SetTilemap(tilemap);
                spyMoveController.Init(new Spy(i+1000, SpyType.Normal, GetRandomQna(QnaDifficulty.Easy), currentSetNormalSpyCount >= currentStageInfo.goalNormalSpyCount));
                currentSetNormalSpyCount++;
            }
            
            for (var i = 0; i < bossSpyPositions.Count; i++)
            {
                var pos = bossSpyPositions[i];
                var spyObj = Instantiate(bossSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(spyObjParent);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.SetTilemap(tilemap);
                spyMoveController.Init(new Spy(i+2000, SpyType.Boss, GetRandomQna(QnaDifficulty.Hard), currentSetBossSpyCount >= currentStageInfo.goalBossSpyCount));
                currentSetBossSpyCount++;
            }
        }

        private void SetItem()
        {
            SetItemPositions();
            for (var i = 0; i < itemPositions.Count; i++)
            {
                var pos = itemPositions[i];
                var itemObj = Instantiate(item, pos, Quaternion.identity);
                itemObj.transform.SetParent(boxObjParent);
                var itemBoxController = itemObj.GetComponent<ItemBoxController>();
                itemBoxController.Init(new Item(i+3000, GetRandomQna((QnaDifficulty) Random.Range(0, 2))));
            }
        }

        private void SetSpyPositions()
        {
            for (var i = 0; i < currentStageInfo.normalSpyCount; i++)
            {
                normalSpyPositions.Add(GetPossiblePosition());
            }
            for (var i = 0; i < currentStageInfo.bossSpyCount; i++)
            {
                bossSpyPositions.Add(GetPossiblePosition());
            }
        }

        private void SetItemPositions()
        {
            for (var i = 0; i < currentStageInfo.boxCount; i++)
            {
                itemPositions.Add(GetPossiblePosition());
            }
        }
        
        private Vector3 GetPossiblePosition()
        {
            tilemap.CompressBounds();
            var bounds = tilemap.cellBounds;
            var result = Vector3.zero;
            while (true)
            {
                var randomX = Random.Range(bounds.xMin, bounds.xMax);
                var randomY = Random.Range(bounds.yMin, bounds.yMax);

                if (HasSpyOrItem(randomX, randomY)) continue;
                if (!tilemap.HasTile(new Vector3Int(randomX, randomY, 0))) continue;
                
                result.x = randomX;
                result.y = randomY;
                break;
            }

            return new Vector3(result.x + nodeSize.x, result.y + nodeSize.y, 0);
        }
        
        private bool HasSpyOrItem(int x, int y)
        {
            if (x == 0 && y == 0) return true;

            var hasPlayer = false;
            var hasSpy = false;
            var hasItem = false;

            if (initPlayerTransform.position.IsSamePosition(new Vector3(x, y, 0)))
            {
                hasPlayer = true;
            }
            
            if (normalSpyPositions.Count > 0)
            {
                foreach (var position in normalSpyPositions)
                {
                    if (position.IsSamePosition(new Vector3(x, y, 0)))
                    {
                        hasSpy = true;
                    }
                }
            }
            
            if (bossSpyPositions.Count > 0)
            {
                foreach (var position in bossSpyPositions)
                {
                    if (position.IsSamePosition(new Vector3(x, y, 0)))
                    {
                        hasSpy = true;
                    }
                }
            }

            if (itemPositions.Count > 0)
            {
                foreach (var position in itemPositions)
                {
                    if (position.IsSamePosition(new Vector3(x, y, 0)))
                    {
                        hasItem = true;
                    }
                }
            }

            return hasItem || hasSpy || hasPlayer;
        }

        private IEnumerator ShowMissionPopup()
        {
            yield return new WaitForSeconds(0.01f);
            EmitOpenStageMissionPopupEvent(new OpenStageMissionPopupEventArgs());
        }
        
        private void EmitOpenStageMissionPopupEvent(OpenStageMissionPopupEventArgs e)
        {
            if (OpenStageMissionPopupEvent == null) return;
            foreach (var invocation in OpenStageMissionPopupEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void ExitStageScene(object _, ExitStageEventArgs e)
        {
            OnClickExitBtn(currentChapterType);
        }

        private void OnClickExitBtn(ChapterType chapterType)
        {
            LoadingManager.Instance.currentType = MainSceneType.Play;
            LoadingManager.Instance.nextType = MainSceneType.Select;
            LoadingManager.Instance.loadingType = LoadingType.Normal;
            LoadingManager.Instance.chapterType = chapterType;

            StartCoroutine(StartLoadingAnimator(
                () => nextScene = SceneNameManager.SceneNormalLoading, 
                () => SceneManager.LoadScene(nextScene)));
        }

        private Qna GetRandomQna(QnaDifficulty qnaDifficulty)
        {
            var result = qna.Where(q => q.GetDifficulty() == qnaDifficulty).ToList();
            return result[Random.Range(0, result.Count)];
        }

        #endregion
    }
}
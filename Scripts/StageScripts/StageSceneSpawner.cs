using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base;
using Camera;
using Control.Item;
using Control.Layer;
using Control.Movement;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using UI.Popup.StageScripts;
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
    
    public class StageSceneSpawner: BaseSceneController
    {
        #region Private Variables

        [SerializeField] private List<Tilemap> tilemaps;
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
        
        private List<NodeInfo> normalSpyPositions;
        private List<NodeInfo> bossSpyPositions;
        private List<NodeInfo> itemPositions;
        private Vector3 nodeSize;
        
        private EControlType eControlType;
        private ChapterType currentChapterType;
        private StageType currentStageType;
        
        private int currentLayer1ObjCount = 0;

        #endregion

        #region Event
        public static event EventHandler<OpenStageMissionPopupEventArgs> OpenStageMissionPopupEvent;

        #endregion

        #region Private Class

        private class NodeInfo
        {
            public Vector3 position;
            public LayerType layer;
        }

        #endregion

        #region Event Methods

        protected override void Start()
        {
            StagePausePopupController.ExitStageEvent += ExitStageScene;
            StageDonePopupController.ExitStageSceneEvent += ExitStageScene;
            StartCoroutine(ShowMissionPopup());
        }

        private void OnDisable()
        {
            StagePausePopupController.ExitStageEvent -= ExitStageScene;
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
        
        public void SetCurrentStage(JoystickMoveController joystick, EControlType e, UnityEngine.Camera mainCamera, UnityEngine.Camera minimapCamera)
        {
            qna = QnaManager.Instance.qna;
            currentChapterType = LoadingManager.Instance.chapterType;
            currentStageType = LoadingManager.Instance.stageType;
            currentStageInfo = ChapterManager.Instance.GetStageInfo(currentChapterType, currentStageType);
            
            eControlType = e;
            
            nodeSize = tilemaps[0].transform.localScale / 2;
            nodeSize.z = 0;

            // Instantiate player and setting controller
            var playerObj = Instantiate(player, initPlayerTransform.position + nodeSize, Quaternion.identity);
            playerObj.transform.SetParent(playerObjParent);
            SetObjectLayer(playerObj, LayerType.Layer1);
            var playerMoveController = playerObj.GetComponent<PlayerMoveController>();
            playerMoveController.Init();
            playerMoveController.SetTilemap(tilemaps[0]);

            // Set joystick
            joystickMoveController = joystick;
            joystickMoveController.SetJoystick(playerMoveController, eControlType);
            
            // Set line generator
            if (eControlType == EControlType.Mouse)
            {
                var lineGenerator = gameObject.AddComponent<LineGenerator>();
                lineGenerator.Init(tilemaps[0], line, playerObj, eControlType, mainCamera);
            }

            // Set Camera offset
            mainCamera.GetComponent<CameraFollowController>().SetOffset(playerObj.transform);
            minimapCamera.GetComponent<CameraFollowController>().SetOffset(playerObj.transform);

            // Init stage game object positions
            normalSpyPositions = new List<NodeInfo>();
            bossSpyPositions = new List<NodeInfo>();
            itemPositions = new List<NodeInfo>();
            
            // Set spies and items
            SetSpy();
            SetItem();
            
            GlobalStageManager.Instance.InitGame();
            
            AudioManager.instance.Play(SoundType.Background);
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
                var pos = normalSpyPositions[i].position;
                var spyObj = Instantiate(normalSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(spyObjParent);
                SetObjectLayer(spyObj, normalSpyPositions[i].layer);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.SetTilemap(tilemaps[0]);
                spyMoveController.Init(new Spy(i+1000, SpyType.Normal, GetRandomQna(QnaDifficulty.Easy), currentSetNormalSpyCount >= currentStageInfo.goalNormalSpyCount));
                currentSetNormalSpyCount++;
            }
            
            for (var i = 0; i < bossSpyPositions.Count; i++)
            {
                var pos = bossSpyPositions[i].position;
                var spyObj = Instantiate(bossSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(spyObjParent);
                SetObjectLayer(spyObj, bossSpyPositions[i].layer);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.SetTilemap(tilemaps[0]);
                spyMoveController.Init(new Spy(i+2000, SpyType.Boss, GetRandomQna(QnaDifficulty.Hard), currentSetBossSpyCount >= currentStageInfo.goalBossSpyCount));
                currentSetBossSpyCount++;
            }
        }

        private void SetItem()
        {
            SetItemPositions();
            for (var i = 0; i < itemPositions.Count; i++)
            {
                var pos = itemPositions[i].position;
                var itemObj = Instantiate(item, pos, Quaternion.identity);
                itemObj.transform.SetParent(boxObjParent);
                SetObjectLayer(itemObj, itemPositions[i].layer);
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
        
        private NodeInfo GetPossiblePosition()
        {
            var targetLayer = Random.Range(0, tilemaps.Count);
            if (targetLayer > 0)
            {
                if (IsOverLayerLimitCount())
                {
                    currentLayer1ObjCount++;   
                }
                else
                {
                    targetLayer = 0;
                }
            }
            var targetTilemap = tilemaps[targetLayer];
            
            targetTilemap.CompressBounds();
            var bounds = targetTilemap.cellBounds;
            
            var pos = Vector3.zero;
            
            while (true)
            {
                var randomX = Random.Range(bounds.xMin, bounds.xMax);
                var randomY = Random.Range(bounds.yMin, bounds.yMax);

                if (HasSpyOrItem(randomX, randomY)) continue;
                if (!targetTilemap.HasTile(new Vector3Int(randomX, randomY, 0))) continue;
                
                pos.x = randomX;
                pos.y = randomY;
                break;
            }
            

            var resultNode = new NodeInfo
            {
                position = new Vector3(pos.x + nodeSize.x, pos.y + nodeSize.y, 0),
                layer = (LayerType) targetLayer
            };

            return resultNode;
        }
        
        private bool IsOverLayerLimitCount()
        {
            var totalObj = currentStageInfo.boxCount + currentStageInfo.normalSpyCount + currentStageInfo.bossSpyCount;
            var limit = (int) (totalObj * 0.1f);
            return limit >= currentLayer1ObjCount;
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
                foreach (var nodeInfo in normalSpyPositions)
                {
                    if (nodeInfo.position.IsSamePosition(new Vector3(x, y, 0)))
                    {
                        hasSpy = true;
                    }
                }
            }
            
            if (bossSpyPositions.Count > 0)
            {
                foreach (var nodInfo in bossSpyPositions)
                {
                    if (nodInfo.position.IsSamePosition(new Vector3(x, y, 0)))
                    {
                        hasSpy = true;
                    }
                }
            }

            if (itemPositions.Count > 0)
            {
                foreach (var nodeInfo in itemPositions)
                {
                    if (nodeInfo.position.IsSamePosition(new Vector3(x, y, 0)))
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

        private void SetObjectLayer(GameObject targetObj, LayerType layerType)
        {
            targetObj.layer = LayerMask.NameToLayer(layerType.LayerTypeToString());
            targetObj.GetComponent<SpriteRenderer>().sortingLayerName = layerType.LayerTypeToString();
        }

        #endregion
    }
}
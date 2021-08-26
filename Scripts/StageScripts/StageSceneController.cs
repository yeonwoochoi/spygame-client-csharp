using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Transform initPlayerTransform;
        [SerializeField] private LineRenderer line;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject normalSpy;
        [SerializeField] private GameObject bossSpy;
        [SerializeField] private GameObject item;
        
        private JoystickMoveController joystickMoveController;
        private Transform playerObjParent;
        private Transform spyObjParent;
        private Transform boxObjParent;
        
        private Stage currentStage;

        private List<Vector3> normalSpyPositions;
        private List<Vector3> bossSpyPositions;
        private List<Vector3> itemPositions;
        private Vector3 nodeSize;

        public Qna[] qna;

        private EControlType eControlType;
        
        public static event EventHandler<OpenStageMissionPopupEventArgs> OpenStageMissionPopupEvent;

        protected override void Start()
        {
            StagePauseController.ExitStageEvent += ExitStage;
            StageDonePopupController.ExitStageEvent += ExitStage;
            StartCoroutine(ShowMissionPopup());
        }

        private void OnDestroy()
        {
            StagePauseController.ExitStageEvent -= ExitStage;
            StageDonePopupController.ExitStageEvent -= ExitStage;
        }

        public void SetStageObjParent(Transform playerParent, Transform spyParent, Transform boxParent)
        {
            playerObjParent = playerParent;
            spyObjParent = spyParent;
            boxObjParent = boxParent;
        }
        
        public void SetCurrentStage(Stage stage, JoystickMoveController joystick, EControlType e)
        {
            currentStage = stage;
            eControlType = e;

            nodeSize = tilemap.transform.localScale / 2;
            nodeSize.z = 0;

            // Instantiate player and setting controller
            var playerObj = Instantiate(player, initPlayerTransform.position + nodeSize, Quaternion.identity);
            playerObj.transform.SetParent(playerObjParent);
            var playerMoveController = playerObj.GetComponent<PlayerMoveController>();
            playerMoveController.SetPlayer();
            playerMoveController.Tilemap = tilemap;

            // Set joystick
            joystickMoveController = joystick;
            joystickMoveController.SetJoystick(playerMoveController, eControlType);
            
            // Set line generator
            if (eControlType == EControlType.Mouse)
            {
                var lineGenerator = gameObject.AddComponent<LineGenerator>();
                lineGenerator.Init(tilemap, line, playerObj, eControlType);
            }

            // Set Camera offset
            UnityEngine.Camera.main.GetComponent<CameraFollowController>().SetOffset(playerObj.transform);

            // Init stage game object positions
            normalSpyPositions = new List<Vector3>();
            bossSpyPositions = new List<Vector3>();
            itemPositions = new List<Vector3>();
            
            // Set spies and items
            SetSpy(stage.goalNormalSpyCount, stage.goalBossSpyCount);
            SetItem();
        }
        

        private void SetSpy(int goalNormalSpyCount, int goalBossSpyCount)
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
                spyMoveController.Tilemap = tilemap;
                spyMoveController.Spy = new Spy(i+1000, SpyType.Normal, GetRandomQna(QnaDifficulty.Easy), currentSetNormalSpyCount >= goalNormalSpyCount);
                currentSetNormalSpyCount++;
            }
            
            for (var i = 0; i < bossSpyPositions.Count; i++)
            {
                var pos = bossSpyPositions[i];
                var spyObj = Instantiate(bossSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(spyObjParent);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.Tilemap = tilemap;
                spyMoveController.Spy = new Spy(i+2000, SpyType.Boss, GetRandomQna(QnaDifficulty.Hard), currentSetBossSpyCount >= goalBossSpyCount);
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
                itemBoxController.Item = new Item(i+3000, GetRandomQna((QnaDifficulty) Random.Range(0, 2)));
            }
        }

        private void SetSpyPositions()
        {
            for (var i = 0; i < currentStage.normalSpyCount; i++)
            {
                normalSpyPositions.Add(GetPossiblePosition());
            }
            for (var i = 0; i < currentStage.bossSpyCount; i++)
            {
                bossSpyPositions.Add(GetPossiblePosition());
            }
        }

        private void SetItemPositions()
        {
            for (var i = 0; i < currentStage.boxCount; i++)
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
            EmitOpenStageMissionPopupEvent(new OpenStageMissionPopupEventArgs(currentStage));
        }
        
        private void EmitOpenStageMissionPopupEvent(OpenStageMissionPopupEventArgs e)
        {
            if (OpenStageMissionPopupEvent == null) return;
            foreach (var invocation in OpenStageMissionPopupEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }
        
        private void ExitStage(object _, ExitStageEventArgs e)
        {
            OnClickExitBtn(currentStage.chapterType);
        }

        private void OnClickExitBtn(ChapterType chapterType)
        {
            LoadingManager.Instance.CurrentType = MainSceneType.Play;
            LoadingManager.Instance.NextType = MainSceneType.Select;
            LoadingManager.Instance.LoadingType = LoadingType.Normal;
            LoadingManager.Instance.ChapterType = chapterType;

            StartCoroutine(StartLoadingAnimator(
                () => nextScene = SceneNameManager.SCENE_NORMAL_LOADING, 
                () => SceneManager.LoadScene(nextScene)));
        }

        private Qna GetRandomQna(QnaDifficulty qnaDifficulty)
        {
            var result = new List<Qna>();

            if (qnaDifficulty == QnaDifficulty.Easy)
            {
                result = qna.Where(qna => qna.difficulty == "Easy").ToList();
                return result[Random.Range(0, result.Count)];
            }
            else
            {
                result = qna.Where(qna => qna.difficulty == "Hard").ToList();
                return result[Random.Range(0, result.Count)];
            }
        }
    }
}
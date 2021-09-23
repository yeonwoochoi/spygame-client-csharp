using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Base;
using Camera;
using Control.Item;
using Control.Movement;
using Control.Pointer;
using Domain;
using Domain.StageObj;
using Event;
using Http;
using Manager;
using Manager.Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using Util;
using Random = UnityEngine.Random;

namespace TutorialScripts
{
    public class TutorialStageSpawner: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private PointerController pointerController;
        
        [SerializeField] private Transform initPlayerTransform;
        [SerializeField] private Transform[] initNormalSpyTransforms;
        [SerializeField] private Transform[] initBossSpyTransforms;
        [SerializeField] private Transform[] initBoxTransforms;

        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private JoystickMoveController joystickMoveController;
        [SerializeField] private Tilemap tilemap;

        [SerializeField] private GameObject player;
        [SerializeField] private GameObject normalSpy;
        [SerializeField] private GameObject bossSpy;
        [SerializeField] private GameObject box;
        [SerializeField] private Transform parent;
        
        private List<Qna> qna;
        [HideInInspector] public Transform playerTransform;

        #endregion
        
        #region Static Variables

        public static readonly int time = 80;

        public static readonly int normalSpyCount = 3;
        public static readonly int bossSpyCount = 2;
        public static readonly int goalNormalSpyCount = 2;
        public static readonly int goalBossSpyCount = 1;

        private static readonly int boxCount = 1;

        #endregion

        #region Getter

        public Transform GetPlayerTransform()
        {
            return playerTransform;
        }

        public Transform GetInitSpyPosition()
        {
            return initNormalSpyTransforms[0];
        }
        public Transform GetInitBoxPosition()
        {
            return initBoxTransforms[0];
        }
        public PointerController GetPointerController()
        {
            return pointerController;
        }
        
        #endregion

        #region Public Method

        public void Init()
        {
            // Instantiate player and setting controller
            var playerObj = Instantiate(player, initPlayerTransform.position, Quaternion.identity);
            playerTransform = playerObj.transform;
            playerTransform.SetParent(parent);
            var playerMoveController = playerObj.GetComponent<PlayerMoveController>();
            playerMoveController.Init();

            // Set joystick
            var eControlManager = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL);
            eControlManager.eControlType = EControlType.KeyBoard;
            GlobalDataManager.Instance.Set(GlobalDataKey.ECONTROL, eControlManager);
            joystickMoveController.SetJoystick(playerMoveController, eControlManager.eControlType);

            // Set Camera offset
            mainCamera.GetComponent<CameraFollowController>().SetOffset(playerTransform);
            
            pointerController.Init(mainCamera);

            // Set spies and items
            qna = QnaManager.Instance.tutorialQna;
            SetSpy();
            SetItem();
        }

        #endregion

        #region Private Method

        private void SetSpy()
        {
            for (var i = 0; i < normalSpyCount+1; i++)
            {
                var pos = initNormalSpyTransforms[i].position;
                var spyObj = Instantiate(normalSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(parent);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.SetTilemap(tilemap);
                spyMoveController.Init(new Spy(i+1000, SpyType.Normal, GetRandomQna(QnaDifficulty.Easy), false, i < goalNormalSpyCount), i == 0);
            }
            
            for (var i = 0; i < bossSpyCount; i++)
            {
                var pos = initBossSpyTransforms[i].position;
                var spyObj = Instantiate(bossSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(parent);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.SetTilemap(tilemap);
                spyMoveController.Init(new Spy(i+2000, SpyType.Boss, GetRandomQna(QnaDifficulty.Hard), false, i < goalBossSpyCount));
            }
        }
        
        private void SetItem()
        {
            for (var i = 0; i < boxCount + 1; i++)
            {
                var itemObj = Instantiate(box, initBoxTransforms[i].position, Quaternion.identity);
                itemObj.transform.SetParent(parent);
                var itemBoxController = itemObj.GetComponent<ItemBoxController>();
                itemBoxController.Init(new Item(i+3000, GetRandomQna(QnaDifficulty.Easy), false));
            }
        }
        
        private Qna GetRandomQna(QnaDifficulty qnaDifficulty)
        {
            var result = qna.Where(q => q.GetDifficulty() == qnaDifficulty).ToList();
            return result[Random.Range(0, result.Count)];
        }

        #endregion
    }
}
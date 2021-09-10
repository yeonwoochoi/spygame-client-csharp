﻿using System;
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
using Http;
using Manager;
using Manager.Data;
using UI.TutorialScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace TutorialScripts
{
    public class TutorialStageSpawner: MonoBehaviour
    {
        #region Public Variables
        
        [SerializeField] public QuestPointerController questPointerController;
        [SerializeField] public Transform initPlayerTransform;
        [SerializeField] public Transform[] initSpyTransform;
        [SerializeField] public Transform[] initBoxTransform;

        #endregion
        
        #region Private Variables

        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private JoystickMoveController joystickMoveController;
        [SerializeField] private Tilemap tilemap;

        [SerializeField] private GameObject player;
        [SerializeField] private GameObject normalSpy;
        [SerializeField] private GameObject box;
        [SerializeField] private Transform parent;
        
        private List<Qna> qna;

        #endregion
        
        #region Static Variables

        public static readonly int time = 120;
        
        // 맨 처음 sample spy 까지 포함
        public static readonly int spyCount = 3;

        // 맨 처음 sample box 까지 포함
        public static readonly int boxCount = 2;

        #endregion

        #region Public Method

        public void Init()
        {
            // Instantiate player and setting controller
            var playerObj = Instantiate(player, initPlayerTransform.position, Quaternion.identity);
            playerObj.transform.SetParent(parent);
            var playerMoveController = playerObj.GetComponent<PlayerMoveController>();
            playerMoveController.Init();

            // Set joystick
            var eControlManager = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL);
            eControlManager.eControlType = EControlType.KeyBoard;
            GlobalDataManager.Instance.Set(GlobalDataKey.ECONTROL, eControlManager);
            joystickMoveController.SetJoystick(playerMoveController, eControlManager.eControlType);

            // Set Camera offset
            mainCamera.GetComponent<CameraFollowController>().SetOffset(playerObj.transform);
            
            questPointerController.Init(mainCamera);

            // Set spies and items
            qna = QnaManager.Instance.tutorialQna;
            SetSpy();
            SetItem();
        }

        #endregion

        #region Private Method

        private void SetSpy()
        {
            for (var i = 0; i < spyCount; i++)
            {
                var pos = initSpyTransform[i].position;
                var spyObj = Instantiate(normalSpy, pos, Quaternion.identity);
                spyObj.transform.SetParent(parent);
                var spyMoveController = spyObj.GetComponent<SpyMoveController>();
                spyMoveController.SetTilemap(tilemap);
                spyMoveController.Init(new Spy(i+1000, SpyType.Normal, GetRandomQna(QnaDifficulty.Easy)), i == 0);
            }
        }
        
        private void SetItem()
        {
            for (var i = 0; i < boxCount; i++)
            {
                var itemObj = Instantiate(box, initBoxTransform[i].position, Quaternion.identity);
                itemObj.transform.SetParent(parent);
                var itemBoxController = itemObj.GetComponent<ItemBoxController>();
                itemBoxController.Init(new Item(i+2000, GetRandomQna(QnaDifficulty.Hard), false));
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
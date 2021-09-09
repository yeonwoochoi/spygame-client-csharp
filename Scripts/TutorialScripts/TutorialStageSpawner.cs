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
        #region Private Variables

        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private QuestArrowController arrowController;
        [SerializeField] private JoystickMoveController joystickMoveController;
        [SerializeField] private Transform initPlayerTransform;
        [SerializeField] private Transform initSpyTransform;
        [SerializeField] private Transform initBoxTransform;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject normalSpy;
        [SerializeField] private GameObject box;
        [SerializeField] private Transform parent;
        
        private EControlType eControlType;
        private bool isSet = false;
        private List<Qna> qna;

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
            eControlType = EControlType.KeyBoard;
            GlobalDataManager.Instance.Set(GlobalDataKey.ECONTROL, eControlType);
            joystickMoveController.SetJoystick(playerMoveController, eControlType);

            // Set Camera offset
            mainCamera.GetComponent<CameraFollowController>().SetOffset(playerObj.transform);
            
            arrowController.Init(mainCamera);

            // Set spies and items
            qna = QnaManager.Instance.tutorialQna;
            SetSpy();
            SetItem();
            isSet = true;
        }

        #endregion

        #region Private Method

        private void SetSpy()
        {
            var spyObj = Instantiate(normalSpy, initSpyTransform.position, Quaternion.identity);
            spyObj.transform.SetParent(parent);
            var spyMoveController = spyObj.GetComponent<SpyMoveController>();
            spyMoveController.Init(new Spy(1000, SpyType.Normal, GetRandomQna(QnaDifficulty.Easy), false));
        }

        private void SetItem()
        {
            var boxObj = Instantiate(box, initBoxTransform.position, Quaternion.identity);
            boxObj.transform.SetParent(parent);
            var itemBoxController = boxObj.GetComponent<ItemBoxController>();
            itemBoxController.Init(new Item(2000, GetRandomQna(QnaDifficulty.Hard)));
        }
        
        private Qna GetRandomQna(QnaDifficulty qnaDifficulty)
        {
            var result = qna.Where(q => q.GetDifficulty() == qnaDifficulty).ToList();
            return result[Random.Range(0, result.Count)];
        }

        #endregion
    }
}
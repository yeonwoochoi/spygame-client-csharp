using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using Control.Movement;
using Control.Pointer;
using Domain;
using Domain.Network.Response;
using Event;
using Http;
using Manager;
using Manager.Data;
using UI.TutorialScripts;
using UnityEngine;
using UnityEngine.Networking;
using Util;

namespace Test
{
    public class NetworkTestManager: MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera camera;
        [SerializeField] private GameObject player;
        [SerializeField] private JoystickMoveController joystickMoveController;
        [SerializeField] private PointerController pointerController;
        [SerializeField] private Transform target;

        private EControlManager eControlManager;

        private void Awake()
        {
            if (!GlobalDataManager.Instance.HasKey(GlobalDataKey.ECONTROL))
            {
                eControlManager = EControlManager.Create();
            }
            else
            {
                eControlManager = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL);   
            }
            eControlManager.eControlType = EControlType.KeyBoard;
            GlobalDataManager.Instance.Set(GlobalDataKey.ECONTROL, eControlManager);
        }

        private void Start()
        {
            // Instantiate player and setting controller
            var playerObj = Instantiate(player, Vector3.zero, Quaternion.identity);
            var playerMoveController = playerObj.GetComponent<PlayerMoveController>();
            playerMoveController.Init();
            
            // Set joystick
            joystickMoveController.SetJoystick(playerMoveController, eControlManager.eControlType);
            
            camera.GetComponent<CameraFollowController>().SetOffset(playerObj.transform);

            pointerController.Init(camera);
            pointerController.StartPointing(playerObj.transform, target);
        }

        private IEnumerator GetQnaData()
        {
            var www = HttpFactory.Build(RequestUrlType.Qna);
            yield return www.SendWebRequest();
            
            NetworkManager.HandleResponse(www, out var response, out var errorResponse);

            if (response == null && errorResponse == null)
            {
                NetworkManager.HandleServerError();
                yield break;
            }

            if (response != null)
            {
                foreach (var qna in QnaManager.Instance.qna)
                {
                    Debug.Log($"{qna.difficulty} / {qna.question}");
                }
                yield break;
            }

            var code = errorResponse.GetErrorCode();
            NetworkManager.HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("Tlqkf")
                .Content(code.message)
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
        }

        private IEnumerator GetStageInfo()
        {
            var www = HttpFactory.Build(RequestUrlType.ChapterInfo);
            yield return www.SendWebRequest();
            
            NetworkManager.HandleResponse(www, out var response, out var errorResponse);
            
            if (response == null && errorResponse == null)
            {
                NetworkManager.HandleServerError();
                yield break;
            }

            if (response != null)
            {
                yield break;
            }

            var code = errorResponse.GetErrorCode();
            NetworkManager.HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("Tlqkf")
                .Content(code.message)
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
        }
    }
}
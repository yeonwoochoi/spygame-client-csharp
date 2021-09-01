using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Domain;
using Manager;
using Manager.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Control.Movement
{
    public class LineGenerator: MonoBehaviour
    {
        #region Private Variables

        private EControlType eControlType;
        private Tilemap tilemap;
        private LineRenderer line;
        private PlayerMoveController playerMoveController;
        public List<Vector3> points;
        private bool isSet = false;
        private GameObject player;
        private UnityEngine.Camera mainCamera;

        #endregion

        #region Event Methods

        private void Start()
        {
            points = new List<Vector3>();
            StartCoroutine(StartLineGenerator());
        }

        #endregion

        #region Public Method

        public void Init(Tilemap map, LineRenderer lr, GameObject playerObj, EControlType eControl, UnityEngine.Camera camera)
        {
            tilemap = map;
            line = lr;
            player = playerObj;
            eControlType = eControl;
            mainCamera = camera;
            playerMoveController = playerObj.GetComponent<PlayerMoveController>();
            isSet = true;
        }

        #endregion

        #region Private Method

        private IEnumerator StartLineGenerator()
        {
            if (!isSet || eControlType == EControlType.KeyBoard) yield break;
            if (playerMoveController.objectType != MoveObjectType.Player) yield break;
            while (true)
            {
                yield return null;
                DrawLine();
                ResetLine();   
            }
        }

        private void DrawLine()
        {
            if (!Input.GetMouseButton(0)) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (playerMoveController.GetCurrentState() != MoveStateType.Idle) return;
            
            SetPoints();

            if (points.Count <= 0) return;

            line.positionCount = points.Count;
            for (var i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, points[i]);
            }
        }

        private void ResetLine()
        {
            if (!Input.GetMouseButtonUp(0)) return;
            MovePlayer();
            line.positionCount = 0;
            points = new List<Vector3>();
        }

        private void SetPoints()
        {
            var clickPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var nodePos = playerMoveController.GetNodePosition(tilemap.WorldToCell(clickPoint));
            nodePos.z = 0;

            if (points.Count == 0)
            {
                if (IsStartNode(nodePos))
                {
                    points.Add(nodePos);
                }
            }
            else if (!points.Contains(nodePos))
            {
                points.Add(nodePos);
            }
        }

        private bool IsStartNode(Vector3 nodePosition)
        {
            var nodeSizeX = tilemap.transform.localScale.x / 2;
            var nodeSizeY = tilemap.transform.localScale.y / 2;

            // TODO(?)
            var playerPos = player.GetComponent<Rigidbody2D>().position;

            if (nodePosition.x - nodeSizeX <= playerPos.x && nodePosition.x + nodeSizeX >= playerPos.x)
            {
                if (nodePosition.y - nodeSizeY <= playerPos.y && nodePosition.y + nodeSizeY >= playerPos.y)
                {
                    return true;
                }
            }
            return false;
        }

        private void MovePlayer()
        {
            if (points.Count <= 0) return;
            var path = new List<Vector3>();
            
            for (var i = 1; i < points.Count; i++)
            {
                var point = points[i];
                path.Add(point);
            }
            
            playerMoveController.MovePlayer(path);
        }

        #endregion
    }
}
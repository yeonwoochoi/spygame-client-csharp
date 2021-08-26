using System;
using System.Collections.Generic;
using System.Linq;
using Control.Item;
using Control.Movement;
using Domain;
using Domain.StageObj;
using Event;
using Manager.Data;
using UI.Qna;
using UI.Talking;
using UnityEngine;

namespace Control.Collision
{
    public class DetectorForPlayer: MonoBehaviour
    {
        private List<GameObject> spies;
        private List<GameObject> boxes;
        private EControlType eControlType;
        private bool isSet = false;
        private bool isClicked = false;

        private void Start()
        {
            SpyTalkingUIBehavior.SkipSpyQnaEvent += SkipSpyCapture;
            ItemTalkingUIBehavior.SkipItemQnaEvent += SkipItemOpen;
            SpyQnaPopupBehavior.SkipSpyQnaEvent += SkipSpyCapture;
            ItemQnaPopupBehavior.SkipItemQnaEvent += SkipItemOpen;
            SpyQnaPopupBehavior.CaptureSpyEvent += RemoveCapturedSpy;
            ItemQnaPopupBehavior.ItemGetEvent += RemoveOpenedItemBox;
        }

        /*
        private void Update()
        {
            var result = "";
            if (spies.Count > 0)
            {
                result = spies.Aggregate(result, (current, spy) => current + $"Spy {spy.GetComponent<SpyMoveController>().Spy.index}\r\n");
            }

            if (boxes.Count > 0)
            {
                result = boxes.Aggregate(result, (current, box) => current + $"Box {box.GetComponent<ItemBoxController>().Item.index}\r\n");
            }

            if (spies.Count > 0 || boxes.Count > 0)
            {
                Debug.Log(result);   
            }
        }
        */

        private void OnDestroy()
        {
            SpyTalkingUIBehavior.SkipSpyQnaEvent -= SkipSpyCapture;
            ItemTalkingUIBehavior.SkipItemQnaEvent -= SkipItemOpen;
            SpyQnaPopupBehavior.SkipSpyQnaEvent -= SkipSpyCapture;
            ItemQnaPopupBehavior.SkipItemQnaEvent -= SkipItemOpen;
            SpyQnaPopupBehavior.CaptureSpyEvent -= RemoveCapturedSpy;
            ItemQnaPopupBehavior.ItemGetEvent -= RemoveOpenedItemBox;
        }

        public void Set(EControlType e)
        {
            spies = new List<GameObject>();
            boxes = new List<GameObject>();
            eControlType = e;
            isSet = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidTrigger()) return;
            boxes ??= new List<GameObject>();
            spies ??= new List<GameObject>();
            
            if (other.gameObject.TryGetComponent(out SpyMoveController spyMoveController))
            {
                if (!spies.Contains(other.gameObject))
                {
                    spies.Add(other.gameObject);
                }
            }
            
            if (other.gameObject.TryGetComponent(out ItemBoxController itemBoxController))
            {
                if (!boxes.Contains(other.gameObject) && !itemBoxController.IsOpen)
                {
                    boxes.Add(other.gameObject);   
                }
            }
        }

        /*
        private void OnTriggerStay2D(Collider2D other)
        {
            if (boxes.Count == 0 || spies.Count == 0) return;
            
            if (other.gameObject.TryGetComponent(out SpyMoveController spyMoveController))
            {
                if (spies.Contains(other.gameObject))
                {
                    Debug.Log($"Error spy : {spyMoveController.Spy.index}");
                }
            }

            if (other.gameObject.TryGetComponent(out ItemBoxController itemBoxController))
            {
                if (boxes.Contains(other.gameObject))
                {
                    Debug.Log($"Error item : {itemBoxController.Item.index}");
                }
            }
        }
        */

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsValidTrigger()) return;
            boxes ??= new List<GameObject>();
            spies ??= new List<GameObject>();
            
            if (other.gameObject.TryGetComponent(out SpyMoveController spyMoveController))
            {
                if (spies.Contains(other.gameObject))
                {
                    spies.Remove(other.gameObject);
                }
            }
            
            if (other.gameObject.TryGetComponent(out ItemBoxController itemBoxController))
            {
                if (boxes.Contains(other.gameObject))
                {
                    boxes.Remove(other.gameObject);
                }
            }
        }

        public void OnClickActionBtn()
        {
            if (!IsValidTrigger()) return;
            if (eControlType != EControlType.KeyBoard) return;
            var totalObjs = spies.Concat(boxes).ToList();
            if (totalObjs.Count == 0) return;
            if (totalObjs.Count == 1) EmitEvent(totalObjs[0]);

            var proximateObj = totalObjs[0];
            var distance = (transform.position - totalObjs[0].transform.position).sqrMagnitude;
            
            for (var i = 1; i < totalObjs.Count; i++)
            {
                var newDistance = (transform.position - totalObjs[i].transform.position).sqrMagnitude;
                if (distance > newDistance)
                {
                    distance = newDistance;
                    proximateObj = totalObjs[i];
                }
            }
            EmitEvent(proximateObj);
        }

        private void SkipSpyCapture(object _, SkipSpyQnaEventArgs e)
        {
            isClicked = false;
        }

        private void SkipItemOpen(object _, SkipItemQnaEventArgs e)
        {
            isClicked = false;
        }
        
        private void RemoveCapturedSpy(object _, CaptureSpyEventArgs e)
        {
            if (!IsValidTrigger()) return;
            if (spies.Count == 0) return;
            spies.RemoveAll(target => target.GetComponent<SpyMoveController>().Spy.index == e.spy.index);
            isClicked = false;
        }

        private void RemoveOpenedItemBox(object _, ItemGetEventArgs e)
        {
            if (!IsValidTrigger()) return;
            if (boxes.Count == 0) return;
            boxes.RemoveAll(target => target.GetComponent<ItemBoxController>().Item.index == e.item.index);
            isClicked = false;
        }
        
        
        private void EmitEvent(GameObject target)
        {
            if (isClicked) return;
            isClicked = true;
            if (target.TryGetComponent(out ItemBoxController itemBoxController))
            {
                itemBoxController.boxSpeechBalloonController.EmitOpenItemQnaEvent(new OpenItemQnaEventArgs(itemBoxController.Item));
                return;
            }

            if (target.TryGetComponent(out SpyMoveController spyMoveController))
            {
                spyMoveController.speechBalloonController.EmitOpenSpyQnaEvent(new OpenSpyQnaEventArgs(spyMoveController.Spy));
            }
        }

        private bool IsValidTrigger()
        {
            if (!isSet) return false;
            return eControlType == EControlType.KeyBoard;
        }
    }
}
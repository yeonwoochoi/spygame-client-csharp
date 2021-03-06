using System.Collections.Generic;
using System.Linq;
using Base;
using Control.Item;
using Control.Movement;
using Domain;
using Event;
using UI.Popup.Qna;
using UnityEngine;

namespace Control.Collision
{
    public class PlayerDetectorBehavior: BaseDetectorBehavior
    {
        #region Private Variables

        private List<GameObject> spies;
        private List<GameObject> boxes;
        private bool isClicked = false;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            SpyQnaPopupBehavior.SkipSpyQnaEvent += SkipSpyCapture;
            ItemQnaPopupBehavior.SkipItemQnaEvent += SkipItemOpen;
            SpyQnaPopupBehavior.CaptureSpyEvent += RemoveCapturedSpy;
            ItemQnaPopupBehavior.ItemGetEvent += RemoveOpenedItemBox;
        }

        private void OnDisable()
        {
            SpyQnaPopupBehavior.SkipSpyQnaEvent -= SkipSpyCapture;
            ItemQnaPopupBehavior.SkipItemQnaEvent -= SkipItemOpen;
            SpyQnaPopupBehavior.CaptureSpyEvent -= RemoveCapturedSpy;
            ItemQnaPopupBehavior.ItemGetEvent -= RemoveOpenedItemBox;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValidTrigger()) return;
            
            if (other.gameObject.TryGetComponent(out SpyMoveController spyMoveController))
            {
                if (!spies.Contains(other.gameObject))
                {
                    spies.Add(other.gameObject);
                }
            }
            
            if (other.gameObject.TryGetComponent(out ItemBoxController itemBoxController))
            {
                if (!boxes.Contains(other.gameObject) && !itemBoxController.GetIsOpen())
                {
                    boxes.Add(other.gameObject);   
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsValidTrigger()) return;
            
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

        #endregion

        #region Protected Method

        protected override void InitDetector()
        {
            base.InitDetector();
            spies ??= new List<GameObject>();
            boxes ??= new List<GameObject>();
            var playerMoveController = GetParentController<PlayerMoveController>();
            playerMoveController.onClickActionBtn = OnClickActionBtn;
            isSet = true;
        }

        #endregion

        #region Private Methods

        private void OnClickActionBtn()
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
            spies.RemoveAll(target => target.GetComponent<SpyMoveController>().GetSpy().index == e.spy.index);
            isClicked = false;
        }

        private void RemoveOpenedItemBox(object _, ItemGetEventArgs e)
        {
            if (!IsValidTrigger()) return;
            if (boxes.Count == 0) return;
            boxes.RemoveAll(target => target.GetComponent<ItemBoxController>().GetItem().index == e.item.index);
            isClicked = false;
        }
        
        // TODO (Event) : 여기에 OpenQnaPopupEvent를 하나 더 만들어야함 원래 (이건 Keyboard 조작방식에서.. 저 speechBalloonController의 OpenQnaPopupEvent는 Mouse 조작방식에서)
        private void EmitEvent(GameObject target)
        {
            if (isClicked) return;
            isClicked = true;
            if (target.TryGetComponent(out ItemBoxController itemBoxController))
            {
                itemBoxController.boxSpeechBalloonController.OpenItemQnaPopup(new OpenItemQnaEventArgs { item = itemBoxController.GetItem() });
                return;
            }

            if (target.TryGetComponent(out SpyMoveController spyMoveController))
            {
                spyMoveController.speechBalloonController.OpenSpyQnaPopup(new OpenSpyQnaPopupEventArgs{ spy = spyMoveController.GetSpy() });
            }
        }

        private bool IsValidTrigger()
        {
            if (!isSet) return false;
            return eControlType == EControlType.KeyBoard;
        }

        #endregion
    }
}
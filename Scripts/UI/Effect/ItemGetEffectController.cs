using System.Collections;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using UI.Hud;
using UI.Popup.Qna;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Effect
{
    public class ItemGetEffectController: MonoBehaviour
    {
        [SerializeField] private ItemInventoryHudController inventoryHudController;
        [SerializeField] private GameObject itemObj;
        [SerializeField] private RectTransform hpItemBtnRect;
        [SerializeField] private RectTransform timerItemBtnRect;
        
        [SerializeField] private Sprite hpImage;
        [SerializeField] private Sprite timerImage;

        private RectTransform itemRect;
        private Image itemImg;
        private CanvasGroup cGroup;
        private Vector3 initPosition;

        private bool isClicked = false;

        private readonly Vector2 itemGoalSize = Vector2.one * 1.5f;
        private readonly float speed = 7f;

        #region Event Methods

        private void Start()
        {
            itemRect = itemObj.GetComponent<RectTransform>();
            itemImg = itemObj.GetComponent<Image>();
            cGroup = itemObj.GetComponent<CanvasGroup>();
            initPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            ItemQnaPopupBehavior.ItemGetEvent += StartItemGetAnim;
        }

        private void OnDisable()
        {
            ItemQnaPopupBehavior.ItemGetEvent -= StartItemGetAnim;
        }

        #endregion

        #region Private Method

        private void StartItemGetAnim(object _, ItemGetEventArgs e)
        {
            if (isClicked) return;
            if (e.type == ItemGetType.Miss)
            {
                AudioManager.instance.Play(SoundType.Wrong);
                return;
            }
            AudioManager.instance.Play(SoundType.Correct);
            isClicked = true;
            itemImg.sprite = e.item.type == ItemType.Hp ? hpImage : timerImage;
            cGroup.Visible();
            StartCoroutine(StartItemGetEffect(e.item));
        }

        private IEnumerator StartItemGetEffect(Item item)
        {
            itemRect.position = initPosition;
            itemRect.localScale = Vector3.zero;

            var temp = 0.05f;

            while (true)
            {
                yield return null;
                if (itemRect.localScale.x >= itemGoalSize.x - temp && itemRect.localScale.y >= itemGoalSize.y - temp)
                {
                    itemRect.localScale = itemGoalSize;
                    break;
                }

                itemRect.localScale = Vector2.Lerp(itemRect.localScale, itemGoalSize, Time.deltaTime * speed);
            }

            var destinationPos = item.type == ItemType.Hp ? hpItemBtnRect.position : timerItemBtnRect.position;

            while (true)
            {
                yield return null;
                if ((destinationPos - itemRect.position).sqrMagnitude <= temp)
                {
                    itemRect.localScale = Vector3.zero;
                    break;
                }

                itemRect.position = Vector3.Lerp(itemRect.position, destinationPos, Time.deltaTime * speed);
                itemRect.localScale = Vector2.Lerp(itemRect.localScale, Vector3.zero, Time.deltaTime * speed);
            }

            inventoryHudController.GetItem(item);
            cGroup.Visible(false);
            isClicked = false;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using StageScripts;
using UI.Qna;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Stage
{
    public class ItemTabController: MonoBehaviour
    {
        [SerializeField] private Button hpItemBtn;
        [SerializeField] private Button timeUpItemBtn;
        [SerializeField] private Text hpItemCountText;
        [SerializeField] private Text timeItemCountText;

        private int currentHp;

        private Dictionary<ItemType, List<Item>> itemRepository;
        
        public static event EventHandler<ItemUseEventArgs> ItemUseEvent;

        private void Start()
        {
            StageStateController.UpdateStageStateEvent += UpdateStageState;
            ItemQnaPopupBehavior.ItemGetEvent += GetItem;
            InitItemRepository();
            SetItemUseBtnEvent();
            UpdateItemCountText();
        }

        private void OnDestroy()
        {
            StageStateController.UpdateStageStateEvent -= UpdateStageState;
            ItemQnaPopupBehavior.ItemGetEvent -= GetItem;
        }

        private void InitItemRepository()
        {
            var itemTypeCount = Enum.GetValues(typeof(ItemType)).Length;
            itemRepository = new Dictionary<ItemType, List<Item>>();
            
            for (var i = 0; i < itemTypeCount; i++)
            {
                var type = (ItemType) i;
                itemRepository[type] = new List<Item>();
            }
        }
        

        private void GetItem(object _, ItemGetEventArgs e)
        {
            if (e.type == ItemGetType.Miss)
            {
                AudioManager.instance.Play(SoundType.Wrong);
            }
            else
            {
                AudioManager.instance.Play(SoundType.Correct);
                itemRepository[e.item.type].Add(e.item);
                UpdateItemCountText();
            }
        }

        private void SetItemUseBtnEvent()
        {
            hpItemBtn.onClick.AddListener(() =>
            {
                UseItem(ItemType.Hp);
            });
            
            timeUpItemBtn.onClick.AddListener(() =>
            {
                UseItem(ItemType.Time);
            });
        }

        private void UseItem(ItemType type)
        {
            if (itemRepository[type].Count <= 0) return;
            if (type == ItemType.Hp && currentHp == StageStateController.playerHp) return;
            EmitItemUseEvent(new ItemUseEventArgs(itemRepository[type][0]));
            itemRepository[type].RemoveAt(0);
            UpdateItemCountText();
            AudioManager.instance.Play(SoundType.ItemUse);
        }

        private void EmitItemUseEvent(ItemUseEventArgs e)
        {
            if (ItemUseEvent == null) return;
            foreach (var invocation in ItemUseEvent.GetInvocationList())
            {
                invocation.DynamicInvoke(this, e);
            }
        }

        private void UpdateStageState(object _, UpdateStageStateEventArgs e)
        {
            currentHp = e.hp;
        }

        private void UpdateItemCountText()
        {
            hpItemCountText.text = $"{itemRepository[ItemType.Hp].Count}";
            timeItemCountText.text = $"{itemRepository[ItemType.Time].Count}";
        }
    }
}
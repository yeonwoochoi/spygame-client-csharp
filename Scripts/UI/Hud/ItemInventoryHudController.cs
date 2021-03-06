using System;
using System.Collections.Generic;
using Domain;
using Domain.StageObj;
using Event;
using Manager;
using Manager.Data;
using StageScripts;
using TutorialScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hud
{
    public class ItemInventoryHudController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Button hpItemBtn;
        [SerializeField] private Button timeUpItemBtn;
        [SerializeField] private Text hpItemCountText;
        [SerializeField] private Text timeItemCountText;

        private int currentHp;
        private Dictionary<ItemType, List<Item>> itemRepository;

        private bool isTutorial = false;
        private bool isTutorialGameStart = false;

        #endregion

        #region Event

        public static event EventHandler<ItemUseEventArgs> ItemUseEvent;

        #endregion

        #region Event Methods

        private void Start()
        {
            TutorialSceneController.StartTutorialGameEvent += StartTutorialGame;
            StageStateController.UpdateStageStateEvent += UpdateStageState;
            InitItemRepository();
            SetItemUseBtnEvent();
            UpdateItemCountText();
        }

        private void OnDisable()
        {
            TutorialSceneController.StartTutorialGameEvent -= StartTutorialGame;
            StageStateController.UpdateStageStateEvent -= UpdateStageState;
        }

        #endregion

        #region Public Method
        
        public void GetItem(Item item)
        {
            itemRepository[item.type].Add(item);
            UpdateItemCountText();
        }

        #endregion

        #region Private Methods

        private void InitItemRepository()
        {
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
            
            var itemTypeCount = Enum.GetValues(typeof(ItemType)).Length;
            itemRepository = new Dictionary<ItemType, List<Item>>();
            
            for (var i = 0; i < itemTypeCount; i++)
            {
                var type = (ItemType) i;
                itemRepository[type] = new List<Item>();
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
            if (isTutorial && !isTutorialGameStart) return;
            if (itemRepository[type].Count <= 0) return;
            if (type == ItemType.Hp && currentHp == StageStateController.PlayerHp) return;
            EmitItemUseEvent(new ItemUseEventArgs { item = itemRepository[type][0] });
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

        private void StartTutorialGame(object _, StartTutorialGameEventArgs e)
        {
            isTutorialGameStart = true;
        }

        private void UpdateItemCountText()
        {
            hpItemCountText.text = $"{itemRepository[ItemType.Hp].Count}";
            timeItemCountText.text = $"{itemRepository[ItemType.Time].Count}";
        }

        #endregion
    }
}
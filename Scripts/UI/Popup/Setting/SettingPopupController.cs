using Domain;
using Manager;
using Manager.Data;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.Setting
{
    public class SettingPopupController: BasePopupBehavior
    {
        #region Private Variables

        [SerializeField] private Button soundButton;
        [SerializeField] private Button effectButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button mouseControlButton;
        [SerializeField] private Text mouseControlButtonText;
        [SerializeField] private Button keyboardControlButton;
        [SerializeField] private Text keyboardControlButtonText;

        private SoundManager soundManager;
        private EControlManager eControlManager;

        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            soundManager = GlobalDataManager.Instance.Get<SoundManager>(GlobalDataKey.SOUND);
            eControlManager = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL);
            SetButtonEvent();
        }

        #endregion

        #region Public Method

        public void OnClickSettingButton()
        {
            OnOpenPopup();    
        }

        #endregion

        #region Private Methods

        private void SetButtonEvent()
        {
            exitButton.onClick.AddListener(OnClosePopup);
            soundButton.onClick.AddListener(OnClickSoundButton);
            effectButton.onClick.AddListener(OnClickEffectButton);
            mouseControlButton.onClick.AddListener(OnClickMouseControlButton);
            keyboardControlButton.onClick.AddListener(OnClickKeyboardControlButton);
            
            soundButton.GetComponent<ImageChangeButtonController>().Init(!soundManager.isSoundMute);
            effectButton.GetComponent<ImageChangeButtonController>().Init(!soundManager.isEffectMute);

            ActivateButton();
        }

        private void OnClickSoundButton()
        {
            soundManager.isSoundMute = !soundManager.isSoundMute;
            GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, soundManager);
        }

        private void OnClickEffectButton()
        {
            soundManager.isEffectMute = !soundManager.isEffectMute;
            GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, soundManager);
        }

        private void OnClickMouseControlButton()
        {
            eControlManager.eControlType = EControlType.Mouse;
            GlobalDataManager.Instance.Set(GlobalDataKey.ECONTROL, eControlManager);
            ActivateButton();
        }

        private void OnClickKeyboardControlButton()
        {
            eControlManager.eControlType = EControlType.KeyBoard;
            GlobalDataManager.Instance.Set(GlobalDataKey.ECONTROL, eControlManager);
            ActivateButton(); 
        }

        private void ActivateButton()
        {
            var isKeyboard = eControlManager.eControlType == EControlType.KeyBoard;
            mouseControlButton.image.color = isKeyboard ? new Color(0.6f, 0.6f, 0.6f, 1f) : Color.white;
            mouseControlButtonText.color = isKeyboard ? new Color(1f, 0.92f, 0.78f, 0.6f) : new Color(1f, 0.92f, 0.78f, 1f);
            
            keyboardControlButton.image.color = !isKeyboard ? new Color(0.6f, 0.6f, 0.6f, 1f) : Color.white;
            keyboardControlButtonText.color = !isKeyboard ? new Color(1f, 0.92f, 0.78f, 0.6f) : new Color(1f, 0.92f, 0.78f, 1f);
        }

        #endregion
    }
}
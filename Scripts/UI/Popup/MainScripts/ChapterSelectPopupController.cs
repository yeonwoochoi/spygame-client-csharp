using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup.MainScripts
{
    public class ChapterSelectPopupController: BasePopupBehavior
    {
        #region Private Variable

        [SerializeField] private Button exitButton;

        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            exitButton.onClick.AddListener(OnClosePopup);
        }

        #endregion

        #region Public Method

        public void OnClickChapterSelectButton()
        {
            OnOpenPopup();    
        }

        #endregion
    }
}
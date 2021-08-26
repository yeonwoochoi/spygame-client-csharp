using Event;
using MainScripts;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Main
{
    public class ChapterSelectPopupController: BasePopupBehavior
    {
        [SerializeField] private Button exitButton;

        protected override void Start()
        {
            base.Start();
            exitButton.onClick.AddListener(OnClosePopup);
        }

        public void OnClickChapterSelectButton()
        {
            OnOpenPopup();    
        }
    }
}
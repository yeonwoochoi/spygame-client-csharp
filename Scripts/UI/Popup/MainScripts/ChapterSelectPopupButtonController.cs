using Domain;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Popup.MainScripts
{
    public class ChapterSelectPopupButtonController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Text titleText;
        [SerializeField] private Image chapterImage;
        [SerializeField] private CanvasGroup lockCanvasGroup;
        
        private bool isLocked;

        #endregion

        #region Getter

        public bool GetIsLocked()
        {
            return isLocked;
        }

        #endregion

        #region Setter

        private void SetIsLocked(bool flag)
        {
            isLocked = flag;
            lockCanvasGroup.Visible(isLocked);
        }

        #endregion

        #region Public Method

        public void SetChapterSelectButtons(ChapterInfo chapterInfo, Sprite chapterSprite, bool flag)
        {
            SetIsLocked(flag);
            titleText.text = chapterInfo.title;
            chapterImage.sprite = chapterSprite;
        }

        #endregion
    }
}
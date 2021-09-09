using System;
using Domain;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.MainScripts
{
    public class ChapterSelectPopupButtonController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Text titleText;
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

        public void SetChapterSelectButtons(ChapterInfo chapterInfo, bool flag)
        {
            SetIsLocked(flag);
            titleText.text = chapterInfo.title;
        }

        #endregion
    }
}
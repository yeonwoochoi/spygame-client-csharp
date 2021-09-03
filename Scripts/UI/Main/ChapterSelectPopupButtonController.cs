using System;
using Domain;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Main
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

        public void SetChapterSelectButtons(Domain.Chapter chapter)
        {
            SetIsLocked(chapter.isLocked);
            titleText.text = $"Chapter {(int) chapter.chapterType + 1}";
        }

        #endregion
    }
}
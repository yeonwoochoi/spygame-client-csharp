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

        public bool IsLocked
        {
            get => isLocked;
            private set
            {
                isLocked = value;
                lockCanvasGroup.Visible(isLocked);
            }
        }

        #region Public Method

        public void SetChapterSelectButtons(Domain.Chapter chapter)
        {
            IsLocked = chapter.isLocked;
            titleText.text = $"Chapter {(int) chapter.chapterType}";
        }

        #endregion
    }
}
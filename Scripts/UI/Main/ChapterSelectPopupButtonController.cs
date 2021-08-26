using System;
using Domain;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Main
{
    public class ChapterSelectPopupButtonController: MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private CanvasGroup lockCanvasGroup;
        
        private bool isLocked;
        
        public bool IsLocked
        {
            get => isLocked;
            set
            {
                isLocked = value;
                lockCanvasGroup.Visible(isLocked);
            }
        }
        
        public void SetChapterSelectButtons(Domain.Chapter chapter)
        {
            IsLocked = chapter.isLocked;
            SetTitleText(chapter.chapterType);
        }

        private void SetTitleText(ChapterType type)
        {
            var title = "";
            switch (type)
            {
                case ChapterType.Chapter1:
                    title = "Chapter 1";
                    break;
                case ChapterType.Chapter2:
                    title = "Chapter 2";
                    break;
                case ChapterType.Chapter3:
                    title = "Chapter 3";
                    break;
                case ChapterType.Chapter4:
                    title = "Chapter 4";
                    break;
                case ChapterType.Chapter5:
                    title = "Chapter 5";
                    break;
                case ChapterType.Chapter6:
                    title = "Chapter 6";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            titleText.text = title;
        }
    }
}
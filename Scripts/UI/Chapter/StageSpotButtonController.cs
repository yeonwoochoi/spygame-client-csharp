using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Chapter
{
    public class StageSpotButtonController: MonoBehaviour
    {
        [SerializeField] private CanvasGroup leftStarImage;
        [SerializeField] private CanvasGroup middleStarImage;
        [SerializeField] private CanvasGroup rightStarImage;

        private int stageScore;
        public int StageScore
        {
            get => stageScore;
            set
            {
                stageScore = value;
                ShowStars(value);
            }
        }
        
        public void ShowStars(int score)
        {
            if (score > 3) score = 3;
            if (score <= 0)
            {
                ResetStar();
                return;
            }

            if (score >= 1)
            {
                leftStarImage.Visible();
            }
            if (score >= 2)
            {
                middleStarImage.Visible();
            }
            if (score >= 3)
            {
                rightStarImage.Visible();
            }
        }

        private void ResetStar()
        {
            leftStarImage.Visible(false);
            middleStarImage.Visible(false);
            rightStarImage.Visible(false);
        }
    }
}
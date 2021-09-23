using UnityEngine;
using Util;

namespace UI
{
    public class StageSpotButtonController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private CanvasGroup leftStarImage;
        [SerializeField] private CanvasGroup middleStarImage;
        [SerializeField] private CanvasGroup rightStarImage;

        private int stageScore;

        #endregion
        
        #region Setter

        public void SetStageScore(int score)
        {
            stageScore = score;
            ShowStars(stageScore);
        }

        #endregion

        #region Public Method

        private void ShowStars(int score)
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

        #endregion

        #region Private Method

        private void ResetStar()
        {
            leftStarImage.Visible(false);
            middleStarImage.Visible(false);
            rightStarImage.Visible(false);
        }

        #endregion
    }
}
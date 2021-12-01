using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI
{
    public class StageSelectButtonController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private CanvasGroup unlockCanvasGroup;
        [SerializeField] private CanvasGroup lockCanvasGroup;
        
        [SerializeField] private Image leftStarImage;
        [SerializeField] private Image middleStarImage;
        [SerializeField] private Image rightStarImage;

        [SerializeField] private Text stageText;

        [SerializeField] private Sprite emptySprite;
        [SerializeField] private Sprite fillSprite;

        private int stageScore;

        #endregion
        
        #region Setter

        public void SetStageScore(int stage, int score, bool isLocked = true)
        {
            unlockCanvasGroup.Visible(!isLocked);
            lockCanvasGroup.Visible(isLocked);
            
            stageText.text = $"{stage}";
            stageScore = score;
            ShowStars(stageScore);
        }

        #endregion
        
        #region Public Method

        private void ShowStars(int score)
        {
            if (score > 3) score = 3;
            
            ResetStar();
            
            if (score <= 0)
            {
                return;
            }

            if (score >= 1)
            {
                leftStarImage.sprite = fillSprite;
            }
            if (score >= 2)
            {
                middleStarImage.sprite = fillSprite;
            }
            if (score >= 3)
            {
                rightStarImage.sprite = fillSprite;
            }
        }

        #endregion

        #region Private Method

        private void ResetStar()
        {
            leftStarImage.sprite = emptySprite;
            middleStarImage.sprite = emptySprite;
            rightStarImage.sprite = emptySprite;
        }

        #endregion
    }
}
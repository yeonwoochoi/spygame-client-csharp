using System;
using Domain;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Chapter
{
    public class StagePlayReadyPopupController: BasePopupBehavior
    {
        #region Private Variables

        [SerializeField] private StarHandler starHandler;
        [SerializeField] private Image stageMapImage;
        [SerializeField] private Text stageExplanationText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button cancelButton;

        #endregion

        #region Public Method

        public void OpenStagePlayReadyPopup(Domain.Stage currentStage, Action<StageType> loadStageAction)
        {
            titleText.text = $"{currentStage.stageType}";
            starHandler.ShowStars(currentStage.score);
            stageMapImage.sprite = currentStage.mapSprite;
            stageExplanationText.text = $"{currentStage.GetStageInfoText()}";
            playButton.onClick.AddListener(() => { loadStageAction?.Invoke(currentStage.stageType);});
            cancelButton.onClick.AddListener(OnClickCancelButton);
            OnOpenPopup();
        }

        #endregion

        #region Private Method

        private void OnClickCancelButton()
        {
            OnClosePopup();
        }

        #endregion
    }
}
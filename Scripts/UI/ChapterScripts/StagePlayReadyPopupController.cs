using System;
using Domain;
using Manager;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChapterScripts
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

        public void OpenStagePlayReadyPopup(StageInfo stageInfo, int score, Sprite mapPreview, Action<StageType> loadStageAction)
        {
            titleText.text = $"{stageInfo.stageType}";
            starHandler.ShowStars(score);
            stageMapImage.sprite = mapPreview;
            stageExplanationText.text = $"{stageInfo.GetStageMissionText()}";
            playButton.onClick.AddListener(() => { loadStageAction?.Invoke(stageInfo.stageType);});
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
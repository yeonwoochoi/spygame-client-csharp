using System;
using Domain;
using UI.Chapter;
using UnityEngine;
using UnityEngine.UI;

namespace ChapterScripts
{
    public class ChapterButtonController: MonoBehaviour
    {
        #region Private Variable
        [SerializeField] private Button[] stageButtons;
        #endregion
        
        #region Public Methods
        public void SetButtonScore(Func<StageType, int> getScore)
        {
            for (var i = 0; i < stageButtons.Length; i++)
            {
                stageButtons[i].GetComponent<StageSpotButtonController>().StageScore = getScore((StageType) i);
            }
        }

        public void SetStageButtonEvent(Action<StageType> openStageReadyPopupAction)
        {
            for (var i = 0; i < stageButtons.Length; i++)
            {
                var currentStage = (StageType) i;
                stageButtons[i].onClick.AddListener(() =>
                {
                    openStageReadyPopupAction?.Invoke(currentStage);
                });
            }
        }   
        #endregion
    }
}
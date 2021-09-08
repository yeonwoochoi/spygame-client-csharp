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

        public void SetStageButtonEvent(Action<StageType> openStageReadyPopupAction, ChapterType chapterType)
        {
            for (var i = 0; i < stageButtons.Length; i++)
            {
                var stageType = (StageType) i;
                var stageInfo = ChapterManager.Instance.GetStageInfo(chapterType, stageType);

                stageButtons[i].onClick.AddListener(() =>
                {
                    openStageReadyPopupAction?.Invoke(stageType);
                });

                stageButtons[i].GetComponent<StageSpotButtonController>().SetStageScore(stageInfo.score);
            }
        }   
        #endregion
    }
}
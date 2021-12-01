using System;
using Domain;
using Manager;
using Manager.Data;
using UI;
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
            var beforeStageScore = 0;
            for (var i = 0; i < stageButtons.Length; i++)
            {
                var stageType = (StageType) i;
                var score = GlobalDataManager.Instance.Get<StageScoreManager>(GlobalDataKey.STAGE_SCORE)
                    .GetStageScore(chapterType, stageType);
                
                var isLocked = !(beforeStageScore > 0 || i == 0);

                stageButtons[i].onClick.AddListener(() =>
                {
                    if (isLocked) return;
                    openStageReadyPopupAction?.Invoke(stageType);
                });
                
                //stageButtons[i].GetComponent<StageSpotButtonController>().SetStageScore(score);
                stageButtons[i].GetComponent<StageSelectButtonController>().SetStageScore(i+1, score, isLocked);
                beforeStageScore = score;
            }
        }   
        #endregion
    }
}
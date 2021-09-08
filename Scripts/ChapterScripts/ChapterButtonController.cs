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
                var stageInfo = PseudoChapter.Instance.GetStageInfo(chapterType, stageType);

                stageButtons[i].onClick.AddListener(() =>
                {
                    openStageReadyPopupAction?.Invoke(stageType);
                });
                
                foreach (var stageInfo1 in PseudoChapter.Instance.GetChapterInfo(chapterType).stageInfos)
                {
                    Debug.Log(stageInfo1.boxCount);
                    Debug.Log(stageInfo1.limitTime);
                    Debug.Log(stageInfo1.normalSpyCount);
                    Debug.Log(stageInfo1.bossSpyCount);
                    Debug.Log(stageInfo1.goalNormalSpyCount);
                    Debug.Log(stageInfo1.goalBossSpyCount);
                    Debug.Log(stageInfo1.score);    
                }

                stageButtons[i].GetComponent<StageSpotButtonController>().SetStageScore(stageInfo.score);
            }
        }   
        #endregion
    }
}
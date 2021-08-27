﻿using System;
using Domain;
using UI.Chapter;
using UnityEngine;
using UnityEngine.UI;

namespace ChapterScripts
{
    // TODO
    public class ChapterButtonController: MonoBehaviour
    {
        [SerializeField] private Button[] stageButtons;
        
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
                stageButtons[i].onClick.AddListener(() =>
                {
                    openStageReadyPopupAction.Invoke((StageType) i);
                });
            }
        }   
    }
}
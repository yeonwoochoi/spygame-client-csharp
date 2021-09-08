﻿using System;
using System.Collections.Generic;
using Manager;
using StageScripts;
using UnityEngine;

namespace Domain
{
    #region Enums

    public enum StageType
    {
        Stage1, Stage2, Stage3, Stage4, Stage5, Stage6
    }

    public enum ChapterType
    {
        Chapter1, Chapter2, Chapter3, Chapter4, Chapter5, Chapter6
    }

    #endregion
    
    public static class StageManager{
        public const int totalChapterCounts = 6;
        public static readonly Dictionary<ChapterType, int> totalStageCounts = new Dictionary<ChapterType, int>
        {
            [ChapterType.Chapter1] = 6,
            [ChapterType.Chapter2] = 6,
            [ChapterType.Chapter3] = 6,
            [ChapterType.Chapter4] = 6,
            [ChapterType.Chapter5] = 6,
            [ChapterType.Chapter6] = 6
        };
    }

    [Serializable]
    public class StageScoreManager
    {
        #region Public Variable

        public Dictionary<ChapterType, List<StageScoreInfo>> scoreInfos;
        
        #endregion

        #region Constructor

        public StageScoreManager()
        {
            scoreInfos = new Dictionary<ChapterType, List<StageScoreInfo>>();
            for (var i = 0; i < StageManager.totalChapterCounts; i++)
            {
                var chapterType = (ChapterType) i;
                var stageCount = StageManager.totalStageCounts[chapterType];
                scoreInfos.Add(chapterType, new List<StageScoreInfo>());
            }
        }

            #endregion

        #region Static Method

        public static StageScoreManager Create()
        {
            var stageScoreManager = new StageScoreManager();
            for (var i = 0; i < StageManager.totalChapterCounts; i++)
            {
                var chapterType = (ChapterType) i;
                var stageCount = StageManager.totalStageCounts[chapterType];

                for (var j = 0; j < stageCount; j++)
                {
                    stageScoreManager.scoreInfos[chapterType].Add(new StageScoreInfo
                    {
                        stageType = (StageType) j,
                        score = 0
                    });
                }
            }
            return stageScoreManager;
        }

        #endregion

        #region Getter

        public List<StageScoreInfo> GetStageInfos(ChapterType chapterType)
        {
            return scoreInfos[chapterType];
        }

        public int GetStageScore(ChapterType chapterType, StageType stageType)
        {
            var stageInfos = GetStageInfos(chapterType);
            var result = -1;
            for (var i = 0; i < stageInfos.Count; i++)
            {
                var currentStageType = (StageType) i;
                if (stageType == currentStageType)
                {
                    result = stageInfos[i].score;
                }
            }

            return result;
        }

        #endregion

        #region Setter

        public void SetStageScore(ChapterType chapterType, StageType stageType, int score)
        {
            var stageInfos = GetStageInfos(chapterType);
            for (var i = 0; i < stageInfos.Count; i++)
            {
                var currentStageType = (StageType) i;
                if (stageType == currentStageType)
                {
                    stageInfos[i].score = score;
                }
            }
        }

        #endregion
    }

    [Serializable]
    public class StageScoreInfo
    {
        #region Public Variables

        public StageType stageType;
        public int score;

        #endregion
    }
}
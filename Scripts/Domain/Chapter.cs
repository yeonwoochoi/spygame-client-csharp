using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Domain
{
    /// <summary>
    /// Init loading에서 불러와야한다.
    /// 모든 chapter info를 불러오는 것이기 때문
    /// </summary>
    [Serializable]
    public class ChapterManager
    {
        #region Public Variable
        
        [SerializeField] public List<ChapterInfo> chapterInfos;

        #endregion

        #region Static Variables

        private static ChapterManager instance = null;
        public static ChapterManager Instance => instance ?? (instance = new ChapterManager());

        #endregion
        
        #region Constructor
        private ChapterManager() {}

        #endregion

        #region Getter
        
        public ChapterInfo GetChapterInfo(ChapterType chapterType)
        {
            ChapterInfo result = null;
            if (chapterInfos == null || chapterInfos.Count == 0) return result;
            foreach (var chapterInfo in chapterInfos
                .Where(chapterInfo => chapterInfo.chapterType == chapterType))
            {
                result = chapterInfo;
            }

            return result;
        }

        public StageInfo GetStageInfo(ChapterType chapterType, StageType stageType)
        {
            StageInfo result = null;
            if (chapterInfos == null || chapterInfos.Count == 0) return result;
            foreach (var chapterInfo in chapterInfos
                .Where(chapterInfo => chapterInfo.chapterType == chapterType))
            {
                if (chapterInfo.stageInfos == null || chapterInfo.stageInfos.Count == 0)
                    return result;
                foreach (var stageInfo in chapterInfo.stageInfos
                    .Where(stageInfo => stageInfo.stageType == stageType))
                {
                    result = stageInfo;
                }
            }

            return result;
        }

        public bool IsSet()
        {
            return !(chapterInfos == null || chapterInfos.Count == 0);
        }

        #endregion

        #region Setter

        public void UpdateStageScore(ChapterType chapterType, StageType stageType, int score)
        {
            foreach (var chapterInfo in chapterInfos.Where(chapterInfo => chapterInfo.chapterType == chapterType))
            {
                foreach (var stageInfo in chapterInfo.stageInfos.Where(stageInfo => stageInfo.stageType == stageType))
                {
                    stageInfo.score = score;
                }
            }
        }

        #endregion

        #region Public Method

        public void SetUp(List<ChapterInfo> chapter)
        {
            chapterInfos = chapter;
        }

        #endregion

        
    }

    [Serializable]
    public class ChapterInfo
    {
        #region Public Variables

        [SerializeField] public string title;
        
        [JsonConverter(typeof(StringEnumConverter))]
        [SerializeField] public ChapterType chapterType;
        
        [SerializeField] public List<StageInfo> stageInfos;

        #endregion
    }

    [Serializable]
    public class StageInfo
    {
        #region Public Variables

        [JsonConverter(typeof(StringEnumConverter))]
        [SerializeField] public StageType stageType;
        
        [SerializeField] public int limitTime;
        [SerializeField] public int boxCount;
        [SerializeField] public int normalSpyCount;
        [SerializeField] public int bossSpyCount;
        [SerializeField] public int goalNormalSpyCount;
        [SerializeField] public int goalBossSpyCount;
        [SerializeField] public int score;

        #endregion

        #region Getter

        public string GetStageMissionText()
        {
            return $"Catch {goalNormalSpyCount} normal spy and {goalBossSpyCount} boss spies through interrogation.";
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Domain
{
    /// <summary>
    /// Init loading에서 불러와야한다.
    /// 모든 chapter info를 불러오는 것이기 때문
    /// </summary>
    [Serializable]
    public class PseudoChapter
    {
        #region Public Variable
        
        [SerializeField] public List<PseudoChapterInfo> chapterInfos;

        #endregion

        #region Static Variables

        private static PseudoChapter instance = null;
        public static PseudoChapter Instance => instance ?? (instance = new PseudoChapter());

        #endregion
        
        #region Constructor
        private PseudoChapter() {}

        #endregion

        #region Getter
        
        public PseudoChapterInfo GetChapterInfo(ChapterType chapterType)
        {
            PseudoChapterInfo result = null;
            if (chapterInfos == null || chapterInfos.Count == 0) return result;
            foreach (var chapterInfo in chapterInfos
                .Where(chapterInfo => chapterInfo.chapterType == chapterType))
            {
                result = chapterInfo;
            }

            return result;
        }

        public PseudoStageInfo GetStageInfo(ChapterType chapterType, StageType stageType)
        {
            PseudoStageInfo result = null;
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

        #endregion

        #region Public Method

        public void SetUp(List<PseudoChapterInfo> chapter)
        {
            chapterInfos = chapter;
        }

        #endregion

        
    }

    [Serializable]
    public class PseudoChapterInfo
    {
        #region Public Variables

        [SerializeField] public string title;
        
        [JsonConverter(typeof(StringEnumConverter))]
        [SerializeField] public ChapterType chapterType;
        
        [SerializeField] public List<PseudoStageInfo> stageInfos;

        #endregion
    }

    [Serializable]
    public class PseudoStageInfo
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
    }
}